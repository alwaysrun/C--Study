using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Diagnostics;

namespace SHCre.Xugd.Extension
{
    /// <summary>
    /// 激发事件或方法
    /// </summary>
    static class XInvoker
    {
        /// <summary>
        /// 默认激发事件
        /// </summary>
        /// <param name="delMethods_">要激发的事件</param>
        /// <typeparam name="T"></typeparam>
        /// <param name="tSender_">激发事件的类</param>
        /// <param name="eArg_">参数列表</param>
        public static void InvokeEvent<T>(MulticastDelegate delMethods_, T tSender_, EventArgs eArg_)where T:class
        {
            if (delMethods_ == null) return;

            object[] argParams = new object[] { tSender_, eArg_ };
            foreach(Delegate del in delMethods_.GetInvocationList())
            {
                ISynchronizeInvoke synInvoker = del.Target as ISynchronizeInvoke;
                if (synInvoker != null)
                {
                    synInvoker.BeginInvoke(del, argParams);
                }
                else
                {
                    DependencyObject objDepend = del.Target as DependencyObject;
                    if(objDepend != null && objDepend.Dispatcher!=null)
                    {
                        objDepend.Dispatcher.BeginInvoke(del, argParams);
                    }
                    else
                    {
                        del.DynamicInvoke(argParams);
                    }
                }
            }
        }

        /// <summary>
        /// 尽力激发每个事件，无异常抛出
        /// </summary>
        /// <param name="delMethods_">要激发的事件</param>
        /// <typeparam name="T"></typeparam>
        /// <param name="tSender_">激发事件的类</param>
        /// <param name="eArg_">参数列表</param>
        public static void InvokeEventTried<T>(MulticastDelegate delMethods_, T tSender_, EventArgs eArg_)where T:class
        {
            if (delMethods_ == null) return;

            object[] argParams = new object[] { tSender_, eArg_ };
            foreach (Delegate del in delMethods_.GetInvocationList())
            {
                try
                {
                    ISynchronizeInvoke synInvoker = del.Target as ISynchronizeInvoke;
                    if (synInvoker != null)
                    {
                        synInvoker.BeginInvoke(del, argParams);
                    }
                    else
                    {
                        DependencyObject objDepend = del.Target as DependencyObject;
                        if (objDepend != null && objDepend.Dispatcher != null)
                        {
                            objDepend.Dispatcher.BeginInvoke(del, argParams);
                        }
                        else
                        {
                            del.DynamicInvoke(argParams);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message, "SHCre.Xugd.Extension.XInvoker");
                }
            }
        }

        /// <summary>
        /// 通过ISynchronizeInvoke激发事件
        /// </summary>
        /// <param name="synInvoker_"></param>
        /// <param name="delMethods_"></param>
        /// <typeparam name="T"></typeparam>
        /// <param name="tSender_">激发事件的类</param>
        /// <param name="eArg_">参数列表</param>
        public static void InvokeEvent<T>(ISynchronizeInvoke synInvoker_, MulticastDelegate delMethods_, T tSender_, EventArgs eArg_)where T:class
        {
            if (delMethods_ == null || synInvoker_==null) return;

            object[] argParams = new object[] { tSender_, eArg_ };
            foreach(Delegate del in delMethods_.GetInvocationList())
            {
                synInvoker_.BeginInvoke(del, argParams);
            }
        }

        /// <summary>
        /// 通过Dispatcher激发事件
        /// </summary>
        /// <param name="dispInvoker_"></param>
        /// <param name="delMethods_"></param>
        /// <typeparam name="T"></typeparam>
        /// <param name="tSender_">激发事件的类</param>
        /// <param name="eArg_">参数列表</param>
        public static void InvokeEvent<T>(Dispatcher dispInvoker_, MulticastDelegate delMethods_, T tSender_, EventArgs eArg_) where T : class
        {
            if (delMethods_ == null || dispInvoker_ == null) return;

            object[] argParams = new object[] { tSender_, eArg_ };
            foreach (Delegate del in delMethods_.GetInvocationList())
            {
                dispInvoker_.BeginInvoke(del, argParams);
            }
        }

        /// <summary>
        /// 激发操作
        /// </summary>
        /// <param name="actMethod_"></param>
        public static void InvokeMethod(Action actMethod_)
        {
            if (actMethod_ == null) return;

            actMethod_();
        }

        /// <summary>
        /// 通过Dispatcher激发操作
        /// </summary>
        /// <param name="dispInvoker_"></param>
        /// <param name="actMethod_"></param>
        public static void InvokeMethod(Dispatcher dispInvoker_, Action actMethod_)
        {
            if (dispInvoker_ == null || actMethod_ == null) return;

            dispInvoker_.BeginInvoke(new MethodInvoker(() => { actMethod_(); }), new object[0]);
        }

        /// <summary>
        /// 通过ISynchronizeInvoke激发操作
        /// </summary>
        /// <param name="synInvoker_"></param>
        /// <param name="actMethod_"></param>
        public static void InvokeMethod(ISynchronizeInvoke synInvoker_, Action actMethod_)
        {
            if (synInvoker_ == null || actMethod_ == null) return;

            synInvoker_.BeginInvoke(new MethodInvoker(() => { actMethod_(); }), new object[0]);
        }
    } // XInvoker
}
