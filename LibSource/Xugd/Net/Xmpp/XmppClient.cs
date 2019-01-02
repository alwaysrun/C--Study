using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Common;
using SHCre.Xugd.CFile;
using SHCre.Xugd.Extension;
using SHCre.Xugd.Data;

namespace SHCre.Xugd.Net.Xmpp
{
    /// <summary>
    /// 只能用于X86平台程序。
    /// Xmpp客户端库（可登录Openfire等jabber服务器），
    /// 依赖Library\xugd.xmpp.dll与xugd.clib.dll库
    /// </summary>
    public partial class XmppClient : XLogEventsBase, IXConnection
    {
        #region "Attributes"
        /// <summary>
        /// 是否已登录（连接）
        /// </summary>
        public bool IsLogged { get; private set;}

        /// <summary>
        /// 断开原因
        /// </summary>
        public XmppResultCode DisconnectError { get; private set; }

        /// <summary>
        /// 远端信息（地址:端口）
        /// </summary>
        public string RemoteAddress { get; private set; }

        /// <summary>
        /// 本地地址
        /// </summary>
        public string LocalAddress { get; private set; }

        /// <summary>
        /// 登录的用户
        /// </summary>
        public string UserJid
        {
            get { return _strSelfJid; }
        }
        #endregion

        #region "Events"
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
        /// 数据接收到的事件
        /// </summary>
        public event Action<XReceiveDataArgs> OnDataReceived;
        private bool InvokeOnDataReceived(XReceiveDataArgs argData_)
        {
            if (OnDataReceived != null)
                OnDataReceived(argData_);

            return true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public XmppClient():base()
        {
            IsLogged = false;
            DisconnectError = XmppResultCode.Success;

            LogPrefix = "XmppClient.";
            _quReceivedMsg = new XQueueIssue<XReceiveDataArgs>(InvokeOnDataReceived, 100, false);
            _quReceivedMsg.OnExcept += InvokeOnExcept;
            _quReceivedMsg.OnLogger += InvokeOnLogger;
            _quReceivedMsg.LogPrefix = "RevMsgQueue.";
        }

        /// <summary>
        /// 是否启用Debug输出（启用后以LogLevels.Debug等级输出OnLogger）
        /// </summary>
        /// <param name="bEnabled_"></param>
        public override void SetDebugEnabled(bool bEnabled_)
        {
            base.SetDebugEnabled(bEnabled_);
            _quReceivedMsg.SetDebugEnabled(bEnabled_);

            XmppImport.XmppSetLogLevel(_strSelfJid, bEnabled_ ? (int)XLogSimple.LogLevels.Debug : (int)XLogSimple.LogLevels.Info);
        }

        /// <summary>
        /// 发送数据的接口：
        /// 发送成功Action中的Exception为null，否则为具体的错误信息
        /// </summary>
        /// <param name="strTo_">接收者</param>
        /// <param name="strMsg_">消息</param>
        /// <param name="actComplete_"></param>
        /// <param name="oTag_"></param>
        public void SendAsync(string strTo_, string strMsg_, Action<XAsyncResult> actComplete_, object oTag_ = null)
        {
            if(!IsLogged){
                if(actComplete_ != null){
                    Exception ex = new XNotLoginException("has not login yet");
                    actComplete_(XAsyncResult.Get(ex));
                }
                return;
            }

            if (DebugEnabled)
            {
                InvokeOnDebug("SendAsync({0}:{1})", strTo_, XString.PrintLimit(strMsg_, 200));
            }

            byte[] bySend = Encoding.UTF8.GetBytes(strMsg_);
            XmppResultCode euCode = XmppImport.XmppSendByteMsg(_strSelfJid, BuildJid(strTo_), bySend, bySend.Length);
            if (actComplete_ != null)
            {
                Exception ex = null;
                if (euCode != XmppResultCode.Success)
                {
                    ex = new XNetConnException((uint)euCode, string.Format("{0} SendMsg to {1} failed {2}", _strSelfJid, strTo_, euCode));
                    InvokeOnLogger(XLogSimple.LogLevels.Error, "XmppImport.XmppSendByteMsg({0}->{1}) failed: {2}", _strSelfJid, strTo_, euCode);
                }
                actComplete_(XAsyncResult.Get(ex));
            }
        }

        /// <summary>
        /// 异步登录（连接）
        /// </summary>
        /// <param name="actComplete_"></param>
        public void LoginAsync(Action<XAsyncResult> actComplete_){
            XThread.StartThread(() =>
            {
                Exception exRet = null;
                try{
                    LoginSync();
                }
                catch(Exception ex){
                    exRet = ex;
                }
                if (actComplete_ != null)
                    actComplete_(XAsyncResult.Get(exRet));
            });
        }

        /// <summary>
        /// 登出（断开连接）
        /// </summary>
        /// <param name="actComplete_"></param>
        public void LogoutAsync(Action<XAsyncResult> actComplete_){
            XThread.StartThread(() =>
                {
                    Exception exRet = null;
                    try{
                        LogoutSync();
                    }
                    catch(Exception ex){
                        exRet = ex;
                    }

                    if (actComplete_ != null)
                        actComplete_(XAsyncResult.Get(exRet));
                });
        }
    }
}
