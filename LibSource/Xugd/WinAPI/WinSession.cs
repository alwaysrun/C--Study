using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace SHCre.Xugd.WinAPI
{
    partial class XWin
    {
        #region "Process API"
        [DllImport("kernel32.dll")]
        static extern uint GetCurrentProcessId();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ProcessIdToSessionId(uint dwProcessId, out uint pSessionId);
        #endregion

        #region "Session APIs"
        static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("wtsapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool WTSSendMessageW(
                    IntPtr hServer,
                    [MarshalAs(UnmanagedType.U4)] uint SessionId,
                    String pTitle,
                    [MarshalAs(UnmanagedType.U4)] int TitleLength,
                    String pMessage,
                    [MarshalAs(UnmanagedType.U4)] int MessageLength,
                    [MarshalAs(UnmanagedType.U4)] int Style,
                    [MarshalAs(UnmanagedType.U4)] int Timeout,
                    [MarshalAs(UnmanagedType.U4)] out int pResponse,
                    [MarshalAs(UnmanagedType.Bool)] bool bWait);


        private enum WTS_CONNECTSTATE_CLASS
        {
            WTSActive,
            WTSConnected,
            WTSConnectQuery,
            WTSShadow,
            WTSDisconnected,
            WTSIdle,
            WTSListen,
            WTSReset,
            WTSDown,
            WTSInit
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WTS_SESSION_INFO
        {
            public uint SessionID;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pWinStationName;
            public WTS_CONNECTSTATE_CLASS State;
        }

        [DllImport("wtsapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]        
        static extern bool WTSEnumerateSessions(
            IntPtr hServer,
            [MarshalAs(UnmanagedType.U4)] int Reserved,
            [MarshalAs(UnmanagedType.U4)] int Version,
            ref IntPtr ppSessionInfo,
            [MarshalAs(UnmanagedType.U4)] ref int pSessionInfoCount
            );

        [DllImport("wtsapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]      
        static extern void WTSFreeMemory(IntPtr pMemory);

        [DllImport("wtsapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool WTSQueryUserToken(
            [MarshalAs(UnmanagedType.U4)] uint sessionId,
            out IntPtr Token);

        #region "CreateProcessAsUser"
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct STARTUPINFO
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        /// <summary>
        /// 以当前登录的windows用户(角色权限)运行指定程序进程
        /// </summary>
        /// <param name="hToken"></param>
        /// <param name="lpApplicationName">指定程序(全路径)</param>
        /// <param name="lpCommandLine">参数</param>
        /// <param name="lpProcessAttributes">进程属性</param>
        /// <param name="lpThreadAttributes">线程属性</param>
        /// <param name="bInheritHandles"></param>
        /// <param name="dwCreationFlags"></param>
        /// <param name="lpEnvironment"></param>
        /// <param name="lpCurrentDirectory"></param>
        /// <param name="lpStartupInfo">程序启动属性</param>
        /// <param name="lpProcessInformation">最后返回的进程信息</param>
        /// <returns>是否调用成功</returns>
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern bool CreateProcessAsUserW(
            IntPtr hToken,
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes, //ref SECURITY_ATTRIBUTES lpProcessAttributes,
            IntPtr lpThreadAttributes,  //ref SECURITY_ATTRIBUTES lpThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);   

        #endregion
        #endregion

        /// <summary>
        /// 获取当前进程所在的Session
        /// </summary>
        /// <returns></returns>
        public static uint GetCurrentSessionId()
        {
            var nProcId = GetCurrentProcessId();
            uint nSessionId = 0;
            if(!ProcessIdToSessionId(nProcId, out nSessionId))
                throw new Win32Exception(Marshal.GetLastWin32Error(), string.Format("ProcessIdToSessionId({0}) fail", nProcId));

            return nSessionId;
        }

        /// <summary>
        /// 服务程序执行消息提示,前台MessageBox.Show
        /// </summary>
        /// <param name="strMsg_">消息内容</param>
        /// <param name="strTitle">标题</param>
        public static void SrvShowMessage(string strMsg_, string strTitle)
        {
            int resp = 0;
            WTSSendMessageW(
                WTS_CURRENT_SERVER_HANDLE, 
                WTSGetActiveConsoleSessionId(), 
                strTitle, 
                strTitle.Length, 
                strMsg_, 
                strMsg_.Length, 
                0, 
                0, 
                out resp, 
                false);
        }

        /// <summary>
        /// 以当前登录系统的用户角色权限启动指定的进程
        /// </summary>
        /// <param name="strProcess_">指定的进程(全路径)</param>
        /// <returns>执行情况说明（若出错，则-ERR:起始的字符串）</returns>
        public static string SrvCreateProcess(string strProcess_)
        {
            const string ErrPrefix = "-ERR: ";

            IntPtr ppSessionInfo = IntPtr.Zero;
            int nCount = 0;
            string strRet = ErrPrefix;
            
            if ( WTSEnumerateSessions(
                     WTS_CURRENT_SERVER_HANDLE,  // Current RD Session Host Server handle would be zero. 
                     0,  // This reserved parameter must be zero. 
                     1,  // The version of the enumeration request must be 1. 
                     ref ppSessionInfo, // This would point to an array of session info. 
                     ref nCount  // This would indicate the length of the above array.
                     )
                )
            {
                strRet += "SessionCount-" + nCount;
                for (int nIndex = 0; nIndex < nCount; nIndex++)
                {
                    WTS_SESSION_INFO tSessionInfo = (WTS_SESSION_INFO)Marshal.PtrToStructure(
                        ppSessionInfo + nIndex * Marshal.SizeOf(typeof(WTS_SESSION_INFO)), 
                        typeof(WTS_SESSION_INFO));

                    if (WTS_CONNECTSTATE_CLASS.WTSActive == tSessionInfo.State)
                    {
                        IntPtr hToken = IntPtr.Zero;
                        //AdjustPrivilege(PrivilegeType.TcbName);
                        if (WTSQueryUserToken(tSessionInfo.SessionID, out hToken))
                        {
                            PROCESS_INFORMATION tProcessInfo;
                            STARTUPINFO tStartUpInfo = new STARTUPINFO();
                            tStartUpInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));
                            bool bCreated = CreateProcessAsUserW(
                                     hToken,             // Token of the logged-on user. 
                                     strProcess_,       // Name of the process to be started. 
                                     null,               // Any command line arguments to be passed. 
                                     IntPtr.Zero,        // Default Process' attributes. 
                                     IntPtr.Zero,        // Default Thread's attributes. 
                                     false,              // Does NOT inherit parent's handles. 
                                     0,                  // No any specific creation flag. 
                                     IntPtr.Zero,        // Default environment path. 
                                     null,               // Default current directory. 
                                     ref tStartUpInfo,   // Process Startup Info.  
                                     out tProcessInfo    // Process information to be returned. 
                                   );
                            if (bCreated)
                            {
                                CloseHandle(tProcessInfo.hThread);
                                CloseHandle(tProcessInfo.hProcess);

                                strRet = string.Format("XWin.ProcStart {0} OK", strProcess_);
                            }
                            else
                            {
                                strRet = string.Format("{0}CreateProcessAsUser fail: {1}", ErrPrefix, Marshal.GetLastWin32Error());
                            }

                            CloseHandle(hToken);
                            break;
                        } // if-QueryToken
                        else
                        {
                            strRet = string.Format("{0}WTSQueryUserToken fail: {1}", ErrPrefix, Marshal.GetLastWin32Error());
                        }
                    }
                } // for
                WTSFreeMemory(ppSessionInfo);
            } // if-Enum
            else
            {
                strRet = string.Format("{0}WTSEnumerateSessions fail: {1}", ErrPrefix, Marshal.GetLastWin32Error());
            }

            return strRet;
        }
    } // class
}
