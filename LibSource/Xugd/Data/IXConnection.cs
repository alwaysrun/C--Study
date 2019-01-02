using System;
using SHCre.Xugd.Common;
using System.Collections.Generic;

namespace SHCre.Xugd.Data
{
    /// <summary>
    /// 通讯库的接口约束
    /// </summary>
    public interface IXConnection
    {
        /// <summary>
        /// 是否已登录（连接）
        /// </summary>
        bool IsLogged { get; }

        /// <summary>
        /// 远端信息（地址:端口）
        /// </summary>
        string RemoteAddress { get; }

        /// <summary>
        /// 连接成功
        /// </summary>
        event Action OnConnected;

        /// <summary>
        /// 连接断开(IsClose)：
        /// 如果IsClosed为true说明是退出，否则是由于出错断开
        /// </summary>
        event Action<bool> OnDisconnected;

        ///// <summary>
        ///// 当遇到错误时，激发的事件
        ///// </summary>
        //event Action<Exception> OnError;

        /// <summary>
        /// 数据接收到的事件
        /// </summary>
        event Action<XReceiveDataArgs> OnDataReceived;

        /// <summary>
        /// 发送数据的接口：
        /// 发送成功Action中的Exception为null，否则为具体的错误信息
        /// </summary>
        /// <param name="strTo"></param>
        /// <param name="strJson_"></param>
        /// <param name="actComplete_"></param>
        /// <param name="oTag_"></param>
        void SendAsync(string strTo, string strJson_, Action<XAsyncResult> actComplete_, object oTag_);        

        /// <summary>
        /// 异步登录（连接）
        /// </summary>
        /// <param name="actComplete_"></param>
        void LoginAsync(Action<XAsyncResult> actComplete_);
        
        /// <summary>
        /// 登出（断开连接）
        /// </summary>
        /// <param name="actComplete_"></param>
        void LogoutAsync(Action<XAsyncResult> actComplete_);
    }

    /// <summary>
    /// 接收到的数据类型
    /// </summary>
    public enum XReceiveDataType 
    {
        /// <summary>
        /// 普通数据
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 发送失败返回的数据
        /// </summary>
        SendBack,
    }

    /// <summary>
    /// 接收到的数据信息
    /// </summary>
    public class XReceiveDataArgs : EventArgs
    {
        /// <summary>
        /// 发送者（对于SendBack，也要设为对方，即发送时的接收者）
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// 数据的类型
        /// </summary>
        public XReceiveDataType Type {get;set;}
        /// <summary>
        /// 具体接收到的数据
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// 属性，通过AddProperty添加
        /// </summary>
        public Dictionary<string, object> Property { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strKey_"></param>
        /// <param name="oValue_"></param>
        public void AddProperty(string strKey_, object oValue_)
        {
            if (Property == null)
                Property = new Dictionary<string, object>();
            Property[strKey_] = oValue_;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strKey_"></param>
        /// <returns></returns>
        public object GetProperty(string strKey_)
        {
            object oValue = null;
            if (Property != null)
                Property.TryGetValue(strKey_, out oValue);

            return oValue;
        }
    }

}
