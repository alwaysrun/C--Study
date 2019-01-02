using System;
using System.IO;

namespace SHCre.Xugd.CFile
{
    /// <summary>
    /// �ļ�·�����������
    /// </summary>
    public static class XPath
    {
        /// <summary>
        /// �ж�strParent_�Ƿ���strSub_�ĸ�Ŀ¼
        /// </summary>
        /// <param name="strParent_">��Ŀ¼</param>
        /// <param name="strSub_">��Ŀ¼</param>
        /// <returns>�Ǹ�Ŀ¼������true�����򣬷���false</returns>
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
        /// ��ȡ����·�������strFile��������·����
        /// ��ʹ�ó�������·����strFile��ϣ�StartPath\\strFile��
        /// </summary>
        /// <param name="strFile_">Ҫ�жϵ�·��</param>
        /// <returns>����·��</returns>
        public static string GetFullPath(string strFile_)
        {
            if (Path.IsPathRooted(strFile_))
                return strFile_;

            return Path.Combine(System.Windows.Forms.Application.StartupPath, strFile_);
        }

        /// <summary>
        /// ��ȡ����·����
        /// strFile_Ϊ����Ŀ¼�����̷�X:��\\��ʼ�����򷵻�strFile_;
        /// �������ΪstrPath\\strFile
        /// </summary>
        /// <param name="strFile_">Ҫ�жϵ�·��</param>
        /// <param name="strPath_">��Ҫʱ��Ϊ��Ŀ¼��·��</param>
        /// <returns>����·��</returns>
        public static string GetFullPath(string strFile_, string strPath_)
        {
            if (Path.IsPathRooted(strFile_))
                return strFile_;

            return Path.Combine(strPath_, strFile_);
        }

        /// <summary>
        /// ����������Ŀ¼���������Ŀ¼�����ڣ���ͬʱ������
        /// </summary>
        /// <param name="strPath_">Ҫ������Ŀ¼</param>
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
        /// Windows·��תLinux
        /// </summary>
        /// <param name="strWinPath_"></param>
        /// <returns></returns>
        public static string Win2LinuxPath(string strWinPath_)
        {
            return strWinPath_.Replace('\\', '/');
        }

        /// <summary>
        /// Linux·��תWindows
        /// </summary>
        /// <param name="strLinuxPath_"></param>
        /// <returns></returns>
        public static string Linux2WinPath(string strLinuxPath_)
        {
            return strLinuxPath_.Replace('/', '\\');
        }
    }
}
