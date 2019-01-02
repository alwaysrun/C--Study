using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.Data
{
    /// <summary>
    /// 接收相关异常
    /// </summary>
    public class XResponseMismatchException : XDataException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        public XResponseMismatchException(string strInfo_) : base(strInfo_) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        /// <param name="exInner_"></param>
        public XResponseMismatchException(string strInfo_, Exception exInner_)
            : base(strInfo_, exInner_)
        { }

        /// <summary>
        /// 来源
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// 获取消息（包括类名)
        /// </summary>
        /// <returns></returns>
        public override string GetMessage()
        {
            return string.Format("XResponseMismatchException(From:{0}, Info:{1})", From, this.Message);
        }
    }
}
