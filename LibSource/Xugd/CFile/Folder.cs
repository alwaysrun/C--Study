using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SHCre.Xugd.CFile
{
    /// <summary>
    /// 文件夹操作相关类
    /// </summary>
    public static partial class XFolder
    {        
        /// <summary>
        /// 判断文件夹是否为空
        /// </summary>
        /// <param name="strFolder_">要判断文件夹</param>
        /// <returns>文件夹为空，返回true；否则，返回false</returns>
        public static bool IsEmpty(string strFolder_)
        {
            return (Directory.GetFiles(strFolder_).Length == 0
                && Directory.GetDirectories(strFolder_).Length == 0);
        }

        /// <summary>
        /// 删除指定目录下的空文件夹
        /// </summary>
        /// <param name="strTop_">要处理的目录</param>
        public static void DeleteEmptyFolder(string strTop_)
        {
            string[] strSubs = Directory.GetDirectories(strTop_);
            foreach(string strF in strSubs)
            {
                DeleteEmptyFolder(strF);
            }

            if (IsEmpty(strTop_))
                Directory.Delete(strTop_);
        }

        /// <summary>
        /// 获取路径的顶层目录
        /// </summary>
        /// <param name="strPath_"></param>
        /// <returns></returns>
        public static string GetTopFolder(string strPath_)
        {
            int nIndex = strPath_.IndexOfAny(new char[]{'\\', '/'});
            while(nIndex==0)
            {
                strPath_ = strPath_.Substring(1);
                nIndex = strPath_.IndexOfAny(new char[] { '\\', '/' });
            }
            if (nIndex == -1)
                return strPath_;

            return strPath_.Substring(0, nIndex);
        }
    }
}
