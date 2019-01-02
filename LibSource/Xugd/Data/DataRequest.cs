using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHCre.Xugd.Data
{
    /// <summary>
    /// 请求的基类
    /// </summary>
    public class XDataRequestBase
    {
    }

    /// <summary>
    /// 应答接口
    /// </summary>
    public interface IXDataResponse
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        /// <returns></returns>
        bool IsSuccess();
    }

    /// <summary>
    /// 应答的基类：
    /// 默认Error为1时IsSuccess返回true；否则返回false。
    /// </summary>
    /// <typeparam name="TRet"></typeparam>
    public class XDataResponseBase<TRet> : IXDataResponse where TRet : IConvertible
    {
        /// <summary>
        /// 是否成功：
        /// 默认1为成功，其他为失败
        /// </summary>
        public virtual bool IsSuccess()
        {
            return Convert.ToInt32(Result) == 1;
        }

        /// <summary>
        /// 如果出错，为出错说明
        /// </summary>
        public string Cause { get; set; }
        /// <summary>
        /// 错误号
        /// </summary>
        public TRet Result { get; set; }
    }

    /// <summary>
    /// 事件的基类：一般事件是不需要应答的
    /// </summary>
    public class XDataEventBase : XDataRequestBase
    {
    }

    /// <summary>
    /// 消息请求
    /// </summary>
    public class XMsgDataRequest : XDataRequestBase
    {
        /// <summary>
        /// 消息属性(Name:Value)
        /// </summary>
        public Dictionary<string,string> Property { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int MsgType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 获取属性值，如果不存在返回null
        /// </summary>
        /// <param name="strName_"></param>
        /// <returns></returns>
        public string GetPropertyValue(string strName_)
        {
            string strValue;
            Property.TryGetValue(strName_, out strValue);
            return strValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public XMsgDataRequest()
        {
            Property = new Dictionary<string, string>();
        }
    }

    /// <summary>
    /// 消息应答基类
    /// </summary>
    /// <typeparam name="TError"></typeparam>
    public class XMsgDataResponse<TError> : XDataResponseBase<TError> where TError : IConvertible
    {
    }
}
