using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SHCre.Xugd.Common
{
    /// <summary>
    /// 处理线程相关操作
    /// </summary>
    public static class XThread
    {
        /// <summary>
        /// 放入线程池中等待执行；放入失败抛出NotSupportedException
        /// </summary>
        /// <param name="thrStart_">要执行的方法</param>
        public static void StartInPool(ThreadStart thrStart_)
        {
            ThreadPool.QueueUserWorkItem(objNone => { thrStart_(); }, null);
        }

        /// <summary>
        /// 放入线程池中等待执行；放入失败抛出NotSupportedException
        /// </summary>
        /// <param name="thrStart_">要执行的方法</param>
        /// <param name="obj_">要传递的参数</param>
        [Obsolete("Use StartPool instead")]
        public static void StartInPool(ParameterizedThreadStart thrStart_, object obj_)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(thrStart_), obj_);
        }

        /// <summary>
        /// 将方法放入线程池，等待执行；放入失败抛出NotSupportedException
        /// </summary>
        /// <param name="callBack_"></param>
        /// <param name="obj_"></param>
        public static void StartPool(WaitCallback callBack_, object obj_)
        {
            ThreadPool.QueueUserWorkItem(callBack_, obj_);
        }

        /// <summary>
        /// 将方法放入线程池，等待执行；放入失败抛出NotSupportedException
        /// </summary>
        /// <param name="callBack_"></param>
        public static void StartPool(WaitCallback callBack_)
        {
            ThreadPool.QueueUserWorkItem(callBack_);
        }

        /// <summary>
        /// 启动后台线程来执行方法
        /// </summary>
        /// <param name="thrStart_">要执行的方法</param>
        /// <returns>新启动的线程</returns>
        public static Thread StartThread(ThreadStart thrStart_)
        {
            return StartPriorityThread(thrStart_, ThreadPriority.Normal);
        }

        /// <summary>
        /// 启动带优先级的后台线程
        /// </summary>
        /// <param name="thrStart_"></param>
        /// <param name="euPrio_"></param>
        /// <returns></returns>
        public static Thread StartPriorityThread(ThreadStart thrStart_, ThreadPriority euPrio_= ThreadPriority.Normal)
        {
            Thread thr = new Thread(thrStart_);
            thr.IsBackground = true;
            thr.Priority = euPrio_;
            thr.Start();

            return thr;
        }

        /// <summary>
        /// 尝试启动后台进程，如果进程已存在（IsAlive）则不错任何操作
        /// </summary>
        /// <param name="thrHandle_">指向进程的句柄</param>
        /// <param name="thrStart_">要执行的方法</param>
        /// <returns>线程已经运行返回false，成功启动新线程返回true</returns>
        public static bool TryStartThread(ref Thread thrHandle_, ThreadStart thrStart_)
        {
            if (thrHandle_ != null && thrHandle_.IsAlive) return false;

            thrHandle_ = StartPriorityThread(thrStart_, ThreadPriority.Normal);
            return true;
        }

        /// <summary>
        /// 尝试启动有优先级的后台进程，如果进程已存在（IsAlive）则不错任何操作
        /// </summary>
        /// <param name="thrHandle_"></param>
        /// <param name="thrStart_"></param>
        /// <param name="euPrio_"></param>
        /// <returns></returns>
        public static bool TryStartPriorityThread(ref Thread thrHandle_, ThreadStart thrStart_, ThreadPriority euPrio_ = ThreadPriority.Normal)
        {
            if (thrHandle_ != null && thrHandle_.IsAlive) return false;

            thrHandle_ = StartPriorityThread(thrStart_, euPrio_);
            return true;
        }

        /// <summary>
        /// 启动后台线程来执行方法
        /// </summary>
        /// <param name="thrStart_">要执行的方法</param>
        /// <param name="obj_">方法所需参数</param>
        /// <returns>新启动的线程</returns>
        public static Thread StartThread(ParameterizedThreadStart thrStart_, object obj_)
        {
            return StartPriorityThread(thrStart_, obj_, ThreadPriority.Normal);
        }

        /// <summary>
        /// 启动带优先级的后台线程
        /// </summary>
        /// <param name="thrStart_"></param>
        /// <param name="obj_"></param>
        /// <param name="euPrio_"></param>
        /// <returns></returns>
        public static Thread StartPriorityThread(ParameterizedThreadStart thrStart_, object obj_, ThreadPriority euPrio_ = ThreadPriority.Normal)
        {
            Thread thr = new Thread(thrStart_);
            thr.IsBackground = true;
            thr.Priority = euPrio_;
            thr.Start(obj_);

            return thr;
        }

        /// <summary>
        /// 尝试启动后台进程，如果进程已存在（IsAlive）则不错任何操作
        /// </summary>
        /// <param name="thrHandle_">指向进程的句柄</param>
        /// <param name="thrStart_">要执行的方法</param>
        /// <param name="obj_">方法所需参数</param>
        /// <returns>线程已经运行返回false，成功启动新线程返回true</returns>
        public static bool TryStartThread(ref Thread thrHandle_, ParameterizedThreadStart thrStart_, object obj_)
        {
            if (thrHandle_ != null && thrHandle_.IsAlive) return false;

            thrHandle_ = StartPriorityThread(thrStart_, obj_, ThreadPriority.Normal);
            return true;
        }

        /// <summary>
        /// 尝试启动有优先级的后台进程，如果进程已存在（IsAlive）则不错任何操作
        /// </summary>
        /// <param name="thrHandle_"></param>
        /// <param name="thrStart_"></param>
        /// <param name="obj_"></param>
        /// <param name="euPrio_"></param>
        /// <returns>线程已经运行返回false，成功启动新线程返回true</returns>
        public static bool TryStartPriorityThread(ref Thread thrHandle_, ParameterizedThreadStart thrStart_, object obj_, ThreadPriority euPrio_ = ThreadPriority.Normal)
        {
            if (thrHandle_ != null && thrHandle_.IsAlive) return false;

            thrHandle_ = StartPriorityThread(thrStart_, obj_, euPrio_);
            return true;
        }
    } // XThread
}
