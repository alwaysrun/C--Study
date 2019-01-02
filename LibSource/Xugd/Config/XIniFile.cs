using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace SHCre.Xugd.Config
{
    /// <summary>
    /// 操作Ini文件到接口
    /// </summary>
    public class XIniFile
    {
        private string _strIniFile;
        private const int c_nBuffSize = 1024;
        private const int c_nNumMaxLen = 32;

        /// <summary>
        /// 获取当前Ini的文件名
        /// </summary>
        public string FileName
        {
            get 
            {
                return _strIniFile; 
            }
        }

        //////////////////////////////////////////////////////////////////////////
        //  封装的WinAPI接口
        //////////////////////////////////////////////////////////////////////////
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WritePrivateProfileStringW
            (
                string strSection_,
                string strKeyName_,
                string strValue_,
                string strFile_
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetPrivateProfileStringW
            (
                string strSection_,
                string strKeyName_,
                string strDefValue_,
                StringBuilder strValue_,
                int nValueSize_,
                string strFile_
            );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetPrivateProfileStringW
            (
                string strSection_,
                string strKeyName_,
                string strDefValue_,
                char[] szValue_,
                int nValueSize_,
                string strFile_
            );


        //////////////////////////////////////////////////////////////////////////
        //
        //////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strFile_">Ini文件名（全路径）</param>
        public XIniFile(string strFile_)
        {
            SetFile(strFile_);
        }

        /// <summary>
        /// 设定Ini的文件名
        /// </summary>
        /// <param name="strFile_">Ini文件名（全路径）</param>
        public void SetFile(string strFile_)
        {
            if (!string.IsNullOrEmpty(_strIniFile))
                Flush();

            if (Path.GetExtension(strFile_).ToLower() != ".ini")
            {
                throw new ArgumentException("File type is wrong！");
            }

            if (!File.Exists(strFile_))
            {
                using (StreamWriter streamFile = new StreamWriter(strFile_, false, Encoding.Default))
                {
                    streamFile.Write("");
                    streamFile.Close();
                }
            }
            _strIniFile = strFile_;
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~XIniFile()
        {
            Flush();
        }

        private bool WriteString(string strSection_, string strKeyName_, string strValue_)
        {
            return WritePrivateProfileStringW(strSection_, strKeyName_, strValue_, _strIniFile);
        }

        private void ReadString(string strSection_, string strKeyName_, string strDefValue_, StringBuilder sbValue_)
        {
            GetPrivateProfileStringW(strSection_, strKeyName_, strDefValue_, sbValue_, sbValue_.Capacity, _strIniFile);
        }

        /// <summary>
        /// 获取指定Section下指定Key的值，返回字符串
        /// </summary>
        /// <param name="strSection_">Section名称</param>
        /// <param name="strKeyName_">Key名称</param>
        /// <param name="strDefValue_">获取失败时返回的默认值</param>
        /// <returns>获取的值（字符串）</returns>
        public string GetString(string strSection_, string strKeyName_, string strDefValue_)
        {
            StringBuilder sbGet = new StringBuilder(c_nBuffSize);
            ReadString(strSection_, strKeyName_, strDefValue_, sbGet);

            return sbGet.ToString();
        }

        /// <summary>
        /// 设定指定Section下指定Key的值
        /// </summary>
        /// <param name="strSection_">Section名称</param>
        /// <param name="strKeyName_">Key名称</param>
        /// <param name="strValue_">要设定的值（字符串）</param>
        public void SetString(string strSection_, string strKeyName_, string strValue_)
        {
            if( !WriteString(strSection_, strKeyName_, strValue_) )
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), string.Format("Write {0}={1} failed", strKeyName_, strValue_));
            }
        }

        /// <summary>
        /// 获取指定Section下指定Key的值
        /// </summary>
        /// <param name="strSection_">Section名称</param>
        /// <param name="strKeyName_">Key名称</param>
        /// <param name="nDefValue_">获取失败时返回的默认值</param>
        /// <returns>获取的值</returns>
        public int GetInt(string strSection_, string strKeyName_, int nDefValue_)
        {
            StringBuilder sbGet = new StringBuilder(c_nNumMaxLen);
            ReadString(strSection_, strKeyName_, nDefValue_.ToString(), sbGet);

            return int.Parse(sbGet.ToString());
        }

        /// <summary>
        /// 设定指定Section下指定Key的值
        /// </summary>
        /// <param name="strSection_">Section名称</param>
        /// <param name="strKeyName_">Key名称</param>
        /// <param name="nValue_">要设定的值</param>
        public void SetInt(string strSection_, string strKeyName_, int nValue_)
        {
            SetString(strSection_, strKeyName_, nValue_.ToString());
        }

        /// <summary>
        /// 获取指定Section下指定Key的值
        /// </summary>
        /// <param name="strSection_">Section名称</param>
        /// <param name="strKeyName_">Key名称</param>
        /// <param name="nDefValue_">获取失败时返回的默认值</param>
        /// <returns>获取的值</returns>
        public long GetLong(string strSection_, string strKeyName_, long nDefValue_)
        {
            StringBuilder sbGet = new StringBuilder(c_nNumMaxLen);
            ReadString(strSection_, strKeyName_, nDefValue_.ToString(), sbGet);

            return long.Parse(sbGet.ToString());
        }

        /// <summary>
        /// 设定指定Section下指定Key的值
        /// </summary>
        /// <param name="strSection_">Section名称</param>
        /// <param name="strKeyName_">Key名称</param>
        /// <param name="nValue_">要设定的值（字符串）</param>
        public void SetLong(string strSection_, string strKeyName_, long nValue_)
        {
            SetString(strSection_, strKeyName_, nValue_.ToString()); ;
        }

        /// <summary>
        /// 获取指定Section下所有Key名称
        /// </summary>
        /// <param name="strSection_">Section名称</param>
        /// <returns>包含Key名称的数组</returns>
        public string[] GetKeys(string strSection_)
        {
            char[] szGet = new char[c_nBuffSize * 4];
            int nRetLen = GetPrivateProfileStringW(strSection_, null, null, szGet, szGet.Length, _strIniFile);

            string strGets = new string(szGet, 0, nRetLen);

            return strGets.TrimEnd('\0').Split('\0');
        }

        /// <summary>
        /// 删除指定Section下指定Key值
        /// </summary>
        /// <param name="strSection_">Section名称</param>
        /// <param name="strKeyName_">要删除的Key名称</param>
        public void DeleteKey(string strSection_, string strKeyName_)
        {
            if (!WriteString(strSection_, strKeyName_, null))
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), string.Format("Delete Key {0} failed", strKeyName_)); 
            }
        }

        /// <summary>
        /// 获取所有Section名称
        /// </summary>
        /// <returns>包含所有Section名称的数组</returns>
        public string[] GetSections()
        {
            return GetKeys(null);
        }

        /// <summary>
        /// 删除指定Section
        /// </summary>
        /// <param name="strSection_">要删除的Section名称</param>
        public void DeleteSection(string strSection_)
        {
            if (!WriteString(strSection_, null, null))
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), string.Format("Delete Section {0} failed", strSection_));
            }
        }

        /// <summary>
        /// 把所有操作刷到硬盘上
        /// </summary>
        public void Flush()
        {
            WriteString(null, null, null);
        }
    }
}
