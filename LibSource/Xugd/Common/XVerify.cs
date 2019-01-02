using System;
using System.Text.RegularExpressions;

namespace SHCre.Xugd.Common
{
    /// <summary>
    /// �����ݸ�ʽ��Ч�Խ�����֤
    /// </summary>
    public static class XVerify
    {
        static readonly string _strDomainPostfix = @"\.("
                    + "(com)"       // ��ҵ�������κθ��˶�����ע��
                    + "|(edu)"      // ��������
                    + "|(gov)"      // ��������
                    + "|(int)"      // ������֯
                    + "|(mil)"      // �������²���
                    + "|(net)"      // ������֯
                    + "|(org)"      // ��ӯ����֯
                    + "|(biz)"      // ��ҵ
                    + "|(info)"     // ������Ϣ������֯
                    + "|(pro)"      // ���ڻ�ơ���ʦ��ҽ��
                    + "|(name)"     // ���ڸ���
                    + "|(museum)"   // ���ڲ����
                    + "|(coop)"     // ������ҵ��������
                    + "|(aero)"     // ���ں��չ�ҵ
                    + "|(xxx)"      // ���ڳ���
                    + "|([a-z]{2})" //����������һ��Ϊ�����ַ���
                    + ")"; 
        /// <summary>
        /// �жϵ����ʼ���ַ��ʽ�Ƿ���ȷ������Ϊ��׼������׺����com�Ȼ��������cn�ȣ�
        /// </summary>
        /// <param name="strAddr_">�����ʼ���ַ</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
        public static bool IsEmail(string strAddr_)
        {
            string strName = @"\w+([-.+]\w+)*";
            return Regex.IsMatch(strAddr_, string.Format("^{0}@{0}{1}$", strName, _strDomainPostfix), RegexOptions.IgnoreCase);
        }

        static readonly string _strCountryCode = @"(((([+0]?86)|(\([+0]?86\)))[-\s]*)|0)?";   // ���Ҵ���+86��(+86) �����0            
        /// <summary>
        /// �ж��Ƿ�Ϊ�й���½�������۰ģ������ĵ绰���룺�������Ҵ��루086��[��ѡ]��
        /// ����[��ѡ](010, 02*, 0[3~9]**, (00)852/853)���绰���루7λ��8λ��
        /// �ͷֻ��ţ�1~4λ��[��ѡ]���绰������ֻ���֮�����ʹ�á�-��ո񡯽��зָ�
        /// </summary>
        /// <param name="strPhone_">�绰����</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
        public static bool IsPhone(string strPhone_)
        {
            string strDistrict = string.Format(@"(({0}|(\({0}\)))[-\s]*)?",@"(10|2[0-9]|(0)?85[2-3]|[3-9]\d{2})");
            string strNum = @"\d{7,8}([-\s]+\d{1,4})?";
            string strReg = string.Format("^{0}{1}{2}$",_strCountryCode, strDistrict, strNum);
            return Regex.IsMatch(strPhone_, strReg);
        }

        /// <summary>
        /// �ж��Ƿ�Ϊ�й���½�������ֻ����룺�������Ҵ��루086��[��ѡ]���ֻ����루13*, 14*, 15*, 17*, 18*��
        /// </summary>
        /// <param name="strMobile_">�ֻ�����</param>
        /// <returns>��ȷ������true�����򣬷���false</returns>
        public static bool IsMobile(string strMobile_)
        {
            string strNum = @"1[3-5,7,8]\d{9}";  // �ֻ��ţ�13*, 14*, 15*, 17*, 18*)
            return Regex.IsMatch(strMobile_, string.Format("^{0}{1}$", _strCountryCode, strNum));
        }

        /// <summary>
        /// �ж�IPv4��ַ�Ƿ��Ǿ�������ַ��10/8��192.168/16��127/8��172.16/12��192.0.2/24��169.254/16��
        /// </summary>
        /// <param name="strIp_"></param>
        /// <returns></returns>
        public static bool IsLanIPv4(string strIp_)
        {
            return strIp_.StartsWith("10.") ||      /* 10.0.0.0        -   10.255.255.255  (10/8 prefix)       */
            strIp_.StartsWith("192.168.") ||        /* 192.168.0.0     -   192.168.255.255 (192.168/16 prefix) */
            strIp_.StartsWith("127.") ||            /* 127.0.0.0       -   127.255.255.255 (127/8 prefix)      */
            strIp_.StartsWith("255.") ||
            strIp_.StartsWith("0.") ||
            strIp_.StartsWith("1.") ||
            strIp_.StartsWith("2.") ||
            strIp_.StartsWith("172.16.") ||         /* 172.16.0.0      -   172.31.255.255  (172.16/12 prefix)  */
            strIp_.StartsWith("172.17.") ||
            strIp_.StartsWith("172.18.") ||
            strIp_.StartsWith("172.19.") ||
            strIp_.StartsWith("172.20.") ||
            strIp_.StartsWith("172.21.") ||
            strIp_.StartsWith("172.22.") ||
            strIp_.StartsWith("172.23.") ||
            strIp_.StartsWith("172.24.") ||
            strIp_.StartsWith("172.25.") ||
            strIp_.StartsWith("172.26.") ||
            strIp_.StartsWith("172.27.") ||
            strIp_.StartsWith("172.28.") ||
            strIp_.StartsWith("172.29.") ||
            strIp_.StartsWith("172.30.") ||
            strIp_.StartsWith("172.31.") ||
            strIp_.StartsWith("192.0.2.") ||        /* 192.0.2.0       -   192.0.2.255      (192.0.2/24 prefix) */
            strIp_.StartsWith("169.254.");          /* 169.254.0.0     -   169.254.255.255  (169.254/16 prefix) */
        
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
        /// �ж��Ƿ�Ϊ�Ϸ���IP��ַ��IPv6��IPv4��
        /// </summary>
        /// <param name="strIP_">Ҫ�жϵ�IP��ַ�ַ�����ʾ��ʽ</param>
        /// <returns>�ǺϷ�IP��ַ������true�����򣬷���false</returns>
        public static bool IsIPAddr(string strIP_)
        {
            System.Net.IPAddress ipAddr;
            return System.Net.IPAddress.TryParse (strIP_, out ipAddr);
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
