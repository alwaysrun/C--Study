using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SHCre.Xugd.CFile
{
    partial class XLogFile
    {
        /// <summary>
        /// 配置文件：可通过SHCre.Xugd.Config名称空间中的XConFile类中的
        /// Read与Write来读写配置文件
        /// </summary>
        public class Config
        {
            /// <summary>
            /// 要移除的文件过期天数，可以使用小数（如2.5）；
            /// 默认0，不删除
            /// </summary>
            [XmlAttribute]
            public double RemoveExpireDays { get; set; }

            /// <summary>
            /// 移除文件时，文件的过滤规则(默认为*.log)
            /// </summary>
            public string RemoveFileFilter {get; set;}

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
            /// 是否允许共享写，默认false；
            /// 一般不能共享写，否则可能会出现日志混乱的情况
            /// </summary>
            [XmlAttribute]
            public bool ShareWrite { get; set; }

            /// <summary>
            /// 日志记录标识（只有对应标识位才记录；默认0xFFFF，记录所有）
            /// </summary>
            [XmlAttribute]
            public uint LogFlags {get; set;}

            /// <summary>
            /// 
            /// </summary>
            public Config()
            {
                RemoveExpireDays = 0;
                //RemoveFileFilter = "*.log";
                NameFormat = string.Empty;
                WithTime = true;
                WithThreadId = false;
                ShareWrite = false;
                LogFlags = 0xFFFF;
            }

            /// <summary>
            /// 初始化按小时记录的日志，文件名为FileName{HH}.log
            /// </summary>
            /// <param name="strFileName_"></param>
            public void InitHourLog(string strFileName_)
            {
                NameFormat = string.Format(@"Log\{{yyyy}}\{{MM}}\{{dd}}\{0}{{HH}}.log", strFileName_);
            }

            /// <summary>
            /// 初始化按天记录的日志，文件名为FileName{dd}.log
            /// </summary>
            /// <param name="strFileName_"></param>
            public void InitDayLog(string strFileName_)
            {
                NameFormat = string.Format(@"Log\{{yyyy}}\{{MM}}\{0}{{dd}}.log", strFileName_);
            }

            /// <summary>
            /// 日志配置转换
            /// </summary>
            /// <param name="logConf_"></param>
            /// <returns></returns>
            public static Config FromLogConfig(XLogConfig logConf_)
            {
                return new Config()
                {
                    NameFormat = logConf_.NameFormat,
                    WithTime = logConf_.WithTime,
                    WithThreadId = logConf_.WithThreadId,
                    LogFlags = (uint)logConf_.LogLevel,
                };
            }

            /// <summary>
            /// 日志配置转换
            /// </summary>
            /// <returns></returns>
            public XLogConfig ToLogConfig()
            {
                return new XLogConfig()
                {
                    NameFormat = this.NameFormat,
                    WithTime = this.WithTime,
                };
            }
        }
    }
}
