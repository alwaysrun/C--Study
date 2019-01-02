using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using SHCre.Xugd.Config;
using SHCre.Xugd.Common;
using SHCre.Xugd.WinAPI;

namespace SHCre.Xugd.Extension
{
            
    /// <summary>
    /// 服务本身的信息
    /// </summary>
    public class XServiceInfo
    {
        /// <summary>
        /// 服务的名称（如果为空或null，则使用文件名作为服务名）
        /// </summary>
        [XmlAttribute]
        public string SrvName {get; set;}
        /// <summary>
        /// 运行的类型
        /// </summary>
        [XmlAttribute]
        public string RunType { get; set; }
        /// <summary>
        /// 互斥体名（如果为空或null，则通过文件路径生成唯一的互斥体名称）
        /// </summary>
        [XmlAttribute]
        public string SynName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public XServiceInfo()
        {
            SrvName = string.Empty;
            RunType = XConvert.Enum2Name(XWin.Service.ServiceType.Win32OwnProcess);
            SynName = string.Empty;
        }
    }

        /// <summary>
        /// 服务参数
        /// </summary>
        public class XServiceParams
        {
            /// <summary>
            /// 依赖的服务服务
            /// </summary>
            public class Depend
            {
                /// <summary>
                /// 依赖的服务的名称
                /// </summary>
                [XmlAttribute]
                public string Name { get; set; }
            }

            /// <summary>
            /// 服务信息
            /// </summary>
            public XServiceInfo Service { get; set; }
            /// <summary>
            /// 依赖项
            /// </summary>
            public List<Depend> Depends { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public XServiceParams()
            {
                Service = new XServiceInfo();
                Depends = new List<Depend>();
            }

            /// <summary>
            /// 读取配置信息，如果没有在返回一个默认的配置
            /// </summary>
            /// <param name="strFile_">文件名（默认：XServiceParam.xml）</param>
            /// <returns></returns>
            public static XServiceParams Read(string strFile_=null)
            {
                if (string.IsNullOrEmpty(strFile_))
                    strFile_ = "XServiceParam.xml";

                XServiceParams vParam = XConFile.Read <XServiceParams>(strFile_);
                if(vParam == null)
                {
                    vParam = new XServiceParams();
                    vParam.Depends.Add(new Depend()
                        {
                            Name = "tcpip",
                        });
                    vParam.Write(strFile_);
                }

                return vParam;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="strFile_"></param>
            public void Write(string strFile_=null)
            {
                if (string.IsNullOrEmpty(strFile_))
                    strFile_ = "XServiceParam.xml";

                XConFile.Write(strFile_, this);
            }
        }
} // namespace
