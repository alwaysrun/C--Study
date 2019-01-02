using System;

namespace SHCre.Xugd.CFile
{
    partial class XFile
    {
        /// <summary>
        /// ���ļ�����ת��Ϊ��KBΪ��λ�ģ���ǧ��λ�ָ�ģ��ַ���
        /// </summary>
        /// <param name="nFSize_">�ļ��Ĵ�С</param>
        /// <returns>ת������ַ���</returns>
        public static string FSize2String(long nFSize_)
        {
            long nLen = nFSize_ / 1024;
            if (0 != (nFSize_ % 1024))
                ++nLen;

            return string.Format("{0:N0} KB", nLen);
        }

        /// <summary>
        /// ���ļ�ʱ��ת��Ϊ�����Ķ����ַ�����ʽ
        /// </summary>
        /// <param name="stTime_">Ҫת����ʱ��</param>
        /// <returns>�ַ�����ʽ��ʱ��</returns>
        public static string FTime2String(DateTime stTime_)
        {
            return stTime_.ToString("yyyy-MM-dd HH:mm");
        }

        /// <summary>
        /// ���ļ�ʱ��ת��Ϊ�����Ķ����ַ�����ʽ
        /// </summary>
        /// <param name="nTime_">Ҫת����ʱ��</param>
        /// <returns>�ַ�����ʽ��ʱ��</returns>
        public static string FTime2String(long nTime_)
        {
            return FTime2String(DateTime.FromFileTime(nTime_));
        }
    }
}
