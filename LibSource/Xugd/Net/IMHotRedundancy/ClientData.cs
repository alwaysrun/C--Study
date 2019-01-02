using System;
using System.Collections.Generic;
using SHCre.Xugd.Extension;
using System.Threading;
using System.Linq;
using SHCre.Xugd.Data;

namespace SHCre.Xugd.Net
{
    partial class XIMRedundantClient<TEnum>
    {
        object _lockClientSrvs = new object();
        List<IMClientSrvInfo> _lstClientSrvs = new List<IMClientSrvInfo>();
        IMClientSrvInfo _srvMaster = null;

        /// <summary>
        /// 获取添加过的通讯信息
        /// </summary>
        public List<XDataComm<TEnum>> AllSrvComm { get { return _lstClientSrvs.Select(z=>z.CommClient).ToList(); } }

        /// <summary>
        /// 当前主服务的连接（没有则返回null）
        /// </summary>
        public XDataComm<TEnum> MasterComm 
        {
            get 
            {
                var srvMaster = GetMasterServer(false);
                return srvMaster == null ? null : srvMaster.CommClient;
            }
        }

        /// <summary>
        /// 出错时触发的事件
        /// </summary>
        public event Action<XDataComm<TEnum>, Exception> OnError;
        void InvokeError(IMClientSrvInfo srvInfo_, Exception ex_)
        {
            if (OnError != null)
            {
                XDataComm<TEnum> dataCom = (srvInfo_ == null) ? null : srvInfo_.CommClient;
                OnError(dataCom, ex_);
            }
        }

        /// <summary>
        /// 请求达到时触发的事件
        /// </summary>
        public event Action<XDataComm<TEnum>, XDataComm<TEnum>.RequestArrivalArgs> OnRequestArrival;
        void InvokeRequestArrival(IMClientSrvInfo srvInfo_, XDataComm<TEnum>.RequestArrivalArgs argRequest_)
        {
            if (OnRequestArrival != null)
                OnRequestArrival(srvInfo_.CommClient, argRequest_);
        }

        /// <summary>
        /// 主服务器变化时触发的事件（如果参数为NULL，说明当前没有主服务器）
        /// </summary>
        public event Action<XDataComm<TEnum>> OnMasterChanged;
        void InvokeMasterChanged(IMClientSrvInfo srvInfo_)
        {
            if (OnMasterChanged != null)
            {
                if (srvInfo_ == null)
                    OnMasterChanged(null);
                else
                    OnMasterChanged(srvInfo_.CommClient);
            }
        }

        void ClearMaster()
        {
            lock (_lockClientSrvs)
            {
                _srvMaster = null;
            }
        }
        
        private IMClientSrvInfo GetMasterServer(bool bTryLoginSrv_=true)
        {
            bool bTry = false;
            lock(_lockClientSrvs)
            {
                // 如果没有主服务器，尝试重新做请求查询
                if (_srvMaster == null && bTryLoginSrv_)
                {
                    bTry = true;
                }
                else
                {
                    return _srvMaster;
                }
            }

            // Try login again
            if (bTry)
            {
                _lstClientSrvs.ForEach(z =>
                {
                    z.IsLogin = false;
                    if (z.CommClient.NetComm.IsLogged)
                    {
                        LoginServer(z, true);
                    }
                });
            }
            return null;
        }

        private List<IMClientSrvInfo> GetOtherServers()
        {
            lock(_lockClientSrvs)
            {
                return _lstClientSrvs.WhereNoDelay(z => z != _srvMaster);
            }
        }

        void MasterChanged(IMClientSrvInfo srvInfo_, IMServerMasterData masterData_)
        {
            bool bInvoke = false;
            IMClientSrvInfo srvInvoke = null;
            lock (_lockClientSrvs)
            {
                if (masterData_.IsMaster)
                {
                    if (_srvMaster != srvInfo_)
                    {
                        _srvMaster = srvInfo_;
                        srvInvoke = srvInfo_;
                        bInvoke = true;
                    }
                }
                else
                {
                    if (_srvMaster == srvInfo_)
                    {
                        _srvMaster = null;
                        bInvoke = true;
                    }
                }
            }

            if (bInvoke)
            {
                InvokeMasterChanged(srvInvoke);
                StartCheckAliveOthers();
            }
        }

        /// <summary>
        /// 检测当前主服务器是否连接
        /// </summary>
        /// <returns></returns>
        public bool MasterConnected()
        {
            return _srvMaster != null;
        }

        /// <summary>
        /// 只发给主服务器的命令
        /// </summary>
        /// <param name="euRequestType_"></param>
        /// <param name="tRequestData_"></param>
        /// <param name="actComplete_"></param>
        /// <param name="bNeedResponse_"></param>
        public void Send2MasterOnly(TEnum euRequestType_, object tRequestData_, Action<XDataComm<TEnum>.SendCompleteArgs> actComplete_, bool bNeedResponse_ = true)
        {
            var srvMaster = GetMasterServer();
            if(srvMaster == null)
            {
                if (actComplete_ != null)
                {
                    actComplete_(new XDataComm<TEnum>.SendCompleteArgs(new XNetException("No master")));
                }

                return;
            }

            srvMaster.CommClient.SendAsyn(srvMaster.ServerName, euRequestType_, tRequestData_, actComplete_, bNeedResponse_);
        }

        /// <summary>
        /// 给其他人发数据
        /// </summary>
        /// <param name="strTo_">接收者</param>
        /// <param name="euRequestType_"></param>
        /// <param name="tRequestData_"></param>
        /// <param name="actComplete_"></param>
        /// <param name="bNeedResponse_"></param>
        public void Send2Other(string strTo_, TEnum euRequestType_, object tRequestData_, Action<XDataComm<TEnum>.SendCompleteArgs> actComplete_, bool bNeedResponse_ = false)
        {
            var srvMaster = GetMasterServer();
            if (srvMaster == null)
            {
                if (actComplete_ != null)
                {
                    actComplete_(new XDataComm<TEnum>.SendCompleteArgs(new XNetException("No master")));
                }

                return;
            }

            srvMaster.CommClient.SendAsyn(strTo_, euRequestType_, tRequestData_, actComplete_, bNeedResponse_);
        }

        /// <summary>
        /// 需要发给所有服务器的命令：
        /// 先发送给主服务器，如果发送成功后再发送给其他服务器（如果失败，激发OnError），
        /// 如果失败，则直接返回，不会继续给其他服务器发送命令
        /// </summary>
        /// <param name="euRequestType_"></param>
        /// <param name="tRequestData_"></param>
        /// <param name="actComplete_"></param>
        /// <param name="bNeedResponse_"></param>
        public void Send2AllServer(TEnum euRequestType_, object tRequestData_, Action<XDataComm<TEnum>.SendCompleteArgs> actComplete_, bool bNeedResponse_ = true)
        {
            Send2MasterOnly(euRequestType_, tRequestData_, (zarg) =>
                {
                    if(zarg.IsSuccess)
                    {
                        foreach(var srv in GetOtherServers())
                        {
                            SendData(srv, euRequestType_, tRequestData_, bNeedResponse_);
                        }
                    }

                    if(actComplete_ != null)
                    {
                        actComplete_(zarg);
                    }
                },
                bNeedResponse_);
        }

        /// <summary>
        /// 发送给指定服务器的命令
        /// </summary>
        /// <param name="srvComm_"></param>
        /// <param name="euRequestType_"></param>
        /// <param name="tRequestData_"></param>
        /// <param name="actComplete_"></param>
        /// <param name="bNeedResponse_"></param>
        public void Send2Server(XDataComm<TEnum> srvComm_, TEnum euRequestType_, object tRequestData_, Action<XDataComm<TEnum>.SendCompleteArgs> actComplete_, bool bNeedResponse_ = true)
        {
            IMClientSrvInfo srvInfo = null;
            lock(_lockClientSrvs)
            {
                srvInfo = _lstClientSrvs.FirstOrDefault(z => z.CommClient == srvComm_);
            }
            if (srvInfo == null)
            {
                if (actComplete_ != null)
                {
                    actComplete_(new XDataComm<TEnum>.SendCompleteArgs(new XNetException("Invalid Server")));
                }

                return;
            }

            srvInfo.CommClient.SendAsyn(srvInfo.ServerName, euRequestType_, tRequestData_, actComplete_, bNeedResponse_);
        }

        private void SendData(IMClientSrvInfo srvInfo_, TEnum euRequestType_, object tRequestData_, bool bNeedResponse_)
        {
            srvInfo_.CommClient.SendAsyn(srvInfo_.ServerName, euRequestType_, tRequestData_, (zarg) =>
                {
                    if (!zarg.IsSuccess)
                    {
                        InvokeError(srvInfo_, zarg.Result);
                    }
                },
                bNeedResponse_);
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 
        /// </summary>
        class IMClientSrvInfo
        {
            public IMClientSrvInfo(XIMConnection imConnect_, string strServer_)
            {
                ServerName = strServer_;
                CommClient = new XDataComm<TEnum>(imConnect_);
            }

            public bool IsLogin {get;set;}
            public bool HasTryLog { get; set; }
            //public Thread ThrReconnect {get;set;}
            public string ServerName { get; private set; }
            public XDataComm<TEnum> CommClient { get; private set; }
        }
    }
}
