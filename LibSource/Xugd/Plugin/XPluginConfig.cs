using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using SHCre.Xugd.CFile;
using System.IO;

namespace SHCre.Xugd.Plugin
{
    /// <summary>
    /// 用于动态加载库配置
    /// </summary>
    public class XDllLoadConfig
    {
        /// <summary>
        /// 简洁、唯一名称
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }
        /// <summary>
        /// 要加载的库的名称（包括路径）
        /// </summary>
        [XmlAttribute]
        public string FileName { get; set; }
        /// <summary>
        /// 库的具体封装类名
        /// （含命名空间）
        /// </summary>
        [XmlAttribute]
        public string ClassName { get; set; }

        /// <summary>
        /// 配置文件
        /// </summary>
        [XmlAttribute]
        public string ConfigFile { get; set; }

        /// <summary>
        /// 说明，用于人识别的全名称
        /// </summary>
        [XmlAttribute]
        public string Remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strName_">插件名称</param>
        /// <param name="strFile_">加载的库文件名</param>
        /// <param name="strClass_">插件类名（包括命名空间）</param>
        public virtual void Init(string strName_, string strFile_, string strClass_)
        {
            Name = strName_;
            FileName = strFile_;
            ClassName = strClass_;
            ConfigFile = string.IsNullOrEmpty(strName_)?string.Empty:string.Format("{0}.xml", Name);
            Remark = string.Empty;
        }
    }

    /// <summary>
    /// App配置信息
    /// </summary>
    public class XPluginLoadConfig : XDllLoadConfig
    {
        /// <summary>
        /// 是否启用此插件
        /// </summary>
        [XmlAttribute]
        public bool Enabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strName_">插件名称</param>
        /// <param name="strFile_">加载的库文件名</param>
        /// <param name="strClass_">插件类名（包括命名空间）</param>
        public override void Init(string strName_, string strFile_, string strClass_)
        {
            base.Init(strName_, strFile_, strClass_);
            if(!string.IsNullOrEmpty(ConfigFile))
                ConfigFile = Path.Combine("Plugins", ConfigFile);
            if (string.IsNullOrEmpty(Path.GetDirectoryName(FileName)))
                FileName = Path.Combine("Plugins", FileName);
            Enabled = true;
        }
    }

    /// <summary>
    /// App配置信息
    /// </summary>
    public class XPluginWithLogConfig : XPluginLoadConfig
    {
        /// <summary>
        /// 日志文件
        /// </summary>
        public XLogConfig AppLog { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public XPluginWithLogConfig()
        {
            AppLog = new XLogConfig();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strName_">插件名称</param>
        /// <param name="strFile_">加载的库文件名</param>
        /// <param name="strClass_">插件类名（包括命名空间）</param>
        public XPluginWithLogConfig(string strName_, string strFile_, string strClass_)
            : this()
        {
            Init(strName_, strFile_, strClass_);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strName_">插件名称</param>
        /// <param name="strFile_">加载的库文件名</param>
        /// <param name="strClass_">插件类名（包括命名空间）</param>
        public override void Init(string strName_, string strFile_, string strClass_)
        {
            base.Init(strName_, strFile_, strClass_);
            AppLog.InitHourLog(strName_);
        }
    }

    /// <summary>
    /// 插件中包含插件
    /// </summary>
    public class XPluginWithSubConfig : XPluginWithLogConfig
    {
        /// <summary>
        /// 子插件
        /// </summary>
        public XDllLoadConfig SubPlugin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public XPluginWithSubConfig()
            : base()
        {
            SubPlugin = new XDllLoadConfig();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strName_"></param>
        /// <param name="strFile_"></param>
        /// <param name="strClass_"></param>
        public XPluginWithSubConfig(string strName_, string strFile_, string strClass_)
            : base(strName_, strFile_, strClass_)
        {
            SubPlugin = new XDllLoadConfig();
        }
    }
}
