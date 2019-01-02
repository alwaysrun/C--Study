using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Data;

namespace SHCre.Xugd.Net
{
    partial class XIMRedundantServer<TEnum>
    {
        /// <summary>
        /// 数据到达时，触发OnRequestArrival(Request,IsSync)：
        /// 如果IsSync是true，说明是其他服务器上的同步用户发送过来的数据；
        /// 否则说明是客户端发送过来的数据
        /// </summary>
        public event Action<XDataComm<TEnum>.RequestArrivalArgs, bool> OnRequestArrival;
        void InvokeRequestArrival(XDataComm<TEnum>.RequestArrivalArgs arg_)
        {
            if (OnRequestArrival != null)
            {
                OnRequestArrival(arg_, IsSyncUser(arg_.From));
            }
        }

        /// <summary>
        /// 错误发生时触发
        /// </summary>
        public event Action<XDataComm<TEnum>, Exception> OnError;
        void InvokeError(XDataComm<TEnum> dataComm_, Exception ex_)
        {
            if (OnError != null)
                OnError(dataComm_, ex_);
        }

        /// <summary>
        /// 连接断开时触发(IsClosed):
        /// 如果IsClosed为true说明是退出，否则是由于出错断开
        /// </summary>
        public event Action<bool> OnDisconnected;
        void InvokeDisconnected(bool bClose_)
        {
            ClearClientUsers();
            ClearSyncUsers();
            if (OnDisconnected != null)
                OnDisconnected(bClose_);
        }

        /// <summary>
        /// 发送给客户端数据
        /// </summary>
        /// <param name="strClient_"></param>
        /// <param name="euRequestType_"></param>
        /// <param name="tRequestData_"></param>
        /// <param name="actComplete_"></param>
        /// <param name="bNeedResponse_"></param>
        public void Send2Client(string strClient_, TEnum euRequestType_, object tRequestData_, Action<XDataComm<TEnum>.SendCompleteArgs> actComplete_, bool bNeedResponse_ = false)
        {
            SrvDataComm.SendAsyn(strClient_, euRequestType_, tRequestData_, actComplete_, bNeedResponse_);
        }

        /// <summary>
        /// 发送给其他服务端（同步服务器）数据
        /// </summary>
        /// <param name="euRequestType_"></param>
        /// <param name="tRequestData_"></param>
        public void Send2Server(TEnum euRequestType_, object tRequestData_)
        {
            _lstOutSyncInfos.ForEach(
                z => Send2SyncSrv(z.SyncSrv, euRequestType_, tRequestData_),
                zc => zc.SyncSrv.HasLogin
                );
        }

        void Send2SyncSrv(XIMRedundantSync<TEnum> synSrv_, TEnum euRequestType_, object tRequestData_)
        {
            synSrv_.SendAsync(euRequestType_, tRequestData_, (zop) =>
                    {
                        if (!zop.IsSuccess)
                            InvokeError(synSrv_.CommSync, zop.Result);
                    }, false);
        }
    }
}
