using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.CFile;
using SHCre.Xugd.Common;
using System.Threading;
using System.Runtime.InteropServices;
using SHCre.Xugd.Extension;
using SHCre.Xugd.Data;

namespace SHCre.Xugd.Net.Xmpp
{
    partial class XmppClient
    {
        string _strSelfJid;

        #region "Del function"
        XmppConfig _conXmpp = null;
        //XmppImport.DelOnLogin _delLogin;
        XmppImport.DelOnClose _delClose;
        XmppImport.DelOnPresence _delPresence;
        XmppImport.DelOnMsgReceive _delMsgReceived;

        void OnLogin(string strUserJid_)
        {
            InvokeOnLogger("Login OK", XLogSimple.LogLevels.Notice);
            DisconnectError = XmppResultCode.Success;
            IsLogged = true;

            _quReceivedMsg.Restart();
            InvokeConnected();
        }

        void OnClose(string strUserJid_, XmppResultCode euErr_)
        {
            InvokeOnLogger(XLogSimple.LogLevels.Notice, "Logout(Reason:{0})", euErr_);

            DisconnectError = euErr_;
            IsLogged = false;

            _quReceivedMsg.Clear();
            _quReceivedMsg.Stop();
            InvokeDisconnected(euErr_ == XmppResultCode.UserDisconnected);
        }

        void OnPresence(string strUserJid_, string strFriendJid_, XmppPresenceType euType_)
        {
            InvokeOnDebug("OnPresence(Friend:{0}[{1}])", strFriendJid_, euType_);

            XThread.StartPool((object none_) =>
            {
                if (euType_ > XmppPresenceType.Chat)
                    InvokeOnFriendLogout(GetNameFromJid(strFriendJid_));
                else
                    InvokeOnFriendLogin(GetNameFromJid(strFriendJid_));
            });
        }

        XReceiveDataType MsgType2DataType(XmppMsgType euType_)
        {
            if (XFlag.CheckAny(euType_, XmppMsgType.Error))
                return XReceiveDataType.SendBack;

            return XReceiveDataType.Normal;
        }

        void OnMsgReceive(string strUserJid_, string strFrom_, IntPtr pMsg_, int nCount_, XmppMsgType euType_)
        {
            if (XFlag.CheckAny(euType_, XmppMsgType.Invalid))
            {
                InvokeOnExcept(new XDataException(string.Format("Msg({0}->{1}) is Invalid type", strFrom_, strUserJid_)));
                return;
            }

            byte[] byRev = new byte[nCount_];
            Marshal.Copy(pMsg_, byRev, 0, nCount_);
            string strMsg = Encoding.UTF8.GetString(byRev);
            if (DebugEnabled)
            {
                InvokeOnDebug("OnMsgReceive({0}[{2}]:{1})", strFrom_, XString.PrintLimit(strMsg, 200), euType_);
            }
            QueueReceivedMsg(GetNameFromJid(strFrom_), GetNameFromJid(strUserJid_), strMsg, MsgType2DataType(euType_));
        }
        #endregion

        #region "Msg handle"
        XQueueIssue<XReceiveDataArgs> _quReceivedMsg = null;

        void QueueReceivedMsg(string strFromJid_, string strToJid_, string strData_, XReceiveDataType euType_)
        {
            XReceiveDataArgs msgInfo = new XReceiveDataArgs()
            {
                From = strFromJid_,
                Type = euType_,
                Data = strData_,
            };
            _quReceivedMsg.AddItem(msgInfo);
        }
        #endregion

        string GetNameFromJid(string strJid_){
            var aryName = strJid_.Split('@');
            return aryName[0];
        }

        string BuildJid(string strName_)
        {
            //if (strName_.Contains('@'))
            //    throw new ArgumentException("Name can not include domain(@)");

            return _conXmpp.LoginInfo.GetUserJid(strName_.ToLower());
        }

        /// <summary>
        /// 设定服务端信息
        /// </summary>
        /// <param name="conXmpp_"></param>
        public void SetServerInfo(XmppConfig conXmpp_)
        {
            _conXmpp = conXmpp_;
            _conXmpp.LoginInfo.UserName = conXmpp_.LoginInfo.UserName.ToLower();
            _conXmpp.LoginInfo.Domain = conXmpp_.LoginInfo.Domain.ToLower();
            XFile.CheckFileExist("Library\\xugd.xmpp.dll", "Library\\xugd.clib.dll");
        }

        /// <summary>
        /// 同步登录服务端，登录完成时会触发OnConnected事件（此事件返回后，LoginSync才返回）
        /// </summary>
        public void LoginSync()
        {
            if (_conXmpp == null)
                throw new XNetException("XmppServer config not set, Call SetServerInfo first");

            LogPrefix = "XmppClient.";
            InvokeOnLogger(XLogSimple.LogLevels.Info, "#LoginSync(Addr:{0}:{1}, User:{2})",
                _conXmpp.LoginInfo.Address, _conXmpp.LoginInfo.Port, _conXmpp.LoginInfo.UserName);

            string strLog = string.Empty;
            if (!string.IsNullOrEmpty(_conXmpp.LogConf.NameFormat))
                strLog = XPath.GetFullPath(_conXmpp.LogConf.NameFormat);
            XmppResultCode euCode = XmppImport.XmppLogin(_conXmpp.LoginInfo.GetUserJid(), _conXmpp.LoginInfo.Password,
                _conXmpp.LoginInfo.Address, _conXmpp.LoginInfo.Port, strLog);
            if (euCode != XmppResultCode.Success)
            {
                if (euCode != XmppResultCode.HasLogin)
                {
                    InvokeOnLogger(XLogSimple.LogLevels.Error, "XmppImport.XmppLogin(User:{0}, Addr:{1}) fail: {2}", _conXmpp.LoginInfo.UserName, _conXmpp.LoginInfo.PrintAddr(), euCode);
                    throw new XNetConnException((uint)euCode,
                        string.Format("LoginSyn(User:{0}, Addr:{1}): XmppLogin failed {2}", _conXmpp.LoginInfo.UserName, _conXmpp.LoginInfo.PrintAddr(), euCode));
                }
                // Has login, do more...
            }

            _strSelfJid = _conXmpp.LoginInfo.GetUserJid();
            if (_conXmpp.LogConf.LogLevel != XLogSimple.LogLevels.Info && !string.IsNullOrEmpty(strLog))
                XmppImport.XmppSetLogLevel(_strSelfJid, (int)_conXmpp.LogConf.LogLevel);

            ToCheckDomain();
            RemoteAddress = _conXmpp.LoginInfo.PrintAddr();
            LocalAddress = XmppImport.GetLocalAddr(_strSelfJid);

            LogPrefix = string.Format("XmppClient({0}).", _strSelfJid);
            // 注册回调
            //_delLogin = new XmppImport.DelOnLogin(OnLogin);
            //XmppImport.XmppSetCallbackLogin(_strSelfJid, _delLogin);
            _delClose = new XmppImport.DelOnClose(OnClose);
            XmppImport.XmppSetCallbackClose(_strSelfJid, _delClose);
            _delPresence = new XmppImport.DelOnPresence(OnPresence);
            XmppImport.XmppSetCallbackPresence(_strSelfJid, _delPresence);
            _delMsgReceived = new XmppImport.DelOnMsgReceive(OnMsgReceive);
            XmppImport.XmppSetCallbackMsgReceive(_strSelfJid, _delMsgReceived);
            OnLogin(_strSelfJid);
        }

        private void ToCheckDomain()
        {
            string strStream = XmppImport.GetStartStream(_strSelfJid);
            InvokeOnLogger(strStream);

            int nFromOffset = strStream.IndexOf("from=");
            if (nFromOffset == -1)
            { // error
                return;
            }

            int nValueStart = nFromOffset + 6;
            int nValueEnd = nValueStart + 1;
            while (strStream[nValueEnd] != '\'' && nValueEnd<strStream.Length)
                ++nValueEnd;
            string strDomain = strStream.Substring(nValueStart, nValueEnd - nValueStart);
            if (string.Compare(strDomain, _conXmpp.LoginInfo.Domain, true) != 0)
            {
                XmppImport.XmppClose(_strSelfJid);
                string strInfo = string.Format("Openfire domain is [{0}], but local set [{1}]",
                    strDomain, _conXmpp.LoginInfo.Domain);
                InvokeOnLogger(strInfo, XLogSimple.LogLevels.Error);
                throw new ArgumentException(strInfo);
            }
        }

        /// <summary>
        /// 同步登出，登出完成时会触发OnDisconnected事件（此事件返回后LogoutSync才返回）
        /// </summary>
        public void LogoutSync()
        {
            InvokeOnLogger("#LogoutSync");
            IsLogged = false;
            //XmppImport.XmppSetCallbackLogin(_strSelfJid, null);
            XmppImport.XmppSetCallbackClose(_strSelfJid, null);
            XmppImport.XmppSetCallbackPresence(_strSelfJid, null);
            XmppImport.XmppSetCallbackMsgReceive(_strSelfJid, null);

            XmppImport.XmppClose(_strSelfJid);
            OnClose(_strSelfJid, XmppResultCode.UserDisconnected);
        }

        /// <summary>
        /// 订阅对方状态信息（会自动添加对方为好友）
        /// </summary>
        /// <param name="strFriend_"></param>
        public void Subscribe(string strFriend_)
        {
            if (!IsLogged)
                throw new XNotLoginException("has not login yet");

            XmppResultCode euCode = XmppImport.XmppSubscribe(_strSelfJid, BuildJid(strFriend_));
            if (euCode != XmppResultCode.Success)
                throw new XNetConnException((uint)euCode, string.Format("{0} subscribe {1} failed {2}", _strSelfJid, strFriend_, euCode));
        }

        /// <summary>
        /// 取消订阅（从好友列表中移除）
        /// </summary>
        /// <param name="strFriend_"></param>
        public void Unsubscribe(string strFriend_)
        {
            if (!IsLogged)
                throw new XNotLoginException("has not login yet");

            XmppResultCode euCode = XmppImport.XmppUnsubscribe(_strSelfJid, BuildJid(strFriend_));
            if (euCode != XmppResultCode.Success)
                throw new XNetConnException((uint)euCode, string.Format("{0} unsubscribe {1} failed {2}", _strSelfJid, strFriend_, euCode));
        }

        /// <summary>
        /// 设定记录等级
        /// </summary>
        /// <param name="euLevel_"></param>
        public void SetLogLevel(XLogSimple.LogLevels euLevel_)
        {
            if (!IsLogged)
                throw new XNotLoginException("has not login yet");

            XmppImport.XmppSetLogLevel(_strSelfJid, (int)euLevel_);
        }
    }
}
