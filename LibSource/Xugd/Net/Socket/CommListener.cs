using SHCre.Xugd.CFile;
using System.Net;
using System;

namespace SHCre.Xugd.Net
{
    /// <summary>
    /// 服务端侦听的基类
    /// </summary>
    public abstract class XCommListener
    {
        /// <summary>
        /// 是否在侦听
        /// </summary>
        public bool IsListening { get; protected set; }
        /// <summary>
        /// 对应的服务端信息
        /// </summary>
        public XCommServer Server { get; private set; }
        /// <summary>
        /// 侦听端点
        /// </summary>
        public IPEndPoint ListenEndPoint { get; private set; }
        string _strListenAddr = null;
        /// <summary>
        /// 侦听的地址
        /// </summary>
        public string ListenAddress 
        {
            get 
            {
                if(string.IsNullOrEmpty(_strListenAddr) && (ListenEndPoint!=null))
                {
                    _strListenAddr = ListenEndPoint.ToString();
                }

                return _strListenAddr;
            }
        }
        /// <summary>
        /// 用户的标识（一般为配置文件中SrvId）
        /// </summary>
        public object UserTag { get; set; }

        /// <summary>
        /// 日志记录
        /// </summary>
        protected XLogSimple LogFile { get { return Server.LogFile; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srvComm_"></param>
        /// <param name="listenPoint_"></param>
        /// 
        public XCommListener(XCommServer srvComm_, IPEndPoint listenPoint_)
        {
            Server = srvComm_;
            ListenEndPoint = listenPoint_;
        }

        /// <summary>
        /// 开始侦听
        /// </summary>
        public void Start()
        {
            if (!IsListening)
            {
                IsListening = true;
                StartToListen();
            }
        }

        /// <summary>
        /// 停止侦听
        /// </summary>
        public void Stop()
        {
            if (IsListening)
            {
                IsListening = false;
                StopListen();
            }
        }

        /// <summary>
        /// 发送数据到客户端
        /// </summary>
        /// <param name="toPoint_"></param>
        /// <param name="byData_"></param>
        /// <param name="act_"></param>
        public virtual void Send2Client(IPEndPoint toPoint_, byte[] byData_, Action<Exception> act_)
        {
            throw new NotImplementedException("CommListener.Send2Client: Should not call it");
        }

        /// <summary>
        /// 开始侦听：在子类中实现
        /// </summary>
        protected abstract void StartToListen();
        /// <summary>
        /// 停止侦听：在子类中实现
        /// </summary>
        protected abstract void StopListen();
    }
}
