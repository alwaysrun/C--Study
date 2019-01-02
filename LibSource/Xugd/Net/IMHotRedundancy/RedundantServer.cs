using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SHCre.Xugd.Common;
using SHCre.Xugd.Data;

namespace SHCre.Xugd.Net
{
    /// <summary>
    /// 热备服务端：
    /// 先通过AddSyncServer添加其他热备服务器后，再Start；
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    [Obsolete("Use SHCre.Xugd.Net.Xmpp.XStandbyServer replace this")]
    public partial class XIMRedundantServer<TEnum> where TEnum:IComparable
    {
        int _nResponseWaitSeconds = 0;

        /// <summary>
        /// 服务器启动时间
        /// </summary>
        public DateTime ServerStartTime {get; private set;}

        /// <summary>
        /// 是否已启动
        /// </summary>
        public bool IsServerStarted { get; private set; }

        bool _bIsMasterServer = false;
        /// <summary>
        /// 是否是主服务器
        /// </summary>
        public bool IsMasterServer 
        {
            get { return _bIsMasterServer; }
            private set
            {
                if (_bIsMasterServer == value)
                    return;

                _bIsMasterServer = value;
                if (IsServerStarted)
                {
                    InvokeMasterChanged(_bIsMasterServer);
                }
            }
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
        public string LocalServerID {get; private set;}
        
        /// <summary>
        /// 对应的底层通讯类
        /// </summary>
        public XDataComm<TEnum> SrvDataComm {get; private set;}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imServer_">用于连接Openfire的类（需要已通过SetServerInfo设定了网络信息）</param>
        /// <param name="strLocalSrvId_">服务器唯一表示</param>
        /// <param name="euLoginCode_">用于表示客户端登录的命令</param>
        /// <param name="euSrvMaster_">用于表示传递主服务器信息的命令</param>
        /// <param name="nResponseWaitSeconds_">发送命令后，等待应答的超时时间</param>
        public XIMRedundantServer(XIMConnection imServer_, string strLocalSrvId_, TEnum euLoginCode_, TEnum euSrvMaster_, int nResponseWaitSeconds_ = 0)
        {
            SrvDataComm = new XDataComm<TEnum>(imServer_);
            LocalServerID = strLocalSrvId_;
            ClientLoginCode = euLoginCode_;
            ServerMasterCode = euSrvMaster_;
            ServerStartTime = DateTime.Now;
            _nResponseWaitSeconds = nResponseWaitSeconds_;

            if(SrvDataComm != null)
            {
                imServer_.OnDisconnected += new Action<bool>(ImServer_OnDisconnected);
                imServer_.OnError += new Action<Exception>(ImServer_OnError);
                //imServer_.OnFriendLogin += new Action<string>(ImServer_OnFriendLogin);
                imServer_.OnFriendLogout += new Action<string>(ImServer_OnFriendLogout);

                SrvDataComm.OnRequestArrival += new Action<XDataComm<TEnum>.RequestArrivalArgs>(CommServer_OnRequestArrival);
                SrvDataComm.OnDecodeDataFail += new Action<XDataComm<TEnum>.ReceiveDataHandleFailedArgs>(CommServer_OnDecodeDataFail);
            }
        }

        #region "Comm Event"
        void ImServer_OnFriendLogout(string obj)
        {
            if (!InvokeClientLogout(obj))
                InvokeSyncLogout(obj);
        }

        void ImServer_OnFriendLogin(string obj)
        {
            // Add user only when ClientUser login by send ClientLoginCode
           // throw new NotImplementedException();
        }

        void ImServer_OnError(Exception obj)
        {
            InvokeError(SrvDataComm, obj);
        }

        void ImServer_OnDisconnected(bool bIsClose_)
        {
            if (IsMasterServer && HasLoggedSyn())
                IsMasterServer = false;

            if (bIsClose_ || HasLoggedSyn())
            {
                InvokeDisconnected(bIsClose_);
            }
            else
            { // If Error(Disconnect as IOException) and not syn-srv try relogin
                XThread.StartThread(() =>
                    {
                        try
                        {
                            LoginServer();
                        }
                        catch(Exception ex)
                        {
                            InvokeError(SrvDataComm, ex);
                            InvokeDisconnected(bIsClose_);
                        }
                    });
            }
        }

        void CommServer_OnRequestArrival(XDataComm<TEnum>.RequestArrivalArgs argRequest_)
        {
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
                    MasterQueryCheck(argRequest_);
                    ResponseMasterData(argRequest_);
                }
                else
                {
                    InvokeRequestArrival(argRequest_);
                }
            }
            catch (Exception ex)
            {
                InvokeError(SrvDataComm, ex);
            }
        }

        private void ClientMasterQueryOP(XDataComm<TEnum>.RequestArrivalArgs argRequest_)
        {
            var clientQuery = argRequest_.RequestData.GetData<IMUserLoginRequest>();
            if (clientQuery.QueryMaster && !IsMasterServer)
            { // check, ensure has master server
                StartSyncs();
            }
        }

        private void MasterQueryCheck(XDataComm<TEnum>.RequestArrivalArgs argRequest_) 
        {
            if (!IsMasterServer)
                return ;

            try 
            {
                var queryData = argRequest_.RequestData.GetData<IMServerMasterData>();
                if (!queryData.IsMaster)
                    return;

                // both is master
                // 出现双主服务器时：启动晚者退出主服务器；如果启动时间相同，则ID大者退出主服务器
                if(ServerStartTime>queryData.StartTime
                    || (string.Compare(LocalServerID, queryData.SrvID)>0))
                {
                    IsMasterServer = false;
                }
                else
                { // 再次发送确认，防止主服务器信号被其他服务器的信号淹没
                    MasterConflict();
                }
            }
            catch(Exception ex)
            {
                InvokeError(SrvDataComm, ex);
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
                InvokeError(SrvDataComm, ex);
            }
        }

        void CommServer_OnDecodeDataFail(XDataComm<TEnum>.ReceiveDataHandleFailedArgs obj)
        {
            //throw new NotImplementedException();
            InvokeError(SrvDataComm, new XDataHandleException("Decode failed", obj.Error)
                {
                    From = obj.From,
                    HandleData = obj.Data,
                });
        }
        #endregion

        /// <summary>
        /// 启动：失败抛出异常
        /// </summary>
        public void Start()
        {
            StartSyncs();
            LoginServer();

            IsServerStarted = true;
            InvokeMasterChanged(_bIsMasterServer);
        }

        /// <summary>
        /// 只登录服务端，不处理热备
        /// </summary>
        public void LoginServer()
        {
            Exception exError = null;
            try
            {
                ManualResetEvent evtWait = new ManualResetEvent(false);
                SrvDataComm.NetComm.LoginAsync((zop) =>
                {
                    if (zop.IsSuccess)
                    {
                        SrvDataComm.Start();
                        ServerStartTime = DateTime.Now;

                        CheckMaster();
                    }
                    else
                    {
                        exError = zop.Result;
                    }

                    evtWait.Set();
                });

                evtWait.WaitOne();
            }
            catch(Exception ex)
            {
                exError = ex;
            }

            if (exError != null)
                throw exError;
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            IsServerStarted = false;

            StopSyncs();
            SrvDataComm.NetComm.LogoutAsync((zop) =>
                {
                    SrvDataComm.Stop();
                });

            IsMasterServer = false;
        }
    } // class
}
