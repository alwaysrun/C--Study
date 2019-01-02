using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SHCre.Xugd.CFile;
using SHCre.Xugd.Common;
using SHCre.Xugd.Data;

namespace SHCre.Xugd.Net
{
    /// <summary>
    /// 网络连接基类：
    /// 用于维护连接的心跳包（CheckAlive）只有客户端连接类处理，服务端不关心。
    /// </summary>
    public abstract partial class XCommConnection : IXConnection
    {
        #region "Event And Act"
        /// <summary>
        /// 连接成功
        /// </summary>
        public event Action OnConnected;
        void InvokeConnected()
        {
            if (OnConnected != null)
                OnConnected();
        }

        /// <summary>
        /// 连接断开
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
        /// 激发错误事件
        /// </summary>
        /// <param name="ex_"></param>
        protected void InvokeError(Exception ex_)
        {
            if (IsServerEnd())
                Server.InvokeError(ex_);

            if (OnError != null)
            {
                OnError(ex_);
            }
        }

        /// <summary>
        /// 数据接收到的事件
        /// </summary>
        public event Action<XReceiveDataArgs> OnDataReceived;
        void InvokeDataReceived(string strFrom_, string strData_)
        {
            var recData = new XReceiveDataArgs()
                    {
                        From = strFrom_,
                        Data = strData_,
                    };

            if (IsServerEnd())
                Server.InvokeDataReceived(recData);

            if (OnDataReceived != null)
                OnDataReceived(recData);
        }

        /// <summary>
        /// 当接收到数据（From, Datas）时，激发的事件
        /// </summary>
        public event Action<string, XCommRawDataArrivalArgs> OnRawDataArrival;
        void InvokeRawDataArrival(string strFrom_, byte[] byData_, int nOffset_, int nCount_)
        {
            if (OnRawDataArrival != null)
                OnRawDataArrival(strFrom_, new XCommRawDataArrivalArgs()
                    {
                        Data = byData_,
                        Offset = nOffset_,
                        Count = nCount_,
                    });
        }
        #endregion

        #region "Vars"
        /// <summary>
        /// 网络用户（使用远端地址）
        /// </summary>
        public string NetUser { get { return this.RemoteAddress; } }

        #region "Address"
        /// <summary>
        /// 对方IP地址、端口号信息
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; protected set; }

        private string _strRemoteAddr = null;
        /// <summary>
        /// 对方地址信息(如：127.0.0.1:80)
        /// </summary>
        public string RemoteAddress
        {
            get
            {
                if (string.IsNullOrEmpty(_strRemoteAddr) && RemoteEndPoint != null)
                {
                    _strRemoteAddr = RemoteEndPoint.ToString();
                }

                return _strRemoteAddr;
            }
        }

        /// <summary>
        /// 本地IP地址、端口号信息
        /// </summary>
        public IPEndPoint LocalEndPoint { get; protected set; }

        private string _strLocalAddr = null;
        /// <summary>
        /// 本地地址信息(如：127.0.0.1:80)
        /// </summary>
        public string LocalAddress
        {
            get
            {
                if (string.IsNullOrEmpty(_strLocalAddr) && LocalEndPoint != null)
                {
                    _strLocalAddr = LocalEndPoint.ToString();
                }

                return _strLocalAddr;
            }
        }
        #endregion

        /// <summary>
        /// 连接配置，只对客户端有效（服务端使用Server的ListenConfig）
        /// </summary>
        public XCommClientConfig ConnectConfig { get; private set; }

        /// <summary>
        /// 服务端对应的Server，客户端为null
        /// </summary>
        public XCommServer Server { get; private set; }

        /// <summary>
        /// 判断是否是服务端连接
        /// </summary>
        /// <returns></returns>
        protected bool IsServerEnd()
        {
            return (Server != null);
        }

        #region "Encoding set"
        private string _strEncodingName = string.Empty;
        /// <summary>
        /// 字符串的编码格式：
        /// 接收时，如果设定则自动解码所有接收数据(OnDataReceived)，否则不解码；
        /// 发送字符串时，如果设定则使用此方式编码，否则使用UTF8方式编码；
        /// </summary>
        public string EncodingName
        {
            get { return _strEncodingName; }
            set
            {
                if (_strEncodingName == value)
                    return;

                _strEncodingName = value;
                _dataEncoding = null;
            }
        }

        private Encoding _dataEncoding;
        /// <summary>
        /// 数据的编码方式
        /// </summary>
        protected Encoding DataEncoding
        {
            get
            {
                if (_dataEncoding == null)
                {
                    try
                    {
                        _dataEncoding = Encoding.GetEncoding(_strEncodingName);
                    }
                    catch { }

                    if (_dataEncoding == null)
                        _dataEncoding = Encoding.UTF8;
                }
                return _dataEncoding;
            }
        }

        bool IsNeedDecode()
        {
            return !string.IsNullOrEmpty(_strEncodingName);
        }
        #endregion

        private bool _bIsStarted = false;
        private bool _bIsClosing = false;

        /// <summary>
        /// 发送的数据是否携带头信息
        /// </summary>
        public bool IsDataCarryHeader { get; set; }

        /// <summary>
        /// 接收缓存大小，应用程序可以根据经常发送数据包的大小来设置
        /// 如果经常发很小的数据包，就设置小一点，如果经常发很大的，就设置大一点，
        /// 以便更有效的利用内存
        /// </summary>
        public int ReceiveBufferSize { get; set; }

        /// <summary>
        /// 日志文件
        /// </summary>
        protected XLogSimple LogFile
        {
            get
            {
                if (_logFile == null)
                {
                    if (IsServerEnd())
                        _logFile = Server.LogFile;
                }

                return _logFile;
            }
        }
        private XLogSimple _logFile = null;

        private readonly XSafeType<bool> _bConnected = new XSafeType<bool>(false);
        /// <summary>
        /// 是否连接
        /// </summary>
        public bool IsConnected
        {
            get { return _bConnected.Value; }

            protected set
            {
                if (_bConnected.EqualOrSet(value)) return;

                if (value)
                {
                    if (IsServerEnd())
                        LogFile.Info("Connect from {0}", RemoteAddress);
                    else
                        LogFile.Info("Connect OK:{0}->{1}", LocalAddress, RemoteAddress);

                    LastCommunicateTime = DateTime.Now;
                    _bIsStarted = true;

                    StartReceive();
                    SetCheckAliveTimer();

                    InvokeConnected();
                }
                else
                {
                    LogFile.Warn("Disconnect from {0}", RemoteAddress);
                    _strRemoteAddr = null;
                    _strLocalAddr = null;

                    DisableCheckIdle();
                    SetReconnectTimer();
                }

                if (IsServerEnd())
                    Server.ProcConnectionChanged(this, value);
            }
        }

        /// <summary>
        /// 是否连接服务端（与IsConnected同）
        /// </summary>
        public bool IsLogged { get { return IsConnected; } }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conConfig_"></param>
        public XCommConnection(XCommClientConfig conConfig_)
        {
            ConnectConfig = conConfig_;
            _logFile = new XLogSimple(ConnectConfig.CommLog);

            InitBaseConfig(conConfig_);
            _nCheckAliveInter = XTime.Second2Interval(conConfig_.CheckAliveInterSecond);
            _nReconnectInter = XTime.Second2Interval(conConfig_.ReconnectInterSecond);
            _bAutoConnectWhenSend = conConfig_.AutoConnectWhenSend;

            RemoteEndPoint = conConfig_.RemoteAddress.ConnectIPEndPoint();

            ReceiveBufferSize = DefaultValue.ReceiveBuffSize;
            BuildAllTimer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commServer_"></param>
        /// <param name="srvConfig_"></param>
        protected XCommConnection(XCommServer commServer_, XCommServerConfig srvConfig_)
        {
            InitBaseConfig(srvConfig_);
            _nCheckAliveInter = 0;
            _nReconnectInter = 0;
            _bAutoConnectWhenSend = false;

            Server = commServer_;

            ReceiveBufferSize = commServer_.ReceiveBufferSize;
            BuildAllTimer();
        }

        private void InitBaseConfig(XCommBaseConfig conConfig_)
        {
            IsDataCarryHeader = conConfig_.DataCarryHeader;
            EncodingName = conConfig_.EncodingName;

            if (conConfig_.SendSynTimeoutSecond <= 0)
                conConfig_.SendSynTimeoutSecond = DefaultValue.SendTimeOutSecond;
            SendSynTimeoutMSec = XTime.Second2Interval(conConfig_.SendSynTimeoutSecond);

            if (conConfig_.ReceiveTimeoutSecond <= 0)
                conConfig_.ReceiveTimeoutSecond = DefaultValue.ReceiveTimeoutSecond;
            ReceiveTimeoutMSec = XTime.Second2Interval(conConfig_.ReceiveTimeoutSecond);

            _nIdleAliveInter = XTime.Second2Interval(conConfig_.IdleAliveSecond);
        }

        #region "Connect"
        /// <summary>
        /// 实现连接
        /// </summary>
        /// <param name="endPoint_"></param>
        /// <param name="act_"></param>
        protected abstract void ConnectAsyncImpl(IPEndPoint endPoint_, Action<XCommConnection, Exception> act_);

        /// <summary>
        /// 使用RemoteEndPoint连接
        /// </summary>
        public void Connect()
        {
            Connect(RemoteEndPoint);
        }

        /// <summary>
        /// 使用(IP, Port)连接服务器，出错抛出异常
        /// </summary>
        /// <param name="strIp_"></param>
        /// <param name="nPort_"></param>
        public void Connect(string strIp_, int nPort_)
        {
            Connect(new IPEndPoint(IPAddress.Parse(strIp_), nPort_));
        }

        /// <summary>
        /// 使用IPEndPoint连接服务器，出错抛出异常
        /// </summary>
        /// <param name="endPoint_"></param>
        public void Connect(IPEndPoint endPoint_)
        {
            if (IsConnected) return;

            ManualResetEvent conWait = new ManualResetEvent(false);
            Exception exConnect = null;
            ConnectAsync(endPoint_, (zresult) =>
                {
                    exConnect = zresult.Result;
                    conWait.Set();
                });

            conWait.WaitOne();
            if (exConnect != null)
            {
                throw exConnect;
            }
        }

        /// <summary>
        /// 异步登录
        /// </summary>
        /// <param name="actComplete_"></param>
        public void LoginAsync(Action<XAsyncResult> actComplete_)
        {
            ConnectAsync(actComplete_);
        }

        /// <summary>
        /// 使用RemoteEndPoint异步连接服务器
        /// </summary>
        /// <param name="actComplete_"></param>
        public void ConnectAsync(Action<XAsyncResult> actComplete_ = null)
        {
            ConnectAsync(RemoteEndPoint, actComplete_);
        }

        /// <summary>
        /// 使用(IP, Port)异步连接服务器
        /// </summary>
        /// <param name="strIp_"></param>
        /// <param name="nPort_"></param>
        /// <param name="actComplete_"></param>
        public void ConnectAsync(string strIp_, int nPort_, Action<XAsyncResult> actComplete_ = null)
        {
            ConnectAsync(new IPEndPoint(IPAddress.Parse(strIp_), nPort_), actComplete_);
        }

        /// <summary>
        /// 使用EndPoint异步连接服务器
        /// </summary>
        /// <param name="endPoint_"></param>
        /// <param name="actComplete_"></param>
        public void ConnectAsync(IPEndPoint endPoint_, Action<XAsyncResult> actComplete_ = null)
        {
            if (IsServerEnd())
                throw new InvalidOperationException("Server end can not call Connect");

            ConnectAsyncImpl(endPoint_, (zcon, zex) =>
                {
                    _bIsStarted = true;
                    if (zex == null) // success
                    {
                        RemoteEndPoint = endPoint_;
                        IsConnected = true;
                        LogFile.Print("Comm.Connect OK");
                    }
                    else
                    {
                        SetReconnectTimer();
                        LogFile.WriteLine(zex, "Comm.Connect {0}", endPoint_);
                    }

                    XAsyncResult.InvokeAct(actComplete_, zex);
                });
        }
        #endregion

        #region "Receive"
        /// <summary>
        /// 开始
        /// </summary>
        protected abstract void StartReceive();

        /// <summary>
        /// 接收数据失败时调用
        /// </summary>
        /// <param name="ex_"></param>
        protected void DataReceiveFailed(Exception ex_)
        {
            ErrorClose(ex_);

            InvokeError(ex_);
        }

        /// <summary>
        /// 接收到自身的数据时（数据发自DefFrom）
        /// </summary>
        /// <param name="byData_"></param>
        /// <param name="nOffset_"></param>
        /// <param name="nCount_"></param>
        internal protected void SelfRawDataReceived(byte[] byData_, int nOffset_, int nCount_)
        {
            LastCommunicateTime = DateTime.Now;
            LogFile.Receive("SelfRawDataReceived(From:{0}, Count:{1})", this.NetUser, nCount_);

            if (IsDataCarryHeader)
            {
                DataWithHeaderReceived(byData_, nOffset_, nCount_);
            }
            else
            {
                UnpackedData(this.NetUser, byData_, nOffset_, nCount_);
            }
        }

        /// <summary>
        /// 处理接收到的数据
        /// </summary>
        /// <param name="strFrom_"></param>
        /// <param name="byData_"></param>
        /// <param name="nOffset_"></param>
        /// <param name="nCount_"></param>
        protected void UnpackedData(string strFrom_, byte[] byData_, int nOffset_, int nCount_)
        {
            if (IsCheckAlive(byData_, nOffset_, nCount_)) return;

            LogFile.Info("UnpackedData(Count:{0})", nCount_);
            InvokeRawDataArrival(strFrom_, byData_, nOffset_, nCount_);

            if (!IsNeedDecode()) return;
            DecodeReceivedData(strFrom_, byData_, nOffset_, nCount_);
        }

        /// <summary>
        /// 接收到的数据解码后
        /// </summary>
        /// <param name="strFrom_">发送者（来源）</param>
        /// <param name="byData_"></param>
        /// <param name="nOffset_"></param>
        /// <param name="nCount_"></param>
        private void DecodeReceivedData(string strFrom_, byte[] byData_, int nOffset_, int nCount_)
        {
            try
            {
                string strData = DataEncoding.GetString(byData_, nOffset_, nCount_);
                InvokeDataReceived(strFrom_, strData);
            }
            catch (Exception ex)
            {
                LogFile.Except(ex, "CommConnection.DecodeReceivedData");
                InvokeError(new XCommDataDecodeException("Decode failed", ex)
                    {
                        RawData = byData_,
                        Offset = nOffset_,
                        Count = nCount_,
                    });
            }
        }
        #endregion

        #region "Send"
        /// <summary>
        /// 发送数据实现
        /// </summary>
        /// <param name="byData_"></param>
        /// <param name="act_"></param>
        protected abstract void SendAsyncImpl(byte[] byData_, Action<XCommConnection, Exception> act_);

        /// <summary>
        /// 发送字符串（使用DataEncoding编码），失败抛出异常：发送到（To）RemoteAddress。
        /// 超过DataSendTimeout后还未发送完成，会抛出TimeOut异常
        /// </summary>
        /// <param name="strData_"></param>
        public void Send(string strData_)
        {
            Send(DataEncoding.GetBytes(strData_));
        }

        /// <summary>
        /// 发送数据，失败抛出异常：发送到（To）RemoteAddress。
        /// 超过DataSendTimeout后还未发送完成，会抛出TimeOut异常
        /// </summary>
        /// <param name="byData_"></param>
        public void Send(byte[] byData_)
        {
            ManualResetEvent eventSend = new ManualResetEvent(false);
            Exception exSend = null;
            SendAsync(byData_, (act_) =>
                {
                    exSend = act_.Result;
                    eventSend.Set();
                });

            if (!eventSend.WaitOne(SendSynTimeoutMSec))
                exSend = new TimeoutException(string.Format("Send byData_ exceed {0}ms", SendSynTimeoutMSec));
            if (exSend != null)
                throw exSend;
        }

        /// <summary>
        /// 异步发送数据：发送到（To）RemoteAddress
        /// </summary>
        /// <param name="byData_"></param>
        /// <param name="actComplete_"></param>
        public void SendAsync(byte[] byData_, Action<XAsyncResult> actComplete_)
        {
            SendAsync(this.NetUser, byData_, actComplete_);
        }

        /// <summary>
        /// 异步发送数据
        /// </summary>
        /// <param name="strTo_">发送目的地（接受方）</param>
        /// <param name="byData_"></param>
        /// <param name="actComplete_"></param>
        private void SendAsync(string strTo_, byte[] byData_, Action<XAsyncResult> actComplete_)
        {
            if (!IsConnected)
            {
                if (!_bAutoConnectWhenSend)
                {
                    XAsyncResult.InvokeAct(actComplete_, new SocketException((int)SocketError.NotConnected));
                    return;
                }

                // Reconnect
                try
                {
                    Connect();
                }
                catch (Exception ex)
                {
                    XAsyncResult.InvokeAct(actComplete_, ex);
                    return;
                }
            }

            LogFile.Info("SendAsync(To:{0}, Count:{1})", strTo_, byData_.Length);
            SendAsyncImpl(BuildSendData(byData_), (zcon, zex) =>
                {
                    if (zex == null)
                    {
                        LastCommunicateTime = DateTime.Now;
                    }
                    else
                    {
                        ErrorClose(zex);
                    }

                    XAsyncResult.InvokeAct(actComplete_, zex);
                });
        }

        /// <summary>
        /// 异步发送字符串
        /// </summary>
        /// <param name="strData_"></param>
        /// <param name="actComplete_"></param>
        public void SendAsync(string strData_, Action<XAsyncResult> actComplete_)
        {
            SendAsync(DataEncoding.GetBytes(strData_), actComplete_);
        }

        /// <summary>
        /// 发送数据的接口：
        /// 发送成功Action中的Exception为null，否则为具体的错误信息
        /// </summary>
        /// <param name="strTo_"></param>
        /// <param name="strJson_"></param>
        /// <param name="actComplete_"></param>
        /// <param name="oTag_"></param>
        public void SendAsync(string strTo_, string strJson_, Action<XAsyncResult> actComplete_, object oTag_ = null)
        {
            SendAsync(strTo_, DataEncoding.GetBytes(strJson_), actComplete_);
        }
        #endregion

        #region "Close"
        /// <summary>
        /// 关闭实现
        /// </summary>
        protected abstract void CloseImpl();

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="actComplete_"></param>
        public void LogoutAsync(Action<XAsyncResult> actComplete_)
        {
            Thread thrLogout = new Thread(() =>
            {
                Exception exError = null;
                try
                {
                    Close();
                }
                catch (Exception ex)
                {
                    exError = ex;
                }

                if (actComplete_ != null)
                {
                    actComplete_(XAsyncResult.Get(exError));
                }
            });
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            if (!_bIsStarted || _bIsClosing) return;
            LogFile.Called("Close");
            _bIsStarted = false;

            ToClose(true);
        }

        private void ToClose(bool bClosed_)
        {
            if (!IsConnected) return;
            try
            {
                _bIsClosing = true;

                CloseImpl();
                IsConnected = false;

                InvokeDisconnected(bClosed_);
            }
            catch (Exception ex)
            {
                LogFile.Except(ex, "ToClose");
            }
            finally
            {
                _bIsClosing = false;
            }
        }

        /// <summary>
        /// 错误关闭（如果设定了重连会继续尝试重连）
        /// </summary>
        /// <param name="ex"></param>
        public void ErrorClose(Exception ex)
        {
            if (_bIsClosing) return;
            LogFile.Except(ex, "ErrorClose");

            ToClose(false);
        }
        #endregion
    } // class
}
