using System.Threading;
using System.Windows.Forms;

using SHCre.Xugd.WinAPI;

namespace SHCre.Xugd.XControl
{
    /// <summary>
    /// 窗体相关的操作
    /// </summary>
    public class XForm
    {
        struct FormCloseParam
        {
            public Form frmOwner;
            public EventWaitHandle eHandle;
            public DelNonParam delToClose;
        };

        struct FormRestartParam
        {
            public Form frmOwner;
            public bool bRestore;
            public EventWaitHandle eHandle;
            public DelNonParam delRestart;
        };

        /// <summary>
        /// 判断窗体是否已启动，如果启动直接退出：
        /// 一般在From.Load中调用
        /// if(IsFormExists(...))
        /// {
        ///     this.Close();
        ///     return;
        /// }
        /// </summary>
        /// <param name="strEvent_">用于窗体互斥的事件名称</param>
        /// <returns>true，窗体已存在，自动退出；false，窗体不存在，启动一个新的</returns>
        public static bool IsFormExists(string strEvent_)
        {
            EventWaitHandle eHandle;
            try
            {
                eHandle = EventWaitHandle.OpenExisting(strEvent_);
                // Form has start, close it
                eHandle.Close();
                return true;
            }
            catch (WaitHandleCannotBeOpenedException)
            {
            }

            eHandle = new EventWaitHandle(false, EventResetMode.ManualReset, strEvent_);
            Thread thrRestore = new Thread(new ParameterizedThreadStart(WaitOnly));
            thrRestore.IsBackground = true;
            thrRestore.Start(eHandle);

            return false;
        }

        /// <summary>
        /// 判断窗体是否已启动，如果启动则激活已启动窗体，否则启动一个新的窗体：
        /// 一般在From.Load中调用
        /// if(IsFormExists(...))
        ///     return;
        /// </summary>
        /// <param name="frmOwner_">窗体，一般传this即可</param>
        /// <param name="strEvent_">用于窗体互斥的事件名称</param>
        /// <returns>true，窗体已存在，激活以存在的窗体；false，窗体不存在，启动一个新的</returns>
        public static bool IsFormExists(Form frmOwner_, string strEvent_)
        {
            return IsFormExists(frmOwner_, strEvent_, null, true);
        }

        /// <summary>
        /// 判断窗体是否已启动，如果启动则调用delRestart_，并根据bRestore_决定是否激活已启动窗体；否则启动一个新的窗体：
        /// 一般在From.Load中调用
        /// if(IsFormExists(...))
        ///     return;
        /// </summary>
        /// <param name="frmOwner_">窗体，一般传this即可</param>
        /// <param name="strEvent_">用于窗体互斥的事件名称</param>
        /// <param name="delRestart_">如果不为null，则再次启动窗体时会自动调用</param>
        /// <param name="bRestore_">再次启动窗体时是否激活已启动窗体</param>
        /// <returns>true，窗体已存在；false，窗体不存在，启动一个新的</returns>
        public static bool IsFormExists(Form frmOwner_, string strEvent_, DelNonParam delRestart_, bool bRestore_)
        {
            EventWaitHandle eHandle;
            try
            {
                eHandle = EventWaitHandle.OpenExisting(strEvent_);
                // Form has start, close it
                eHandle.Set();
                eHandle.Close();
                frmOwner_.Close();
                return true;
            }
            catch (WaitHandleCannotBeOpenedException)
            {
            }

            eHandle = new EventWaitHandle(false, EventResetMode.ManualReset, strEvent_);
            Thread thrRestore = new Thread(new ParameterizedThreadStart(RestoreForm));
            thrRestore.IsBackground = true;
            thrRestore.Priority = ThreadPriority.AboveNormal;
            FormRestartParam stParam = new FormRestartParam();
            stParam.frmOwner = frmOwner_;
            stParam.bRestore = bRestore_;
            stParam.eHandle = eHandle;
            stParam.delRestart = delRestart_;
            thrRestore.Start(stParam);

            return false;
        }

        /// <summary>
        /// 窗体启动，创建信号并等待退出
        /// </summary>
        /// <param name="frmOwner_">窗体句柄</param>
        /// <param name="strEvent_">事件名，窗体启动时创建并等待，在事件有信号时窗体退出</param>
        public static void FormStartAndWait(Form frmOwner_, string strEvent_)
        {
            FormStartAndWait(frmOwner_, strEvent_, null);
        }

        /// <summary>
        /// 窗体启动，创建信号并等待退出
        /// </summary>
        /// <param name="frmOwner_">窗体句柄</param>
        /// <param name="strEvent_">事件名，窗体启动时创建并等待，在事件有信号时窗体退出</param>
        /// <param name="delToClose_">窗体退出时，调用的委托</param>
        public static void FormStartAndWait(Form frmOwner_, string strEvent_, DelNonParam delToClose_)
        {
            EventWaitHandle eHandle = new EventWaitHandle(false, EventResetMode.ManualReset, strEvent_);
            Thread thrClose = new Thread(new ParameterizedThreadStart(CloseForm));
            thrClose.IsBackground = true;
            FormCloseParam stParam = new FormCloseParam();
            stParam.frmOwner = frmOwner_;
            stParam.eHandle = eHandle;
            stParam.delToClose = delToClose_;
            thrClose.Start(stParam);
        }

        /// <summary>
        /// 通过设定事件信号，退出所有等待事件的窗体
        /// </summary>
        /// <param name="strEvent_">事件名称</param>
        public static void CloseFormBySetEvent(string strEvent_)
        {
            try
            {
                EventWaitHandle eHandle;
                eHandle = EventWaitHandle.OpenExisting(strEvent_);
                // Form has start, close it
                eHandle.Set();
                eHandle.Close();
            }
            catch (WaitHandleCannotBeOpenedException)
            {
            }
        }

        /// <summary>
        /// 判断窗体能否启动（事件strEvent_是否已创建，创建过了说明能启动；否则不能启动），
        /// 如果窗体能启动，启动后会自动等待退出事件，如果等到则关闭窗体（From.Close())：
        /// 一般在From.Load中调用
        /// if(！CanFormStart(...))
        ///     return;
        /// </summary>
        /// <param name="frmOwner_">窗体，一般传this即可</param>
        /// <param name="strEvent_">用于等待窗体退出的事件名称</param>
        /// <param name="delCannotStart_">如果窗体无法启动时，调用的委托（一般用于给出Message提示，说明原因）</param>
        /// <returns>true，事件已存在，可以启动；否则，事件不存在，不能启动</returns>
        public static bool CanFormStart(Form frmOwner_, string strEvent_, DelNonParam delCannotStart_)
        {
            return CanFormStart(frmOwner_, strEvent_, delCannotStart_, null);
        }

        /// <summary>
        /// 判断窗体能否启动（事件strEvent_是否已创建，创建过了说明能启动；否则不能启动），
        /// 如果窗体能启动，启动后会自动等待退出事件，如果等到则关闭窗体（From.Close())：
        /// 一般在From.Load中调用
        /// if(！CanFormStart(...))
        ///     return;
        /// </summary>
        /// <param name="frmOwner_">窗体，一般传this即可</param>
        /// <param name="strEvent_">用于等待窗体退出的事件名称</param>
        /// <param name="delCannotStart_">如果窗体无法启动时，调用的委托（一般用于给出Message提示，说明原因）</param>
        /// <param name="delToClose_">在窗体退出时，调用的委托</param>
        /// <returns>true，事件已存在，可以启动；否则，事件不存在，不能启动</returns>
        public static bool CanFormStart(Form frmOwner_, string strEvent_, DelNonParam delCannotStart_, DelNonParam delToClose_)
        {
            try
            {
                EventWaitHandle eHandle = EventWaitHandle.OpenExisting(strEvent_);
                // The caller has start, start now
                if( !eHandle.WaitOne(0, false) )
                {
                    Thread thrClose = new Thread(new ParameterizedThreadStart(CloseForm));
                    thrClose.IsBackground = true;
                    FormCloseParam stParam = new FormCloseParam();
                    stParam.frmOwner = frmOwner_;
                    stParam.eHandle = eHandle;
                    stParam.delToClose = delToClose_;
                    thrClose.Start(stParam);

                    return true;
                }

                // Has close-signal, so we not need to start the form
                eHandle.Close();
            }
            catch (WaitHandleCannotBeOpenedException)
            {
            }

            if (delCannotStart_ != null)
                frmOwner_.Invoke(delCannotStart_);
            frmOwner_.Close();

            return false;
        }

        /// <summary>
        /// 没有参数，返回void的委托
        /// </summary>
        public delegate void DelNonParam();

        /// <summary>
        /// 只是防止事件释放而用的空线程
        /// </summary>
        /// <param name="oParam_">事件句柄</param>
        static void WaitOnly(object oParam_)
        {
            try
            {
                EventWaitHandle eHandle = (EventWaitHandle)oParam_;
                while (true)
                {
                    eHandle.WaitOne();
                    eHandle.Reset();
                }
            }
            catch { }
        }

        static void RestoreForm(object oParam_)
        {
            FormRestartParam stParam = (FormRestartParam)oParam_;
            try
            {
                while (true)
                {
                    stParam.eHandle.WaitOne();
                    stParam.eHandle.Reset();

                    if (stParam.delRestart != null)
                        stParam.frmOwner.Invoke(stParam.delRestart);

                    if( stParam.bRestore )
                    {
                        stParam.frmOwner.Invoke(new DelNonParam(delegate()
                                       {
                                           XWin.ShowAtTop(stParam.frmOwner);
                                       })
                                   );
                    }           
                }
            }
            catch { }
        }

        static void CloseForm(object oParam_)
        {
            FormCloseParam stParam = (FormCloseParam)oParam_;
            try
            {
                stParam.eHandle.WaitOne();

                if (stParam.delToClose != null)
                    stParam.frmOwner.Invoke(stParam.delToClose);

                stParam.frmOwner.Invoke(new DelNonParam(delegate()
                    {
                        foreach (Form frmOpened in Application.OpenForms)
                            frmOpened.Close();
                    })
                );

                stParam.eHandle.Close();
            }
            catch { }
        }
    }
}
