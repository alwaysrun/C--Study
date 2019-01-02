using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHCre.Xugd.Extension
{
    /// <summary>
    /// 对数组的扩展
    /// </summary>
    public static class XArrayExtension
    {
        /// <summary>
        /// 提取子数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arySource_"></param>
        /// <param name="nStart_">起始位置，不能小于0也不能大于数组长度</param>
        /// <param name="nLength_">要提取的长度：-1则为提取至尾部</param>
        /// <returns></returns>
        public static T[] SubArray<T>(this T[] arySource_, int nStart_, int nLength_=-1)
        {
            if (nStart_ < 0 || nStart_ >= arySource_.Length)
                throw new IndexOutOfRangeException("Start index invalid");
            if (nLength_ == -1)
                nLength_ = arySource_.Length - nStart_;
            else
            {
                if (nStart_ + nLength_ > arySource_.Length)
                    throw new ArgumentOutOfRangeException("Length too large");
            }

            T[] aryDest = new T[nLength_];
            if (nLength_ > 0)
                Array.Copy(arySource_, nStart_, aryDest, 0, nLength_);

            return aryDest;
        }

        /// <summary>
        /// 拆分数组为Blocks份，每份长度为数组长度除以Blocks，最后一份为剩余长度
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arySource_"></param>
        /// <param name="nBlocks_"></param>
        /// <returns></returns>
        public static List<T[]> Split2Blocks<T>(this T[] arySource_, int nBlocks_)
        {
            int nStart = 0;
            int nSize = arySource_.Length / nBlocks_;
            List<T[]> lstDest = new List<T[]>(nBlocks_);
            for(int i=0 ; i<nBlocks_-1 ; ++i)
            {
                lstDest.Add(arySource_.SubArray(nStart, nSize));
                nStart += nSize;
            }
            lstDest.Add(arySource_.SubArray(nStart));

            return lstDest;
        }
    }
}
