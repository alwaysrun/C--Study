using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Extension;
using System.Threading;
using SHCre.Xugd.Common;
using SHCre.Xugd.Data;

namespace SHCre.Xugd.Net
{
    partial class XIMRedundantServer<TEnum>
    {
        /// <summary>
        /// 其他服务器连接到本服务器（本服务端登录的服务器）的用户名
        /// </summary>
        XSafeList<string> _lstInSyncUsers = new XSafeList<string>();
        /// <summary>
        /// 连接到其他服务器上的同步连接信息
        /// </summary>
        XSafeList<SyncServerInfo> _lstOutSyncInfos = new XSafeList<SyncServerInfo>();

        /// <summary>
        /// 同步用户登录
        /// </summary>
        public event Action<string> OnSyncLogin;
        void InvokeSyncLogin(string strUser_)
        {
            if (_lstInSyncUsers.AddIfNotExist(strUser_, z=>z==strUser_))
            {
                if (OnSyncLogin != null)
                    OnSyncLogin(strUser_);
            }
        }

        /// <summary>
        /// 同步用户退出
        /// </summary>
        public event Action<string> OnSyncLogout;
        void InvokeSyncLogout(string strUser_)
        {
            if (_lstInSyncUsers.Remove(strUser_))
            {
                if (OnSyncLogout != null)
                    OnSyncLogout(strUser_);
            }
        }

        /// <summary>
        /// 连接到其他服务器的同步连接连接成功时触发Connect(XDataComm, IsRemoteMaster)
        /// </summary>
        public event Action<XDataComm<TEnum>, bool> OnSyncConnect;
        void InvokeSyncConnect(XDataComm<TEnum> dataComm_, bool bIsMaster_)
        {
            if (OnSyncConnect != null)
                OnSyncConnect(dataComm_, bIsMaster_);
        }

        /// <summary>
        /// 连接到其他服务器的同步连接断开时触发
        /// </summary>
        public event Action<XDataComm<TEnum>> OnSyncDisconnect;
        void InvokeSyncDisconnect(XDataComm<TEnum> dataComm_)
        {
            if (OnSyncConnect != null)
                OnSyncDisconnect(dataComm_);
        }

        /// <summary>
        /// 当前是否有登录成功同步服务器
        /// </summary>
        /// <returns></returns>
        public bool HasLoggedSyn()
        {
            return _lstOutSyncInfos.Any(z => z.SyncSrv.HasLogin);
        }

        /// <summary>
        /// 当前同步用户数
        /// </summary>
        /// <returns></returns>
        public int GetSynCount()
        {
            return _lstOutSyncInfos.Count;
        }

        bool IsSyncUser(string strUser_)
        {
            return _lstInSyncUsers.Contains(strUser_);
        }

        void ClearSyncUsers()
        {
            _lstInSyncUsers.Clear();
        }
        
        private int _nLoginSyncTimeoutInterval = XTime.Second2Interval(45);
        /// <summary>
        /// 登录其他热备服务器的超时时间
        /// </summary>
        public int LoginSyncTimeoutSeconds
        {
            get { return XTime.Interval2Second(_nLoginSyncTimeoutInterval); }
            set
            {
                if (value < 1)
                    value = 45;

                _nLoginSyncTimeoutInterval = XTime.Second2Interval(value);
            }
        }

        private int _nLoginSyncRetryInterval = XTime.Second2Interval(60);
        /// <summary>
        /// 登录其他热备服务器的失败后，重连时间（一般要大于LogTimeoutSeconds）
        /// </summary>
        public int LoginSyncRetrySeconds
        {
            get { return XTime.Interval2Second(_nLoginSyncRetryInterval); }

            set
            {
                if (value < 1)
                    value = LoginSyncTimeoutSeconds + 1;

                _nLoginSyncRetryInterval = XTime.Second2Interval(value);
            }
        }

        /// <summary>
        /// 主服务器变化时触发
        /// </summary>
        public event Action<bool> OnMasterChanged;
        void InvokeMasterChanged(bool bMaster_)
        {
            SendUserMasterChanged(bMaster_);
            _lstOutSyncInfos.ForEach(z => z.SyncSrv.SendMasterData(), zc=>zc.SyncSrv.HasLogin);

            if (OnMasterChanged != null)
                OnMasterChanged(bMaster_);
        }

        void MasterConflict()
        {
            SendUserMasterChanged(true);
            _lstOutSyncInfos.ForEach(z => z.SyncSrv.ConnectSync());
        }

        /// <summary>
        /// 添加连接到其他服务器的用于同步的同步用户连接
        /// </summary>
        /// <param name="imSync_">用于连接Openfire的类（需要已通过SetServerInfo设定了网络信息）</param>
        /// <param name="strRemoteSrvName_">连接到Openfire服务器上对应的服务端用户名</param>
        public void AddSyncServer(XIMConnection imSync_, string strRemoteSrvName_)
        {
            var srvSync = new XIMRedundantSync<TEnum>(imSync_, LocalServerID, strRemoteSrvName_, ServerMasterCode, 
                ()=>IsMasterServer, _nResponseWaitSeconds);
            var syncInfo = new SyncServerInfo(srvSync);

            if(_lstOutSyncInfos.AddIfNotExist(syncInfo, z=>z.SyncSrv.CommSync.NetComm == imSync_))
            {
                syncInfo.SyncSrv.LoginRetrySeconds = LoginSyncRetrySeconds;
                syncInfo.SyncSrv.LoginTimeoutSeconds = LoginSyncTimeoutSeconds;

                syncInfo.SyncSrv.OnConnected += (zis)=>SyncSrv_OnConnected(srvSync, zis);
                syncInfo.SyncSrv.OnDisconnect += ()=>SyncSrv_OnDisconnect(srvSync);
                syncInfo.SyncSrv.OnError += (zex)=>SyncSrv_OnError(syncInfo, zex);
                // syncInfo.SyncSrv.OnRequestArrival += (zarg)=>SyncSrv_OnRequestArrival(syncInfo, zarg);
            }
        }

        void SyncSrv_OnError(SyncServerInfo syncInfo_, Exception obj)
        {
            InvokeError(syncInfo_.SyncSrv.CommSync, obj);
        }

        void SyncSrv_OnDisconnect(XIMRedundantSync<TEnum> synComm_)
        {
            if (!IsServerStarted)
                return;

            CheckMaster();
            InvokeSyncDisconnect(synComm_.CommSync);
        }

        void SyncSrv_OnConnected(XIMRedundantSync<TEnum> synComm_, bool bIsMaster_)
        {
            InvokeSyncConnect(synComm_.CommSync, bIsMaster_);
        }

        void CheckMaster()
        {
            if(_lstOutSyncInfos.Count == 0)
            {
                IsMasterServer = true;
                return;
            }

            if (_lstOutSyncInfos.Any(z => z.SyncSrv.IsRemoteMaster))
            {
                IsMasterServer = false;
                return;
            }
            // 最先登录者，作为主服务器（极个别情况下，会出现登录时间相等的情况；
            // 这时如果出现多个主服务器，则由服务端确定哪些服务器退出为非主服务器）
            if(_lstOutSyncInfos.All(z=>z.SyncSrv.RemoteMasterData==null || z.SyncSrv.RemoteMasterData.StartTime>ServerStartTime))
            {
                IsMasterServer = true;
            }
        }

        void StartSyncs()
        {
            if (_lstOutSyncInfos.Count == 0)
            {
                IsMasterServer = true;
                return;
            }

            ManualResetEvent evtWait = new ManualResetEvent(false);
            var lstSyncs = _lstOutSyncInfos.Update((z) => z.TryStarted = false);
            lstSyncs.ForEach(z => TryStart(z, evtWait));

            evtWait.WaitOne(_nLoginSyncTimeoutInterval);
        }

        void StopSyncs()
        {
            if (_lstOutSyncInfos.Count == 0)
            {
                return;
            }

            _lstOutSyncInfos.ForEach(z => z.SyncSrv.StopAsync());
        }

        void TryStart(SyncServerInfo synSrv_, ManualResetEvent evtWait_)
        {
            synSrv_.SyncSrv.StartAsync(ServerStartTime, (zop)=>
                    {
                        synSrv_.TryStarted = true;
                        if(zop.IsSuccess)
                        {
                            if(synSrv_.SyncSrv.IsRemoteMaster)
                            {
                                IsMasterServer = false;
                                evtWait_.Set();
                                return;
                            }
                        }
                        else
                        {
                            InvokeError(synSrv_.SyncSrv.CommSync, zop.Result);
                        }

                        CheckStartComplete(evtWait_);
                    });
        }

        void CheckStartComplete(ManualResetEvent evtWait_)
        {
            if(_lstOutSyncInfos.Any(z=>!z.TryStarted))
                return;

            CheckMaster();
            evtWait_.Set();
        }


        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 
        /// </summary>
        class SyncServerInfo
        {
            public SyncServerInfo(XIMRedundantSync<TEnum> srvSync_)
            {
                SyncSrv = srvSync_;
            }

            public bool TryStarted {get; set;}
            public XIMRedundantSync<TEnum> SyncSrv { get; private set; }
        }
    } //
}
