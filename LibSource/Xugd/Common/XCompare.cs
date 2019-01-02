using System;
using System.Collections.Generic;

namespace SHCre.Xugd.Common
{
    /// <summary>
    /// 用于集合比较的函数
    /// </summary>
    public static class XCompare
    {
        /// <summary>
        /// 判断集合是否相等（长度相同，且每一个元素相等）
        /// </summary>
        /// <typeparam name="T">集合中元素的类型</typeparam>
        /// <param name="tFirst_">集合1</param>
        /// <param name="tSecond_">集合2</param>
        /// <returns>相等，返回true；否则，返回false</returns>
        public static bool AreEqual<T>(IList<T> tFirst_, IList<T> tSecond_) where T : IComparable
        {
            if (tFirst_.Count != tSecond_.Count)
                return false;

            return AreEqual<T>(tFirst_, tSecond_, tFirst_.Count);
        }

        /// <summary>
        /// 判断数组中指定数量的元素是否相等（每一个元素相等）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tFirst_"></param>
        /// <param name="tSecond_"></param>
        /// <param name="nCount_">要比较的元素数量</param>
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
        /// 判断数组中指定数量的元素是否相等（每一个元素相等）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tFirst_"></param>
        /// <param name="nFirstOffset_"></param>
        /// <param name="tSecond_"></param>
        /// <param name="nSecondOffset_"></param>
        /// <param name="nCount_">要比较的元素数量</param>
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
        /// 比较两个集合大小
        /// </summary>
        /// <typeparam name="T">集合中元素的类型</typeparam>
        /// <param name="tFirst_">集合1</param>
        /// <param name="tSecond_">集合2</param>
        /// <returns>相等，返回0；tFirst_>tSecond_，返回值大于零；否则，返回值小于零</returns>
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
