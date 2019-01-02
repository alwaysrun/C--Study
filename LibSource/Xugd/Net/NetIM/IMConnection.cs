using System;
using System.Security.Authentication;
using Icld.NetIM.Client;
using Icld.NetIM.Client.Xmpp;
using System.IO;
using SHCre.Xugd.Data;

namespace SHCre.Xugd.Net
{
    /// <summary>
    /// 即时通讯连接:
    /// 注册OnDataReceived事件获取返回信息
    /// SetServerInfo();
    /// LoginAsync();
    /// SendAsync
    /// ...
    /// LogoutAsync()
    /// </summary>
    [Obsolete("Use SHCre.Xugd.Net.Xmpp.XmppClient replace this")]
    public partial class XIMConnection : IXConnection
    {
        private XmppChat _imChat;
        private XmppConnection _imConnection;

        /// <summary>
        /// 当遇到错误时，激发的事件（登录、登出时错误不会触发此事件；需要在登录登出接口中获取）
        /// </summary>
        public event Action<Exception> OnError;
        private void InvokeOnError(Exception ex_)
        {
            if (OnError != null)
                OnError(ex_);
        }

        /// <summary>
        /// 连接成功
        /// </summary>
        public event Action OnConnected;
        void InvokeConnected()
        {
            if (OnConnected != null)
                OnConnected();
        }

        /// <summary>
        /// 当连接断开时(IsClose)（无论是主动、还是被动断开，都会触发此事件）：
        /// 如果IsClosed为true说明是退出，否则是由于出错断开
        /// </summary>
        public event Action<bool> OnDisconnected;
        private void InvokeDisconnected(bool bIsClose_)
        {
            ClearFriends();
            if (OnDisconnected != null)
                OnDisconnected(bIsClose_);
        }

        /// <summary>
        /// 通过SetServerInfo设定要连接服务端的信息；
        /// 然后通过LoginSync/LoginAsync登录服务端
        /// </summary>
        public XIMConnection()
        {
            _imConnection = new XmppConnection();
            _imConnection.OnLogin += new Action(IMConnection_OnLogin);
            _imConnection.OnClose += new Action(IMConnection_OnClose);
            _imConnection.OnAuthenticationFailure += new Action<string>(IMConnection_OnAuthenticationFailure);
            _imConnection.OnPresence += new Action<Presence>(IMConnection_OnPresence);
            _imConnection.OnMessage += new Action<Message>(IMConnection_OnMessage);
            _imConnection.OnError += new Action<Exception>(IMConnection_OnError);
            _imConnection.OnConnectError += new Action<Exception>(IMConnection_OnConnectError);

            _imChat = new XmppChat(_imConnection);
        }

        #region "IM Connect Event"
        void IMConnection_OnConnectError(Exception exError_)
        {
            if (!HandleErrorWhenLoginout(exError_))
                InvokeOnError(exError_);
        }

        void IMConnection_OnError(Exception exError_)
        {
            if (!HandleErrorWhenLoginout(exError_))
            {
                if (exError_ is IOException)
                {
                    _bHasLogged.Set(false);
                    InvokeDisconnected(false);
                }
                InvokeOnError(exError_);
            }
        }

        void IMConnection_OnMessage(Message msgReceive_)
        {
            InvokeOnDataReceived(msgReceive_.From, msgReceive_.To, msgReceive_.Body,
                (msgReceive_.MessageType == MessageType.Error) ? XReceiveDataType.SendBack : XReceiveDataType.Normal);
        }

        void IMConnection_OnPresence(Presence preUser_)
        {
            FriendPresenceChanged(preUser_.From, preUser_.PresenceType);
        }

        void IMConnection_OnAuthenticationFailure(string obj)
        {
            HandleErrorWhenLoginout(new AuthenticationException(string.Format("User {0} authenticate failed when login", UserName)));
        }

        void IMConnection_OnClose()
        {
            HandleLoginout(false);
            InvokeDisconnected(true);
        }

        void IMConnection_OnLogin()
        {
            HandleLoginout(true);
            InvokeConnected();
        }
        #endregion
    }
}
