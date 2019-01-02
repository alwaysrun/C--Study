using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Extension;
using SHCre.Xugd.Common;
using SHCre.Xugd.CFile;

namespace SHCre.Xugd.MsgData
{
    partial class XMsgDispatch<TEnum>
    {
        class MsgToSend : XDelExpireItemBase
        {
            public Action<XAsyncResult> ActDataComplete { get; set; }
            public Action<XAsyncResult, XMsgWithType<TEnum>> ActRequestComplete { get; set; }
            public void InvokeComplete(XAsyncResult asynResult_, XMsgWithType<TEnum> msgResponse_)
            {
                if (MsgWithType.IsRequest())
                {
                    if (ActRequestComplete != null)
                        ActRequestComplete(asynResult_, msgResponse_);
                }
                else
                {
                    if (ActDataComplete != null)
                        ActDataComplete(asynResult_);
                }
            }

            public string To { get; set; }
            public object Tag { get; set; }

            public IMsgDataBase Data { get; set; }
            public XMsgWithType<TEnum> MsgWithType { get; set; }

            public DateTime AddTime { get; set; }
            public DateTime? SendTime { get; set; }

            public void SetData(int nSeq_)
            {
                MsgWithType.Index = nSeq_;
                MsgWithType.SetData(Data);
                if(Tag != null)
                {
                    var dicAdd = Tag as Dictionary<string, string>;
                    if(dicAdd != null)
                    {
                        MsgWithType.AddHeader(dicAdd);
                    }
                }
            }

            public DateTime AnchorTime()
            {
                return SendTime.Value;
            }

            public bool IsResponse(int nIndex_)
            {
                return (MsgWithType.Index == nIndex_);
            }

            public MsgToSend(string strTo_, TEnum euType_, IMsgDataBase argData_, object oTag_, int nVersion_, XMsgMode euMode_)
            {
                To = strTo_;
                Tag = oTag_;
                Data = argData_;
                AddTime = DateTime.Now;

                MsgWithType = new XMsgWithType<TEnum>()
                {
                    Type = euType_,
                    Version = nVersion_,
                    Mode = euMode_,
                };
            }
        }

        //////////////////////////////////////////////////////////////////////////
        private XSafeSequence _nSequence = new XSafeSequence();
        XQueueIssue<MsgToSend> _quToSend = null;

        private bool SendMsgData(MsgToSend msgSend_)
        {
            try
            {
                string strJson = msgSend_.MsgWithType.ToJson();

                msgSend_.SendTime = DateTime.Now;
                if (msgSend_.MsgWithType.IsRequest())
                {
                    _lstWaitResponse.AddItem(msgSend_);
                }

                MsgConnection.SendAsync(msgSend_.To, strJson, (zSend) =>
                    {
                        if (zSend.IsSuccess && msgSend_.MsgWithType.IsRequest())
                        { // Just wait OP-Response
                        }
                        else
                        {
                            if (msgSend_.MsgWithType.IsRequest())
                            { // Remove from Wait-List
                                _lstWaitResponse.RemoveItem(msgSend_);
                            }

                            msgSend_.InvokeComplete(zSend, null);
                        }
                    },
                    msgSend_.Tag);
            }
            catch (Exception ex)
            {
                InvokeOnExcept(ex, "SendMsgData");
                msgSend_.InvokeComplete(XAsyncResult.Get(ex), null);
            }

            return true;
        }

        private void ClearSendData()
        {
            _quToSend.Clear((quAll) =>
                {
                    foreach (var quData in quAll)
                    {
                        var exStop = new XDataException(string.Format("MsgDispatch stopped: Msg(Add:{0}, Type:{1}, Mode:{2}, Index:{3}) has not send",
                                    XTime.GetTimeString(quData.AddTime),
                                    quData.MsgWithType.Type, quData.MsgWithType.Mode, quData.MsgWithType.Index));
                        quData.InvokeComplete(XAsyncResult.Get(exStop), null); ;
                    }
                });

            _quToSend.Stop();
        }
    } // class
}
