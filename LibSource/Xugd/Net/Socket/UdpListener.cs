using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.Net
{
    /// <summary>
    /// UDP侦听
    /// </summary>
    public class XUdpListener : XCommListener
    {
        private object _oSocketLocker = new object();
        private Socket _udpSocket = null;
        //private SocketAsyncEventArgs _argReceive = null;
        //private EndPoint _ReceiveEnd = new IPEndPoint(IPAddress.Any, 0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srvComm_"></param>
        /// <param name="listenPoint_"></param>
        public XUdpListener(XCommServer srvComm_, IPEndPoint listenPoint_)
            : base(srvComm_, listenPoint_)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void StartToListen()
        {
            LogFile.Info("UdpListener: Start {0}", ListenEndPoint);

            lock (_oSocketLocker)
            {
                _udpSocket = new Socket(ListenEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                _udpSocket.Bind(ListenEndPoint);
                StartReceive();
            }
        }

        #region "Received"
        Thread _thrReceiveData = null;

        /// <summary>
        /// 
        /// </summary>
        protected void StartReceive()
        {
            if (!IsListening) return;

            lock (_oSocketLocker)
            {
                XThread.TryStartThread(ref _thrReceiveData, ReceiveDataThread);
            }
        }

        void ReceiveDataThread()
        {
            try
            {
                while (IsListening)
                {
                    byte[] byBuffer = new byte[Server.ReceiveBufferSize];
                    var recEnd = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));
                    int nCount = _udpSocket.ReceiveFrom(byBuffer, ref recEnd);
                    if (nCount > 0)
                    {
                        if (UdpLoginout.IsLoginRequest(byBuffer, 0, nCount))
                        {
                            Send2Client((IPEndPoint)recEnd, UdpLoginout.LoginResponse, null);

                            var srvUdp = Server as XUdpServer;
                            srvUdp.AcceptConnection(ListenEndPoint, (IPEndPoint)recEnd);
                        }
                        else if (UdpLoginout.IsLogoutRequest(byBuffer, 0, nCount))
                        {
                            Server.ClientLogout(((IPEndPoint)recEnd).ToString());
                        }
                        else
                        {
                            Server.RawDataReceived(((IPEndPoint)recEnd).ToString(), byBuffer, 0, nCount);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile.Except(ex, "UdpListener.ReceiveDataThread");
                InvokeListenError(ex);
            }

            _thrReceiveData = null;
        }

        //#region "My failed Under XP"
        ///// <summary>
        ///// 
        ///// </summary>
        //protected void StartReceive()
        //{
        //    if (!IsListening) return;

        //    lock (_oSocketLocker)
        //    {
        //        if (_argReceive == null)
        //        {
        //            _argReceive = new SocketAsyncEventArgs();
        //            _argReceive.Completed += new EventHandler<SocketAsyncEventArgs>(ReceiveCompleted);
        //        }
        //        byte[] byBuffer = new byte[Server.ReceiveBufferSize];
        //        _argReceive.SetBuffer(byBuffer, 0, byBuffer.Length);
        //        _argReceive.RemoteEndPoint = _ReceiveEnd;

        //        try
        //        {
        //            if (!_udpSocket.ReceiveFromAsync(_argReceive))
        //            {
        //                ReceiveCompleted(_udpSocket, _argReceive);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            LogFile.Except(ex, "UdpListener.StartReceived");
        //            InvokeListenError(ex);
        //        }
        //    }
        //}

        //void ReceiveCompleted(object sender, SocketAsyncEventArgs e)
        //{
        //    if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
        //    {
        //        DataReceived(e);
        //        StartReceive();
        //    }
        //    else
        //    {
        //        int nError = (int)(e.SocketError == SocketError.Success ? SocketError.Shutdown : e.SocketError);
        //        InvokeListenError(new SocketException(nError));
        //    }
        //}

        //private void DataReceived(SocketAsyncEventArgs e)
        //{
        //    try
        //    {
        //        if (UdpLoginout.IsLoginRequest(e.Buffer, e.Offset, e.BytesTransferred))
        //        {
        //            Send2Client((IPEndPoint)e.RemoteEndPoint, UdpLoginout.LoginResponse, null);

        //            var srvUdp = Server as XUdpServer;
        //            srvUdp.AcceptConnection(ListenEndPoint, (IPEndPoint)e.RemoteEndPoint);
        //        }
        //        else if(UdpLoginout.IsLogoutRequest(e.Buffer, e.Offset, e.BytesTransferred))
        //        {
        //            Server.ClientLogout(((IPEndPoint)e.RemoteEndPoint).ToString());
        //        }
        //        else
        //        {
        //            Server.RawDataReceived(((IPEndPoint)e.RemoteEndPoint).ToString(), e.Buffer, e.Offset, e.BytesTransferred);
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        LogFile.Except(ex, "UdpListener.DataReceived");
        //    }
        //}
        //#endregion
        #endregion

        /// <summary>
        /// 发送数据到客户端
        /// </summary>
        /// <param name="toPoint_"></param>
        /// <param name="byData_"></param>
        /// <param name="actSend_"></param>
        public override void Send2Client(IPEndPoint toPoint_, byte[] byData_, Action<Exception> actSend_)
        {
            LogFile.Called("UdpListener.Send2Client(To:{0}, Count:{1})", toPoint_, byData_.Length);

            lock (_oSocketLocker)
            {
                if (_udpSocket == null)
                {
                    _udpSocket = new Socket(ListenEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                    _udpSocket.Bind(ListenEndPoint);
                }

                SocketAsyncEventArgs argSend = new SocketAsyncEventArgs();
                argSend.SetBuffer(byData_, 0, byData_.Length);
                argSend.UserToken = actSend_;
                argSend.Completed += new EventHandler<SocketAsyncEventArgs>(SendCompleted);
                argSend.RemoteEndPoint = toPoint_;

                try
                {
                    if (!_udpSocket.SendToAsync(argSend))
                    {
                        SendCompleted(null, argSend);
                    }
                }
                catch (Exception ex)
                {
                    LogFile.Except(ex, "UdpListener.Send2Client");
                    if (actSend_ != null)
                        actSend_(ex);

                    argSend.Dispose();
                }
            }
        }

        void SendCompleted(object sender, SocketAsyncEventArgs e)
        {
            var actSend = e.UserToken as Action<Exception>;
            if (actSend != null)
            {
                Exception ex = null;
                if (e.SocketError != SocketError.Success)
                    ex = new SocketException((int)e.SocketError);

                actSend(ex);
            }
            e.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void StopListen()
        {
            try
            {
                lock (_oSocketLocker)
                {
                    if (_udpSocket != null)
                    {
                        _udpSocket.Close();
                        _udpSocket.Dispose();
                        _udpSocket = null;
                    }
                }

                LogFile.Info("UdpListener({0}): Listen stopped", ListenEndPoint);
            }
            catch (Exception ex)
            {
                LogFile.Except(ex, "UdpListener.StopListen");
            }
        }

        private void InvokeListenError(Exception ex)
        {
            if (IsListening)
            {
                Stop();

                XThread.StartThread(() => Server.InvokeListenError(this, ex));
            }
        }
    } // class
}
