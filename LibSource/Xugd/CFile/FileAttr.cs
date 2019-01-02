using System;

namespace SHCre.Xugd.CFile
{
    partial class XFile
    {
        /// <summary>
        /// 把文件长度转换为以KB为单位的（以千分位分割的）字符串
        /// </summary>
        /// <param name="nFSize_">文件的大小</param>
        /// <returns>转换后的字符串</returns>
        public static string FSize2String(long nFSize_)
        {
            long nLen = nFSize_ / 1024;
            if (0 != (nFSize_ % 1024))
                ++nLen;

            return string.Format("{0:N0} KB", nLen);
        }

        /// <summary>
        /// 将文件时间转换为方便阅读的字符串形式
        /// </summary>
        /// <param name="stTime_">要转换的时间</param>
        /// <returns>字符串形式的时间</returns>
        public static string FTime2String(DateTime stTime_)
        {
            return stTime_.ToString("yyyy-MM-dd HH:mm");
        }

        /// <summary>
        /// 将文件时间转换为方便阅读的字符串形式
        /// </summary>
        /// <param name="nTime_">要转换的时间</param>
        /// <returns>字符串形式的时间</returns>
        public static string FTime2String(long nTime_)
        {
            return FTime2String(DateTime.FromFileTime(nTime_));
        }
    }
}
