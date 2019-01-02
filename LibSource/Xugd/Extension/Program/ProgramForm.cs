using System;
using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using SHCre.Xugd.CFile;
using SHCre.Xugd.Common;
using SHCre.Xugd.WinAPI;

namespace SHCre.Xugd.Extension
{
    partial class XProgram
    {
        /// <summary>
        /// Form相关操作
        /// </summary>
        internal static class ProgramForm
        {
            static object _synLog = new object();
            static EventWaitHandle _ewProcess=null;
            /// <summary>
            /// 主窗体
            /// </summary>
            public static Form FrmMain {get; internal set;}
            /// <summary>
            /// 如果是服务，则要设定服务的名称；否则，不要设定
            /// </summary>
            public static string ServiceName {get; set;}

            /// <summary>
            /// 判断进程能否启动，并根据HandleExistsMode来处理已有进程；
            /// 如果要处理已有进程，必须设定FrmMain
            /// </summary>
            /// <param name="strSynName_"></param>
            /// <param name="euMode_"></param>
            /// <returns></returns>
            public static bool CanStart(string strSynName_, HandleExistsMode euMode_= HandleExistsMode.Remain)
            {
                try
                {
                    _ewProcess = EventWaitHandle.OpenExisting(strSynName_);
                    // Form has start, close it
                    if (euMode_ != HandleExistsMode.Remain)
                        _ewProcess.Set();

                    if(euMode_ == HandleExistsMode.Remain || euMode_==HandleExistsMode.Restore)
                    {
                        _ewProcess.Close();
                        _ewProcess = null;
                        return false;
                    }
                }
                catch (WaitHandleCannotBeOpenedException)
                {
                }

                if(_ewProcess==null)
                    _ewProcess = new EventWaitHandle(false, EventResetMode.ManualReset, strSynName_);
                Thread thrHandleForm = new Thread(new ParameterizedThreadStart(WaitHandleForm));
                thrHandleForm.IsBackground = true;
                thrHandleForm.Start(euMode_);

                return true;
            }

            public static void TryStop(string strSynName_)
            {
                try
                {
                    _ewProcess = EventWaitHandle.OpenExisting(strSynName_);
                    _ewProcess.Set();
                    _ewProcess.Close();
                }
                catch (WaitHandleCannotBeOpenedException)
                {
                }
            }

            static void WaitHandleForm(object oParam_)
            {
                HandleExistsMode euMode = (HandleExistsMode)oParam_;
                try
                {
                    while (true)
                    {
                        _ewProcess.WaitOne();
                        _ewProcess.Reset();
                        if (FrmMain == null) // From has not start
                            continue;
                        if (FrmMain.IsDisposed)
                            break;

                        switch(euMode)
                        {
                            case HandleExistsMode.Restore:
                                FrmMain.Invoke(new MethodInvoker(() =>
                                {
                                    FrmMain.Show();
                                    XWin.ShowAtTop(FrmMain);
                                }));
                                break;
                            case HandleExistsMode.Close:
                                FrmMain.Close();
                                break;
                            default:
                                break;
                        }
                    }

                    _ewProcess.Close();
                    _ewProcess = null;
                }
                catch { }
            }

            /// <summary>
            /// 处理所有异常（UnhandledException、ThreadException）
            /// </summary>
            [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.ControlAppDomain)]
            public static void HandleAllException()
            {
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            }

            static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
            {
                if (Debugger.IsAttached)
                    throw e.ExceptionObject as Exception;

                LogError(string.Format("Unhandled Exception\r\nIsTerminating:{0}\r\nExceptionObject:{1}", e.IsTerminating, e.ExceptionObject));
                if(UnhandledException != null)
                    UnhandledException(sender, e);

                CheckException();
            }

            static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
            {
                if (Debugger.IsAttached)
                    throw e.Exception;

                LogError("ThreadException:" + e.Exception);
                if (ThreadException != null)
                    ThreadException(sender, e);

                CheckException();
            }

            static void LogError(string strLog_)
            {
                try 
                {
                    lock(_synLog)
                    {
                        if(!string.IsNullOrEmpty(ErrorLogFile))
                        {
                            string strName = XPath.GetFullPath(ErrorLogFile);
                            File.AppendAllText(strName,
                                string.Format("{0}\t{1}{2}{2}", XTime.GetFullString(DateTime.Now, true), strLog_, XString.NewLine));
                        }
                    }
                }
                catch {}
            }

            static void CheckException()
            {
                try
                {
                    if (ExceptionRestart)
                    {
                        if (!string.IsNullOrEmpty(ServiceName)) // IsService
                        {
                            XProcess.StartService(Application.StartupPath, ServiceName, ExitWaitSeconds + 5);

                            // 在服务重启时，要总是保证服务能退掉；如果没有设定EnsureExit
                            // 则程序退出时不会保证退出，所以在此处保证退出
                            if (!EnsureExit)
                                XProcess.ExitEnsure(Application.StartupPath, ExitWaitSeconds);
                            Application.Exit();
                        }
                        else // Program
                        {
                            Application.Restart();
                        }
                    }
                    else
                    {
                        if (EnsureExit)
                            XProcess.ExitEnsure(Application.StartupPath, ExitWaitSeconds);
                        Application.Exit();
                    }
                }
                catch{}
            }
        } // Program
    } // XProgram
}
