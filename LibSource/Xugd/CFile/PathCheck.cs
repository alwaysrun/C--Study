using System;
using System.IO;

namespace SHCre.Xugd.CFile
{
    /// <summary>
    /// 文件路径处理相关类
    /// </summary>
    public static class XPath
    {
        /// <summary>
        /// 判断strParent_是否是strSub_的父目录
        /// </summary>
        /// <param name="strParent_">父目录</param>
        /// <param name="strSub_">子目录</param>
        /// <returns>是父目录，返回true；否则，返回false</returns>
        public static bool IsParent(string strParent_, string strSub_)
        {
            if (strParent_.Length >= strSub_.Length)
                return false;

            char chSeparator = strSub_[strParent_.Length];
            if( (chSeparator != '\\') && (chSeparator != '/') )
                return false;

            return (strSub_.StartsWith(strParent_, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 获取完整路径，如果strFile不是完整路径，
        /// 则使用程序启动路径与strFile组合（StartPath\\strFile）
        /// </summary>
        /// <param name="strFile_">要判断的路径</param>
        /// <returns>完整路径</returns>
        public static string GetFullPath(string strFile_)
        {
            if (Path.IsPathRooted(strFile_))
                return strFile_;

            return Path.Combine(System.Windows.Forms.Application.StartupPath, strFile_);
        }

        /// <summary>
        /// 获取完整路径：
        /// strFile_为完整目录（以盘符X:或\\开始），则返回strFile_;
        /// 否则，组合为strPath\\strFile
        /// </summary>
        /// <param name="strFile_">要判断的路径</param>
        /// <param name="strPath_">必要时作为父目录的路径</param>
        /// <returns>完整路径</returns>
        public static string GetFullPath(string strFile_, string strPath_)
        {
            if (Path.IsPathRooted(strFile_))
                return strFile_;

            return Path.Combine(strPath_, strFile_);
        }

        /// <summary>
        /// 创建完整的目录（如果祖先目录不存在，则同时创建）
        /// </summary>
        /// <param name="strPath_">要创建的目录</param>
        public static void CreateFullPath(string strPath_)
        {
            if (string.IsNullOrEmpty(strPath_))
                return;

            if(!Directory.Exists(strPath_))
            {
                CreateFullPath(Path.GetDirectoryName(strPath_));
                Directory.CreateDirectory(strPath_);
            }
        }

        /// <summary>
        /// Windows路径转Linux
        /// </summary>
        /// <param name="strWinPath_"></param>
        /// <returns></returns>
        public static string Win2LinuxPath(string strWinPath_)
        {
            return strWinPath_.Replace('\\', '/');
        }

        /// <summary>
        /// Linux路径转Windows
        /// </summary>
        /// <param name="strLinuxPath_"></param>
        /// <returns></returns>
        public static string Linux2WinPath(string strLinuxPath_)
        {
            return strLinuxPath_.Replace('/', '\\');
        }
    }
}
