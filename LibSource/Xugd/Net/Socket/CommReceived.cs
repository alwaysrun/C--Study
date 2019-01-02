using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Extension;
using System.Threading;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.Net
{
    partial class XCommConnection
    {
        int _nTailLen = 0;
        byte[] _byDataTail = new byte[HeaderLength];

        int _nWaitingLen = 0;
        DateTime _dtWaitingTime = DateTime.Now;
        readonly object _locReceivedQueue = new object();
        readonly object _locReceivedList = new object();
        Thread _thrReceivedQueue = null;
        AutoResetEvent _evtReceivedQueue = new AutoResetEvent(false);
        Queue<ReceiveDataBuffer> _quReceivedQueue = new Queue<ReceiveDataBuffer>();
        List<ReceiveDataBuffer> _lstReceivedList = new List<ReceiveDataBuffer>();

        #region "queue"
        void DataWithHeaderReceived(byte[] byData_, int nOffset_, int nCount_)
        {
            lock (_locReceivedQueue)
            {
                _quReceivedQueue.Enqueue(new ReceiveDataBuffer(byData_, nOffset_, nCount_));
            }

            _evtReceivedQueue.Set();
            XThread.TryStartThread(ref _thrReceivedQueue, ReceivedQueueThread);
        }

        ReceiveDataBuffer GetQueueData()
        {
            lock(_locReceivedQueue)
            {
                if (_quReceivedQueue.Count == 0)
                    return null;

                return _quReceivedQueue.Dequeue();
            }
        }

        void ReceivedQueueThread()
        {
            try 
            {
                while(IsConnected)
                {
                    _evtReceivedQueue.WaitOne(XTime.Second2Interval(60));

                    while(true)
                    {
                        var revData = GetQueueData();
                        if (revData == null)
                            break;

                        ProcDataWithHeader(revData);
                    }
                }
            }
            catch(Exception ex)
            {
                LogFile.Except(ex, "ReceivedQueueThread");
            }

            _thrReceivedQueue = null;
        }
        #endregion

        void Add2ReceivedList(ReceiveDataBuffer revBuffer_)
        {
            _lstReceivedList.Add(revBuffer_);

            SetReceivedDataTimer(true);
        }

        private void ProcDataWithHeader(ReceiveDataBuffer revBuffer_)
        {
            try
            {
                SetReceivedDataTimer(false);

                // Has data-header
                lock (_locReceivedList)
                {
                    if (_nWaitingLen == 0)
                    {
                        CheckDataHeader(revBuffer_);
                    }
                    else
                    {
                        if (revBuffer_.Count >= _nWaitingLen)
                        {
                            BuildRawData(revBuffer_);
                        }
                        else
                        {
                            _nWaitingLen -= revBuffer_.Count;
                            Add2ReceivedList(revBuffer_);
                        }
                    }
                } // lock
            }
            catch(Exception ex)
            {
                LogFile.Except(ex, "CheckRawData");
            }
        }

        void CheckDataHeader(ReceiveDataBuffer revBuffer_)
        {
            if(_nTailLen != 0)
            {
                byte[] byData = new byte[_nTailLen + revBuffer_.Count];
                Array.Copy(_byDataTail, byData, _nTailLen);
                Array.Copy(revBuffer_.Data, revBuffer_.Offset, byData, _nTailLen, revBuffer_.Count);

                revBuffer_ = new ReceiveDataBuffer(byData);
                _nTailLen = 0;
            }

            if(!IsHeaderFound(revBuffer_))
                ToFindHeader(revBuffer_);
        }

        private bool IsHeaderFound(ReceiveDataBuffer revBuffer_)
        {
            if (IsDataHeader(revBuffer_.Data, revBuffer_.Offset, revBuffer_.Count))
            {
                _nWaitingLen = GetDataLen(revBuffer_.Data, revBuffer_.Offset);
                revBuffer_.RemoveData(HeaderLength);
                if (revBuffer_.Count >= _nWaitingLen)
                {
                    BuildRawData(revBuffer_);
                }
                else
                {
                    _nWaitingLen -= revBuffer_.Count;
                    Add2ReceivedList(revBuffer_);
                }

                return true;
            }

            return false;
        }

        private void ToFindHeader(ReceiveDataBuffer revBuffer_)
        {
            while (true)
            {
                var indexFind = Array.IndexOf(revBuffer_.Data, HeaderFirst, revBuffer_.Offset, revBuffer_.Count);
                if (indexFind == -1)
                {
                    LogFile.Error("ToFindHeader(Offset:{0}, Count:{1}): No header, discard", revBuffer_.Offset, revBuffer_.Count);
                    return; // no header, discard data.
                }

                revBuffer_.RemoveData(indexFind - revBuffer_.Offset);
                if(revBuffer_.Count<HeaderLength)
                { // Not enough data, reserver to wait the next data
                    Array.Copy(revBuffer_.Data, revBuffer_.Offset, _byDataTail, 0, revBuffer_.Count);
                    _nTailLen = revBuffer_.Count;
                    return;
                }

                if (IsHeaderFound(revBuffer_))
                    return;

                // Find next
                revBuffer_.RemoveData(1);
            }
        }

        void BuildRawData(ReceiveDataBuffer revBuffer_)
        {            
            int nLen = _lstReceivedList.Sum(z => z.Count) + _nWaitingLen;
            byte[] byData = new byte[nLen];
            int nStart = 0;
            foreach(var revData in _lstReceivedList)
            {
                Array.Copy(revData.Data, revData.Offset, byData, nStart, revData.Count);
                nStart += revData.Count;
            }
            Array.Copy(revBuffer_.Data, revBuffer_.Offset, byData, nStart, _nWaitingLen);
            // todo: use queue
            XThread.StartPool((object none_) => UnpackedData(this.NetUser, byData, 0, byData.Length));

            // Clear
            revBuffer_.RemoveData(_nWaitingLen);
            _lstReceivedList.Clear();
            _nWaitingLen = 0;

            if (revBuffer_.Count > 0)
                CheckDataHeader(revBuffer_);
        }

        //////////////////////////////////////////////////////////////////////////
        class ReceiveDataBuffer
        {
            public DateTime Time {get; private set;}
            public byte[] Data { get; private set; }
            public int Offset { get; private set; }
            public int Count { get; private set; }

            public ReceiveDataBuffer(byte[] byData_)
                : this(byData_, 0, byData_.Length)
            {
            }

            public ReceiveDataBuffer(byte[] byData_, int nOffset_, int nCount_)
            {
                Time = DateTime.Now;
                Data = byData_;
                Offset = nOffset_;
                Count = nCount_;
            }

            public void RemoveData(int nCount_)
            {
                if (Count <= nCount_)
                {
                    Count = 0;
                    return;
                }

                Offset += nCount_;
                Count -= nCount_;
            }
        }
    }
}
