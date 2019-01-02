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
    /// 优先级接口
    /// </summary>
    public interface IXPorityQueueItemBase
    {
        /// <summary>
        /// 用于比较的优先级：0表示默认优先级，自动放在队尾
        /// </summary>
        /// <returns></returns>
        int AnchorPriority();
    }

    /// <summary>
    /// 优先级队列（使用AddItem添加元素后，自动启用线程调用ActIssue来分发处理）；
    /// 添加元素（AddItem）后自动启动分发线程；调用Stop后，可重新调用Restart来重新
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XPriorityQueue<T> : XLogEventsBase where T : class, IXPorityQueueItemBase
    {
        #region "Var"
        readonly object _lkerIssue = new object();
        LinkedList<T> _quIssue = new LinkedList<T>();
        Thread _thrIssue = null;
        AutoResetEvent _evtIssue = new AutoResetEvent(false);

        Func<T, bool> _funcIssue = null;
        int _nInterval = 0;
        bool _bStarted = true;
        #endregion

        #region "Property"
        /// <summary>
        /// 设定发布线程的优先级
        /// </summary>
        public ThreadPriority Priority { get;set; }

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

                    return _quIssue.First.Value;
                }
            }
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="funIssue_">分发处理函数（在线程中调用）：如果GetItemViaPeek，只有返回true时才真正从队列中移除元素</param>
        /// <param name="nCheckSeconds_">尝试处理最大间隔时间（即使没有信号，也会在此时间间隔后尝试查看队列中是否有未处理的元素）</param>
        public XPriorityQueue(Func<T, bool> funIssue_, int nCheckSeconds_ = 600)
        {
            if (funIssue_ == null)
                throw new ArgumentNullException("Action for issue queue can not NULL");

            if (nCheckSeconds_ < 10)
                nCheckSeconds_ = 10;

            Priority = ThreadPriority.Normal;
            _funcIssue = funIssue_;
            _nInterval = XTime.Second2Interval(nCheckSeconds_);

            BuildLogPrefix<T>("XPriorityQueue");
        }

        /// <summary>
        /// 添加元素：添加后会自动启动线程进行分发处理；
        /// 如果AnchorPriority不为0，在根据AnchorPriority由大到小的顺序插入到队列中。
        /// </summary>
        /// <param name="tItem_"></param>
        public void AddItem(T tItem_)
        {
            lock(_lkerIssue)
            {
                if(tItem_.AnchorPriority() == 0)
                {
                    _quIssue.AddLast(tItem_);
                }
                else
                {
                    LinkedListNode<T> firstNode = null;
                    var firstItem = _quIssue.FirstOrDefault(z => z.AnchorPriority() < tItem_.AnchorPriority());
                    if (firstItem != null)
                        firstNode = _quIssue.Find(firstItem);

                    if (firstNode == null)
                    {
                        _quIssue.AddLast(tItem_);
                    }
                    else
                    {
                        _quIssue.AddBefore(firstNode, tItem_);
                    }
                }

                TryStartIssuer();
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
        public void Clear(Action<LinkedList<T>> actClear_ = null)
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
            InvokeOnCalled("Restart");

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
            InvokeOnCalled("Stop");
            _bStarted = false;
            _evtIssue.Set();
        }

        T GetIssueItem()
        {
            lock(_lkerIssue)
            {
                if (_quIssue.Count == 0)
                    return null;

                return _quIssue.First.Value;
            }
        }

        void ItemIssueThread()
        {
            try
            {
                InvokeOnLogger("Thread Start");
                while (_bStarted)
                {
                    _evtIssue.WaitOne(_nInterval);

                    while (true)
                    {
                        var tItem = GetIssueItem();
                        if (tItem == null)
                            break;

                        _funcIssue(tItem);
                    }
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
