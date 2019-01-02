using System;
using System.IO;

namespace SHCre.Xugd.XControl
{
    /// <summary>
    /// 安全区处理相关类
    /// </summary>
    class XSafeArea
    {
        /// <summary>
        /// 获取路径的显示名（使用strRootName_替换strPath_中的strRootPath_）
        /// </summary>
        /// <param name="strPath_">要显示的路径</param>
        /// <param name="strRootPath_">要处理路径的根</param>
        /// <param name="strRootName_">要处理路径的根名</param>
        /// <returns>处理后显示的路径</returns>
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
