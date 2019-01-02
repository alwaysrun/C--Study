using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.CFile
{
    /// <summary>
    /// 配置文件：用于配制XLogSimple类型的日志文件
    /// </summary>
    public class XLogConfig
    {
        /// <summary>
        /// 文件名格式：
        /// 可使用{xxxx}来表示年、月、日等时间,x的具体值参考DateTime.ToString中的格式化字符串；
        /// 如"Log\{yyyy}\{MM}\{dd}\MyLog {HH}.log"就是按年月日创建目录，并每小时创建一个新的文件
        /// </summary>
        [XmlAttribute]
        public string NameFormat { get; set; }

        /// <summary>
        /// WriteLine时，日志每行前面是否带时间；
        /// 默认true
        /// </summary>
        [XmlAttribute]
        public bool WithTime { get; set; }

        /// <summary>
        /// WriteLine时，每行日志前面是否带线程号；
        /// 默认false
        /// </summary>
        [XmlAttribute]
        public bool WithThreadId { get; set; }
        
        /// <summary>
        /// 日志记录标识（记录不大于此等级的所有日志，默认XLogSimple.LogLevels.Info）
        /// </summary>
        [XmlAttribute]
        public XLogSimple.LogLevels LogLevel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public string LevelChoice {get;set;}

        /// <summary>
        /// 
        /// </summary>
        public XLogConfig()
        {
            LogLevel = XLogSimple.LogLevels.Info;
        }

        /// <summary>
        /// 
        /// </summary>
        void Init()
        {
            NameFormat = string.Empty;
            WithTime = true;
            WithThreadId = false;
            LevelChoice = XString.GetEnumNames<XLogSimple.LogLevels>();
        }

        /// <summary>
        /// 初始化按小时记录的日志，文件名为FileName{HH}.log
        /// </summary>
        /// <param name="strFileName_"></param>
        public void InitHourLog(string strFileName_)
        {
            Init();
            NameFormat = string.Format(@"Log\{{yyyy}}\{{MM}}\{{dd}}\{0}{{HH}}.log", strFileName_);
        }

        /// <summary>
        /// 初始化按天记录的日志，文件名为FileName{dd}.log
        /// </summary>
        /// <param name="strFileName_"></param>
        public void InitDayLog(string strFileName_)
        {
            Init();
            NameFormat = string.Format(@"Log\{{yyyy}}\{{MM}}\{0}{{dd}}.log", strFileName_);
        }
    }
}
