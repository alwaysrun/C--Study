using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHCre.Xugd.Extension
{
    /// <summary>
    /// 对队列的扩展
    /// </summary>
    public static class XQueueExtension
    {
        /// <summary>
        /// 移除队列前面指定数量的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="quItems_"></param>
        /// <param name="nCount_"></param>
        public static void RemoveHeader<T>(this Queue<T> quItems_, int nCount_)
        {
            if (nCount_ >= quItems_.Count)
                quItems_.Clear();

            while(nCount_ > 0)
            {
                quItems_.Dequeue();
                --nCount_;
            }
        }
    } // class
}
