using System;
using SHCre.Xugd.Extension;

namespace SHCre.Tools.ProcMonitor
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] strArgs_)
        {
//             Application.EnableVisualStyles();
//             Application.SetCompatibleTextRenderingDefault(false);
//             Application.Run(new FrmMain());

            XProgram.RunService<FrmMain>(strArgs_);
        }
    }
}
