using System;
using SHCre.Xugd.Extension;
using System.IO;
using SHCre.Xugd.Data;

namespace SHCre.Xugd.Net
{
    /// <summary>
    /// IM双机热备的客户端：
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    [Obsolete("Use SHCre.Xugd.Net.Xmpp.XStandbyClient replace this")]
    public partial class XIMRedundantClient<TEnum> where TEnum : IComparable
    {
        //int _nResponseWaitSeconds = 0;

        /// <summary>
        /// 用于客户端登录的命令
        /// </summary>
        public TEnum ClientLoginCode { get; private set; }
        /// <summary>
        /// 主服务器变化的命令
        /// </summary>
        public TEnum ServerMasterCode { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="euLoginCode_">用于客户端登录的命令</param>
        /// <param name="euSrvMaster_">主服务器变化的命令</param>
        public XIMRedundantClient(TEnum euLoginCode_, TEnum euSrvMaster_)
        {
            ClientLoginCode = euLoginCode_;
            ServerMasterCode = euSrvMaster_;
            //_nResponseWaitSeconds = nResponseWaitSeconds_;
        }

        /// <summary>
        /// 增加客户端到Openfire服务器的连接
        /// </summary>
        /// <param name="imConnect_">IM连接</param>
        /// <param name="strServer_">服务器用户名</param>
        /// <param name="nRespondTimeoutSecond_">应答超时时间（传入0则为45s）</param>
        /// <returns>成功添加则返回添加的通讯信息，否则返回null(一般是因为已存在相同连接)</returns>
        public XDataComm<TEnum> AddClientServer(XIMConnection imConnect_, string strServer_, int nRespondTimeoutSecond_=0)
        {
            strServer_ = strServer_.ToLower();
            lock (_lockClientSrvs)
            {
                var srvInfo = new IMClientSrvInfo(imConnect_, strServer_);
                if (_lstClientSrvs.AddIfNotExist(
                        srvInfo,
                        (z) => (z.CommClient.NetComm == imConnect_||z.CommClient.NetComm.RemoteAddress==imConnect_.RemoteAddress)
                        )
                    )
                {
                    imConnect_.OnDisconnected += (zClose) => ImConnect_OnDisconnected(srvInfo, zClose);
                    imConnect_.OnError += (zErr) => ImConnect_OnError(srvInfo, zErr);
                    imConnect_.OnFriendLogin += (zName) => ImConnect_OnFriendLogin(srvInfo, strServer_, zName);
                    imConnect_.OnFriendLogout += (zName) => ImConnect_OnFriendLogout(srvInfo, strServer_, zName);
                    
                    srvInfo.CommClient.ResponseWaitSeconds = nRespondTimeoutSecond_;
                    srvInfo.CommClient.OnRequestArrival += (zarg) => CommClient_OnRequestArrival(srvInfo, zarg);
                    srvInfo.CommClient.OnDecodeDataFail += (zarg) => CommClient_OnDecodeDataFail(srvInfo, zarg);
                    srvInfo.CommClient.OnResponseMismatch += (zarg) => CommClient_OnResponseMismatch(srvInfo, zarg);

                    return srvInfo.CommClient;
                }
            }

            return null;
        }

        /// <summary>
        /// 增加客户端到Openfire服务器的连接
        /// </summary>
        /// <param name="redConfig_"></param>
        /// <returns></returns>
        public XDataComm<TEnum> AddClientServer(XIMRedundClientConfig redConfig_)
        {
            if (!redConfig_.Enabled)
                throw new ArgumentException("Config not enabled");

            var imConn = new XIMConnection();
            imConn.SetServerInfo(redConfig_.IMServer);

            return AddClientServer(imConn, redConfig_.ServerUser, redConfig_.RespondTimeoutSecond);
        }

        /// <summary>
        /// 清空客户端
        /// </summary>
        public void ClearClientServer()
        {
            HasStarted = false;
            _evtStoptReconnect.Set();
            lock (_lockClientSrvs)
            {
                var lstSrvs = _lstClientSrvs.WhereNoDelay();
                lstSrvs.ForEach(z => Disconnect(z));
                _lstClientSrvs.Clear();
            }
        }

        void CommClient_OnResponseMismatch(IMClientSrvInfo srvInfo_, XDataComm<TEnum>.ResponseMismatchArgs argData_)
        {
            string strInfo = string.Format("Response-Mismatch(From:{0}, Type:{1}, Index:{2}) Mismatch", argData_.From, argData_.DataType, argData_.DataIndex);
            InvokeError(srvInfo_, new XDataHandleException(strInfo)
                {
                    HandleData = argData_.ResponseData,
                    From = argData_.From,
                });
        }

        void CommClient_OnDecodeDataFail(IMClientSrvInfo srvInfo_, XDataComm<TEnum>.ReceiveDataHandleFailedArgs argData_)
        {
            InvokeError(srvInfo_, new XDataHandleException("Decode data failed", argData_.Error)
            {
                HandleData = argData_.Data,
                From = argData_.From,
            });
        }

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
                    InvokeError(srvInfo_, new XDataHandleException("MasterChange: Get data failed", ex)
                        {
                            HandleData = argRequest_.RequestData.DataJson,
                            From = argRequest_.From,
                        });
                }
            }
            else
            {
                InvokeRequestArrival(srvInfo_, argRequest_);
            }
        }

        private void ServerLogout(IMClientSrvInfo srvInfo_)
        {
            bool bInvoke = false;
            lock (_lockClientSrvs)
            {
                if (srvInfo_ == _srvMaster)
                {
                    _srvMaster = null;
                    bInvoke = true;
                }
            }
            if (bInvoke)
                InvokeMasterChanged(null);

            srvInfo_.IsLogin = false;
            InvokeServerDisconnected(srvInfo_);
            //Reconnect(srvInfo_);
        }

        void ImConnect_OnFriendLogout(IMClientSrvInfo srvInfo_, string strServer_, string strLogout_)
        {
            if (strServer_ == strLogout_)
            {
                ServerLogout(srvInfo_);
            }
        }

        void ImConnect_OnFriendLogin(IMClientSrvInfo srvInfo_, string strServer_, string strLogin_)
        {
            if (strServer_ == strLogin_)
            {
                LoginServer(srvInfo_, false);
            }
        }

        void ImConnect_OnError(IMClientSrvInfo srvInfo_, Exception exError_)
        {
            InvokeError(srvInfo_, exError_);
        }

        void ImConnect_OnDisconnected(IMClientSrvInfo srvInfo_, bool bIsClose_)
        {
            if (!bIsClose_)
                Connect(srvInfo_);

            ServerLogout(srvInfo_);
        }
    }
}
