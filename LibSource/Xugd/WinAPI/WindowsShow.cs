using System;

using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SHCre.Xugd.WinAPI
{
    /// <summary>
    /// Windows窗体显示方式
    /// </summary>
    public enum WinShowMode
    {
        /// <summary>
        /// 隐藏
        /// </summary>
        Hide = 0,
        /// <summary>
        /// 激活窗体，并以原大小在原位置显示
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 以最小化方式显示
        /// </summary>
        Minimize = 2,
        /// <summary>
        /// 以最大化方式显示
        /// </summary>
        Maximize = 3,
        /// <summary>
        /// 激活窗体
        /// </summary>
        Show = 5,
        /// <summary>
        /// 激活并正常显示窗体（无论现在是最大化还是最小化）
        /// </summary>
        Restore = 9
    };

    /// <summary>
    /// 窗体显示的位置
    /// </summary>
    public enum WinShowPosition
    {
        /// <summary>
        /// 移到最顶层
        /// </summary>
        ToTop = 0,
        /// <summary>
        /// 移到最底层
        /// </summary>
        Bottom = 1,
        /// <summary>
        /// 一直处于最顶层
        /// </summary>
        AlwaysTop = -1
    };

    /// <summary>
    /// Win API封装(需要库CreXWin.dll的支持)
    /// </summary>
    public static partial class XWin
    {
        #region Show Windows
        /// <summary>
        /// 把窗体移动到最顶端
        /// </summary>
        /// <param name="hWnd_">窗体句柄</param>
        /// <returns>移动到最顶端，返回true；否则返回false</returns>
        [DllImport("user32.dll", SetLastError=true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BringWindowToTop(IntPtr hWnd_);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos
            (
            IntPtr hWnd_, 
            IntPtr hPosition_,
            int nX_,
            int nY_,
            int nCx_,
            int nCy_,
            uint nFlags_
            );

        /// <summary>
        /// 设定窗体位置时的标志位信息
        /// </summary>
        [Flags]
        public enum WinFlags
        {
            /// <summary>
            /// 保持当前大小不变
            /// </summary>
            NoSize = 0x0001,
            /// <summary>
            /// 保持当前位置不变
            /// </summary>
            NoMove = 0x0002,
            /// <summary>
            /// 保持当前层次位置（Z轴）
            /// </summary>
            NoZorder = 0x0004,
            /// <summary>
            /// 不重绘变化
            /// </summary>
            NoRedraw = 0x0008,
            /// <summary>
            /// 不激活，如果不设，则会激活窗体
            /// </summary>
            NoActivate = 0x0010,
            /// <summary>
            /// 窗体边框变化，发送消息（WM_NCCALCSIZE），
            /// 与DRAWFRAME相同
            /// </summary>
            FrameChanged = 0x0020,
            /// <summary>
            /// 显示窗体
            /// </summary>
            ShowWindow = 0x0040,
            /// <summary>
            /// 隐藏窗体
            /// </summary>
            HideWindow = 0x0080,
            /// <summary>
            /// 不改变层次位置（Z轴），
            /// 与NoReposition相同
            /// </summary>
            NoOwnerZorder = 0x0200,
            /// <summary>
            /// 阻止接收窗体改变消息（WM_WINDOWPOSCHANGING）
            /// </summary>
            NoSendChanging = 0x0400,
            /// <summary>
            /// 阻止同步重绘消息（WM_SYNCPAINT）
            /// </summary>
            DeferErase = 0x2000,
            /// <summary>
            /// 异步发送消息
            /// </summary>
            AsyncWindowPos = 0x4000
        };

        /// <summary>
        /// 设定窗体的位置信息
        /// </summary>
        /// <param name="hWnd_">窗体句柄</param>
        /// <param name="euPosition_">窗体位置（最顶层等）</param>
        /// <param name="nX_">窗体左侧边位置</param>
        /// <param name="nY_">窗体上边位置</param>
        /// <param name="nCx_">窗体的宽度</param>
        /// <param name="nCy_">窗体的高度</param>
        /// <param name="euFlags_">标志信息</param>
        /// <returns>成功，返回true；否则，返回false</returns>
        public static bool SetWinPosition
            (
            IntPtr hWnd_,
            WinShowPosition euPosition_,
            int nX_,
            int nY_,
            int nCx_,
            int nCy_,
            WinFlags euFlags_
            )
        {
            return SetWindowPos(hWnd_, new IntPtr((int)euPosition_), nX_, nY_, nCx_, nCy_, (uint)euFlags_);
        }

        /// <summary>
        /// 设定窗体的位置
        /// </summary>
        /// <param name="hWnd_">窗体句柄</param>
        /// <param name="euPosition_">窗体位置（最顶层等）</param>
        /// <returns>成功，返回true；否则，返回false</returns>
        public static bool SetWinPosition
            (
            IntPtr hWnd_,
            WinShowPosition euPosition_
            )
        {
            return SetWinPosition(hWnd_, euPosition_, 0, 0, 0, 0, WinFlags.ShowWindow | WinFlags.NoSize | WinFlags.NoMove);
        }

        /// <summary>
        /// 显示窗体
        /// </summary>
        /// <param name="hWnd_">窗体句柄</param>
        /// <param name="euShow_">显示方式</param>
        /// <returns>如果原来处于显示状态，返回true；否则返回false</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd_, WinShowMode euShow_);

        /// <summary>
        /// 把窗体显示在最顶端
        /// </summary>
        /// <param name="frmShow_">要显示的窗体</param>
        public static void ShowAtTop(Form frmShow_)
        {
            ShowWindow(frmShow_.Handle, WinShowMode.Restore);
            frmShow_.Activate();
            BringWindowToTop(frmShow_.Handle);
        }
        #endregion        
    };
}
