using System;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.Net
{
    /// <summary>
    /// 网络相关异常
    /// </summary>
    public class XNetException : XBaseException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        public XNetException(string strInfo_) : base(strInfo_) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        /// <param name="exInner_"></param>
        public XNetException(string strInfo_, Exception exInner_)
            : base(strInfo_, exInner_)
        { }
    }

    /// <summary>
    /// 网络连接相关异常
    /// </summary>
    public class XNetConnException : XNetException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nError_">错误号</param>
        /// <param name="strInfo_"></param>
        public XNetConnException(uint nError_, string strInfo_) : base(strInfo_) {
            ErrorCode = nError_;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nError_">错误号</param>
        /// <param name="strInfo_"></param>
        /// <param name="exInner_"></param>
        public XNetConnException(uint nError_, string strInfo_, Exception exInner_)
            : base(strInfo_, exInner_)
        {
            ErrorCode = nError_;
        }

        /// <summary>
        /// 错误码
        /// </summary>
        public uint ErrorCode {get;private set;}
    }

    /// <summary>
    /// 登录登出时相关异常
    /// </summary>
    public class XLoginoutException : XNetException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        public XLoginoutException(string strInfo_) : base(strInfo_) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        /// <param name="exInner_"></param>
        public XLoginoutException(string strInfo_, Exception exInner_)
            : base(strInfo_, exInner_)
        { }
    }

    ///// <summary>
    ///// 接收相关异常
    ///// </summary>
    //public class XReceiveException : XNetException
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="strInfo_"></param>
    //    public XReceiveException(string strInfo_) : base(strInfo_) { }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="strInfo_"></param>
    //    /// <param name="exInner_"></param>
    //    public XReceiveException(string strInfo_, Exception exInner_)
    //        : base(strInfo_, exInner_)
    //    { }
    //}

    /// <summary>
    /// 接收相关异常
    /// </summary>
    public class XDataHandleException : XNetException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        public XDataHandleException(string strInfo_) : base(strInfo_) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        /// <param name="exInner_"></param>
        public XDataHandleException(string strInfo_, Exception exInner_)
            : base(strInfo_, exInner_)
        { }

        /// <summary>
        /// 字符串数据
        /// </summary>
        public string HandleData { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string From { get; set; }
    }

    /// <summary>
    /// 接收相关异常
    /// </summary>
    public class XResponseMismatchException : XNetException
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
