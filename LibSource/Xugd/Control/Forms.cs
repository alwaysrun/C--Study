using System.Threading;
using System.Windows.Forms;

using SHCre.Xugd.WinAPI;

namespace SHCre.Xugd.XControl
{
    /// <summary>
    /// ������صĲ���
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
        /// �жϴ����Ƿ����������������ֱ���˳���
        /// һ����From.Load�е���
        /// if(IsFormExists(...))
        /// {
        ///     this.Close();
        ///     return;
        /// }
        /// </summary>
        /// <param name="strEvent_">���ڴ��廥����¼�����</param>
        /// <returns>true�������Ѵ��ڣ��Զ��˳���false�����岻���ڣ�����һ���µ�</returns>
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
        /// �жϴ����Ƿ�����������������򼤻����������壬��������һ���µĴ��壺
        /// һ����From.Load�е���
        /// if(IsFormExists(...))
        ///     return;
        /// </summary>
        /// <param name="frmOwner_">���壬һ�㴫this����</param>
        /// <param name="strEvent_">���ڴ��廥����¼�����</param>
        /// <returns>true�������Ѵ��ڣ������Դ��ڵĴ��壻false�����岻���ڣ�����һ���µ�</returns>
        public static bool IsFormExists(Form frmOwner_, string strEvent_)
        {
            return IsFormExists(frmOwner_, strEvent_, null, true);
        }

        /// <summary>
        /// �жϴ����Ƿ���������������������delRestart_��������bRestore_�����Ƿ񼤻����������壻��������һ���µĴ��壺
        /// һ����From.Load�е���
        /// if(IsFormExists(...))
        ///     return;
        /// </summary>
        /// <param name="frmOwner_">���壬һ�㴫this����</param>
        /// <param name="strEvent_">���ڴ��廥����¼�����</param>
        /// <param name="delRestart_">�����Ϊnull�����ٴ���������ʱ���Զ�����</param>
        /// <param name="bRestore_">�ٴ���������ʱ�Ƿ񼤻�����������</param>
        /// <returns>true�������Ѵ��ڣ�false�����岻���ڣ�����һ���µ�</returns>
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
        /// ���������������źŲ��ȴ��˳�
        /// </summary>
        /// <param name="frmOwner_">������</param>
        /// <param name="strEvent_">�¼�������������ʱ�������ȴ������¼����ź�ʱ�����˳�</param>
        public static void FormStartAndWait(Form frmOwner_, string strEvent_)
        {
            FormStartAndWait(frmOwner_, strEvent_, null);
        }

        /// <summary>
        /// ���������������źŲ��ȴ��˳�
        /// </summary>
        /// <param name="frmOwner_">������</param>
        /// <param name="strEvent_">�¼�������������ʱ�������ȴ������¼����ź�ʱ�����˳�</param>
        /// <param name="delToClose_">�����˳�ʱ�����õ�ί��</param>
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
        /// ͨ���趨�¼��źţ��˳����еȴ��¼��Ĵ���
        /// </summary>
        /// <param name="strEvent_">�¼�����</param>
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
        /// �жϴ����ܷ��������¼�strEvent_�Ƿ��Ѵ�������������˵������������������������
        /// �����������������������Զ��ȴ��˳��¼�������ȵ���رմ��壨From.Close())��
        /// һ����From.Load�е���
        /// if(��CanFormStart(...))
        ///     return;
        /// </summary>
        /// <param name="frmOwner_">���壬һ�㴫this����</param>
        /// <param name="strEvent_">���ڵȴ������˳����¼�����</param>
        /// <param name="delCannotStart_">��������޷�����ʱ�����õ�ί�У�һ�����ڸ���Message��ʾ��˵��ԭ��</param>
        /// <returns>true���¼��Ѵ��ڣ����������������¼������ڣ���������</returns>
        public static bool CanFormStart(Form frmOwner_, string strEvent_, DelNonParam delCannotStart_)
        {
            return CanFormStart(frmOwner_, strEvent_, delCannotStart_, null);
        }

        /// <summary>
        /// �жϴ����ܷ��������¼�strEvent_�Ƿ��Ѵ�������������˵������������������������
        /// �����������������������Զ��ȴ��˳��¼�������ȵ���رմ��壨From.Close())��
        /// һ����From.Load�е���
        /// if(��CanFormStart(...))
        ///     return;
        /// </summary>
        /// <param name="frmOwner_">���壬һ�㴫this����</param>
        /// <param name="strEvent_">���ڵȴ������˳����¼�����</param>
        /// <param name="delCannotStart_">��������޷�����ʱ�����õ�ί�У�һ�����ڸ���Message��ʾ��˵��ԭ��</param>
        /// <param name="delToClose_">�ڴ����˳�ʱ�����õ�ί��</param>
        /// <returns>true���¼��Ѵ��ڣ����������������¼������ڣ���������</returns>
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
        /// û�в���������void��ί��
        /// </summary>
        public delegate void DelNonParam();

        /// <summary>
        /// ֻ�Ƿ�ֹ�¼��ͷŶ��õĿ��߳�
        /// </summary>
        /// <param name="oParam_">�¼����</param>
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
