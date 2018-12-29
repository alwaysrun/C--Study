using System;
using System.Threading;
using System.Windows.Forms;

namespace SHCre.Tools.DllLoad
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += new ThreadExceptionEventHandler(Application_Exception);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppDomain_Exception);

            Application.Run(new FrmInfo());
        }

        private static void Application_Exception(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString());
        }

        private static void AppDomain_Exception(object sender, UnhandledExceptionEventArgs e)
        {
           // System.Diagnostics.Trace.Write((e.ExceptionObject as Exception).ToString());
           // Application.Exit();
            MessageBox.Show((e.ExceptionObject as Exception).ToString());
        }
    }
}
