using System;
using System.Collections.Generic;

namespace SHCre.Xugd.Common
{
    /// <summary>
    /// ���ڼ��ϱȽϵĺ���
    /// </summary>
    public static class XCompare
    {
        /// <summary>
        /// �жϼ����Ƿ���ȣ�������ͬ����ÿһ��Ԫ����ȣ�
        /// </summary>
        /// <typeparam name="T">������Ԫ�ص�����</typeparam>
        /// <param name="tFirst_">����1</param>
        /// <param name="tSecond_">����2</param>
        /// <returns>��ȣ�����true�����򣬷���false</returns>
        public static bool AreEqual<T>(IList<T> tFirst_, IList<T> tSecond_) where T : IComparable
        {
            if (tFirst_.Count != tSecond_.Count)
                return false;

            return AreEqual<T>(tFirst_, tSecond_, tFirst_.Count);
        }

        /// <summary>
        /// �ж�������ָ��������Ԫ���Ƿ���ȣ�ÿһ��Ԫ����ȣ�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tFirst_"></param>
        /// <param name="tSecond_"></param>
        /// <param name="nCount_">Ҫ�Ƚϵ�Ԫ������</param>
        /// <returns></returns>
        public static bool AreEqual<T>(IList<T> tFirst_, IList<T> tSecond_, int nCount_) where T : IComparable
        {
            if (tFirst_.Count < nCount_ || tSecond_.Count < nCount_)
                return false;

            for (int i = 0; i < nCount_; ++i)
            {
                if (0 != tFirst_[i].CompareTo(tSecond_[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// �ж�������ָ��������Ԫ���Ƿ���ȣ�ÿһ��Ԫ����ȣ�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tFirst_"></param>
        /// <param name="nFirstOffset_"></param>
        /// <param name="tSecond_"></param>
        /// <param name="nSecondOffset_"></param>
        /// <param name="nCount_">Ҫ�Ƚϵ�Ԫ������</param>
        /// <returns></returns>
        public static bool AreEqual<T>(IList<T> tFirst_, int nFirstOffset_, IList<T> tSecond_, int nSecondOffset_, int nCount_) where T : IComparable
        {
            if ((tFirst_.Count - nFirstOffset_) < nCount_ || (tSecond_.Count - nSecondOffset_) < nCount_)
                return false;

            for(int i=0 ; i<nCount_ ; ++i)
            {
                if (0 != tFirst_[nFirstOffset_ + i].CompareTo(tSecond_[nSecondOffset_ + i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// �Ƚ��������ϴ�С
        /// </summary>
        /// <typeparam name="T">������Ԫ�ص�����</typeparam>
        /// <param name="tFirst_">����1</param>
        /// <param name="tSecond_">����2</param>
        /// <returns>��ȣ�����0��tFirst_>tSecond_������ֵ�����㣻���򣬷���ֵС����</returns>
        public static int Compare<T>(IList<T> tFirst_, IList<T> tSecond_) where T : IComparable
        {
            int nResult = 1;    // Assume tFirst_.Length>tSecond_.Length
            int nLen = tFirst_.Count;  // The least length
            if (nLen == tSecond_.Count)
            {
                nResult = 0;
            }
            else if (tSecond_.Count < nLen)
            {
                nLen = tSecond_.Count;
                nResult = -1;
            }

            int nComp = 0;
            for (int i = 0; i < nLen; ++i)
            {
                nComp = tFirst_[i].CompareTo(tSecond_[i]);
                if (nComp != 0)
                    return nComp;
            }

            // All equal, use the length
            return nResult;
        }
    };
}
