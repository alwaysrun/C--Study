using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace SHCre.Xugd.Common
{
    /// <summary>
    /// 程序相关操作
    /// </summary>
    public static class XProcess
    {
        private static void Start(string strStartPath_, string strStartCmd_, int nWaitSeconds_, bool bErrDialog_)
        {
            string strFile = Path.Combine(strStartPath_, "XRestart.bat");
            if (nWaitSeconds_ <= 0)
                nWaitSeconds_ = 1;
            string strBat = string.Format("ping 127.0.0.1 -n {0}>nul{1}", nWaitSeconds_, XString.NewLine) // Use ping to wait(assume 1 sec each request)
                + strStartCmd_; 
            File.WriteAllText(strFile, strBat);

            // Exec the bat in hidden
            ProcessStartInfo procInfo = new ProcessStartInfo(strFile)
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                ErrorDialog = bErrDialog_,
            };
            Process.Start(procInfo);
        }

        /// <summary>
        /// 运行Cmd命令（通过创建bat文件并调用执行的方式实现）：
        /// 运行带空格程序(start "" "File with blank.exe")
        /// </summary>
        /// <param name="strCmd_">要执行的命令</param>
        /// <param name="strCmdFile_">存放Cmd命令的文件（默认在当前运行程序的目录下创建一个XExecuteCmd.bat文件）</param>
        /// <param name="bErrDialog_">不能启动进程时，是否向用户显示对话框</param>
        public static void ExecCmdViaBat(string strCmd_, string strCmdFile_=null, bool bErrDialog_=false)
        {
            if (string.IsNullOrEmpty(strCmdFile_))
            {
                strCmdFile_ = Path.Combine(Application.StartupPath, "XExecuteCmd.bat");
            }
            else
            {
                if (!strCmdFile_.EndsWith(".bat", StringComparison.OrdinalIgnoreCase))
                    strCmdFile_ += ".bat";
            }
            File.WriteAllText(strCmdFile_, strCmd_);

            // Exec the bat in hidden
            ProcessStartInfo procInfo = new ProcessStartInfo(strCmdFile_)
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                ErrorDialog = bErrDialog_,
            };
            Process.Start(procInfo);
        }

        ///// <summary>
        ///// 通过C#的Process执行批处理命令：
        ///// 运行带空格可执行程序要加引号("File with blank.exe")
        ///// </summary>
        ///// <param name="strCmd_"></param>
        ///// <param name="nWaitSeconds_"></param>
        ///// <returns>执行的命令与错误信息</returns>
        //public static string ExecCommand(string strCmd_, int nWaitSeconds_ = 5)
        //{
        //    return ExecCommand(nWaitSeconds_, strCmd_);
        //}

        /// <summary>
        /// 通过C#的Process执行批处理命令（命令执行完成后等待cmd退出最长5秒钟）：
        /// 运行带空格可执行程序要加引号("File with blank.exe")
        /// </summary>
        /// <param name="aryCmds_"></param>
        /// <returns>执行的命令与错误信息（-ERR: ***)</returns>
        public static string ExecCommand(params string[] aryCmds_)
        {
            return ExecCommand(5, aryCmds_);
        }

        /// <summary>
        /// 通过C#的Process执行批处理命令：
        /// 运行带空格可执行程序要加引号("File with blank.exe")
        /// </summary>
        /// <param name="nWaitSeconds_">命令执行完成后等待cmd退出的最长时间</param>
        /// <param name="aryCmds_">要执行的命令</param>
        /// <returns>执行的命令或错误信息（-ERR: ***)</returns>
        public static string ExecCommand(int nWaitSeconds_, params string[] aryCmds_)
        {
            const string ErrPrefix = "-ERR: ";
            if (aryCmds_.Length == 0)
                return ErrPrefix + "No command";

            Process proCmd = new Process();
            proCmd.StartInfo.FileName = "cmd.exe";
            proCmd.StartInfo.UseShellExecute = false;
            proCmd.StartInfo.RedirectStandardInput = true;
            proCmd.StartInfo.RedirectStandardOutput = true;
            proCmd.StartInfo.RedirectStandardError = true;
            proCmd.StartInfo.CreateNoWindow = true;
            proCmd.StartInfo.ErrorDialog = false;

            // Write execute command
            string strOutput = string.Empty;
            string strError = string.Empty;
            proCmd.ErrorDataReceived += (osender, errData) => { strError += errData.Data; };
            try
            {
                if (proCmd.Start())
                {
                    foreach (var cmd in aryCmds_)
                    {
                        proCmd.StandardInput.WriteLine(cmd);
                        strOutput += cmd + XString.NewLine;
                    }
                    proCmd.StandardInput.WriteLine("exit");

                    proCmd.BeginErrorReadLine();

                    // 不能同步获取，会产生死锁，一直等待执行程序退出后才能兑取到数据
                    //while (!proCmd.StandardError.EndOfStream)
                    //{
                    //    strError += proCmd.StandardError.ReadLine();
                    //}


                    // wait
                    if (nWaitSeconds_ <= 0)
                        nWaitSeconds_ = 5;
                    proCmd.WaitForExit(XTime.Second2Interval(nWaitSeconds_));
                    if (strError.Length > 0)
                    {
                        strOutput = ErrPrefix + strError;
                    }
                    else
                    {
                        strOutput = string.Format("CMD: {0}", string.Join("; ", aryCmds_));
                    }

                    proCmd.Close();
                }
                else
                {
                    strOutput = ErrPrefix + "Start process failed";
                }
            }
            catch (Exception ex)
            {
                strOutput = string.Format("{0}{1}({2})", ErrPrefix,  
                    XReflex.GetTypeName(ex, false), ex.Message);
            }

            return strOutput;
        }

        /// <summary>
        /// 通过批处理重启电脑
        /// </summary>
        /// <param name="strComment_">重启的原因</param>
        /// <param name="nWaitSecond_">重启前等待的时间</param>
        public static void RestartOS(string strComment_=null, int nWaitSecond_=5)
        {
            try 
            {
                if (string.IsNullOrEmpty(strComment_))
                    strComment_ = "Restart By XProcess.RestartOS at:" + DateTime.Now;
                if (nWaitSecond_ <= 0)
                    nWaitSecond_ = 5;
                string strCmd = string.Format("shutdown /r /f /t {0} /c \"{1}\"", nWaitSecond_, strComment_);
                ExecCmdViaBat(strCmd, Path.Combine(Application.StartupPath, "XRestartCmd.bat"));
            }
            catch{}
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="strStartPath_">启动服务程序的运行目录</param>
        /// <param name="strSrvName_">服务名（如果有启动参数，则包含参数）</param>
        /// <param name="nWaitSeconds_">在服务启动前等待的时间（秒数）</param>
        /// <param name="bErrDialog_">不能启动进程时，是否向用户显示对话框</param>
        public static void StartService(string strStartPath_, string strSrvName_, int nWaitSeconds_=1, bool bErrDialog_=false)
        {
            Start(strStartPath_, string.Format("net start {0}", strSrvName_), nWaitSeconds_, bErrDialog_);
        }

        /// <summary>
        /// 启动程序
        /// </summary>
        /// <param name="strFullName_">程序名（包括完整路径）</param>
        /// <param name="nWaitSeconds_">在启动前等待的时间（秒数）</param>
        /// <param name="bErrDialog_">不能启动进程时，是否向用户显示对话框</param>
        public static void StartProgram(string strFullName_, int nWaitSeconds_ = 1, bool bErrDialog_ = true)
        {
            Start(Path.GetDirectoryName(strFullName_), strFullName_, nWaitSeconds_, bErrDialog_);
        }

        /// <summary>
        /// 保证程序退出，如果超过指定时间（秒数）没有退出，则强制Kill掉
        /// </summary>
        /// <param name="strStartPath_"></param>
        /// <param name="nWaitSeconds_"></param>
        /// <param name="strCmdFile_">存放临时CMD命令的文件名（默认XTryExit.bat）</param>
        public static void ExitEnsure(string strStartPath_, int nWaitSeconds_=10, string strCmdFile_=null)
        {
            if (string.IsNullOrEmpty(strCmdFile_))
            {
                strCmdFile_ = "XTryExit.bat";
            }
            else
            {
                if (!strCmdFile_.EndsWith(".bat", StringComparison.OrdinalIgnoreCase))
                    strCmdFile_ += ".bat";
            }
            string strFile = Path.Combine(strStartPath_, strCmdFile_);
            if (nWaitSeconds_ <= 0)
                nWaitSeconds_ = 1;

            var curProc = Process.GetCurrentProcess();
            string strCmd = string.Format("taskkill /F /PID {0}", curProc.Id); // Force kill the process
            string strBat = string.Format("ping 127.0.0.1 -n {0}>nul{1}", nWaitSeconds_, XString.NewLine) // Use ping to wait(assume 1 sec each request)
                + strCmd;
            File.WriteAllText(strFile, strBat);

            // Exec the bat in hidden
            ProcessStartInfo procInfo = new ProcessStartInfo(strFile)
            {
                WindowStyle = ProcessWindowStyle.Hidden,
            };
            Process.Start(procInfo);
        }

        /// <summary>
        /// 判断是否已启动，如果启动直接退出
        /// </summary>
        /// <param name="strEvent_">用于窗体互斥的事件名称</param>
        /// <returns>true，已存在，自动退出；false，不存在，启动一个新的</returns>
        public static bool IsExists(string strEvent_)
        {
            EventWaitHandle eHandle;
            try
            {
                eHandle = EventWaitHandle.OpenExisting(strEvent_);
                // Form has start, close it
                eHandle.Close();
                return true;
            }
            catch (WaitHandleCannotBeOpenedException)
            {
            }

            eHandle = new EventWaitHandle(false, EventResetMode.ManualReset, strEvent_);
            Thread thrRestore = new Thread(new ParameterizedThreadStart(WaitOnly));
            thrRestore.IsBackground = true;
            thrRestore.Start(eHandle);

            return false;
        }
        
        /// <summary>
        /// 只是防止事件释放而用的空线程
        /// </summary>
        /// <param name="oParam_">事件句柄</param>
        static void WaitOnly(object oParam_)
        {
            try
            {
                EventWaitHandle eHandle = (EventWaitHandle)oParam_;
                while (true)
                {
                    eHandle.WaitOne();
                    eHandle.Reset();
                }
            }
            catch { }
        }

        /// <summary>
        /// 根据文件名获取互斥体名
        /// </summary>
        /// <param name="strFileName_"></param>
        /// <param name="strType_"></param>
        /// <returns></returns>
        public static string GetSynNameFromFileName(string strFileName_, string strType_="Event")
        {
            return string.Format(@"Syn:_SHCre_Xugd_{0}.{1}", XConvert.Uint2HexString((uint)strFileName_.GetHashCode()), strType_);
        }

        const int WaitSecondsBeforeProcExit = 1000 * 30;
        /// <summary>
        /// 杀死所有指定的进程：全部杀掉返回true；
        /// 如果有进程超时未退出则返回false（每个进程等待30秒的退出时间）
        /// </summary>
        /// <param name="strProcName_">进程名</param>
        /// <returns></returns>
        public static bool KillAll(string strProcName_)
        {
            bool bKilled = true;
            var allProc = Process.GetProcessesByName(strProcName_);
            foreach(var pr in allProc)
            {
                pr.Kill();
                if (!pr.WaitForExit(WaitSecondsBeforeProcExit))
                    bKilled = false;
                
                pr.Dispose();
            }

            return bKilled;
        }

        /// <summary>
        /// 杀死所有与当前进程名相同的进程，但保留当前进程：全部杀掉返回true；
        /// 如果有进程超时未退出则返回false（每个进程等待30秒的退出时间）
        /// </summary>
        /// <returns></returns>
        public static bool KillAllButThis()
        {
            bool bKilled = true;
            using (var curProc = Process.GetCurrentProcess())
            {
                var allProc = Process.GetProcessesByName(curProc.ProcessName);
                foreach (var pr in allProc)
                {
                    if (pr.Id == curProc.Id)
                        continue;

                    pr.Kill();
                    if (!pr.WaitForExit(WaitSecondsBeforeProcExit))
                        bKilled = false;

                    pr.Dispose();
                }
            }

            return bKilled;
        }
    }
}
