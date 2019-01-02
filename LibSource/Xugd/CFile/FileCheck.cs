using System;
using System.IO;

using SHCre.Xugd.Common;

namespace SHCre.Xugd.CFile
{
    partial class XFile
    {
        /// <summary>
        /// 判断文件是否存在：若不存在或是文件夹，则抛出异常
        /// </summary>
        /// <param name="strFiles_"></param>
        public static void CheckFileExist(params string[] strFiles_)
        {
            if (strFiles_.Length == 0) return;

            foreach (string strF in strFiles_)
            {
                var strFull = XPath.GetFullPath(strF);
                if (!File.Exists(strFull))
                    throw new FileNotFoundException(string.Format("{0} not exist", strF));

                if (!IsFile(strFull))
                    throw new FileNotFoundException(string.Format("{0} is not file", strF));
            }
        }

        /// <summary>
        /// 判断文件是否是文件
        /// </summary>
        /// <param name="strFile_">要判断的文件（全路径）</param>
        /// <returns>是文件返回true，否则返回false</returns>
        public static bool IsFile(string strFile_)
        {
            if (!File.Exists(strFile_)) return false;

            return !XFlag.Check((uint)File.GetAttributes(strFile_), (uint)FileAttributes.Directory);
        }

        /// <summary>
        /// 判断文件是否在使用（被其他程序打开）
        /// </summary>
        /// <param name="strFile_">要判断的文件</param>
        /// <returns>在使用返回true，否则返回false</returns>
        public static bool IsInUse(string strFile_)
        {
            bool bInUse = false;
            if (File.Exists(strFile_))
            {
                try
                {
                    using (FileStream fs = File.Open(strFile_, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        fs.Close();
                    }
                }
                catch (Exception)
                {
                    bInUse = true;
                }
            }

            return bInUse;
        }
        
        /// <summary>
        /// 判断文件是否为系统文件（有System与Hide属性）
        /// </summary>
        /// <param name="strFile_">要判断的文件</param>
        /// <returns>为系统文件，返回true；否则返回false</returns>
        public static bool IsSystemFile(string strFile_)
        {
            uint nAttr = (uint)File.GetAttributes(strFile_);
            if (!XFlag.Check(nAttr, (uint)FileAttributes.Directory)
                && XFlag.Check(nAttr, (uint)(FileAttributes.System | FileAttributes.Hidden)))
                return true;

            return false;
        }
    }
}
