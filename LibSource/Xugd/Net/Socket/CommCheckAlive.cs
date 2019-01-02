using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.Net
{
    partial class XCommConnection
    {
        const int CheckAliveDataLen = 2;
        readonly byte[] _byCheckAliveRequest = { 0xFF, 0x05 };
        readonly byte[] _byCheckAliveResponse = { 0x05, 0xFF };
        /// <summary>
        /// 心跳包的实现：只有客户端连接需要设定（如果不设定，则程序内部自动实现）
        /// 只有在同时在配置文件中配置了CheckAliveInteval时，才有效。
        /// </summary>
        public Action<XCommConnection> ActCheckAlive;
        void InvokeCheckAlive()
        {
            if (ActCheckAlive != null)
            {
                ActCheckAlive(this);
            }
            else
            {
                SendAsync(_byCheckAliveRequest, null);
            }
        }

        bool IsCheckAlive(byte[] byData_, int nOffset_, int nCount_)
        {
            if (ActCheckAlive != null || nCount_ != CheckAliveDataLen)
                return false;

            if(XCompare.AreEqual(_byCheckAliveRequest, 0, byData_, nOffset_, CheckAliveDataLen))
            { // Request
                LogFile.Info("CheckAlive request");
                SendAsync(_byCheckAliveResponse, null);
                return true;
            }
            else if(XCompare.AreEqual(_byCheckAliveResponse, 0, byData_, nOffset_, CheckAliveDataLen))
            {
                LogFile.Info("CheckAlive respond");
                return true;
            }

            return false;
        }
    } // class
}
