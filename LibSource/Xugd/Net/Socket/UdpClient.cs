using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using SHCre.Xugd.Common;
using System.Threading;

namespace SHCre.Xugd.Net
{
    /// <summary>
    /// UDP客户端，用于登录XUdpServer
    /// </summary>
    public partial class XUdpClient : XCommConnection
    {
        private object _oSocketLocker = new object();
        private Socket _udpSocket = null;
        //private SocketAsyncEventArgs _argReceive = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conConfig_"></param>
        public XUdpClient(XCommClientConfig conConfig_)
            : base(conConfig_)
        {
            ConnectConfig.DataCarryHeader = false;
            IsDataCarryHeader = false;
        }

        /// <summary>
        /// 由服务端获取到客户端连接时调用
        /// </summary>
        /// <param name="localEnd_"></param>
        /// <param name="remoteEnd_"></param>
        /// <param name="udpSever_"></param>
        /// <param name="srvConfig_"></param>
        public XUdpClient(IPEndPoint localEnd_, IPEndPoint remoteEnd_, XUdpServer udpSever_, XCommServerConfig srvConfig_)
            : base(udpSever_, srvConfig_)
        {
            // todo: new a UdpSocket to send/receive data
            LocalEndPoint = localEnd_;
            RemoteEndPoint = remoteEnd_;

            IsConnected = true;
        }

        #region "Connect"
        Thread _thrSimulateConnect = null;

        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="endPoint_"></param>
        /// <param name="actComplete_"></param>
        protected override void ConnectAsyncImpl(IPEndPoint endPoint_, Action<XCommConnection, Exception> actComplete_)
        {
            if (IsConnected)
            {
                actComplete_(this, null);
                return;
            }

            lock (_oSocketLocker)
            {
                if (_thrSimulateConnect != null)
                {
                    actComplete_(this, new SocketException((int)SocketError.AlreadyInProgress));
                    return;
                }

                _thrSimulateConnect = XThread.StartThread(() =>
                    {
                        SimulateConnect(endPoint_, actComplete_);
                    });
            }
        }

        private void SimulateConnect(IPEndPoint endPoint_, Action<XCommConnection, Exception> actComplete_)
        {
            Exception exError = null;
            try
            {
                lock (_oSocketLocker)
                {
                    if (_udpSocket == null)
                    {
                        _udpSocket = new Socket(endPoint_.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                        _udpSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
                    }
                    _udpSocket.SendTo(UdpLoginout.LoginRequest, endPoint_);
                }

                byte[] byReceived = new byte[10];
                EndPoint endSender = endPoint_;
                var oriTimeout = (int)_udpSocket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout);
                _udpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, ReceiveTimeoutMSec);
                int nCount = _udpSocket.ReceiveFrom(byReceived, ref endSender);
                _udpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, oriTimeout);
                if (UdpLoginout.IsLoginResponse(byReceived, 0, nCount))
                {
                    LocalEndPoint = (IPEndPoint)_udpSocket.LocalEndPoint;
                    IsConnected = true;
                }
                else
                {
                    exError = new SocketException((int)SocketError.HostUnreachable);
                }
            }
            catch (Exception ex)
            {
                LogFile.WriteLine(ex, "UdpClient.Connect {0}", endPoint_);
                exError = ex;
            }

            _thrSimulateConnect = null;
            actComplete_(this, exError);
        }
        #endregion

        #region "Received"
        Thread _thrReceiveData = null;
        /// <summary>
        /// 
        /// </summary>
        protected override void StartReceive()
        {
            // Received by the listener
            if (IsServerEnd() || !IsConnected) return;

            lock (_oSocketLocker)
            {
                XThread.TryStartThread(ref _thrReceiveData, ReceiveDataThread);
            }
        }

        void ReceiveDataThread()
        {
            try
            {
                while (IsConnected)
                {
                    byte[] byBuffer = new byte[ReceiveBufferSize];
                    var recEnd = (EndPoint)RemoteEndPoint;
                    int nCount = _udpSocket.ReceiveFrom(byBuffer, ref recEnd);
                    if (nCount > 0)
                    {
                        if (UdpLoginout.IsLogoutRequest(byBuffer, 0, nCount))
                        {
                            LogFile.Warn("Server {0} closed", recEnd);
                            ErrorClose(new SocketException((int)SocketError.HostDown));
                        }
                        else
                        {
                            UnpackedData(((IPEndPoint)recEnd).ToString(), byBuffer, 0, nCount);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DataReceiveFailed(ex);
            }

            _thrReceiveData = null;
        }

        //#region "Throw OperationAborted Under XP"
        //protected override void StartReceive()
        //{
        //    // Received by the listener
        //    if (IsServerEnd() || !IsConnected) return;
        //    lock (_oSocketLocker)
        //    {
        //        if (_argReceive == null)
        //        {
        //            _argReceive = new SocketAsyncEventArgs();
        //            _argReceive.Completed += new EventHandler<SocketAsyncEventArgs>(ReceiveCompleted);
        //        }
        //        byte[] byBuffer = new byte[ReceiveBufferSize];
        //        _argReceive.SetBuffer(byBuffer, 0, byBuffer.Length);
        //        _argReceive.RemoteEndPoint = RemoteEndPoint;

        //        try
        //        {
        //            if (!_udpSocket.ReceiveFromAsync(_argReceive))
        //            {
        //                ReceiveCompleted(_udpSocket, _argReceive);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            LogFile.Except(ex, "UdpClient.StartReceived");
        //            DataReceiveFailed(ex);
        //        }
        //    }
        //}

        //void ReceiveCompleted(object sender, SocketAsyncEventArgs e)
        //{
        //    if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
        //    {
        //        if (UdpLoginout.IsLogoutRequest(e.Buffer, e.Offset, e.BytesTransferred))
        //        {
        //            LogFile.Warn("Server {0} closed", e.RemoteEndPoint);
        //            ErrorClose(new SocketException((int)SocketError.HostDown));
        //            return;
        //        }

        //        UnpackedData(((IPEndPoint)e.RemoteEndPoint).ToString(), e.Buffer, e.Offset, e.BytesTransferred);
        //        StartReceive();
        //    }
        //    else
        //    {
        //        int nError = (int)(e.SocketError == e.SocketError ? SocketError.Shutdown : e.SocketError);
        //        DataReceiveFailed(new SocketException(nError));
        //    }
        //}
        //#endregion
        #endregion

        #region "Send"
        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="byData_"></param>
        /// <param name="actSend_"></param>
        protected override void SendAsyncImpl(byte[] byData_, Action<XCommConnection, Exception> actSend_)
        {
            if (IsServerEnd())
            {
                Server.Send2Client(LocalAddress, RemoteEndPoint, byData_, zEx =>
                    {
                        if (actSend_ != null)
                            actSend_(this, zEx);
                    });
                return;
            }

            SocketAsyncEventArgs argSend = new SocketAsyncEventArgs();
            argSend.SetBuffer(byData_, 0, byData_.Length);
            argSend.UserToken = actSend_;
            argSend.Completed += new EventHandler<SocketAsyncEventArgs>(SendCompleted);
            argSend.RemoteEndPoint = RemoteEndPoint;

            lock (_oSocketLocker)
            {
                try
                {
                    if (!_udpSocket.SendToAsync(argSend))
                    {
                        SendCompleted(null, argSend);
                    }
                }
                catch (Exception ex)
                {
                    LogFile.Except(ex, "UdpClient.SendAsyncImpl");
                    if (actSend_ != null)
                        actSend_(this, ex);

                    argSend.Dispose();
                }
            }
        }

        void SendCompleted(object sender, SocketAsyncEventArgs e)
        {
            var actSend = e.UserToken as Action<XCommConnection, Exception>;
            if (actSend != null)
            {
                Exception ex = null;
                if (e.SocketError != SocketError.Success)
                    ex = new SocketException((int)e.SocketError);

                actSend(this, ex);
            }
            e.Dispose();
        }
        #endregion

        private void TryDispose()
        {
            //if (_argReceive != null)
            //{
            //    _argReceive.Dispose();
            //    _argReceive = null;
            //}

            // Socket
            if (_udpSocket != null)
            {
                _udpSocket.Dispose();
                _udpSocket = null;
            }
        }

        /// <summary>
        /// 实现关闭
        /// </summary>
        protected override void CloseImpl()
        {
            try
            {
                lock (_oSocketLocker)
                {
                    if (IsConnected && _udpSocket != null)
                    {
                        //if (!IsServerEnd())
                        { // Server end: Send by server before close
                            ManualResetEvent eWait = new ManualResetEvent(false);
                            SendAsyncImpl(UdpLoginout.LogoutRequest, (zop, zex) => eWait.Set());
                            eWait.WaitOne(SendSynTimeoutMSec);
                        }

                        _udpSocket.Close();
                    }

                    TryDispose();
                }
            }
            catch (Exception ex)
            {
                LogFile.Except(ex, "UdpClient.CloseImpl");
            }
        }
    } // class
}
