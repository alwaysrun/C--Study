using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SHCre.Xugd.WinAPI
{
    partial class XWin
    {
        /// <summary>
        /// windows操作相关类
        /// </summary>
        public static class Windows
        {
            #region "Exit Window(Restart logoff shudown)"
            enum ExitType
            {
                Logoff = 0,
                Shutdown = 0x01,
                Reboot = 0x02,
                //Force = 0x04,
                //Poweroff = 0x08,
                ForceIfHung = 0x010
            }

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool ExitWindowsEx(ExitType euType_, uint nReason_);

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool LockWorkStation();

            private static void ExitWindow(ExitType euType_)
            {
                AdjustPrivilege(PrivilegeType.Shutdown);
                if (!ExitWindowsEx(euType_, 0))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            /// <summary>
            /// 注销
            /// </summary>
            public static void Logoff()
            {
                ExitWindow(ExitType.Logoff | ExitType.ForceIfHung);
            }

            /// <summary>
            /// 关机
            /// </summary>
            public static void Shutdown()
            {
                ExitWindow(ExitType.Shutdown | ExitType.ForceIfHung);
            }

            /// <summary>
            /// 重启
            /// </summary>
            public static void Reboot()
            {
                ExitWindow(ExitType.Reboot | ExitType.ForceIfHung);
            }

            /// <summary>
            /// 锁定计算机
            /// </summary>
            public static void Lock()
            {
                if (!LockWorkStation())
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Call LockWorkStation failed");
            }
            #endregion

            #region "ExecutionState(System Sleep, Display turn on/off)"
            /// <summary>
            /// 休眠与显示器关闭与恢复标志
            /// </summary>
            [Flags]
            public enum SleepState : uint
            {
                /// <summary>
                /// 重设系统休眠策略
                /// </summary>
                System = 0x00000001,
                /// <summary>
                /// 重设显示器显示策略
                /// </summary>
                Display = 0x00000002,
                /// <summary>
                /// 与上面两个组合则禁止休眠与关闭；单独则恢复默认策略
                /// </summary>
                Continus = 0x80000000,
            }

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern uint SetThreadExecutionState(SleepState flags);

            /// <summary>
            /// 阻止系统休眠，直到线程结束恢复休眠策略（调用后，线程不能退出）
            /// </summary>
            /// <param name="bDisplayOn_">是否阻止关闭显示器</param>
            /// <returns>设定前的状态；如果出错抛出Win32Exception异常</returns>
            public static SleepState PreventSleep(bool bDisplayOn_ = false)
            {
                SleepState euState = SleepState.System | SleepState.Continus;
                if (bDisplayOn_)
                    euState |= SleepState.Display;

                uint nResult = SetThreadExecutionState(euState);
                if (nResult == 0)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                return (SleepState)nResult;
            }

            /// <summary>
            /// 恢复系统休眠策略
            /// </summary>
            public static void RestoreSleep()
            {
                SetThreadExecutionState(SleepState.Continus);
            }

            /// <summary>
            /// 重置系统休眠计时器
            /// </summary>
            /// <param name="bDisplayOn_">是否阻止关闭显示器</param>
            /// <returns>设定前的状态；如果出错抛出Win32Exception异常</returns>
            public static SleepState ResetSleepTimer(bool bDisplayOn_ = false)
            {
                SleepState euState = SleepState.System;
                if (bDisplayOn_)
                    euState |= SleepState.Display;

                uint nResult = SetThreadExecutionState(euState);
                if (nResult == 0)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                return (SleepState)nResult;
            }

            /// <summary>
            /// 关闭显示器
            /// </summary>
            public static void TurnoffDisplay()
            {
                XUser.PostSysCommandMsg(XUser.BroadCastMsgWnd, XUser.SysCommandParam.MonitorPower, 2);
            }
            #endregion

            [DllImport("Kernel32.dll")]
            private static extern ulong GetTickCount64();
            [DllImport("Kernel32.dll")]
            private static extern uint GetTickCount();

            /// <summary>
            /// 获取电脑已运行的时间
            /// </summary>
            /// <returns></returns>
            public static TimeSpan GetStartPeriod()
            {
                ulong nTicks;
                var verOS = Environment.OSVersion;
                if (verOS.Version.Major >= 6) // Vista or Win7
                    nTicks = GetTickCount64();
                else // <=Win2003
                    nTicks = GetTickCount();

                return TimeSpan.FromMilliseconds(nTicks);
            }

            /// <summary>
            /// 获取电脑的启动时间
            /// </summary>
            /// <returns></returns>
            public static DateTime GetStartTime()
            {
                return DateTime.Now - GetStartPeriod();
            }
        } // Windows
    } // XWin
}
