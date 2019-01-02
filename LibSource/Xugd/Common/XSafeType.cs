using System;

namespace SHCre.Xugd.Common
{
    /// <summary>
    /// 获取或设定时都先锁定
    /// </summary>
    /// <typeparam name="T">被保护数据的类型</typeparam>
    public class XSafeType<T> where T : IComparable
    {
        T _tValue;
        readonly object _lkerType = new object();

        /// <summary>
        /// 
        /// </summary>
        public XSafeType()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tObj_"></param>
        public XSafeType(T tObj_)
        {
            _tValue = tObj_;
        }

        /// <summary>
        /// 线程锁定-获取/设定值
        /// </summary>
        public T Value
        {
            get
            {
                lock (_lkerType) { return _tValue; }
            }

            set
            {
                lock (_lkerType) { _tValue = value; }
            }
        }

        /// <summary>
        /// 设定值
        /// </summary>
        /// <param name="tObj_"></param>
        public void Set(T tObj_)
        {
            lock(_lkerType)
            {
                _tValue = tObj_;
            }
        }

        /// <summary>
        /// 比较：
        /// 小于0：当前值比tObj_小；
        /// 等于0：当前值与tObj_相等；
        /// 大于0：当前值比tObj_大
        /// </summary>
        /// <param name="tObj_"></param>
        /// <returns></returns>
        public int Compare(T tObj_)
        {
            lock (_lkerType)
            {
                return _tValue.CompareTo(tObj_);
            }
        }

        /// <summary>
        /// 判断是否相等
        /// </summary>
        /// <param name="tObj_"></param>
        /// <returns></returns>
        public bool IsEqual(T tObj_)
        {
            return Compare(tObj_) == 0;
        }

        /// <summary>
        /// 判断是否相等
        /// </summary>
        /// <param name="tObj_"></param>
        /// <returns></returns>
        public bool IsEqual(XSafeType<T> tObj_)
        {
            return Compare(tObj_.Value) == 0;
        }

        /// <summary>
        /// 比较并赋值：
        /// 如果相等返回true；否则，把Value设为新的值并返回false。
        /// </summary>
        /// <param name="tObj_"></param>
        /// <returns></returns>
        public bool EqualOrSet(T tObj_)
        {
            lock(_lkerType)
            {
                if (_tValue.CompareTo(tObj_) == 0)
                    return true;

                _tValue = tObj_;
                return false;
            }
        }

        /// <summary>
        /// 由GetResult(T)调用的委托：调用时为(Value, Param)
        /// </summary>
        public DelTwoParamFunc<T, T, T> FunTwoParam {get; set;}

        /// <summary>
        /// 操作并返回结果：
        /// Value设为FunTwoParam(Value, Param)并返回
        /// </summary>
        /// <param name="tParam_"></param>
        /// <returns></returns>
        public T GetResult(T tParam_)
        {
            if (FunTwoParam == null)
                throw new InvalidOperationException("Delegate FunTwoParam not set");

            lock(_lkerType)
            {
                _tValue = FunTwoParam(_tValue, tParam_);
                return _tValue;
            }
        }

        /// <summary>
        /// 由GetResult()调用的委托
        /// </summary>
        public DelOneParamFunc<T,T> FunOneParam {get; set;}

        /// <summary>
        /// 操作并返回结果：
        /// Value设为fun_(Value)并返回
        /// </summary>
        /// <returns></returns>
        public T GetResult()
        {
            if (FunOneParam == null)
                throw new InvalidOperationException("FunOneParam has not set");

            lock(_lkerType)
            {
                _tValue = FunOneParam(_tValue);
                return _tValue;
            }
        }
    }

    /// <summary>
    /// 获取指定范围内的序列[Min, Max]，每次获取递增1，当超过最大值时就从最小值重新开始
    /// </summary>
    public class XSafeSequence
    {
        /// <summary>
        /// 序列的最大值
        /// </summary>
        public int Max {get; private set;}

        /// <summary>
        /// 序列的最小值
        /// </summary>
        public int Min {get; private set;}

        /// <summary>
        /// 获取当前值
        /// </summary>
        public int CurrValue { get { return _nSafeSeq.Value; } }

        XSafeType<int> _nSafeSeq;

        /// <summary>
        /// 序列限定在[Min, Max]范围内
        /// </summary>
        /// <param name="nMax_"></param>
        /// <param name="nMin_"></param>
        public XSafeSequence(int nMin_, int nMax_)
        {
            if (nMin_ >= nMax_)
                throw new ArgumentException("Argument Min must less than Max");

            Max = nMax_;
            Min = nMin_;
            _nSafeSeq = new XSafeType<int>(nMin_ - 1)
            {
                FunOneParam = DelNextDataInRange,
            };
        }

        /// <summary>
        /// 序列为从1开始的所有整数
        /// </summary>
        public XSafeSequence()
        {
            Max = int.MaxValue;
            Min = int.MinValue;

            _nSafeSeq = new XSafeType<int>(0)
            {
                FunOneParam = DelNextDataAll,
            };
        }

        private int DelNextDataAll(int nValue_)
        {
            return nValue_ + 1;
        }

        private int DelNextDataInRange(int nValue_)
        {
            ++nValue_;
            if (nValue_ > Max)
                nValue_ = Min;

            return nValue_;
        }

        /// <summary>
        /// 如果在合理范围[Min,Max]内，则重设序列为指定的值，再次获取时从(nValue+1)开始；
        /// 否则抛出ArgumentException
        /// </summary>
        /// <param name="nValue_"></param>
        public void Reset(int nValue_)
        {
            if (nValue_ > Max || nValue_ < Min)
                throw new ArgumentException(string.Format("Value {0} not between {1} and {2}", nValue_, Min, Max));

            _nSafeSeq.Value = nValue_;
        }

        /// <summary>
        /// 如果在合理范围[Min,Max]内，则重设序列为指定的值，再次获取时从(nValue+1)开始；
        /// 否则不做任何处理
        /// </summary>
        /// <param name="nValue_"></param>
        public void TryReset(int nValue_)
        {
            if (nValue_ > Max || nValue_ < Min)
                return;

            _nSafeSeq.Value = nValue_;
        }

        /// <summary>
        /// 获取序列
        /// </summary>
        /// <returns></returns>
        public int GetNext()
        {
            return _nSafeSeq.GetResult();
        }
    }

    /// <summary>
    /// 可自动重新计数的自增序列（范围内为[Min, Max]），每次获取递增1，当超过最大值时自动从最小值重新开始；
    /// 如果重试标识与上次调用时设定的不同，则重新计数（设为Min）。
    /// </summary>
    public class XSafeResetSeq
    {
        /// <summary>
        /// 序列的最大值
        /// </summary>
        public int Max { get; private set; }

        /// <summary>
        /// 序列的最小值
        /// </summary>
        public int Min { get; private set; }

        /// <summary>
        /// 获取当前值
        /// </summary>
        public int CurrValue { get { return _nSafeSeq.Value; } }

        int _nResetTag = 0; // 如果传入的参数与此不相同，则重设序列
        XSafeType<int> _nSafeSeq;

        /// <summary>
        /// 获取从Min+1开始所有[Min, Max]范围内的序列
        /// </summary>
        /// <param name="nMax_"></param>
        /// <param name="nMin_"></param>
        public XSafeResetSeq(int nMin_, int nMax_)
        {
            if (nMin_ >= nMax_)
                throw new ArgumentException("Argument Min must less than Max");

            Max = nMax_;
            Min = nMin_;
            _nSafeSeq = new XSafeType<int>(nMin_-1)
            {
                FunTwoParam = DelNextDataInRange,
            };
        }

        /// <summary>
        /// 序列为从1开始的所有整数
        /// </summary>
        public XSafeResetSeq()
        {
            Max = int.MaxValue;
            Min = int.MinValue;

            _nSafeSeq = new XSafeType<int>(0)
            {
                FunTwoParam = DelNextDataAll,
            };
        }

        private int DelNextDataAll(int nValue_, int nResetTag_)
        {
            if (_nResetTag != nResetTag_)
            {
                _nResetTag = nResetTag_;
                nValue_ = 0;
            }
            return nValue_ + 1;
        }

        private int DelNextDataInRange(int nValue_, int nResetTag_)
        {
            if (_nResetTag == nResetTag_)
            {
                ++nValue_;
                if (nValue_ > Max)
                    nValue_ = Min;

                return nValue_;
            }
            else
            {
                _nResetTag = nResetTag_;
                return Min;
            }
        }

        /// <summary>
        /// 如果在合理范围[Min,Max]内，则重设序列为指定的值，再次获取时从(nValue+1)开始；
        /// 否则抛出ArgumentException
        /// </summary>
        /// <param name="nResetTag_">重设标识（参见GetResult说明）</param>
        /// <param name="nValue_"></param>
        public void Reset(int nValue_, int nResetTag_)
        {
            if (nValue_ > Max || nValue_ < Min)
                throw new ArgumentException(string.Format("Value {0} not between {1} and {2}", nValue_, Min, Max));

            _nResetTag = nResetTag_;
            _nSafeSeq.Value = nValue_;
        }

        /// <summary>
        /// 如果在合理范围[Min,Max]内，则重设序列为指定的值，再次获取时从(nValue+1)开始；
        /// 否则不做任何处理
        /// </summary>
        /// <param name="nResetTag_">重设标识（参见GetResult说明）</param>
        /// <param name="nValue_"></param>
        public void TryReset(int nValue_, int nResetTag_)
        {
            if (nValue_ > Max || nValue_ < Min)
                return;

            _nResetTag = nResetTag_;
            _nSafeSeq.Value = nValue_;
        }

        /// <summary>
        /// 获取序列
        /// </summary>
        /// <returns></returns>
        public int GetNext()
        {
            return _nSafeSeq.GetResult(_nResetTag);
        }

        /// <summary>
        /// 获取序列
        /// </summary>
        /// <param name="nResetTag_">重设标识：如果此标识与上次调用时传入的不同，则重设序列为起始值</param>
        /// <returns></returns>
        public int GetNext(int nResetTag_)
        {
            return _nSafeSeq.GetResult(nResetTag_);
        }
    }
}
