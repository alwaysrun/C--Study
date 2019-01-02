using System;
using System.IO;

using SHCre.Xugd.Common;

namespace SHCre.Xugd.CFile
{
    partial class XFile
    {
        /// <summary>
        /// �ж��ļ��Ƿ���ڣ��������ڻ����ļ��У����׳��쳣
        /// </summary>
        /// <param name="strFiles_"></param>
        public static void CheckFileExist(params string[] strFiles_)
        {
            if (strFiles_.Length == 0) return;

            foreach (string strF in strFiles_)
            {
                var strFull = XPath.GetFullPath(strF);
                if (!File.Exists(strFull))
                    throw new FileNotFoundException(string.Format("{0} not exist", strF));

                if (!IsFile(strFull))
                    throw new FileNotFoundException(string.Format("{0} is not file", strF));
            }
        }

        /// <summary>
        /// �ж��ļ��Ƿ����ļ�
        /// </summary>
        /// <param name="strFile_">Ҫ�жϵ��ļ���ȫ·����</param>
        /// <returns>���ļ�����true�����򷵻�false</returns>
        public static bool IsFile(string strFile_)
        {
            if (!File.Exists(strFile_)) return false;

            return !XFlag.Check((uint)File.GetAttributes(strFile_), (uint)FileAttributes.Directory);
        }

        /// <summary>
        /// �ж��ļ��Ƿ���ʹ�ã�����������򿪣�
        /// </summary>
        /// <param name="strFile_">Ҫ�жϵ��ļ�</param>
        /// <returns>��ʹ�÷���true�����򷵻�false</returns>
        public static bool IsInUse(string strFile_)
        {
            bool bInUse = false;
            if (File.Exists(strFile_))
            {
                try
                {
                    using (FileStream fs = File.Open(strFile_, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        fs.Close();
                    }
                }
                catch (Exception)
                {
                    bInUse = true;
                }
            }

            return bInUse;
        }
        
        /// <summary>
        /// �ж��ļ��Ƿ�Ϊϵͳ�ļ�����System��Hide���ԣ�
        /// </summary>
        /// <param name="strFile_">Ҫ�жϵ��ļ�</param>
        /// <returns>Ϊϵͳ�ļ�������true�����򷵻�false</returns>
        public static bool IsSystemFile(string strFile_)
        {
            uint nAttr = (uint)File.GetAttributes(strFile_);
            if (!XFlag.Check(nAttr, (uint)FileAttributes.Directory)
                && XFlag.Check(nAttr, (uint)(FileAttributes.System | FileAttributes.Hidden)))
                return true;

            return false;
        }
    }
}
