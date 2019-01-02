using System;
using System.Collections.Generic;
using System.Text;

namespace SHCre.Xugd.Common
{
    /// <summary>
    /// 获取随机数
    /// </summary>
    public static class XRandom
    {
        static Random _rand = new Random();

        /// <summary>
        /// 获取指定范围内[MinValue, MaxValue)的整数
        /// </summary>
        /// <param name="nMinValue_"></param>
        /// <param name="nMaxValue_"></param>
        /// <returns></returns>
        public static int GetInt(int nMinValue_=0, int nMaxValue_=int.MaxValue)
        {
            if (nMinValue_ > nMaxValue_)
                nMaxValue_ = nMaxValue_ + 1;

            return _rand.Next(nMinValue_, nMaxValue_);
        }

        /// <summary>
        /// 随机的返回true/false
        /// </summary>
        /// <returns></returns>
        public static bool GetBool()
        {
            return _rand.Next(0, 2) == 0;
        }

        /// <summary>
        /// 获取指定长度的随机数
        /// </summary>
        /// <param name="nLen_"></param>
        /// <returns></returns>
        public static byte[] GetBytes(int nLen_)
        {
            if (nLen_ <= 0)
                nLen_ = 1;

            byte[] byBuff = new byte[nLen_];
            _rand.NextBytes(byBuff);

            return byBuff;
        }

        /// <summary>
        /// 获取包括英文字符数字以及标点符号组成的随机Ascii字符串
        /// </summary>
        /// <param name="nLen_"></param>
        /// <returns></returns>
        public static string GetString(int nLen_)
        {
            if (nLen_ <= 0)
                nLen_ = 1;

            char[] strBuff = new char[nLen_];
            int i = 0;
            while(i<nLen_)
            {
                char ch = (char)_rand.Next(32, 127); // [BlankSpace, ~]
                // if(char.IsLetterOrDigit(ch) || char.IsPunctuation(ch))
                {
                    strBuff[i] = ch;
                    ++i;
                }
            }

            return new string(strBuff);
        }

        static char GetLetterOrNumber(bool bHasNumber_)
        {
            int nStart = bHasNumber_ ? 0 : 10;
            int nGet = _rand.Next(nStart, 63);   // 0~9,A~Z, a~z, _
            if (nGet < 10) // 0~9
                return (char)(nGet + 48);   // '0' = 48
            if (nGet < 36)   // A~Z
                return (char)(nGet - 10 + 65);  // 'A'=65
            if (nGet == 62)
                return '_';
            return (char)(nGet - 36 + 97);      // 'a'=97
        }

        /// <summary>
        /// 获取标识符（以字符或下划线开始，包括英文字符、数字组成的随机Ascii字符串）
        /// </summary>
        /// <param name="nLen_"></param>
        /// <returns></returns>
        public static string GetSymbol(int nLen_)
        {
            if (nLen_ <= 0)
                nLen_ = 1;

            char[] strBuff = new char[nLen_];
            strBuff[0] = GetLetterOrNumber(false);
            int i = 1;
            while (i < nLen_)
            {
                strBuff[i] = GetLetterOrNumber(true);
                ++i;
            }

            return new string(strBuff);
        }
    }
}
