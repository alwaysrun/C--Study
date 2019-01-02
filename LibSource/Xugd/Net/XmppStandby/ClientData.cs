using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Extension;
using System.Threading;
using SHCre.Xugd.Common;
using SHCre.Xugd.CFile;
using SHCre.Xugd.Data;

namespace SHCre.Xugd.Net.Xmpp
{
    partial class XStandbyClient<TEnum>
    {
        readonly object _lockClientSrvs = new object();
        List<IMClientSrvInfo> _lstClientSrvs = new List<IMClientSrvInfo>();
        IMClientSrvInfo _srvMaster = null;
        /// <summary>
        /// 获取添加过的通讯信息
        /// </summary>
        public List<XDataComm<TEnum>> AllSrvComm { get { return _lstClientSrvs.Select(z => z.CommClient).ToList(); } }

        /// <summary>
        /// 当前主服务的连接（没有则返回null，并尝试重新连接）
        /// </summary>
        public XDataComm<TEnum> MasterComm
        {
            get
            {
                var srvMaster = GetMasterServer(true);
                return srvMaster == null ? null : srvMaster.CommClient;
            }
        }

        /// <summary>
        /// 请求达到时触发的事件（不能阻塞，所有Openfire相关事件与应答使用同一个线程）
        /// </summary>
        public event Action<XDataComm<TEnum>, XDataComm<TEnum>.RequestArrivalArgs> OnRequestArrival;
        void InvokeRequestArrival(IMClientSrvInfo srvInfo_, XDataComm<TEnum>.RequestArrivalArgs argRequest_)
        {
            if (OnRequestArrival != null)
                OnRequestArrival(srvInfo_.CommClient, argRequest_);
        }

        /// <summary>
        /// 备用机上执行命令出错时，返回的信息（CmdType, RemoteAddr, Exception)
        /// </summary>
        public event Action<TEnum, string, Exception> OnStandbyCmdFail;
        void InvokeStandbyCmdFail(TEnum euType_, string strRemote_, Exception exErr_)
        {
            if (OnStandbyCmdFail != null)
                OnStandbyCmdFail(euType_, strRemote_, exErr_);
        }

        /// <summary>
        /// 主服务器变化时触发的事件（如果参数为NULL，说明当前没有主服务器）；
        /// 当没有主服务时，内部会自动尝试检测，调用者只需等待即可
        /// </summary>
        public event Action<XDataComm<TEnum>> OnMasterChanged;
        void InvokeMasterChanged()
        {
            if (OnMasterChanged != null)
            {
                IMClientSrvInfo srvInfo = _srvMaster;
                if (srvInfo == null)
                    OnMasterChanged(null);
                else
                    OnMasterChanged(srvInfo.CommClient);
            }
        }
        void ClearMaster()
        {
            lock (_lockClientSrvs)
            {
                _srvMaster = null;
            }
        }

        private IMClientSrvInfo GetMasterServer(bool bTryLoginSrv_ = true)
        {
            bool bTry = false;
            lock (_lockClientSrvs)
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
            if (bTry && IsClientStarted)
            {
                StartCheckImmediately();
            }
            return null;
        }

        private List<IMClientSrvInfo> GetOtherServers()
        {
            lock (_lockClientSrvs)
            {
                return _lstClientSrvs.WhereNoDelay(z => z != _srvMaster);
            }
        }

        void MasterChanged(IMClientSrvInfo srvInfo_, IMServerMasterData masterData_)
        {
            bool bInvoke = false;
            lock (_lockClientSrvs)
            {
                if (masterData_.IsMaster)
                {
                    if (_srvMaster != srvInfo_)
                    {
                        InvokeOnLogger(XLogSimple.LogLevels.Warn, "Master changed to {0}", srvInfo_.ConnClient.RemoteAddress);
                        _srvMaster = srvInfo_;
                        bInvoke = true;
                        StartCheckAliveOthers();
                    }
                }
                else
                {
                    if (_srvMaster == srvInfo_)
                    {
                        InvokeOnLogger(XLogSimple.LogLevels.Warn, "NoMaster: Master {0} change to false", srvInfo_.ConnClient.RemoteAddress);

                        _srvMaster = null;
                        bInvoke = true;
                        StartCheckImmediately();
                    }
                }
            }

            if (bInvoke && IsClientStarted)
            {
                InvokeMasterChanged();
            }
        }

        /// <summary>
        /// 根据远端地址获取服务端连接
        /// </summary>
        /// <param name="strRemoteAddr_"></param>
        /// <returns></returns>
        public XDataComm<TEnum> GetCommByRemoteAddr(string strRemoteAddr_)
        {
            var firstSrv = _lstClientSrvs.FirstOrDefault(z => z.ConnClient.RemoteAddress == strRemoteAddr_);
            if (firstSrv == null) return null;

            return firstSrv.CommClient;
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
        /// <param name="nDataVer_">发送数据的版本(DataWithType.DataVer)</param>
        public void Send2MasterOnly(TEnum euRequestType_, object tRequestData_, Action<XDataComm<TEnum>.SendCompleteArgs> actComplete_, bool bNeedResponse_ = true, int nDataVer_=1)
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

            srvMaster.CommClient.SendAsyn(srvMaster.ServerName, euRequestType_, tRequestData_, nDataVer_, actComplete_, bNeedResponse_);
        }

        /// <summary>
        /// 给主服务器上其他人发数据
        /// </summary>
        /// <param name="strTo_">接收者</param>
        /// <param name="euRequestType_"></param>
        /// <param name="tRequestData_"></param>
        /// <param name="actComplete_"></param>
        /// <param name="bNeedResponse_"></param>
        /// <param name="nDataVer_">发送数据的版本(DataWithType.DataVer)</param>
        public void Send2UserByMaster(string strTo_, TEnum euRequestType_, object tRequestData_, Action<XDataComm<TEnum>.SendCompleteArgs> actComplete_, bool bNeedResponse_ = false, int nDataVer_ = 1)
        {
            var srvMaster = GetMasterServer(false);
            if (srvMaster == null)
            {
                if (actComplete_ != null)
                {
                    actComplete_(new XDataComm<TEnum>.SendCompleteArgs(new XNetException("No master")));
                }

                return;
            }

            srvMaster.CommClient.SendAsyn(strTo_, euRequestType_, tRequestData_, nDataVer_, actComplete_, bNeedResponse_);
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
        /// <param name="nDataVer_">发送数据的版本(DataWithType.DataVer)</param>
        public void Send2AllServer(TEnum euRequestType_, object tRequestData_, Action<XDataComm<TEnum>.SendCompleteArgs> actComplete_, bool bNeedResponse_ = true, int nDataVer_ = 1)
        {
            Send2MasterOnly(euRequestType_, tRequestData_, (zarg) =>
                {
                    if (zarg.IsSuccess)
                    {
                        foreach (var srv in GetOtherServers())
                        {
                            SendData(srv, euRequestType_, tRequestData_, bNeedResponse_, nDataVer_);
                        }
                    }

                    if (actComplete_ != null)
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
        /// <param name="nDataVer_">发送数据的版本(DataWithType.DataVer)</param>
        public void Send2Server(XDataComm<TEnum> srvComm_, TEnum euRequestType_, object tRequestData_, Action<XDataComm<TEnum>.SendCompleteArgs> actComplete_, bool bNeedResponse_ = true, int nDataVer_ = 1)
        {
            IMClientSrvInfo srvInfo = null;
            lock (_lockClientSrvs)
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

            srvInfo.CommClient.SendAsyn(srvInfo.ServerName, euRequestType_, tRequestData_, nDataVer_, actComplete_, bNeedResponse_);
        }

        private void SendData(IMClientSrvInfo srvInfo_, TEnum euRequestType_, object tRequestData_, bool bNeedResponse_, int nDataVer_)
        {
            srvInfo_.CommClient.SendAsyn(srvInfo_.ServerName, euRequestType_, tRequestData_, nDataVer_, (zarg) =>
                {
                    if (!zarg.IsSuccess)
                    {
                        InvokeOnExcept(zarg.Result, string.Format("[Send:{0}->{1}, {2}]", srvInfo_.ConnClient.UserJid, srvInfo_.ServerName, srvInfo_.ConnClient.RemoteAddress));
                        InvokeStandbyCmdFail(euRequestType_, srvInfo_.ConnClient.RemoteAddress, zarg.Result);
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
            public IMClientSrvInfo(XmppClient imConnect_, string strServer_)
            {
                ServerName = strServer_.Trim().ToLower();
                ConnClient = imConnect_;
                CommClient = new XDataComm<TEnum>(imConnect_);
                HasLogSrv = false;
                HasTryLog = false;
            }

            public void Disconnect()
            {
                CommClient.Stop();
                ConnClient.LogoutAsync(null);
            }

            public bool IsLogged { get { return ConnClient.IsLogged; } }

            public bool HasLogSrv { get; set; }
            public bool HasTryLog { get; set; }
            public string ServerName { get; private set; }
            public XmppClient ConnClient { get; private set; }
            public XDataComm<TEnum> CommClient { get; private set; }
        }
    } // XStandbyClient
}
