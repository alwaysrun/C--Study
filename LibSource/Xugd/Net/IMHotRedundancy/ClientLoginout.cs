using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SHCre.Xugd.Common;
using SHCre.Xugd.Extension;
using SHCre.Xugd.Data;

namespace SHCre.Xugd.Net
{
    partial class XIMRedundantClient<TEnum>
    {
        private XSafeType<bool> _bIsLoginout = new XSafeType<bool>(false);
        ManualResetEvent _evtWaitLoginout = new ManualResetEvent(false);
        ManualResetEvent _evtStoptReconnect = new ManualResetEvent(false);
        Thread _thrCheckAliveOthers = null;

        /// <summary>
        /// 是否开始运行（Login后开始运行，Logout则结束运行）
        /// </summary>
        public bool HasStarted { get; private set; }

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
        public event Action<XDataComm<TEnum>> OnLoginServer;
        void InvokeLoginServer(IMClientSrvInfo srvInfo_)
        {
            if (srvInfo_.IsLogin) return;

            srvInfo_.IsLogin = true;
            if (OnLoginServer != null)
            {
                OnLoginServer(srvInfo_.CommClient);
            }
        }

        /// <summary>
        /// 服务端断开连接时
        /// </summary>
        public event Action<XDataComm<TEnum>> OnServerDisconnected;
        void InvokeServerDisconnected(IMClientSrvInfo srvInfo_)
        {
            if (OnServerDisconnected != null)
            {
                XDataComm<TEnum> dataComm = (srvInfo_ == null) ? null : srvInfo_.CommClient;
                OnServerDisconnected(dataComm);
            }
        }

        /// <summary>
        /// 登录（只有发现主服务器，才视为成功）：
        /// 如果登录失败，只要没有调用Logout还会一直尝试重新登录，直到Logout；
        /// 可通过OnMasterChanged来获取是否真正登录成功
        /// </summary>
        public void LoginSync()
        {
            if (GetMasterServer(false) != null)
                return;
            if (_bIsLoginout.EqualOrSet(true))
                throw new XLoginoutException("Another thread is loginout now.");

            try
            {
                HasStarted = true;
                _evtStoptReconnect.Reset();
                _evtWaitLoginout.Reset();

                List<IMClientSrvInfo> lstSrvs;
                lock (_lockClientSrvs)
                {
                    lstSrvs = _lstClientSrvs.Update((zsrv) =>
                    {
                        zsrv.HasTryLog = false;
                        zsrv.IsLogin = false;
                    });
                } // lock
                lstSrvs.ForEach(z => Connect(z));

                if (!_evtWaitLoginout.WaitOne(_nLogTimeoutInterval))
                {
                    throw new TimeoutException(string.Format("Login failed: No response in {0}s", LoginTimeoutSeconds));
                }

                if (GetMasterServer(false) == null)
                    throw new XLoginoutException("No master server can login");
            }
            finally
            {
                _bIsLoginout.Set(false);
                StartCheckAliveOthers();
            }
        }

        /// <summary>
        /// 对于非主服务器，定时发送一个Query请求，保证连接畅通
        /// </summary>
        void StartCheckAliveOthers()
        {
            if (_thrCheckAliveOthers == null)
                _thrCheckAliveOthers = XThread.StartThread(CheckAliveOthersThread);
        }

        void CheckAliveOthersThread()
        {
            try
            {
                while (true)
                {
                    if (_evtStoptReconnect.WaitOne(_nLoginRetryInterval))
                        return;

                    var lstOthers = GetOtherServers();
                    lstOthers.ForEach(z => Connect(z));
                }
            }
            catch(Exception ex)
            {
                InvokeError(null, ex);
            }

            _thrCheckAliveOthers = null;
        }

        /// <summary>
        /// 登出
        /// </summary>
        public void LogoutSync()
        {
            if (_bIsLoginout.EqualOrSet(true))
                throw new XLoginoutException("Another thread is loginout now.");

            try
            {
                HasStarted = false;
                _evtStoptReconnect.Set();
                _evtWaitLoginout.Reset();

                List<IMClientSrvInfo> lstSrvs;
                lock (_lockClientSrvs)
                {
                    lstSrvs = _lstClientSrvs.Update((zsrv) =>
                    {
                        zsrv.HasTryLog = false;
                        zsrv.IsLogin = false;
                    });
                } // lock
                lstSrvs.ForEach(z => Disconnect(z));

                if (!_evtWaitLoginout.WaitOne(_nLogTimeoutInterval))
                {
                    throw new TimeoutException(string.Format("Logout failed: No response in {0}s", LoginTimeoutSeconds));
                }
            }
            finally
            {
                ClearMaster();
                _bIsLoginout.Set(false);
            }
        }

        void Disconnect(IMClientSrvInfo srvInfo_)
        {
            srvInfo_.CommClient.NetComm.LogoutAsync((zop) =>
                {
                    CheckLogoutComplete(srvInfo_);
                    if (!zop.IsSuccess)
                    {
                        InvokeError(srvInfo_, zop.Result);
                    }

                    srvInfo_.CommClient.Stop();
                });
        }

        void CheckLogoutComplete(IMClientSrvInfo srvInfo_)
        {
            if (_bIsLoginout.IsEqual(false))
                return;

            lock (_lockClientSrvs)
            {
                srvInfo_.HasTryLog = true;

                if (_lstClientSrvs.All(z => z.HasTryLog))
                    _evtWaitLoginout.Set();
            }
        }

        void Connect(IMClientSrvInfo srvInfo_)
        {
            if (srvInfo_.CommClient.NetComm.IsLogged)
            {
                srvInfo_.CommClient.Start();
                LoginServer(srvInfo_, false);
            }
            else
            {
                srvInfo_.CommClient.NetComm.LoginAsync((zop) =>
                {
                    if (zop.IsSuccess)
                    {
                        srvInfo_.CommClient.Start();
                        // Login when OnFriendLogin
                        //if (_bIsLoginout.IsEqual(false))
                        //    LoginServer(srvInfo_);
                    }
                    else
                    {
                        CheckLoginComplete(srvInfo_, zop.Result);
                    }
                });
            }
        }

        void LoginServer(IMClientSrvInfo srvInfo_, bool bQueryMaster_)
        {
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
                            MasterChanged(srvInfo_, retData);
                            if (retData.IsMaster)
                            {
                                _evtWaitLoginout.Set();
                            }

                            //CloseReconnect(srvInfo_);
                            InvokeLoginServer(srvInfo_);
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

                    CheckLoginComplete(srvInfo_, exError);
                },
                true);
        }

        void CheckLoginComplete(IMClientSrvInfo srvInfo_, Exception exError_)
        {
            InvokeError(srvInfo_, exError_);

            if (!HasStarted)
                return;

            if (_bIsLoginout.IsEqual(true))
            {
                lock (_lockClientSrvs)
                {
                    srvInfo_.HasTryLog = true;
                    if (_lstClientSrvs.All(z => z.HasTryLog))
                        _evtWaitLoginout.Set();
                }
            }
            // Reconnect(srvInfo_);
        }

        // Use CheckAliveOthersThread instead of
        //private void CloseReconnect(IMClientSrvInfo srvInfo_)
        //{
        //    if (srvInfo_.ThrReconnect == null)
        //        return;

        //    _evtStoptReconnect.Set();
        //    if (HasStarted)
        //        _evtStoptReconnect.Reset();
        //}

        //private void Reconnect(IMClientSrvInfo srvInfo_)
        //{
        //    if (!HasStarted)
        //        return;

        //    if (srvInfo_.ThrReconnect == null)
        //    {
        //        srvInfo_.ThrReconnect = XThread.StartThread(ReconnectThread, srvInfo_);
        //    }
        //}

        //private void ReconnectThread(object oParam_)
        //{
        //    IMClientSrvInfo srvInfo = oParam_ as IMClientSrvInfo;
        //    try
        //    {
        //        if (_evtStoptReconnect.WaitOne(_nLoginRetryInterval))
        //            return;

        //        Connect(srvInfo);
        //    }
        //    catch (Exception ex)
        //    {
        //        InvokeError(srvInfo, new XLoginoutException("Retry login failed", ex));
        //    }
        //    finally
        //    {
        //        srvInfo.ThrReconnect = null;
        //    }
        //}
    } // class
}
