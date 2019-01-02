using System;
using System.Text.RegularExpressions;

namespace SHCre.CXugd.Common
{
    /// <summary>
    /// ���ַ�����ʽ��Ч����֤
    /// </summary>
    public static class XVerify
    {
        /// <summary>
        /// �жϵ����ʼ���ַ��ʽ�Ƿ���ȷ������Ϊ��׼������׺����com�Ȼ��������cn�ȣ�
        /// </summary>
        /// <param name="strAddr_">�����ʼ���ַ</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
        public static bool IsEmail(string strAddr_)
        {
            string strDomainPosfix = @"\.((com)|(edu)|(gov)|(int)|(mil)|(net)|(org)|([a-z]{2}))";  // ����������׺����������ң���Ϊ�����ַ�����
            string strName = @"\w+([-.+]\w+)*";
            return Regex.IsMatch(strAddr_, string.Format("^{0}@{0}{1}$", strName, strDomainPosfix), RegexOptions.IgnoreCase);
        }

        static readonly string _strCountryCode = @"((([+0]?86)|(\([+0]?86\)))[-\s]*)?";   // ���Ҵ���+86��(+86)            
        /// <summary>
        /// �ж��Ƿ�Ϊ�й���½�������۰ģ������ĵ绰���룺�������Ҵ��루086��[��ѡ]������[��ѡ]���绰���루7λ��8λ���ͷֻ��ţ�1~4λ��[��ѡ]
        /// </summary>
        /// <param name="strPhone_">�绰����</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
        public static bool IsPhone(string strPhone_)
        {
            string strDistrict = string.Format("(({0}|(\\({0}\\)))[-\\s]*)?",@"(010|02[1-9]|0085[2-3]|0[3-9][1-9]{2})");
            string strNum = @"\d{7,8}([-\s]*\d{1,4})?";
            string strReg = string.Format("^{0}{1}{2}$",_strCountryCode, strDistrict, strNum);
            return Regex.IsMatch(strPhone_, strReg);
        }

        /// <summary>
        /// �ж��Ƿ�Ϊ�й���½�������ֻ����룺�������Ҵ��루086��[��ѡ]���ֻ����루13*,15*,180*,185~189*��
        /// </summary>
        /// <param name="strPhone_">�ֻ�����</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
        public static bool IsMobile(string strMobile_)
        {
            string strNum = @"((13[0-9])|(15[0-9])|(18[0,5-9]))\d{8}";  // �ֻ��ţ�13*, 15*, 180*, 185*~189*)
            return Regex.IsMatch(strMobile_, string.Format("^{0}{1}$", _strCountryCode, strNum));
        }

        /// <summary>
        /// �ж��Ƿ�ΪIPv4��ַ
        /// </summary>
        /// <param name="strIP_">Ip��ַ</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
        public static bool IsIPv4(string strIP_)
        {
            string strIPNum = @"([01]?\d\d?|2[0-4]\d|25[0-5])"; // 0~255
            return Regex.IsMatch(strIP_, string.Format("^({0}\\.){{3}}{0}$", strIPNum));
        }

        /// <summary>
        /// �ж��Ƿ�Ϊʮ��������
        /// </summary>
        /// <param name="strInput_">ʮ������</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
        public static bool IsDigit(string strInput_)
        {
            return Regex.IsMatch(strInput_, @"^\d+$");
        }

        /// <summary>
        /// �ж��Ƿ�Ϊָ��λ��ʮ��������
        /// </summary>
        /// <param name="strInput_">ʮ������</param>
        /// <param name="nCount_">λ��</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
        public static bool IsDigit(string strInput_, int nCount_)
        {
            string strMatch = @"^\d{" + nCount_.ToString() + "}$";
            return Regex.IsMatch(strInput_, strMatch);
        }

        /// <summary>
        /// �ж��Ƿ�Ϊָ����Χ�ڵ�ʮ��������
        /// </summary>
        /// <param name="strInput_">ʮ������</param>
        /// <param name="nMin_">��Сλ��</param>
        /// <param name="nMax_">���λ��</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
        public static bool IsDigit(string strInput_, int nMin_, int nMax_)
        {
            string strMatch = @"^\d{" + nMin_.ToString() + "," + nMax_.ToString() + "}$";
            return Regex.IsMatch(strInput_, strMatch);
        }

        /// <summary>
        /// �ж��Ƿ�ΪӢ���ַ���
        /// </summary>
        /// <param name="strInput_">Ӣ���ַ���</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
        public static bool IsAlpha(string strInput_)
        {
            return Regex.IsMatch(strInput_, @"^[a-zA-Z]+$");
        }

        /// <summary>
        /// �ж��Ƿ�Ϊָ��λ����Ӣ���ַ���
        /// </summary>
        /// <param name="strInput_">Ӣ���ַ���</param>
        /// <param name="nCount_">λ��</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
        public static bool IsAlpha(string strInput_, int nCount_)
        {
            string strMatch = @"^[a-zA-Z]{" + nCount_.ToString() + "}$";
            return Regex.IsMatch(strInput_, strMatch);
        }

        /// <summary>
        /// �ж��Ƿ�Ϊָ����Χ�ڵ�Ӣ���ַ���
        /// </summary>
        /// <param name="strInput_">Ӣ���ַ���</param>
        /// <param name="nMin_">��С����</param>
        /// <param name="nMax_">��󳤶�</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
        public static bool IsAlpha(string strInput_, int nMin_, int nMax_)
        {
            string strMatch = @"^[a-zA-Z]{" + nMin_.ToString() + "," + nMax_.ToString() + "}$";
            return Regex.IsMatch(strInput_, strMatch);
        }

        static readonly string _strIsHex = @"^(0x|0X)?[0-9a-fA-F]";
        /// <summary>
        /// �ж��Ƿ�Ϊʮ�����������ɰ���ǰ��0x
        /// </summary>
        /// <param name="strInput_">ʮ��������</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
        public static bool IsHex(string strInput_)
        {
            return Regex.IsMatch(strInput_, _strIsHex + "+$");
        }

        /// <summary>
        /// �ж��Ƿ�Ϊָ��λ��ʮ�����������ɰ���ǰ��0x
        /// </summary>
        /// <param name="strInput_">ʮ��������</param>
        /// <param name="nCount_">λ��</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
        public static bool IsHex(string strInput_, int nCount_)
        {
            string strMatch = _strIsHex + "{" + nCount_.ToString() + "}$";
            return Regex.IsMatch(strInput_, strMatch);
        }

        /// <summary>
        /// �ж��Ƿ�Ϊָ����Χ�ڵ�ʮ�����������ɰ���ǰ��0x
        /// </summary>
        /// <param name="strInput_">ʮ��������</param>
        /// <param name="nMin_">��С����</param>
        /// <param name="nMax_">��󳤶�</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
        public static bool IsHex(string strInput_, int nMin_, int nMax_)
        {
            string strMatch = _strIsHex + "{" + nMin_.ToString() + "," + nMax_.ToString() + "}$";
            return Regex.IsMatch(strInput_, strMatch);
        }

        /// <summary>
        /// �ж��Ƿ�Ϊ��ʶ�������ַ���_��ʼ�����԰����ַ�����_-.+
        /// </summary>
        /// <param name="strInput_">��ʶ��</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
        public static bool IsSymbol(string strInput_)
        {
            string _strIsSymbol = @"^[_a-zA-Z]+[-_.+a-zA-Z0-9]*$";
            return Regex.IsMatch(strInput_, _strIsSymbol);
        }

        /// <summary>
        /// �ж��Ƿ�Ϊָ�����ȵı�ʶ�������ַ���_��ʼ�����԰����ַ�����_-.+
        /// </summary>
        /// <param name="strInput_">��ʶ��</param>
        /// <param name="nCount_">����</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
        public static bool IsSymbol(string strInput_, int nCount_)
        {
            if (strInput_.Length != nCount_)
                return false;

            return IsSymbol(strInput_);
        }

        /// <summary>
        /// �ж��Ƿ�Ϊָ�����ȷ�Χ�ڵı�ʶ�������ַ���_��ʼ�����԰����ַ�����_-.+
        /// </summary>
        /// <param name="strInput_">��ʶ��</param>
        /// <param name="nMin_">��С����</param>
        /// <param name="nMax_">��󳤶�</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
        public static bool IsSymbol(string strInput_, int nMin_, int nMax_)
        {
            if( (strInput_.Length < nMin_) || (strInput_.Length > nMax_))
                return false;

            return IsSymbol(strInput_);
        }

        static readonly string _strIsSN = @"^[-_a-zA-Z0-9]";
        /// <summary>
        /// �ж��Ƿ�Ϊ���кŻ��ţ������ַ��������Լ�-_
        /// </summary>
        /// <param name="strInput_">���кŻ���</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
        public static bool IsSN(string strInput_)
        {
            return Regex.IsMatch(strInput_, _strIsSN + "+$");
        }

        /// <summary>
        /// �ж��Ƿ�Ϊָ�����ȵ����кŻ��ţ������ַ��������Լ�-_
        /// </summary>
        /// <param name="strInput_">���кŻ���</param>
        /// <param name="nCount_">����</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
        public static bool IsSN(string strInput_, int nCount_)
        {
            string strMatch = _strIsSN + "{" + nCount_.ToString() + "}$";

            return Regex.IsMatch(strInput_, strMatch);
        }

        /// <summary>
        /// ���Ƿ�Ϊָ�����ȷ�Χ�ڵ����кŻ��ţ������ַ��������Լ�-_
        /// </summary>
        /// <param name="strInput_">���кŻ���</param>
        /// <param name="nMin_">��󳤶�</param>
        /// <param name="nMax_">��С����</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
        public static bool IsSN(string strInput_, int nMin_, int nMax_)
        {
            string strMatch = _strIsSN + "{" + nMin_.ToString() + "," + nMax_.ToString() + "}$";

            return Regex.IsMatch(strInput_, strMatch);
        }

        /// <summary>
        /// �ж��Ƿ�Ϊ����·���������̷��������ж�·���Ƿ���ʵ����
        /// </summary>
        /// <param name="strPath_">Ҫ�жϵ�·��</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
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
        /// �ж��Ƿ�Ϊ��Ч·�������ж�·���Ƿ���ʵ����
        /// </summary>
        /// <param name="strPath_">Ҫ�жϵ�·��</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
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
