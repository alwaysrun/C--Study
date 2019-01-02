using System.IO;

using SHCre.Xugd.Common;

namespace SHCre.Xugd.CFile
{
    partial class XFolder
    {
        /// <summary>
        /// �ж��ļ��Ƿ����ļ���
        /// </summary>
        /// <param name="strPath_">Ҫ�жϵ��ļ���ȫ·����</param>
        /// <returns>���ļ��з���true�����򷵻�false</returns>
        public static bool IsFolder(string strPath_)
        {
            if (!File.Exists(strPath_)) return false;

            return XFlag.Check((uint)File.GetAttributes(strPath_), (uint)FileAttributes.Directory);
        }

        static readonly string r_strFatRecycle = @":\Recycled";
        static readonly string r_strNtfsRecycle = @":\RECYCLER";
        /// <summary>
        /// �ж��ļ����Ƿ�Ϊ����վ
        /// </summary>
        /// <param name="strPath_">Ҫ�жϵ�·��</param>
        /// <returns>�Ƿ���true�����򷵻�false</returns>
        public static bool IsRecycle(string strPath_)
        {
            bool bIsRecycle = false;

            try
            {
                string strFull = Path.GetFullPath(strPath_);
                if ((0 == string.Compare(strFull, 1, r_strNtfsRecycle, 0, r_strNtfsRecycle.Length, true))
                    || (0 == string.Compare(strFull, 1, r_strFatRecycle, 0, r_strFatRecycle.Length, true)))
                {
                    if (XFlag.Check((uint)File.GetAttributes(strFull), (uint)(FileAttributes.System | FileAttributes.Hidden | FileAttributes.Directory)))
                        bIsRecycle = true;
                }

            }
            catch { }

            return bIsRecycle;
        }

        /// <summary>
        /// �ж��ļ��Ƿ�Ϊϵͳ�ļ��У���System��Hide���ԣ�
        /// </summary>
        /// <param name="strPath_">Ҫ�жϵ��ļ���</param>
        /// <returns>Ϊϵͳ�ļ��У�����true�����򷵻�false</returns>
        public static bool IsSystemFolder(string strPath_)
        {
            return XFlag.Check((uint)File.GetAttributes(strPath_), 
                            (uint)(FileAttributes.System | FileAttributes.Hidden | FileAttributes.Directory));
        }
    }
}
