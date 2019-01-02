using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Extension;

namespace SHCre.Xugd.Net
{
    partial class XIMRedundantServer<TEnum>
    {
        XSafeList<string> _lstClientUsers = new XSafeList<string>();

        /// <summary>
        /// 客户端用户登录时触发
        /// </summary>
        public event Action<string> OnClientLogin;
        void InvokeClientLogin(string strUser_)
        {
            if (_lstClientUsers.AddIfNotExist(strUser_, z=>z==strUser_))
            {
                if (OnClientLogin != null)
                    OnClientLogin(strUser_);
            }
        }

        /// <summary>
        /// 客户端用户退出时触发
        /// </summary>
        public event Action<string> OnClientLogout;
        bool InvokeClientLogout(string strUser_)
        {
            if(_lstClientUsers.Remove(strUser_))
            {
                if (OnClientLogout != null)
                    OnClientLogout(strUser_);

                return true;
            }

            return false;
        }

        void ClearClientUsers()
        {
            _lstClientUsers.Clear();
        }

        void SendUserMasterChanged(bool bIsMaster_)
        {
            if (!IsServerStarted)
                return;

            IMServerMasterData dataMaster = new IMServerMasterData()
                {
                    IsMaster = IsMasterServer,
                    SrvID = LocalServerID,
                    StartTime = ServerStartTime,
                };

            _lstClientUsers.ForEach(z => SrvDataComm.SendAsyn(z, ServerMasterCode, dataMaster, null, false));
        }
    }
}
