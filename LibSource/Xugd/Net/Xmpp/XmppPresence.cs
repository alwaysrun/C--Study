using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Extension;

namespace SHCre.Xugd.Net.Xmpp
{
    partial class XmppClient
    {
        XSafeList<string> _lstFriends = new XSafeList<string>();

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
        /// 好友登录时，触发的事件；
        /// </summary>
        public event Action<string> OnFriendLogin;
        private void InvokeOnFriendLogin(string strJid_)
        {
            if (_lstFriends.AddIfNotExist(strJid_, z => z == strJid_))
            {
                if (OnFriendLogin != null)
                    OnFriendLogin(strJid_);
            }
        }

        /// <summary>
        /// 好友退出时，触发的事件
        /// </summary>
        public event Action<string> OnFriendLogout;
        private void InvokeOnFriendLogout(string strJid_)
        {
            // if (_lstFriends.Remove(strJid_))
            _lstFriends.Remove(strJid_);
            { // When open-fire and Xmpp at same PC, Presence may come earlier before login-return.
                if (OnFriendLogout != null)
                    OnFriendLogout(strJid_);
            }
        }
    }
}
