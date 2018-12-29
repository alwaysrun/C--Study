using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using SHCre.Xugd.CFile;
using SHCre.Xugd.Common;
using SHCre.Xugd.Extension;
using SHCre.Xugd.WinAPI;

namespace SHCre.Tools.ProcMonitor
{
    public partial class FrmMain : XProgram.ServiceForm //Form
    {
        ProcConfig _conMonitor;
        XLogSimple _logFile;
        bool _bIsRunning = true;
        uint _nCurSession = uint.MaxValue;
        ManualResetEvent _evtQuit = new ManualResetEvent(false);

        public FrmMain()
            : base()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            _conMonitor = ProcConfig.Read();

            _logFile = new XLogSimple(_conMonitor.LogConfig);
            _logFile.VerInfo();
            try
            {
                _nCurSession = XWin.GetCurrentSessionId();
                _logFile.Info("Current Session is: {0}", _nCurSession);
            }
            catch(Exception ex)
            {
                _logFile.Except(ex, "GetCurrentSessionId");
            }
            //_logFile.Info("Form show:{0}, Hide:{1}, TrayHide:{2}", this.Visible, this.IsHide, this.IsHideTrayIcon);

            _bIsRunning = true;
            ToMonitor(_conMonitor.ToMonitors);
            ToCheckMem(_conMonitor.ToCheckMems);
        }

        #region "Check Memory"
        private void ToCheckMem(List<MemCheckConfig> lstProcess_)
        {
            if (lstProcess_ == null || lstProcess_.Count == 0) return;

            _logFile.Info("To CheckMem {0} Processes:", lstProcess_.Count(z => !string.IsNullOrEmpty(z.ProcName)));
            foreach (var proc in lstProcess_)
            {
                if (string.IsNullOrEmpty(proc.ProcName))
                    continue;

                _logFile.Info("\tTo Start Check {0}", proc.ProcName);
                XThread.StartThread(CheckMemThread, proc);
            }
        }

        private void CheckMemThread(object oParam_)
        {
            var chkConfig = oParam_ as MemCheckConfig;
            _logFile.Info("CheckMem {0} start: {1} MB", chkConfig.ProcName, chkConfig.MaxSizeInMB);

            try
            {
                var nClock = chkConfig.CheckAtClock;
                if (nClock < 0 || nClock >= 24) nClock = 2.5;   // Start at 2:30
                int nHour = (int)nClock;
                int nMinute = (int)((nClock - nHour) * 60);
                while (_bIsRunning)
                {
                    var dtNow = DateTime.Now;
                    var dtNextDay = dtNow;
                    if (dtNow.Hour >= nHour)
                        dtNextDay = dtNow.AddDays(1);
                    var dtCheck = new DateTime(dtNextDay.Year, dtNextDay.Month, dtNextDay.Day, nHour, nMinute, 0);
                    _logFile.Info("CheckMem {0} will check at {1}", chkConfig.ProcName, XTime.GetFullString(dtCheck));

                    var tsSpan = dtCheck - dtNow;
                    // var tsSpan = new TimeSpan(0, 5, 0); // debug: 5 minute
                    if (_evtQuit.WaitOne(tsSpan))
                        break;

                    CheckProcMemory(chkConfig);
                }
            }
            catch (Exception ex)
            {
                _logFile.Except(ex, "CheckMemThread");
            }

            _logFile.Info("CheckMem {0} stop", chkConfig.ProcName);
        }

        private void CheckProcMemory(MemCheckConfig chkConfig)
        {
            var proc = SearchProcess(chkConfig.ProcName, string.Empty);
            if (proc == null)
            {
                _logFile.Warn("CheckProcMemory({0}): Process not found", chkConfig.ProcName);
                return;
            }

            try 
            {
                _logFile.Info("CheckProcMemory({0}): WorkingSet is {1} KB", chkConfig.ProcName, proc.WorkingSet64/1024);
                if( proc.WorkingSet64/1024/1024 > chkConfig.MaxSizeInMB)
                {
                    _logFile.Warn("CheckProcMemory({0}): Memory too large, to kill now!", chkConfig.ProcName);
                    proc.Kill();
                }
                proc.Dispose();           
            }
            catch(Exception ex)
            {
                _logFile.Except(ex, "CheckProcMemory");
            }
        }
        #endregion

        private void ToMonitor(List<MonitorConfig> lstProces_)
        {
            if (lstProces_ == null || lstProces_.Count == 0)
            {
                _logFile.Warn("No process has set to Monitor");
                return;
            }

            _logFile.Info("To Monitor {0} Processes:", lstProces_.Count(z => !string.IsNullOrEmpty(z.ProcName)));
            foreach (var proc in lstProces_)
            {
                if (string.IsNullOrEmpty(proc.ProcName))
                    continue;

                _logFile.Info("\tTo Monitor {0}", proc.ProcName);
                XThread.StartThread(MonitorThread, proc);
            }
        }

        private void MonitorThread(object oParam_)
        {
            MonitorConfig prConfig = (MonitorConfig)oParam_;
            prConfig.ProcName = prConfig.ProcName.Trim();
            prConfig.File = prConfig.File.Trim();
            _logFile.Info("Monitor {0} Start", prConfig.ProcName);

            try
            {
                int nCycles = 0;
                if (prConfig.CheckIntervalMinutes <= 0)
                    prConfig.CheckIntervalMinutes = 1;
                int nCheckInterval = XTime.Minute2Interval(prConfig.CheckIntervalMinutes);
                if (prConfig.WaitCheckCountBeforeAsExit < 0)
                    prConfig.WaitCheckCountBeforeAsExit = 0;

                Thread.Sleep(100);
                while (_bIsRunning)
                {
                    Process proc = SearchProcess(prConfig.ProcName, prConfig.File);
                    if (proc != null)
                    {
                        nCycles = prConfig.WaitCheckCountBeforeAsExit;  // Once has found; if not find again, treat as exit.
                        prConfig.ProcFile = GetFileName(proc);

                        if (WaitProcessExit(proc, prConfig.File, nCheckInterval))
                        {
                            _logFile.Info("Process {0} exit", prConfig.ProcName);
                            Thread.Sleep(1000); // wait a second for process quit.
                            HandleExitAndRestart(prConfig);
                        }
                    } // Process not found
                    else
                    {
                        ++nCycles;
                        if (nCycles > prConfig.WaitCheckCountBeforeAsExit)
                        {//当做做退出处理
                            nCycles = prConfig.WaitCheckCountBeforeAsExit;

                            _logFile.Warn("Process {0} not found, treat as Exit", prConfig.ProcName);
                            HandleExitAndRestart(prConfig);
                        }
                        else
                        {
                            _logFile.Info("{0}: Process {1} not found", nCycles, prConfig.ProcName);
                        }
                    }

                    _evtQuit.WaitOne(nCheckInterval);
                }
            }
            catch (Exception ex)
            {
                _logFile.Info(ex.ToString());
            }

            _logFile.Info("Monitor {0} Stopped", prConfig.ProcName);
        }

        private Process SearchProcess(string strName_, string strFile_)
        {
            var aryProc = Process.GetProcessesByName(strName_);
            Process proFound = null;
            foreach (var proc in aryProc)
            {
                if (CheckFileName(proc, strFile_))
                {
                    _logFile.Info("Process {0}[{1}] is running", strName_, proc.Id);
                    proFound = proc;
                    continue;
                }

                proc.Dispose();
            }

            //_logFile.Info("Process {0} not found", strName_);
            return proFound;
        }

        private bool WaitProcessExit(Process proc_, string strFile_, int nInterval_)
        {
            try
            {
                Process prRunning = proc_;
                string strName = prRunning.ProcessName;
                nInterval_ *= 10;   
                while (_bIsRunning)
                {
                    if (prRunning.WaitForExit(nInterval_) || prRunning.HasExited)
                    {
                        prRunning.Dispose();
                        return true;
                    }

                    //prRunning.Dispose();
                    //prRunning = SearchProcess(strName, strFile_);
                    //if (prRunning == null)
                    //    return true;

                    _logFile.Info("Process {0} is running", strName);
                }
            }
            catch (Exception ex)
            {
                _logFile.Except(ex, "WaitProcessExit");
            }

            return _bIsRunning;
        }

        private void ExcuteRestartCmd(MonitorConfig procConfig_)
        {
            if (procConfig_.WaitSecondBeforeRestart > 0)
            {
                if (procConfig_.WaitSecondBeforeRestart > 30)
                    procConfig_.WaitSecondBeforeRestart = 30;

                Thread.Sleep(procConfig_.WaitSecondBeforeRestart * 1000);
            }

            string strCmd = procConfig_.RestartCmd;
            if (string.IsNullOrEmpty(strCmd))
            {
                strCmd = procConfig_.ProcFile;
                if (string.IsNullOrEmpty(strCmd))
                    strCmd = procConfig_.File;
            }

            bool bStartProc = procConfig_.ExitMode == ExitHandleMode.RestartProc;
            string strOut = string.Format("Process {0} Restart-", procConfig_.ProcName);
            strCmd = strCmd.Trim();
            if(bStartProc)
            {
                if (_nCurSession == 0)   // Serivce
                {
                    strOut += XWin.SrvCreateProcess(strCmd);
                }
                else
                {
                    Process.Start(strCmd);
                    strOut += "ProcStart " + strCmd;
                }
            }
            else
            {
                strOut += XProcess.ExecCommand(strCmd);
            }

            _logFile.Info(strOut);
        }

        private void HandleExitAndRestart(MonitorConfig procConfig_)
        {
            try
            {
                switch (procConfig_.ExitMode)
                {
                    case ExitHandleMode.RestartExe:
                    case ExitHandleMode.RestartProc:
                        {
                            ExcuteRestartCmd(procConfig_);
                        }
                        break;

                    case ExitHandleMode.RestartOS:
                        {
                            try
                            {
                                var startSpan = XWin.Windows.GetStartPeriod();
                                if (startSpan.TotalMinutes < _conMonitor.RestartOsOnlyAfterMinute)
                                {
                                    _logFile.Info(string.Format("Windows start only {0} min, Restart-Process {1} instead of Restart-OS", startSpan.TotalMinutes, procConfig_.ProcName));
                                    ExcuteRestartCmd(procConfig_);
                                }
                                else
                                {
                                    string strInfo = string.Format("Process {0} RestartOS", procConfig_.ProcName);
                                    _logFile.Info(strInfo);

                                    XProcess.RestartOS(strInfo, 15);
                                    XWin.Windows.Reboot();

                                    // Wait for OS exit
                                    Thread.Sleep(XTime.Minute2Interval(2));
                                }
                            }
                            catch (Exception ex)
                            {
                                _logFile.Info(ex.Message);
                            }
                        }
                        break;

                    default:
                        // _logFile.Info("Process {0} Exit monitor", procConfig_.ProcName);
                        break;
                }
            }
            catch(Exception ex)
            {
                _logFile.Except(ex, "HandleExitAndRestart");
            }
        }

        private bool CheckFileName(Process proc_, string strFile_)
        {
            // 只提供进程名，而没有提供具体的路径：
            // 都认为是要关心的进程
            if (string.IsNullOrEmpty(strFile_))
                return true;

            // 如果无法获取进程的进程名，则也认为是要关心的进程
            string strName = GetFileName(proc_);
            if (string.IsNullOrEmpty(strName))
                return true;

            return string.Compare(strFile_, strName, true) == 0;
        }

        private string GetFileName(Process proc_)
        {
            string strName = string.Empty;
            try
            {
                strName = proc_.MainModule.FileName;
            }
            catch { }

            return strName;
        }

        private void btnGetList_Click(object sender, EventArgs e)
        {
            lvwProcess.Items.Clear();

            int nIndex = 0;
            var aryProc = Process.GetProcesses();
            Array.Sort(aryProc, (x, y) => String.Compare(x.ProcessName, y.ProcessName));
            foreach (var proc in aryProc)
            {
                ListViewItem lvItem = new ListViewItem(string.Format("[{0}]{1}", ++nIndex, proc.Id));
                lvItem.SubItems.Add(proc.ProcessName);
                lvItem.SubItems.Add(GetFileName(proc));

                lvwProcess.Items.Add(lvItem);
                proc.Close();
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.CanExit) return;

            _bIsRunning = false;
            _evtQuit.Set();
        }
    } // class
}
