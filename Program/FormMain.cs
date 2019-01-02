using System;
using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using SHCre.Xugd.Common;
using SHCre.Xugd.WinAPI;

namespace SHCre.Xugd.Program
{
    /// <summary>
    /// 用于From窗体程序启动处理的类
    /// </summary>
    public static class XFormMain
    {
        /// <summary>
        /// 强制退出或重启前等待的时间
        /// </summary>
        public static int ExitWaitSeconds { get; set; }
        /// <summary>
        /// 是否强制保证退出，如果是则等待ExitWaitSeconds后，直接杀掉进程
        /// </summary>
        public static bool EnsureExit { get; set; }
        /// <summary>
        /// 抛出异常时记录错误的日志文件，默认为Error.log
        /// </summary>
        public static string ErrorLogFile { get; set; }
        /// <summary>
        /// 遇到异常时，是否重启
        /// </summary>
        public static bool ExceptionRestart { get; set; }

        static XFormMain()
        {
            ExitWaitSeconds = 15;
            ErrorLogFile = "Error.log";
        }

        /// <summary>
        /// 如果进程已存在，如何处理
        /// </summary>
        public enum HandleExistsMode
        {
            /// <summary>
            /// 如果已有进程，不做任何处理；当前进程不启动
            /// </summary>
            Remain = 0,
            /// <summary>
            /// 如果已有进程，则恢复；当前进程不启动
            /// </summary>
            Restore,
            /// <summary>
            /// 如果已有进程，则关闭；并启动当前进程
            /// </summary>
            Close,
            /// <summary>
            /// 总是启动当前进程(允许启动多个实例)，不对已有进程有任何影响
            /// </summary>
            StillStart,
        }

        /// <summary>
        /// 运行Form程序；
        /// 直接把Program文件中的main函数内容全部注释掉，调用此函数即可。
        /// 如果一运行就出错，则说明Form的构造函数中有异常，对构造函数进行单步调试查找即可。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="euMode_"></param>
        /// <param name="strSynName_">互斥体名（如果为空或null，则通过文件路径生成唯一的互斥体名称）</param>
        public static void Run<T>(HandleExistsMode euMode_ = HandleExistsMode.Remain, string strSynName_ = null) where T : Form, new()
        {
            if (string.IsNullOrEmpty(strSynName_))
            {
                strSynName_ = XProcess.GetSynNameFromFileName(Application.ExecutablePath);
            }

            if (euMode_ == HandleExistsMode.StillStart || ProgramForm.CanStart(strSynName_, euMode_))
            {
                Application.EnableVisualStyles();
                ProgramForm.HandleAllException();
                T frm = Activator.CreateInstance<T>();
                ProgramForm.FrmMain = frm;
                Application.Run(frm);

                if (EnsureExit)
                    XProcess.ExitEnsure(Application.StartupPath, ExitWaitSeconds);
            }
        }

        /// <summary>
        /// Form相关操作
        /// </summary>
        internal static class ProgramForm
        {
            static object _synLog = new object();
            static EventWaitHandle _ewProcess = null;
            /// <summary>
            /// 主窗体
            /// </summary>
            public static Form FrmMain { get; internal set; }

            /// <summary>
            /// 判断进程能否启动，并根据HandleExistsMode来处理已有进程；
            /// 如果要处理已有进程，必须设定FrmMain
            /// </summary>
            /// <param name="strSynName_"></param>
            /// <param name="euMode_"></param>
            /// <returns></returns>
            public static bool CanStart(string strSynName_, HandleExistsMode euMode_ = HandleExistsMode.Remain)
            {
                try
                {
                    _ewProcess = EventWaitHandle.OpenExisting(strSynName_);
                    // Form has start, close it
                    if (euMode_ != HandleExistsMode.Remain)
                        _ewProcess.Set();

                    if (euMode_ == HandleExistsMode.Remain || euMode_ == HandleExistsMode.Restore)
                    {
                        _ewProcess.Close();
                        _ewProcess = null;
                        return false;
                    }
                }
                catch (WaitHandleCannotBeOpenedException)
                {
                }

                if (_ewProcess == null)
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

                        switch (euMode)
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
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
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

                CheckException();
            }

            static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
            {
                if (Debugger.IsAttached)
                    throw e.Exception;

                LogError("ThreadException:" + e.Exception);

                CheckException();
            }

            static void LogError(string strLog_)
            {
                try
                {
                    lock (_synLog)
                    {
                        if (!string.IsNullOrEmpty(ErrorLogFile))
                        {
                            if (!Path.IsPathRooted(ErrorLogFile))
                                ErrorLogFile = Path.Combine(Application.StartupPath, ErrorLogFile);

                            File.AppendAllText(ErrorLogFile,
                                string.Format("{0}\t{1}{2}{2}", XTime.GetFullString(DateTime.Now, true), strLog_, XString.NewLine));
                        }
                    }
                }
                catch { }
            }

            static void CheckException()
            {
                //IsError = true;
                if (ExceptionRestart)
                {
                    Application.Restart();
                }
                else
                {
                    Application.Exit();
                }
            }
        } // Program
    } // FromMain
} // namespace
