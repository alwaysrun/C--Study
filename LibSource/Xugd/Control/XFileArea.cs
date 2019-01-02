using System;
using System.IO;

namespace SHCre.Xugd.XControl
{
    /// <summary>
    /// ��ȫ�����������
    /// </summary>
    class XSafeArea
    {
        /// <summary>
        /// ��ȡ·������ʾ����ʹ��strRootName_�滻strPath_�е�strRootPath_��
        /// </summary>
        /// <param name="strPath_">Ҫ��ʾ��·��</param>
        /// <param name="strRootPath_">Ҫ����·���ĸ�</param>
        /// <param name="strRootName_">Ҫ����·���ĸ���</param>
        /// <returns>�������ʾ��·��</returns>
        public static string GetShowPath(string strPath_, string strRootPath_, string strRootName_)
        {
            string strShowText = string.Format("[{0}]{1}{2}", strRootName_, Path.VolumeSeparatorChar, Path.DirectorySeparatorChar);
            if (strPath_.Length > (strRootPath_.Length + 1))    // Not include the '\'
            {
                string strRelative = strPath_.Substring(strRootPath_.Length).Trim(Path.DirectorySeparatorChar);
                strShowText = Path.Combine(strShowText, strRelative);
            }

            return strShowText;
        }
    }
}
