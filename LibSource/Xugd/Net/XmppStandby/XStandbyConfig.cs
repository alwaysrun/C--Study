using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SHCre.Xugd.Net.Xmpp
{
    /// <summary>
    /// 热备模式时，客户端的服务器信息配置
    /// </summary>
    public class XStandbyClientConfig
    {
        /// <summary>
        /// 登录超时，如果在此时间内没有发现主服务器，则作为超时出错
        /// </summary>
        [XmlAttribute]
        public int LoginTimeoutSecond {get;set;}
        /// <summary>
        /// 登录失败后重试时间间隔
        /// </summary>
        [XmlAttribute]
        public int LoginRetrySecond {get;set;}

        /// <summary>
        /// Xmpp服务器信息
        /// </summary>
        public List<XStandbyUserConfig> StandbyServer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public XStandbyClientConfig()
        {
            StandbyServer = new List<XStandbyUserConfig>();
        }

        /// <summary>
        /// 初始化，生成默认的配置
        /// </summary>
        public void Init()
        {
            LoginTimeoutSecond = 45;
            LoginRetrySecond = 60;
            var srv = new XStandbyUserConfig();
            srv.Init("cc2001", true);
            StandbyServer.Add(srv);
        }
    }

    /// <summary>
    /// Xmpp用户登录配置信息
    /// </summary>        
    public class XStandbyUserConfig{
        /// <summary>
        /// 是否启用
        /// </summary>
        [XmlAttribute]
        public bool Enabled { get; set; }
        /// <summary>
        /// 热备服务端的Cti服务用户
        /// </summary>
        [XmlAttribute]
        public string ServerName { get; set; }
        /// <summary>
        /// 应答超时时间
        /// </summary>
        [XmlAttribute]
        public int ResponseTimeoutSecond { get; set; }
        /// <summary>
        /// Xmpp服务器配置信息
        /// </summary>
        public XmppConfig XmppServer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public XStandbyUserConfig()
        {
            XmppServer = new XmppConfig();
        }

        /// <summary>
        /// 初始化，生成默认配置
        /// </summary>
        /// <param name="strUser_"></param>
        /// <param name="bEnable_"></param>
        public void Init(string strUser_, bool bEnable_=false){
            Enabled = bEnable_;
            ServerName = "ctisrv";
            ResponseTimeoutSecond = 30;
            XmppServer.Init(strUser_, "CreCTI", "xmppSync");
        }
    }

    /// <summary>
    /// 热备服务端的xmpp服务器信息配置
    /// </summary>
    public class XStandbyServerConfig
    {
        /// <summary>
        /// 唯一的服务器标识
        /// </summary>
        [XmlAttribute]
        public string SrvId { get; set; }
        /// <summary>
        /// 本地（近端）的xmpp服务器配置信息
        /// </summary>
        public XmppConfig LocalServer { get; set; }
        /// <summary>
        /// 远端（热备）的xmpp服务器配置信息
        /// </summary>
        public XStandbyUserConfig SyncServer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public XStandbyServerConfig()
        {
            LocalServer = new XmppConfig();
            SyncServer = new XStandbyUserConfig();
        }

        /// <summary>
        /// 初始化，生成默认配置
        /// </summary>
        public void Init()
        {
            SrvId = string.Empty;
            LocalServer.Init("ctisrv", "CreCTI", "xmppSrv");
            SyncServer.Init("ctisync");
            SyncServer.ResponseTimeoutSecond = 15;
        }
    }
}
