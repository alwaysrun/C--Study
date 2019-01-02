using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SHCre.Xugd.Common
{
    /// <summary>
    /// 用于转换的函数
    /// </summary>
    public static class XConvert
    {
        #region "Enum"
        /// <summary>
        /// 枚举转换为数字
        /// </summary>
        /// <param name="euData_"></param>
        /// <returns></returns>
        public static uint Enum2Uint(Enum euData_)
        {
            return Convert.ToUInt32(euData_);
        }

        /// <summary>
        /// 数字转换为枚举
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="nData_"></param>
        /// <returns></returns>
        public static U Uint2Enum<U>(uint nData_)
        {
            return (U)Enum.ToObject(typeof(U), nData_);
        }

        /// <summary>
        /// 把枚举类型转换为数字字符串
        /// </summary>
        /// <param name="euData_">要转换的值</param>
        /// <returns>对应的数字字符串</returns>
        public static string Enum2Digits(System.Enum euData_)
        {
            return euData_.ToString("d");
        }

        /// <summary>
        /// 把数字转换为枚举类型
        /// </summary>
        /// <typeparam name="U">类型</typeparam>
        /// <param name="strEnum_">要转换的字符串</param>
        /// <returns>枚举类型</returns>
        public static U Digits2Enum<U>(string strEnum_)
        {
            return (U)Enum.Parse(typeof(U), strEnum_);
        }

        /// <summary>
        /// 把枚举类型转换为名称字符串
        /// </summary>
        /// <param name="euData_">要转换的值</param>
        /// <returns>对应的名称</returns>
        public static string Enum2Name(System.Enum euData_)
        {
            return euData_.ToString();
        }

        /// <summary>
        /// 把名称转换为对应的枚举类型
        /// </summary>
        /// <typeparam name="U">枚举类型</typeparam>
        /// <param name="strName_">名称</param>
        /// <param name="bIgnoreCase_">转换时是否忽略大小写</param>
        /// <returns>枚举类型</returns>
        public static U Name2Enum<U>(string strName_, bool bIgnoreCase_=true)
        {
            return (U)(Enum.Parse(typeof(U), strName_, bIgnoreCase_));
        }
        #endregion
        
        #region "Xml Type Convert"
        /// <summary>
        /// 把Xml字符串转换为对应的类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strXml_"></param>
        /// <returns></returns>
        public static T Xml2Type<T>(string strXml_)where T:class
        {
            XmlSerializer xmlSerial = new XmlSerializer(typeof(T));
            T objGet = null;
            using (StringReader sRead = new StringReader(strXml_))
            {
                objGet = xmlSerial.Deserialize(sRead) as T;

                sRead.Close();
            }

            return objGet;
        }

        /// <summary>
        /// 把类转换为Xml字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tObj_"></param>
        /// <param name="bOmitDeclare_">是否去掉声明部分（即xml?...头）</param>
        /// <param name="bOmitNamespace_">是否去掉限定名的XML命名空间和前缀（即xmlns...）</param>
        /// <returns></returns>
        public static string Type2Xml<T>(T tObj_, bool bOmitDeclare_=true, bool bOmitNamespace_=true) where T : class
        {
            if (tObj_ == null)
                return string.Empty;

            XmlSerializer xmlSerial = new XmlSerializer(typeof(T));
            using (StringWriter sWrite = new StringWriter())
            {
                XmlWriterSettings setXml = new XmlWriterSettings();
                setXml.OmitXmlDeclaration = bOmitDeclare_; // Rid declaration(<xml?..>)
                setXml.Encoding = Encoding.UTF8;
                using (XmlWriter xmlWriter = XmlWriter.Create(sWrite, setXml))
                {
                    XmlSerializerNamespaces nsXml = new XmlSerializerNamespaces();
                    if (bOmitNamespace_)
                        nsXml.Add(string.Empty, string.Empty); // Rid the namesapce(xmlns)

                    xmlSerial.Serialize(xmlWriter, tObj_, nsXml);
                    return sWrite.ToString();
                }
            }
        }
        #endregion
        
        #region "Struct Type Convert"
        /// <summary>
        /// 把指定类型的数据转换为字符串
        /// </summary>
        /// <typeparam name="T">要转换的类型</typeparam>
        /// <param name="tData_">要转换的数据</param>
        /// <returns>转换后的字符串</returns>
        public static string Type2String<T>(T tData_) where T : struct
        {
            return Bytes2Base64String(Type2Bytes(tData_));
        }

        /// <summary>
        /// 把字符串（必须是原来Type2String转换获取的）转换为指定类型的数据
        /// </summary>
        /// <typeparam name="T">转换数据的类型</typeparam>
        /// <param name="strData_">要转换的字符串</param>
        /// <returns>转换后的数据</returns>
        public static T String2Type<T>(string strData_) where T : struct
        {
            return Bytes2Type<T>(Base64String2Bytes(strData_));
        }

        /// <summary>
        /// 把指定类型的数据转换为数组
        /// </summary>
        /// <typeparam name="T">要转换数据的类型</typeparam>
        /// <param name="tData_">要转换的数据</param>
        /// <returns>转换后的数组</returns>
        public static byte[] Type2Bytes<T>(T tData_) where T : struct
        {
            int nSize = Marshal.SizeOf(tData_);
            IntPtr ptrGlobal = Marshal.AllocHGlobal(nSize);
            byte[] byData = null;

            try 
            {
                Marshal.StructureToPtr(tData_, ptrGlobal, false);
                byData = new byte[nSize];
                Marshal.Copy(ptrGlobal, byData, 0, nSize);
            }
            finally
            {
                Marshal.FreeHGlobal(ptrGlobal);
            }

            return byData;
        }

        /// <summary>
        /// 把数组（必须是原来Type2Bytes转换获取的）转换为指定类型的数据
        /// </summary>
        /// <typeparam name="T">转换数据的类型</typeparam>
        /// <param name="byData_">要转换的数组</param>
        /// <returns>转换后的数据</returns>
        public static T Bytes2Type<T>(byte[] byData_) where T : struct
        {
            int nSize = Marshal.SizeOf(typeof(T));
            IntPtr ptrGlobal = Marshal.AllocHGlobal(nSize);

            try 
            {
                Marshal.Copy(byData_, 0, ptrGlobal, nSize);
                return (T)Marshal.PtrToStructure(ptrGlobal, typeof(T));
            }
            finally
            {
                Marshal.FreeHGlobal(ptrGlobal);
            }
        }
        #endregion

        #region "String-Bytes Convert"
        /// <summary>
        /// 把Unicode字符串编码为字节序列
        /// </summary>
        /// <param name="strSource_">Unicode字符串</param>
        /// <returns>编码后的字节序列</returns>
        public static byte[] UnicodeString2Bytes(string strSource_)
        {
            return Encoding.Unicode.GetBytes(strSource_);
        }

        /// <summary>
        /// 将一个字节序列解码为Unicode字符串
        /// </summary>
        /// <param name="bySource_">字节序列</param>
        /// <param name="nOffset_">第一个要解码的字节索引</param>
        /// <param name="nCount_">要解码的字节长度</param>
        /// <returns>解码后的Unicode字符串</returns>
        public static string Bytes2UnicodeString(byte[] bySource_, int nOffset_, int nCount_)
        {
            return Encoding.Unicode.GetString(bySource_, nOffset_, nCount_);
        }

        /// <summary>
        /// 将一个字节序列解码为Unicode字符串
        /// </summary>
        /// <param name="bySource_">字节序列号</param>
        /// <returns>解码后的Unicode字符串</returns>
        public static string Bytes2UnicodeString(byte[] bySource_)
        {
            return Encoding.Unicode.GetString(bySource_);
        }

        /// <summary>
        /// 将字节序列转换成16进制表示的字符串（每个字节转换为两个字符，如0会转换为"00"）
        /// </summary>
        /// <param name="bySource_">字节序列</param>
        /// <param name="nOffset">数组的起始位置</param>
        /// <param name="nCount_">要转换的长度</param>
        /// <returns>转换后的字符串</returns>
        public static string Bytes2HexString(byte[] bySource_, int nOffset, int nCount_)
        {
            char[] cHex ={ '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            char[] cDest = new char[nCount_ * 2];           
            for (int i = 0; i < bySource_.Length; i++)
            {
                cDest[2 * i] = cHex[(bySource_[nOffset+i] >> 4) & 0x0F];
                cDest[2 * i + 1] = cHex[bySource_[nOffset+i] & 0x0F];
            }            
            return new string(cDest);
        }

        /// <summary>
        /// 将字节序列转换成16进制表示的字符串（每个字节转换为两个字符，如0会转换为"00"）
        /// </summary>      
        /// <param name="bySource_">字节序列</param>       
        /// <returns>转换后的字符串</returns>
        public static string Bytes2HexString(byte[] bySource_)
        {
            return Bytes2HexString(bySource_, 0, bySource_.Length);
        }

        /// <summary>
        /// 将16进制的字符串（每两个字符表示一个字节（"00"~"FF"）），转换为字节序列（每两个字符，转换为一个字节0
        /// </summary>
        /// <param name="strHex_">16进制的字符串</param>
        /// <returns>转换后的字节序列</returns>
        public static byte[] HexString2Bytes(string strHex_)
        {
            int nBytesLen = strHex_.Length / 2;
            byte[] byHex = new byte[nBytesLen];
            for( int i=0 ; i<nBytesLen ; ++i )
            {
                byHex[i] = byte.Parse(strHex_.Substring(2 * i, 2));
            }

            return byHex;
        }

        /// <summary>
        /// 把字节序列转换为字符串（Base64编码）
        /// </summary>
        /// <param name="bySrc_">要转换的数组</param>
        /// <returns>转换后的字符串</returns>
        public static string Bytes2Base64String(byte[] bySrc_)
        {
            return Convert.ToBase64String(bySrc_);
        }

        /// <summary>
        /// 把字节序列转换为字符串（Base64编码）
        /// </summary>
        /// <param name="bySrc_">要转换的数组</param>
        /// <param name="nOffset_">要开始转换的偏移位置</param>
        /// <param name="nCount_">转换的长度</param>
        /// <returns>转换后的字符串</returns>
        public static string Bytes2Base64String(byte[] bySrc_, int nOffset_, int nCount_)
        {
            return Convert.ToBase64String(bySrc_, nOffset_, nCount_);
        }

        /// <summary>
        /// 把字符串（Base64编码）转换为字节序列
        /// </summary>
        /// <param name="strSrc_">要转换的字符串</param>
        /// <returns>转换后的数组</returns>
        public static byte[] Base64String2Bytes(string strSrc_)
        {
            return Convert.FromBase64String(strSrc_);
        }
        #endregion

        #region "Mac Addr"
        /// <summary>
        /// 网卡地址转换：从字节形式（AABBCCDDEEFF)转换为字符串形式（AA-BB-CC-DD-EE-FF)；
        ///		输入必须为6字节长的byte数组，输出为17个字符的string
        /// </summary>
        /// <returns>字符串形式的网卡mac地址</returns>
        public static string MacAddrBytes2String(byte[] byMacAddr_)
        {
            if (byMacAddr_.Length != CLen.MacAddrSize)
                throw new SHParamException("Mac Address bytes length must be 6", (int)SHErrorCode.InvalidParam);


            string strMac = string.Format("{0}", byMacAddr_[0].ToString("X2"));
            for (int i = 1; i < CLen.MacAddrSize; ++i)
            {
                strMac += string.Format("-{0}", byMacAddr_[i].ToString("X2"));
            }

            return strMac;
        }

        /// <summary>
        /// 网卡地址转换：从字符串形式（AA-BB-CC-DD-EE-FF)转换为字节形式（AABBCCDDEEFF)；
        ///		输入必须为为17个字符的string，输出6字节长的byte数组
        /// </summary>
        /// <returns>字节序列形式的网卡mac地址</returns>
        public static byte[] MacAddrString2Bytes(string strMacAddr_)
        {
            if (strMacAddr_.Length != CLen.MacAddrLen)
                throw new SHParamException("Mac Address string length must be 17", (int)SHErrorCode.InvalidParam);

            byte[] byMac = new byte[CLen.MacAddrSize];
            for (int i = 0; i < CLen.MacAddrSize; ++i)
            {
                byMac[i] = Byte.Parse(strMacAddr_.Substring(3 * i, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }

            return byMac;
        }
        #endregion

        #region "Ip Addr"
        /// <summary>
        /// 获取以字节序列表示的IP地址（IPv6或IPv4）
        /// </summary>
        /// <param name="strIpAddr_">字符串表示的IP地址（IPv6或IPv4）</param>
        /// <returns>字节序列表示的IP地址</returns>				
        public static byte[] IPAddrString2Bytes(string strIpAddr_)
        {
            return IPAddress.Parse(strIpAddr_).GetAddressBytes();
        }

        /// <summary>
        /// 获取以字符串表示的IP地址（IPv6或IPv4）
        /// </summary>
        /// <param name="byIpAddr_">字节序列表示的IP地址（IPv6或IPv4）</param>
        /// <returns>字符串表示的IP地址（IPv6或IPv4）</returns>				
        public static string IPAddrBytes2String(byte[] byIpAddr_)
        {
            IPAddress ipAddr = new IPAddress(byIpAddr_);
            return ipAddr.ToString();
        }

        /// <summary>
        /// 把字符串地址转为数字: 如"1.2.3.4"会转为0x4030201
        /// </summary>
        /// <param name="strIpAddr_"></param>
        /// <returns></returns>
        public static uint IPAddrString2Unit(string strIpAddr_)
        {
            var byIp = IPAddress.Parse(strIpAddr_).GetAddressBytes();
            return BitConverter.ToUInt32(byIp, 0);
        }

        //public static string IPAddrUnit2String(uint nIp_)
        //{
        //    IPAddress.Parse(nIp_);
        //}
        #endregion

        #region "Guid"
        /// <summary>
        /// 获取以字节序列表示的Guid
        /// </summary>
        /// <param name="strGuid_">字符串表示的Guid</param>
        /// <returns>字节序列表示的Guid</returns>				
        public static byte[] GuidString2Bytes(string strGuid_)
        {
            Guid gId = new Guid(strGuid_);
            return gId.ToByteArray();
        }

        /// <summary>
        /// 获取以字符串表示的Guid
        /// </summary>
        /// <param name="byGuid_">字节序列表示的Guid</param>
        /// <returns>字符串表示的Guid</returns>				
        public static string GuidBytes2String(byte[] byGuid_)
        {
            Guid gId = new Guid(byGuid_);
            return gId.ToString("B");
        }
        #endregion

        #region "Unit"
        /// <summary>
        /// 数字转换为十六进制字符串
        /// </summary>
        /// <param name="nNum_">要转换的数字</param>
        /// <param name="nDigits">数字的最少位数，如果大于实际的位数则在前面填充0
        /// （如0xa234：digit小于4时，输出A234；大于4时，在前面填充0，如6时输出为00A234）</param>
        /// <returns>转换后的字符串</returns>
        public static string Uint2HexString(uint nNum_, int nDigits=0)
        {
            string strFormat = string.Format("X{0}", nDigits);
            return nNum_.ToString(strFormat);
        }

        /// <summary>
        /// 十六进制字符串转换为数字
        /// </summary>
        /// <param name="strHex_">要转换的字符串</param>
        /// <returns>转换后的数字</returns>
        public static uint HexString2Uint(string strHex_)
        {
            if (strHex_.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                strHex_ = strHex_.Substring(2);

            return uint.Parse(strHex_, NumberStyles.HexNumber);
        }

        /// <summary>
        /// 数字转换为十进制字符串
        /// </summary>
        /// <param name="nNum_">要转换的数字</param>
        /// <param name="nDigits">数字的最少位数，如果大于实际的位数则在前面填充0
        /// （如1234：digit小于4时，输出1234；大于4时，在前面填充0，如6时输出为001234）</param>
        /// <returns>转换后的字符串</returns>
        public static string Uint2String(uint nNum_, int nDigits=0)
        {
            string strFormat = string.Format("D{0}", nDigits);
            return nNum_.ToString(strFormat);
        }

        /// <summary>
        /// 十进制字符串转换为数字
        /// </summary>
        /// <param name="strNum_">要转换的字符串</param>
        /// <returns>转换后的数字</returns>
        public static uint String2Unit(string strNum_)
        {
            return uint.Parse(strNum_, NumberStyles.Integer);
        }

        /// <summary>
        /// 字符数组转数字
        /// </summary>
        /// <param name="byNumber_"></param>
        /// <param name="nStart_"></param>
        /// <returns></returns>
        public static uint Bytes2Unit(byte[] byNumber_, int nStart_=0)
        {
            return BitConverter.ToUInt32(byNumber_, nStart_);
        }

        /// <summary>
        /// 把数字转为4字节数组
        /// </summary>
        /// <param name="nNumber_"></param>
        /// <returns></returns>
        public static byte[] Unit2Bytes(uint nNumber_)
        {
            return BitConverter.GetBytes(nNumber_);
        }
        #endregion
    }
}
