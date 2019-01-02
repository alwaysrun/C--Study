using System;
using System.Collections.Generic;
using System.Text;

namespace SHCre.Xugd.Common
{
    /// <summary>
    /// 格式化相关
    /// </summary>
    public static class XFormat
    {
        /// <summary>
        /// 输出带指定精度的小数（小数部分非零部分才输出）：
        /// 12.1=DecimalWithPrecision(12.10002, 3) ；
        /// </summary>
        /// <param name="oNumber_"></param>
        /// <param name="nDecimals_"></param>
        /// <returns></returns>
        public static string DecimalWithPrecision(object oNumber_, int nDecimals_ = 2)
        {
            if (nDecimals_ <= 0)
                nDecimals_ = 2;
            string strFormat = string.Format("{{0:.{0}}}", new string('#', nDecimals_));
            return string.Format(strFormat, oNumber_);
        }

        /// <summary>
        /// 输出带指定精度的小数（小数部分总是输出指定位数）：
        /// 12.100=DecimalWithPrecision(12.10002, 3) 
        /// </summary>
        /// <param name="oNumber_"></param>
        /// <param name="nDecimals_"></param>
        /// <returns></returns>
        public static string DecimalWithExactPrecision(object oNumber_, int nDecimals_ = 2)
        {
            if (nDecimals_ <= 0)
                nDecimals_ = 2;
            string strFormat = string.Format("{{0:.{0}}}", new string('0', nDecimals_));
            return string.Format(strFormat, oNumber_);
        }

        /// <summary>
        /// 输出指定长度的整数（如果长度小于指定的长度，则左侧填充0）：
        /// 0012=DecimalWithPrecision(12.10002, 4) ；
        /// 123456=DecimalWithPrecision(123456, 4) ；
        /// </summary>
        /// <param name="oNumber_"></param>
        /// <param name="nDigital_"></param>
        /// <returns></returns>
        public static string IntegerWithLength(object oNumber_, int nDigital_ = 1)
        {
            if (nDigital_ <= 0)
                nDigital_ = 1;
            string strFormat = string.Format("{{0:{0}}}", new string('0', nDigital_));
            return string.Format(strFormat, oNumber_);
        }
    }
}
