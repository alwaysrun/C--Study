using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.CFile;
using SHCre.Xugd.Extension;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.MsgData
{
    /// <summary>
    /// 消息分发类（如果是请求Request，则会等待应答Response获取后才返回；否则在发送完成后即返回）
    /// </summary>
    /// <typeparam name="TEnum">枚举类型：用于标识消息类型，便于序列化</typeparam>
    public partial class XMsgDispatch<TEnum> : XLogEventsBase where TEnum : struct,IComparable
    {
        /// <summary>
        /// 底层消息连接
        /// </summary>
        public IMsgConnection MsgConnection {get; private set;}

        /// <summary>
        /// 获取原始消息：若处理过，不需继续下发（通过OnMsgArrived等处理）则返回true；否则返回false。
        /// 若有多个处理程序，则任何一个处理程序返回true，则认为是true。
        /// </summary>
        public event Func<XMsgDispatch<TEnum>, XMsgReceivedArgs, bool> FunGetRawMsg;
        bool HasGetRawMsg(XMsgReceivedArgs argMsg_)
        {
            if (FunGetRawMsg == null) return false;

            bool bRet = false;
            try
            {
                var lstGet = FunGetRawMsg.GetInvocationList();
                foreach(var fun in lstGet)
                {
                    var funGet = fun as Func<XMsgDispatch<TEnum>, XMsgReceivedArgs, bool>;
                    if (funGet(this, argMsg_))
                        bRet = true;
                }
            }
            catch(Exception ex)
            {
                InvokeOnExcept(ex, "HasGetRawMsg");
            }

            return bRet;
        }

        /// <summary>
        /// 消息到达(FunGetRawMsg没处理的消息）：如果是Request，可调用InvokeResponse来应答
        /// </summary>
        public event Action<XMsgDispatch<TEnum>, ReceivedMsg> OnMsgArrived;
        void InvokeMsgArrived(string strFrom_, string strTopic_, XMsgWithType<TEnum> msgData_)
        {
            if (OnMsgArrived == null)
            {
                InvokeOnLogger("No handler add for OnMsgArrived", XLogSimple.LogLevels.Warn);
                return;
            }

            var revMsg = new ReceivedMsg(strFrom_, strTopic_, msgData_);
            if (msgData_.IsRequest())
                revMsg.ActResponse = AddResponseToSend;
            OnMsgArrived(this, revMsg);
        }

        /// <summary>
        /// 操作出错时触发的事件
        /// </summary>
        public event Action<XMsgDispatch<TEnum>, Exception> OnHandleError;
        void InvokeHandleError(Exception ex)
        {
            if (OnHandleError != null)
                OnHandleError(this, ex);
        }

        /// <summary>
        /// 构造分发类
        /// </summary>
        /// <param name="conMsg_">底层消息连接</param>
        /// <param name="nTimeoutSecond_">等待应答时长（默认30s）：请求发送后，若在TimeoutSecond时间内没有收到应答，则返回超时错误</param>
        /// <param name="nMsgWaitSendMaxCount">允许等待队列的长度（默认20）：超过此长度则启用辅助线程，加快发送</param>
        public XMsgDispatch(IMsgConnection conMsg_, int nTimeoutSecond_ = 30, int nMsgWaitSendMaxCount = 20)
        {
            LogPrefix = "MsgDisp.";
            MsgConnection = conMsg_;
            MsgConnection.OnMsgReceived += MsgConnection_OnMsgReceived;
            if (!MsgConnection.IsLoggerSet())
            {
                MsgConnection.OnLogger += InvokeOnLogger;
                MsgConnection.OnExcept += InvokeOnExcept;
            }

            if (nMsgWaitSendMaxCount < 10)
                nMsgWaitSendMaxCount = 10;
            _quToSend = new XQueueIssue<MsgToSend>(SendMsgData, nMsgWaitSendMaxCount, false);
            _quToSend.OnExcept += InvokeOnExcept;
            _quToSend.OnLogger += InvokeOnLogger;
            _quToSend.LogPrefix = "ToSendMsg.";

            if(nTimeoutSecond_ < 5)
                nTimeoutSecond_ = 5;
            _lstWaitResponse = new XDelExpireList<MsgToSend>(nTimeoutSecond_, nTimeoutSecond_ / 10 + 1, HandleExpiredMsg);
            _lstWaitResponse.OnExcept += InvokeOnExcept;
            _lstWaitResponse.OnLogger += InvokeOnLogger;
            _lstWaitResponse.LogPrefix = "WaitResponse.";
        }

        void MsgConnection_OnMsgReceived(IMsgConnection msgCon_, XMsgReceivedArgs argMsg_)
        {
            try 
            {
                if (HasGetRawMsg(argMsg_)) return;

                var revMsg = XMsgFormat.Json2Data<TEnum>(argMsg_.Data);
                if(DebugEnabled)
                {
                    InvokeOnDebug("OnMsgReceived(From:{0}, Topic:{1}, Type:{2}, Mode:{3}, Index:{4})",
                        argMsg_.From, argMsg_.Topic, revMsg.Type, revMsg.Mode, revMsg.Index);
                }

                if(argMsg_.Type == XMsgReceivedType.SendBack || revMsg.IsResponse())
                {
                    HandleResponseMsg(argMsg_, revMsg);
                }
                else
                {
                    InvokeMsgArrived(argMsg_.From, argMsg_.Topic, revMsg);
                }
            }
            catch(Exception ex)
            {
                InvokeOnExcept(ex, "OnMsgReceived");
                InvokeHandleError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bEnabled_"></param>
        public override void SetDebugEnabled(bool bEnabled_)
        {
            base.SetDebugEnabled(bEnabled_);

            _quToSend.SetDebugEnabled(bEnabled_);
            _lstWaitResponse.SetDebugEnabled(bEnabled_);
            MsgConnection.SetDebugEnabled(bEnabled_);
        }

        /// <summary>
        /// 重启，Stop后再次收发时需要重启
        /// </summary>
        public void Restart()
        {
            _nSequence.Reset(0);
            _quToSend.Restart();
            _lstWaitResponse.Restart();
        }

        /// <summary>
        /// 停掉
        /// </summary>
        public void Stop()
        {
            ClearSendData();
            ClearWaitData();
        }

        /// <summary>
        /// 发送数据（不需要应答）
        /// </summary>
        /// <param name="strTo_"></param>
        /// <param name="euType_"></param>
        /// <param name="argData_"></param>
        /// <param name="actComplete_"></param>
        /// <param name="nDataVer_"></param>
        /// <param name="oTag_"></param>
        public void SendDataAsyn(string strTo_, TEnum euType_, IMsgDataBase argData_, Action<XAsyncResult> actComplete_, int nDataVer_=1, object oTag_ = null)
        {
            InvokeOnCalled("SendDataAsyn(To:{0}, Type:{1})", strTo_, euType_);

            var toSend = new MsgToSend(strTo_, euType_, argData_, oTag_, nDataVer_, XMsgMode.Data);
            toSend.ActDataComplete = actComplete_;
            toSend.SetData(_nSequence.GetNext());
            _quToSend.AddItem(toSend);
        }

        /// <summary>
        /// 发送请求（需要应答）
        /// </summary>
        /// <param name="strTo_"></param>
        /// <param name="euType_"></param>
        /// <param name="argData_"></param>
        /// <param name="actComplete_"></param>
        /// <param name="nDataVer_"></param>
        /// <param name="oTag_"></param>
        public void SendRequestAsyn(string strTo_, TEnum euType_, IMsgDataBase argData_, Action<XAsyncResult, XMsgWithType<TEnum>> actComplete_, int nDataVer_=1, object oTag_ = null)
        {
            InvokeOnCalled("SendRequestAsyn(To:{0}, Type:{1})", strTo_, euType_);

            var toSend = new MsgToSend(strTo_, euType_, argData_, oTag_, nDataVer_, XMsgMode.Request);
            toSend.ActRequestComplete = actComplete_;
            toSend.SetData(_nSequence.GetNext());
            _quToSend.AddItem(toSend);
        }

    } // class
}
