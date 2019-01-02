using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.MsgData
{
    /// <summary>
    /// 应答没有匹配
    /// </summary>
    public class XResponsedMismatchException : XDataException
    {
        /// <summary>
        /// 应答的消息
        /// </summary>
        public XMsgReceivedArgs ResponseMsg {get; set;}

        /// <summary>
        /// 
        /// </summary>
        public XResponsedMismatchException()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        public XResponsedMismatchException(string strInfo_) : base(strInfo_) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        /// <param name="exInner_"></param>
        public XResponsedMismatchException(string strInfo_, Exception exInner_)
            : base(strInfo_, exInner_)
        { }
    }
}
