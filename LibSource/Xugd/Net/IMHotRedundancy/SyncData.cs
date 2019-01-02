using System;
using System.Threading;
using SHCre.Xugd.Common;
using SHCre.Xugd.Data;

namespace SHCre.Xugd.Net
{
    partial class XIMRedundantSync<TEnum>
    {
        Thread _thrReconnect = null;
        ManualResetEvent _evtLoginWait = new ManualResetEvent(false);
        ManualResetEvent _evtStopWait = new ManualResetEvent(false);

        public event Action<XDataComm<TEnum>.RequestArrivalArgs> OnRequestArrival;
        void InvokeRequestArrival(XDataComm<TEnum>.RequestArrivalArgs argRequest_)
        {
            if(OnRequestArrival != null)
            {
                OnRequestArrival(argRequest_);
            }
        }

        /// <summary>
        /// 连接断开时触发
        /// </summary>
        public event Action OnDisconnect;
        void InvokeDisconnect()
        {
            RemoteMasterData = null;
            Reconnect();
            if (OnDisconnect != null)
                OnDisconnect();
        }

        /// <summary>
        /// 连接到远端服务器时触发（参数表示远端服务器是否是主服务器）
        /// </summary>
        public event Action<bool> OnConnected;
        void InvokeConnected(IMServerMasterData dataMaster_)
        {
            RemoteMasterData = dataMaster_;
            if (_bIsLogining)
                _evtLoginWait.Set();

            if (OnConnected != null)
                OnConnected(dataMaster_.IsMaster);
        }

        /// <summary>
        /// 出错时触发
        /// </summary>
        public event Action<Exception> OnError;
        void InvokeError(Exception exError_)
        {
            if (OnError != null)
                OnError(exError_);
        }

        private int _nLoginTimeoutInterval = XTime.Second2Interval(45);
        /// <summary>
        /// 登录超时时间
        /// </summary>
        public int LoginTimeoutSeconds
        {
            get { return XTime.Interval2Second(_nLoginTimeoutInterval); }
            set
            {
                if (value < 1)
                    value = 45;

                _nLoginTimeoutInterval = XTime.Second2Interval(value);
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
        /// 发送数据
        /// </summary>
        /// <param name="euRequestType_"></param>
        /// <param name="tRequestData_"></param>
        /// <param name="actComplete_"></param>
        /// <param name="bNeedResponse_"></param>
        public void SendAsync(TEnum euRequestType_, object tRequestData_, Action<XDataComm<TEnum>.SendCompleteArgs> actComplete_, bool bNeedResponse_ = false)
        {
            CommSync.SendAsyn(RemoteServerName, euRequestType_, tRequestData_, actComplete_, bNeedResponse_);
        }

        /// <summary>
        /// 发送主服务器信息
        /// </summary>
        public void SendMasterData()
        {
            QueryMasterServer(false);
        }

        /// <summary>
        /// 再次连接
        /// </summary>
        public void ConnectSync()
        {
            if(CommSync.NetComm.IsLogged)
            {
                CommSync.Start();
                QueryMasterServer();
            }
            else
            {
                CommSync.NetComm.LoginAsync((zop) =>
                    {
                        if (zop.IsSuccess)
                        {
                            CommSync.Start();
                            // Login when OnFriendLogin
                            //if (!_bIsLogining)   // 初次登录时，由OnFriendLogin中调用
                            //    QueryMasterServer();
                        }
                        else
                        {
                            Reconnect();
                        }
                    });
            }
        }

        void Reconnect()
        {
            if (!IsSyncStarted)
                return;
            if (_bIsLogining)
                _evtLoginWait.Set();

            if (_thrReconnect == null)
            {
                _thrReconnect = XThread.StartThread(ReconnectThread);
            }
        }

        private void ReconnectThread()
        {
            try
            {
                if (_evtStopWait.WaitOne(_nLoginRetryInterval))
                    return;

                ConnectSync();
            }
            catch (Exception ex)
            {
                InvokeError(ex);
            }
            finally
            {
                _thrReconnect = null;
            }
        }

        void QueryMasterServer(bool bReconnect_=true)
        {
            if (!IsSyncStarted)
                return;

            IMServerMasterData queryMaster = new IMServerMasterData()
            {
                IsMaster = FunIsMaster(),
                SrvID = LocalServerID,
                StartTime = StartTime,
            };

            try 
            {
                CommSync.SendAsyn(RemoteServerName, MasterQueryCode, queryMaster, (zop) =>
                    {
                        if(zop.IsSuccess)
                        {
                            try
                            {
                                var responMaster = zop.ResponseData.GetData<IMServerMasterData>();
                                InvokeConnected(responMaster);
                                return;
                            }
                            catch(Exception ex)
                            {
                                InvokeError(ex);
                            }
                        }

                        if(bReconnect_)
                            Reconnect();
                    },
                    true);
            }
            catch{}
        }
    }
}
