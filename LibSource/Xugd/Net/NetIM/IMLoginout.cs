using System;
using System.Linq;
using System.Threading;
using SHCre.Xugd.Common;
using System.IO;

namespace SHCre.Xugd.Net
{
    partial class XIMConnection
    {
        const int WaitLoggingTimeout = 45 * 1000;
        private int _nLogTimeoutInterval = WaitLoggingTimeout;
        private XSafeType<bool> _bIsLoginout = new XSafeType<bool>(false);

        // Set by handlLoginout
        private XSafeType<bool> _bHasLogged = new XSafeType<bool>(false);
        private ManualResetEvent _evtWaitLoginout = new ManualResetEvent(false);
        private Exception _exLoginoutError = null;
        private string _strUserPsw;

        #region "Pulic Attri"
        /// <summary>
        /// 登录超时时间
        /// </summary>
        public int LogTimeoutSecond
        {
            get { return XTime.Interval2Second(_nLogTimeoutInterval); }
            set
            {
                if (value < 1)
                    value = 15;

                _nLogTimeoutInterval = XTime.Second2Interval(value);
            }
        }

        /// <summary>
        /// 是否已登录
        /// </summary>
        public bool IsLogged { get { return _bHasLogged.Value; } }

        /// <summary>
        /// IM服务器域名
        /// </summary>
        public string Domain { get; private set; }

        /// <summary>
        /// IM服务器IP地址
        /// </summary>
        public string IPAddr
        {
            get { return _imConnection.Ip; }
        }

        /// <summary>
        /// IM服务器端口号
        /// </summary>
        public int Port
        {
            get { return _imConnection.Port; }
        }

        /// <summary>
        /// 登录IM服务器的用户名
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// 远端信息（地址:端口）
        /// </summary>
        public string RemoteAddress { get; private set;}
        #endregion

        private bool HandleErrorWhenLoginout(Exception exError_)
        {
            if (_bIsLoginout.IsEqual(false))
            {
                return false;
            }

            _exLoginoutError = exError_;
            _evtWaitLoginout.Set();
            return true;
        }

        private void HandleLoginout(bool bIsLogged_)
        {
            _bHasLogged.Set(bIsLogged_);
            _exLoginoutError = null;
            _evtWaitLoginout.Set();
        }

        /// <summary>
        /// 设定服务器地址信息
        /// </summary>
        /// <param name="strSrvDomain_">域名（Openfire服务器名称）</param>
        /// <param name="strSrvIP_">IP地址</param>
        /// <param name="nSrvPort_">链接端口号</param>
        /// <param name="strClientName_">用户名（不能包含@符）</param>
        /// <param name="strPassword_"></param>
        public void SetServerInfo(string strSrvDomain_, string strSrvIP_, int nSrvPort_, string strClientName_, string strPassword_)
        {
            if (strClientName_.Contains(DomainSeparatorInJid))
                throw new ArgumentException("Name can not include @");

            _imConnection.Server = Domain = strSrvDomain_;
            _imConnection.Ip = strSrvIP_;
            _imConnection.Port = nSrvPort_;
            RemoteAddress = string.Format("{0}:{1}", strSrvIP_, nSrvPort_);

            UserName = strClientName_;
            _strUserPsw = strPassword_;
        }

        /// <summary>
        /// 设定服务器地址信息
        /// </summary>
        /// <param name="imConfig_"></param>
        public void SetServerInfo(XNetIMConfig imConfig_)
        {
            SetServerInfo(imConfig_.Domain, imConfig_.Address, imConfig_.Port, imConfig_.UserName, imConfig_.Password);
        }

        /// <summary>
        /// 登录服务器，如果失败抛出异常:
        /// XLoginoutException：登录出错；
        /// AuthenticationException：用户名、密码验证出错；
        /// TimeoutException：等待超时
        /// </summary>
        public void LoginSync()
        {
            if (_bHasLogged.IsEqual(true))
                return;
            if (_bIsLoginout.EqualOrSet(true))
            { // is logging
                throw new XLoginoutException("Another thread is loginout now.");
            }

            try
            {
                _exLoginoutError = null;
                _evtWaitLoginout.Reset();
                _imConnection.Connect(UserName, _strUserPsw);

                if (!_evtWaitLoginout.WaitOne(_nLogTimeoutInterval))
                { // Timeout
                    throw new TimeoutException(string.Format("Login {0}@{1} failed: No response in {2}s", UserName, Domain, LogTimeoutSecond));
                }

                if (_exLoginoutError != null)
                    throw _exLoginoutError;
            }
            finally
            {
                _bIsLoginout.Set(false);
            }
        }

        /// <summary>
        /// 异步登录
        /// </summary>
        /// <param name="actComplete_"></param>
        public void LoginAsync(Action<XAsyncResult> actComplete_)
        {
            Thread thrLogin = new Thread(() =>
                {
                    Exception exErr = null;
                    try
                    {
                        LoginSync();
                    }
                    catch (Exception ex)
                    {
                        exErr = ex;
                    }

                    if (actComplete_ != null)
                        actComplete_(XAsyncResult.Get(exErr));
                });
            thrLogin.Start();
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="actComplete_"></param>
        public void LogoutAsync(Action<XAsyncResult> actComplete_)
        {
            Thread thrLogout = new Thread(() =>
            {
                Exception exErr = null;
                try
                {
                    LogoutSync();
                }
                catch (Exception ex)
                {
                    exErr = ex;
                }

                if (actComplete_ != null)
                    actComplete_(XAsyncResult.Get(exErr));
            });
            thrLogout.Start();
        }

        /// <summary>
        /// 如果失败抛出异常:
        /// XLoginoutException：登录出错；
        /// TimeoutException：等待超时
        /// </summary>
        public void LogoutSync()
        {
            if (_bHasLogged.IsEqual(false))
                return;
            if (_bIsLoginout.EqualOrSet(true))
            { // is logging
                throw new XLoginoutException("Another thread is loginout now.");
            }

            try
            {
                _exLoginoutError = null;
                _evtWaitLoginout.Reset();
                _imConnection.Disconnect();

                if (!_evtWaitLoginout.WaitOne(_nLogTimeoutInterval))
                { // Timeout
                    throw new TimeoutException(string.Format("No response in {0}s", LogTimeoutSecond));
                }

                if (_exLoginoutError != null)
                    throw new XLoginoutException("Logout failed", _exLoginoutError);
            }
            finally
            {
                _bIsLoginout.Set(false);
            }
        }
    } // class
}
