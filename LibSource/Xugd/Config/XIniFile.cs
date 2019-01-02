using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace SHCre.Xugd.Config
{
    /// <summary>
    /// ����Ini�ļ����ӿ�
    /// </summary>
    public class XIniFile
    {
        private string _strIniFile;
        private const int c_nBuffSize = 1024;
        private const int c_nNumMaxLen = 32;

        /// <summary>
        /// ��ȡ��ǰIni���ļ���
        /// </summary>
        public string FileName
        {
            get 
            {
                return _strIniFile; 
            }
        }

        //////////////////////////////////////////////////////////////////////////
        //  ��װ��WinAPI�ӿ�
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
        /// ���캯��
        /// </summary>
        /// <param name="strFile_">Ini�ļ�����ȫ·����</param>
        public XIniFile(string strFile_)
        {
            SetFile(strFile_);
        }

        /// <summary>
        /// �趨Ini���ļ���
        /// </summary>
        /// <param name="strFile_">Ini�ļ�����ȫ·����</param>
        public void SetFile(string strFile_)
        {
            if (!string.IsNullOrEmpty(_strIniFile))
                Flush();

            if (Path.GetExtension(strFile_).ToLower() != ".ini")
            {
                throw new ArgumentException("File type is wrong��");
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
        /// ��������
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
        /// ��ȡָ��Section��ָ��Key��ֵ�������ַ���
        /// </summary>
        /// <param name="strSection_">Section����</param>
        /// <param name="strKeyName_">Key����</param>
        /// <param name="strDefValue_">��ȡʧ��ʱ���ص�Ĭ��ֵ</param>
        /// <returns>��ȡ��ֵ���ַ�����</returns>
        public string GetString(string strSection_, string strKeyName_, string strDefValue_)
        {
            StringBuilder sbGet = new StringBuilder(c_nBuffSize);
            ReadString(strSection_, strKeyName_, strDefValue_, sbGet);

            return sbGet.ToString();
        }

        /// <summary>
        /// �趨ָ��Section��ָ��Key��ֵ
        /// </summary>
        /// <param name="strSection_">Section����</param>
        /// <param name="strKeyName_">Key����</param>
        /// <param name="strValue_">Ҫ�趨��ֵ���ַ�����</param>
        public void SetString(string strSection_, string strKeyName_, string strValue_)
        {
            if( !WriteString(strSection_, strKeyName_, strValue_) )
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), string.Format("Write {0}={1} failed", strKeyName_, strValue_));
            }
        }

        /// <summary>
        /// ��ȡָ��Section��ָ��Key��ֵ
        /// </summary>
        /// <param name="strSection_">Section����</param>
        /// <param name="strKeyName_">Key����</param>
        /// <param name="nDefValue_">��ȡʧ��ʱ���ص�Ĭ��ֵ</param>
        /// <returns>��ȡ��ֵ</returns>
        public int GetInt(string strSection_, string strKeyName_, int nDefValue_)
        {
            StringBuilder sbGet = new StringBuilder(c_nNumMaxLen);
            ReadString(strSection_, strKeyName_, nDefValue_.ToString(), sbGet);

            return int.Parse(sbGet.ToString());
        }

        /// <summary>
        /// �趨ָ��Section��ָ��Key��ֵ
        /// </summary>
        /// <param name="strSection_">Section����</param>
        /// <param name="strKeyName_">Key����</param>
        /// <param name="nValue_">Ҫ�趨��ֵ</param>
        public void SetInt(string strSection_, string strKeyName_, int nValue_)
        {
            SetString(strSection_, strKeyName_, nValue_.ToString());
        }

        /// <summary>
        /// ��ȡָ��Section��ָ��Key��ֵ
        /// </summary>
        /// <param name="strSection_">Section����</param>
        /// <param name="strKeyName_">Key����</param>
        /// <param name="nDefValue_">��ȡʧ��ʱ���ص�Ĭ��ֵ</param>
        /// <returns>��ȡ��ֵ</returns>
        public long GetLong(string strSection_, string strKeyName_, long nDefValue_)
        {
            StringBuilder sbGet = new StringBuilder(c_nNumMaxLen);
            ReadString(strSection_, strKeyName_, nDefValue_.ToString(), sbGet);

            return long.Parse(sbGet.ToString());
        }

        /// <summary>
        /// �趨ָ��Section��ָ��Key��ֵ
        /// </summary>
        /// <param name="strSection_">Section����</param>
        /// <param name="strKeyName_">Key����</param>
        /// <param name="nValue_">Ҫ�趨��ֵ���ַ�����</param>
        public void SetLong(string strSection_, string strKeyName_, long nValue_)
        {
            SetString(strSection_, strKeyName_, nValue_.ToString()); ;
        }

        /// <summary>
        /// ��ȡָ��Section������Key����
        /// </summary>
        /// <param name="strSection_">Section����</param>
        /// <returns>����Key���Ƶ�����</returns>
        public string[] GetKeys(string strSection_)
        {
            char[] szGet = new char[c_nBuffSize * 4];
            int nRetLen = GetPrivateProfileStringW(strSection_, null, null, szGet, szGet.Length, _strIniFile);

            string strGets = new string(szGet, 0, nRetLen);

            return strGets.TrimEnd('\0').Split('\0');
        }

        /// <summary>
        /// ɾ��ָ��Section��ָ��Keyֵ
        /// </summary>
        /// <param name="strSection_">Section����</param>
        /// <param name="strKeyName_">Ҫɾ����Key����</param>
        public void DeleteKey(string strSection_, string strKeyName_)
        {
            if (!WriteString(strSection_, strKeyName_, null))
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), string.Format("Delete Key {0} failed", strKeyName_)); 
            }
        }

        /// <summary>
        /// ��ȡ����Section����
        /// </summary>
        /// <returns>��������Section���Ƶ�����</returns>
        public string[] GetSections()
        {
            return GetKeys(null);
        }

        /// <summary>
        /// ɾ��ָ��Section
        /// </summary>
        /// <param name="strSection_">Ҫɾ����Section����</param>
        public void DeleteSection(string strSection_)
        {
            if (!WriteString(strSection_, null, null))
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), string.Format("Delete Section {0} failed", strSection_));
            }
        }

        /// <summary>
        /// �����в���ˢ��Ӳ����
        /// </summary>
        public void Flush()
        {
            WriteString(null, null, null);
        }
    }
}
