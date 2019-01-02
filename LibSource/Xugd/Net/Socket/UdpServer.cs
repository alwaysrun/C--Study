using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using SHCre.Xugd.Common;
using System.Threading;

namespace SHCre.Xugd.Net
{
    /// <summary>
    /// UDP服务端，由XUdpClient来连接
    /// </summary>
    public class XUdpServer:XCommServer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="srvConfig_"></param>
        public XUdpServer(XCommServerConfig srvConfig_)
            : base(srvConfig_)
        {
            ServerConfig.DataCarryHeader = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Stop()
        {
            _lstConnections.Collect.ForEach(z => SendClose(z.LocalAddress, z.RemoteEndPoint));

            base.Stop();
        }

        void SendClose(string strSender_, IPEndPoint toPoint_)
        {
            var firstSender = _lstListeners.FirstOrDefault(z => z.ListenAddress == strSender_);
            if (firstSender == null) return;

            ManualResetEvent evtWait = new ManualResetEvent(false);
            firstSender.Send2Client(toPoint_, UdpLoginout.LogoutRequest, zop =>evtWait.Set());
            evtWait.WaitOne(SendSynTimeoutMSec);
        }

        /// <summary>
        /// 创建侦听者
        /// </summary>
        /// <param name="listenPoint_"></param>
        /// <param name="oTag_"></param>
        /// <returns></returns>
        protected override XCommListener CreateListener(IPEndPoint listenPoint_, object oTag_)
        {
            return new XUdpListener(this, listenPoint_);
        }

        /// <summary>
        /// 发送数据到客户端
        /// </summary>
        /// <param name="strSender_">发送者信息</param>
        /// <param name="toPoint_"></param>
        /// <param name="byData_"></param>
        /// <param name="actSend_"></param>
        public override void Send2Client(string strSender_, IPEndPoint toPoint_, byte[] byData_, Action<Exception> actSend_)
        {
            var firstSender = _lstListeners.FirstOrDefault(z => z.ListenAddress == strSender_);
            if(firstSender == null)
            {
                LogFile.Error("UdpServer.Send2Client: No sender for {0}->{1}", strSender_, toPoint_);
                if(actSend_ != null)
                    actSend_(new XNotFoundException(string.Format("Sender {0} Not login", strSender_)));
                return;
            }

            firstSender.Send2Client(toPoint_, byData_, actSend_);
        }

        internal void AcceptConnection(IPEndPoint localEnd_, IPEndPoint remoteEnd_)
        {
            new XUdpClient(localEnd_, remoteEnd_, this, ServerConfig);
        }
    } // class
}
