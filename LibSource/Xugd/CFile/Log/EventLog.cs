using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SHCre.Xugd.CFile
{
    /// <summary>
    /// 写事件日志的类
    /// </summary>
    public static class XEventLog
    {
        static EventLog _logEvent;

        private static string _strSrcName;
        /// <summary>
        /// 显示在事件查看器中来源的名字
        /// </summary>
        public static string SourceName
        {
            get 
            {
                if(string.IsNullOrEmpty(_strSrcName))
                {
                    _strSrcName = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
                }

                return _strSrcName;
            }
            set 
            {
                _strSrcName = value;
            }
        }

        private static string _strLogName;
        /// <summary>
        /// 事件日志名称：显示在事件查看器左侧树形分类中的，默认为‘XAppEvent’
        /// </summary>
        public static string LogName
        {
            get 
            {
                if (string.IsNullOrEmpty(_strLogName))
                    _strLogName = "XAppEvent";

                return _strLogName;
            }
            set 
            {
                _strLogName = value;
            }
        }

        /// <summary>
        /// 写事件日志
        /// </summary>
        /// <param name="strLog_"></param>
        /// <param name="euType_"></param>
        public static void Write(string strLog_, EventLogEntryType euType_)
        {
            try
            {
                if (_logEvent == null)
                {
                    _logEvent = new EventLog(LogName);
                    if (!EventLog.SourceExists(SourceName))
                        EventLog.CreateEventSource(SourceName, LogName);
                }

                _logEvent.WriteEntry(strLog_, euType_);
            }
            catch{}
        }

        /// <summary>
        /// 写事件日志
        /// </summary>
        /// <param name="euType_"></param>
        /// <param name="strFormate_"></param>
        /// <param name="oParams_"></param>
        public static void Write(EventLogEntryType euType_, string strFormate_, params object[] oParams_)
        {
            Write(string.Format(strFormate_, oParams_), euType_);
        }
    }
}
