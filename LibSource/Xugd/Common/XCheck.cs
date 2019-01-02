using System;
using System.Text.RegularExpressions;

namespace SHCre.CXugd.Common
{
    /// <summary>
    /// 对字符串格式有效性验证
    /// </summary>
    public static class XVerify
    {
        /// <summary>
        /// 判断电子邮件地址格式是否正确，必须为标准域名后缀（如com等或国家域名cn等）
        /// </summary>
        /// <param name="strAddr_">电子邮件地址</param>
        /// <returns>正确，返回true；否则，返回false</returns>
        public static bool IsEmail(string strAddr_)
        {
            string strDomainPosfix = @"\.((com)|(edu)|(gov)|(int)|(mil)|(net)|(org)|([a-z]{2}))";  // 所有域名后缀（公共与国家（都为两个字符））
            string strName = @"\w+([-.+]\w+)*";
            return Regex.IsMatch(strAddr_, string.Format("^{0}@{0}{1}$", strName, strDomainPosfix), RegexOptions.IgnoreCase);
        }

        static readonly string _strCountryCode = @"((([+0]?86)|(\([+0]?86\)))[-\s]*)?";   // 国家代码+86或(+86)            
        /// <summary>
        /// 判断是否为中国大陆（包括港澳）地区的电话号码：包括国家代码（086）[可选]、区号[可选]、电话号码（7位或8位）和分机号（1~4位）[可选]
        /// </summary>
        /// <param name="strPhone_">电话号码</param>
        /// <returns>正确，返回true；否则，返回false</returns>
        public static bool IsPhone(string strPhone_)
        {
            string strDistrict = string.Format("(({0}|(\\({0}\\)))[-\\s]*)?",@"(010|02[1-9]|0085[2-3]|0[3-9][1-9]{2})");
            string strNum = @"\d{7,8}([-\s]*\d{1,4})?";
            string strReg = string.Format("^{0}{1}{2}$",_strCountryCode, strDistrict, strNum);
            return Regex.IsMatch(strPhone_, strReg);
        }

        /// <summary>
        /// 判断是否为中国大陆地区的手机号码：包括国家代码（086）[可选]和手机号码（13*,15*,180*,185~189*）
        /// </summary>
        /// <param name="strPhone_">手机号码</param>
        /// <returns>正确，返回true；否则，返回false</returns>
        public static bool IsMobile(string strMobile_)
        {
            string strNum = @"((13[0-9])|(15[0-9])|(18[0,5-9]))\d{8}";  // 手机号（13*, 15*, 180*, 185*~189*)
            return Regex.IsMatch(strMobile_, string.Format("^{0}{1}$", _strCountryCode, strNum));
        }

        /// <summary>
        /// 判断是否为IPv4地址
        /// </summary>
        /// <param name="strIP_">Ip地址</param>
        /// <returns>正确，返回true；否则，返回false</returns>
        public static bool IsIPv4(string strIP_)
        {
            string strIPNum = @"([01]?\d\d?|2[0-4]\d|25[0-5])"; // 0~255
            return Regex.IsMatch(strIP_, string.Format("^({0}\\.){{3}}{0}$", strIPNum));
        }

        /// <summary>
        /// 判断是否为十进制数字
        /// </summary>
        /// <param name="strInput_">十进制数</param>
        /// <returns>正确，返回true；否则，返回false</returns>
        public static bool IsDigit(string strInput_)
        {
            return Regex.IsMatch(strInput_, @"^\d+$");
        }

        /// <summary>
        /// 判断是否为指定位数十进制数字
        /// </summary>
        /// <param name="strInput_">十进制数</param>
        /// <param name="nCount_">位数</param>
        /// <returns>正确，返回true；否则，返回false</returns>
        public static bool IsDigit(string strInput_, int nCount_)
        {
            string strMatch = @"^\d{" + nCount_.ToString() + "}$";
            return Regex.IsMatch(strInput_, strMatch);
        }

        /// <summary>
        /// 判断是否为指定范围内的十进制数字
        /// </summary>
        /// <param name="strInput_">十进制数</param>
        /// <param name="nMin_">最小位数</param>
        /// <param name="nMax_">最大位数</param>
        /// <returns>正确，返回true；否则，返回false</returns>
        public static bool IsDigit(string strInput_, int nMin_, int nMax_)
        {
            string strMatch = @"^\d{" + nMin_.ToString() + "," + nMax_.ToString() + "}$";
            return Regex.IsMatch(strInput_, strMatch);
        }

        /// <summary>
        /// 判断是否为英文字符串
        /// </summary>
        /// <param name="strInput_">英文字符串</param>
        /// <returns>正确，返回true；否则，返回false</returns>
        public static bool IsAlpha(string strInput_)
        {
            return Regex.IsMatch(strInput_, @"^[a-zA-Z]+$");
        }

        /// <summary>
        /// 判断是否为指定位数的英文字符串
        /// </summary>
        /// <param name="strInput_">英文字符串</param>
        /// <param name="nCount_">位数</param>
        /// <returns>正确，返回true；否则，返回false</returns>
        public static bool IsAlpha(string strInput_, int nCount_)
        {
            string strMatch = @"^[a-zA-Z]{" + nCount_.ToString() + "}$";
            return Regex.IsMatch(strInput_, strMatch);
        }

        /// <summary>
        /// 判断是否为指定范围内的英文字符串
        /// </summary>
        /// <param name="strInput_">英文字符串</param>
        /// <param name="nMin_">最小长度</param>
        /// <param name="nMax_">最大长度</param>
        /// <returns>正确，返回true；否则，返回false</returns>
        public static bool IsAlpha(string strInput_, int nMin_, int nMax_)
        {
            string strMatch = @"^[a-zA-Z]{" + nMin_.ToString() + "," + nMax_.ToString() + "}$";
            return Regex.IsMatch(strInput_, strMatch);
        }

        static readonly string _strIsHex = @"^(0x|0X)?[0-9a-fA-F]";
        /// <summary>
        /// 判断是否为十六进制数：可包括前导0x
        /// </summary>
        /// <param name="strInput_">十六进制数</param>
        /// <returns>正确，返回true；否则，返回false</returns>
        public static bool IsHex(string strInput_)
        {
            return Regex.IsMatch(strInput_, _strIsHex + "+$");
        }

        /// <summary>
        /// 判断是否为指定位数十六进制数：可包括前导0x
        /// </summary>
        /// <param name="strInput_">十六进制数</param>
        /// <param name="nCount_">位数</param>
        /// <returns>正确，返回true；否则，返回false</returns>
        public static bool IsHex(string strInput_, int nCount_)
        {
            string strMatch = _strIsHex + "{" + nCount_.ToString() + "}$";
            return Regex.IsMatch(strInput_, strMatch);
        }

        /// <summary>
        /// 判断是否为指定范围内的十六进制数：可包括前导0x
        /// </summary>
        /// <param name="strInput_">十六进制数</param>
        /// <param name="nMin_">最小长度</param>
        /// <param name="nMax_">最大长度</param>
        /// <returns>正确，返回true；否则，返回false</returns>
        public static bool IsHex(string strInput_, int nMin_, int nMax_)
        {
            string strMatch = _strIsHex + "{" + nMin_.ToString() + "," + nMax_.ToString() + "}$";
            return Regex.IsMatch(strInput_, strMatch);
        }

        /// <summary>
        /// 判断是否为标识符：以字符或_开始，可以包括字符数字_-.+
        /// </summary>
        /// <param name="strInput_">标识符</param>
        /// <returns>正确，返回true；否则，返回false</returns>
        public static bool IsSymbol(string strInput_)
        {
            string _strIsSymbol = @"^[_a-zA-Z]+[-_.+a-zA-Z0-9]*$";
            return Regex.IsMatch(strInput_, _strIsSymbol);
        }

        /// <summary>
        /// 判断是否为指定长度的标识符：以字符或_开始，可以包括字符数字_-.+
        /// </summary>
        /// <param name="strInput_">标识符</param>
        /// <param name="nCount_">长度</param>
        /// <returns>正确，返回true；否则，返回false</returns>
        public static bool IsSymbol(string strInput_, int nCount_)
        {
            if (strInput_.Length != nCount_)
                return false;

            return IsSymbol(strInput_);
        }

        /// <summary>
        /// 判断是否为指定长度范围内的标识符：以字符或_开始，可以包括字符数字_-.+
        /// </summary>
        /// <param name="strInput_">标识符</param>
        /// <param name="nMin_">最小长度</param>
        /// <param name="nMax_">最大长度</param>
        /// <returns>正确，返回true；否则，返回false</returns>
        public static bool IsSymbol(string strInput_, int nMin_, int nMax_)
        {
            if( (strInput_.Length < nMin_) || (strInput_.Length > nMax_))
                return false;

            return IsSymbol(strInput_);
        }

        static readonly string _strIsSN = @"^[-_a-zA-Z0-9]";
        /// <summary>
        /// 判断是否为序列号或编号：包括字符、数组以及-_
        /// </summary>
        /// <param name="strInput_">序列号或编号</param>
        /// <returns>正确，返回true；否则，返回false</returns>
        public static bool IsSN(string strInput_)
        {
            return Regex.IsMatch(strInput_, _strIsSN + "+$");
        }

        /// <summary>
        /// 判断是否为指定长度的序列号或编号：包括字符、数组以及-_
        /// </summary>
        /// <param name="strInput_">序列号或编号</param>
        /// <param name="nCount_">长度</param>
        /// <returns>正确，返回true；否则，返回false</returns>
        public static bool IsSN(string strInput_, int nCount_)
        {
            string strMatch = _strIsSN + "{" + nCount_.ToString() + "}$";

            return Regex.IsMatch(strInput_, strMatch);
        }

        /// <summary>
        /// 断是否为指定长度范围内的序列号或编号：包括字符、数组以及-_
        /// </summary>
        /// <param name="strInput_">序列号或编号</param>
        /// <param name="nMin_">最大长度</param>
        /// <param name="nMax_">最小长度</param>
        /// <returns>正确，返回true；否则，返回false</returns>
        public static bool IsSN(string strInput_, int nMin_, int nMax_)
        {
            string strMatch = _strIsSN + "{" + nMin_.ToString() + "," + nMax_.ToString() + "}$";

            return Regex.IsMatch(strInput_, strMatch);
        }

        /// <summary>
        /// 判断是否为完整路径（包括盘符），不判断路径是否真实存在
        /// </summary>
        /// <param name="strPath_">要判断的路径</param>
        /// <returns>正确，返回true；否则，返回false</returns>
        public static bool IsFullPath(string strPath_)
        {
            try 
            {
                return (strPath_ == System.IO.Path.GetFullPath(strPath_));
            }
            catch(Exception){}

            return false;
        }

        /// <summary>
        /// 判断是否为有效路径，不判断路径是否真实存在
        /// </summary>
        /// <param name="strPath_">要判断的路径</param>
        /// <returns>正确，返回true；否则，返回false</returns>
        public static bool IsValidPath(string strPath_)
        {
            try 
            {
                string strFull = System.IO.Path.GetFullPath(strPath_);
                return strFull.EndsWith(strPath_);
            }
            catch (Exception) { }

            return false;
        }
    };
}
