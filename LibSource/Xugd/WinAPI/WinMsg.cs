using System;

using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SHCre.Xugd.WinAPI
{
    partial class XUser
    {
        /// <summary>
        /// 同步给窗体发送消息
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, int lParam);

        /// <summary>
        /// 异步给窗体发送消息
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="Msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, uint wParam, int lParam);

        /// <summary>
        /// SysCommand消息时的wParam
        /// </summary>
        public enum SysCommandParam : uint
        {
            /// <summary>
            /// Indicates whether the screen saver is secure
            /// </summary>
            IsSecure = 0x0001,
            /// <summary>
            /// Sizes the window
            /// </summary>
            Size = 0xF000,
            /// <summary>
            /// Moves the window
            /// </summary>
            Move = 0xF010,
            /// <summary>
            /// Minimizes the window
            /// </summary>
            Minimize = 0xF020,
            /// <summary>
            /// Maximizes the window
            /// </summary>
            Maximize = 0xF030,
            /// <summary>
            /// Moves to the next window
            /// </summary>
            NextWindow = 0xF040,
            /// <summary>
            /// Moves to the previous window
            /// </summary>
            PrevWindow = 0xF050,
            /// <summary>
            /// Closes the window
            /// </summary>
            Close = 0xF060,
            /// <summary>
            /// Scrolls vertically
            /// </summary>
            VScroll = 0xF070,
            /// <summary>
            /// Scrolls horizontally
            /// </summary>
            HScroll = 0xF080,
            /// <summary>
            /// Retrieves the window menu as a result of a mouse click
            /// </summary>
            MouseMenu = 0xF090,
            /// <summary>
            /// Retrieves the window menu as a result of a keystroke. For more information, see the Remarks section
            /// </summary>
            KeyMenu = 0xF100,
            /// <summary>
            /// Restores the window to its normal position and size
            /// </summary>
            Restore = 0xF120,
            /// <summary>
            /// Activates the Start menu
            /// </summary>
            TaskList = 0xF130,
            /// <summary>
            /// Executes the screen saver application specified in the [boot] section of the System.ini file
            /// </summary>
            ScreenSave = 0xF140,
            /// <summary>
            /// Activates the window associated with the application-specified hot key. The lParam parameter identifies the window to activate
            /// </summary>
            HotKey = 0xF150,
            /// <summary>
            /// Selects the default item; the user double-clicked the window menu
            /// </summary>
            Default = 0xF160,
            /// <summary>
            /// Sets the state of the display. This command supports devices that have power-saving features, such as a battery-powered personal computer.
            /// The lParam parameter can have the following values:
            /// -1 (the display is powering on),
            /// 1 (the display is going to low power),
            /// 2 (the display is being shut off)
            /// </summary>
            MonitorPower = 0xF170,
            /// <summary>
            /// Changes the cursor to a question mark with a pointer. If the user then clicks a control in the dialog box, the control receives a WM_HELP message
            /// </summary>
            ContextHelp = 0xF180,
        };

        /// <summary>
        /// 广播消息的窗体
        /// </summary>
        public static readonly IntPtr BroadCastMsgWnd = new IntPtr(0xFFFF);
        const uint MsgSysCommand = 0x0112;

        /// <summary>
        /// 同步发送WM_SYSCOMMAND的消息
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="euWParam_"></param>
        /// <param name="lParam_"></param>
        public static void SendSysCommandMsg(IntPtr hWnd, SysCommandParam euWParam_, int lParam_)
        {
            SendMessage(hWnd, MsgSysCommand, (uint)euWParam_, lParam_);
        }

        /// <summary>
        /// 异步发送WM_SYSCOMMAND的消息
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="euWParam_"></param>
        /// <param name="lParam_"></param>
        public static void PostSysCommandMsg(IntPtr hWnd, SysCommandParam euWParam_, int lParam_)
        {
            PostMessage(hWnd, MsgSysCommand, (uint)euWParam_, lParam_);
        }
    } // XUser
}
