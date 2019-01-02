using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SHCre.Xugd.Common
{
    /// <summary>
    /// 字符串常量
    /// </summary>
    public static class XString
    {
        /// <summary>
        /// 回车换行
        /// </summary>
        public static readonly string NewLine = Environment.NewLine; //"\r\n";
        /// <summary>
        /// C格式字符串结束符
        /// </summary>
        public const string NullEnd = "\0";
        /// <summary>
        /// 空格字符串
        /// </summary>
        public const string BlankSpace = " ";
        /// <summary>
        /// 使用四个空格表示的Tap
        /// </summary>
        public static readonly string BlanksOfOneTap = new string(' ', 4);

        /// <summary>
        /// 获取枚举类型所有值，并以逗号分隔
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetEnumNames<T>()
        {
            return string.Join(",", Enum.GetNames(typeof(T)));
        }

        /// <summary>
        /// 把Tap缩进表示为空格（一个缩进用4个空格代替）字符串；
        /// 如果缩进数小于等于0，则返回空字符串
        /// </summary>
        /// <param name="nIndent_"></param>
        /// <returns></returns>
        public static string TapIndent2Blanks(int nIndent_)
        {
            if(nIndent_<=0)
                return string.Empty;

            return new string(' ', 4 * nIndent_);
        }

        const int TapDefSize = 4;
        /// <summary>
        /// 左对齐字符；
        /// 不足处使用Tap（假设为4空格）填充；
        /// 如果nWidth小于strInfo长度，直接返回strInfo。
        /// </summary>
        /// <param name="strInfo_"></param>
        /// <param name="nWidth_"></param>
        /// <param name="nTapSize_">Tap（'\t'）的大小（默认为4各空格）</param>
        /// <returns></returns>
        public static string AlignLeft(string strInfo_, int nWidth_, int nTapSize_=TapDefSize)
        {
            int nRealWidth = nWidth_ - strInfo_.Length;
            if(nRealWidth<=0)
                return strInfo_;
            if (nTapSize_ <= 0)
                nTapSize_ = TapDefSize;

            int nTaps = nRealWidth / nTapSize_;
            if (nRealWidth % nTapSize_ != 0)
                nTaps += 1;

            return string.Format("{0}{1}", strInfo_, new string('\t', nTaps));
        }

        /// <summary>
        /// 字符串输出，转义字符直接输出（如：\n直接输出为\与n两个字符而非换行）
        /// </summary>
        /// <returns></returns>
        public static string PrintInEscap(string strPrint_)
        {
            return Regex.Escape(strPrint_);
        }

        /// <summary>
        /// 从字符串中移除指定的后缀字符串：
        /// TrimAll是否移除所有后缀：true递归移除所有，false只移除最后一个后缀。
        /// </summary>
        /// <param name="strSrc_"></param>
        /// <param name="strTrimed_"></param>
        /// <param name="bTrimAll_"></param>
        /// <param name="bIgnoreCase_"></param>
        /// <returns></returns>
        public static string TrimEnd(string strSrc_, string strTrimed_, bool bTrimAll_ = false, bool bIgnoreCase_=true)
        {
            if (string.IsNullOrEmpty(strSrc_))
                return string.Empty;
            if (string.IsNullOrEmpty(strTrimed_))
                return strSrc_;

            StringComparison strCmp = StringComparison.Ordinal;
            if(bIgnoreCase_)
                strCmp = StringComparison.OrdinalIgnoreCase;
            while (strSrc_.EndsWith(strTrimed_, strCmp))
            {
                if (strSrc_.Length == strTrimed_.Length)
                    return string.Empty;
                strSrc_ = strSrc_.Remove(strSrc_.Length - strTrimed_.Length);

                if (!bTrimAll_)
                    break;
            }

            return strSrc_;
        }

        /// <summary>
        /// 输出字符串（超过最大长度则截断并添加...)
        /// </summary>
        /// <param name="strPrint_"></param>
        /// <param name="nMaxLen_"></param>
        /// <returns></returns>
        public static string PrintLimit(string strPrint_, int nMaxLen_=20)
        {
            if (string.IsNullOrEmpty(strPrint_))
                return string.Empty;

            nMaxLen_ -= 3;
            if (nMaxLen_ <= 0)
                nMaxLen_ = 1;
            if (strPrint_.Length <= nMaxLen_)
                return strPrint_;

            return strPrint_.Substring(0, nMaxLen_) + "...";
        }
    }
}
