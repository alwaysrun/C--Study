using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

using SHCre.Xugd.Common;

namespace SHCre.Xugd.WinAPI
{
    /// <summary>
    /// WinShell API封装
    /// </summary>
    public static class XShell
    {
        /// <summary>
        /// 文件属性标志位
        /// </summary>
        [Flags]
        public enum FileAttributes
        {
            /// <summary>
            /// 没有任何属性
            /// </summary>
            None = 0,
            /// <summary>
            /// 存档文件
            /// </summary>
            Archive = 0x00000020,
            /// <summary>
            /// 为一个设备
            /// </summary>
            Device = 0x00000040,
            /// <summary>
            /// 是文件夹
            /// </summary>
            Directory = 0x00000010,
            /// <summary>
            /// 加密文件
            /// </summary>
            Encrypted = 0x00004000,
            /// <summary>
            /// 隐藏文件
            /// </summary>
            Hidden = 0x00000002,
            /// <summary>
            /// 普通文件，没有其他标志
            /// </summary>
            Normal = 0x00000080,
            /// <summary>
            /// 只读文件
            /// </summary>
            ReadOnly = 0x00000001,
            /// <summary>
            /// 系统文件
            /// </summary>
            System = 0x00000004,
            /// <summary>
            /// 临时文件
            /// </summary>
            Tempory = 0x00000100
        };

        #region 获取文件/文件夹信息与图标

        /// <summary>
        /// 标识要获取的信息的标志位
        /// </summary>
        [Flags]
        public enum SHFileFlag
        {
            /// <summary>
            /// 获取文件对象的属性
            /// </summary>
            Attributes = 0x000000800,
            /// <summary>
            /// 获取用于显示的文件名称
            /// </summary>
            DisplayName = 0x000000200,
            /// <summary>
            /// 返回可执行文件的类型，不能与其他标志位同时设定
            /// </summary>
            ExeType = 0x000002000,
            /// <summary>
            /// 获取图标句柄和系统图标索引
            /// </summary>
            Icon = 0x000000100,
            /// <summary>
            /// 包含图标的文件名与图标在文件中的索引
            /// </summary>
            IconLocation = 0x000001000,
            /// <summary>
            /// 获取大图标，Icon标志位必须同时设定
            /// </summary>
            LargeIcon = 0x000000000,
            /// <summary>
            /// 在图标上添加连接标识，Icon标志位必须同时设定
            /// </summary>
            LinkOverlay = 0x000008000,
            /// <summary>
            /// 获取文件的打开图标，Icon或SysIcon标志位必须同时设定
            /// </summary>
            OpenIcon = 0x000000002,
            /// <summary>
            /// 说明strPath是一个ITEMIDLIST地址
            /// </summary>
            Pidl = 0x000000008,
            /// <summary>
            /// 获取选中样式图标（与系统高亮色混合），Icon标志位必须同时设定
            /// </summary>
            Selected = 0x000010000,
            /// <summary>
            /// 获取Shell-size大小的图标，Icon标志位必须同时设定
            /// </summary>
            ShellIconSize = 0x000000004,
            /// <summary>
            /// 获取小图标，Icon标志位必须同时设定
            /// </summary>
            SmallIcon = 0x000000001,
            /// <summary>
            /// 获取图标在系统图形列表中的索引
            /// </summary>
            SysIconIndex = 0x000004000,
            /// <summary>
            /// 获取类型类型描述信息
            /// </summary>
            TypeName = 0x000000400,
            /// <summary>
            /// 不试图去打开文件（文件可以不存在），不能与Attributes,ExeType,PDIL标志共存
            /// </summary>
            UseAttributes = 0x000000010
        };

        /// <summary>
        /// 存放文件信息的结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHFileInfo
        {
            /// <summary>
            /// 获取到的ICON图标的句柄，需要调用者负责释放(DestroyIcon)
            /// </summary>
            public IntPtr hIcon;
            /// <summary>
            /// 获取到的ICON图标在系统图标列表中的索引
            /// </summary>
            public int nIcon;
            /// <summary>
            /// 文件对象的属性(与IShellFolder::GetAttributesOf返回相同，如 SFGAO_FOLDER)
            /// </summary>
            public uint nAttributes;
            /// <summary>
            /// 用于显示的文件名
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            /// <summary>
            /// 文件类型类型描述信息
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        /// <summary>
        /// 获取文件系统中一个对象的信息；如果返回的对象中包含图标的句柄，则需要使用DestroyIcon来释放
        /// </summary>
        /// <param name="strPath">文件路径：如果设定PIDL标志则为ITEMIDLIST地址；
        ///     如果设定UserAttributes文件可以不存在</param>
        /// <param name="euAttributes">文件的属性，只有UserAttributes设定时才有效</param>
        /// <param name="stFileInfo">存放对象信息结构体</param>
        /// <param name="nFileInfoSize">结构体的大小</param>
        /// <param name="euFlags">获取信息的标志</param>
        /// <returns>如果标志中不包含ExeType,SysIconIndex：成功返回非零；失败返回零。</returns>
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
        /// 获取文件系统中一个对象的信息；如果返回的对象中包含图标的句柄，则需要使用DestroyIcon来释放。
        /// 失败会抛出Win32Exception异常
        /// </summary>
        /// <param name="strPath_">文件路径：如果设定UserAttributes文件可以不存在</param>
        /// <param name="euAttri_">文件的属性，只有UserAttributes设定时才有效</param>
        /// <param name="euFlags_">获取信息的标志</param>
        /// <returns>文件对象信息</returns>
        public static SHFileInfo SHGetFileInfo(string strPath_, FileAttributes euAttri_, SHFileFlag euFlags_)
        {
            SHFileInfo stInfo = new SHFileInfo();
            if (IntPtr.Zero == SHGetFileInfo(strPath_, euAttri_, ref stInfo, (uint)Marshal.SizeOf(stInfo), euFlags_))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "WinAPI-=-SHGetFileInfo(): failed");

            return stInfo;
        }

        /// <summary>
        /// 获取文件系统中一个对象的信息；如果返回的对象中包含图标的句柄，则需要使用DestroyIcon来释放。
        /// 失败会抛出Win32Exception异常
        /// </summary>
        /// <param name="pIdl_">ITEMIDLIST地址(需要设定PIDL标志)</param>
        /// <param name="euAttri_">文件的属性</param>
        /// <param name="euFlags_">获取信息的标志</param>
        /// <returns>文件对象信息</returns>
        public static SHFileInfo SHGetFileInfo(IntPtr pIdl_, FileAttributes euAttri_, SHFileFlag euFlags_)
        {
            SHFileInfo stInfo = new SHFileInfo();
            if (IntPtr.Zero == SHGetFileInfo(pIdl_, euAttri_, ref stInfo, (uint)Marshal.SizeOf(stInfo), euFlags_ | SHFileFlag.Pidl))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "WinAPI-=-SHGetFileInfo(): failed");

            return stInfo;
        }

        /// <summary>
        /// 释放图标的句柄
        /// </summary>
        /// <param name="hIcon_">图标的句柄</param>
        /// <returns>成功，返回true；否则，返回false</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr hIcon_);

        /// <summary>
        /// 从指定的可执行的文件中获取大图标或小图标的数组，所有的图标都需通过DestroyIcon来释放；
        /// 如果nIconIndex为-1，phiconLarge和phiconSmall为IntPtr.zero则返回文件中包含的图标数量
        /// </summary>
        /// <param name="strFile">可执行文件路径</param>
        /// <param name="nIconIndex">图标的索引（从0开始计数）</param>
        /// <param name="phiconLarge">大图标数组</param>
        /// <param name="phiconSmall">小图标数组</param>
        /// <param name="nIcons">要获取图标的数量</param>
        /// <returns>获取的图标数量</returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern uint ExtractIconEx(
                [MarshalAs(UnmanagedType.LPWStr)]
                string strFile,
                int nIconIndex,
                IntPtr[] phiconLarge,
                IntPtr[] phiconSmall,
                uint nIcons);

        /// <summary>
        /// 根据文件名，获取文件的图标和类型描述信息（文件必须存在）。
        /// 失败会抛出Win32Exception异常
        /// </summary>
        /// <param name="strFullName_">文件的全名</param>
        /// <param name="euAttri_">文件属性（Flag中设定了UserAttributes时有效）</param>
        /// <param name="euFlags_">获取信息的标志</param>
        /// <param name="strTypeInfo_">返回文件的类型类型描述信息</param>
        /// <returns>获取的ICON图标</returns>
        public static System.Drawing.Icon GetFileInfo(string strFullName_, FileAttributes euAttri_, SHFileFlag euFlags_, out string strTypeInfo_)
        {
            SHFileInfo stInfo = SHGetFileInfo(strFullName_, euAttri_, euFlags_);
            System.Drawing.Icon hIcon = System.Drawing.Icon.FromHandle(stInfo.hIcon);
            strTypeInfo_ = stInfo.szTypeName;
            return hIcon;
        }

        /// <summary>
        /// 根据文件名，获取文件的图标和类型类型描述信息（文件必须存在）。
        /// 失败会抛出Win32Exception异常
        /// </summary>
        /// <param name="strFullName_">文件的全名</param>
        /// <param name="euFlags_">获取信息的标志</param>
        /// <param name="strTypeInfo_">返回文件的类型类型描述信息</param>
        /// <returns>获取的ICON图标</returns>
        public static System.Drawing.Icon GetFileInfo(string strFullName_, SHFileFlag euFlags_, out string strTypeInfo_)
        {
            return GetFileInfo(strFullName_, FileAttributes.Normal, euFlags_, out strTypeInfo_);
        }

        /// <summary>
        /// 获取文件的图标(小图标)和类型类型描述信息。
        /// 失败会抛出Win32Exception异常
        /// </summary>
        /// <param name="strFullName_">文件的全名</param>
        /// <param name="strTypeInfo_">返回文件的类型类型描述信息</param>
        /// <returns>获取的ICON图标</returns>
        public static System.Drawing.Icon GetFileInfo(string strFullName_, out string strTypeInfo_)
        {
            return GetFileInfo(strFullName_, FileAttributes.Normal,
                SHFileFlag.Icon | SHFileFlag.SmallIcon | SHFileFlag.TypeName, out strTypeInfo_);
        }

        /// <summary>
        /// 根据文件名（扩展名）获取文件的图标(小图标)和类型类型描述信息；文件不需要存在。
        /// 失败会抛出Win32Exception异常
        /// </summary>
        /// <param name="strFileName_">文件名</param>
        /// <param name="strTypeInfo_">返回文件的类型类型描述信息</param>
        /// <returns>获取的ICON图标</returns>
        public static System.Drawing.Icon GetFileInfoByExt(string strFileName_, out string strTypeInfo_)
        {
            return GetFileInfo(strFileName_, FileAttributes.Normal,
                SHFileFlag.Icon | SHFileFlag.SmallIcon | SHFileFlag.TypeName | SHFileFlag.UseAttributes, out strTypeInfo_);
        }

        /// <summary>
        /// 获取文件夹的图标(小图标)和类型描述信息。
        /// 失败会抛出Win32Exception异常
        /// </summary>
        /// <param name="strTypeInfo_">文件夹的类型描述信息</param>
        /// <returns>获取的ICON图标</returns>
        public static System.Drawing.Icon GetDirInfo(out string strTypeInfo_)
        {
            // Use SystemDirectory (As at win7, if the file not existed, can not get the icon)
            return GetFileInfo(Environment.SystemDirectory, FileAttributes.Directory,
                SHFileFlag.Icon | SHFileFlag.SmallIcon | SHFileFlag.TypeName | SHFileFlag.UseAttributes, out strTypeInfo_);
        }

        /// <summary>
        /// 获取文件夹的图标(小图标)和类型描述信息。
        /// 失败会抛出Win32Exception异常
        /// </summary>
        /// <param name="chDrive_">要获取信息的盘符</param>
        /// <param name="strTypeInfo_">盘符的类型描述信息</param>
        /// <returns>获取的ICON图标</returns>
        public static System.Drawing.Icon GetDriveInfo(char chDrive_, out string strTypeInfo_)
        {
            return GetFileInfo(string.Format("{0}:\\", chDrive_), FileAttributes.None,
                SHFileFlag.Icon | SHFileFlag.SmallIcon | SHFileFlag.TypeName, out strTypeInfo_);
        }

        /// <summary>
        /// 获取文件夹的图标(小图标)和类型描述信息。
        /// 失败会抛出Win32Exception异常
        /// </summary>
        /// <param name="chDrive_">要获取信息的盘符</param>
        /// <param name="strTypeInfo_">盘符的类型描述信息</param>
        /// <param name="strDisplayName_">盘符的显示信息（名称）</param>
        /// <returns>获取的ICON图标</returns>
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
        /// 特殊文件夹
        /// </summary>
        public enum SpecialFolder
        {
            /// <summary>
            /// 我的电脑
            /// </summary>
            Computer = 0,
            /// <summary>
            /// 网上邻居
            /// </summary>
            Network,
            /// <summary>
            /// 回收站
            /// </summary>
            Recycler,
            /// <summary>
            /// 浏览器IE
            /// </summary>
            Internet,
            /// <summary>
            /// 控制面板
            /// </summary>
            ControlPanel,
            /// <summary>
            /// 打印机
            /// </summary>
            Printer,
            /// <summary>
            /// 桌面（快速启动中的桌面图标）
            /// </summary>
            Desktop,
            /// <summary>
            /// 收藏夹（IE）图标
            /// </summary>
            Favorite,
            /// <summary>
            /// 程序（开始菜单中的程序图标）
            /// </summary>
            Programs,
            /// <summary>
            /// 最近使用文档（开始菜单中的文档图标）
            /// </summary>
            Recent,
            /// <summary>
            /// 音乐
            /// </summary>
            Music,
            /// <summary>
            /// 图片
            /// </summary>
            Pcture,
            /// <summary>
            /// 视频
            /// </summary>
            Video,
            /// <summary>
            /// 本地磁盘
            /// </summary>
            Disk
        };

        /// <summary>
        /// 获取特殊文件夹的图标与信息
        /// </summary>
        /// <param name="euFolder_">特殊文件夹标识</param>
        /// <param name="strInfo_">说明信息</param>
        /// <returns>图标句柄</returns>
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


        #region 文件Shell操作
        /// <summary>
        /// ShellExcuteEx执行时状态掩码(打开、打开方式、属性)
        /// </summary>
        public enum SHExecMask
        {
            /// <summary>
            /// 成员lpClass有效
            /// </summary>
            ClassName = 0x00000001,
            /// <summary>
            /// 成员hKeyClass有效
            /// </summary>
            ClassKey = 0x00000003,
            /// <summary>
            /// 成员lpIDList有效
            /// </summary>
            IDList = 0x00000004,
            /// <summary>
            /// 允许通过快捷菜单扩展了激活程序
            /// </summary>
            InvokeIDList = 0x0000000C,
            /// <summary>
            /// 成员dwHotKey有效
            /// </summary>
            HotKey = 0x00000020,
            /// <summary>
            /// 说明hProcess包含程序句柄
            /// </summary>
            NoCloseEProcess = 0x00000040,
            /// <summary>
            /// Validate the share and connect to a drive letter
            /// </summary>
            ConnectNetdrv = 0x00000080,
            /// <summary>
            /// 返回前等待DDE转换完成
            /// </summary>
            FlagDdeWait = 0x00000100,
            /// <summary>
            /// 扩展lpDirectory或lpFile中的环境变量
            /// </summary>
            DoEnvsubst = 0x00000200,
            /// <summary>
            /// 出错时，不要显示错误消息框
            /// </summary>
            FlagNoUI = 0x00000400,
            /// <summary>
            /// 说明是宽字符环境
            /// </summary>
            Unicode = 0x00004000,
            /// <summary>
            /// 创建新的Console，而不是用非进程
            /// </summary>
            NoConsole = 0x00008000,
            /// <summary>
            /// 成员hMonitor（与hIcon共用同一位置）有效
            /// </summary>
            Monitor = 0x00200000,
            /// <summary>
            /// 不执行Zone检测
            /// </summary>
            NoZoneChecks = 0x00800000,
            /// <summary>
            /// 持续跟踪程序的使用时间
            /// </summary>
            FlagLogUsage = 0x04000000
        };
        /// <summary>
        /// 用于包含文件Shell操作所需信息的结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHExecInfo
        {
            /// <summary>
            /// 此结构体的大小（以自己计算）
            /// </summary>
            public int cbSize;
            /// <summary>
            /// 影响内容和其他成员的标志位
            /// </summary>
            public SHExecMask euMask;
            /// <summary>
            /// 执行时系统所需的消息窗体句柄
            /// </summary>
            public IntPtr hWnd;
            /// <summary>
            /// 表示要执行的动作的文本
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpVerb;
            /// <summary>
            /// 要操作的文件
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpFile;
            /// <summary>
            /// 参数列表
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpParameters;
            /// <summary>
            /// 工作目录，如果没有指定则使用当前目录
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpDirectory;
            /// <summary>
            /// 表示程序启动时如何显示
            /// </summary>
            public WinShowMode euShow;
            /// <summary>
            /// 如果成功，返回大于32的数；如果出错，则设为SE_ERR_XXX值
            /// </summary>
            public int hRetCode;
            /// <summary>
            /// 标识文件的ID列表（只有fMask包含SEE_MASK_IDLIST或SEE_MASK_INVOKEIDLIST时有效）
            /// </summary>
            public int lpIDList;
            /// <summary>
            /// 表示文件类名或全球唯一标识（只有fMask包含SEE_MASK_CLASSKEY时有效）
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpClass;
            /// <summary>
            /// 注册表中文件类句柄（只有fMask包含SEE_MASK_CLASSKEY时有效）
            /// </summary>
            public IntPtr hkeyClass;
            /// <summary>
            /// 程序相关热键（只有fMask包含SEE_MASK_HOTKEY时有效）
            /// </summary>
            public uint dwHotKey;
            /// <summary>
            /// 包含文件类图标的句柄（只有fMask包含SEE_MASK_ICON时有效）
            /// </summary>
            public IntPtr hIcon;
            /// <summary>
            /// 新启动程序的句柄（只有fMask包含SEE_MASK_NOCLOSEPROCESS时有效）
            /// </summary>
            public IntPtr hProcess;
        };

        /// <summary>
        /// 执行文件相关操作
        /// </summary>
        /// <param name="stExecInfo_">操行信息</param>
        /// <returns>成功，true；失败，false</returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShellExecuteEx(ref SHExecInfo stExecInfo_);

        /// <summary>
        /// 执行一些文件相关的操作
        /// </summary>
        /// <param name="hWnd_">弹出窗体的父窗体，没有可传递IntPtr.Zero</param>
        /// <param name="strOperate_">操作字符串，没有传递null</param>
        /// <param name="strFile_">操作的文件</param>
        /// <param name="strParams_">操作参数</param>
        /// <param name="strDir_">相关路径</param>
        /// <param name="euShow_">弹出窗体的初始状态</param>
        /// <returns>可转换为int32的句柄：大于32，成功；否则参考MSDN中的Return Value说明</returns>
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
        /// 文件的操作动作
        /// </summary>
        public enum SHExecAction
        {
            /// <summary>
            /// 打开文件
            /// </summary>
            Open,
            /// <summary>
            /// 弹出文件的打开方式对话框
            /// </summary>
            OpenAs,
            /// <summary>
            /// 弹出文件的属性对话框
            /// </summary>
            Property
        };

        /// <summary>
        /// 执行文件相关操作
        /// </summary>
        /// <param name="strFile_">要执行操作的文件</param>
        /// <param name="euAction_">具体要进行的操作</param>
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

        #region 文件操作（移动、复制、删除、重命名）
        /// <summary>
        /// 操作说明
        /// </summary>
        public enum SHFileOPFunc
        {
            /// <summary>
            /// 移动
            /// </summary>
            Move = 0x0001,
            /// <summary>
            /// 复制
            /// </summary>
            Copy = 0x0002,
            /// <summary>
            /// 删除
            /// </summary>
            Delete = 0x0003,
            /// <summary>
            /// 重命名
            /// </summary>
            Rename = 0x0004
        };

        /// <summary>
        /// 文件操作标志
        /// </summary>
        [Flags]
        public enum SHFileOPFlag : ushort
        {
            /// <summary>
            /// 空，无效的
            /// </summary>
            None = 0,
            /// <summary>
            /// pTo中指定多个目标文件
            /// </summary>
            MultiDestFiles = 0x0001,
            /// <summary>
            /// 不显示进度对话框
            /// </summary>
            Silent = 0x0004,
            /// <summary>
            /// 如果同名文件已存在，重命名文件
            /// </summary>
            RenameOnCollision = 0x0008,
            /// <summary>
            /// 对所有询问操作执行‘Yes to All’
            /// </summary>
            NoConfirmation = 0x0010,
            /// <summary>
            /// 如果重命名了文件，在返回一个旧文件与新文件名的映射对象
            /// </summary>
            WantMappingHandle = 0x0020,
            /// <summary>
            /// 允许撤销当前操作
            /// </summary>
            AllowUndo = 0x0040,
            /// <summary>
            /// 如果有通配符（*.*）时，只匹配文件
            /// </summary>
            FilesOnly = 0x0080,
            /// <summary>
            /// 进度条上不显示文件名
            /// </summary>
            SimplEProgress = 0x0100,
            /// <summary>
            /// 如果需要创建文件夹，不需要确认直接创建
            /// </summary>
            NoConfirmMkdir = 0x0200,
            /// <summary>
            /// 出错时，不显示UI
            /// </summary>
            NoErrorUI = 0x0400,
            /// <summary>
            /// 不复制文件安全属性
            /// </summary>
            NoCopySecurityAttribs = 0x0800,
            /// <summary>
            /// 只处理当前文件夹，不递归处理到子文件夹
            /// </summary>
            NoRecursion = 0x1000,
            /// <summary>
            /// Do not move connected files as a group. Only move the specified files.
            /// </summary>
            NoConnectedElements = 0x2000,
            /// <summary>
            /// 删除文件时，发出警告
            /// </summary>
            WantNukeWarning = 0x4000,
            /// <summary>
            /// 没有UI
            /// </summary>
            NoUI = Silent | NoConfirmation | NoErrorUI | NoConfirmMkdir
        };

        /// <summary>
        /// 文件操作信息结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHFileOPInfo
        {
            /// <summary>
            /// 显示信息的窗体句柄
            /// </summary>
            public IntPtr hWnd;
            /// <summary>
            /// 具体的操作
            /// </summary>
            public SHFileOPFunc euFunc;
            /// <summary>
            /// 原文件，以两个'\0'作为结束（多个文件时，中间以一个'\0'作为间隔）
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pFrom;
            /// <summary>
            /// 目标文件，以两个'\0'作为结束（多个文件时，中间以一个'\0'作为间隔）
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pTo;
            /// <summary>
            /// 操作标志
            /// </summary>
            public SHFileOPFlag euFlag;
            /// <summary>
            /// 如果有任何Abort操作，返回true，否则false
            /// </summary>
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            /// <summary>
            /// 映射表对象句柄
            /// </summary>
            public IntPtr hNameMappings;
            /// <summary>
            /// 设定SimplEProgress时，显示的标题
            /// </summary>
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszProgressTitle;
        };

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        private static extern int SHFileOperation(ref SHFileOPInfo stInfo_);

        /// <summary>
        /// 文件操作
        /// </summary>
        /// <param name="stInfo_">操作信息结构体</param>
        public static void FileOperation(ref SHFileOPInfo stInfo_)
        {
            if (0 != SHFileOperation(ref stInfo_))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "WinAPI-=-SHFileOperation(): failed");
        }

        /// <summary>
        /// 文件操作
        /// </summary>
        /// <param name="strFroms_">源文件</param>
        /// <param name="strTos_">目标文件</param>
        /// <param name="euFunc_">具体操作</param>
        /// <param name="euFlag_">操作标志</param>
        /// <returns>用户取消过操作返回true；否则false</returns>
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
        /// 文件操作
        /// </summary>
        /// <param name="strFroms_">源文件</param>
        /// <param name="strTos_">目标文件</param>
        /// <param name="euFunc_">具体操作</param>
        /// <returns>用户取消过操作返回true；否则false</returns>
        public static bool FileOperation(string strFroms_, string strTos_, SHFileOPFunc euFunc_)
        {
            return FileOperation(strFroms_, strTos_, euFunc_, SHFileOPFlag.None);
        }

        /// <summary>
        /// 把输入的多个文件字符串封装成可以被FileOperation使用的单个字符串
        /// </summary>
        /// <param name="strFiles_">文件名</param>
        /// <returns>已组装好的字符串，以两个'\0'结尾</returns>
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
        /// 文件操作
        /// </summary>
        /// <param name="lstFrom_">源文件列表</param>
        /// <param name="lstTo_">目标文件列表</param>
        /// <param name="euFunc_">具体操作</param>
        /// <param name="euFlag_">操作标志</param>
        /// <returns>用户取消过操作返回true；否则false</returns>
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
        /// 文件操作
        /// </summary>
        /// <param name="lstFrom_">源文件列表</param>
        /// <param name="lstTo_">目标文件列表</param>
        /// <param name="euFunc_">具体操作</param>
        /// <returns>用户取消过操作返回true；否则false</returns>
        public static bool FileOperation(List<string> lstFrom_, List<string> lstTo_, SHFileOPFunc euFunc_)
        {
            return FileOperation(lstFrom_, lstTo_, euFunc_, SHFileOPFlag.None);
        }
        #endregion

    }
}
