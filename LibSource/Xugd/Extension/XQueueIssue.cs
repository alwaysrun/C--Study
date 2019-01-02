using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SHCre.Xugd.Common;
using SHCre.Xugd.CFile;

namespace SHCre.Xugd.Extension
{
    /// <summary>
    /// 队列分发类（使用AddItem添加元素后，自动启用线程调用ActIssue来分发处理）；
    /// 添加元素（AddItem）后自动启动分发线程；调用Stop后，可重新调用Restart来重新
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XQueueIssue<T> : XLogEventsBase where T : class
    {
        #region "Var"
        readonly object _lkerIssue = new object();
        Queue<T> _quIssue = new Queue<T>();
        Thread _thrIssue = null;
        Thread _thrTmpWhenToomany = null;
        AutoResetEvent _evtIssue = new AutoResetEvent(false);

        bool _bRemovedWhenToomany = true;
        bool _bGetItemViaPeek = false;
        Func<T, bool> _funcIssue = null;
        int _nInterval = 0;
        bool _bStarted = true;
        //int _nTimeoutSecond = 0;
        //DateTime _dtLastBegin = DateTime.Now;
        #endregion

        #region "Property"
        /// <summary>
        /// 设定发布线程的优先级
        /// </summary>
        public ThreadPriority Priority { get;set; }

        /// <summary>
        /// 队列最大运行长度
        /// </summary>
        public int MaxAllowSize { get; private set; }

        /// <summary>
        /// 获取当前队列数量
        /// </summary>
        public int Count
        {
            get
            {
                lock (_lkerIssue)
                { 
                    return _quIssue.Count; 
                }
            }
        }

        /// <summary>
        /// 因条目太多而移除时调用的方法
        /// </summary>
        public event Action<List<T>> ActRemovedItems;

        ///// <summary>
        ///// 设定程序执行的超时时间（只对非Peek模式有效），如果超过此时间没有完成则强制结束；
        ///// 默认0，表示没有超时限制
        ///// </summary>
        //public int ExecuteTimeoutSecond
        //{
        //    get { return _nTimeoutSecond; }
        //    set 
        //    {
        //        if (value < 10 && value!=0)
        //            value = 10;
        //        _nTimeoutSecond = value;
        //    }
        //}

        /// <summary>
        /// 获取队列头部元素（不改变队列中元素数量），如果队列空则返回null。
        /// </summary>
        public T Header 
        {
            get 
            {
                lock(_lkerIssue)
                {
                    if (_quIssue.Count == 0)
                        return null;

                    return _quIssue.Peek();
                }
            }
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="funIssue_">分发处理函数（在线程中调用）：如果GetItemViaPeek，只有返回true时才真正从队列中移除元素</param>
        /// <param name="nMaxSize_">队列中允许存放的最大元素数（超过此数量，则会移除队列顶部一半的元素）</param>
        /// <param name="bRemoveIfToomany_">如果队列中的元素数量超过设定的最大值：则当RemoveIfToomany为true时，删除头部一半的数据(可通过ActRemovedItems获取删除的数据)；
        /// 为false时，启动一个新的临时线程来处理数据分发操作</param>
        /// <param name="bGetViaPeek_"> 通过ActIssue处理的元素如何获取：
        /// true，处理前不从队列中移除元素（Peek），只有在处理成功后再移除（RemoveIfToomany要为true）；
        /// false，处理前先从队列中移除元素（Dequeue）</param>
        /// <param name="nCheckSeconds_">尝试处理最大间隔时间（即使没有信号，也会在此时间间隔后尝试查看队列中是否有未处理的元素）</param>
        public XQueueIssue(Func<T, bool> funIssue_, int nMaxSize_ = 100, bool bRemoveIfToomany_ = true, bool bGetViaPeek_=false, int nCheckSeconds_ = 600)
        {
            if (funIssue_ == null)
                throw new ArgumentNullException("Action for issue queue can not NULL");
            if(bGetViaPeek_)
            {
                if (!bRemoveIfToomany_)
                    throw new ArgumentException("When GetViaPeek is true, RemoveIfToomay must is true");
            }

            if (nCheckSeconds_ < 10)
                nCheckSeconds_ = 10;
            if (nMaxSize_ < 5)
                nMaxSize_ = 5;

            Priority = ThreadPriority.Normal;
            _funcIssue = funIssue_;
            MaxAllowSize = nMaxSize_;
            _nInterval = XTime.Second2Interval(nCheckSeconds_);
            _bRemovedWhenToomany = bRemoveIfToomany_;
            _bGetItemViaPeek = bGetViaPeek_;

            BuildLogPrefix<T>("XQueueIssue");
        }

        /// <summary>
        /// 添加元素：添加后会自动启动线程进行分发处理；
        /// 如果队列中的元素数量超过设定的最大值：则当RemoveIfToomany为true时，删除头部一半的数据；
        /// 为false时，启动一个新的临时线程来处理数据分发操作
        /// </summary>
        /// <param name="tItem_"></param>
        public void AddItem(T tItem_)
        {
            lock(_lkerIssue)
            {
                if(_quIssue.Count>MaxAllowSize)
                {
                    HandleToomanyItems();
                }

                _quIssue.Enqueue(tItem_);
                TryStartIssuer();
            }
        }

        /// <summary>
        /// 如果未到达最大值，则继续添加并返回true；否则返回false。
        /// </summary>
        /// <param name="tItem_"></param>
        /// <returns></returns>
        public bool AddItemIfNotFull(T tItem_)
        {
            lock (_lkerIssue)
            {
                if (_quIssue.Count >= MaxAllowSize)
                {
                    return false;
                }

                _quIssue.Enqueue(tItem_);
                TryStartIssuer();
                return true;
            }
        }

        private void HandleToomanyItems()
        {
            InvokeOnLogger(XLogSimple.LogLevels.Warn, "Too many items(Count:{0}, Max:{1}), {2}",
                _quIssue.Count, MaxAllowSize, _bRemovedWhenToomany ? "To remove half-header" : "To start tmp thread");
            if (_bRemovedWhenToomany)
            {
                if (ActRemovedItems == null)
                {
                    _quIssue.RemoveHeader(MaxAllowSize / 2);
                }
                else
                {
                    try
                    {
                        List<T> lstItems = new List<T>(MaxAllowSize / 2);
                        for (int i = 0; i < MaxAllowSize / 2; ++i)
                        {
                            lstItems.Add(_quIssue.Dequeue());
                        }
                        ActRemovedItems(lstItems);
                    }
                    catch (Exception ex)
                    {
                        InvokeOnExcept(ex, "ActRemovedItems");
                    }
                }
            }
            else
            {
                XThread.TryStartPriorityThread(ref _thrTmpWhenToomany, TmpIssueWhenToomany, Priority);
            }
        }

        private void TryStartIssuer()
        {
            if (!_bStarted) return;

            _evtIssue.Set();
            XThread.TryStartPriorityThread(ref _thrIssue, ItemIssueThread, Priority);
        }

        /// <summary>
        /// 清除所有元素
        /// </summary>
        /// <param name="actClear_"></param>
        public void Clear(Action<Queue<T>> actClear_=null)
        {
            InvokeOnLogger("#Clear");
            lock(_lkerIssue)
            {
                try
                {
                    if (actClear_ != null)
                        actClear_(_quIssue);
                    InvokeOnDebug("Clear {0} Item in Queue", _quIssue.Count);
                    _quIssue.Clear();
                }
                catch(Exception ex)
                {
                    InvokeOnExcept(ex, "Clear");
                }
            }
        }

        /// <summary>
        /// 重新启用，调用Stop后，如果想继续处理则需先调用此函数
        /// </summary>
        public void Restart()
        {
            InvokeOnCalled("Restart(MaxAllow:{0}, WhenTooMany:{1})",
                MaxAllowSize, (_bRemovedWhenToomany?"Removed":"StartTmpThread"));

            _bStarted = true;
            if(Count != 0)
            {
                TryStartIssuer();
            }
        }

        /// <summary>
        /// 停止：启动线程尝试处理完队列中的元素，并在处理完成后退出线程。
        /// </summary>
        public void Stop()
        {
            InvokeOnLogger("#Stop");
            _bStarted = false;
            _evtIssue.Set();
        }

        T GetIssueItem()
        {
            lock(_lkerIssue)
            {
                if (_quIssue.Count == 0)
                    return null;

                return _quIssue.Dequeue();
            }
        }

        T PeekIssueItem()
        {
            lock (_lkerIssue)
            {
                if (_quIssue.Count == 0)
                    return null;

                return _quIssue.Peek();
            }
        }

        void DelIssueItem()
        {
            lock(_lkerIssue)
            {
                if (_quIssue.Count == 0)    // Only happened, stopped imm after issueitem.
                    return;

                _quIssue.Dequeue();
            }
        }

        void TmpIssueWhenToomany()
        {
            try
            {
                InvokeOnLogger("Thread-Tmp Start");
                while (_bStarted)
                {
                    var tItem = GetIssueItem();
                    if (tItem == null)
                        break;

                    _funcIssue(tItem);
                }
            }
            catch (Exception ex)
            {
                InvokeOnExcept(ex);
            }

            _thrTmpWhenToomany = null;
            InvokeOnLogger("Thread-Tmp Stopped");
        }

        void ItemIssueThread()
        {
            try 
            {
                InvokeOnLogger("Thread Start");
                while (_bStarted)
                {
                    _evtIssue.WaitOne(_nInterval);
                    if (_bGetItemViaPeek)
                    {
                        while (true)
                        {
                            var tItem = PeekIssueItem();
                            if (tItem == null)
                                break;

                            if (_funcIssue(tItem))
                            {
                                DelIssueItem();
                            }
                            else
                            {   // Wait, next item add (or timeout) to try
                                break;
                            }
                        } // while(true)
                    }
                    else
                    {
                        while (true)
                        {
                            var tItem = GetIssueItem();
                            if (tItem == null)
                                break;
                            
                            _funcIssue(tItem);
                        }
                    } // else
                } // while
            }
            catch(Exception ex)
            {
                InvokeOnExcept(ex);
            }

            _thrIssue = null;
            InvokeOnLogger("Thread Stopped");
        }
    } // class
}
