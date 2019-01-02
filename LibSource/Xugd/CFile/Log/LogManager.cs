using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Text;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.CFile
{
    partial class XLogFile
    {
        private StreamWriter _fWriter;
        private string _strLastErr = string.Empty;

        private readonly object _lkerWriteFile = new object();
        AutoResetEvent _evtToWrite = new AutoResetEvent(false);
        Thread _thrToWrite = null;

        private object _oQueueLocker = new object();
        Queue<string> _quWriteLogs = new Queue<string>();

        string _strProcName = null;
        string GetCurrentProcName()
        {
            if(_strProcName == null)
            {                
                using (var curProc = Process.GetCurrentProcess())
                {
                    _strProcName = string.Format("{0}({1})",
                        curProc.MainModule.FileName, curProc.Id);
                }
            }

            return _strProcName;
        }

        private bool CheckAndOpenFile()
        {
            if(!_bNeedReopen && _fWriter!=null) return true;

            lock (_lkerWriteFile)
            {
                if (_bNeedReopen)
                {
                    CloseFile();
                    _bNeedReopen = false;
                }
                if (_fWriter == null)
                {
                    OpenFile(XPath.GetFullPath(_strCurFileName));
                }
                return (_fWriter != null);
            }
        }

        private void OpenFile(string strFile_)
        {
            try
            {
                XPath.CreateFullPath(Path.GetDirectoryName(strFile_));

                _fWriter = new StreamWriter(strFile_, true, Encoding.UTF8);
                _fWriter.AutoFlush = true;

                string strInfo = string.Format("{0}#### Open by {1} ####",
                (_fWriter.BaseStream.Position > 0 ? XString.NewLine : string.Empty),
                GetCurrentProcName());
                _fWriter.WriteLine(strInfo);
            }
            catch(Exception ex){
                string strErr = string.Format("Open {0} failed: {1}\n", strFile_, ex);
                LogErr2File(strErr);
            }
        }

        private void LogErr2File(string strErr)
        {
            try
            {
                if (strErr != _strLastErr)
                {
                    _strLastErr = strErr;
                    File.AppendAllText(XPath.GetFullPath("xugd.logfile.err"), 
                        XTime.GetFullString(DateTime.Now) + ": " + strErr);
                }
            }
            catch{}
        }

        private void CloseFile()
        {
            lock (_lkerWriteFile)
            {
                if (this._fWriter != null)
                {
                    this._fWriter.Close();
                    this._fWriter = null;
                }
            }
        }

        //private void AddWriteLog(byte[] byLog_)
        //{
        //    lock (_oQueueLocker)
        //    {
        //        _quWriteLogs.Enqueue(new LogInfo2Write(byLog_));
        //        _evtToWrite.Set();

        //        XThread.TryStartThread(ref _thrToWrite, WriteLogThread);
        //    }
        //}

        private void AddWriteLog(string strLog_)
        {
            lock (_oQueueLocker)
            {
                _quWriteLogs.Enqueue(strLog_);
                _evtToWrite.Set();

                XThread.TryStartThread(ref _thrToWrite, WriteLogThread);
            }
        }

        DateTime _dtLastShrink = DateTime.Now;
        StringBuilder _sbLogFormat = new StringBuilder(1024);
        internal void WriteLogDirectly(XLogSimple.LogLevels logType_, string strFormat_, params object[] oArgs_)
        {
            lock (_oQueueLocker)
            {
                _sbLogFormat.Clear();

                // Add header
                if (WithTime)
                    _sbLogFormat.Append(XTime.GetFullString(DateTime.Now, true));
                if (WithThreadId)
                {   // (id)
                    _sbLogFormat.Append("(");
                    _sbLogFormat.Append(Thread.CurrentThread.ManagedThreadId);
                    _sbLogFormat.Append(")");
                }
                // Type: [type]
                _sbLogFormat.Append("[");
                _sbLogFormat.Append(logType_);
                _sbLogFormat.Append("]:\t");

                // Log to write
                if (oArgs_.Length == 0)
                    _sbLogFormat.Append(strFormat_);
                else
                    _sbLogFormat.AppendFormat(strFormat_, oArgs_);
                _sbLogFormat.Append(XString.NewLine);

                _quWriteLogs.Enqueue(_sbLogFormat.ToString());
                _evtToWrite.Set();

                if(_sbLogFormat.Capacity > 1024*8) // 8K
                {
                    if ((DateTime.Now - _dtLastShrink).TotalMinutes > 5)
                    {
                        _dtLastShrink = DateTime.Now;
                        _quWriteLogs.Enqueue(string.Format("#!#!LogFormat builder capacity is {0}, length is {1}{2}", _sbLogFormat.Capacity, _sbLogFormat.Length, XString.NewLine));
                        _sbLogFormat.Capacity = Math.Max(1024, _sbLogFormat.Length + 16);
                    }
                }

                XThread.TryStartThread(ref _thrToWrite, WriteLogThread);
            }
        }

        private void WriteLogThread()
        {
            while(_bStartLog)
            {
                try
                {
                    string strToWrite = null;
                    _evtToWrite.WaitOne();
                    while (true)
                    {
                        lock (_oQueueLocker)
                        {
                            if (_quWriteLogs.Count == 0)
                                break;

                            strToWrite = _quWriteLogs.Dequeue();
                        }

                        // write now
                        WriteLog2File(strToWrite);
                    }
                }
                catch(Exception ex)
                {
                    string strErr = string.Format("WriteLogThread failed: {0}\n", ex.Message);
                    LogErr2File(strErr);
                }
            }

            CloseFile();
            _thrToWrite = null;
        }

        void WriteLog2File(string strLog_)
        {
            if(CheckAndOpenFile())
            {
                _fWriter.Write(strLog_);
            }
        }

        //class LogInfo2Write
        //{
        //    public LogInfo2Write(object oLog_)
        //    {
        //        Log = oLog_;
        //    }
        //    public object Log { get; set; }
        //}
    }
}
