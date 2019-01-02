using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Common;
using SHCre.Xugd.CFile;

namespace SHCre.Xugd.MsgData
{
    /// <summary>
    /// 消息连接接口
    /// </summary>
    public interface IMsgConnection : ILogEventsBase
    {
        /// <summary>
        /// 定制标识
        /// </summary>
        object CustomTag {get;}

        /// <summary>
        /// 是否已登录（连接）
        /// </summary>
        bool IsLogged { get; }

        /// <summary>
        /// 登录用户名
        /// </summary>
        string UserName {get;}
        /// <summary>
        /// 远端信息（地址:端口）
        /// </summary>
        string RemoteAddress { get; }
        /// <summary>
        /// 近端地址
        /// </summary>
        string LocalAddress {get;}

        /// <summary>
        /// 连接成功
        /// </summary>
        event Action<IMsgConnection> OnConnected;

        /// <summary>
        /// 连接断开(Exception)：
        /// 如果Exception为NULL说明是正常退出，否则是由于出错断开
        /// </summary>
        event Action<IMsgConnection, Exception> OnDisconnected;
        
        /// <summary>
        /// 数据接收到的事件
        /// </summary>
        event Action<IMsgConnection, XMsgReceivedArgs> OnMsgReceived;

        /// <summary>
        /// 发送数据的接口：
        /// 发送成功Action中的Exception为null，否则为具体的错误信息
        /// </summary>
        /// <param name="strTopic_"></param>
        /// <param name="strJson_"></param>
        /// <param name="actComplete_"></param>
        /// <param name="oTag_"></param>
        void SendAsync(string strTopic_, string strJson_, Action<XAsyncResult> actComplete_, object oTag_);

        /// <summary>
        /// 订阅主题
        /// </summary>
        /// <param name="strTopic_"></param>
        /// <param name="actComplete_"></param>
        void Subscribe(string strTopic_, Action<XAsyncResult> actComplete_);

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="strTopic_"></param>
        /// <param name="actComplete_"></param>
        void Unsubscribe(string strTopic_, Action<XAsyncResult> actComplete_);

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

}
