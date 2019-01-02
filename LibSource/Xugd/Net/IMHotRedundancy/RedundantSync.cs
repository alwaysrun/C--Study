using System;
using SHCre.Xugd.Common;
using SHCre.Xugd.Data;

namespace SHCre.Xugd.Net
{
    [Obsolete]
    internal partial class XIMRedundantSync<TEnum>
    {
        bool _bIsLogining = false;
        Func<bool> FunIsMaster;

        /// <summary>
        /// 启动时间
        /// </summary>
        public DateTime StartTime { get; private set; }
        /// <summary>
        /// 是否已启动
        /// </summary>
        public bool IsSyncStarted { get; private set; }

        /// <summary>
        /// 用于客户端登录的命令
        /// </summary>
        public TEnum MasterQueryCode { get; private set; }

        /// <summary>
        /// 本地服务器标识
        /// </summary>
        public string LocalServerID { get; private set; }

        /// <summary>
        /// 远端服务器名
        /// </summary>
        public string RemoteServerName { get; private set; }

        /// <summary>
        /// 远端是否是主服务器
        /// </summary>
        public bool IsRemoteMaster
        {
            get { return (RemoteMasterData != null) && (RemoteMasterData.IsMaster); }
        }

        public bool HasLogin { get { return RemoteMasterData != null; } }

        /// <summary>
        /// 远端是否是主服务器
        /// </summary>
        public IMServerMasterData RemoteMasterData { get; private set; }

        /// <summary>
        /// 连接
        /// </summary>
        public XDataComm<TEnum> CommSync { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imConnect_"></param>
        /// <param name="strLocalSrvId_"></param>
        /// <param name="strRemoteSrvName_"></param>
        /// <param name="euMasterQuery_"></param>
        /// <param name="funIsMaster_">用于获取当前本地服务器是否为主服务器</param>
        /// <param name="nResponseWaitSeconds_"></param>
        public XIMRedundantSync(XIMConnection imConnect_, string strLocalSrvId_, string strRemoteSrvName_, TEnum euMasterQuery_, Func<bool> funIsMaster_, int nResponseWaitSeconds_ = 0)
        {
            CommSync = new XDataComm<TEnum>(imConnect_);
            LocalServerID = strLocalSrvId_;
            MasterQueryCode = euMasterQuery_;
            RemoteServerName = strRemoteSrvName_.ToLower();
            FunIsMaster = funIsMaster_;

            if (CommSync != null)
            {
                imConnect_.OnDisconnected += new Action<bool>(ImConnect_OnDisconnected);
                imConnect_.OnError += new Action<Exception>(ImConnect_OnError);
                imConnect_.OnFriendLogin += new Action<string>(ImConnect_OnFriendLogin);
                imConnect_.OnFriendLogout += new Action<string>(ImConnect_OnFriendLogout);

                CommSync.ResponseWaitSeconds = nResponseWaitSeconds_;
                //CommSync.OnRequestArrival += new Action<XDataComm<TEnum>.RequestArrivalArgs>(CommSync_OnRequestArrival);
                CommSync.OnDecodeDataFail += new Action<XDataComm<TEnum>.ReceiveDataHandleFailedArgs>(CommSync_OnDecodeDataFail);
            }
        }

        #region "Comm Event"
        void ImConnect_OnFriendLogout(string strName_)
        {
            if (strName_ == RemoteServerName)
                InvokeDisconnect();
        }

        void ImConnect_OnFriendLogin(string strName_)
        {
            if (strName_ == RemoteServerName)
                QueryMasterServer();
        }

        void ImConnect_OnError(Exception obj)
        {
            InvokeError(obj);
        }

        void ImConnect_OnDisconnected(bool bIsClose_)
        {
            InvokeDisconnect();
        }

        void CommSync_OnRequestArrival(XDataComm<TEnum>.RequestArrivalArgs obj)
        {
            //throw new NotImplementedException();
        }

        void CommSync_OnDecodeDataFail(XDataComm<TEnum>.ReceiveDataHandleFailedArgs obj)
        {
            InvokeError(new XDataHandleException("Decode failed", obj.Error)
                {
                    HandleData = obj.Data,
                    From = obj.From,
                });
        }
        #endregion

        /// <summary>
        /// 启动，连接到远端服务器：
        /// 如果失败，抛出异常；
        /// 完成后可通过IsRemoteMaster判断对方是否是主服务器
        /// </summary>
        /// <param name="dtStart_"></param>
        public void StartSync(DateTime dtStart_)
        {
            IsSyncStarted = true;
            StartTime = dtStart_;
            _evtStopWait.Reset();

            RemoteMasterData = null;
            try
            {
                _bIsLogining = true;
                _evtLoginWait.Reset();
                ConnectSync();
                if (!_evtLoginWait.WaitOne(_nLoginTimeoutInterval))
                {
                    throw new TimeoutException(string.Format("Connect Sync timeout: over {0}s", LoginTimeoutSeconds));
                }
            }
            finally
            {
                _bIsLogining = false;
            }
        }

        /// <summary>
        /// 异步连接远端服务器：
        /// 完成后可通过IsRemoteMaster判断对方是否是主服务器
        /// </summary>
        /// <param name="dtStart_"></param>
        /// <param name="actComplete_"></param>
        public void StartAsync(DateTime dtStart_, Action<XAsyncResult> actComplete_)
        {
            XThread.StartThread(() =>
                {
                    Exception exError = null;
                    try
                    {
                        StartSync(dtStart_);
                    }
                    catch (Exception ex)
                    {
                        exError = ex;
                    }

                    actComplete_(XAsyncResult.Get(exError));
                });
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void StopAsync()
        {
            IsSyncStarted = false;
            _evtStopWait.Set();

            CommSync.NetComm.LogoutAsync((zop) =>
            {
                CommSync.Stop();
            });
        }
    } // class
}
