using System;
using SHCre.Xugd.Data;
using SHCre.Xugd.Common;
using SHCre.Xugd.Extension;
using SHCre.Xugd.CFile;

namespace SHCre.Xugd.Data
{
    /// <summary>
    /// 用于数据发送与接收的类：
    /// 通过NetComm进行发送与接收数据。
    /// 发送的数据如果需要应答，则等待应答到达后才会调用完成Action；否则，发送完成后即调用完成Action。
    /// 通过ResponseWaitSeconds来设定等待应答的时间，超过此时间后会作为TimeoutException来处理。
    /// </summary>
    /// <typeparam name="TEnum">定义命令的枚举类型</typeparam>
    public partial class XDataComm<TEnum> : XLogEventsBase
    {
        #region "Var"
        private bool _bStarted = false;
        private XSafeSequence _nSequence = new XSafeSequence();

        /// <summary>
        /// 真正发送与接收数据的通讯库
        /// </summary>
        public IXConnection NetComm { get; private set; }
        
        private double _nResponseWaitSeconds = 45;
        /// <summary>
        /// 设定等待应答的时间，超过此时间后会作为TimeoutException来处理
        /// </summary>
        public int ResponseWaitSeconds
        {
            get { return (int)_nResponseWaitSeconds; }
            set
            {
                if (value <= 0)
                    value = 45;
                _nResponseWaitSeconds = value;
            }
        }
        #endregion

        #region "Event"
       /// <summary>
        /// 有请求到达时，激发的事件
        /// </summary>
        public event Action<RequestArrivalArgs> OnRequestArrival;
        void InvokeRequestArrival(string strFrom_, XJsonDataFormat.DataWithType<TEnum> requestData_)
        {
            if (OnRequestArrival != null)
            {
                var requestArgs = new RequestArrivalArgs()
                {
                    From = strFrom_,
                    RequestData = requestData_,
                };
                if (requestData_.NeedResponse)
                {
                    requestArgs.ActResponse = AddResponseData;
                }

                OnRequestArrival(requestArgs);
            }
        }

        /// <summary>
        /// 处理接收数据失败时，激发的事件
        /// </summary>
        public event Action<ReceiveDataHandleFailedArgs> OnDecodeDataFail;
        void InvokeDecodeDataFail(string strFrom_, string strJson_, Exception ex_)
        {
            if (OnDecodeDataFail != null)
                OnDecodeDataFail(new ReceiveDataHandleFailedArgs()
                    {
                        From = strFrom_,
                        Data = strJson_,
                        Error = ex_,
                    });
        }

        /// <summary>
        /// 应答匹配失败时激发
        /// </summary>
        public event Action<ResponseMismatchArgs> OnResponseMismatch;
        void InvokeResponseMismatch(string strFrom_, XJsonDataFormat.DataWithType<TEnum> receivData_)
        {
            if(OnResponseMismatch != null)
            {
                OnResponseMismatch(new ResponseMismatchArgs()
                    {
                        From = strFrom_,
                        DataIndex = receivData_.DataIndex,
                        DataType = receivData_.DataType,
                        ResponseData = receivData_.DataJson,
                    });
            }

            var ex = new XResponseMismatchException(string.Format("Index:{0}, Type:{1}, {2})", receivData_.DataIndex, receivData_.DataType, receivData_.IsResponse ? "Response" : "Request"))
            {
                From = strFrom_,
            };
            InvokeOnExcept(ex);
        }
       #endregion

        /// <summary>
        /// 构造函数，netComm的事件OnExcept与OnLogger在XDataComm中不做任何处理，如需事件信息需在外部获取
        /// </summary>
        /// <param name="netComm_"></param>
        public XDataComm(IXConnection netComm_)
        {
            NetComm = netComm_;
            NetComm.OnDataReceived += new Action<XReceiveDataArgs>(NetComm_OnDataReceived);

            _quToSend = new XQueueIssue<SendDataInfo>(SendJsonData);
            _quToSend.OnExcept += _quToSend_OnExcept;
            _quToSend.OnLogger += new Action<string, XLogSimple.LogLevels>(_quToSend_OnLogger);
            _quToSend.LogPrefix = "SendQueue.";
            BuildLogPrefix<TEnum>("XDataComm");
            //LogPrefix = string.Format("DataComm({0})", NetComm.RemoteAddress);

            Start();
        }

        void _quToSend_OnLogger(string strInfo, XLogSimple.LogLevels euLevel_)
        {
            InvokeOnLogger(strInfo, euLevel_);
        }

        void _quToSend_OnExcept(Exception obj, string strInfo_)
        {
            InvokeOnExcept(obj, strInfo_);
        }

        /// <summary>
        /// 是否启用Debug输出（启用后以LogLevels.Debug等级输出OnLogger）
        /// </summary>
        /// <param name="bEnabled_"></param>
        public override void SetDebugEnabled(bool bEnabled_)
        {
            base.SetDebugEnabled(bEnabled_);
            _quToSend.SetDebugEnabled(bEnabled_);
        }

        void NetComm_OnDataReceived(XReceiveDataArgs argData_)
        {
            try
            {
                var receiveData = XJsonDataFormat.Json2DataWithType<TEnum>(argData_.Data);
                if (DebugEnabled)
                {
                    InvokeOnDebug("DataReceived(From:{0}, Type:{1}, Index:{2})",
                        argData_.From, receiveData.DataType, receiveData.DataIndex);
                }

                if (argData_.Type == XReceiveDataType.SendBack || receiveData.IsResponse)
                {
                    HandleResponseData(argData_.Type, argData_.From, receiveData);
                }
                else
                {
                    InvokeRequestArrival(argData_.From, receiveData);
                }
            }
            catch (Exception ex)
            {
                InvokeOnExcept(ex, string.Format("[From:{0}]", argData_.From));
                InvokeDecodeDataFail(argData_.From, argData_.Data, ex);
            }
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            _bStarted = true;
            _quToSend.Restart();
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            _bStarted = false;

            ClearSendData();
            ClearWaitData();
        }

        /// <summary>
        /// 异步发送数据(Version 0)
        /// </summary>
        /// <param name="strTo_">数据发送给谁（对于一对一连接，如TCP，使用空字符串代替）</param>
        /// <param name="euRequestType_">数据类型（用于处理应答与请求的对应）,不能是简单类型（string、int等不能序列化，必须封装到类中）</param>
        /// <param name="tRequestData_">发送的具体数据</param>
        /// <param name="actComplete_">完成时调用激发的动作（如果Exception为null，说明发送成功）</param>
        /// <param name="bNeedResponse_">是否需要应答：如果需要，则等待应答包到达或超时，才会完成；否则，发送完数据即为完成</param>
        /// <param name="oTag_"></param>
        public void SendAsyn(string strTo_, TEnum euRequestType_, object tRequestData_, Action<SendCompleteArgs> actComplete_, bool bNeedResponse_ = true, object oTag_=null)
        {
            SendAsyn(strTo_, euRequestType_, tRequestData_, 0, actComplete_, bNeedResponse_, oTag_);
        }

        /// <summary>
        /// 异步发送数据
        /// </summary>
        /// <param name="strTo_">数据发送给谁（对于一对一连接，如TCP，使用空字符串代替）</param>
        /// <param name="euRequestType_">数据类型（用于处理应答与请求的对应）,不能是简单类型（string、int等不能序列化，必须封装到类中）</param>
        /// <param name="nDataVer_">发送数据的版本(DataWithType.DataVer)</param>
        /// <param name="tRequestData_">发送的具体数据</param>
        /// <param name="actComplete_">完成时调用激发的动作（如果Exception为null，说明发送成功）</param>
        /// <param name="bNeedResponse_">是否需要应答：如果需要，则等待应答包到达或超时，才会完成；否则，发送完数据即为完成</param>
        /// /// <param name="oTag_"></param>
        public void SendAsyn(string strTo_, TEnum euRequestType_, object tRequestData_, int nDataVer_, Action<SendCompleteArgs> actComplete_, bool bNeedResponse_ = true, object oTag_ = null)
        {
            if (string.IsNullOrEmpty(strTo_))
                strTo_ = NetComm.RemoteAddress;
            //else
            //    strTo_ = strTo_.ToLower();

            AddSendData(strTo_, _nSequence.GetNext(), nDataVer_, euRequestType_, tRequestData_, actComplete_, false, bNeedResponse_, oTag_);
        }
    }
}
