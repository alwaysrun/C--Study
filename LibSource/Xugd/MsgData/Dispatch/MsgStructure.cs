using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.MsgData
{
    partial class XMsgDispatch<TEnum>
    {
        /// <summary>
        /// 接收到的消息
        /// </summary>
        public class ReceivedMsg
        {
            internal Action<ReceivedMsg, IMsgDataBase, Action<XAsyncResult>, object> ActResponse;
            /// <summary>
            /// 发送者（对于SendBack，也要设为对方，即发送时的接收者）
            /// </summary>
            public string From { get; private set; }
            /// <summary>
            /// 主题
            /// </summary>
            public string Topic { get; private set; }
            /// <summary>
            /// 接收到的数据
            /// </summary>
            public XMsgWithType<TEnum> Data { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="strFrom_"></param>
            /// <param name="strTopic_"></param>
            /// <param name="revMsg_"></param>
            public ReceivedMsg(string strFrom_, string strTopic_, XMsgWithType<TEnum> revMsg_)
            {
                From = strFrom_;
                Topic = strTopic_;
                Data = revMsg_;
            }

            /// <summary>
            /// 发送应答
            /// </summary>
            /// <param name="argResponse_"></param>
            /// <param name="actComplete_"></param>
            /// <param name="oTag_">若要重设应答类型，则为实际的类型</param>
            public void SendResponse(IMsgDataBase argResponse_, Action<XAsyncResult> actComplete_, object oTag_=null)
            {
                if (ActResponse != null)
                    ActResponse(this, argResponse_, actComplete_, oTag_);
            }
        }
    } // class
}
