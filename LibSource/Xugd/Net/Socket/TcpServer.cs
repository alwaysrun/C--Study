using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace SHCre.Xugd.Net
{
    /// <summary>
    /// TCP通讯的服务端:
    /// 重载OnNewConnection来处理新连接进入时的处理
    /// </summary>
    public class XTcpServer : XCommServer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="srvConfig_"></param>
        public XTcpServer(XCommServerConfig srvConfig_):base(srvConfig_)
        {
        }

        /// <summary>
        /// 新的连接进入时
        /// </summary>
        /// <param name="sockAccept_"></param>
        /// <param name="listener_"></param>
        internal void AcceptConnection(Socket sockAccept_, XTcpListener listener_)
        {
            XTcpClient.CreateConnection(sockAccept_, this, ServerConfig);
        }

        /// <summary>
        /// 创建侦听者
        /// </summary>
        /// <param name="listenPoint_"></param>
        /// <param name="oTag_"></param>
        /// <returns></returns>
        protected override XCommListener CreateListener(IPEndPoint listenPoint_, object oTag_)
        {
            return new XTcpListener(this, listenPoint_);
        }
    }
}
