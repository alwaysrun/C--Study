using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.Extension
{
    /// <summary>
    /// 字符串相关的扩展
    /// </summary>
    public static class XStringExtension
    {
        /// <summary>
        /// 字符串替换：可根据选项设定是否区分大小写（如果区分大小的替换，可直接用string.Replace)
        /// </summary>
        /// <param name="strSource_">要处理的字符串</param>
        /// <param name="strOldValue_">要替换掉的值</param>
        /// <param name="strNewValue_">替换为的值</param>
        /// <param name="regOption_">替换选项</param>
        /// <returns>替换后的字符串</returns>
        public static string ReplaceEx(this string strSource_, string strOldValue_, string strNewValue_, RegexOptions regOption_=RegexOptions.IgnoreCase)
        {
            return Regex.Replace(strSource_, strOldValue_, strNewValue_, regOption_);
        }

        /// <summary>
        /// 从字符串中移除指定的后缀字符串(忽略大小写)：
        /// TrimAll是否移除所有后缀：true递归移除所有，false只移除最后一个后缀。
        /// </summary>
        /// <param name="strSrc_"></param>
        /// <param name="strTrimed_"></param>
        /// <param name="bTrimAll_"></param>
        /// <returns></returns>
        public static string TrimEnd(this string strSrc_, string strTrimed_, bool bTrimAll_ = true)
        {
            return XString.TrimEnd(strSrc_, strTrimed_, bTrimAll_);
        }
    }
}
