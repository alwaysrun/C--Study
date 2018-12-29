using System;

using System.Runtime.InteropServices;

namespace SHCre.Xugd.WinAPI
{
    /// <summary>
    /// User.dll��ط�װ
    /// </summary>
    public static partial class XUser
    {
        #region ���У���ꡢ����δ��ģ�ʱ��
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
        /// ��ȡ���У���ꡢ����δ����ĺ�������ǧ��֮һ�룩
        /// </summary>
        /// <returns>���еĺ�����</returns>
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
