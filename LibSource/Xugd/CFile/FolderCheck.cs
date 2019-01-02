using System.IO;

using SHCre.Xugd.Common;

namespace SHCre.Xugd.CFile
{
    partial class XFolder
    {
        /// <summary>
        /// 判断文件是否是文件夹
        /// </summary>
        /// <param name="strPath_">要判断的文件（全路径）</param>
        /// <returns>是文件夹返回true，否则返回false</returns>
        public static bool IsFolder(string strPath_)
        {
            if (!File.Exists(strPath_)) return false;

            return XFlag.Check((uint)File.GetAttributes(strPath_), (uint)FileAttributes.Directory);
        }

        static readonly string r_strFatRecycle = @":\Recycled";
        static readonly string r_strNtfsRecycle = @":\RECYCLER";
        /// <summary>
        /// 判断文件夹是否为回收站
        /// </summary>
        /// <param name="strPath_">要判断的路径</param>
        /// <returns>是返回true，否则返回false</returns>
        public static bool IsRecycle(string strPath_)
        {
            bool bIsRecycle = false;

            try
            {
                string strFull = Path.GetFullPath(strPath_);
                if ((0 == string.Compare(strFull, 1, r_strNtfsRecycle, 0, r_strNtfsRecycle.Length, true))
                    || (0 == string.Compare(strFull, 1, r_strFatRecycle, 0, r_strFatRecycle.Length, true)))
                {
                    if (XFlag.Check((uint)File.GetAttributes(strFull), (uint)(FileAttributes.System | FileAttributes.Hidden | FileAttributes.Directory)))
                        bIsRecycle = true;
                }

            }
            catch { }

            return bIsRecycle;
        }

        /// <summary>
        /// 判断文件是否为系统文件夹（有System与Hide属性）
        /// </summary>
        /// <param name="strPath_">要判断的文件夹</param>
        /// <returns>为系统文件夹，返回true；否则返回false</returns>
        public static bool IsSystemFolder(string strPath_)
        {
            return XFlag.Check((uint)File.GetAttributes(strPath_), 
                            (uint)(FileAttributes.System | FileAttributes.Hidden | FileAttributes.Directory));
        }
    }
}
