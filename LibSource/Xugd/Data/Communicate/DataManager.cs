using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SHCre.Xugd.Data;
using SHCre.Xugd.Common;
using SHCre.Xugd.Extension;
using SHCre.Xugd.CFile;

namespace SHCre.Xugd.Data
{
    partial class XDataComm<TEnum>
    {
        class SendDataInfo
        {
            public Action<XAsyncResult> ActReponse { get; set; }
            public Action<SendCompleteArgs> ActComplete { get; set; }
            public void InvokeComplete(XJsonDataFormat.DataWithType<TEnum> responseData_, Exception ex_)
            {
                if (IsResponse)
                {
                    if (ActReponse != null)
                        ActReponse(XAsyncResult.Get(ex_));
                }
                else
                {
                    if (ActComplete != null)
                    {
                        ActComplete(new SendCompleteArgs(ex_)
                            {
                                ResponseData = responseData_,
                            });
                    }
                }
            }
            public int DataIndex { get; set; }
            public int Version {get;set;}
            public bool IsResponse { get; set; }
            public bool NeedResponse { get; set; }
            public string SendTo { get; set; }
            public TEnum RequestType { get; set; }
            public object RequestData { get; set; }
            public object SendTag {get;set;}

            public DateTime AddTime { get; set; }
            public DateTime? SendTime { get; set; }
            //public DateTime? ReceiveTime { get; set; }
        }

        #region "Send"
        XQueueIssue<SendDataInfo> _quToSend = null;

        private void ClearSendData()
        {
            _quToSend.Clear((quAll) =>
                {
                    try
                    {
                        foreach (var quData in quAll)
                        {
                            quData.InvokeComplete(null, new XDataException(string.Format("DataComm stopped: AddTime {0}, has not send, Index:{1}",
                                        XTime.GetTimeString(quData.AddTime), quData.DataIndex)));
                        }
                    }
                    catch (Exception) { }
                });
            _quToSend.Stop();
        }

        private void AddResponseData(RequestArrivalArgs reqData_, object responseData_, Action<XAsyncResult> actComplete_, object oTag_)
        {
            InvokeOnDebug("AddResponseData(To:{0}, Index:{1}, Type:{2})", 
                reqData_.From, reqData_.RequestData.DataIndex, reqData_.RequestData.DataType);

            var sendData = new SendDataInfo()
            {
                SendTo = reqData_.From,
                DataIndex = reqData_.RequestData.DataIndex,
                Version = reqData_.RequestData.GetVerion(),
                RequestType = reqData_.RequestData.DataType,
                RequestData = responseData_,
                SendTag = oTag_,
                ActComplete = actComplete_,
                IsResponse = true,
                NeedResponse = false,
                AddTime = DateTime.Now,
            };
            _quToSend.AddItem(sendData);
        }

        private void AddSendData(string strTo_, int nDataIndex_, int nVersion_, TEnum euRequestType_, object objRequestData_, Action<SendCompleteArgs> actComplete_, bool bIsResponse_, bool bNeedResponse_, object oTag_)
        {
            InvokeOnDebug("AddSendData(To:{0}, Index:{1}, Type:{2})", strTo_, nDataIndex_, euRequestType_);

            var sendData = new SendDataInfo()
                        {
                            SendTo = strTo_,
                            DataIndex = nDataIndex_,
                            Version = nVersion_,
                            RequestType = euRequestType_,
                            RequestData = objRequestData_,
                            SendTag = oTag_,
                            ActComplete = actComplete_,
                            IsResponse = bIsResponse_,
                            NeedResponse = bNeedResponse_,
                            AddTime = DateTime.Now,
                        };
            _quToSend.AddItem(sendData);
        }

        private bool SendJsonData(SendDataInfo dataSend_)
        {
            try
            {
                string strJson = XJsonDataFormat.Data2JsonWithType(dataSend_.RequestType, dataSend_.RequestData, dataSend_.DataIndex, dataSend_.Version, dataSend_.IsResponse, dataSend_.NeedResponse);
                // Add before send to avoid: before add the response has arrived
                if (dataSend_.NeedResponse)
                {
                    AddWaitData(dataSend_);
                }

                NetComm.SendAsync(dataSend_.SendTo, strJson, (exSend) =>
                {
                    if (exSend.IsSuccess && dataSend_.NeedResponse)
                    { // Success, wait response
                    }
                    else
                    {
                        if (dataSend_.NeedResponse)
                        {
                            RemoveWaitData(dataSend_);
                        }
                        dataSend_.InvokeComplete(null, exSend.Result);
                    }
                },
                dataSend_.SendTag);
            }
            catch (Exception ex)
            {
                dataSend_.InvokeComplete(null, ex);
            }

            return true;
        }

        #endregion

        #region "Wait reponse"
        Thread _thrWaitData = null;
        private readonly object _lockWaitDic = new object();
        Dictionary<string, List<SendDataInfo>> _dicWaitData = new Dictionary<string, List<SendDataInfo>>();

        void ClearWaitData()
        {
            lock (_lockWaitDic)
            {
                foreach(var dicData in _dicWaitData){
                    foreach(var data in dicData.Value){
                        data.InvokeComplete(null, new XDataException(string.Format("DataComm stopped: AddTime {0}, SendTime {1}, Index:{2}",
                                XTime.GetTimeString(data.AddTime), XTime.GetTimeString(data.SendTime.Value), data.DataIndex)));
                    }
                }
                
                _dicWaitData.Clear();
            }
        }

        private SendDataInfo GetCorrespondRequest(string strFrom_, XJsonDataFormat.DataWithType<TEnum> receivData_)
        {
            lock (_lockWaitDic)
            {
                if (_dicWaitData.ContainsKey(strFrom_))
                {
                    var lstDatas = _dicWaitData[strFrom_];
                    var sendInfo = lstDatas.FirstOrDefault(z => z.DataIndex == receivData_.DataIndex);
                    if (sendInfo != null)
                    {
                        InvokeOnDebug("Receive-Response(From:{0}, Type:{1}, Index:{2})", strFrom_, receivData_.DataType, receivData_.DataIndex);

                        lstDatas.Remove(sendInfo);
                        return sendInfo;
                    }
                }
            }

            return null;
        }

        private void HandleResponseData(XReceiveDataType euType_, string strFrom_, XJsonDataFormat.DataWithType<TEnum> receivData_)
        {
            InvokeOnLogger(XLogSimple.LogLevels.Info, "HandleResponseData(From:{0}, Type:{1})",
                strFrom_, euType_);

            var corrRequest = GetCorrespondRequest(strFrom_, receivData_);
            if (corrRequest != null)
            {
                if (euType_ == XReceiveDataType.SendBack)
                {
                    corrRequest.InvokeComplete(null, new XNotLoginException("Message SendBack"));
                }
                else
                {
                    corrRequest.InvokeComplete(receivData_, null);
                }
            }
            else
            {
                if(euType_ != XReceiveDataType.SendBack)    // IsResponse
                    InvokeResponseMismatch(strFrom_, receivData_);
            }
        }

        private void AddWaitData(SendDataInfo sentData_)
        {
            sentData_.SendTime = DateTime.Now;

            lock (_lockWaitDic)
            {
                List<SendDataInfo> lstDatas = null;
                if (!_dicWaitData.TryGetValue(sentData_.SendTo, out lstDatas))
                {
                    lstDatas = new List<SendDataInfo>();
                    _dicWaitData[sentData_.SendTo] = lstDatas;
                }

                lstDatas.Add(sentData_);
            }

            XThread.TryStartThread(ref _thrWaitData, WaitDataThread);
        }

        private void RemoveWaitData(SendDataInfo sentData_)
        {
            lock (_lockWaitDic)
            {
                if (_dicWaitData.ContainsKey(sentData_.SendTo))
                {
                    var lstDatas = _dicWaitData[sentData_.SendTo];
                    var sendInfo = lstDatas.FirstOrDefault(z => z.DataIndex == sentData_.DataIndex);
                    if (sendInfo != null)
                    {
                        lstDatas.Remove(sendInfo);
                    }
                }
            }
        }

        private void WaitDataThread()
        {
            int nInterval = (int)_nResponseWaitSeconds / 10 + 1;
            nInterval = XTime.Second2Interval(nInterval);

            while (_bStarted)
            {
                Thread.Sleep(nInterval);
                if (!_bStarted)
                    break;

                try
                {
                    List<SendDataInfo> lstDatas = new List<SendDataInfo>();
                    lock (_lockWaitDic)
                    {
                        DateTime dtAnchorTime = DateTime.Now.AddSeconds(-_nResponseWaitSeconds);
                        foreach (var waitPair in _dicWaitData)
                        {
                            int nCount = 0;
                            var waitDatas = waitPair.Value;
                            foreach (var data in waitDatas)
                            {
                                // if ((DateTime.Now - data.SendTime.Value).TotalSeconds < _nResponseWaitSeconds)
                                if(data.SendTime.Value > dtAnchorTime)
                                {
                                    break;
                                }

                                ++nCount;
                            }

                            if (nCount > 0) // Has overtime data
                            {
                                lstDatas.AddRange(waitDatas.GetRange(0, nCount));
                                waitDatas.RemoveRange(0, nCount);
                            }
                        }
                    }

                    // Complete timeout datas
                    foreach (var data in lstDatas)
                    {
                        InvokeOnLogger(XLogSimple.LogLevels.Warn, "Request-waitResponse-Timeout(Index:{0}, SendTime:{1})", data.DataIndex, data.SendTime.Value);

                        data.InvokeComplete(null, new TimeoutException(string.Format("Timeout: AddTime {0}, SendTime {1}, OverTime {2}, Index:{3}", 
                                XTime.GetTimeString(data.AddTime), XTime.GetTimeString(data.SendTime.Value), XTime.GetTimeString(DateTime.Now), data.DataIndex)));
                    }
                }
                catch(Exception ex)
                {
                    InvokeOnExcept(ex, "WaitData");
                }
            }

            _thrWaitData = null;
        }
        #endregion

    } // XNetCommunicate<TEnum>
}
