using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHCre.Xugd.Extension
{
    /// <summary>
    /// 安全列表：操作前，先自动锁定（lock）。
    /// 允许为空的谓词（即设有=null的参数），才可传递null
    /// </summary>
    /// <typeparam name="U">列表中元素类型</typeparam>
    public class XSafeList<U> where U : class
    {
        private List<U> _lstCollect;

        /// <summary>
        /// 返回当前列表的一个副本
        /// </summary>
        public List<U> Collect { get { return Where(); } }

        readonly object _lkerList = new object();
        object SyncRoot 
        {
            get { return _lkerList;}
        }

        private Func<U, bool> CheckCmpFun(Func<U, bool> funCmp_)
        {
            if (funCmp_ == null)
                return z => { return true; };

            return funCmp_;
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public XSafeList()
        {
            _lstCollect = new List<U>();
        }

        /// <summary>
        /// 构造指定容量的列表
        /// </summary>
        /// <param name="nCapacity_"></param>
        public XSafeList(int nCapacity_)
        {
            _lstCollect = new List<U>(nCapacity_);
        }

        /// <summary>
        /// 根据已有列表构造
        /// </summary>
        /// <param name="initCollect_">用于初始化的列表</param>
        public XSafeList(IEnumerable<U> initCollect_)
        {
            _lstCollect = new List<U>(initCollect_);
        }

        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="uItem_">要添加的元素</param>
        public void Add(U uItem_)
        {
            lock (SyncRoot)
            {
                _lstCollect.Add(uItem_);
            }
        }

        /// <summary>
        /// 添加多个元素
        /// </summary>
        /// <param name="addCollect_">要添加的元素</param>
        public void AddRange(IEnumerable<U> addCollect_)
        {
            lock (SyncRoot)
            {
                _lstCollect.AddRange(addCollect_);
            }
        }

        /// <summary>
        /// 如果不存在，则添加并返回true；否则不操作并返回false；
        /// 如果泛型为string，则不能使用默认的比较函数，需要
        /// AddIfNotExist(uItem_, z=>z==uItem_)
        /// </summary>
        /// <param name="uItem_">要添加的元素</param>
        /// <param name="funCmp_">比较谓词；如果为null，则默认使用object.Equal做的比较</param>
        /// <returns>添加，返回true；否则，返回false</returns>
        public bool AddIfNotExist(U uItem_, Func<U, bool> funCmp_=null)
        {
            bool bAdded = false;
            lock(SyncRoot)
            {
                if (funCmp_ == null)
                {
                    funCmp_ = (z) => z == uItem_;
                }
                var uExist = _lstCollect.FirstOrDefault(funCmp_);
                if(uExist == null)
                {
                    _lstCollect.Add(uItem_);
                    bAdded = true;
                }
            }

            return bAdded;
        }

        /// <summary>
        /// 添加，并保证唯一（添加前，移除所有funCmp为true的项目）
        /// 如果泛型为string，则不能使用默认的比较函数，需要
        /// AddAsUnique(uItem_, z=>z==uItem_)
        /// </summary>
        /// <param name="uItem_">要添加的元素</param>
        /// <param name="funCmp_">比较谓词；如果为null，则默认使用object.Equal做的比较</param>
        /// <returns>移除的元素列表</returns>
        public List<U> AddAsUnique(U uItem_, Func<U, bool> funCmp_=null)
        {
            lock (SyncRoot)
            {
                if (funCmp_ == null)
                    funCmp_ = z => { return z == uItem_; };

                List<U> lstRemove = RemoveAll(funCmp_);
                _lstCollect.Add(uItem_);

                return lstRemove;
            }
        }

        /// <summary>
        /// 如果不存在，则添加并返回新添加的元素；否则返回已有元素。
        /// 如果泛型为string，则不能使用默认的比较函数，需要
        /// AddIfNotExist(uItem_, z=>z==uItem_)
        /// </summary>
        /// <param name="uItem_">要添加的元素</param>
        /// <param name="funCmp_">比较谓词；如果为null，则默认使用object.Equal做的比较</param>
        /// <returns></returns>
        public U AddOrGet(U uItem_, Func<U, bool> funCmp_ = null)
        {
            lock (SyncRoot)
            {
                if (funCmp_ == null)
                {
                    funCmp_ = (z) => z == uItem_;
                }

                var uResult = _lstCollect.FirstOrDefault(funCmp_);
                if (uResult == null)
                {
                    _lstCollect.Add(uItem_);
                    uResult = uItem_;
                }

                return uResult;
            }
        }

        /// <summary>
        /// 如果不存在，则添加；已存在，则更新
        /// </summary>
        /// <param name="uItem_">要添加的元素</param>
        /// <param name="funCmp_">比较谓词</param>
        /// <param name="actUpdate_">更新谓词</param>
        /// <returns>添加，返回true；否则，返回false</returns>
        public bool AddOrUpdate(U uItem_, Func<U, bool> funCmp_, Action<U> actUpdate_=null)
        {
            bool bAdded = false;
            lock(SyncRoot)
            {
                var uExist = _lstCollect.FirstOrDefault(funCmp_);
                if(uExist == null)
                {
                    _lstCollect.Add(uItem_);
                    bAdded = true;
                }
                else
                {
                    if (actUpdate_ != null)
                        actUpdate_(uExist);
                }
            }

            return bAdded;
        }

        /// <summary>
        /// 清空列表
        /// </summary>
        /// <param name="actClear_">如果不为null，则清空前先调用此处理函数</param>
        public void Clear(Action<U> actClear_ = null)
        {
            lock (SyncRoot)
            {
                if(actClear_ !=null)
                {
                    foreach (var item in _lstCollect)
                        actClear_(item);
                }

                _lstCollect.Clear();
            }
        }

        /// <summary>
        /// 判断是否存在指定元素
        /// </summary>
        /// <param name="uItem_">判断的元素</param>
        /// <returns>存在，返回true；否则，返回false</returns>
        public bool Contains(U uItem_)
        {
            lock (SyncRoot)
            {
                return _lstCollect.Contains(uItem_);
            }
        }

        /// <summary>
        /// 判断是否存在满足条件的元素
        /// </summary>
        /// <param name="funCmp_">判断条件</param>
        /// <returns>存在，返回true；否则，返回false</returns>
        public bool Contains(Func<U, bool> funCmp_)
        {
            lock(SyncRoot)
            {
                foreach (var item in _lstCollect)
                {
                    if (funCmp_(item))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取指定位置的元素
        /// </summary>
        /// <param name="nIndex_">索引</param>
        /// <returns>获取的元素</returns>
        public U this[int nIndex_]
        {
            get
            {
                lock (SyncRoot)
                {
                    return _lstCollect[nIndex_];
                }
            }
        }

        /// <summary>
        /// 获取数组形式
        /// </summary>
        /// <returns>数组形式的元素</returns>
        public U[] ToArray()
        {
            lock(SyncRoot)
            {
                return _lstCollect.ToArray();
            }
        }

        /// <summary>
        /// 操作元素
        /// </summary>
        /// <param name="actItem_"></param>
        public void HandleItem(Action<List<U>> actItem_)
        {
            lock(SyncRoot)
            {
                actItem_(_lstCollect);
            }
        }

        /// <summary>
        /// 更新指定的元素
        /// </summary>
        /// <param name="actUpdate_">操作谓词</param>
        /// <param name="funCmp_">比较谓词</param>
        /// <returns>有满足条件的元素，返回第一个；否则，返回默认值</returns>
        public U UpdateFirst(Action<U> actUpdate_, Func<U, bool> funCmp_=null)
        {
            lock(SyncRoot)
            {
                U firstItem = FirstNotLock(funCmp_);
                if (firstItem != null)
                    actUpdate_(firstItem);

                return firstItem;
            }
        }

        /// <summary>
        /// 对满足条件的每个元素执行指定的操作，并返回操作过的列表；
        /// 如果不需要返回列表，可使用ForEach代替
        /// </summary>
        /// <param name="actUpdate_">操作谓词</param>
        /// <param name="funCmp_">比较谓词</param>
        /// <returns>满足条件的列表</returns>
        public List<U> Update(Action<U> actUpdate_, Func<U, bool> funCmp_ = null)
        {
            lock (SyncRoot)
            {
                var lstWhere = _lstCollect.Where(CheckCmpFun(funCmp_)).ToList();
                lstWhere.ForEach(z => actUpdate_(z));

                return lstWhere;
            }
        }

        /// <summary>
        /// 对满足条件的每个元素执行指定的操作
        /// </summary>
        /// <param name="actUpdate_">操作谓词</param>
        /// <param name="funCmp_">比较谓词</param>
        public void ForEach(Action<U> actUpdate_, Func<U, bool> funCmp_=null)
        {
            lock(SyncRoot)
            {
                foreach (var item in _lstCollect.Where(CheckCmpFun(funCmp_)).ToList())
                {
                    actUpdate_(item);
                }
            }
        }

        private U FirstNotLock(Func<U, bool> funCmp_)
        {
            if (funCmp_ == null)
                return _lstCollect.FirstOrDefault();
            else
                return _lstCollect.FirstOrDefault(funCmp_);
        }

        /// <summary>
        /// 获取满足条件第一个，否则返回默认值
        /// </summary>
        /// <param name="funCmp_">比较谓词</param>
        /// <returns>有满足条件的元素，返回第一个；否则，返回默认值</returns>
        public U FirstOrDefault(Func<U, bool> funCmp_=null)
        {
            lock(SyncRoot)
            {
                return FirstNotLock(funCmp_);
            }
        }

        /// <summary>
        /// 获取满足条件的最后已个，否则返回默认值
        /// </summary>
        /// <param name="funCmp_">比较谓词</param>
        /// <returns></returns>
        public U LastOrDefault(Func<U, bool> funCmp_ = null)
        {
            lock(SyncRoot)
            {
                if (funCmp_ == null)
                    return _lstCollect.LastOrDefault();
                else
                    return _lstCollect.LastOrDefault(funCmp_);
            }
        }

        int _nLastGet = -1;
        /// <summary>
        /// 获取满足条件下一个（循环获取），否则返回默认值
        /// </summary>
        /// <param name="funCmp_">比较谓词</param>
        /// <returns>有满足条件的元素，返回第一个；否则，返回默认值</returns>
        public U GetCycleNext(Func<U, bool> funCmp_ = null)
        {
            lock (SyncRoot)
            {
                if(_lstCollect.Count == 0)
                {
                    _nLastGet = -1;
                    return null;
                }

                if (++_nLastGet >= _lstCollect.Count)
                    _nLastGet = 0;
                if(funCmp_ == null)
                {
                    return _lstCollect[_nLastGet];
                }

                // Cycle get next
                int nOldGet = _nLastGet;
                Predicate<U> preCmp = new Predicate<U>(funCmp_);
                _nLastGet = _lstCollect.FindIndex(_nLastGet, preCmp);
                if(_nLastGet == -1 && nOldGet>0)
                {
                    _nLastGet = _lstCollect.FindIndex(0, nOldGet, preCmp);
                }

                if (_nLastGet == -1)
                    return null;

                return _lstCollect[_nLastGet];
            }
        } 

        /// <summary>
        /// 移动指定的项至头部
        /// </summary>
        /// <param name="uItem_">要移动的项</param>
        /// <returns>项移动到了头部返回true；否则，返回false</returns>
        public bool MoveToHead(U uItem_)
        {
            lock (SyncRoot)
            {
                if (_lstCollect.Count == 0)
                    return false;

                if (_lstCollect[0] == uItem_)
                    return true;

                if (_lstCollect.Remove(uItem_))
                {
                    _lstCollect.Insert(0, uItem_);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 移动指定的项至尾部
        /// </summary>
        /// <param name="uItem_">要移动的项</param>
        /// <returns>项移动到了尾部返回true；否则，返回false</returns>
        public bool MoveToTail(U uItem_)
        {
            lock(SyncRoot)
            {
                if (_lstCollect.Count == 0)
                    return false;

                if (_lstCollect[_lstCollect.Count - 1] == uItem_)
                    return true;

                if(_lstCollect.Remove(uItem_))
                {
                    _lstCollect.Add(uItem_);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 移除指定元素
        /// </summary>
        /// <param name="uItem_">要移除的元素</param>
        /// <returns>移除，返回true；否则，返回false</returns>
        public bool Remove(U uItem_)
        {
            lock(SyncRoot)
            {
                return _lstCollect.Remove(uItem_);
            }
        }

        /// <summary>
        /// 移除第一个满足条件的元素（不存在，返回null）
        /// </summary>
        /// <param name="funCmp_">比较谓词</param>
        /// <returns>移除的元素</returns>
        public U RemoveFirst(Func<U, bool> funCmp_=null)
        {
            lock(SyncRoot)
            {
                var delItem = FirstNotLock(funCmp_);
                if(delItem != null)
                {
                    _lstCollect.Remove(delItem);
                }

                return delItem;
            }
        }

        /// <summary>
        /// 移除所有满足条件的元素
        /// </summary>
        /// <param name="funCmp_">比较谓词，为null则移除所有元素</param>
        /// <returns>移除的元素列表</returns>
        public List<U> RemoveAll(Func<U, bool> funCmp_=null)
        {
            lock(SyncRoot)
            {
                return _lstCollect.RemoveAndReturnAll(funCmp_);
            }
        }

        /// <summary>
        /// 获取满足条件的列表
        /// </summary>
        /// <param name="funCmp_">比较谓词</param>
        /// <returns>满足条件的列表</returns>
        public List<U> Where(Func<U, bool> funCmp_=null)
        {
            lock(SyncRoot)
            {
                return _lstCollect.Where(CheckCmpFun(funCmp_)).ToList();
            }
        }

        /// <summary>
        /// 确定序列中的任何元素是否都满足条件
        /// </summary>
        /// <param name="funCmp_">用于测试每个元素是否满足条件的函数</param>
        /// <returns>如果源序列中的任何元素都通过指定谓词中的测试，则为 true；否则为 false。</returns>
        public bool Any(Func<U, bool> funCmp_)
        {
            lock(SyncRoot)
            {
                return _lstCollect.Any(funCmp_);
            }
        }

        /// <summary>
        /// 确定序列中的所有元素是否满足条件
        /// </summary>
        /// <param name="funCmp_">用于测试每个元素是否满足条件的函数</param>
        /// <returns>如果源序列中的每个元素都通过指定谓词中的测试，或者序列为空，则为 true；否则为 false</returns>
        public bool All(Func<U, bool> funCmp_)
        {
            lock(SyncRoot)
            {
                return _lstCollect.All(funCmp_);
            }
        }

        /// <summary>
        /// 获取序列中满足条件的元素数量
        /// </summary>
        /// <param name="funCmp_"></param>
        /// <returns></returns>
        public int FilterCount(Func<U, bool> funCmp_)
        {
            lock(SyncRoot)
            {
                return _lstCollect.Count(funCmp_);
            }
        }

        /// <summary>
        /// 元素个数
        /// </summary>
        public int Count
        {
            get
            {
                lock(SyncRoot)
                {
                    return _lstCollect.Count;
                }
            }
        }
    } // Class
} // Namespace
