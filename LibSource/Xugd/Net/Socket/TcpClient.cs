using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using SHCre.Xugd.Common;
using System.Net;

namespace SHCre.Xugd.Net
{
    /// <summary>
    /// TCP链接类
    /// </summary>
    public class XTcpClient : XCommConnection
    {
        #region "Var"
        private object _oSocketLocker = new object();
        private Socket _tcpSocket = null;
        private SocketAsyncEventArgs _argReceive = null;

        private bool _bIsConnecting = false;
        private bool _bIsDisconnecting = false;
        #endregion

        #region "Construct"
        /// <summary>
        /// 构造函数：客户端构造连接
        /// </summary>
        /// <param name="conConfig_"></param>
        public XTcpClient(XCommClientConfig conConfig_)
            : base(conConfig_)
        {
        }

        /// <summary>
        /// 构造函数：服务端构造连接
        /// </summary>
        /// <param name="socket_"></param>
        /// <param name="tcpServer_"></param>
        /// <param name="srvConfig_"></param>
        protected XTcpClient(Socket socket_, XTcpServer tcpServer_, XCommServerConfig srvConfig_)
            : base(tcpServer_, srvConfig_)
        {
            _tcpSocket = socket_;
            RemoteEndPoint = (IPEndPoint)socket_.RemoteEndPoint;
            LocalEndPoint = (IPEndPoint)socket_.LocalEndPoint;

            IsConnected = true;
        }

        internal static XTcpClient CreateConnection(Socket socket_, XTcpServer tcpServer_, XCommServerConfig listenConfig_)
        {
            return new XTcpClient(socket_, tcpServer_, listenConfig_);
        }
        #endregion

        #region "Connect"
        /// <summary>
        /// 实现连接
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
                if (_bIsConnecting || _bIsDisconnecting)
                {
                    actComplete_(this, new SocketException((int)SocketError.AlreadyInProgress));
                    return;
                }
                _bIsConnecting = true;

                if (_tcpSocket == null)
                    _tcpSocket = new Socket(endPoint_.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                var argConnect = new SocketAsyncEventArgs();
                argConnect.RemoteEndPoint = endPoint_;
                argConnect.Completed += (sender, e) =>
                    {
                        ConnectImplCompleted(e, actComplete_);
                    };

                // Connect now
                try
                {
                    if (!_tcpSocket.ConnectAsync(argConnect))
                    {
                        ConnectImplCompleted(argConnect, actComplete_);
                    }
                }
                catch (Exception ex)
                {
                    LogFile.Except(ex, "TcpClient.ConnectAsyncImpl");
                    if (actComplete_ != null)
                        actComplete_(this, ex);
                }
            }
        }

        private void ConnectImplCompleted(SocketAsyncEventArgs argCon_, Action<XCommConnection, Exception> actComplete_)
        {
            _bIsConnecting = false;

            if (actComplete_ != null)
            {
                Exception exConnect = null;
                if (argCon_.SocketError != SocketError.Success && argCon_.SocketError != SocketError.IsConnected)
                {
                    exConnect = new SocketException((int)argCon_.SocketError);
                }
                else
                {
                    RemoteEndPoint = (IPEndPoint)_tcpSocket.RemoteEndPoint;
                    LocalEndPoint = (IPEndPoint)_tcpSocket.LocalEndPoint;
                }

                actComplete_(this, exConnect);
            }
        }
        #endregion

        #region "Receive"
        /// <summary>
        /// 开始接收
        /// </summary>
        protected override void StartReceive()
        {
            if (!IsConnected || _bIsDisconnecting) return;

            lock (_oSocketLocker)
            {
                if (_argReceive == null)
                {
                    _argReceive = new SocketAsyncEventArgs();
                    _argReceive.Completed += ReceiveCompleted;
                }
                byte[] byBuffer = new byte[ReceiveBufferSize];
                _argReceive.SetBuffer(byBuffer, 0, byBuffer.Length);

                try
                {
                    if (!_tcpSocket.ReceiveAsync(_argReceive))
                    { // Complete syn
                        ReceiveCompleted(_tcpSocket, _argReceive);
                    }
                }
                catch (Exception ex)
                {
                    LogFile.Except(ex, "TcpClient.StartReceive");
                    DataReceiveFailed(ex);
                }
            }
        }

        void ReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0)
            {
                if (e.SocketError == SocketError.Success)
                {
                    SelfRawDataReceived(e.Buffer, e.Offset, e.BytesTransferred);
                    StartReceive();
                }
                else
                {
                    DataReceiveFailed(new SocketException((int)e.SocketError));
                }
            }
            else
            { // Socke has close by remote
                DataReceiveFailed(new SocketException((int)SocketError.Shutdown));
            }
        }
        #endregion

        #region "send"
        private class SendBufferInfo
        {
            public Action<XCommConnection, Exception> ActComplete { get; set; }

            public int Count { get; set; }
            public int Offset { get; set; }
            public byte[] Data { get; set; }

            public SendBufferInfo(byte[] bySend_, int nOffset_, int nCount_)
            {
                Data = bySend_;
                Offset = nOffset_;
                Count = nCount_;
            }
        }

        /// <summary>
        /// 实现发送
        /// </summary>
        /// <param name="byData_"></param>
        /// <param name="actSend_"></param>
        protected override void SendAsyncImpl(byte[] byData_, Action<XCommConnection, Exception> actSend_)
        {
            SocketAsyncEventArgs argSend = new SocketAsyncEventArgs();
            argSend.SetBuffer(byData_, 0, byData_.Length);
            SendBufferInfo buffInfo = new SendBufferInfo(byData_, 0, byData_.Length);
            buffInfo.ActComplete = actSend_;
            argSend.UserToken = buffInfo;
            argSend.Completed += SendCompleted;
            SendAsyncImpl(argSend);
        }

        private void SendAsyncImpl(SocketAsyncEventArgs argSend_)
        {
            SendBufferInfo buffInfo = argSend_.UserToken as SendBufferInfo;
            if (buffInfo.Offset >= buffInfo.Count)
            {
                OnSendCompleted(buffInfo, SocketError.Success);
                argSend_.Dispose();
                return;
            }

            if (!IsConnected || _bIsDisconnecting)
            {
                OnSendCompleted(buffInfo, SocketError.NotConnected);
                argSend_.Dispose();
                return;
            }

            lock (_oSocketLocker)
            {
                try
                {
                    if (!_tcpSocket.SendAsync(argSend_))
                    { // Complete sync
                        SendCompleted(null, argSend_);
                    }
                }
                catch (Exception ex)
                {
                    LogFile.Except(ex, "TcpClient.SendAsyncImpl");
                    if (buffInfo.ActComplete != null)
                        buffInfo.ActComplete(this, ex);

                    argSend_.Dispose();
                }
            }
        }

        private void SendCompleted(object sender, SocketAsyncEventArgs e)
        {
            SendBufferInfo sendBuff = e.UserToken as SendBufferInfo;
            sendBuff.Offset += e.BytesTransferred;
            if (e.SocketError == SocketError.Success && sendBuff.Offset < sendBuff.Count)
            {
                e.SetBuffer(sendBuff.Offset, sendBuff.Count - sendBuff.Offset);
                SendAsyncImpl(e);
            }
            else
            {
                OnSendCompleted(sendBuff, e.SocketError);
                e.Dispose();
            }
        }

        private void OnSendCompleted(SendBufferInfo buffInfo_, SocketError socketError)
        {
            if (buffInfo_.ActComplete != null)
            {
                Exception ex= null;
                if (socketError!= SocketError.Success)
                    ex = new SocketException((int)socketError);

                buffInfo_.ActComplete(this, ex);
            }
        }
        #endregion

        #region "Close"
        private void TryDispose()
        {
            if (_argReceive != null)
            {
                _argReceive.Dispose();
                _argReceive = null;
            }

            // Socket
            if (_tcpSocket != null)
            {
                _tcpSocket.Dispose();
                _tcpSocket = null;
            }
        }

        /// <summary>
        /// 实现关闭
        /// </summary>
        protected override void CloseImpl()
        {
            if (_bIsDisconnecting) return;

            try
            {
                lock (_oSocketLocker)
                {
                    _bIsDisconnecting = true;

                    if (IsConnected)
                    {
                        _tcpSocket.Shutdown(SocketShutdown.Both);
                        _tcpSocket.Close();
                    }

                    TryDispose();
                }
            }
            catch (Exception ex)
            {
                LogFile.Except(ex, "TcpClient.CloseImpl");
            }

            _bIsDisconnecting = false;
        }
        #endregion
    }
}
