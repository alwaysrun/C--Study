using System;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.CFile
{
    /// <summary>
    /// 可触发异常与日志事件的接口
    /// </summary>
    public interface ILogEventsBase
    {
        /// <summary>
        /// 使用者设定的Tag
        /// </summary>
        object ConsumerTag { get; set; }
        /// <summary>
        /// 日志前缀（推荐以点号'.'结束）
        /// </summary>
        string LogPrefix { get; set; }
        /// <summary>
        /// 是否启用Debug输出
        /// </summary>
        bool DebugEnabled { get; }

        /// <summary>
        /// 是否已设定事件
        /// </summary>
        /// <returns></returns>
        bool IsLoggerSet();

        /// <summary>
        /// 是否启用Debug输出（启用后以LogLevels.Debug等级输出OnLogger）
        /// </summary>
        /// <param name="bEnabled_"></param>
        void SetDebugEnabled(bool bEnabled_);

        /// <summary>
        /// 抛出异常时触发的事件
        /// </summary>
        event Action<Exception, string> OnExcept;

        /// <summary>
        /// 调试事件，一些基本信息输出
        /// </summary>
        event Action<string, XLogSimple.LogLevels> OnLogger;
    }

    /// <summary>
    /// 可触发异常与日志事件的基类
    /// </summary>
    public abstract class XLogEventsBase:ILogEventsBase
    {
        /// <summary>
        /// 使用者设定的Tag
        /// </summary>
        public object ConsumerTag { get; set; }
        /// <summary>
        /// 日志前缀（推荐以点号'.'结束）
        /// </summary>
        public string LogPrefix {get; set;}

        private bool _bDebugEnabled = false;
        /// <summary>
        /// 是否启用Debug输出
        /// </summary>
        public bool DebugEnabled { get { return _bDebugEnabled; } }

        /// <summary>
        /// 是否启用Debug输出（启用后以LogLevels.Debug等级输出OnLogger）
        /// </summary>
        /// <param name="bEnabled_"></param>
        public virtual void SetDebugEnabled(bool bEnabled_)
        {
            _bDebugEnabled = bEnabled_;
        }

        /// <summary>
        /// 获取泛型子类的日志前缀（"ClassName&lt;T-Name&gt; "）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strClassName_"></param>
        protected void BuildLogPrefix<T>(string strClassName_)
        {
            LogPrefix = string.Format("{0}<{1}>.", strClassName_, XReflex.GetTypeName(typeof(T), false));
        }

        /// <summary>
        /// 
        /// </summary>
        public XLogEventsBase()
        {
            LogPrefix = string.Empty;
        }

        /// <summary>
        /// 是否已设定事件
        /// </summary>
        /// <returns></returns>
        public bool IsLoggerSet()
        {
            return OnLogger != null && OnLogger.GetInvocationList().Length > 0;
        }

        /// <summary>
        /// 抛出异常时触发的事件
        /// </summary>
        public event Action<Exception, string> OnExcept;
        /// <summary>
        /// 触发异常事件
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="strInfo_">描述异常的信息</param>
        protected void InvokeOnExcept(Exception ex, string strInfo_="")
        {
            if (OnExcept != null)
                OnExcept(ex, string.Format("{0}{1}", LogPrefix, strInfo_));
        }

        /// <summary>
        /// 调试事件，一些基本信息输出
        /// </summary>
        public event Action<string, XLogSimple.LogLevels> OnLogger;

        /// <summary>
        /// 触发日志事件
        /// </summary>
        /// <param name="strInfo_"></param>
        /// <param name="euLevel_">日志级别</param>
        protected void InvokeOnLogger(string strInfo_, XLogSimple.LogLevels euLevel_=XLogSimple.LogLevels.Info)
        {
            if (OnLogger != null)
                OnLogger(LogPrefix + strInfo_, euLevel_);
        }

        /// <summary>
        /// 函数被调用（自动为日志添加#前缀）
        /// </summary>
        /// <param name="strInfo_"></param>
        /// <param name="oParams_"></param>
        protected void InvokeOnCalled(string strInfo_, params object[] oParams_)
        {
            if (OnLogger != null)
                InvokeOnLogger(XLogSimple.LogLevels.Info, "#" + strInfo_, oParams_);
        }

        /// <summary>
        /// 触发日志事件
        /// </summary>
        /// <param name="euLevel_"></param>
        /// <param name="strInfo_"></param>
        /// <param name="oParams_"></param>
        protected void InvokeOnLogger(XLogSimple.LogLevels euLevel_, string strInfo_, params object[] oParams_)
        {
            if(OnLogger != null)
            {
                string strLog = string.Format(LogPrefix + strInfo_, oParams_);
                OnLogger(strLog, euLevel_);
            }
        }

        /// <summary>
        /// 以LogLevels.Debug等级输出信息（如果要写日志，日志文件要同时启用Debug模式）
        /// </summary>
        /// <param name="strInfo_"></param>
        /// <param name="oParams_"></param>
        protected void InvokeOnDebug(string strInfo_, params object[] oParams_)
        {
            if (DebugEnabled)
            {
                //InvokeOnLogger(XLogSimple.LogLevels.Debug, XLogSimple.GetDebugSrcInfo(1) + strInfo_, oParams_);
                InvokeOnLogger(XLogSimple.LogLevels.Debug, strInfo_, oParams_);
            }
        }
    }
}
