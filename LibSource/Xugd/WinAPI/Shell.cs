using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

using SHCre.Xugd.Common;

namespace SHCre.Xugd.WinAPI
{
    /// <summary>
    /// WinShell API��װ
    /// </summary>
    public static class XShell
    {
        /// <summary>
        /// �ļ����Ա�־λ
        /// </summary>
        [Flags]
        public enum FileAttributes
        {
            /// <summary>
            /// û���κ�����
            /// </summary>
            None = 0,
            /// <summary>
            /// �浵�ļ�
            /// </summary>
            Archive = 0x00000020,
            /// <summary>
            /// Ϊһ���豸
            /// </summary>
            Device = 0x00000040,
            /// <summary>
            /// ���ļ���
            /// </summary>
            Directory = 0x00000010,
            /// <summary>
            /// �����ļ�
            /// </summary>
            Encrypted = 0x00004000,
            /// <summary>
            /// �����ļ�
            /// </summary>
            Hidden = 0x00000002,
            /// <summary>
            /// ��ͨ�ļ���û��������־
            /// </summary>
            Normal = 0x00000080,
            /// <summary>
            /// ֻ���ļ�
            /// </summary>
            ReadOnly = 0x00000001,
            /// <summary>
            /// ϵͳ�ļ�
            /// </summary>
            System = 0x00000004,
            /// <summary>
            /// ��ʱ�ļ�
            /// </summary>
            Tempory = 0x00000100
        };

        #region ��ȡ�ļ�/�ļ�����Ϣ��ͼ��

        /// <summary>
        /// ��ʶҪ��ȡ����Ϣ�ı�־λ
        /// </summary>
        [Flags]
        public enum SHFileFlag
        {
            /// <summary>
            /// ��ȡ�ļ����������
            /// </summary>
            Attributes = 0x000000800,
            /// <summary>
            /// ��ȡ������ʾ���ļ�����
            /// </summary>
            DisplayName = 0x000000200,
            /// <summary>
            /// ���ؿ�ִ���ļ������ͣ�������������־λͬʱ�趨
            /// </summary>
            ExeType = 0x000002000,
            /// <summary>
            /// ��ȡͼ������ϵͳͼ������
            /// </summary>
            Icon = 0x000000100,
            /// <summary>
            /// ����ͼ����ļ�����ͼ�����ļ��е�����
            /// </summary>
            IconLocation = 0x000001000,
            /// <summary>
            /// ��ȡ��ͼ�꣬Icon��־λ����ͬʱ�趨
            /// </summary>
            LargeIcon = 0x000000000,
            /// <summary>
            /// ��ͼ����������ӱ�ʶ��Icon��־λ����ͬʱ�趨
            /// </summary>
            LinkOverlay = 0x000008000,
            /// <summary>
            /// ��ȡ�ļ��Ĵ�ͼ�꣬Icon��SysIcon��־λ����ͬʱ�趨
            /// </summary>
            OpenIcon = 0x000000002,
            /// <summary>
            /// ˵��strPath��һ��ITEMIDLIST��ַ
            /// </summary>
            Pidl = 0x000000008,
            /// <summary>
            /// ��ȡѡ����ʽͼ�꣨��ϵͳ����ɫ��ϣ���Icon��־λ����ͬʱ�趨
            /// </summary>
            Selected = 0x000010000,
            /// <summary>
            /// ��ȡShell-size��С��ͼ�꣬Icon��־λ����ͬʱ�趨
            /// </summary>
            ShellIconSize = 0x000000004,
            /// <summary>
            /// ��ȡСͼ�꣬Icon��־λ����ͬʱ�趨
            /// </summary>
            SmallIcon = 0x000000001,
            /// <summary>
            /// ��ȡͼ����ϵͳͼ���б��е�����
            /// </summary>
            SysIconIndex = 0x000004000,
            /// <summary>
            /// ��ȡ��������������Ϣ
            /// </summary>
            TypeName = 0x000000400,
            /// <summary>
            /// ����ͼȥ���ļ����ļ����Բ����ڣ���������Attributes,ExeType,PDIL��־����
            /// </summary>
            UseAttributes = 0x000000010
        };

        /// <summary>
        /// ����ļ���Ϣ�Ľṹ��
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHFileInfo
        {
            /// <summary>
            /// ��ȡ����ICONͼ��ľ������Ҫ�����߸����ͷ�(DestroyIcon)
            /// </summary>
            public IntPtr hIcon;
            /// <summary>
            /// ��ȡ����ICONͼ����ϵͳͼ���б��е�����
            /// </summary>
            public int nIcon;
            /// <summary>
            /// �ļ����������(��IShellFolder::GetAttributesOf������ͬ���� SFGAO_FOLDER)
            /// </summary>
            public uint nAttributes;
            /// <summary>
            /// ������ʾ���ļ���
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            /// <summary>
            /// �ļ���������������Ϣ
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        /// <summary>
        /// ��ȡ�ļ�ϵͳ��һ���������Ϣ��������صĶ����а���ͼ��ľ��������Ҫʹ��DestroyIcon���ͷ�
        /// </summary>
        /// <param name="strPath">�ļ�·��������趨PIDL��־��ΪITEMIDLIST��ַ��
        ///     ����趨UserAttributes�ļ����Բ�����</param>
        /// <param name="euAttributes">�ļ������ԣ�ֻ��UserAttributes�趨ʱ����Ч</param>
        /// <param name="stFileInfo">��Ŷ�����Ϣ�ṹ��</param>
        /// <param name="nFileInfoSize">�ṹ��Ĵ�С</param>
        /// <param name="euFlags">��ȡ��Ϣ�ı�־</param>
        /// <returns>�����־�в�����ExeType,SysIconIndex���ɹ����ط��㣻ʧ�ܷ����㡣</returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr SHGetFileInfo
        (
            [MarshalAs(UnmanagedType.LPWStr)]
            string strPath,
            FileAttributes euAttributes,
            ref SHFileInfo stFileInfo,
            uint nFileInfoSize,
            SHFileFlag euFlags
       );

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr SHGetFileInfo
        (
            IntPtr pIdl,
            FileAttributes euAttributes,
            ref SHFileInfo stFileInfo,
            uint nFileInfoSize,
            SHFileFlag euFlags
        );

        /// <summary>
        /// ��ȡ�ļ�ϵͳ��һ���������Ϣ��������صĶ����а���ͼ��ľ��������Ҫʹ��DestroyIcon���ͷš�
        /// ʧ�ܻ��׳�Win32Exception�쳣
        /// </summary>
        /// <param name="strPath_">�ļ�·��������趨UserAttributes�ļ����Բ�����</param>
        /// <param name="euAttri_">�ļ������ԣ�ֻ��UserAttributes�趨ʱ����Ч</param>
        /// <param name="euFlags_">��ȡ��Ϣ�ı�־</param>
        /// <returns>�ļ�������Ϣ</returns>
        public static SHFileInfo SHGetFileInfo(string strPath_, FileAttributes euAttri_, SHFileFlag euFlags_)
        {
            SHFileInfo stInfo = new SHFileInfo();
            if (IntPtr.Zero == SHGetFileInfo(strPath_, euAttri_, ref stInfo, (uint)Marshal.SizeOf(stInfo), euFlags_))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "WinAPI-=-SHGetFileInfo(): failed");

            return stInfo;
        }

        /// <summary>
        /// ��ȡ�ļ�ϵͳ��һ���������Ϣ��������صĶ����а���ͼ��ľ��������Ҫʹ��DestroyIcon���ͷš�
        /// ʧ�ܻ��׳�Win32Exception�쳣
        /// </summary>
        /// <param name="pIdl_">ITEMIDLIST��ַ(��Ҫ�趨PIDL��־)</param>
        /// <param name="euAttri_">�ļ�������</param>
        /// <param name="euFlags_">��ȡ��Ϣ�ı�־</param>
        /// <returns>�ļ�������Ϣ</returns>
        public static SHFileInfo SHGetFileInfo(IntPtr pIdl_, FileAttributes euAttri_, SHFileFlag euFlags_)
        {
            SHFileInfo stInfo = new SHFileInfo();
            if (IntPtr.Zero == SHGetFileInfo(pIdl_, euAttri_, ref stInfo, (uint)Marshal.SizeOf(stInfo), euFlags_ | SHFileFlag.Pidl))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "WinAPI-=-SHGetFileInfo(): failed");

            return stInfo;
        }

        /// <summary>
        /// �ͷ�ͼ��ľ��
        /// </summary>
        /// <param name="hIcon_">ͼ��ľ��</param>
        /// <returns>�ɹ�������true�����򣬷���false</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr hIcon_);

        /// <summary>
        /// ��ָ���Ŀ�ִ�е��ļ��л�ȡ��ͼ���Сͼ������飬���е�ͼ�궼��ͨ��DestroyIcon���ͷţ�
        /// ���nIconIndexΪ-1��phiconLarge��phiconSmallΪIntPtr.zero�򷵻��ļ��а�����ͼ������
        /// </summary>
        /// <param name="strFile">��ִ���ļ�·��</param>
        /// <param name="nIconIndex">ͼ�����������0��ʼ������</param>
        /// <param name="phiconLarge">��ͼ������</param>
        /// <param name="phiconSmall">Сͼ������</param>
        /// <param name="nIcons">Ҫ��ȡͼ�������</param>
        /// <returns>��ȡ��ͼ������</returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern uint ExtractIconEx(
                [MarshalAs(UnmanagedType.LPWStr)]
                string strFile,
                int nIconIndex,
                IntPtr[] phiconLarge,
                IntPtr[] phiconSmall,
                uint nIcons);

        /// <summary>
        /// �����ļ�������ȡ�ļ���ͼ�������������Ϣ���ļ�������ڣ���
        /// ʧ�ܻ��׳�Win32Exception�쳣
        /// </summary>
        /// <param name="strFullName_">�ļ���ȫ��</param>
        /// <param name="euAttri_">�ļ����ԣ�Flag���趨��UserAttributesʱ��Ч��</param>
        /// <param name="euFlags_">��ȡ��Ϣ�ı�־</param>
        /// <param name="strTypeInfo_">�����ļ�����������������Ϣ</param>
        /// <returns>��ȡ��ICONͼ��</returns>
        public static System.Drawing.Icon GetFileInfo(string strFullName_, FileAttributes euAttri_, SHFileFlag euFlags_, out string strTypeInfo_)
        {
            SHFileInfo stInfo = SHGetFileInfo(strFullName_, euAttri_, euFlags_);
            System.Drawing.Icon hIcon = System.Drawing.Icon.FromHandle(stInfo.hIcon);
            strTypeInfo_ = stInfo.szTypeName;
            return hIcon;
        }

        /// <summary>
        /// �����ļ�������ȡ�ļ���ͼ�����������������Ϣ���ļ�������ڣ���
        /// ʧ�ܻ��׳�Win32Exception�쳣
        /// </summary>
        /// <param name="strFullName_">�ļ���ȫ��</param>
        /// <param name="euFlags_">��ȡ��Ϣ�ı�־</param>
        /// <param name="strTypeInfo_">�����ļ�����������������Ϣ</param>
        /// <returns>��ȡ��ICONͼ��</returns>
        public static System.Drawing.Icon GetFileInfo(string strFullName_, SHFileFlag euFlags_, out string strTypeInfo_)
        {
            return GetFileInfo(strFullName_, FileAttributes.Normal, euFlags_, out strTypeInfo_);
        }

        /// <summary>
        /// ��ȡ�ļ���ͼ��(Сͼ��)����������������Ϣ��
        /// ʧ�ܻ��׳�Win32Exception�쳣
        /// </summary>
        /// <param name="strFullName_">�ļ���ȫ��</param>
        /// <param name="strTypeInfo_">�����ļ�����������������Ϣ</param>
        /// <returns>��ȡ��ICONͼ��</returns>
        public static System.Drawing.Icon GetFileInfo(string strFullName_, out string strTypeInfo_)
        {
            return GetFileInfo(strFullName_, FileAttributes.Normal,
                SHFileFlag.Icon | SHFileFlag.SmallIcon | SHFileFlag.TypeName, out strTypeInfo_);
        }

        /// <summary>
        /// �����ļ�������չ������ȡ�ļ���ͼ��(Сͼ��)����������������Ϣ���ļ�����Ҫ���ڡ�
        /// ʧ�ܻ��׳�Win32Exception�쳣
        /// </summary>
        /// <param name="strFileName_">�ļ���</param>
        /// <param name="strTypeInfo_">�����ļ�����������������Ϣ</param>
        /// <returns>��ȡ��ICONͼ��</returns>
        public static System.Drawing.Icon GetFileInfoByExt(string strFileName_, out string strTypeInfo_)
        {
            return GetFileInfo(strFileName_, FileAttributes.Normal,
                SHFileFlag.Icon | SHFileFlag.SmallIcon | SHFileFlag.TypeName | SHFileFlag.UseAttributes, out strTypeInfo_);
        }

        /// <summary>
        /// ��ȡ�ļ��е�ͼ��(Сͼ��)������������Ϣ��
        /// ʧ�ܻ��׳�Win32Exception�쳣
        /// </summary>
        /// <param name="strTypeInfo_">�ļ��е�����������Ϣ</param>
        /// <returns>��ȡ��ICONͼ��</returns>
        public static System.Drawing.Icon GetDirInfo(out string strTypeInfo_)
        {
            // Use SystemDirectory (As at win7, if the file not existed, can not get the icon)
            return GetFileInfo(Environment.SystemDirectory, FileAttributes.Directory,
                SHFileFlag.Icon | SHFileFlag.SmallIcon | SHFileFlag.TypeName | SHFileFlag.UseAttributes, out strTypeInfo_);
        }

        /// <summary>
        /// ��ȡ�ļ��е�ͼ��(Сͼ��)������������Ϣ��
        /// ʧ�ܻ��׳�Win32Exception�쳣
        /// </summary>
        /// <param name="chDrive_">Ҫ��ȡ��Ϣ���̷�</param>
        /// <param name="strTypeInfo_">�̷�������������Ϣ</param>
        /// <returns>��ȡ��ICONͼ��</returns>
        public static System.Drawing.Icon GetDriveInfo(char chDrive_, out string strTypeInfo_)
        {
            return GetFileInfo(string.Format("{0}:\\", chDrive_), FileAttributes.None,
                SHFileFlag.Icon | SHFileFlag.SmallIcon | SHFileFlag.TypeName, out strTypeInfo_);
        }

        /// <summary>
        /// ��ȡ�ļ��е�ͼ��(Сͼ��)������������Ϣ��
        /// ʧ�ܻ��׳�Win32Exception�쳣
        /// </summary>
        /// <param name="chDrive_">Ҫ��ȡ��Ϣ���̷�</param>
        /// <param name="strTypeInfo_">�̷�������������Ϣ</param>
        /// <param name="strDisplayName_">�̷�����ʾ��Ϣ�����ƣ�</param>
        /// <returns>��ȡ��ICONͼ��</returns>
        public static System.Drawing.Icon GetDriveInfo(char chDrive_, out string strTypeInfo_, out string strDisplayName_)
        {
            SHFileInfo stInfo = SHGetFileInfo(string.Format("{0}:\\", chDrive_), FileAttributes.None,
                        SHFileFlag.Icon | SHFileFlag.SmallIcon | SHFileFlag.TypeName | SHFileFlag.DisplayName);
            System.Drawing.Icon hIcon = System.Drawing.Icon.FromHandle(stInfo.hIcon);
            strTypeInfo_ = stInfo.szTypeName;
            strDisplayName_ = stInfo.szDisplayName;
            return hIcon;
        }

        #region Special Folder info
        static readonly int[] _FolderIdl = new int[] 
                {
                    0x011,
                    0x012,
                    0x0A,
                    0x01,
                    0x03,
                    0x04,
                    0x00,
                    0x06,
                    0x02,
                    0x08,
                    0x035,
                    0x036,
                    0x037
                };
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int SHGetSpecialFolderLocation
        (
            IntPtr hOwner,
            int nFolder,
            out IntPtr pIdl
        );

        /// <summary>
        /// �����ļ���
        /// </summary>
        public enum SpecialFolder
        {
            /// <summary>
            /// �ҵĵ���
            /// </summary>
            Computer = 0,
            /// <summary>
            /// �����ھ�
            /// </summary>
            Network,
            /// <summary>
            /// ����վ
            /// </summary>
            Recycler,
            /// <summary>
            /// �����IE
            /// </summary>
            Internet,
            /// <summary>
            /// �������
            /// </summary>
            ControlPanel,
            /// <summary>
            /// ��ӡ��
            /// </summary>
            Printer,
            /// <summary>
            /// ���棨���������е�����ͼ�꣩
            /// </summary>
            Desktop,
            /// <summary>
            /// �ղؼУ�IE��ͼ��
            /// </summary>
            Favorite,
            /// <summary>
            /// ���򣨿�ʼ�˵��еĳ���ͼ�꣩
            /// </summary>
            Programs,
            /// <summary>
            /// ���ʹ���ĵ�����ʼ�˵��е��ĵ�ͼ�꣩
            /// </summary>
            Recent,
            /// <summary>
            /// ����
            /// </summary>
            Music,
            /// <summary>
            /// ͼƬ
            /// </summary>
            Pcture,
            /// <summary>
            /// ��Ƶ
            /// </summary>
            Video,
            /// <summary>
            /// ���ش���
            /// </summary>
            Disk
        };

        /// <summary>
        /// ��ȡ�����ļ��е�ͼ������Ϣ
        /// </summary>
        /// <param name="euFolder_">�����ļ��б�ʶ</param>
        /// <param name="strInfo_">˵����Ϣ</param>
        /// <returns>ͼ����</returns>
        public static System.Drawing.Icon GetSpecialFolderInfo(SpecialFolder euFolder_, out string strInfo_)
        {
            if (euFolder_ == SpecialFolder.Disk)
            {
                return GetDriveInfo(Environment.SystemDirectory[0], out strInfo_);
            }

            // Get special folder's idl
            IntPtr pIdl;
            SHGetSpecialFolderLocation(IntPtr.Zero, _FolderIdl[(int)euFolder_], out pIdl);

            SHFileInfo stInfo = SHGetFileInfo(pIdl, FileAttributes.None,
                        SHFileFlag.Icon | SHFileFlag.SmallIcon | SHFileFlag.DisplayName);
            System.Drawing.Icon hIcon = System.Drawing.Icon.FromHandle(stInfo.hIcon);
            strInfo_ = stInfo.szDisplayName;
            return hIcon;
        }
        #endregion

        #endregion


        #region �ļ�Shell����
        /// <summary>
        /// ShellExcuteExִ��ʱ״̬����(�򿪡��򿪷�ʽ������)
        /// </summary>
        public enum SHExecMask
        {
            /// <summary>
            /// ��ԱlpClass��Ч
            /// </summary>
            ClassName = 0x00000001,
            /// <summary>
            /// ��ԱhKeyClass��Ч
            /// </summary>
            ClassKey = 0x00000003,
            /// <summary>
            /// ��ԱlpIDList��Ч
            /// </summary>
            IDList = 0x00000004,
            /// <summary>
            /// ����ͨ����ݲ˵���չ�˼������
            /// </summary>
            InvokeIDList = 0x0000000C,
            /// <summary>
            /// ��ԱdwHotKey��Ч
            /// </summary>
            HotKey = 0x00000020,
            /// <summary>
            /// ˵��hProcess����������
            /// </summary>
            NoCloseEProcess = 0x00000040,
            /// <summary>
            /// Validate the share and connect to a drive letter
            /// </summary>
            ConnectNetdrv = 0x00000080,
            /// <summary>
            /// ����ǰ�ȴ�DDEת�����
            /// </summary>
            FlagDdeWait = 0x00000100,
            /// <summary>
            /// ��չlpDirectory��lpFile�еĻ�������
            /// </summary>
            DoEnvsubst = 0x00000200,
            /// <summary>
            /// ����ʱ����Ҫ��ʾ������Ϣ��
            /// </summary>
            FlagNoUI = 0x00000400,
            /// <summary>
            /// ˵���ǿ��ַ�����
            /// </summary>
            Unicode = 0x00004000,
            /// <summary>
            /// �����µ�Console���������÷ǽ���
            /// </summary>
            NoConsole = 0x00008000,
            /// <summary>
            /// ��ԱhMonitor����hIcon����ͬһλ�ã���Ч
            /// </summary>
            Monitor = 0x00200000,
            /// <summary>
            /// ��ִ��Zone���
            /// </summary>
            NoZoneChecks = 0x00800000,
            /// <summary>
            /// �������ٳ����ʹ��ʱ��
            /// </summary>
            FlagLogUsage = 0x04000000
        };
        /// <summary>
        /// ���ڰ����ļ�Shell����������Ϣ�Ľṹ��
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHExecInfo
        {
            /// <summary>
            /// �˽ṹ��Ĵ�С�����Լ����㣩
            /// </summary>
            public int cbSize;
            /// <summary>
            /// Ӱ�����ݺ�������Ա�ı�־λ
            /// </summary>
            public SHExecMask euMask;
            /// <summary>
            /// ִ��ʱϵͳ�������Ϣ������
            /// </summary>
            public IntPtr hWnd;
            /// <summary>
            /// ��ʾҪִ�еĶ������ı�
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpVerb;
            /// <summary>
            /// Ҫ�������ļ�
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpFile;
            /// <summary>
            /// �����б�
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpParameters;
            /// <summary>
            /// ����Ŀ¼�����û��ָ����ʹ�õ�ǰĿ¼
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpDirectory;
            /// <summary>
            /// ��ʾ��������ʱ�����ʾ
            /// </summary>
            public WinShowMode euShow;
            /// <summary>
            /// ����ɹ������ش���32�����������������ΪSE_ERR_XXXֵ
            /// </summary>
            public int hRetCode;
            /// <summary>
            /// ��ʶ�ļ���ID�б�ֻ��fMask����SEE_MASK_IDLIST��SEE_MASK_INVOKEIDLISTʱ��Ч��
            /// </summary>
            public int lpIDList;
            /// <summary>
            /// ��ʾ�ļ�������ȫ��Ψһ��ʶ��ֻ��fMask����SEE_MASK_CLASSKEYʱ��Ч��
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpClass;
            /// <summary>
            /// ע������ļ�������ֻ��fMask����SEE_MASK_CLASSKEYʱ��Ч��
            /// </summary>
            public IntPtr hkeyClass;
            /// <summary>
            /// ��������ȼ���ֻ��fMask����SEE_MASK_HOTKEYʱ��Ч��
            /// </summary>
            public uint dwHotKey;
            /// <summary>
            /// �����ļ���ͼ��ľ����ֻ��fMask����SEE_MASK_ICONʱ��Ч��
            /// </summary>
            public IntPtr hIcon;
            /// <summary>
            /// ����������ľ����ֻ��fMask����SEE_MASK_NOCLOSEPROCESSʱ��Ч��
            /// </summary>
            public IntPtr hProcess;
        };

        /// <summary>
        /// ִ���ļ���ز���
        /// </summary>
        /// <param name="stExecInfo_">������Ϣ</param>
        /// <returns>�ɹ���true��ʧ�ܣ�false</returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShellExecuteEx(ref SHExecInfo stExecInfo_);

        /// <summary>
        /// ִ��һЩ�ļ���صĲ���
        /// </summary>
        /// <param name="hWnd_">��������ĸ����壬û�пɴ���IntPtr.Zero</param>
        /// <param name="strOperate_">�����ַ�����û�д���null</param>
        /// <param name="strFile_">�������ļ�</param>
        /// <param name="strParams_">��������</param>
        /// <param name="strDir_">���·��</param>
        /// <param name="euShow_">��������ĳ�ʼ״̬</param>
        /// <returns>��ת��Ϊint32�ľ��������32���ɹ�������ο�MSDN�е�Return Value˵��</returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr ShellExecute
            (
            IntPtr hWnd_,
            string strOperate_,
            string strFile_,
            string strParams_,
            string strDir_,
            WinShowMode euShow_
            );

        /// <summary>
        /// �ļ��Ĳ�������
        /// </summary>
        public enum SHExecAction
        {
            /// <summary>
            /// ���ļ�
            /// </summary>
            Open,
            /// <summary>
            /// �����ļ��Ĵ򿪷�ʽ�Ի���
            /// </summary>
            OpenAs,
            /// <summary>
            /// �����ļ������ԶԻ���
            /// </summary>
            Property
        };

        /// <summary>
        /// ִ���ļ���ز���
        /// </summary>
        /// <param name="strFile_">Ҫִ�в������ļ�</param>
        /// <param name="euAction_">����Ҫ���еĲ���</param>
        public static void ShellExec(string strFile_, SHExecAction euAction_)
        {
            if( euAction_ == SHExecAction.OpenAs )
            {
                string strParam = string.Format("shell32.dll, OpenAs_RunDLL \"{0}\"", strFile_);
                IntPtr ptrRet = ShellExecute(IntPtr.Zero, null, "rundll32.exe", strParam, string.Empty, WinShowMode.Normal);
                int nRet = ptrRet.ToInt32();
                if( nRet < 32 )
                {
                    throw new Win32Exception(nRet, "WinAPI-=-ShellExec(): call ShellExecute failed(Look Return Value Tips of ShellExecute for reason)");
                }
                return;
            }

            // Other Actions
            SHExecInfo stInfo = new SHExecInfo();
            stInfo.cbSize = Marshal.SizeOf(stInfo);
            stInfo.lpFile = strFile_;
            stInfo.euMask = SHExecMask.Unicode;
            stInfo.euShow = WinShowMode.Normal;
            switch (euAction_)
            {
                case SHExecAction.Open:
                    stInfo.lpVerb = "open";
                    break;

                case SHExecAction.Property:
                    stInfo.lpVerb = "properties";
                    stInfo.euMask = (SHExecMask)XFlag.Add((uint)stInfo.euMask, (uint)SHExecMask.InvokeIDList);
                    break;

                default:
                    throw new ArgumentException("Invalid Action type");
            }

            if (!ShellExecuteEx(ref stInfo))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "WinAPI-=-ShellExec(): call ShellExecuteEx failed");
        }
        #endregion

        #region �ļ��������ƶ������ơ�ɾ������������
        /// <summary>
        /// ����˵��
        /// </summary>
        public enum SHFileOPFunc
        {
            /// <summary>
            /// �ƶ�
            /// </summary>
            Move = 0x0001,
            /// <summary>
            /// ����
            /// </summary>
            Copy = 0x0002,
            /// <summary>
            /// ɾ��
            /// </summary>
            Delete = 0x0003,
            /// <summary>
            /// ������
            /// </summary>
            Rename = 0x0004
        };

        /// <summary>
        /// �ļ�������־
        /// </summary>
        [Flags]
        public enum SHFileOPFlag : ushort
        {
            /// <summary>
            /// �գ���Ч��
            /// </summary>
            None = 0,
            /// <summary>
            /// pTo��ָ�����Ŀ���ļ�
            /// </summary>
            MultiDestFiles = 0x0001,
            /// <summary>
            /// ����ʾ���ȶԻ���
            /// </summary>
            Silent = 0x0004,
            /// <summary>
            /// ���ͬ���ļ��Ѵ��ڣ��������ļ�
            /// </summary>
            RenameOnCollision = 0x0008,
            /// <summary>
            /// ������ѯ�ʲ���ִ�С�Yes to All��
            /// </summary>
            NoConfirmation = 0x0010,
            /// <summary>
            /// ������������ļ����ڷ���һ�����ļ������ļ�����ӳ�����
            /// </summary>
            WantMappingHandle = 0x0020,
            /// <summary>
            /// ��������ǰ����
            /// </summary>
            AllowUndo = 0x0040,
            /// <summary>
            /// �����ͨ�����*.*��ʱ��ֻƥ���ļ�
            /// </summary>
            FilesOnly = 0x0080,
            /// <summary>
            /// �������ϲ���ʾ�ļ���
            /// </summary>
            SimplEProgress = 0x0100,
            /// <summary>
            /// �����Ҫ�����ļ��У�����Ҫȷ��ֱ�Ӵ���
            /// </summary>
            NoConfirmMkdir = 0x0200,
            /// <summary>
            /// ����ʱ������ʾUI
            /// </summary>
            NoErrorUI = 0x0400,
            /// <summary>
            /// �������ļ���ȫ����
            /// </summary>
            NoCopySecurityAttribs = 0x0800,
            /// <summary>
            /// ֻ����ǰ�ļ��У����ݹ鴦�����ļ���
            /// </summary>
            NoRecursion = 0x1000,
            /// <summary>
            /// Do not move connected files as a group. Only move the specified files.
            /// </summary>
            NoConnectedElements = 0x2000,
            /// <summary>
            /// ɾ���ļ�ʱ����������
            /// </summary>
            WantNukeWarning = 0x4000,
            /// <summary>
            /// û��UI
            /// </summary>
            NoUI = Silent | NoConfirmation | NoErrorUI | NoConfirmMkdir
        };

        /// <summary>
        /// �ļ�������Ϣ�ṹ��
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHFileOPInfo
        {
            /// <summary>
            /// ��ʾ��Ϣ�Ĵ�����
            /// </summary>
            public IntPtr hWnd;
            /// <summary>
            /// ����Ĳ���
            /// </summary>
            public SHFileOPFunc euFunc;
            /// <summary>
            /// ԭ�ļ���������'\0'��Ϊ����������ļ�ʱ���м���һ��'\0'��Ϊ�����
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pFrom;
            /// <summary>
            /// Ŀ���ļ���������'\0'��Ϊ����������ļ�ʱ���м���һ��'\0'��Ϊ�����
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pTo;
            /// <summary>
            /// ������־
            /// </summary>
            public SHFileOPFlag euFlag;
            /// <summary>
            /// ������κ�Abort����������true������false
            /// </summary>
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            /// <summary>
            /// ӳ��������
            /// </summary>
            public IntPtr hNameMappings;
            /// <summary>
            /// �趨SimplEProgressʱ����ʾ�ı���
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszProgressTitle;
        };

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        private static extern int SHFileOperation(ref SHFileOPInfo stInfo_);

        /// <summary>
        /// �ļ�����
        /// </summary>
        /// <param name="stInfo_">������Ϣ�ṹ��</param>
        public static void FileOperation(ref SHFileOPInfo stInfo_)
        {
            if (0 != SHFileOperation(ref stInfo_))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "WinAPI-=-SHFileOperation(): failed");
        }

        /// <summary>
        /// �ļ�����
        /// </summary>
        /// <param name="strFroms_">Դ�ļ�</param>
        /// <param name="strTos_">Ŀ���ļ�</param>
        /// <param name="euFunc_">�������</param>
        /// <param name="euFlag_">������־</param>
        /// <returns>�û�ȡ������������true������false</returns>
        public static bool FileOperation(string strFroms_, string strTos_, SHFileOPFunc euFunc_, SHFileOPFlag euFlag_)
        {
            SHFileOPInfo stInfo = new SHFileOPInfo();
            stInfo.hWnd = IntPtr.Zero;
            stInfo.pFrom = strFroms_ + '\0';
            stInfo.pTo = strTos_ + '\0';
            stInfo.euFunc = euFunc_;
            stInfo.euFlag = euFlag_;
            FileOperation(ref stInfo);
            return stInfo.fAnyOperationsAborted;
        }

        /// <summary>
        /// �ļ�����
        /// </summary>
        /// <param name="strFroms_">Դ�ļ�</param>
        /// <param name="strTos_">Ŀ���ļ�</param>
        /// <param name="euFunc_">�������</param>
        /// <returns>�û�ȡ������������true������false</returns>
        public static bool FileOperation(string strFroms_, string strTos_, SHFileOPFunc euFunc_)
        {
            return FileOperation(strFroms_, strTos_, euFunc_, SHFileOPFlag.None);
        }

        /// <summary>
        /// ������Ķ���ļ��ַ�����װ�ɿ��Ա�FileOperationʹ�õĵ����ַ���
        /// </summary>
        /// <param name="strFiles_">�ļ���</param>
        /// <returns>����װ�õ��ַ�����������'\0'��β</returns>
        public static string BuildFiles(params string[] strFiles_)
        {
            if (null == strFiles_)
                return "\0";

            const int FileSize = 256;
            StringBuilder sbFiles = new StringBuilder(strFiles_.Length * FileSize + 1);
            foreach (string strF in strFiles_)
            {
                sbFiles.Append(strF);
                sbFiles.Append('\0');
            }
            sbFiles.Append('\0');
            return sbFiles.ToString();
        }


        /// <summary>
        /// �ļ�����
        /// </summary>
        /// <param name="lstFrom_">Դ�ļ��б�</param>
        /// <param name="lstTo_">Ŀ���ļ��б�</param>
        /// <param name="euFunc_">�������</param>
        /// <param name="euFlag_">������־</param>
        /// <returns>�û�ȡ������������true������false</returns>
        public static bool FileOperation(List<string> lstFrom_, List<string> lstTo_, SHFileOPFunc euFunc_, SHFileOPFlag euFlag_)
        {
            const int FileSize = 256;
            StringBuilder sbFroms = new StringBuilder(lstFrom_.Count * FileSize + 1);
            StringBuilder sbTos = new StringBuilder(lstTo_.Count * FileSize + 1);

            // Build From Files string
            foreach (string strF in lstFrom_)
            {
                sbFroms.Append(strF);
                sbFroms.Append('\0');
            }
            sbFroms.Append('\0');

            // Build To Files string
            foreach (string strT in lstTo_)
            {
                sbTos.Append(strT);
                sbTos.Append('\0');
            }
            sbTos.Append('\0');

            return FileOperation(sbFroms.ToString(), sbTos.ToString(), euFunc_, euFlag_);
        }

        /// <summary>
        /// �ļ�����
        /// </summary>
        /// <param name="lstFrom_">Դ�ļ��б�</param>
        /// <param name="lstTo_">Ŀ���ļ��б�</param>
        /// <param name="euFunc_">�������</param>
        /// <returns>�û�ȡ������������true������false</returns>
        public static bool FileOperation(List<string> lstFrom_, List<string> lstTo_, SHFileOPFunc euFunc_)
        {
            return FileOperation(lstFrom_, lstTo_, euFunc_, SHFileOPFlag.None);
        }
        #endregion

    }
}
