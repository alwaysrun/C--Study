using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using SHCre.Xugd.Data;
using SHCre.Xugd.CFile;
using System.Net;
using SHCre.Xugd.Extension;
using System.Net.Sockets;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.Net
{
    /// <summary>
    /// 网络连接的服务端基类
    /// </summary>
    public abstract class XCommServer : IXConnection
    {
        #region "Override"
        /// <summary>
        /// 是否已登录(开始侦听，即为登录)
        /// </summary>
        public bool IsLogged 
        {
            get { return IsListening; }
        }

        /// <summary>
        /// 远端信息（空）
        /// </summary>
        public string RemoteAddress 
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// 开始侦听
        /// </summary>
        public event Action OnConnected;
        void InvokeConnected()
        {
            if (OnConnected != null)
                OnConnected();
        }

        /// <summary>
        /// 停止侦听：
        /// 如果IsClosed为true说明是退出，否则是由于出错断开
        /// </summary>
        public event Action<bool> OnDisconnected;
        void InvokeDisconnected(bool bIsClosed_)
        {
            if (OnDisconnected != null)
                OnDisconnected(bIsClosed_);
        }

        /// <summary>
        /// 当遇到错误时，激发的事件
        /// </summary>
        public event Action<Exception> OnError;
        /// <summary>
        /// 激发错误
        /// </summary>
        /// <param name="ex"></param>
        internal protected void InvokeError(Exception ex)
        {
            if(OnError != null)
                OnError(ex);
        }

        /// <summary>
        /// 数据接收到的事件
        /// </summary>
        public event Action<XReceiveDataArgs> OnDataReceived;
        internal void InvokeDataReceived(XReceiveDataArgs dataArg_)
        {
            if(OnDataReceived != null)
                OnDataReceived(dataArg_);
        }
        #endregion

        #region "Act event"
        /// <summary>
        /// 有新的连接到来
        /// </summary>
        public event Action<XCommConnection> OnConnectionAdded;
        void InvokeConnectionAdded(XCommConnection comConnection_)
        {
            if (OnConnectionAdded != null)
                OnConnectionAdded(comConnection_);
        }

        /// <summary>
        /// 有连接移除（断开）
        /// </summary>
        public event Action<XCommConnection> OnConnectionRemoved;
        void InvokeConnectionRemoved(XCommConnection comConnection_)
        {
            if (OnConnectionRemoved != null)
                OnConnectionRemoved(comConnection_);
        }
        
        /// <summary>
        /// 侦听出错时，触发
        /// </summary>
        public event Action<XCommListener, Exception> OnListenError;
        /// <summary>
        /// 侦听出错时，激发事件
        /// </summary>
        /// <param name="comListener_"></param>
        /// <param name="ex_"></param>
        internal protected void InvokeListenError(XCommListener comListener_, Exception ex_)
        {
            if (OnListenError != null)
                OnListenError(comListener_, ex_);

            if(_bIsListening)
            {
                LogFile.WriteLine(ex_, "Listener {0}", comListener_.ListenEndPoint);

                _bIsListening = _lstListeners.Any(z => z.IsListening);
                if (!_bIsListening)
                    InvokeDisconnected(false);

                LoginAsync(null);   // ReListen
            }
        }
        #endregion

        #region "Var"

        /// <summary>
        /// 侦听配置信息
        /// </summary>
        public XCommServerConfig ServerConfig {get; private set;}

        /// <summary>
        /// 接收缓存大小，应用程序可以根据经常发送数据包的大小来设置
        /// 如果经常发很小的数据包，就设置小一点，如果经常发很大的，就设置大一点，
        /// 以便更有效的利用内存
        /// </summary>
        public int ReceiveBufferSize { get; set; }

        /// <summary>
        /// 发送超时时间
        /// </summary>
        protected int SendSynTimeoutMSec = 0;

        private bool _bIsListening = false;
        private bool IsListening
        {
            get 
            {
                return _bIsListening;
            }

            set 
            {
                if(value)
                {
                    int nCount = _lstListeners.FilterCount(z => z.IsListening);
                    LogFile.Info("Listen Count:{0}", nCount);
                    if (nCount > 0)
                    {
                        _bIsListening = true;
                        InvokeConnected();
                    }
                }
                else
                {
                    _bIsListening = false;
                }
            }
        }

        /// <summary>
        /// 日志文件
        /// </summary>
        internal protected XLogSimple LogFile {get;private set;}

        /// <summary>
        /// 此Server侦听的端口信息
        /// </summary>
        protected XSafeList<XCommListener> _lstListeners = new XSafeList<XCommListener>();

        /// <summary>
        /// 所有连接
        /// </summary>
        protected XSafeList<XCommConnection> _lstConnections = new XSafeList<XCommConnection>();

        /// <summary>
        /// 当前此服务端的所有侦听者
        /// </summary>
        public XCommListener[] Listeners
        {
            get { return _lstListeners.ToArray(); }
        }

        /// <summary>
        /// 当前此服务端的所有连接
        /// </summary>
        public XCommConnection[] Connections
        {
            get { return _lstConnections.ToArray(); }
        }

        /// <summary>
        /// 查找指定的连接
        /// </summary>
        /// <param name="strRemoteAddr_"></param>
        /// <returns></returns>
        public XCommConnection GetConnection(string strRemoteAddr_)
        {
            return _lstConnections.FirstOrDefault(z => z.RemoteAddress == strRemoteAddr_);
        }
        #endregion
        
        /// <summary>
        /// 
        /// </summary>
        public XCommServer(XCommServerConfig srvConfig_)
        {
            ReceiveBufferSize = DefaultValue.ReceiveBuffSize;

            LogFile = new XLogSimple(srvConfig_.CommLog);
            LogFile.VerInfo("CommServer", "14.929");

            ServerConfig = srvConfig_;
            SendSynTimeoutMSec = XTime.Second2Interval(ServerConfig.SendSynTimeoutSecond);
        }

        /// <summary>
        /// 开始侦听并接受连接请求
        /// </summary>
        public virtual void Start()
        {
            if (IsListening) return;
            LogFile.Called("Start()");

            Clear();

            LogFile.Info("Listen-addr count:{0}", ServerConfig.ListenAddresses.Count);
            foreach (var addr in ServerConfig.ListenAddresses)
            {
                var endPoint = addr.ListenIPEndPoint();
                LogFile.Info("CommServer: add listener {0}", addr.PrintAddr());
                var listener = CreateListener(endPoint, addr.SrvId);
                listener.Start();

                _lstListeners.Add(listener);
            }

            LogFile.Print("CommServer Started");
            IsListening = true;
        }

        private void Clear()
        {
            _lstConnections.Collect.ForEach(z => z.Close());
            _lstConnections.Clear();
            _lstListeners.Clear(z => z.Stop());
        }

        /// <summary>
        /// 停止侦听并断开所有连接
        /// </summary>
        public virtual void Stop()
        {
            if (!IsListening) return;

            Clear();

            IsListening = false;
            InvokeDisconnected(true);

            LogFile.Print("CommServer Stopped");
            LogFile.Close();
        }

        /// <summary>
        /// 异步登录(重新开始侦听，需要已启动过)
        /// </summary>
        /// <param name="actComplete_"></param>
        public void LoginAsync(Action<XAsyncResult> actComplete_)
        {
            if(_lstListeners.Count == 0)
            {
                XAsyncResult.InvokeAct(actComplete_, new XConnectException("Not listener"));
                return;
            }

            Exception exError = null;
            try 
            {
                _lstListeners.ForEach(z => z.Start());

                IsListening = true;
            }
            catch(Exception ex)
            {
                exError = ex;
            }

            XAsyncResult.InvokeAct(actComplete_, exError);
        }

        /// <summary>
        /// 登出（停止监听）
        /// </summary>
        /// <param name="actComplete_"></param>
        public void LogoutAsync(Action<XAsyncResult> actComplete_)
        {
            Exception exError = null;
            try
            {
                Stop();
            }
            catch(Exception ex)
            {
                exError = ex;
            }
            XAsyncResult.InvokeAct(actComplete_, exError);
        }

        /// <summary>
        /// 发送数据的接口：
        /// 发送成功Action中的Exception为null，否则为具体的错误信息
        /// </summary>
        /// <param name="strTo">远端地址</param>
        /// <param name="strJson_"></param>
        /// <param name="actComplete_"></param>
        /// <param name="oTag_"></param>
        public void SendAsync(string strTo, string strJson_, Action<XAsyncResult> actComplete_, object oTag_ = null)
        {
            var conTo = GetConnection(strTo);
            if(conTo == null)
            {
                if (actComplete_ != null)
                {
                    var ex = new XNotLoginException("Not Login")
                    {
                        UserName = strTo,
                    };
                    actComplete_(XAsyncResult.Get(ex));
                }
                return;
            }

            conTo.SendAsync(strTo, strJson_, actComplete_);
        }

        /// <summary>
        /// 发送数据到客户端
        /// </summary>
        /// <param name="strSender_">发送者信息</param>
        /// <param name="toPoint_"></param>
        /// <param name="byData_"></param>
        /// <param name="actSend_"></param>
        public virtual void Send2Client(string strSender_, IPEndPoint toPoint_, byte[] byData_, Action<Exception> actSend_)
        {
            throw new NotImplementedException("CommServer.Send2Client: Should not call it");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comConnection_"></param>
        /// <param name="bAdd_"></param>
        internal protected void ProcConnectionChanged(XCommConnection comConnection_, bool bAdd_)
        {
            if (bAdd_)
            {
                if (!_lstConnections.AddIfNotExist(comConnection_))
                    return;

                LogFile.Info("Add Connection {0}", comConnection_.RemoteAddress);
                InvokeConnectionAdded(comConnection_);
            }
            else
            {
                if (!_lstConnections.Remove(comConnection_))
                    return;

                LogFile.Info("Remove Connection {0}", comConnection_.RemoteAddress);
                InvokeConnectionRemoved(comConnection_);
            }
        }

        internal void RawDataReceived(string strFrom_, byte[] byData_, int nOffset_, int nCount_)
        {
            var conFirst = _lstConnections.FirstOrDefault(z => z.NetUser == strFrom_);
            if(conFirst == null)
            {
                LogFile.Error("RawDataReceived(From: {0}): no client correspond", strFrom_);
                return;
            }

            XThread.StartPool((object none_) =>
                {
                    conFirst.SelfRawDataReceived(byData_, nOffset_, nCount_);
                });
        }

        internal void ClientLogout(string strFrom_)
        {
            var conFirst = _lstConnections.FirstOrDefault(z => z.NetUser == strFrom_);
            if(conFirst != null)
            {
                LogFile.Info("Client {0} Close", strFrom_);
                conFirst.Close();
            }
        }

        /// <summary>
        /// 衍生类需要在此方法中创建Listener，可以支持多个Listener
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        protected abstract XCommListener CreateListener(IPEndPoint endPoint, object tag);
    }
}
