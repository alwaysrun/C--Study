using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Common;
using System.Threading;
using SHCre.Xugd.CFile;

namespace SHCre.Xugd.Net.Xmpp
{
    /// <summary>
    /// 热备客户端,只能用于X86平台程序：
    /// 依赖Library\xugd.xmpp.dll与xugd.clib.dll库，
    /// 先通过AddClientServer添加Xmpp服务器信息，然后调用LoginSync登录
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public partial class XStandbyClient<TEnum> : XLogEventsBase where TEnum : IComparable
    {
        XStandbyClientConfig _conClient = null; 

        /// <summary>
        /// 用于客户端登录的命令
        /// </summary>
        public TEnum ClientLoginCode { get; private set; }
        /// <summary>
        /// 主服务器变化的命令
        /// </summary>
        public TEnum ServerMasterCode { get; private set; }

        /// <summary>
        /// 登录用户名
        /// </summary>
        public string LoginUser {
            get 
            {
                if(_conClient!=null)
                {
                    if (_conClient.StandbyServer.Count > 0)
                        return _conClient.StandbyServer[0].XmppServer.LoginInfo.UserName;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="euLoginCode_">用于客户端登录的命令</param>
        /// <param name="euSrvMaster_">主服务器变化的命令</param>
        public XStandbyClient(TEnum euLoginCode_, TEnum euSrvMaster_)
        {
            ClientLoginCode = euLoginCode_;
            ServerMasterCode = euSrvMaster_;

            BuildLogPrefix<TEnum>("XStandbyClient");
            _chkAliveWork.LogPrefix = "ChkAliveWork.";

            _chkAliveWork.OnLogger += this.InvokeOnLogger;
            _chkAliveWork.OnExcept += this.InvokeOnExcept;
            _chkAliveWork.SetWorkInfo(ToCheckAliveOthers, null);
        }

        /// <summary>
        /// 是否启用Debug输出（启用后以LogLevels.Debug等级输出OnLogger）
        /// </summary>
        /// <param name="bEnabled_"></param>
        public override void SetDebugEnabled(bool bEnabled_)
        {
            base.SetDebugEnabled(bEnabled_);
            foreach(var srv in AllSrvComm)
            {
                srv.SetDebugEnabled(bEnabled_);
                var netCon = srv.NetComm as XLogEventsBase;
                if (netCon != null)
                    netCon.SetDebugEnabled(bEnabled_);
            }

            _chkAliveWork.SetDebugEnabled(bEnabled_);
        }

        /// <summary>
        /// 添加xmpp服务器（如Openfire）信息
        /// </summary>
        /// <param name="conClient_">xmpp服务器（如Openfire）信息</param>
        public void AddClientServer(XStandbyClientConfig conClient_){
            InvokeOnCalled("AddClient(Count:{0})", conClient_.StandbyServer.Count(z=>z.Enabled));
            if (conClient_.StandbyServer.Count(z=>z.Enabled) == 0)
                throw new ArgumentException("StandbyServer at least one enabled");

            List<string> lstServer = new List<string>(conClient_.StandbyServer.Count);
            foreach(var srv in conClient_.StandbyServer){
                if (!srv.Enabled) continue;

                if(lstServer.Contains(srv.XmppServer.LoginInfo.Address)){
                    throw new ArgumentException("StandbyServer has duplicate address");
                }

                lstServer.Add(srv.XmppServer.LoginInfo.Address);
            }
            InvokeOnLogger(XLogSimple.LogLevels.Info, "AddClientServer: {0}", string.Join(";", lstServer));

            LoginTimeoutSeconds = conClient_.LoginTimeoutSecond;
            LoginRetrySeconds = conClient_.LoginRetrySecond;
            _conClient = conClient_;
        }

    } // XStandbyClient
} // ns
