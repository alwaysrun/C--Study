using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Extension;
using SHCre.Xugd.Common;
using System.Threading;

namespace SHCre.Xugd.Net.Xmpp
{
    partial class XStandbyServer<TEnum>
    {
        XSafeList<string> _lstClientUsers = new XSafeList<string>();

        /// <summary>
        /// 客户端用户登录时触发
        /// </summary>
        public event Action<string> OnClientLogin;
        void InvokeClientLogin(string strUser_)
        {
            if (_lstClientUsers.AddIfNotExist(strUser_, z => z == strUser_))
            {
                if (OnClientLogin != null)
                    OnClientLogin(strUser_);

                TrySubscribe(strUser_);
            }
        }

        /// <summary>
        /// 客户端用户退出时触发
        /// </summary>
        public event Action<string> OnClientLogout;
        bool InvokeClientLogout(string strUser_)
        {
            if (_lstClientUsers.Remove(strUser_))
            {
                if (OnClientLogout != null)
                    OnClientLogout(strUser_);

                return true;
            }

            return false;
        }

        private void TrySubscribe(string strUser_)
        {
            XThread.StartPool((zUser) =>
            {
                try
                {
                    Thread.Sleep(100);  // wait a while
                    _srvDataClient.Subscribe(zUser as string);
                }
                catch (Exception ex) 
                {
                    InvokeOnExcept(ex, "TrySubscribe");
                }
            }, strUser_);
        }

        void ClearClientUser()
        {
            _lstClientUsers.Clear();
        }

        void SendMasterChange2User(bool bIsMaster_)
        {
            if (!IsServerStarted) return;

            XThread.StartPool((master_) =>
            {
                IMServerMasterData dataMaster = new IMServerMasterData()
                {
                    IsMaster = (bool)bIsMaster_ ,
                    SrvID = LocalServerID,
                    StartTime = ServerStartTime,
                };

                _lstClientUsers.ForEach(z => SrvDataComm.SendAsyn(z, ServerMasterCode, dataMaster, null, false));
            }, bIsMaster_);
        }
    }
}
