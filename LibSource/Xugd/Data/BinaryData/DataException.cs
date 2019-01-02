using SHCre.Xugd.Common;

namespace SHCre.Xugd.Data
{
    /// <summary>
    /// 序列化数据相关异常
    /// </summary>
    public class XBinaryDataException:SHException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strMsg_"></param>
        /// <param name="nErrCode_"></param>
        public XBinaryDataException(string strMsg_, int nErrCode_)
            : base(strMsg_, nErrCode_)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strMsg_"></param>
        /// <param name="euErrCode_"></param>
        public XBinaryDataException(string strMsg_, SHErrorCode euErrCode_)
            : base(strMsg_, euErrCode_)
        {
        }
    }
}
