using System;
using System.Collections.Generic;
using System.Text;
using SHCre.Xugd.Common;
using System.Xml.Serialization;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace SHCre.Xugd.CFile
{
    /// <summary>
    /// 一个简单的记录类，日志类型限定为LogType，只记录字符串与异常
    /// </summary>
    public class XLogSimple
    {
        XLogFile _fLog = null;
        XLogConfig _logConfig = null;

        /// <summary>
        /// 
        /// </summary>
        public XLogSimple(){}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logConfig_"></param>
        public XLogSimple(XLogConfig logConfig_)
        {
            InitLogFile(logConfig_);
        }

        /// <summary>
        /// 为兼容旧配置，不推荐使用
        /// </summary>
        /// <param name="logConfig_"></param>
        public XLogSimple(XLogFile.Config logConfig_)
        {
            XLogConfig con = null;
            if (logConfig_ != null)
            {
                con = new XLogConfig()
                    {
                        NameFormat = logConfig_.NameFormat,
                        WithTime = logConfig_.WithTime,
                        WithThreadId = logConfig_.WithThreadId,
                        LogLevel = (LogLevels)logConfig_.LogFlags,
                    };
            }
            InitLogFile(con);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strNameFormat_"></param>
        public XLogSimple(string strNameFormat_)
        {
            SetLogNameFormat(strNameFormat_);
        }

        /// <summary>
        /// 设定日志记录的等级
        /// </summary>
        public LogLevels LogLevel 
        {
            get 
            {
                if (_logConfig == null)
                    return LogLevels.None;
                return _logConfig.LogLevel;
            }

            set 
            {
                if (_logConfig != null)
                    _logConfig.LogLevel = value;
                if (_fLog != null)
                {
                    _fLog.LogFlags = (uint)value;
                    Notice("LogLevel changed to {0}", value);
                }
            }
        }

        /// <summary>
        /// 获取日志文件名
        /// </summary>
        public string LogNameFormat
        {
            get
            {
                if (_logConfig == null)
                    return string.Empty;

                return _logConfig.NameFormat;
            }
        }

        /// <summary>
        /// 设定日志文件名
        /// </summary>
        /// <param name="strNameForamt_"></param>
        public void SetLogNameFormat(string strNameForamt_)
        {
            if (_fLog != null)
            {
                _fLog.FileNameFormat = strNameForamt_;
                if (_logConfig != null)
                    _logConfig.NameFormat = strNameForamt_;
            }
            else
            {
                if (!string.IsNullOrEmpty(strNameForamt_))
                {
                    var conTemp = _logConfig;
                    if (conTemp == null)
                    {
                        conTemp = new XLogConfig()
                        {  
                            WithTime = true,
                            LogLevel = LogLevels.Info,
                        };
                    }

                    conTemp.NameFormat = strNameForamt_;
                    InitLogFile(conTemp);
                }
            }
        }

        /// <summary>
        /// 重设配置信息
        /// </summary>
        /// <param name="conFile_"></param>
        public void ResetConfig(XLogConfig conFile_)
        {
            if (conFile_ == null) return;
            InitLogFile(conFile_);
        }

        /// <summary>
        /// 获取当前调试源码信息（FileName-Method[Line]）
        /// </summary>
        /// <param name="bFullPath_">全路径还是只有文件名</param>
        /// <param name="nLevel_">层次，当前函数位置则为0；父函数为1，一次类推</param>
        /// <returns></returns>
        public static string GetDebugSrcInfo(int nLevel_=0, bool bFullPath_=false)
        {
            if (nLevel_ < 0) nLevel_ = 0;
            StackTrace stParent = new StackTrace(nLevel_ + 1, true);
            var frame = stParent.GetFrame(0);
            if (frame == null) return string.Empty;

            return string.Format("{0}-{1}[{2}]",
                bFullPath_ ? frame.GetFileName() : Path.GetFileName(frame.GetFileName()),
                frame.GetMethod().Name,
                frame.GetFileLineNumber());
        }

        void InitLogFile(XLogConfig conFile_)
        {
            _logConfig = conFile_;

            if (_logConfig != null)
            {
                if (_fLog != null)
                {
                    _fLog.Close();
                    _fLog = null;
                }

                var logFile = new XLogFile(_logConfig.NameFormat, _logConfig.LogLevel, _logConfig.WithTime, _logConfig.WithThreadId);
                //logFile.WriteLine();
                _fLog = logFile;
            }
        }

        /// <summary>
        /// 关闭日志
        /// </summary>
        public void Close()
        {
            if (_fLog != null)
            {
                _fLog.Close();
                _fLog = null;
            }
        }

        /// <summary>
        /// 写入一个空行
        /// </summary>
        public void WriteLine()
        {
            if (_fLog != null)
                _fLog.WriteLine();
        }

        /// <summary>
        /// 写字符串日志
        /// </summary>
        /// <param name="strLog_"></param>
        /// <param name="logType_"></param>
        public void WriteLine(string strLog_, LogLevels logType_ = LogLevels.Info)
        {
            WriteLine(logType_, strLog_);
        }

        /// <summary>
        /// 根据格式写日志
        /// </summary>
        /// <param name="logType_"></param>
        /// <param name="strFormat_"></param>
        /// <param name="oParams_"></param>
        public void WriteLine(LogLevels logType_, string strFormat_, params object[] oParams_)
        {
            if (_fLog != null && _fLog.IsLevelNeedLog((uint)logType_))
            {
                _fLog.WriteLogDirectly(logType_, strFormat_, oParams_);
            }
        }

        /// <summary>
        /// 输出简单信息（Information）
        /// </summary>
        /// <param name="strInfo_"></param>
        public void Print(string strInfo_)
        {
            WriteLine(LogLevels.Info, strInfo_);
        }

        /// <summary>
        /// 记录版本信息：
        /// ----Name: {Name}, Version: {Version}----
        /// </summary>
        /// <param name="strName_"></param>
        /// <param name="strVersion_"></param>
        public void VerInfo(string strName_, string strVersion_)
        {
            WriteLine(LogLevels.Info, "----Name: {0}, Version: {1}----", strName_, strVersion_);
        }

        /// <summary>
        /// 输出调用者所在程序集的名称（包括程序集版本AssemblyVersion、CPU架构）与文件版本信息(assembly: AssemblyFileVersion)
        /// </summary>
        public void VerInfo()
        {
            VerInfo(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// 输出程序集的名称（包括程序集版本AssemblyVersion、CPU架构）与文件版本信息(assembly: AssemblyFileVersion)
        /// </summary>
        /// <param name="tVer_">指定类型（类中可通过this.GetType()获取当前类的类型）</param>
        public void VerInfo(Type tVer_)
        {
            VerInfo(Assembly.GetAssembly(tVer_));
        }

        /// <summary>
        /// 输出程序集的名称（包括程序集版本AssemblyVersion、CPU架构）与文件版本信息(assembly: AssemblyFileVersion)
        /// </summary>
        /// <param name="assFile_"></param>
        public void VerInfo(Assembly assFile_)
        {
            if (_fLog == null)
                return;

            try
            {
                string strFileVer = "";
                var assFileVer = assFile_.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
                var assName = assFile_.GetName();
                if (assFileVer != null && assFileVer.Length > 0)
                    strFileVer = (assFileVer[0] as AssemblyFileVersionAttribute).Version;
                WriteLine(LogLevels.Info, "----Assembly: {0}-{1}({2}) on .NET {3}, FileVersion: {4}----", 
                    assName.Name, assName.Version, assName.ProcessorArchitecture, assFile_.ImageRuntimeVersion, strFileVer);
            }
            catch(Exception ex)
            {
                WriteLine(LogLevels.Info, "Get version from assembly fail: {0}", ex);
            }
        }

        /// <summary>
        /// 写一行信息，行首添加"#"
        /// </summary>
        /// <param name="strFormat_"></param>
        /// <param name="oParams_"></param>
        public void Info(string strFormat_, params object[] oParams_)
        {
            WriteLine(LogLevels.Info, strFormat_, oParams_);
        }

        /// <summary>
        /// 函数被调用，行首添加"#"
        /// </summary>
        /// <param name="strFormat_"></param>
        /// <param name="oParams_"></param>
        public void Called(string strFormat_, params object[] oParams_)
        {
            WriteLine(LogLevels.Info, "#" + strFormat_, oParams_);
        }

        /// <summary>
        /// 发送数据日志，行首添加"&gt;"
        /// </summary>
        /// <param name="strFormat_"></param>
        /// <param name="oParams_"></param>
        public void Send(string strFormat_, params object[] oParams_)
        {
            WriteLine(LogLevels.Info, ">" + strFormat_, oParams_);
        }

        /// <summary>
        /// 接收数据日志，行首添加"&lt;"
        /// </summary>
        /// <param name="strFormat_"></param>
        /// <param name="oParams_"></param>
        public void Receive(string strFormat_, params object[] oParams_)
        {
            WriteLine(LogLevels.Info, "<" + strFormat_, oParams_);
        }

        /// <summary>
        /// 事件日志，行首添加"*"
        /// </summary>
        /// <param name="strFormat_"></param>
        /// <param name="oParams_"></param>
        public void Event(string strFormat_, params object[] oParams_)
        {
            WriteLine(LogLevels.Info, "*" + strFormat_, oParams_);
        }

        /// <summary>
        /// 写一行错误错误
        /// </summary>
        /// <param name="strFormat_"></param>
        /// <param name="oParams_"></param>
        public void Error(string strFormat_, params object[] oParams_)
        {
            WriteLine(LogLevels.Error, strFormat_, oParams_);
        }

        /// <summary>
        /// 写一行重要信息
        /// </summary>
        /// <param name="strFormat_"></param>
        /// <param name="oParams_"></param>
        public void Notice(string strFormat_, params object[] oParams_)
        {
            WriteLine(LogLevels.Notice, strFormat_, oParams_);
        }

        /// <summary>
        /// 写一行警告
        /// </summary>
        /// <param name="strFormat_"></param>
        /// <param name="oParams_"></param>
        public void Warn(string strFormat_, params object[] oParams_)
        {
            WriteLine(LogLevels.Warn, strFormat_, oParams_);
        }

        /// <summary>
        /// 写一行调试信息
        /// </summary>
        /// <param name="strFormat_"></param>
        /// <param name="oParams_"></param>
        public void Debug(string strFormat_, params object[] oParams_)
        {
            WriteLine(LogLevels.Debug, strFormat_, oParams_);
        }

        /// <summary>
        /// 记录异常，行首添加"!$"
        /// </summary>
        /// <param name="exError"></param>
        /// <param name="strInfo_"></param>
        public void Except(Exception exError, string strInfo_)
        {
            if (_fLog == null)
                return;

            string strMsg;
            var exBase = exError as XBaseException;
            if (exBase == null)
                strMsg = string.Format("{0}({1})", XReflex.GetTypeName(exError, false), exError.Message);
            else
                strMsg = exBase.GetMessage();

            if (exError.TargetSite == null)
                WriteLine(LogLevels.Error, "!${0} fail: {1}", strInfo_, strMsg);
            else
                WriteLine(LogLevels.Error, "!${0} fail at {1} in {2}: {3}\n {4}", 
                    strInfo_, exError.TargetSite, exError.Source, strMsg, exError.StackTrace);
        }

        /// <summary>
        /// 记录异常日志（输出异常的名称与消息Message），行首添加"!$"
        /// </summary>
        /// <param name="exError"></param>
        /// <param name="strFormat_"></param>
        /// <param name="oParams_"></param>
        public void WriteLine(Exception exError, string strFormat_, params object[] oParams_)
        {
            Except(exError, string.Format(strFormat_, oParams_));
        }

        /// <summary>
        /// 显示日志类型字符串
        /// </summary>
        /// <returns></returns>
        public static string PrintLogType()
        {
            return string.Format("None=0, Information=0x01, Notice=0x02, Warn=0x04, Error=0x08, Debug=0x10");
        }

        /// <summary>
        /// 日志等级
        /// </summary>
        public enum LogLevels
        {
            /// <summary>
            /// 不记录
            /// </summary>
            None = 0,
            /// <summary>
            /// 严重错误
            /// </summary>
            Crit,
            /// <summary>
            /// 错误
            /// </summary>
            Error,
            /// <summary>
            /// 警告
            /// </summary>
            Warn,
            /// <summary>
            /// 重要提示
            /// </summary>
            Notice,
            /// <summary>
            /// 信息
            /// </summary>
            Info,

            /// <summary>
            /// 调试用
            /// </summary>
            Debug,
        }
    }
}
