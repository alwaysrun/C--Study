using System;
using System.IO;
using System.Threading;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.CFile
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public partial class XLogFile
    {
        #region "Var"
        private bool _bStartLog = false;

        /// <summary>
        /// 是否记录日志
        /// </summary>
        public bool NeedLog { get; set; }
        /// <summary>
        /// 日志中自动添加时间（只对WriteLine有效）
        /// </summary>
        public bool WithTime { get; set; }
        /// <summary>
        /// 日志中自动添加线程id（只对WriteLine有效）
        /// </summary>
        public bool WithThreadId { get; set; }

        /// <summary>
        /// 是否允许共享写，只能通过配置文件来设定
        /// </summary>
        private bool ShareWrite { get; set; }

        /// <summary>
        /// 日志记录标识或等级
        /// </summary>
        public uint LogFlags { get; set; }

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strNameFormat_">文件名格式（参加Config下的NameFormat）</param>
        /// <param name="nLogFlags_">要记录的日志标志位信息</param>
        /// <param name="bWithTime_">调用WriteLine时输出中是否自动添加当前时间</param>
        /// <param name="bWithThreadId_">调用WriteLine时输出中是否自动添加当前线程号</param>
        public XLogFile(string strNameFormat_, uint nLogFlags_ = uint.MaxValue, bool bWithTime_ = true, bool bWithThreadId_ = false)
        {
            this.FileNameFormat = strNameFormat_;
            this.WithTime = bWithTime_;
            this.WithThreadId = bWithThreadId_;
            NeedLog = true;
            ShareWrite = false;
            LogFlags = nLogFlags_;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strNameFormat_"></param>
        /// <param name="euLogFlags_"></param>
        /// <param name="bWithTime_"></param>
        /// <param name="bWithThreadId_"></param>
        public XLogFile(string strNameFormat_, Enum euLogFlags_, bool bWithTime_ = true, bool bWithThreadId_ = false)
            : this(strNameFormat_, Convert.ToUInt32(euLogFlags_), bWithTime_, bWithThreadId_)
        {
        }

        /// <summary>
        /// 使用配置文件的构造函数
        /// </summary>
        /// <param name="conLog_"></param>
        public XLogFile(Config conLog_) :
            this(conLog_.NameFormat, conLog_.LogFlags, conLog_.WithTime, conLog_.WithThreadId)
        {
            //RemoveFileFilter = conLog_.RemoveFileFilter;
            //RemoveFileExpire = TimeSpan.FromDays(conLog_.RemoveExpireDays);
            ShareWrite = conLog_.ShareWrite;
        }

        /// <summary>
        /// 关闭，当不在使用时，需要关闭文件；
        /// 关闭后FileNameFormat将被清空，如果要再次打开日志记录，可重设FileNameFormat
        /// </summary>
        public void Close()
        {
            FileNameFormat = string.Empty;
            _evtToWrite.Set();
        }


        private bool IsNeedLog()
        {
            return (NeedLog && _bStartLog);
        }

        #region "Write Buffer"
        ///// <summary>
        ///// 写字节数组（只有LogFlags标志位包含的Flag才会记录）
        ///// </summary>
        ///// <param name="byBuffer_">要写的内容</param>
        ///// <param name="nFlag_">当前记录的标志位</param>
        //public void Write(byte[] byBuffer_, uint nFlag_ = uint.MaxValue)
        //{
        //    if (!XFlag.CheckAny(LogFlags, nFlag_) || !IsNeedLog()) return;

        //    AddWriteLog(byBuffer_);
        //}

        ///// <summary>
        ///// 写字节数组（只有LogFlags标志位包含的Flag才会记录）
        ///// </summary>
        ///// <param name="byBuffer_"></param>
        ///// <param name="euFlag_"></param>
        //public void Write(byte[] byBuffer_, Enum euFlag_)
        //{
        //    Write(byBuffer_, Convert.ToUInt32(euFlag_));
        //}
        #endregion

        #region "Write String"
        private string BuildLogHeader(bool bWithTime_, bool bWithThreadId_, bool bAddTap_=true)
        {
            string strHeader = string.Empty;
            if (bWithTime_)
                strHeader = XTime.GetFullString(DateTime.Now, true);
            if (bWithThreadId_)
                strHeader += "[" + Thread.CurrentThread.ManagedThreadId.ToString() + "]";

            if (bAddTap_ && !string.IsNullOrEmpty(strHeader))
                strHeader += "\t";

            return strHeader;
        }

        /// <summary>
        /// 写字符串（只有LogFlags标志位包含的Flag才会记录）
        /// </summary>
        /// <param name="strLog_">内容</param>
        /// <param name="bWithTime_">是否包含当前时间</param>
        /// <param name="bWithThreadId_">是否包含当前线程号</param>
        /// <param name="nFlag_">当前记录的标志位</param>
        public void Write(string strLog_, uint nFlag_ = uint.MaxValue, bool bWithTime_ = false, bool bWithThreadId_ = false)
        {
            if (!XFlag.CheckAny(LogFlags, nFlag_) || !IsNeedLog()) return;

            AddWriteLog(BuildLogHeader(bWithTime_, bWithThreadId_) + strLog_);
        }

        /// <summary>
        /// 写字符串（只有LogFlags标志位包含的Flag才会记录）
        /// </summary>
        /// <param name="strLog_"></param>
        /// <param name="euFlag_"></param>
        /// <param name="bWithTime_"></param>
        /// <param name="bWithThreadId_"></param>
        public void Write(string strLog_, Enum euFlag_, bool bWithTime_ = false, bool bWithThreadId_ = false)
        {
            Write(strLog_, Convert.ToUInt32(euFlag_), bWithTime_, bWithThreadId_);
        }

        /// <summary>
        /// 写入格式化内容（参考string.Format）并换行（只有LogFlags标志位包含的Flag才会记录）：
        /// 由WithTime决定是否包含时间，由WithThreadId决定是否包含线程号
        /// </summary>
        /// <param name="nFlag_">当前记录的标志位</param>
        /// <param name="strFormat_">复合格式字符串</param>
        /// <param name="oArgs_">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public void WriteLine(uint nFlag_, string strFormat_, params object[] oArgs_)
        {
            if (!XFlag.CheckAny(LogFlags, nFlag_) || !IsNeedLog()) return;

            string strLog;
            if (oArgs_.Length == 0)
                strLog = strFormat_;
            else
                strLog = string.Format(strFormat_, oArgs_);
            AddWriteLog(BuildLogHeader(WithTime, WithThreadId) + strLog + XString.NewLine);
        }

        /// <summary>
        /// 写入格式化内容（参考string.Format）并换行（只有LogFlags标志位包含的Flag才会记录）：
        /// 由WithTime决定是否包含时间，由WithThreadId决定是否包含线程号
        /// </summary>
        /// <param name="euFlag_"></param>
        /// <param name="strFormat_"></param>
        /// <param name="oArgs_"></param>
        public void WriteLine(Enum euFlag_, string strFormat_, params object[] oArgs_)
        {
            WriteLine(Convert.ToUInt32(euFlag_), strFormat_, oArgs_);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="euLevel_"></param>
        /// <returns></returns>
        internal bool IsLevelNeedLog(uint euLevel_)
        {
            return (euLevel_ <= LogFlags) && IsNeedLog();
        }

        /// <summary>
        /// 写入格式化内容（参考string.Format）并换行：
        /// 由WithTime决定是否包含时间，由WithThreadId决定是否包含线程号
        /// </summary>
        /// <param name="strFormat_">复合格式字符串</param>
        /// <param name="oArgs_">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public void WriteLine(string strFormat_, params object[] oArgs_)
        {
            WriteLine(uint.MaxValue, strFormat_, oArgs_);
        }

        /// <summary>
        /// 写入内容并换行（只有LogFlags标志位包含的Flag才会记录）：
        /// 由WithTime决定是否包含时间，由WithThreadId决定是否包含线程号
        /// </summary>
        /// <param name="strLog_">要写入的内容</param>
        /// <param name="nFlag_">当前记录的标志位</param>
        public void WriteLine(string strLog_, uint nFlag_ = uint.MaxValue)
        {
            if (!XFlag.CheckAny(LogFlags, nFlag_) || !IsNeedLog()) return;

            AddWriteLog(BuildLogHeader(WithTime, WithThreadId) + strLog_ + XString.NewLine);
        }

        /// <summary>
        /// 写入内容并换行（只有LogFlags标志位包含的Flag才会记录）：
        /// 由WithTime决定是否包含时间，由WithThreadId决定是否包含线程号
        /// </summary>
        /// <param name="strLog_"></param>
        /// <param name="euFlag_"></param>
        public void WriteLine(string strLog_, Enum euFlag_)
        {
            WriteLine(strLog_, Convert.ToUInt32(euFlag_));
        }
        #endregion

        /// <summary>
        /// 写入一个空行
        /// </summary>
        public void WriteLine()
        {
            if (!IsNeedLog()) return;

            AddWriteLog(XString.NewLine);
        }
    } // class
}
