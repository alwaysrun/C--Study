using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icld.NetIM.Client;
using Icld.NetIM.Client.Xmpp;
using SHCre.Xugd.Common;
using SHCre.Xugd.Data;

namespace SHCre.Xugd.Net
{
    partial class XIMConnection
    {
        /// <summary>
        /// 数据接收到的事件
        /// </summary>
        public event Action<XReceiveDataArgs> OnDataReceived;
        private void InvokeOnDataReceived(string strFromJid_, string strToJid_, string strData_, XReceiveDataType euType_)
        {
            if (OnDataReceived != null)
                OnDataReceived(new XReceiveDataArgs()
                {
                    From = Jid2Name(strFromJid_),
                    Type = euType_,
                    Data = strData_,
                });
        }

        /// <summary>
        /// 发送数据的接口：
        /// 发送成功与否的信息通过actComplete获取
        /// </summary>
        /// <param name="strTo">接收者</param>
        /// <param name="strMsg_">要发送的字符串</param>
        /// <param name="actComplete_"></param>
        /// <param name="oTag_"></param>
        public void SendAsync(string strTo, string strMsg_, Action<XAsyncResult> actComplete_, object oTag_ = null)
        {
            Exception exErr = null;
            try
            {
                _imChat.Send(Name2Jid(strTo), strMsg_);
            }
            catch(Exception ex)
            {
                exErr = ex;
            }

            if (actComplete_ != null)
                actComplete_(XAsyncResult.Get(exErr));
        }
    }
}
