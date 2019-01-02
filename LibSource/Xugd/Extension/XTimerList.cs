using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Common;
using SHCre.Xugd.CFile;

namespace SHCre.Xugd.Extension
{
    /// <summary>
    /// 有定时（构造时指定的间隔秒数）执行指定操作功能的链表；
    /// 在添加元素（AddItem）后，会自动启动（未调用Stop情况下）定时器执行指定的工作；
    /// 如果Stop过，则必须通过Restart重启
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XTimerList<T> : XLogEventsBase where T : class
    {
        readonly object _lkerDatas = new object();
        System.Timers.Timer _tmDatas = new System.Timers.Timer();
        List<T> _lstDatas = new List<T>();
        Action<List<T>> _actElapse = null;
        bool _bStart = true;

        /// <summary>
        /// 列表变空时触发
        /// </summary>
        public event Action OnEmpty;
        void InvokeEmpty()
        {
            if (OnEmpty != null)
                OnEmpty();
        }

        /// <summary>
        /// 元素数量
        /// </summary>
        public int Count
        {
            get
            {
                lock (_lkerDatas)
                {
                    return _lstDatas.Count;
                }
            }
        }

        /// <summary>
        /// 定时器检测周期
        /// </summary>
        public int IntervalSecond {get; private set;}

        /// <summary>
        /// 获取元素的副本
        /// </summary>
        public List<T> Collect
        {
            get
            {
                lock (_lkerDatas)
                {
                    return _lstDatas.ToList();
                }
            }
        }

        /// <summary>
        /// 构造函数，如果使用此构造函数，则必须调用SetElapse来设定回调
        /// </summary>
        /// <param name="nInterSecond_">定时器重启时间间隔（秒数）</param>
        protected XTimerList(int nInterSecond_)
        {
            _tmDatas.AutoReset = false;
            _tmDatas.Elapsed += new System.Timers.ElapsedEventHandler(tmDatas_Elapsed);
            ResetInterval(nInterSecond_);

            BuildLogPrefix<T>("XTimerList");
        }

        /// <summary>
        /// 重设定时器时间间隔（如果定时器已启用，在设定的时间间隔后触发）
        /// </summary>
        /// <param name="nInterSecond_"></param>
        public void ResetInterval(int nInterSecond_)
        {
            if (nInterSecond_ < 1)
                nInterSecond_ = 1;

            IntervalSecond = nInterSecond_;
            _tmDatas.Interval = XTime.Second2Interval(nInterSecond_);
        }

        /// <summary>
        /// 设定定时器回调函数
        /// </summary>
        /// <param name="actHandle_"></param>
        protected void SetElapse(Action<List<T>> actHandle_)
        {
            if (actHandle_ == null)
                throw new ArgumentNullException("ActHandle for elapse can not NULL");

            _actElapse = actHandle_;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actHandle_">定时器启动时执行的操作</param>
        /// <param name="nInterSecond_">定时器重启时间间隔（秒数）</param>
        public XTimerList(Action<List<T>> actHandle_, int nInterSecond_)
            : this(nInterSecond_)
        {
            SetElapse(actHandle_);
        }

        private void TryStart()
        {
            if (_bStart && !_tmDatas.Enabled)
            {
                InvokeOnDebug("Timer started: interval is {0}s", _tmDatas.Interval / 1000);
                _tmDatas.Start();
            }
        }

        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="tItem_"></param>
        public void AddItem(T tItem_)
        {
            lock (_lkerDatas)
            {
                _lstDatas.Add(tItem_);
                TryStart();
            }
        }

        /// <summary>
        /// 如果元素不存在，则添加
        /// </summary>
        /// <param name="tItem_"></param>
        /// <param name="funCmp_"></param>
        /// <returns></returns>
        public bool AddItem(T tItem_, Func<T, bool> funCmp_)
        {
            bool bAdd;
            lock (_lkerDatas)
            {
                bAdd = _lstDatas.AddIfNotExist(tItem_, funCmp_);
                TryStart();
            }

            return bAdd;
        }

        /// <summary>
        /// 批量添加元素
        /// </summary>
        /// <param name="collItems_"></param>
        public void AddRange(IEnumerable<T> collItems_)
        {
            if (collItems_ == null || collItems_.Count() == 0)
                return;

            lock (_lkerDatas)
            {
                _lstDatas.AddRange(collItems_);
                TryStart();
            }   
        }

        /// <summary>
        /// 保证添加的元素是唯一的（如果有重复的，先移除再添加新的）
        /// </summary>
        /// <param name="tItem_"></param>
        /// <param name="funCmp_"></param>
        /// <returns>移除的元素（如果没有重复的，返回null）</returns>
        public T AddItemAsUnique(T tItem_, Func<T, bool> funCmp_)
        {
            T tRemoved_ = null;
            lock (_lkerDatas)
            {
                tRemoved_ = _lstDatas.AddAsUnique(tItem_, funCmp_);
                TryStart();
            }

            return tRemoved_;
        }

        /// <summary>
        /// 获取满足条件的列表
        /// </summary>
        /// <param name="funCmp_"></param>
        /// <returns></returns>
        public List<T> Where(Func<T, bool> funCmp_)
        {
            lock (_lkerDatas)
            {
                return _lstDatas.WhereNoDelay(funCmp_);
            }
        }

        /// <summary>
        /// 是否包含指定元素
        /// </summary>
        /// <param name="preCmp_"></param>
        /// <returns></returns>
        public bool Contains(Predicate<T> preCmp_)
        {
            lock (_lkerDatas)
            {
                return _lstDatas.FindIndex(preCmp_) != -1;
            }
        }

        /// <summary>
        /// 获取第一个满足条件的元素
        /// </summary>
        /// <param name="funCmp_"></param>
        /// <returns></returns>
        public T FirstOrDefault(Func<T, bool> funCmp_)
        {
            lock (_lkerDatas)
            {
                return _lstDatas.FirstOrDefault(funCmp_);
            }
        }

        /// <summary>
        /// 移除满足条件第一个元素
        /// </summary>
        /// <param name="funCmp_"></param>
        /// <returns></returns>
        public T RemoveFirst(Func<T, bool> funCmp_)
        {
            lock (_lkerDatas)
            {
                var delItem = _lstDatas.RemoveAndReturnFirst(funCmp_);
                if (_lstDatas.Count == 0)
                {
                    _tmDatas.Stop();
                    InvokeEmpty();
                }

                return delItem;
            }
        }

        /// <summary>
        /// 移除指定元素
        /// </summary>
        /// <param name="tItem_"></param>
        public void RemoveItem(T tItem_)
        {
            lock(_lkerDatas)
            {
                _lstDatas.Remove(tItem_);
            }
        }

        /// <summary>
        /// 移除所有满足条件的元素
        /// </summary>
        /// <param name="funCmp_"></param>
        /// <returns></returns>
        public List<T> RemoveAll(Func<T, bool> funCmp_ = null)
        {
            lock (_lkerDatas)
            {
                var lstItem = _lstDatas.RemoveAndReturnAll(funCmp_);
                //if (_lstDatas.Count == 0)
                {
                    _tmDatas.Stop();
                    InvokeEmpty();
                }

                return lstItem;
            }
        }

        /// <summary>
        /// 操作元素
        /// </summary>
        /// <param name="actHandle_"></param>
        public void HandleItem(Action<List<T>> actHandle_)
        {
            lock (_lkerDatas)
            {
                actHandle_(_lstDatas);
            }
        }

        /// <summary>
        /// 操作元素，如果返回true则尝试启动定时器
        /// </summary>
        /// <param name="actHandle_"></param>
        public void HandleItemWithStart(Func<List<T>, bool> actHandle_)
        {
            lock (_lkerDatas)
            {
                if( actHandle_(_lstDatas) )
                {
                    if (_lstDatas.Count > 0)
                        TryStart();
                }
            }
        }

        /// <summary>
        /// 重新启用：调用Stop后，如果想继续处理则需先调用此函数
        /// </summary>
        public virtual void Restart()
        {
            InvokeOnCalled("Restart(IntervalSecond:{0})", IntervalSecond);
            StartTimer();
        }

        /// <summary>
        /// 启用定时器
        /// </summary>
        protected void StartTimer()
        {
            _bStart = true;
            if (!_tmDatas.Enabled)
                _tmDatas.Start();
        }

        /// <summary>
        /// 停止：停用定时器，如果要清空元素请使用Clear
        /// </summary>
        public void Stop()
        {
            InvokeOnLogger("#Stop");
            _bStart = false;
            _tmDatas.Stop();
        }

        /// <summary>
        /// 清空所有元素
        /// </summary>
        /// <param name="actClear_"></param>
        public void Clear(Action<List<T>> actClear_ = null)
        {
            InvokeOnLogger("#Clear");

            _tmDatas.Stop();
            lock (_lkerDatas)
            {
                try
                {
                    if (actClear_ != null)
                        actClear_(_lstDatas);
                    InvokeOnDebug("Clear {0} Items in list", _lstDatas.Count);
                    _lstDatas.Clear();

                    InvokeEmpty();
                }
                catch(Exception ex)
                {
                    InvokeOnExcept(ex, "Clear");
                }
            }
        }

        void tmDatas_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                InvokeOnDebug("Timer-begin(Item-Count:{0})", _lstDatas.Count);
                lock (_lkerDatas)
                {
                    if (_lstDatas.Count > 0)
                        _actElapse(_lstDatas);
                    
                    if (_lstDatas.Count == 0)
                        InvokeEmpty();
                }
            }
            catch (Exception ex)
            {
                InvokeOnExcept(ex);
            }

            InvokeOnDebug("Timer-end(Item-Count:{0})", _lstDatas.Count);
            if (_bStart && _lstDatas.Count > 0)
                _tmDatas.Start();
        }
    } // class

    /// <summary>
    /// 自动移除类的基类
    /// </summary>
    public interface XDelExpireItemBase
    {
        /// <summary>
        /// 获取用于判断是否要移除的时间点
        /// </summary>
        /// <returns></returns>
        DateTime AnchorTime();
    }

    /// <summary>
    /// 过期自动移除的列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XDelExpireList<T> : XTimerList<T> where T : class,XDelExpireItemBase
    {
        TimeSpan _tsDiffFromNow = new TimeSpan(1);
        Action<List<T>> _actRemoved = null;
        /// <summary>
        /// 比较时：当前时间与锚点时间差值或存活时间（AnchorTime小于Now-DiffSecond则移除）
        /// </summary>
        public int DiffSecond {get; private set;}

        /// <summary>
        /// 移除委托：HandleMoved(RemovedItems)
        /// </summary>
        /// <param name="nDiffSecond_">当前时间与锚点时间差值或存活时间（AnchorTime小于Now-DiffSecond则移除）</param>
        /// <param name="nCheckCycleSecond_">查看（比较）周期</param>
        /// <param name="actExpired_">对移除的过期数据处理，参数（删除的列表）</param>
        public XDelExpireList(int nDiffSecond_, int nCheckCycleSecond_, Action<List<T>> actExpired_ = null)
            : base(nCheckCycleSecond_)
        {
            SetElapse(RemovedExpired);
            if (nDiffSecond_ != 0)
            {
                _tsDiffFromNow = TimeSpan.FromSeconds(nDiffSecond_);
            }

            DiffSecond = nDiffSecond_;
            _actRemoved = actExpired_;
        }
        
        /// <summary>
        /// 重新启用：调用Stop后，如果想继续处理则需先调用此函数
        /// </summary>
        public override void Restart()
        {
            InvokeOnCalled("Restart(DiffSecond:{0}, CycleSecond:{1})", DiffSecond, IntervalSecond);
            StartTimer();
        }

        void RemovedExpired(List<T> lstData_)
        {
            if(lstData_.Count == 0)
            {
                if(DebugEnabled)
                    InvokeOnLogger("No Expire-Items need to remove", XLogSimple.LogLevels.Debug);
                return;
            }

            int nCount = 0;
            DateTime dtExpired = DateTime.Now - _tsDiffFromNow;
            if (_actRemoved != null)
            {
                var lstRemoved = lstData_.RemoveAndReturnAll(z => z.AnchorTime() < dtExpired);
                nCount = lstRemoved.Count;

                if (lstRemoved.Count > 0)
                {
                    XThread.StartPool(ActExpired, lstRemoved);
                }
            }
            else
            {
                nCount = lstData_.RemoveAll(z => z.AnchorTime() < dtExpired);
            }

            InvokeOnDebug("Removed {0} Expire-Items", nCount);
        }

        void ActExpired(object oParam_)
        {
            try
            {
                _actRemoved(oParam_ as List<T>);
            }
            catch(Exception ex)
            {
                InvokeOnExcept(ex, "ActExpired");
            }
        }
    } // class
}
