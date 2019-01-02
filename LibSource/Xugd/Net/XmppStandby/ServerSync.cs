using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SHCre.Xugd.Common;
using SHCre.Xugd.Extension;
using SHCre.Xugd.CFile;
using SHCre.Xugd.Data;

namespace SHCre.Xugd.Net.Xmpp
{
    partial class XStandbyServer<TEnum>
    {
        XmppClient _syncClient = null;
        XDataComm<TEnum> _syncComm = null;
        string _strRemoteSrvJid = string.Empty;  // This sync connect remote Server
        string _strRemoteSynJid = string.Empty;  // Remote sync connect This Server
        int _nResponseTimeout = 0;
        bool _isSyncStart = false;
        XTimerWork<XmppClient> _synReconnectWork = new XTimerWork<XmppClient>(60);

        /// <summary>
        /// 同步用户登录（远端的Sync用户登录到本地）
        /// </summary>
        public event Action<string> OnSyncUserLogin;
        void InvokeSyncLogin(string strUser_)
        {
            if (_strRemoteSynJid != strUser_)
            {
                _strRemoteSynJid = strUser_;
                TrySubscribe(strUser_);

                if (OnSyncUserLogin != null)
                    OnSyncUserLogin(strUser_);
            }
        }

        /// <summary>
        /// 同步用户退出（远端的Sync用户从本地退出）
        /// </summary>
        public event Action<string> OnSyncUserLogout;
        bool InvokeSyncLogout(string strUser){
            if(_strRemoteSynJid == strUser){
                _strRemoteSynJid = string.Empty;
                SetMasterServer(true);

                if (OnSyncUserLogout != null)
                    OnSyncUserLogout(strUser);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 连接到远端服务器的同步用户,连接成功时触发
        /// </summary>
        public event Action<XDataComm<TEnum>> OnSyncConnect;
        void InvokeSyncConnect()
        {
            if (OnSyncConnect != null)
                OnSyncConnect(_syncComm);
        }

        /// <summary>
        /// 连接到远端服务器的同步用户,断开时触发
        /// </summary>
        public event Action<XDataComm<TEnum>> OnSyncDisconnect;
        void InvokeSyncDisconnect(XDataComm<TEnum> synComm_)
        {
            if (OnSyncDisconnect != null && synComm_ != null)
                OnSyncDisconnect(synComm_);
        }

        void ClearSyncUser(){
            _strRemoteSynJid = string.Empty;
        }

        bool IsSyncUser(string strUser_){
            return _strRemoteSynJid == strUser_;
        }

        /// <summary>
        /// 是否已登录热备
        /// </summary>
        /// <returns></returns>
        public bool HasLoggedSyn(){
            return _syncClient!=null && _syncClient.IsLogged && !string.IsNullOrEmpty(_strRemoteSynJid);
        }

        /// <summary>
        /// 当前同步用户数（无热备返回0，否则返回1）
        /// </summary>
        /// <returns></returns>
        public int GetSyncCount()
        {
            return _conServer.SyncServer.Enabled ? 1 : 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conSync_"></param>
        /// <returns>Is-Master return true, else false</returns>
        void StartSyncSrv(XStandbyUserConfig conSync_)
        {
            InvokeOnLogger("StartSyncSrv");

            _isSyncStart = false;
            _strRemoteSrvJid = string.Empty; 
            if (_syncClient != null)
            {
                _syncClient.LogoutSync();
                _syncClient = null;
            }
            _synReconnectWork.Stop();

            if (!conSync_.Enabled)
            {
                _bIsMasterServer = true;
                return;
            }            

            _syncClient = new XmppClient();
            _syncClient.SetServerInfo(conSync_.XmppServer);
            _syncClient.OnConnected += new Action(syncClient_OnConnected);
            _syncClient.OnDisconnected += new Action<bool>(syncClient_OnDisconnected);
            _syncClient.OnFriendLogin += new Action<string>(syncClient_OnFriendLogin);
            _syncClient.OnFriendLogout += new Action<string>(syncClient_OnFriendLogout);
            TryAddEvent(_syncClient);

            if (conSync_.ResponseTimeoutSecond < 5)
                conSync_.ResponseTimeoutSecond = 15;
            _syncComm = new XDataComm<TEnum>(_syncClient);
            _syncComm.Start();
            _syncComm.ResponseWaitSeconds = conSync_.ResponseTimeoutSecond;
            _syncComm.LogPrefix = "SynComm.";
            TryAddEvent(_syncComm);

            _synReconnectWork.SetWorkInfo(ToConnectSync, _syncClient);
            _synReconnectWork.OnExcept -= SynNet_OnExcept;
            _synReconnectWork.OnLogger -= SynNet_OnLogger;
            _synReconnectWork.LogPrefix = "SynReconnect.";
            TryAddEvent(_synReconnectWork);

            // to login sync
            _strRemoteSrvJid = conSync_.ServerName.ToLower();
            _nResponseTimeout = XTime.Second2Interval(conSync_.ResponseTimeoutSecond);

            bool bMaster = true;
            _isSyncStart = true;
            try
            {
                _syncClient.LoginSync();
                var srvData = QueryMasterServer();
                if (srvData != null)
                    bMaster = !srvData.IsMaster;
            }
            catch(Exception ex){
                InvokeOnExcept(ex, "SynLogin");
            }

            _bIsMasterServer = bMaster;
        }

        void TryAddEvent(XLogEventsBase logBase_)
        {
            logBase_.OnExcept += SynNet_OnExcept;
            logBase_.OnLogger += SynNet_OnLogger;

            if (DebugEnabled)
                logBase_.SetDebugEnabled(true);
        }
        void SynNet_OnLogger(string strInfo_, CFile.XLogSimple.LogLevels euLevel_)
        {
            InvokeOnLogger(string.Format("Syn-{0}", strInfo_), euLevel_);
        }

        void SynNet_OnExcept(Exception ex_, string strInfo_)
        {
            InvokeOnExcept(ex_, string.Format("Syn-{0}", strInfo_));
        }

        void StopSyncSrv(){
            InvokeOnLogger("StopSyncSrv");

            _synReconnectWork.Stop();
            _isSyncStart = false;
            if (_syncClient != null)
            {
                bool bInvoke = _syncClient.IsLogged;
                _syncClient.OnConnected -= new Action(syncClient_OnConnected);
                _syncClient.OnDisconnected -= new Action<bool>(syncClient_OnDisconnected);
                _syncClient.OnFriendLogin -= new Action<string>(syncClient_OnFriendLogin);
                _syncClient.OnFriendLogout -= new Action<string>(syncClient_OnFriendLogout);
                _syncClient.LogoutSync();
                _syncClient = null;

                var synDiscon = _syncComm;
                if (_syncComm != null)
                {
                    _syncComm.Stop();
                    _syncComm = null;
                }

                _strRemoteSrvJid = string.Empty;
                if(bInvoke)
                {
                    InvokeSyncDisconnect(synDiscon);
                }
            }
        }

        void RemoteServerDisconnect(){
            SetMasterServer(true);
        }

        void TryReconnect(){
            if (_isSyncStart)
                _synReconnectWork.Start(false);
        }

        private XTimerWorkResult ToConnectSync(XmppClient xmClient_)
        {
            if (!xmClient_.IsLogged)
                xmClient_.LoginSync();

            bool bSet = false;
            var srvData = QueryMasterServer();
            if (srvData == null || !srvData.IsMaster)
                bSet = SetMasterServer(true);

            if (IsMasterServer && !bSet)
            {
                SendMasterChange2User(true);  // Declare master periodic
            }

            return XTimerWorkResult.Restart;
        }

        IMServerMasterData QueryMasterServer()
        {
            if (!_isSyncStart || !_syncClient.IsLogged)
                return null;

            IMServerMasterData queryMaster = new IMServerMasterData()
            {
                IsMaster = IsMasterServer,
                SrvID = LocalServerID,
                StartTime = ServerStartTime,
            };

            Exception exError = null;
            IMServerMasterData srvData = null;
            ManualResetEvent evtWait = new ManualResetEvent(false);
            _syncComm.SendAsyn(_strRemoteSrvJid, ServerMasterCode, queryMaster, (zop) =>
            {
                if (zop.IsSuccess)
                {
                    try
                    {
                        srvData = zop.ResponseData.GetData<IMServerMasterData>();
                    }
                    catch (Exception ex)
                    {
                        exError = ex;
                    }
                }
                else
                {
                    exError = zop.Result;
                }
                evtWait.Set();
            },
            true);

            evtWait.WaitOne(_nResponseTimeout);
            if (exError != null)
                InvokeOnExcept(exError, "SynQueryMaster");

            return srvData;
        }

        void syncClient_OnFriendLogout(string obj)
        {
            if (_strRemoteSrvJid == obj)
                RemoteServerDisconnect();
        }

        void syncClient_OnFriendLogin(string obj)
        {
            if (_strRemoteSrvJid == obj)
            {
                if (IsServerStarted)
                {
                    _synReconnectWork.Start(true);
                }
            }
        }

        void syncClient_OnDisconnected(bool obj)
        {
            RemoteServerDisconnect();
            InvokeSyncDisconnect(_syncComm);
        }

        void syncClient_OnConnected()
        {
            InvokeSyncConnect();
        }
    }
}
