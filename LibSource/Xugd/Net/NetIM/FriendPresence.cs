using System;
using Icld.NetIM.Client;
using SHCre.Xugd.Extension;

namespace SHCre.Xugd.Net
{
    partial class XIMConnection
    {
        private XSafeList<string> _lstFriends = new XSafeList<string>();

        private void ClearFriends()
        {
            _lstFriends.Clear();
        }

        /// <summary>
        /// 获取已登录好友列表
        /// </summary>
        public string[] LoggedFriends
        {
            get { return _lstFriends.ToArray(); }
        }

        /// <summary>
        /// 好友登录时，触发的事件
        /// </summary>
        public event Action<string> OnFriendLogin;
        private void InvokeOnFriendLogin(string strJid_)
        {
            string strName = Jid2Name(strJid_).ToLower();
            if(_lstFriends.AddIfNotExist(strName, z=>z==strName))
            {
                if (OnFriendLogin != null)
                    OnFriendLogin(strName);
            }
        }

        /// <summary>
        /// 好友退出时，触发的事件
        /// </summary>
        public event Action<string> OnFriendLogout;
        private void InvokeOnFriendLogout(string strJid_)
        {
            string strName = Jid2Name(strJid_).ToLower();
            if(_lstFriends.Remove(strName))
            {
                if (OnFriendLogout != null)
                    OnFriendLogout(strName);
            }
        }

        private void FriendPresenceChanged(string strJid_, PresenceType euPresence_)
        {
            switch(euPresence_)
            {
                case PresenceType.Available:
                    InvokeOnFriendLogin(strJid_);
                    break;

                case PresenceType.Unavailable:
                    InvokeOnFriendLogout(strJid_);
                    break;

                default:
                    break;
            }
        }
    }
}
