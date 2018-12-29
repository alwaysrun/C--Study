using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SHCre.Xugd.WinAPI
{
    partial class XWin
    {
        #region "Adjust Privileges"
        static string[] _strPrivName = 
            {
                "SeSecurityPrivilege",
                "SeTakeOwnershipPrivilege",
                "SeLoadDriverPrivilege",
                "SeSystemProfilePrivilege",
                "SeSystemtimePrivilege",
                "SeBackupPrivilege",
                "SeRestorePrivilege",
                "SeShutdownPrivilege",
                "SeDebugPrivilege",
                "SeAuditPrivilege",
                "SeSystemEnvironmentPrivilege",
                "SeRemoteShutdownPrivilege",
                "SeManageVolumePrivilege",
                "SeImpersonatePrivilege",
                "SeCreateGlobalPrivilege",
                "SeTcbPrivilege"
            };

        /// <summary>
        /// 权限类型
        /// </summary>
        public enum PrivilegeType
        {
            /// <summary>
            /// 安全权限
            /// </summary>
            Security = 0,
            /// <summary>
            /// 所有权
            /// </summary>
            TakeOwnership,
            /// <summary>
            /// 加载驱动
            /// </summary>
            LoadDriver,
            /// <summary>
            /// 系统概要
            /// </summary>
            SystemProfile,
            /// <summary>
            /// 系统时间
            /// </summary>
            SystemTime,
            /// <summary>
            /// 备份
            /// </summary>
            Backup,
            /// <summary>
            /// 回复
            /// </summary>
            Restore,
            /// <summary>
            /// 关机
            /// </summary>
            Shutdown,
            /// <summary>
            /// 调试
            /// </summary>
            Debug,
            /// <summary>
            /// 审计
            /// </summary>
            Audit,
            /// <summary>
            /// 系统环境
            /// </summary>
            Environment,
            /// <summary>
            /// 远程关机
            /// </summary>
            RemoteShutdown,
            /// <summary>
            /// 操作卷
            /// </summary>
            ManageVolume,
            /// <summary>
            /// 模拟
            /// </summary>
            Impersonate,
            /// <summary>
            /// 创建全局
            /// </summary>
            CreateGlobal,
            /// <summary>
            /// 持有者为可信的计算机
            /// </summary>
            TcbName,
        };

        /// <summary>
        /// 更改权限
        /// </summary>
        /// <param name="euType_">权限类型</param>
        public static void AdjustPrivilege(PrivilegeType euType_)
        {
            string strName = _strPrivName[(int)euType_];
            if (string.IsNullOrEmpty(strName))
                throw new ArgumentException("Invalid Type");

            TOKEN_PRIVILEGES stPriv = new TOKEN_PRIVILEGES();
            if (!LookupPrivilegeValueW(null, strName, out stPriv.Luid))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Call LookupPrivilegeValue failed");

            IntPtr hToken;
            if (!OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, out hToken))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Call OpenProcessToken failed");

            try
            {
                stPriv.PrivilegeCount = 1;
                stPriv.Attributes = SE_PRIVILEGE_ENABLED;
                if (!AdjustTokenPrivileges(hToken, false, ref stPriv, 0, IntPtr.Zero, IntPtr.Zero))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Call AdjustTokenPrivileges failed");
            }
            finally
            {
                CloseHandle(hToken);
                hToken = IntPtr.Zero;
            }
        }
        #endregion

        #region "Token Pinvoke"
        const uint SE_PRIVILEGE_ENABLED_BY_DEFAULT = 0x00000001;
        const uint SE_PRIVILEGE_ENABLED = 0x00000002;
        const uint SE_PRIVILEGE_REMOVED = 0x00000004;
        const uint SE_PRIVILEGE_USED_FOR_ACCESS = 0x80000000;
        const uint TOKEN_QUERY = 0x00000008;
        const uint TOKEN_ADJUST_PRIVILEGES = 0x00000020;

        // Full structure
        //[StructLayout(LayoutKind.Sequential)]
        //struct LUID_AND_ATTRIBUTES
        //{
        //    public LUID Luid;
        //    public uint Attributes;
        //}
        //struct TOKEN_PRIVILEGES
        //{
        //    public uint PrivilegeCount;
        //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = ANYSIZE_ARRAY)]
        //    public LUID_AND_ATTRIBUTES[] Privileges;
        //}

        // Alternate simple structure for single privilege setting
        [StructLayout(LayoutKind.Sequential)]
        struct TOKEN_PRIVILEGES
        {
            public uint PrivilegeCount;
            public LUID Luid;
            public uint Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct LUID
        {
            public uint LowPart;
            public int HighPart;
        }

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool LookupPrivilegeValueW(string lpSystemName, string lpName, out LUID lpLuid);

        /// <summary>
        /// 获取当前进程的句柄
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

        /// <summary>
        /// 关闭句柄
        /// </summary>
        /// <param name="hObject"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AdjustTokenPrivileges(
            IntPtr TokenHandle,
           [MarshalAs(UnmanagedType.Bool)]
            bool DisableAllPrivileges,
           ref TOKEN_PRIVILEGES NewState,
           uint BufferLengthInBytes,
           ref TOKEN_PRIVILEGES PreviousState,
           out uint ReturnLengthInBytes);

        // Use this signature if you do not want the previous state
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AdjustTokenPrivileges(
            IntPtr TokenHandle,
           [MarshalAs(UnmanagedType.Bool)]
            bool DisableAllPrivileges,
           ref TOKEN_PRIVILEGES NewState,
           uint Zero,
           IntPtr Null1,
           IntPtr Null2);
        #endregion
    }
}
