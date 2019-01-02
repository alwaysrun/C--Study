using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.International.Converters.PinYinConverter;

namespace SHCre.Xugd.Chars
{
    /// <summary>
    /// 处理有关汉字的类
    /// </summary>
    public static class Chinese
    {

        /// <summary>
        /// 拼音的声调
        /// </summary>
        public enum Tones
        {
            /// <summary>
            /// 一声平，阴平
            /// </summary>
            Yinping = 1,
            /// <summary>
            /// 二声仰，阳平
            /// </summary>
            Yangping,
            /// <summary>
            /// 三声弯，上声
            /// </summary>
            Shangsheng,
            /// <summary>
            /// 四声降，去声
            /// </summary>
            Qusheng
        };

        /// <summary>
        /// 获取带有声调（用1、2、3、4代表四声）的拼音，
        /// 不是有效汉字时（字符、数字、空白都不看作汉字）会抛出NotSupportedException
        /// </summary>
        /// <param name="chHanzi_">要判断的汉字</param>
        /// <returns>汉字对应的所有拼音列表</returns>
        public static List<string> GetPinyinWithTone(char chHanzi_)
        {
            ChineseChar chChar = new ChineseChar(chHanzi_);
            return new List<string>(chChar.Pinyins);
        }

        /// <summary>
        /// 获取拼音，
        /// 不是有效汉字时（字符、数字、空白都不看作汉字）会抛出NotSupportedException
        /// </summary>
        /// <param name="chHanzi_">要判断的汉字</param>
        /// <returns>汉字对应的拼音</returns>
        public static string GetPinyin(char chHanzi_)
        {
            string strPinyin = GetPinyinWithTone(chHanzi_)[0];
            return strPinyin.Substring(0, strPinyin.Length - 1);
        }

        /// <summary>
        /// 获取带有声调的拼音，
        /// 不是有效汉字时（字符、数字、空白都不看作汉字）会抛出NotSupportedException
        /// </summary>
        /// <param name="chHanzi_">要判断的汉字</param>
        /// <param name="euTone_">拼音的声调</param>
        /// <returns>汉字对应拼音</returns>
        public static string GetPinyin(char chHanzi_, out Tones euTone_)
        {
            string strPinyin = GetPinyinWithTone(chHanzi_)[0];
            euTone_ = (Tones)int.Parse(strPinyin[strPinyin.Length - 1].ToString());
            return strPinyin.Substring(0, strPinyin.Length - 1);
        }

        /// <summary>
        /// 获取汉字字符串对应的拼音（不包括声调），拼音间用空格分割；
        /// 所有非汉字字符，直接作为拼音返回
        /// </summary>
        /// <param name="strHanzi_">汉字字符串</param>
        /// <returns>对应的拼音</returns>
        public static string GetPinyin(string strHanzi_)
        {
            StringBuilder sbPinyin = new StringBuilder(strHanzi_.Length * 3);
            foreach (char ch in strHanzi_.Trim())
            {
                if (char.IsPunctuation(ch) || char.IsWhiteSpace(ch))
                    sbPinyin.Append(ch);

                try 
                {
                    sbPinyin.Append(GetPinyin(ch) + " ");
                }
                catch
                {
                    sbPinyin.Append(ch);
                }
            }

            return sbPinyin.ToString().Trim();
        }

        /// <summary>
        /// 判断是否是有效的汉字(使用微软的库，起始速度比较慢)
        /// </summary>
        /// <param name="chHanzi_">要判断的汉字</param>
        /// <returns>是汉字，返回true；否则，返回false</returns>
        public static bool IsValidHanzi(char chHanzi_)
        {
            return ChineseChar.IsValidChar(chHanzi_);
        }

        /// <summary>
        /// 判断字符串中是否包含汉字(使用微软的库，起始速度比较慢)
        /// </summary>
        /// <param name="strHanzi_">要判断的字符串</param>
        /// <returns>包含汉字，返回true；否则，返回false</returns>
        public static bool HasHanzi(string strHanzi_)
        {
            foreach(char ch in strHanzi_.Trim())
            {
                if (IsValidHanzi(ch))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 判断是否是有效的Unicode汉字(使用编码范围：'\u4E00'~'\u9FA5'与'\uF900'~'\uFA2D')
        /// </summary>
        /// <param name="chHanzi_">要判断的汉字</param>
        /// <returns>是汉字，返回true；否则，返回false</returns>
        public static bool IsUnicodeHanzi(char chHanzi_)
        {
            return ((chHanzi_ >= '\u4E00') && (chHanzi_ <= '\u9FA5'))
                    || ((chHanzi_ >= '\uF900') && (chHanzi_ <= '\uFA2D'));
        }

        /// <summary>
        /// 判断字符串中是否包含汉字(使用编码范围：'\u4E00'~'\u9FA5'与'\uF900'~'\uFA2D')
        /// </summary>
        /// <param name="strHanzi_">要判断的字符串</param>
        /// <returns>包含汉字，返回true；否则，返回false</returns>
        public static bool HasUnicodeHanzi(string strHanzi_)
        {
            Regex regHanzi = new Regex(@"[\u4E00-\u9FA5\uF900-\uFA2D]+");
            return regHanzi.IsMatch(strHanzi_);
        }

        /// <summary>
        /// 判断是否是有效的拼音
        /// </summary>
        /// <param name="strPinyin_">拼音</param>
        /// <param name="euTone_">声调</param>
        /// <returns>是拼音，返回true；否则，返回false</returns>
        public static bool IsValidPinyin(string strPinyin_, Tones euTone_)
        {
            return ChineseChar.IsValidPinyin(strPinyin_.Trim() + ((int)euTone_).ToString());
        }
    }
}
