using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using SHCre.Xugd.Common;
using SHCre.Xugd.WinAPI;

namespace SHCre.Xugd.Extension
{
    /// <summary>
    /// 程序相关操作
    /// </summary>
    public partial class XProgram
    {
        /// <summary>
        /// UnhandledException事件
        /// </summary>
        public static event UnhandledExceptionEventHandler UnhandledException;
        /// <summary>
        /// ThreadException事件
        /// </summary>
        public static event ThreadExceptionEventHandler ThreadException;

        /// <summary>
        /// 强制退出或重启前等待的时间，默认30秒
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
        /// 服务参数的配置文件：如果不设定，默认使用"XServiceParam.xml"作为配置文件
        /// </summary>
        public static string SrvParamFile {get; set; }
        /// <summary>
        /// 遇到异常时，是否重启
        /// </summary>
        public static bool ExceptionRestart { get; set; }

        /// <summary>
        /// 
        /// </summary>
        static XProgram()
        {
            ExitWaitSeconds = 30;
            ErrorLogFile = "Error.log";
        }

        /// <summary>
        /// 如果进程已存在，如何处理
        /// </summary>
        public enum HandleExistsMode
        {
            /// <summary>
            /// 不做任何处理，如果已有进程则当前进程不启动
            /// </summary>
            Remain = 0,
            /// <summary>
            /// 如果已有进程，则恢复，当前进程不启动
            /// </summary>
            Restore,
            /// <summary>
            /// 如果已有进程，则关闭，并启动当前进程
            /// </summary>
            Close,
            /// <summary>
            /// 继续启动(允许启动多个实例)
            /// </summary>
            StillStart,
        }

        /// <summary>
        /// 运行Form程序；
        /// 直接把Program文件中的main函数内容全部注释掉，调用此函数即可。
        /// 如果一运行就出错，则说明Form的构造函数中有异常，对构造函数进行单步调试查找即可。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strSynName_">互斥体名（如果为空或null，则通过文件路径生成唯一的互斥体名称）</param>
        /// <param name="euMode_"></param>
        public static void RunProgram<T>(string strSynName_ = null, HandleExistsMode euMode_ = HandleExistsMode.Remain) where T : Form, new()
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

        static void ToRunProgram<T>(string strSynName_, List<string> lstParam) where T : ServiceForm, new()
        {
            if (string.IsNullOrEmpty(strSynName_))
            {
                strSynName_ = XProcess.GetSynNameFromFileName(Application.ExecutablePath);
            }

            if (ProgramForm.CanStart(strSynName_, HandleExistsMode.Remain))
            {
                Application.EnableVisualStyles();
                ProgramForm.HandleAllException();
                T frm = new T();
                frm.IsHide = XArgument.IsHide(lstParam);
                frm.IsHideTrayIcon = !XArgument.IsShowTray(lstParam);
                // If hidetray, must can exit;
                frm.InitCanExit(frm.IsHideTrayIcon);

                ProgramForm.FrmMain = frm;
                Application.Run(frm);
                if (EnsureExit)
                    XProcess.ExitEnsure(Application.StartupPath, ExitWaitSeconds);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lstDepends_"></param>
        /// <returns></returns>
        static string[] GetDepends(List<XServiceParams.Depend> lstDepends_)
        {
            List<string> lstName = new List<string>(lstDepends_.Count);
            foreach(var de in lstDepends_)
            {
                if (!string.IsNullOrEmpty(de.Name))
                    lstName.Add(de.Name);
            }

            return lstName.ToArray();
        }

        static XWin.Service.ServiceType GetServiceType(string strName_)
        {
            XWin.Service.ServiceType euType = XWin.Service.ServiceType.Win32OwnProcess;
            if (!string.IsNullOrEmpty(strName_))
            {
                try
                {
                    euType = XConvert.Name2Enum<XWin.Service.ServiceType>(strName_);
                }
                catch{}
            }

            return euType;
        }
        /// <summary>
        /// 可以服务方式运行
        /// (服务名（空则使用不带扩展名的程序名）、单实例运行的互斥体名（空则使用文件全路径）、
        /// 依赖项以及运行方式由配置文件SrvParamFile（默认XServiceParam.xml）决定)：
        ///   如果一运行就出错，则说明Form的构造函数中有异常，对构造函数进行单步调试查找即可。
        /// 直接把Program文件中的main函数内容全部注释掉，
        /// 并把main改为带参数形式static void Main(string[] strArgs_)，然后调用此函数即可。
        /// 并把主窗体的基类从Form改为XProgram.ServiceForm
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strArgs_">传递给程序的参数：
        /// -install(-i)：安装服务；
        /// -uninstall(-u)：卸载服务；
        /// -hide：隐藏方式运行；
        /// -silent：以安静方式运行（不弹出窗体）；
        /// -service：以服务方式运行(安装时会自动添加此参数)
        /// </param>
        public static void RunService<T>(string[] strArgs_)
            where T:ServiceForm, new()
        {
            XServiceParams siParam = null;
            try
            {
                siParam = XServiceParams.Read(SrvParamFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Read Config File failed:\n" + ex.Message);
                return;
            }
            string strSynName = siParam.Service.SynName;

            var lstParam = XArgument.Parse(strArgs_);
            if (lstParam.Count == 0)  // Application
            {
                RunProgram<T>(strSynName);
            }
            else
            {
                string strSrvName = siParam.Service.SrvName;
                if (string.IsNullOrEmpty(strSrvName))
                    strSrvName = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
                if (XArgument.IsService(lstParam))
                {
                    ToRunService<T>(strSynName, lstParam, strSrvName);
                }
                else if (XArgument.IsApplication(lstParam))
                {
                    ToRunProgram<T>(strSynName, lstParam);
                }
                else if (XArgument.IsInstall(lstParam))
                {
                    InstallService(siParam, lstParam, strSrvName);
                }
                else if (XArgument.IsUninstall(lstParam))
                {
                    UninstallService(lstParam, strSrvName);
                }
                else if(XArgument.IsHelp(lstParam))
                {
                    ShowHelp();
                }
                else if (XArgument.IsStart(lstParam))
                {
                    StartService(lstParam, strSrvName);
                }
                else if(XArgument.IsStop(lstParam))
                {
                    StopService(lstParam, strSrvName);
                }
            }
        }

        private static void StopService(List<string> lstParam, string strSrvName)
        {
            string strInfo;
            try
            {
                XWin.Service.Stop(strSrvName);
                strInfo = "Stop success";
            }
            catch (Win32Exception ex)
            {
                strInfo = string.Format("!!Stop Failed: {0}({1})", ex.Message, ex.ErrorCode.ToString("X"));
            }

            if (!XArgument.IsSilent(lstParam))
                MessageBox.Show(strInfo);
        }

        private static void StartService(List<string> lstParam, string strSrvName)
        {
            string strInfo;
            try
            {
                XWin.Service.Start(strSrvName);
                strInfo = "Start success";
            }
            catch (Win32Exception ex)
            {
                strInfo = string.Format("!!Start Failed: {0}({1})", ex.Message, ex.ErrorCode.ToString("X"));
            }

            if (!XArgument.IsSilent(lstParam))
                MessageBox.Show(strInfo);
        }

        private static void ShowHelp()
        {
            List<string> lstShow = new List<string>();
            //lstShow.Add(XArgument.ArgHelp());
            lstShow.Add(XArgument.ArgInstall());
            lstShow.Add(XArgument.ArgUninstall());
            lstShow.Add(XArgument.ArgService());
            lstShow.Add(XArgument.ArgApplication());
            lstShow.Add(XArgument.ArgStart());
            lstShow.Add(XArgument.ArgStop());
            //lstShow.Add(XArgument.ArgSilent());
            lstShow.Add(XArgument.ArgHide());
            lstShow.Add(XArgument.ArgShowTray());
            MessageBox.Show(string.Join(XString.NewLine, lstShow));
        }

        private static void UninstallService(List<string> lstParam, string strSrvName)
        {
            string strInfo;
            try
            {
                XWin.Service.Uninstall(strSrvName);
                strInfo = "Uninstall success";
            }
            catch (Win32Exception ex)
            {
                strInfo = string.Format("!!Uninstall Failed: {0}({1})", ex.Message, ex.ErrorCode.ToString("X"));
            }

            if (!XArgument.IsSilent(lstParam))
                MessageBox.Show(strInfo);
        }

        private static void InstallService(XServiceParams siParam, List<string> lstParam, string strSrvName)
        {
            string[] argDepend = GetDepends(siParam.Depends);
            string strParms = string.Join(" ", lstParam.WhereNoDelay(z => z != "-i" && z != "-install" && z != "-silent"));
            string strSrvExe = Application.ExecutablePath + " -service " + strParms;

            string strInfo;
            try
            {
                XWin.Service.Install(strSrvName, strSrvExe, argDepend, GetServiceType(siParam.Service.RunType));
                strInfo = "Install Success";
            }
            catch (Win32Exception ex)
            {
                strInfo = string.Format("!!Install Failed: {0}({1})", ex.Message, ex.ErrorCode.ToString("X"));
            }
            if (!XArgument.IsSilent(lstParam))
                MessageBox.Show(strInfo);
        }

        private static void ToRunService<T>(string strSynName, List<string> lstParam, string strSrvName) where T : ServiceForm, new()
        {
            if (string.IsNullOrEmpty(strSynName))
            {
                strSynName = XProcess.GetSynNameFromFileName(Application.ExecutablePath);
            }

            if (ProgramForm.CanStart(strSynName, HandleExistsMode.Remain))
            {
                using (Service srv = new Service())
                {
                    T frmMain = Activator.CreateInstance<T>();

                    frmMain.IsService = true;
                    frmMain.IsHide = XArgument.IsHide(lstParam);
                    frmMain.IsHideTrayIcon = !XArgument.IsShowTray(lstParam);

                    srv.FrmMain = frmMain;
                    srv.ServiceName = strSrvName;
                    srv.CanStop = true;
                    srv.CanShutdown = true;
                    srv.CanPauseAndContinue = false;
                    ServiceBase.Run(srv);
                }
            }
        }
    } // XProgram
}
