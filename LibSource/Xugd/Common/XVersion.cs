using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace SHCre.Xugd.Common
{
    /// <summary>
    /// 版本号类
    /// </summary>
    public class XVersion
    {
        /// <summary>
        /// 主版本
        /// </summary>
        public ushort Major {get; set;}
        /// <summary>
        /// 次版本
        /// </summary>
        public ushort Minor {get; set;}

        /// <summary>
        /// 
        /// </summary>
        public XVersion()
        {
            Major = Minor = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nVersion_"></param>
        public XVersion(uint nVersion_)
        {
            Major = (ushort)((nVersion_ >> 16) & 0xFFFF);
            Minor = (ushort)(nVersion_ & 0xFFFF);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nMajor_"></param>
        /// <param name="nMinor_"></param>
        public XVersion(ushort nMajor_, ushort nMinor_)
        {
            Major = nMajor_;
            Minor = nMinor_;
        }

        private bool Equals(XVersion cmpVer_)
        {
            return cmpVer_.Major == Major && cmpVer_.Minor == Minor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return Equals(obj as XVersion);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Major.GetHashCode() ^ Minor.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vFirst_"></param>
        /// <param name="vSecond_"></param>
        /// <returns></returns>
        public static bool operator==(XVersion vFirst_, XVersion vSecond_)
        {
            return vFirst_.Equals(vSecond_);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vFirst_"></param>
        /// <param name="vSecond_"></param>
        /// <returns></returns>
        public static bool operator !=(XVersion vFirst_, XVersion vSecond_)
        {
            return !vFirst_.Equals(vSecond_);
        }

        /// <summary>
        /// 转换为（Major.Minor）格式字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}.{1}", Major, Minor);
        }

        /// <summary>
        /// 转换为uint数字
        /// </summary>
        /// <returns>对应的uint数字</returns>
        public uint ToUint()
        {
            return (uint)((Major << 16) | Minor);
        }

        /// <summary>
        /// 从字符串（Major.Minor）中分析出版本信息：
        /// 如果字符串为空抛出ArgumentException异常；
        /// 如果不是版本信息会抛出FormatException异常
        /// </summary>
        /// <param name="strVer_">版本号</param>
        /// <returns>版本信息</returns>
        public static XVersion Parse(string strVer_)
        {
            if(string.IsNullOrEmpty(strVer_))
                throw new ArgumentException("Version should like Major.Minor");

            XVersion newVer = new XVersion();
            string[] aryVer = strVer_.Split('.');
            newVer.Major = ushort.Parse(aryVer[0]);
            if(aryVer.Length>1)
                newVer.Minor = ushort.Parse(aryVer[1]);

            return newVer;
        }

        /// <summary>
        /// 分析版本号成功，则返回对应版本号；否则返回默认的版本（0.0)
        /// </summary>
        /// <param name="strVer_"></param>
        /// <returns></returns>
        public static XVersion ParseOrDefault(string strVer_)
        {
            try 
            {
                return Parse(strVer_);
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex.Message, "SHCre.Xugd.CFile.XVersion");
            }

            return new XVersion();
        }
    }
}
