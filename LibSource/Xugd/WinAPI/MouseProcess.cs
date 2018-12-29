using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SHCre.Xugd.WinAPI
{
    /// <summary>
    /// Windows system info
    /// </summary>
    public static class XSystem
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SystemParametersInfo(uint nAction_, uint nParam_, IntPtr pvParam_, uint nInitValue_);

        /// <summary>
        /// GetSystemMetrics的参数
        /// </summary>
        public enum SystemMetricType
        {
            /// <summary>
            /// width of the screen of the primary display monitor, in pixels
            /// </summary>
            CxScreen = 0,
            /// <summary>
            /// height of the screen of the primary display monitor, in pixels
            /// </summary>
            CyScreen = 1,
            /// <summary>
            /// width of a vertical scroll bar in pixels
            /// </summary>
            CxVScroll = 2,
            /// <summary>
            /// height
            /// </summary>
            CyHScroll = 3,
            /// <summary>
            /// Width of a windows border in pixels
            /// </summary>
            CxBorder = 5,
            /// <summary>
            /// Default width of an icon in pixels
            /// </summary>
            CxIcon = 11,
            /// <summary>
            /// Height
            /// </summary>
            CyIcon = 12,
            /// <summary>
            /// Width of a cursor in pixels
            /// </summary>
            CxCursor = 13,
            /// <summary>
            /// Width of the client area for full-screen windows on
            /// the primary monitor, in pixels
            /// </summary>
            CxFullScreen = 16,
            /// <summary>
            /// Height of the client area for full-screen windows on
            /// the primary monitor, in pixels
            /// </summary>
            CyFullScreen = 17,
            /// <summary>
            /// Nonzero if a mouse is installed; otherwise, 0. This value is rarely zero, 
            /// because of support for virtual mice and because some systems detect the presence of the port instead of the presence of a mouse
            /// </summary>
            MousePresent = 19,
            /// <summary>
            /// height of the arrow bitmap on a vertical scroll bar, in pixels
            /// </summary>
            CyVScroll = 20,
            /// <summary>
            /// width of the arrow bitmap on a horizontal scroll bar
            /// in pixels
            /// </summary>
            CxHScroll = 21,
            /// <summary>
            /// Nonzero if the debug version of User.exe is installed; otherwise, 0
            /// </summary>
            Debug = 22,
            /// <summary>
            /// Nonzero if the meanings of the left and right mouse buttons are swapped; otherwise, 0
            /// </summary>
            SwapButton = 23,
            /// <summary>
            /// same as SM_CXSIZEFRAME
            /// </summary>
            CxFrame = 32,
            /// <summary>
            /// same as SM_CYSIZEFRAME
            /// </summary>
            CyFrame = 33,
            /// <summary>
            /// Nonzero if drop-down menus are right-aligned with the corresponding menu-bar item; 
            /// 0 if the menus are left-aligned
            /// </summary>
            MenuDropAlignment = 40,
            /// <summary>
            /// Numbers of buttons on a mouse,
            /// zero if no mouse is installed
            /// </summary>
            MouseButtons = 43,
            /// <summary>
            /// The flags that specify how the system arranged minimized windows:
            /// ARW_BOTTOMLEFT(0): Start at the lower-left corner of the screen. The default position. 
            /// ARW_BOTTOMRIGHT(1): Start at the lower-right corner of the screen. Equivalent to ARW_STARTRIGHT. 
            /// ARW_TOPLEFT(2): Start at the upper-left corner of the screen. Equivalent to ARW_STARTTOP. 
            /// ARW_TOPRIGHT(3): Start at the upper-right corner of the screen. Equivalent to ARW_STARTTOP | SRW_STARTRIGHT. 
            /// </summary>
            Array = 56,
            /// <summary>
            /// width of a minimized window, in pixels
            /// </summary>
            CxMinized = 57,
            /// <summary>
            /// height of a minimized window, in pixels
            /// </summary>
            CyMinized = 58,
            /// <summary>
            /// Default width of a maximized top-level window on the
            /// primary display monitor in pixels
            /// </summary>
            CxMaximized = 61,
            /// <summary>
            /// Default height of a maximized top-level window on the
            /// primary display monitor in pixels
            /// </summary>
            CyMaximized = 62,
            /// <summary>
            /// The least significant bit is set if a network is present; otherwise, it is cleared. 
            /// The other bits are reserved for future use
            /// </summary>
            Network = 63,
            /// <summary>
            /// How system is started:
            /// 0: Normal boot,
            /// 1: Fail-safe boot,
            /// 2: Fail-safe with network boot
            /// </summary>
            CleanBoot = 67,
            /// <summary>
            /// Nonzero if a mouse with a vertical scroll wheel is installed; otherwise 0
            /// </summary>
            MouseWheelPresent = 75,
            /// <summary>
            /// The numbers of display monitor on a desktop
            /// </summary>
            Monitors = 80,
            /// <summary>
            /// Nonzero if Input Method Manager/Input Method Editor features are enabled;
            /// otherwise, 0
            /// </summary>
            ImmEnabled = 82,
            /// <summary>
            /// Nonzero if the current operating system is Windows 7 or Windows Server 2008 R2 
            /// and the Tablet PC Input service is started; otherwise, 0. 
            /// The return value is a bit mask that specifies the type of digitizer input supported by the device.            /// 
            /// </summary>
            Digitizer = 94,
            /// <summary>
            /// This system metric is used in a Terminal Services environment. If the calling process is associated with a Terminal Services client session,
            /// the return value is nonzero. If the calling process is associated with the Terminal Services console session, the return value is 0
            /// </summary>
            RemoteSession = 0x1000,
            /// <summary>
            /// Nonzero if the current session is shutting down; otherwise, 0
            /// </summary>
            ShuttingDown = 0x2000,
            /// <summary>
            /// This system metric is used in a Terminal Services environment to determine if the current Terminal Server session is being remotely controlled. 
            /// Its value is nonzero if the current session is remotely controlled; otherwise, 0
            /// </summary>
            RemoteControl = 0x2001,
        }
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetSystemMetrics(SystemMetricType euType_);

        /// <summary>
        /// 判断鼠标左右键是否切换
        /// </summary>
        /// <returns>切换返回true，否则返回false</returns>
        public static bool IsMouseButtonSwapped()
        {
            return GetSystemMetrics(SystemMetricType.SwapButton) != 0;
        }

        /// <summary>
        /// reverses or restores the meaning of the left and right mouse buttons
        /// </summary>
        /// <param name="bSwap_">Specifies whether the mouse button meanings are reversed or restored. If this parameter is TRUE, the left button generates right-button messages and the right button generates left-button messages. 
        /// If this parameter is FALSE, the buttons are restored to their original meanings</param>
        /// <returns>If the meaning of the mouse buttons was reversed previously, before the function was called, the return value is nonzero.
        /// If the meaning of the mouse buttons was not reversed, the return value is zero
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SwapMouseButton
            (
            [MarshalAs(UnmanagedType.Bool)]
            bool bSwap_
            );
    }
}
