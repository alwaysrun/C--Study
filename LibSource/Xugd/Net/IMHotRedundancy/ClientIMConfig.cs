using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SHCre.Xugd.Net
{
    /// <summary>
    /// 热备客户端配置
    /// </summary>
    [Obsolete("Use SHCre.Xugd.Net.Xmpp.XStandbyClientConfig replace this")]
    public class XIMRedundClientConfig
    {
        /// <summary>
        /// 服务端用户名
        /// </summary>
        [XmlAttribute]
        public string ServerUser {get;set;}
        /// <summary>
        /// 是否启用
        /// </summary>
        [XmlAttribute]
        public bool Enabled {get;set;}
        /// <summary>
        /// 应答超时秒数
        /// </summary>
        [XmlAttribute]
        public int RespondTimeoutSecond {get;set;}

        /// <summary>
        /// IM配置信息
        /// </summary>
        public XNetIMConfig IMServer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public XIMRedundClientConfig()
        {
            IMServer = new XNetIMConfig();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            ServerUser = "ctisrv";
            Enabled = true;
            RespondTimeoutSecond = 0;
            IMServer.Init();
        }
    }
}
