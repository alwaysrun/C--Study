using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SHCre.Xugd.Net
{
    /// <summary>
    /// 通讯（IM-Openfire）相关配置
    /// </summary>
    [Obsolete("Use SHCre.Xugd.Net.Xmpp.XmppConfig replace this")]
    public class XNetIMConfig
    {
        /// <summary>
        /// 服务器标识
        /// </summary>
        [XmlAttribute]
        public string SrvId {get;set;}

        /// <summary>
        /// 域名（Openfire服务器的域名）
        /// </summary>
        [XmlAttribute]
        public string Domain { get; set; }

        /// <summary>
        /// Openfire服务器地址
        /// </summary>
        [XmlAttribute]
        public string Address { get; set; }

        /// <summary>
        /// Openfire服务器端口
        /// </summary>
        [XmlAttribute]
        public int Port { get; set; }

        /// <summary>
        /// 登录Openfire服务器的用户名
        /// </summary>
        [XmlAttribute]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [XmlAttribute]
        public string Password { get; set; }

        /// <summary>
        /// Address:{IP}:{Port}[{Domain}]
        /// </summary>
        /// <returns></returns>
        public string PrintAddr()
        {
            return string.Format("{0}:{1}[{2}]", Address, Port, Domain);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            SrvId = string.Empty;
            Domain = "cti.net";
            Address = "192.168.1.202";
            Port = 5222;
            UserName = "cc1000";
            Password = "CreCTI";
        }
    }
}
