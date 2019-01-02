using System;
using System.Net.Sockets;

namespace SHCre.Xugd.Net
{
    /// <summary>
    /// 数据到达
    /// </summary>
    public class XCommRawDataArrivalArgs : EventArgs
    {
        /// <summary>
        /// 数据缓冲区
        /// </summary>
        public byte[] Data { get; set; }
        /// <summary>
        /// 开始位置
        /// </summary>
        public int Offset { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        public int Count { get; set; }
    }

    /// <summary>
    /// 侦听失败
    /// </summary>
    public class XCommListenErrorArgs : EventArgs
    {
        /// <summary>
        /// 失败信息
        /// </summary>
        public Exception Error { get; set; }
        /// <summary>
        /// 侦听者信息
        /// </summary>
        public XCommListener Listener { get; set; }
    }

    /// <summary>
    /// 数据解码失败的异常
    /// </summary>
    public class XCommDataDecodeException:XNetException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        public XCommDataDecodeException(string strInfo_) : base(strInfo_) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        /// <param name="exInner_"></param>
        public XCommDataDecodeException(string strInfo_, Exception exInner_)
            : base(strInfo_, exInner_)
        { }

        /// <summary>
        /// 数据缓冲区
        /// </summary>
        public byte[] RawData { get; set; }
        /// <summary>
        /// 开始位置
        /// </summary>
        public int Offset { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        public int Count { get; set; }
    }
}
