using System;
using SHCre.Xugd.Data;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.Data
{
    partial class XDataComm<TEnum>
    {
        /// <summary>
        /// 请求数据信息：使用此类时要添加引用‘SHCre.Xugd.Data’
        /// </summary>
        public class RequestArrivalArgs : EventArgs
        {
            /// <summary>
            /// 完成后，应答的处理
            /// </summary>
            internal Action<RequestArrivalArgs, object, Action<XAsyncResult>, object> ActResponse;

            /// <summary>
            /// 对request的应答
            /// </summary>
            /// <param name="responseType_"></param>
            /// <param name="responseData_"></param>
            /// <param name="actComplete_"></param>
            /// <param name="oTag_"></param>
            public void InvokeResponse(TEnum responseType_, object responseData_, Action<XAsyncResult> actComplete_, object oTag_=null)
            {
                if (ActResponse != null)
                {
                    ActResponse(this, responseData_, actComplete_, oTag_);
                }
            }

            /// <summary>
            /// 请求者
            /// </summary>
            public string From { get; set; }

            /// <summary>
            /// 请求的数据
            /// </summary>
            public XJsonDataFormat.DataWithType<TEnum> RequestData { get; set; }
        }

        /// <summary>
        /// 应答匹配失败时的数据
        /// </summary>
        public class ResponseMismatchArgs : EventArgs
        {
            /// <summary>
            /// 数据的索引号：
            /// 在应答时用于Response与Request匹配
            /// </summary>
            public int DataIndex { get; set; }
            /// <summary>
            /// 数据对应的类型
            /// </summary>
            public TEnum DataType { get; set; }

            /// <summary>
            /// 谁的
            /// </summary>
            public string From { get; set; }

            /// <summary>
            /// 应答数据（Json字符串）
            /// </summary>
            public string ResponseData { get; set; }
        }

        /// <summary>
        /// 接收到数据处理出错时的信息
        /// </summary>
        public class ReceiveDataHandleFailedArgs : EventArgs
        {
            /// <summary>
            /// 数据来源
            /// </summary>
            public string From { get; set; }
            /// <summary>
            /// 具体的数据
            /// </summary>
            public string Data { get; set; }
            /// <summary>
            /// 错误
            /// </summary>
            public Exception Error { get; set; }
        }

        /// <summary>
        /// 发送完成时的信息
        /// </summary>
        public class SendCompleteArgs : XAsyncResult
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="exError_"></param>
            public SendCompleteArgs(Exception exError_ = null)
                : base(exError_)
            {
                ResponseData = null;
            }

            /// <summary>
            /// 应答数据：如果不需要应答，或发送失败则为null
            /// </summary>
            public XJsonDataFormat.DataWithType<TEnum> ResponseData { get; set; }
        }
    } // XNetCommunicate
}
