using System;

using System.Runtime.InteropServices;

namespace SHCre.Xugd.WinAPI
{
    /// <summary>
    /// User.dll相关封装
    /// </summary>
    public static partial class XUser
    {
        #region 空闲（鼠标、键盘未活动的）时间
        [StructLayout(LayoutKind.Sequential)]
        private struct XLastInputInfo
        {
            public int cbSize;
            public int nMilliSecs;
        };

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetLastInputInfo(ref XLastInputInfo stInfo_);

        /// <summary>
        /// 获取空闲（鼠标、键盘未活动）的毫秒数（千分之一秒）
        /// </summary>
        /// <returns>空闲的毫秒数</returns>
        public static int GetIdleMilliSecs()
        {
            XLastInputInfo stInfo = new XLastInputInfo();
            stInfo.cbSize = Marshal.SizeOf(stInfo);
            if (!GetLastInputInfo(ref stInfo))
                return 0;

            return Environment.TickCount - stInfo.nMilliSecs;
        }
        #endregion
    }
}
