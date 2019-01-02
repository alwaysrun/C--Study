using System;
using System.Collections.Generic;
using System.Text;

namespace SHCre.Xugd.Common
{
    /// <summary>
    /// 大小
    /// </summary>
    public static class XSize
    {
        const long SizeScaler = 1024;

        /// <summary>
        /// 字节转换为Kb
        /// </summary>
        /// <param name="lBytes_"></param>
        /// <returns></returns>
        public static long Bytes2Kb(long lBytes_)
        {
            return lBytes_ / SizeScaler;
        }

        /// <summary>
        /// 字节转换为Mb
        /// </summary>
        /// <param name="lBytes_"></param>
        /// <returns></returns>
        public static long Bytes2Mb(long lBytes_)
        {
            return lBytes_ / (SizeScaler * SizeScaler);
        }

        /// <summary>
        /// 字节转换为Gb
        /// </summary>
        /// <param name="lBytes_"></param>
        /// <returns></returns>
        public static long Bytes2Gb(long lBytes_)
        {
            return lBytes_ / (SizeScaler * SizeScaler * SizeScaler);
        }

        /// <summary>
        /// 输出大小（dd.ff G/M/KB），按最易识别的方式（保留两位小数）
        /// </summary>
        /// <param name="lBytes_"></param>
        /// <returns></returns>
        public static string PrintSize(long lBytes_)
        {
            char chUnit = 'K';
            double dSize = lBytes_ / SizeScaler;
            if(dSize > SizeScaler)
            {
                chUnit = 'M';
                dSize /= SizeScaler;
            }
            if(dSize > SizeScaler)
            {
                chUnit = 'G';
                dSize /= SizeScaler;
            }

            return string.Format("{0:N2} {1}B", dSize, chUnit);
        }
    }
}
