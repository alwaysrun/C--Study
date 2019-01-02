using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SHCre.Xugd.CFile;
using SHCre.Xugd.Common;
using SHCre.Xugd.Data;

namespace SHCre.Xugd.Net.Xmpp
{
    /// <summary>
    /// 热备服务端信息,只能用于X86平台程序：
    /// 依赖Library\xugd.xmpp.dll与xugd.clib.dll库，
    /// 先调用SetServerInfo设定xmpp服务器配置信息，
    /// 然后调用Start来启动
    /// </summary>
    /// <typeparam name="TEnum">命令码类型</typeparam>
    public partial class XStandbyServer<TEnum> : XLogEventsBase where TEnum : IComparable
    {
        #region "Vars"
        XStandbyServerConfig _conServer = null;

        /// <summary>
        /// 服务器启动时间
        /// </summary>
        public DateTime ServerStartTime { get; private set; }

        /// <summary>
        /// 是否已启动
        /// </summary>
        public bool IsServerStarted { get; private set; }

        bool _bIsMasterServer = false;

        bool SetMasterServer(bool isMaster)
        {
            if (!IsServerStarted || (_bIsMasterServer == isMaster))
                return false;

            _bIsMasterServer = isMaster;
            InvokeMasterChanged();
            return true;
        }

        /// <summary>
        /// 是否是主服务器
        /// </summary>
        public bool IsMasterServer
        {
            get { return _bIsMasterServer; }
        }

        /// <summary>
        /// 用于客户端登录的命令
        /// </summary>
        public TEnum ClientLoginCode { get; private set; }
        /// <summary>
        /// 主服务器变化的命令
        /// </summary>
        public TEnum ServerMasterCode { get; private set; }

        /// <summary>
        /// 服务器标识（多个服务器时，要保证唯一）
        /// </summary>
        public string LocalServerID { get; private set; }

        XmppClient _srvDataClient = null;
        /// <summary>
        /// 对应的底层通讯类
        /// </summary>
        public XDataComm<TEnum> SrvDataComm { get; private set; }
        #endregion

        #region "Event"
        /// <summary>
        /// 主服务器变化时触发
        /// </summary>
        public event Action<bool> OnMasterChanged;
        void InvokeMasterChanged()
        {
            bool bMaster = _bIsMasterServer;
            if (IsServerStarted && bMaster)
            {
                SendMasterChange2User(bMaster);
            }

            if (OnMasterChanged != null)
                OnMasterChanged(bMaster);
        }

        /// <summary>
        /// 连接断开时触发(IsClosed):
        /// 如果IsClosed为true说明是退出，否则是由于出错断开
        /// </summary>
        public event Action<bool> OnDisconnected;
        void InvokeDisconnected(bool bClose_)
        {
            ClearClientUser();
            ClearSyncUser();
            if (OnDisconnected != null)
                OnDisconnected(bClose_);
        }

        /// <summary>
        /// 数据到达时，触发OnRequestArrival(Request,IsSync)：
        /// 如果IsSync是true，说明是其他服务器上的同步用户发送过来的数据；
        /// 否则说明是客户端发送过来的数据（不能阻塞，所有Openfire相关事件使用同一个线程）
        /// </summary>
        public event Action<XDataComm<TEnum>.RequestArrivalArgs, bool> OnRequestArrival;
        void InvokeRequestArrival(XDataComm<TEnum>.RequestArrivalArgs arg_)
        {
            if (OnRequestArrival != null)
            {
                OnRequestArrival(arg_, IsSyncUser(arg_.From));
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="euLoginCode_">登录命令码</param>
        /// <param name="euSrvMaster_">主服务器变化时的命令码</param>
        public XStandbyServer(TEnum euLoginCode_, TEnum euSrvMaster_)
        {
            ClientLoginCode = euLoginCode_;
            ServerMasterCode = euSrvMaster_;

            BuildLogPrefix<TEnum>("XStandbyServer");

            SrvDataComm = null;
        }

        /// <summary>
        /// 是否启用Debug输出（启用后以LogLevels.Debug等级输出OnLogger）
        /// </summary>
        /// <param name="bEnabled_"></param>
        public override void SetDebugEnabled(bool bEnabled_)
        {
            base.SetDebugEnabled(bEnabled_);

            if (SrvDataComm != null)
            {
                SrvDataComm.SetDebugEnabled(bEnabled_);
                var netCon = SrvDataComm.NetComm as XLogEventsBase;
                if (netCon != null)
                    netCon.SetDebugEnabled(bEnabled_);
            }

            // Sync
            if (_syncClient != null)
                _syncClient.SetDebugEnabled(bEnabled_);
            if (_syncComm != null)
                _syncComm.SetDebugEnabled(bEnabled_);
            if (_synReconnectWork != null)
                _synReconnectWork.SetDebugEnabled(bEnabled_);
        }

        /// <summary>
        /// 设定热备服务器配置信息
        /// </summary>
        /// <param name="srvConf_"></param>
        public void SetServerInfo(XStandbyServerConfig srvConf_)
        {
            XStandbyUserConfig synConf = srvConf_.SyncServer;
            if (synConf.Enabled)
            {
                if (synConf.XmppServer.LoginInfo.Address == srvConf_.LocalServer.LoginInfo.Address
                    && synConf.XmppServer.LoginInfo.UserName == srvConf_.LocalServer.LoginInfo.UserName)
                    throw new ArgumentException("Standby server-info cannot equal sync-info");

                InvokeOnLogger(XLogSimple.LogLevels.Info, "SetServerInfo: Server(User:{0}, Addr:{1}); Sync(User:{2}, Addr:{3})",
                    srvConf_.LocalServer.LoginInfo.UserName, srvConf_.LocalServer.LoginInfo.Address,
                    synConf.XmppServer.LoginInfo.UserName, synConf.XmppServer.LoginInfo.Address);
            }
            else
            {
                InvokeOnLogger(XLogSimple.LogLevels.Info, "SetServerInfo: Server(User:{0}, Addr:{1}) and no Sync",
                                    srvConf_.LocalServer.LoginInfo.UserName, srvConf_.LocalServer.LoginInfo.Address);
            }

            _conServer = srvConf_;
            LocalServerID = _conServer.SrvId;
            if (string.IsNullOrEmpty(LocalServerID))
                LocalServerID = _conServer.LocalServer.LoginInfo.Address;
        }

        /// <summary>
        /// 启动：返回前一定会触发OnMasterChanged的事件，失败抛出异常；
        /// 正在登录时，不能再次调用，否则结果未知
        /// </summary>
        public void Start()
        {
            InvokeOnLogger("#Start");
            if (_conServer == null)
                throw new ArgumentException("Server config not set, call SetServerInfo first");

            IsServerStarted = false;
            _bIsMasterServer = false;
            ClearSyncUser();
            ClearClientUser();

            ServerStartTime = DateTime.Now;
            StartSyncSrv(_conServer.SyncServer);
            InvokeOnLogger(XLogSimple.LogLevels.Info, "Start-StartSyncSrv finished, {0}", _bIsMasterServer ? "IsMaster" : "NotMaster");

            // Login server
            if (_srvDataClient != null)
            {
                if (SrvDataComm != null)
                {
                    SrvDataComm.Stop();
                }

                _srvDataClient.LogoutSync();
                _srvDataClient = null;
            }
            try
            {
                LoginServer();
            }
            catch (Exception ex)
            {
                InvokeOnExcept(ex);

                StopSyncSrv();
                throw ex;
            }

            // Reconnect sync
            TryReconnect();

            InvokeOnLogger(XLogSimple.LogLevels.Info, "Start finished, {0}", _bIsMasterServer ? "IsMaster" : "NotMaster");
            if (!_bIsMasterServer) // Reset, to avoid change-over when start at almost same time
                ServerStartTime = DateTime.Now;
            IsServerStarted = true;
            InvokeMasterChanged();
        }

        /// <summary>
        /// 停止：返回前一定会触发OnMasterChanged的事件
        /// </summary>
        public void Stop()
        {
            InvokeOnLogger("#Stop");
            IsServerStarted = false;
            _bIsMasterServer = false;

            StopSyncSrv();

            if (_srvDataClient != null)
            {
                _srvDataClient.OnDisconnected -= new Action<bool>(srvDataClient_OnDisconnected);
                _srvDataClient.OnFriendLogout -= new Action<string>(srvDataClient_OnFriendLogout);
                _srvDataClient.LogoutSync();
                _srvDataClient = null;

                if (SrvDataComm != null)
                {
                    SrvDataComm.OnRequestArrival -= new Action<XDataComm<TEnum>.RequestArrivalArgs>(SrvDataComm_OnRequestArrival);
                    SrvDataComm.Stop();
                    SrvDataComm = null;
                }
            }

            _bIsMasterServer = false;
            InvokeMasterChanged();
        }

        /// <summary>
        /// 只登录服务端，不处理热备
        /// </summary>
        public void LoginServer()
        {
            if (_conServer == null)
                throw new ArgumentException("call SetServerInfo to set Configure first");

            InvokeOnLogger("#LoginServer");
            bool bNew = false;
            if (_srvDataClient == null)
            {
                bNew = true;
                _srvDataClient = new XmppClient();
                _srvDataClient.SetServerInfo(_conServer.LocalServer);

                SrvDataComm = new XDataComm<TEnum>(_srvDataClient);

                _srvDataClient.OnExcept += (ex, info) => NetAndComm_OnError(SrvDataComm, ex, info);
                _srvDataClient.OnLogger += new Action<string, XLogSimple.LogLevels>(NetAndComm_OnLogger);
                SrvDataComm.OnExcept += (ex, info) => NetAndComm_OnError(SrvDataComm, ex, info);
                SrvDataComm.OnLogger += new Action<string, XLogSimple.LogLevels>(NetAndComm_OnLogger);
                if (DebugEnabled)
                {
                    _srvDataClient.SetDebugEnabled(true);
                    SrvDataComm.SetDebugEnabled(true);
                }
            }
            _srvDataClient.LoginSync();
            SrvDataComm.Start();

            if (bNew)
            {
                _srvDataClient.OnDisconnected += new Action<bool>(srvDataClient_OnDisconnected);
                _srvDataClient.OnFriendLogout += new Action<string>(srvDataClient_OnFriendLogout);

                SrvDataComm.OnRequestArrival += new Action<XDataComm<TEnum>.RequestArrivalArgs>(SrvDataComm_OnRequestArrival);
            }

            if (!_srvDataClient.IsLogged)
            {
                string strInfo = string.Format("Login failed: {0}", _srvDataClient.DisconnectError);
                InvokeOnLogger(strInfo, XLogSimple.LogLevels.Error);
                throw new XLoginoutException(strInfo);
            }
        }

        void NetAndComm_OnLogger(string strLog_, XLogSimple.LogLevels euLevel_)
        {
            InvokeOnLogger(strLog_, euLevel_);
        }

        void NetAndComm_OnError(XDataComm<TEnum> srvComm_, Exception obj, string strInfo_)
        {
            InvokeOnExcept(obj, strInfo_);
        }

        #region "DataComm"
        private void ClientMasterQueryOP(XDataComm<TEnum>.RequestArrivalArgs argRequest_)
        {
            var clientQuery = argRequest_.RequestData.GetData<IMUserLoginRequest>();
            if (clientQuery.QueryMaster)
            {
                InvokeOnLogger(XLogSimple.LogLevels.Warn, "{0} query master, local {1}",
                    argRequest_.From, _bIsMasterServer ? "IsMaster" : "NotMaster");

                if (!IsMasterServer)
                { // check, ensure has master server
                    var srvData = QueryMasterServer();
                    if (srvData == null || !srvData.IsMaster)
                        SetMasterServer(true);
                }
            }
        }

        private void ResponseMasterData(XDataComm<TEnum>.RequestArrivalArgs argRequest_)
        {
            try
            {
                IMServerMasterData dataMaster = new IMServerMasterData()
                {
                    IsMaster = IsMasterServer,
                    SrvID = LocalServerID,
                    StartTime = ServerStartTime,
                };

                argRequest_.InvokeResponse(ServerMasterCode, dataMaster, null);
            }
            catch (Exception ex)
            {
                InvokeOnExcept(ex, "ResponseMasterData");
            }
        }

        private void MasterQueryCheck(XDataComm<TEnum>.RequestArrivalArgs argRequest_)
        {
            var queryData = argRequest_.RequestData.GetData<IMServerMasterData>();
            if ((IsMasterServer && queryData.IsMaster))
            { // All is master
                // 出现双主服务器时：启动晚者退出主服务器；如果启动时间相同，则ID大者退出主服务器
                if (ServerStartTime > queryData.StartTime
                    || (string.Compare(LocalServerID, queryData.SrvID) > 0))
                {
                    SetMasterServer(false);
                }
                else // Remain Master
                {
                    InvokeMasterChanged();
                }
                InvokeOnLogger(XLogSimple.LogLevels.Warn, "Both is master(Local: {0}, Remote: {1}), local changeto {2}",
                    ServerStartTime, queryData.StartTime, _bIsMasterServer ? "IsMaster" : "NotMaster");
            }
            else if ((!IsMasterServer && !queryData.IsMaster))
            { // All not-master
                // 出现都不是主服务器时：启动早者设为主服务器
                if (ServerStartTime < queryData.StartTime
                    || string.Compare(LocalServerID, queryData.SrvID) < 0)
                {
                    SetMasterServer(true);
                }

                InvokeOnLogger(XLogSimple.LogLevels.Warn, "Both not master(Local: {0}, Remote: {1}), local changeto {2}",
                    ServerStartTime, queryData.StartTime, _bIsMasterServer ? "IsMaster" : "NotMaster");
            }
        }

        void SrvDataComm_OnRequestArrival(XDataComm<TEnum>.RequestArrivalArgs argRequest_)
        {
            if (!IsServerStarted) return; // todo: response with no-login exception

            try
            {
                if (argRequest_.RequestData.DataType.CompareTo(ClientLoginCode) == 0)
                {
                    InvokeClientLogin(argRequest_.From);
                    ClientMasterQueryOP(argRequest_);
                    ResponseMasterData(argRequest_);
                }
                else if (argRequest_.RequestData.DataType.CompareTo(ServerMasterCode) == 0)
                {
                    InvokeSyncLogin(argRequest_.From);

                    if (!_isSyncStart)
                    {  // Remote
                        InvokeOnLogger("Remote enable standby, while local not", XLogSimple.LogLevels.Warn);
                    }
                    else
                    { // Check Only when Sync enabled 
                        MasterQueryCheck(argRequest_);
                    }

                    ResponseMasterData(argRequest_);
                }
                else
                {
                    InvokeRequestArrival(argRequest_);
                }
            }
            catch (Exception ex)
            {
                InvokeOnExcept(ex, "OnRequestArrival");
            }
        }

        void srvDataClient_OnFriendLogout(string strFriend_)
        {
            if (IsServerStarted)
            {
                if (!InvokeSyncLogout(strFriend_))
                    InvokeClientLogout(strFriend_);
            }
        }

        void srvDataClient_OnDisconnected(bool obj)
        {
            InvokeDisconnected(obj);
        }
        #endregion

        /// <summary>
        /// 发送给客户端数据
        /// </summary>
        /// <param name="strClient_"></param>
        /// <param name="euRequestType_"></param>
        /// <param name="tRequestData_"></param>
        /// <param name="actComplete_"></param>
        /// <param name="bNeedResponse_"></param>
        /// <param name="nDataVer_">发送数据的版本(DataWithType.DataVer)</param>
        public void Send2Client(string strClient_, TEnum euRequestType_, object tRequestData_, Action<XDataComm<TEnum>.SendCompleteArgs> actComplete_, bool bNeedResponse_ = false, int nDataVer_ = 1)
        {
            if (!IsServerStarted)
            {
                if (actComplete_ != null)
                {
                    Exception ex = new XNotLoginException("Server has not started");
                    actComplete_(new XDataComm<TEnum>.SendCompleteArgs(ex));
                }
                return;
            }

            SrvDataComm.SendAsyn(strClient_, euRequestType_, tRequestData_, nDataVer_, actComplete_, bNeedResponse_);
        }

        /// <summary>
        /// 发送给其他服务端（同步服务器）数据
        /// </summary>
        /// <param name="euRequestType_"></param>
        /// <param name="tRequestData_"></param>
        /// <param name="nDataVer_">发送数据的版本(DataWithType.DataVer)</param>
        public void Send2Server(TEnum euRequestType_, object tRequestData_, int nDataVer_ = 1)
        {
            if (!IsServerStarted) return;

            if (_syncComm != null)
            {
                _syncComm.SendAsyn(_strRemoteSrvJid, euRequestType_, tRequestData_, nDataVer_, (zop) =>
                    {
                        if (!zop.IsSuccess)
                        {
                            InvokeOnExcept(zop.Result, "Send2Server");
                        }
                    }
                    , false);
            }
        }
    } // class
}
