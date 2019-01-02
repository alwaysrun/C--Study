using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace SHCre.Xugd.Net.Xmpp
{
    /// <summary>
    /// 错误返回值
    /// </summary>
    public enum XmppResultCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success,
        /// <summary>
        /// 流错误
        /// </summary>
        StreamError,
        /// <summary>
        /// 流版本错误
        /// </summary>
        StreamVersionError,
        /// <summary>
        /// 流关闭
        /// </summary>
        StreamClosed,
        /// <summary>
        /// 需要代理验证
        /// </summary>
        ProxyAuthRequired,
        /// <summary>
        /// 代理验证失败
        /// </summary>
        ProxyAuthFailed,
        /// <summary>
        /// 代理不支持验证
        /// </summary>
        ProxyNoSupportedAuth,
        /// <summary>
        /// 读写错误
        /// </summary>
        IoError,
        /// <summary>
        /// 解析错误
        /// </summary>
        ParseError,
        /// <summary>
        /// 链接被拒绝
        /// </summary>
        ConnectionRefused,
        /// <summary>
        /// Dns解析错误
        /// </summary>
        DnsError,
        /// <summary>
        /// 内存错误
        /// </summary>
        OutOfMemory,
        /// <summary>
        /// 不支持的验证方式
        /// </summary>
        NoSupportedAuth,
        /// <summary>
        /// Tls错误
        /// </summary>
        TlsFailed,
        /// <summary>
        /// Tls无效
        /// </summary>
        TlsNotAvailable,
        /// <summary>
        /// 压缩失败
        /// </summary>
        CompressionFailed,
        /// <summary>
        /// 验证失败
        /// </summary>
        AuthenticationFailed,
        /// <summary>
        /// 用户断开了链接
        /// </summary>
        UserDisconnected,
        /// <summary>
        /// 未连接
        /// </summary>
        NotConnected,

        // Below is added
        /// <summary>
        /// 参数为null
        /// </summary>
        ParamIsNULL,
        /// <summary>
        /// 超时
        /// </summary>
        Timeout,
        /// <summary>
        /// 用户Jid未查找到（还未登录）
        /// </summary>
        NotFound,	// UserJid not found(nod login)
        /// <summary>
        /// 已登录
        /// </summary>
        HasLogin,	// User has login
        /// <summary>
        /// 没有应答Ping
        /// </summary>
        NoPong,
    };

    internal enum XmppPresenceType
    {
        Available,    // The entity is online. 
        Chat,         // The entity is 'available for chat'. 
        Away,         // The entity is away. 
        DND,          // The entity is DND (Do Not Disturb). 
        XA,           // The entity is XA (eXtended Away). 
        Unavailable,  // The entity is offline. 
        Probe,        // This is a presence probe. 
        Error,        // This is a presence error. 
        Invalid       // The stanza is invalid. 
    };


    internal enum XmppMsgType
    {
        Chat = 1,        // A chat message.
        Error = 2,        // An error message.
        Groupchat = 4,        // A groupchat message.
        Headline = 8,        // A headline message.
        Normal = 16,        // A normal message.
        Invalid = 32         // The message stanza is invalid.
    };

    /// <summary>
    /// 导入xugd.xmpp库
    /// </summary>
    internal static class XmppImport
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DelOnLogin([MarshalAs(UnmanagedType.LPStr)] string strUserJid_);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DelOnClose(
            [MarshalAs(UnmanagedType.LPStr)] string strUserJid_,
            XmppResultCode euErr_);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DelOnPresence(
            [MarshalAs(UnmanagedType.LPStr)] string strUserJid_,
            [MarshalAs(UnmanagedType.LPStr)] string strFriendJid_,
            XmppPresenceType euType_);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet=CharSet.Ansi)]
        public delegate void DelOnMsgReceive(
            [MarshalAs(UnmanagedType.LPStr)] string strUserJid_,
            [MarshalAs(UnmanagedType.LPStr)] string strFrom_,
            IntPtr pMsg_,
            int nCount_,
            XmppMsgType euType_);

        //////////////////////////////////////////////////////////////////////////
        // jid（要全部使用小写）: name@domain

        [DllImport("Library\\xugd.xmpp.dll")]
        public static extern XmppResultCode XmppLogin(
            [MarshalAs(UnmanagedType.LPStr)] string strUserJid_,
            [MarshalAs(UnmanagedType.LPStr)] string strPsw_,
            [MarshalAs(UnmanagedType.LPStr)] string strIp_,
            int nPort_,
            [MarshalAs(UnmanagedType.LPWStr)] string strLogFile_);

        [DllImport("Library\\xugd.xmpp.dll")]
        public static extern void XmppClose([MarshalAs(UnmanagedType.LPStr)] string strUserJid_);

        [DllImport("Library\\xugd.xmpp.dll")]
        public static extern XmppResultCode XmppGetStartStream(
            [MarshalAs(UnmanagedType.LPStr)] string strUserJid_,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder strStream_,
            int nSize_);

        public static string GetStartStream(string strUserJid_)
        {
            StringBuilder sbAddr = new StringBuilder(250);
            XmppResultCode euCode = XmppGetStartStream(strUserJid_, sbAddr, sbAddr.Capacity);
            if (euCode != XmppResultCode.Success)
                throw new XNetConnException((uint)euCode,
                    string.Format("XmppGetStartStream({0}) failed: {1}", strUserJid_, euCode));
            return sbAddr.ToString();
        }

        [DllImport("Library\\xugd.xmpp.dll")]
        public static extern XmppResultCode XmppGetRemoteAddress(
            [MarshalAs(UnmanagedType.LPStr)] string strUserJid_,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder strAddress_,
            int nSize_);

        [DllImport("Library\\xugd.xmpp.dll")]
        public static extern XmppResultCode XmppGetLocalAddress(
            [MarshalAs(UnmanagedType.LPStr)] string strUserJid_,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder strAddress_,
            int nSize_);

        public static string GetLocalAddr(string strUserJid_){
            StringBuilder sbAddr = new StringBuilder(50);
            XmppResultCode euCode = XmppGetLocalAddress(strUserJid_, sbAddr, sbAddr.Capacity);
            if (euCode != XmppResultCode.Success)
                throw new XNetConnException((uint)euCode,
                    string.Format("XmppGetLocalAddress({0}) failed: {1}", strUserJid_, euCode));
            return sbAddr.ToString();
        }

        [DllImport("Library\\xugd.xmpp.dll")]
        public static extern XmppResultCode XmppSetLogLevel(
            [MarshalAs(UnmanagedType.LPStr)] string strUserJid_,
            int nLevel_);

        //[DllImport("Library\\xugd.xmpp.dll")]
        //public static extern XmppResultCode XmppSendMsg(
        //    [MarshalAs(UnmanagedType.LPStr)] string strUserJid_,
        //    [MarshalAs(UnmanagedType.LPStr)] string strToJid_,
        //    [MarshalAs(UnmanagedType.LPStr)] string strUtf8Msg_);

        [DllImport("Library\\xugd.xmpp.dll")]
        public static extern XmppResultCode XmppSendByteMsg(
            [MarshalAs(UnmanagedType.LPStr)] string strUserJid_,
            [MarshalAs(UnmanagedType.LPStr)] string strToJid_,
            [In] byte[] strUtf8_,
            int nCount_);

        [DllImport("Library\\xugd.xmpp.dll")]
        public static extern XmppResultCode XmppSubscribe(
            [MarshalAs(UnmanagedType.LPStr)] string strUserJid_,
            [MarshalAs(UnmanagedType.LPStr)] string strFriendJid_);

        [DllImport("Library\\xugd.xmpp.dll")]
        public static extern XmppResultCode XmppUnsubscribe(
            [MarshalAs(UnmanagedType.LPStr)] string strUserJid_,
            [MarshalAs(UnmanagedType.LPStr)] string strFriendJid_);

        [DllImport("Library\\xugd.xmpp.dll")]
        public static extern XmppResultCode XmppSetCallbackLogin(
            [MarshalAs(UnmanagedType.LPStr)] string strUserJid_,
            DelOnLogin pfunLogin_);

        [DllImport("Library\\xugd.xmpp.dll")]
        public static extern XmppResultCode XmppSetCallbackClose(
            [MarshalAs(UnmanagedType.LPStr)] string strUserJid_,
            DelOnClose pfunClose_);

        [DllImport("Library\\xugd.xmpp.dll")]
        public static extern XmppResultCode XmppSetCallbackPresence(
            [MarshalAs(UnmanagedType.LPStr)] string strUserJid_,
            DelOnPresence pfunPresence_);

        [DllImport("Library\\xugd.xmpp.dll")]
        public static extern XmppResultCode XmppSetCallbackMsgReceive(
            [MarshalAs(UnmanagedType.LPStr)] string strUserJid_,
            DelOnMsgReceive pfunMsgReceive_);
    }
}
