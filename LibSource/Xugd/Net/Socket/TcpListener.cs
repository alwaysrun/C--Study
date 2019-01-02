using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace SHCre.Xugd.Net
{
    /// <summary>
    /// Tcp侦听类
    /// </summary>
    public class XTcpListener : XCommListener
    {
        private Socket _tcpSocket;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srvComm_"></param>
        /// <param name="listenPoint_"></param>
        public XTcpListener(XCommServer srvComm_, IPEndPoint listenPoint_)
            : base(srvComm_, listenPoint_)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void StartToListen()
        {
            LogFile.Info("TcpListener: start {0}", ListenEndPoint);

            _tcpSocket = new Socket(ListenEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _tcpSocket.Bind(ListenEndPoint);
            _tcpSocket.Listen(-1);

            StartAccept(null);
        }

        private void StartAccept(SocketAsyncEventArgs argAccept_)
        {
            if(!IsListening) 
            {
                if (argAccept_ != null)
                    argAccept_.Dispose();
                argAccept_ = null;
                return;
            }

            // To accept
            if (argAccept_ == null)
            {
                argAccept_ = new SocketAsyncEventArgs();
                argAccept_.Completed += OnAcceptConnection;
            }
            else
            {
                // Socket must be cleared since it be reused.
                argAccept_.AcceptSocket = null;
            }

            try
            {
                if (!_tcpSocket.AcceptAsync(argAccept_))
                { // Complete synch
                    OnAcceptConnection(null, argAccept_);
                }
            }
            catch (Exception ex)
            {
                LogFile.Except(ex, "TcpListener.StartAccept");
                ProcAcceptExcption(argAccept_, ex);
            }
        }

        private void ProcAcceptExcption(SocketAsyncEventArgs arg, Exception ex_)
        {
            arg.Dispose();
            Stop();

            Server.InvokeListenError(this, ex_);
        }

        private void ProcAcceptError(SocketAsyncEventArgs arg)
        {
            LogFile.Error("TcpListner({0}): Accept socket failed {1}", ListenEndPoint, arg.SocketError);

            ProcAcceptExcption(arg, new SocketException((int)arg.SocketError));
        }

        private void OnAcceptConnection(object sender, SocketAsyncEventArgs arg)
        {
            if (!IsListening)
            {
                arg.Dispose();
                return;
            }

            try
            {
                if (arg.SocketError == SocketError.Success)
                {
                    LogFile.Info("TcpListener: accept connection {0}", (IPEndPoint)arg.AcceptSocket.RemoteEndPoint);

                    var srvTcp = Server as XTcpServer;
                    srvTcp.AcceptConnection(arg.AcceptSocket, this);

                    StartAccept(arg);
                }
                else
                {
                    ProcAcceptError(arg);
                }
            }
            catch(Exception ex)
            {
                ProcAcceptExcption(arg, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void StopListen()
        {
            if (_tcpSocket == null)
                return;

            try
            {
                //_tcpSocket.Shutdown(SocketShutdown.Both);
                _tcpSocket.Close();
                _tcpSocket.Dispose();

                LogFile.Info("TcpListener({0}): Listen stopped", ListenEndPoint);
            }
            catch(Exception ex)
            {
                LogFile.Except(ex, "TpcListner.StopListen");
            }

            _tcpSocket = null;
        }
    }
}
