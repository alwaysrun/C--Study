using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Extension;
using SHCre.Xugd.CFile;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.MsgData
{
    partial class XMsgDispatch<TEnum>
    {
        XDelExpireList<MsgToSend> _lstWaitResponse = null;

        private void AddResponseToSend(ReceivedMsg msgRequest_, IMsgDataBase argResponse_, Action<XAsyncResult> actComplete_, object oTag_)
        {
            TEnum euType = msgRequest_.Data.Type;
            if (oTag_ is TEnum)
            {
                euType = (TEnum)oTag_;
				oTag_ = null;
            }
            var toSend = new MsgToSend(msgRequest_.From, euType, argResponse_, oTag_, msgRequest_.Data.Version, XMsgMode.Response);
            toSend.ActDataComplete = actComplete_;
            toSend.SetData(msgRequest_.Data.Index);
            _quToSend.AddItem(toSend);
        }

        private void HandleResponseMsg(XMsgReceivedArgs argMsg_, XMsgWithType<TEnum> revMsg)
        {
            var oriRequest = _lstWaitResponse.RemoveFirst(z => z.MsgWithType.Index == revMsg.Index);
            if (oriRequest != null)
            {
                if (argMsg_.Type == XMsgReceivedType.SendBack)
                    oriRequest.InvokeComplete(XAsyncResult.Get(new XNotLoginException("Message SendBack")), null);
                else
                    oriRequest.InvokeComplete(XAsyncResult.OK, revMsg);
            }
            else
            {
                string strInfo = string.Format("MismatchResponse(From:{0}, Type:{1}, Mode:{2}, Index:{3})",
                    argMsg_.From, revMsg.Type, revMsg.Mode, revMsg.Index);
                InvokeOnLogger(strInfo, XLogSimple.LogLevels.Warn);
                InvokeHandleError(new XResponsedMismatchException(strInfo)
                    {
                        ResponseMsg = argMsg_,
                    });
            }
        }

        private void HandleExpiredMsg(List<MsgToSend> lstMsg_)
        {
            DateTime dtNow = DateTime.Now;
            foreach (var msg in lstMsg_)
            {
                string strInfo = string.Format("Request wait response Timeout(Type:{0}, Index:{1}, AddTime:{2}, SendTime:{3}, OverTime:{4})",
                    msg.MsgWithType.Type, msg.MsgWithType.Index, msg.AddTime, msg.SendTime.Value, dtNow);
                InvokeOnLogger(strInfo, XLogSimple.LogLevels.Warn);

                msg.ActRequestComplete(XAsyncResult.Get(new TimeoutException(strInfo)), null);
            }
        }

        private void ClearWaitData()
        {
            _lstWaitResponse.Clear((lstWait) =>
                {
                    foreach (var wData in lstWait)
                    {
                        var exStop = new XDataException(string.Format("MsgDispatch stopped: Msg(Add:{0}, Send:{1}, Type:{2}, Mode:{3}, Index:{4}) has not get response",
                                    XTime.GetTimeString(wData.AddTime),
                                    XTime.GetTimeString(wData.SendTime.Value),
                                    wData.MsgWithType.Type, wData.MsgWithType.Mode, wData.MsgWithType.Index));
                        wData.InvokeComplete(XAsyncResult.Get(exStop), null);
                    }
                });

            _lstWaitResponse.Stop();
        }
    } // class
}
