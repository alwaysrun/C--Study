using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.Net
{
    partial class XCommConnection
    {
        /// <summary>
        /// 数据发送超时时间，毫秒
        /// </summary>
        protected int SendSynTimeoutMSec;
        /// <summary>
        /// 数据接收超时时间
        /// </summary>
        protected int ReceiveTimeoutMSec;

        private int _nIdleAliveInter;
        private int _nCheckAliveInter;
        private int _nReconnectInter;
        private bool _bAutoConnectWhenSend;

        private DateTime _dtLastCommunicate = DateTime.Now;
        /// <summary>
        /// 最近一次数据收发时间
        /// </summary>
        protected DateTime LastCommunicateTime
        {
            get { return _dtLastCommunicate; }
            set
            {
                if (_dtLastCommunicate == value) return;

                _dtLastCommunicate = value;
                SetCheckIdleTimeout();
            }
        }

        private void BuildAllTimer()
        {
            if (_nIdleAliveInter > 0)
                _timerCheckIdleTime = new Timer(TimerCheckIdleTime);
            if (_nReconnectInter > 0)
                _timerReconnect = new Timer(TimerReconnect);
            if (_nCheckAliveInter > 0)
                _timerCheckAlive = new Timer(TimerCheckAlive);

            _timerReceivedData = new Timer(TimerReceivedData);
        }

        #region "Idle Time"
        private Timer _timerCheckIdleTime;
        private void SetCheckIdleTimeout()
        {
            if (!IsConnected || _nIdleAliveInter <= 0) return;

            _timerCheckIdleTime.Change(_nIdleAliveInter, Timeout.Infinite);
        }

        private void DisableCheckIdle()
        {
            if(_nIdleAliveInter > 0)
            {
                _timerCheckIdleTime.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private void TimerCheckIdleTime(object state)
        {
            ErrorClose(new TimeoutException(string.Format("Connect {0} Idle Time out: {1}MSec", RemoteAddress, _nIdleAliveInter)));
        }
        #endregion

        #region "Check Alive"
        private Timer _timerCheckAlive;

        private void SetCheckAliveTimer()
        {
            if (_timerCheckAlive == null || !IsConnected)
                return;

            _timerCheckAlive.Change(_nCheckAliveInter, _nCheckAliveInter);
        }

        private void TimerCheckAlive(object state)
        {
            if (_bIsStarted)
            {
                InvokeCheckAlive();
            }
        }
        #endregion

        #region "Reconnect"
        private Timer _timerReconnect;

        private void TimerReconnect(object state_)
        {
            if (IsConnected || !_bIsStarted) return;

            LogFile.Print("Reconnect");
            ConnectAsyncImpl(RemoteEndPoint, (zcon, zex) =>
            {
                if (zex == null)
                {
                    IsConnected = true;
                }
                else
                {
                    SetReconnectTimer();
                }
            });
        }

        private void SetReconnectTimer()
        {
            if (_timerReconnect == null || !_bIsStarted) return;

            if (_nReconnectInter > 0)
                _timerReconnect.Change(_nReconnectInter, Timeout.Infinite);
            else
                _timerReconnect.Change(Timeout.Infinite, Timeout.Infinite);
        }
        #endregion

        #region "ReceivedData"
        private Timer _timerReceivedData;
        private void SetReceivedDataTimer(bool bEnabled_)
        {
            if (bEnabled_)
                _timerReceivedData.Change(ReceiveTimeoutMSec, Timeout.Infinite);
            else
                _timerReceivedData.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void TimerReceivedData(object state)
        {
            lock (_locReceivedList)
            {
                if(_lstReceivedList.Count>0)
                {
                    LogFile.Error("TimerReceivedData(Receive:{0}, Discard:{1}, WaitLen:{2})",
                        XTime.GetTimeString(_lstReceivedList[0].Time),
                        XTime.GetTimeString(DateTime.Now),
                        _nWaitingLen);
                }
                _lstReceivedList.Clear();
                _nTailLen = 0;
                _nWaitingLen = 0;
            }
        }
        #endregion
    } // class
}
