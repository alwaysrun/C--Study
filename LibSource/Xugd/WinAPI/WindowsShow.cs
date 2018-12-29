using System;

using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SHCre.Xugd.WinAPI
{
    /// <summary>
    /// Windows������ʾ��ʽ
    /// </summary>
    public enum WinShowMode
    {
        /// <summary>
        /// ����
        /// </summary>
        Hide = 0,
        /// <summary>
        /// ����壬����ԭ��С��ԭλ����ʾ
        /// </summary>
        Normal = 1,
        /// <summary>
        /// ����С����ʽ��ʾ
        /// </summary>
        Minimize = 2,
        /// <summary>
        /// ����󻯷�ʽ��ʾ
        /// </summary>
        Maximize = 3,
        /// <summary>
        /// �����
        /// </summary>
        Show = 5,
        /// <summary>
        /// ���������ʾ���壨������������󻯻�����С����
        /// </summary>
        Restore = 9
    };

    /// <summary>
    /// ������ʾ��λ��
    /// </summary>
    public enum WinShowPosition
    {
        /// <summary>
        /// �Ƶ����
        /// </summary>
        ToTop = 0,
        /// <summary>
        /// �Ƶ���ײ�
        /// </summary>
        Bottom = 1,
        /// <summary>
        /// һֱ�������
        /// </summary>
        AlwaysTop = -1
    };

    /// <summary>
    /// Win API��װ(��Ҫ��CreXWin.dll��֧��)
    /// </summary>
    public static partial class XWin
    {
        #region Show Windows
        /// <summary>
        /// �Ѵ����ƶ������
        /// </summary>
        /// <param name="hWnd_">������</param>
        /// <returns>�ƶ�����ˣ�����true�����򷵻�false</returns>
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
        /// �趨����λ��ʱ�ı�־λ��Ϣ
        /// </summary>
        [Flags]
        public enum WinFlags
        {
            /// <summary>
            /// ���ֵ�ǰ��С����
            /// </summary>
            NoSize = 0x0001,
            /// <summary>
            /// ���ֵ�ǰλ�ò���
            /// </summary>
            NoMove = 0x0002,
            /// <summary>
            /// ���ֵ�ǰ���λ�ã�Z�ᣩ
            /// </summary>
            NoZorder = 0x0004,
            /// <summary>
            /// ���ػ�仯
            /// </summary>
            NoRedraw = 0x0008,
            /// <summary>
            /// �����������裬��ἤ���
            /// </summary>
            NoActivate = 0x0010,
            /// <summary>
            /// ����߿�仯��������Ϣ��WM_NCCALCSIZE����
            /// ��DRAWFRAME��ͬ
            /// </summary>
            FrameChanged = 0x0020,
            /// <summary>
            /// ��ʾ����
            /// </summary>
            ShowWindow = 0x0040,
            /// <summary>
            /// ���ش���
            /// </summary>
            HideWindow = 0x0080,
            /// <summary>
            /// ���ı���λ�ã�Z�ᣩ��
            /// ��NoReposition��ͬ
            /// </summary>
            NoOwnerZorder = 0x0200,
            /// <summary>
            /// ��ֹ���մ���ı���Ϣ��WM_WINDOWPOSCHANGING��
            /// </summary>
            NoSendChanging = 0x0400,
            /// <summary>
            /// ��ֹͬ���ػ���Ϣ��WM_SYNCPAINT��
            /// </summary>
            DeferErase = 0x2000,
            /// <summary>
            /// �첽������Ϣ
            /// </summary>
            AsyncWindowPos = 0x4000
        };

        /// <summary>
        /// �趨�����λ����Ϣ
        /// </summary>
        /// <param name="hWnd_">������</param>
        /// <param name="euPosition_">����λ�ã����ȣ�</param>
        /// <param name="nX_">��������λ��</param>
        /// <param name="nY_">�����ϱ�λ��</param>
        /// <param name="nCx_">����Ŀ��</param>
        /// <param name="nCy_">����ĸ߶�</param>
        /// <param name="euFlags_">��־��Ϣ</param>
        /// <returns>�ɹ�������true�����򣬷���false</returns>
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
        /// �趨�����λ��
        /// </summary>
        /// <param name="hWnd_">������</param>
        /// <param name="euPosition_">����λ�ã����ȣ�</param>
        /// <returns>�ɹ�������true�����򣬷���false</returns>
        public static bool SetWinPosition
            (
            IntPtr hWnd_,
            WinShowPosition euPosition_
            )
        {
            return SetWinPosition(hWnd_, euPosition_, 0, 0, 0, 0, WinFlags.ShowWindow | WinFlags.NoSize | WinFlags.NoMove);
        }

        /// <summary>
        /// ��ʾ����
        /// </summary>
        /// <param name="hWnd_">������</param>
        /// <param name="euShow_">��ʾ��ʽ</param>
        /// <returns>���ԭ��������ʾ״̬������true�����򷵻�false</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd_, WinShowMode euShow_);

        /// <summary>
        /// �Ѵ�����ʾ�����
        /// </summary>
        /// <param name="frmShow_">Ҫ��ʾ�Ĵ���</param>
        public static void ShowAtTop(Form frmShow_)
        {
            ShowWindow(frmShow_.Handle, WinShowMode.Restore);
            frmShow_.Activate();
            BringWindowToTop(frmShow_.Handle);
        }
        #endregion        
    };
}
