using System;
using System.Collections.Generic;
using System.Text;

namespace SHCre.Xugd.Common
{
    /// <summary>
    /// 可执行程序的启动参数处理类
    /// </summary>
    public static class XArgument
    {
        /// <summary>
        /// 默认对齐长度
        /// </summary>
        public static int DefPadWidth = 20;

        /// <summary>
        /// 启动参数分析（如ExeName -help)：
        /// 获取所有以“-”开始的参数，转换为小写字符
        /// </summary>
        /// <param name="strArgs_"></param>
        /// <returns></returns>
        public static List<string> Parse(string[] strArgs_)
        {
            List<string> lstArgs = new List<string>();
            if(strArgs_==null || strArgs_.Length==0)  // only program name
                return lstArgs;

            foreach(var str in strArgs_)
            {
                if (str.StartsWith("-") || str.StartsWith("/"))
                    lstArgs.Add("-" + str.Substring(1).Trim().ToLower());
            }

            return lstArgs;
        }

        /// <summary>
        /// 是否是帮助请求（-h, -help)
        /// </summary>
        /// <param name="lstArgs_"></param>
        /// <returns></returns>
        public static bool IsHelp(List<string> lstArgs_)
        {
            return lstArgs_.Contains("-h") || lstArgs_.Contains("-help");
        }

        /// <summary>
        /// Help parameter
        /// </summary>
        /// <returns></returns>
        public static string ArgHelp()
        {
            return string.Format("{0}\tShow this help", ("-help(-h):").PadRight(DefPadWidth));
        }

        /// <summary>
        /// 是否已安静方式运行
        /// </summary>
        /// <param name="lstArgs_"></param>
        /// <returns></returns>
        public static bool IsSilent(List<string> lstArgs_)
        {
            return lstArgs_.Contains("-silent");
        }

        /// <summary>
        /// silent parameter
        /// </summary>
        /// <returns></returns>
        public static string ArgSilent()
        {
            return string.Format("{0}\tRun in silent", ("-silent:").PadRight(DefPadWidth)); 
        }

        /// <summary>
        /// 是否要隐藏主窗体
        /// </summary>
        /// <param name="lstArgs_"></param>
        /// <returns></returns>
        public static bool IsHide(List<string> lstArgs_)
        {
            return lstArgs_.Contains("-hide");
        }

        /// <summary>
        /// hide parameter
        /// </summary>
        /// <returns></returns>
        public static string ArgHide()
        {
            return string.Format("{0}\tRun in hide", ("-hide:").PadRight(DefPadWidth));
        }

        /// <summary>
        /// 是否显示托盘显示
        /// </summary>
        /// <param name="lstArgs_"></param>
        /// <returns></returns>
        public static bool IsShowTray(List<string> lstArgs_)
        {
            return lstArgs_.Contains("-showtray");
        }

        /// <summary>
        /// Show Tray
        /// </summary>
        /// <returns></returns>
        public static string ArgShowTray()
        {
            return string.Format("{0}\tShow Tray in Taskbar", ("-showtray").PadRight(DefPadWidth));
        }

        /// <summary>
        /// 是否已程序方式运行（没有任何参数，默认也是以程序方式运行）
        /// </summary>
        /// <param name="lstArgs_"></param>
        /// <returns></returns>
        public static bool IsApplication(List<string> lstArgs_)
        {
            return lstArgs_.Contains("-app");
        }

        /// <summary>
        /// app parameter
        /// </summary>
        /// <returns></returns>
        public static string ArgApplication()
        {
            return string.Format("{0}\tRun as service", ("-app:").PadRight(DefPadWidth));
        }

        /// <summary>
        /// 是否以服务方式运行
        /// </summary>
        /// <param name="lstArgs_"></param>
        /// <returns></returns>
        public static bool IsService(List<string> lstArgs_)
        {
            return lstArgs_.Contains("-service");
        }

        /// <summary>
        /// service parameter
        /// </summary>
        /// <returns></returns>
        public static string ArgService()
        {
            return string.Format("{0}\tRun as service", ("-service:").PadRight(DefPadWidth));
        }

        /// <summary>
        /// 是否要安装为服务
        /// </summary>
        /// <param name="lstArgs_"></param>
        /// <returns></returns>
        public static bool IsInstall(List<string> lstArgs_)
        {
            return lstArgs_.Contains("-i") || lstArgs_.Contains("-install");
        }

        /// <summary>
        /// install parameter
        /// </summary>
        /// <returns></returns>
        public static string ArgInstall()
        {
            return string.Format("{0}\tInstall as service", ("-install(-i):").PadRight(DefPadWidth));
        }

        /// <summary>
        /// 是否要卸载服务
        /// </summary>
        /// <param name="lstArgs_"></param>
        /// <returns></returns>
        public static bool IsUninstall(List<string> lstArgs_)
        {
            return lstArgs_.Contains("-u") || lstArgs_.Contains("-uninstall");
        }

        /// <summary>
        /// uninstall parameter
        /// </summary>
        /// <returns></returns>
        public static string ArgUninstall()
        {
            return string.Format("{0}\tUninstall this service", ("-uninstall(-u):").PadRight(DefPadWidth));
        }

        /// <summary>
        /// Start service
        /// </summary>
        /// <param name="lstArgs_"></param>
        /// <returns></returns>
        public static bool IsStart(List<string> lstArgs_)
        {
            return lstArgs_.Contains("-start");
        }

        /// <summary>
        /// start parameter
        /// </summary>
        /// <returns></returns>
        public static string ArgStart()
        {
            return string.Format("{0}\tStart this service", ("-start:").PadRight(DefPadWidth));
        }

        /// <summary>
        /// Stop service
        /// </summary>
        /// <param name="lstArgs_"></param>
        /// <returns></returns>
        public static bool IsStop(List<string> lstArgs_)
        {
            return lstArgs_.Contains("-stop");
        }

        /// <summary>
        /// start parameter
        /// </summary>
        /// <returns></returns>
        public static string ArgStop()
        {
            return string.Format("{0}\tStop this service", ("-stop:").PadRight(DefPadWidth));
        }
    }
}
