using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Common;
using System.Threading;
using SHCre.Xugd.Extension;
using SHCre.Xugd.CFile;
using SHCre.Xugd.Data;

namespace SHCre.Xugd.Net.Xmpp
{
    partial class XStandbyClient<TEnum>
    {
        XSafeType<bool> _bIsLoginout = new XSafeType<bool>(false);
        ManualResetEvent _evtWaitLoginout = new ManualResetEvent(false);
        XTimerWork<object> _chkAliveWork = new XTimerWork<object>(60);
        
        #region "Attr"
        /// <summary>
        /// 是否开始运行（Login后开始运行，Logout则结束运行）
        /// </summary>
        public bool IsClientStarted { get; private set; }

        private int _nLogTimeoutInterval = XTime.Second2Interval(45);
        /// <summary>
        /// 登录超时时间
        /// </summary>
        public int LoginTimeoutSeconds
        {
            get { return XTime.Interval2Second(_nLogTimeoutInterval); }
            set
            {
                if (value < 1)
                    value = 45;

                _nLogTimeoutInterval = XTime.Second2Interval(value);
            }
        }

        private int _nLoginRetryInterval = XTime.Second2Interval(60);
        /// <summary>
        /// 登录失败后，重连时间（一般要大于LogTimeoutSeconds）
        /// </summary>
        public int LoginRetrySeconds
        {
            get { return XTime.Interval2Second(_nLoginRetryInterval); }

            set
            {
                if (value < 1)
                    value = LoginTimeoutSeconds + 1;

                _nLoginRetryInterval = XTime.Second2Interval(value);
            }
        }

        /// <summary>
        /// 登录服务器成功后触发的事件,
        /// 可在里面处理登录相关的事务（如登录话机等）
        /// </summary>
        public event Action<XDataComm<TEnum>> OnSrvConnect;
        void InvokeSrvConnect(IMClientSrvInfo srvInfo_)
        {
            if (srvInfo_.HasLogSrv) return;

            srvInfo_.HasLogSrv = true;
            if (OnSrvConnect != null)
            {
                OnSrvConnect(srvInfo_.CommClient);
            }
        }

        /// <summary>
        /// 服务端断开连接时
        /// </summary>
        public event Action<XDataComm<TEnum>> OnSrvDiconnect;
        void InvokeSrvDisconnect(IMClientSrvInfo srvInfo_)
        {
            if (!srvInfo_.HasLogSrv) return;
            srvInfo_.HasLogSrv = false;

            if (OnSrvDiconnect != null)
            {
                XDataComm<TEnum> dataComm = (srvInfo_ == null) ? null : srvInfo_.CommClient;
                OnSrvDiconnect(dataComm);
            }
        }
        #endregion
        
        /// <summary>
        /// 登录（只有发现主服务器，才视为成功）：
        /// 返回前会触发OnLoginServer与OnMasterChanged事件；
        /// 通过MasterComm可获取当前主服务器（为null说明当前没有主服务器）；
        /// 通过OnMasterChanged来获取主服务器的变化。
        /// </summary>
        public void LoginSync()
        {
            InvokeOnLogger("#LoginSync");
            if (_conClient == null)
                throw new ArgumentException("Server config not set, call AddClientServer first");

            if (GetMasterServer(false) != null)
            {
                InvokeOnLogger("LoginSync: has master server, just return");
                return;
            }
            if (_bIsLoginout.EqualOrSet(true))
                throw new XLoginoutException("Another thread is loginout now.");

            try
            {
                IsClientStarted = false;
                _chkAliveWork.Stop();

                lock (_lockClientSrvs)
                {
                    _lstClientSrvs.ForEach(zc =>
                    {
                        ClearEventHandles(zc);
                        zc.Disconnect();
                    });
                    _lstClientSrvs.Clear();

                    // Add new
                    foreach (var srvInfo in _conClient.StandbyServer.WhereNoDelay(z => z.Enabled))
                    {
                        XmppClient conClient = new XmppClient();
                        conClient.SetServerInfo(srvInfo.XmppServer);
                        InvokeOnLogger(XLogSimple.LogLevels.Info, "LoginSync-Server: User{0}, Addr:{1}", srvInfo.XmppServer.LoginInfo.UserName, srvInfo.XmppServer.LoginInfo.PrintAddr());

                        var conSrv = new IMClientSrvInfo(conClient, srvInfo.ServerName);
                        conSrv.CommClient.ResponseWaitSeconds = srvInfo.ResponseTimeoutSecond;

                        _lstClientSrvs.Add(conSrv);
                    }
                                        
                    _evtWaitLoginout.Reset();
                    _lstClientSrvs.ForEach(zc => ToConnectAndCheck(zc));
                } // lock

                if (!_evtWaitLoginout.WaitOne(_nLogTimeoutInterval))
                {
                    throw new TimeoutException(string.Format("Login failed: No response in {0}s", LoginTimeoutSeconds));
                }
                if (GetMasterServer(false) == null)
                    throw new XLoginoutException("No master server can login");
                
                lock (_lockClientSrvs)
                {
                    _lstClientSrvs.ForEach((zc) =>
                    {
                        CreateEventHandles(zc);
                        if (zc.IsLogged)
                        {
                            InvokeSrvConnect(zc);
                        }
                    });
                }

                // Login OK
                IsClientStarted = true;
                InvokeMasterChanged();
                StartCheckAliveOthers();
            }
            finally
            {
                _bIsLoginout.Set(false);
            }
        }

        private void CreateEventHandles(IMClientSrvInfo conSrv)
        {
            var conClient = conSrv.ConnClient;
            conClient.OnDisconnected += (zClose) => ImConnect_OnDisconnected(conSrv, zClose);
            conClient.OnFriendLogin += (zName) => ImConnect_OnFriendLogin(conSrv, zName);
            conClient.OnFriendLogout += (zName) => ImConnect_OnFriendLogout(conSrv, zName);

            var comm = conSrv.CommClient;
            comm.OnRequestArrival += (zarg) => CommClient_OnRequestArrival(conSrv, zarg);
            //comm.OnDecodeDataFail += (zarg) => CommClient_OnDecodeDataFail(conSrv, zarg);
            //comm.OnResponseMismatch += (zarg) => CommClient_OnResponseMismatch(conSrv, zarg);

            conClient.OnExcept += InvokeOnExcept;
            conClient.OnLogger += InvokeOnLogger;
            comm.OnExcept += InvokeOnExcept;
            comm.OnLogger += InvokeOnLogger;
            if(DebugEnabled)
            {
                conClient.SetDebugEnabled(true);
                comm.SetDebugEnabled(true);
            }
        }

        /// <summary>
        /// 登出：
        /// 返回前会触发OnServerDisconnected与OnMasterChanged事件；
        /// </summary>
        public void LogoutSync()
        {
            InvokeOnLogger("#LogoutSync");
            if (_bIsLoginout.EqualOrSet(true))
                throw new XLoginoutException("Another thread is loginout now.");

            try
            {
                IsClientStarted = false;
                _chkAliveWork.Stop();

                List<IMClientSrvInfo> lstSrvs;
                lock (_lockClientSrvs)
                {
                    lstSrvs = _lstClientSrvs.Update((zsrv) =>
                    {
                        zsrv.HasTryLog = false;

                        ClearEventHandles(zsrv);
                    });
                } // lock

                _evtWaitLoginout.Reset();
                lstSrvs.ForEach(z => ToDisconnect(z));
                if (!_evtWaitLoginout.WaitOne(_nLogTimeoutInterval))
                {
                    throw new TimeoutException(string.Format("Logout failed: No response in {0}s", LoginTimeoutSeconds));
                }

                // Logout OK
                lock (_lockClientSrvs)
                {
                    _lstClientSrvs.ForEach((zc) => InvokeSrvDisconnect(zc));
                    _lstClientSrvs.Clear();
                }
            }
            finally
            {
                _bIsLoginout.Set(false);

                ClearMaster();
                InvokeMasterChanged();
            }
        }

        private void ClearEventHandles(IMClientSrvInfo conSrv)
        {
            // below is Useless
            //var conClient = conSrv.ConnClient;
            //conClient.OnDisconnected -= (zClose) => ImConnect_OnDisconnected(conSrv, zClose);
            //conClient.OnError -= (zErr) => ImConnect_OnError(conSrv, zErr);
            //conClient.OnFriendLogin -= (zName) => ImConnect_OnFriendLogin(conSrv, zName);
            //conClient.OnFriendLogout -= (zName) => ImConnect_OnFriendLogout(conSrv, zName);

            //conSrv.CommClient.OnRequestArrival -= (zarg) => CommClient_OnRequestArrival(conSrv, zarg);
       }

        #region "Event-handle"

        void CommClient_OnRequestArrival(IMClientSrvInfo srvInfo_, XDataComm<TEnum>.RequestArrivalArgs argRequest_)
        {
            if (argRequest_.RequestData.DataType.CompareTo(ServerMasterCode) == 0)
            {
                try
                {
                    var masterData = argRequest_.RequestData.GetData<IMServerMasterData>();
                    MasterChanged(srvInfo_, masterData);
                }
                catch (Exception ex)
                {
                    InvokeOnExcept(new XDataHandleException("MasterChange: Get data failed", ex)
                    {
                        HandleData = argRequest_.RequestData.DataJson,
                        From = argRequest_.From,
                    },
                    string.Format("[{0}]", srvInfo_.ConnClient.RemoteAddress));
                }
            }
            else
            {
                InvokeRequestArrival(srvInfo_, argRequest_);
            }
        }

        void ImConnect_OnFriendLogout(IMClientSrvInfo srvInfo_, string strLogout_)
        {
            if (srvInfo_.ServerName == strLogout_)
            {
                ServerLogout(srvInfo_);
            }
        }

        void ImConnect_OnFriendLogin(IMClientSrvInfo srvInfo_, string strLogin_)
        {
            if (srvInfo_.ServerName == strLogin_)
            {
                // Avoid all clients Connect synchronously
                Thread.Sleep(XRandom.GetInt(100, 5000));
                LoginServer(srvInfo_, false);
            }
        }

        void ImConnect_OnDisconnected(IMClientSrvInfo srvInfo_, bool bIsClose_)
        {
            ServerLogout(srvInfo_);
        }

        private void ServerLogout(IMClientSrvInfo srvInfo_)
        {
            if (!IsClientStarted) return;

            bool bInvoke = false;
            lock (_lockClientSrvs)
            {
                if (srvInfo_ == _srvMaster)
                {
                    InvokeOnLogger(XLogSimple.LogLevels.Warn, "NoMaster: Master {0} logout", srvInfo_.ConnClient.RemoteAddress);
                    _srvMaster = null;
                    bInvoke = true;
                }
            }
            if (bInvoke)
                InvokeMasterChanged();

            StartCheckAliveOthers();
            InvokeSrvDisconnect(srvInfo_);
        }
        #endregion

        #region "Check Alive(Other-server)"
        /// <summary>
        /// 对于非主服务器，定时发送一个Query请求，保证连接畅通
        /// </summary>
        void StartCheckAliveOthers()
        {
            _chkAliveWork.Start(false);
        }

        void StartCheckImmediately()
        {
            _chkAliveWork.Start(true);
        }

        XTimerWorkResult ToCheckAliveOthers(object oNone_)
        {
            var lstOthers = GetOtherServers();
            if (IsClientStarted && lstOthers.Count > 0)
            {
                bool bCheckMaster = (_srvMaster == null);
                lstOthers.ForEach(z => ToConnect(z, bCheckMaster));
            }
            else
            {
                return XTimerWorkResult.Complete;   // No server need check.
            }

            return XTimerWorkResult.Restart;
        }
        #endregion

        #region "Connect"
        /// <summary>
        /// 
        /// </summary>
        /// <param name="srvInfo_"></param>
        /// <param name="bCheckMaster_">对于已登录的服务端，检测是否存在主服务</param>
        void ToConnect(IMClientSrvInfo srvInfo_, bool bCheckMaster_)
        {
            if (srvInfo_.IsLogged)
            {
                srvInfo_.CommClient.Start();
                LoginServer(srvInfo_, bCheckMaster_);
            }
            else
            {
                try
                {
                    srvInfo_.ConnClient.LoginSync();
                    srvInfo_.CommClient.Start();
                    LoginServer(srvInfo_, false);
                }
                catch(Exception ex)
                {
                    InvokeOnExcept(ex, string.Format("Login[{0}]", srvInfo_.ConnClient.RemoteAddress));
                }
            }
        }

        void LoginServer(IMClientSrvInfo srvInfo_, bool bQueryMaster_)
        {
            if (!IsClientStarted) return;

            IMUserLoginRequest loginData = new IMUserLoginRequest()
            {
                QueryMaster = bQueryMaster_,
            };
            srvInfo_.CommClient.SendAsyn(srvInfo_.ServerName, ClientLoginCode, loginData,
                (zarg) =>
                {
                    Exception exError = null;
                    if (zarg.IsSuccess)
                    {
                        try
                        {
                            var retData = zarg.ResponseData.GetData<IMServerMasterData>();
                            if (retData.IsMaster)
                            {
                                MasterChanged(srvInfo_, retData);
                            }

                            InvokeSrvConnect(srvInfo_);
                            return;
                        }
                        catch (Exception ex)
                        {
                            exError = ex;
                        }
                    }
                    else
                    {
                        exError = zarg.Result;
                    }

                    InvokeOnExcept(exError, string.Format("[{0}]", srvInfo_.ConnClient.RemoteAddress));
                },
                true);
        }

        void ToConnectAndCheck(IMClientSrvInfo srvInfo_)
        {
            srvInfo_.ConnClient.LoginAsync((zop) =>
            {
                if (zop.IsSuccess)
                {
                    srvInfo_.CommClient.Start();
                    // This must in Thread.
                    XThread.StartThread(() => LoginServerAndCheck(srvInfo_));
                }
                else
                {
                    CheckLoginFailed(srvInfo_, zop.Result);
                }
            });
        }

        void LoginServerAndCheck(IMClientSrvInfo srvInfo_)
        {
            IMUserLoginRequest loginData = new IMUserLoginRequest()
            {
                QueryMaster = false,
            };
            srvInfo_.CommClient.SendAsyn(srvInfo_.ServerName, ClientLoginCode, loginData,
                (zarg) =>
                {
                    Exception exError = null;
                    if (zarg.IsSuccess)
                    {
                        try
                        {
                            var retData = zarg.ResponseData.GetData<IMServerMasterData>();
                            if (retData.IsMaster)
                            {
                                MasterChanged(srvInfo_, retData);
                                _evtWaitLoginout.Set();
                                InvokeOnLogger(XLogSimple.LogLevels.Info, "Login {0} is master", srvInfo_.CommClient.NetComm.RemoteAddress);
                            }
                            else
                            {
                                CheckLoginComplete(srvInfo_);
                                InvokeOnLogger(XLogSimple.LogLevels.Info, "Login {0} not master", srvInfo_.CommClient.NetComm.RemoteAddress);
                            }

                            //InvokeLoginServer(srvInfo_);
                            return;
                        }
                        catch (Exception ex)
                        {
                            exError = ex;
                        }
                    }
                    else
                    {
                        exError = zarg.Result;
                    }

                    CheckLoginFailed(srvInfo_, exError);
                },
                true);
        }

        void CheckLoginFailed(IMClientSrvInfo srvInfo_, Exception exError_)
        {
            CheckLoginComplete(srvInfo_);

            InvokeOnExcept(exError_, string.Format("Login[{0}]", srvInfo_.ConnClient.RemoteAddress));
        }

        private void CheckLoginComplete(IMClientSrvInfo srvInfo_)
        {
            if (_bIsLoginout.IsEqual(true))
            {
                lock (_lockClientSrvs)
                {
                    srvInfo_.HasTryLog = true;
                    if (_lstClientSrvs.All(z => z.HasTryLog))
                        _evtWaitLoginout.Set();
                }
            }
        }
        #endregion

        #region "Disconnect"
        void ToDisconnect(IMClientSrvInfo srvInfo_)
        {
            srvInfo_.CommClient.NetComm.LogoutAsync((zop) =>
            {
                CheckLogoutComplete(srvInfo_);

                srvInfo_.CommClient.Stop();
            });
        }

        void CheckLogoutComplete(IMClientSrvInfo srvInfo_)
        {
            lock (_lockClientSrvs)
            {
                srvInfo_.HasTryLog = true;

                if (_lstClientSrvs.All(z => z.HasTryLog))
                    _evtWaitLoginout.Set();
            }
        }
        #endregion

    } // Class 
}
