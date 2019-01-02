using System.Collections.Generic;
using System.Xml.Serialization;
using SHCre.Xugd.CFile;
using System;

namespace SHCre.Xugd.Net
{
    internal static class DefaultValue
    {
        public const int SendTimeOutSecond = 10;
        public const int ReceiveTimeoutSecond = 15;
        public const int IdleAliveSecond = 120;
        public const int CheckAliveSecond = 60;
        public const int ReconnectSecond = 30;
        public const int ReceiveBuffSize = 4096;
    }
    
    /// <summary>
    /// Comm的协议
    /// </summary>
    public enum XCommProtocol
    {
        /// <summary>
        /// 
        /// </summary>
        Tcp = 0,
        /// <summary>
        /// 
        /// </summary>
        Udp,
    }

    /// <summary>
    /// 
    /// </summary>
    public class XCommBaseConfig
    {
        /// <summary>
        /// 协议
        /// </summary>
        [XmlAttribute]
        public XCommProtocol Protocol {get;set;}
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public string ProtocolChoice {get;set;}

        /// <summary>
        /// 编码方式设定后：
        /// 所有接收到数据都会自动解码，否则不解码；
        /// 所有发送的字符串都使用此方式编码；
        /// </summary>
        [XmlAttribute]
        public string EncodingName { get; set; }

        /// <summary>
        /// 发送数据时，是否增加发送头（魔数+长度）
        /// </summary>
        [XmlAttribute]
        public bool DataCarryHeader { get; set; }

        /// <summary>
        /// 允许连接空闲时间（没有数据收发）
        /// </summary>
        [XmlAttribute]
        public int IdleAliveSecond { get; set; }
        /// <summary>
        /// 接收带数据头的信息时，如果数据不足，最长等待时间；
        /// 对UPD则为登录时，等待应答的时间
        /// </summary>
        [XmlAttribute]
        public int ReceiveTimeoutSecond {get;set;}

        /// <summary>
        /// 同步发送时，返回前等待的秒数：
        /// 如果不设定默认15s
        /// </summary>
        [XmlAttribute]
        public int SendSynTimeoutSecond { get; set; }

        /// <summary>
        /// 日志记录
        /// </summary>
        public XLogConfig CommLog { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Init()
        {
            CommLog = new XLogConfig();
            CommLog.InitHourLog("Comm");

            Protocol = XCommProtocol.Tcp;
            ProtocolChoice = string.Join(",", Enum.GetNames(typeof(XCommProtocol)));

            DataCarryHeader = true;
            SendSynTimeoutSecond = DefaultValue.SendTimeOutSecond;
            ReceiveTimeoutSecond = DefaultValue.ReceiveTimeoutSecond;
            IdleAliveSecond = DefaultValue.IdleAliveSecond;
            EncodingName = "utf-8";
        }
    }

    /// <summary>
    /// 侦听端配置信息
    /// </summary>
    public class XCommServerConfig : XCommBaseConfig
    {
        /// <summary>
        /// 侦听地址列表
        /// </summary>
        public List<XNetAddrConfig> ListenAddresses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public override void Init()
        {
            Init(5900, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nPort_"></param>
        /// <param name="strIP_"></param>
        public void Init(int nPort_, string strIP_="")
        {
            base.Init();

            ListenAddresses = new List<XNetAddrConfig>();
            var addrNone = new XNetAddrConfig();
            addrNone.Init(strIP_, nPort_);
            ListenAddresses.Add(addrNone);
        }

        /// <summary>
        /// 获取通讯服务端
        /// </summary>
        /// <returns></returns>
        public XCommServer GetServer()
        {
            switch(Protocol)
            {
                case XCommProtocol.Tcp:
                    return new XTcpServer(this);

                case XCommProtocol.Udp:
                    return new XUdpServer(this);

                default:
                    throw new NotSupportedException(string.Format("Protocol {0} not support", Protocol));
            }
        }
    }

    /// <summary>
    /// 连接相关配置信息
    /// </summary>
    public class XCommClientConfig : XCommBaseConfig
    {
        /// <summary>
        /// 发送心跳包的时间间隔
        /// </summary>
        [XmlAttribute]
        public int CheckAliveInterSecond { get; set; }

        /// <summary>
        /// 连接断开后，多长时间尝试重连
        /// </summary>
        [XmlAttribute]
        public int ReconnectInterSecond { get; set; }

        /// <summary>
        /// 在发送数据前，如果连接断开是否尝试重连
        /// </summary>
        [XmlAttribute]
        public bool AutoConnectWhenSend { get; set; }

        /// <summary>
        /// 对方地址
        /// </summary>
        public XNetAddrConfig RemoteAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public XCommClientConfig()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strIP_"></param>
        /// <param name="nPort_"></param>
        public XCommClientConfig(string strIP_, int nPort_)
            : base()
        {
            Init(strIP_, nPort_);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Init()
        {
            Init("127.0.0.1", 5900);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strIP_"></param>
        /// <param name="nPort_"></param>
        public void Init(string strIP_, int nPort_)
        {
            base.Init();
            RemoteAddress = new XNetAddrConfig();
            RemoteAddress.Init(strIP_, nPort_);

            CheckAliveInterSecond = DefaultValue.CheckAliveSecond;
            ReconnectInterSecond = DefaultValue.ReconnectSecond;
            AutoConnectWhenSend = true;
        }

        /// <summary>
        /// 获取通讯客户端
        /// </summary>
        /// <returns></returns>
        public XCommConnection GetClient()
        {
            switch(Protocol)
            {
                case XCommProtocol.Tcp:
                    return new XTcpClient(this);

                case XCommProtocol.Udp:
                    return new XUdpClient(this);

                default:
                    throw new NotSupportedException(string.Format("Protocol {0} not support", Protocol));
            }
        }
    }
}
