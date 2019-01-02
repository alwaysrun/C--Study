using System;
using System.Collections.Generic;
using System.Linq;

namespace SHCre.Xugd.Extension
{
    /// <summary>
    /// 用于去重的接口
    /// </summary>
    public interface IUniqueListItemBase
    {
        /// <summary>
        /// 用于判断是否重复的依据
        /// </summary>
        /// <returns></returns>
        string GetUniqueKey();
    }

    /// <summary>
    /// 对集合类的扩展
    /// </summary>
    public static class XCollectionExtension
    {
        /// <summary>
        /// 去重
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList_"></param>
        /// <returns></returns>
        public static List<T> WhereUnique<T>(this List<T> tList_) where T : IUniqueListItemBase
        {
            //Predicate<T> preFind = new Predicate<T>(funPre_);
            var toUnique = tList_.Where((zw, zi) => tList_.FindIndex(xf => zw.GetUniqueKey() == xf.GetUniqueKey()) == zi);
            return toUnique.ToList();
        }

        /// <summary>
        /// 获取非延迟执行的结果（因默认的Where是延迟执行的，如果获取后但使用前列表内容有改变，会出错）
        /// </summary>
        /// <typeparam name="T">可枚举的类型</typeparam>
        /// <param name="tCollect_">要筛选的对象</param>
        /// <param name="funPre_">筛选条件</param>
        /// <returns>获取的结果列表</returns>
        public static List<T> WhereNoDelay<T>(this IEnumerable<T> tCollect_, Func<T, bool> funPre_=null)
        {
            var toGet = tCollect_.Where(CheckCmpFun(funPre_));
            return toGet.ToList();
        }

        private static Func<T, bool> CheckCmpFun<T>(Func<T, bool> funCmp_)
        {
            if (funCmp_ == null)
                return z => { return true; };

            return funCmp_;
        }

        /// <summary>
        /// 获取非延迟执行的结果（因默认的Where是延迟执行的，如果获取后但使用前列表内容有改变，会出错）
        /// </summary>
        /// <typeparam name="T">可枚举的类型</typeparam>
        /// <param name="tCollect_">要筛选的列表</param>
        /// <param name="funPre_">筛选条件</param>
        /// <returns>获取的结果列表</returns>
        public static List<T> WhereNoDelay<T>(this List<T> tCollect_, Func<T, bool> funPre_=null)
        {
            var toGet = tCollect_.Where(CheckCmpFun(funPre_));
            return toGet.ToList();
        }

        /// <summary>
        /// 对满足条件的每个元素执行指定的操作，并返回操作过的列表;
        /// 如果不需要返回列表，可使用ForEach代替
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tCollect_"></param>
        /// <param name="actUpdate_">操作谓词</param>
        /// <param name="funPre_">比较谓词</param>
        /// <returns>满足条件的列表</returns>
        public static List<T> Update<T>(this List<T> tCollect_, Action<T> actUpdate_, Func<T, bool> funPre_=null)
        {
            var toGet = tCollect_.Where(CheckCmpFun(funPre_)).ToList();
            toGet.ForEach(z => actUpdate_(z));
            return toGet;
        }    

        /// <summary>
        /// 打印列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lstInfo_"></param>
        /// <param name="strSeperator_"></param>
        /// <returns></returns>
        public static string PrintList<T>(this List<T> lstInfo_, string strSeperator_=",")
        {
            if (lstInfo_.Count == 0)
                return string.Empty;

            return string.Join(",", lstInfo_);
        }

        /// <summary>
        /// 如果不存在，则添加：
        /// 如果泛型为string，则不能使用默认的比较函数，需要
        /// AddIfNotExist(uItem_, z=>z==uItem_)
        /// </summary>
        /// <typeparam name="T">可枚举的类型</typeparam>
        /// <param name="tCollect_">要添加到的列表</param>
        /// <param name="uItem_">要添加的对象</param>
        /// <param name="funCmp_">比较算子</param>
        /// <returns>添加返回true；否则返回false</returns>
        public static bool AddIfNotExist<T>(this List<T> tCollect_, T uItem_, Func<T, bool> funCmp_=null)where T:class
        {
            bool bAdd = false;
            if (funCmp_ == null)
                funCmp_ = (z) => { return z == uItem_; }; 
            if(tCollect_.FirstOrDefault(funCmp_)==null)
            {
                tCollect_.Add(uItem_);
                bAdd = true;
            }

            return bAdd;
        }

        /// <summary>
        /// 保证添加的元素是唯一的（如果有重复的，先移除再添加新的）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tCollect_"></param>
        /// <param name="uItem_"></param>
        /// <param name="funCmp_"></param>
        /// <returns>移除的元素（如果没有重复的，返回null）</returns>
        public static T AddAsUnique<T>(this List<T> tCollect_, T uItem_, Func<T, bool> funCmp_) where T : class
        {
            if (funCmp_ == null)
                funCmp_ = (z) => { return z == uItem_; };

            T tRemoved_ = tCollect_.RemoveAndReturnFirst(funCmp_);
            tCollect_.Add(uItem_);
            return tRemoved_;
        }

        /// <summary>
        /// 移除所有满足条件的元素
        /// </summary>
        /// <typeparam name="T">可枚举的类型</typeparam>
        /// <param name="tCollect_">要移除的列表</param>
        /// <param name="funCmp_">比较谓词，为null则移除所有元素</param>
        /// <returns>移除的元素列表</returns>
        public static List<T> RemoveAndReturnAll<T>(this List<T> tCollect_, Func<T, bool> funCmp_ = null)
        {
            List<T> lstResult = new List<T>();
            if(funCmp_ == null)
            {
                lstResult.AddRange(tCollect_);
                tCollect_.Clear();
                return lstResult;
            }

            for (int i = tCollect_.Count - 1; i >= 0; --i)
            {
                if (funCmp_(tCollect_[i]))
                {
                    lstResult.Add(tCollect_[i]);
                    tCollect_.RemoveAt(i);
                }
            }
            return lstResult;
        }

        /// <summary>
        /// 移除满足条件的元素
        /// </summary>
        /// <typeparam name="T">可枚举的类型</typeparam>
        /// <param name="tCollect_">要移除的列表</param>
        /// <param name="funCmp_">比较谓词</param>
        /// <returns>移除的元素</returns>
        public static T RemoveAndReturnFirst<T>(this List<T> tCollect_, Func<T, bool> funCmp_ = null)
        {
            T tItem = tCollect_.FirstOrDefault(CheckCmpFun(funCmp_));
            if (tItem != null)
                tCollect_.Remove(tItem);

            return tItem;
        }

        /// <summary>
        /// 把指定元素移动到头部
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="lstCollect_">所在集合列表</param>
        /// <param name="tItem_">要移动的元素</param>
        /// <returns>移动到头部，返回true；否则，返回false</returns>
        public static bool MoveToHead<T>(this List<T> lstCollect_, T tItem_) where T : class
        {
            if (lstCollect_.Count == 0)
                return false;

            if (lstCollect_[0] == tItem_)
                return true;

            if (lstCollect_.Remove(tItem_))
            {
                lstCollect_.Insert(0, tItem_);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 把指定元素移动到尾部
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="lstCollect_">所在集合列表</param>
        /// <param name="tItem_">要移动的元素</param>
        /// <returns>移动到尾部，返回true；否则，返回false</returns>
        public static bool MoveToTail<T>(this List<T> lstCollect_, T tItem_) where T:class
        {
            if (lstCollect_.Count == 0)
                return false;

            if (lstCollect_[lstCollect_.Count - 1] == tItem_)
                return true;

            if (lstCollect_.Remove(tItem_))
            {
                lstCollect_.Add(tItem_);
                return true;
            }

            return false;
        }
    } // class
}
