using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.WinAPI
{
    partial class XWin
    {
        /// <summary>
        /// Service，出错时抛出Win32Exception
        /// </summary>
        public static class Service
        {
            #region "SCM"
            /// <summary>
            /// Access Rights for the Service Control Manager
            /// </summary>
            [Flags]
            public enum SCMAccess : uint
            {
                /// <summary>
                /// Includes STANDARD_RIGHTS_REQUIRED, in addition to all access rights in this table.
                /// </summary>
                All = 0xF003F,
                /// <summary>
                /// Required to connect to the service control manager
                /// </summary>
                Connect = 1,
                /// <summary>
                /// Required to call the CreateService function to create a service object and add it to the database
                /// </summary>
                CreateService = 2,
                /// <summary>
                /// Required to call the EnumServicesStatus or EnumServicesStatusEx function to list the services that are in the database.
                /// Required to call the NotifyServiceStatusChange function to receive notification when any service is created or deleted
                /// </summary>
                EnumService = 4,
                /// <summary>
                /// Required to call the LockServiceDatabase function to acquire a lock on the database
                /// </summary>
                Lock = 8,
                /// <summary>
                /// Required to call the NotifyBootConfigStatus function
                /// </summary>
                ModifyBootConfig = 0x20,
                /// <summary>
                /// Required to call the QueryServiceLockStatus function to retrieve the lock status information for the database
                /// </summary>
                QueryLockStatus = 0x10,
                /// <summary>
                /// 标准权限，读写与删除
                /// </summary>
                StandardRight = 0x20000,
                /// <summary>
                /// 读取权限
                /// </summary>
                Read = StandardRight|EnumService|QueryLockStatus,
                /// <summary>
                /// 写权限
                /// </summary>
                Write = StandardRight|CreateService|ModifyBootConfig,
                /// <summary>
                /// 执行权限
                /// </summary>
                Execute = StandardRight|Connect|Lock,
            }

            /// <summary>
            /// Establishes a connection to the service control manager on the specified computer and opens the specified service control manager database
            /// </summary>
            /// <param name="machineName">The name of the target computer(if null/Empty use local machine)</param>
            /// <param name="databaseName">The name of the service control manager database</param>
            /// <param name="dwAccess"></param>
            /// <returns></returns>
            [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
            public static extern IntPtr OpenSCManager(string machineName, string databaseName, SCMAccess dwAccess);
            
            /// <summary>
            /// 打开本机默认的SCManager：出错抛出Win32Exception
            /// </summary>
            /// <param name="euAccess_"></param>
            /// <returns></returns>
            public static IntPtr OpenSCManager(SCMAccess euAccess_= SCMAccess.All)
            {
                IntPtr hSc = OpenSCManager(null, null, euAccess_);
                if(hSc == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "OpenSCManager");

                return hSc;
            }
            #endregion

            #region "Service" 
            /// <summary>
            /// Service Security and Access Rights
            /// </summary>
            [Flags]
            public enum ServiceAccess : uint
            {
                /// <summary>
                /// Includes STANDARD_RIGHTS_REQUIRED in addition to all access rights in this table
                /// </summary>
                All = 0xF01FF,
                /// <summary>
                /// Required to call the ChangeServiceConfig or ChangeServiceConfig2 function to change the service configuration
                /// </summary>
                ChangeConfig = 0x02,
                /// <summary>
                /// Required to call the EnumDependentServices function to enumerate all the services dependent on the service
                /// </summary>
                EnumDependents = 0x08,
                /// <summary>
                /// Required to call the ControlService function to ask the service to report its status immediately
                /// </summary>
                Interrogate =  0x080,
                /// <summary>
                /// Required to call the ControlService function to pause or continue the service
                /// </summary>
                PauseContinue = 0x40,
                /// <summary>
                /// Required to call the QueryServiceConfig and QueryServiceConfig2 functions to query the service configuration
                /// </summary>
                QueryConfig = 0x01,
                /// <summary>
                /// Required to call the QueryServiceStatusEx function to ask the service control manager about the status of the service
                /// </summary>
                QueryStatus = 0x04,
                /// <summary>
                /// Required to call the StartService function to start the service
                /// </summary>
                Start = 0x010,
                /// <summary>
                /// Required to call the ControlService function to stop the service
                /// </summary>
                Stop = 0x020,
                /// <summary>
                /// Required to call the ControlService function to specify a user-defined control code
                /// </summary>
                UserDefinedControl = 0x0100,
                /// <summary>
                /// 标准权限，读写与删除
                /// </summary>
                StandardRight = 0x20000,
                /// <summary>
                /// 普通读
                /// </summary>
                Read = StandardRight|QueryConfig|QueryStatus|Interrogate|EnumDependents,
                /// <summary>
                /// 普通写
                /// </summary>
                Write = StandardRight|ChangeConfig,
                /// <summary>
                /// 执行
                /// </summary>
                Execute = StandardRight|Start|Stop|PauseContinue|UserDefinedControl,
            }

            /// <summary>
            /// 服务类型
            /// </summary>
            [Flags]
            public enum ServiceType : uint
            {
                /// <summary>
                /// File system driver service
                /// </summary>
                FileSystemDriver = 0x02,
                /// <summary>
                /// Driver service
                /// </summary>
                KernelDriver = 0x01,
                /// <summary>
                /// 运行于独立进程的服务程序
                /// </summary>
                Win32OwnProcess = 0x10,
                /// <summary>
                /// 被多个进程共享的服务程序
                /// </summary>
                Win32ShareProcess = 0x20,
                /// <summary>
                /// 该服务可以与桌面程序进行交互操作，如果用户注销可能会被停掉
                /// </summary>
                InteractiveProcess = 0x100,
                /// <summary>
                /// 由Form生成可与桌面交互的服务，如果用户注销可能会被停掉
                /// </summary>
                FormService = Win32OwnProcess|InteractiveProcess,
            }

            /// <summary>
            /// 服务启动选项
            /// </summary>
            public enum StartMode : uint
            {
                /// <summary>
                /// 系统启动时由服务控制管理器自动启动该服务程序
                /// </summary>
                AutoStart = 0x02,
                /// <summary>
                /// 用于由系统加载器创建的设备驱动程序; 
                /// 只能用于驱动服务程序
                /// </summary>
                BootStart = 0,
                /// <summary>
                /// 由服务控制管理器(SCM)启动的服务
                /// </summary>
                DemandStart = 0x03,
                /// <summary>
                /// 表示该服务不可启动
                /// </summary>
                Disabled = 0x04,
                /// <summary>
                /// 由IoInitSystem启动，只有驱动可使用此选项
                /// </summary>
                SystemStart = 0x01,
            }

            /// <summary>
            /// 当该启动服务失败时产生错误的严重程度以及采取的保护措施
            /// </summary>
            public enum ErrorControl
            {
                /// <summary>
                /// 服务启动程序将把该错误记录到事件日志中
                /// </summary>
                Critical = 0x03,
                /// <summary>
                /// 服务启动程序将忽略该错误并返回继续执行
                /// </summary>
                Ignore = 0,
                /// <summary>
                /// 服务启动程序将把该错误记录到事件日志中并返回继续执行
                /// </summary>
                Normal = 0x01,
                /// <summary>
                /// 服务启动程序将把该错误记录到事件日志中; 
                /// 否则将返回继续执行
                /// </summary>
                Severe = 0x02,
            }

            /// <summary>
            /// Creates a service object and adds it to the specified service control manager database
            /// </summary>
            /// <param name="hSCManager"></param>
            /// <param name="lpServiceName"></param>
            /// <param name="lpDisplayName"></param>
            /// <param name="dwDesiredAccess"></param>
            /// <param name="dwServiceType"></param>
            /// <param name="dwStartType"></param>
            /// <param name="dwErrorControl"></param>
            /// <param name="lpBinaryPathName"></param>
            /// <param name="lpLoadOrderGroup"></param>
            /// <param name="lpdwTagId"></param>
            /// <param name="lpDependencies"></param>
            /// <param name="lpServiceStartName"></param>
            /// <param name="lpPassword"></param>
            /// <returns></returns>
            [DllImport("advapi32.dll", EntryPoint = "CreateServiceW", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr Create(IntPtr hSCManager, string lpServiceName, string lpDisplayName, ServiceAccess dwDesiredAccess, ServiceType dwServiceType, StartMode dwStartType,
                ErrorControl dwErrorControl, string lpBinaryPathName, string lpLoadOrderGroup, int lpdwTagId, string lpDependencies, string lpServiceStartName, string lpPassword);

            /// <summary>
            /// 创建服务，失败抛出Win32Exception
            /// </summary>
            /// <param name="hManager_">SCManager的句柄</param>
            /// <param name="strSrvName_">服务的名称</param>
            /// <param name="strExeFilePath_">服务器文件（完整路径）</param>
            /// <param name="aryDependencies_">依赖服务列表</param>
            /// <param name="euAccess_"></param>
            /// <param name="euType_"></param>
            /// <param name="euStart_"></param>
            /// <param name="euError_"></param>
            /// <returns></returns>
            public static IntPtr Create(IntPtr hManager_, string strSrvName_, string strExeFilePath_, string[] aryDependencies_=null, ServiceAccess euAccess_ = ServiceAccess.All, ServiceType euType_ = ServiceType.FormService, StartMode euStart_ = StartMode.AutoStart, ErrorControl euError_ = ErrorControl.Normal)
            {
                string strDepend = null;
                if (aryDependencies_ != null && aryDependencies_.Length > 0)
                {
                    strDepend = string.Join(XString.NullEnd, aryDependencies_);
                    strDepend += XString.NullEnd;
                }

                IntPtr hSrv = Create(hManager_, strSrvName_, strSrvName_, euAccess_, euType_, euStart_, euError_, strExeFilePath_, null, 0, strDepend, null, null);
                if (hSrv == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "CreateService");

                return hSrv;
            }

            /// <summary>
            /// Opens an existing service
            /// </summary>
            /// <param name="hSCManager"></param>
            /// <param name="lpServiceName"></param>
            /// <param name="dwDesiredAccess"></param>
            /// <returns></returns>
            [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            private static extern IntPtr OpenServiceW(IntPtr hSCManager, string lpServiceName, ServiceAccess dwDesiredAccess);

            /// <summary>
            /// 打开服务：失败抛出Win32Exception
            /// </summary>
            /// <param name="hManager_"></param>
            /// <param name="strSrvName_"></param>
            /// <param name="euAccess_"></param>
            /// <returns></returns>
            public static IntPtr Open(IntPtr hManager_, string strSrvName_, ServiceAccess euAccess_ = ServiceAccess.All)
            {
                IntPtr hSrv = OpenServiceW(hManager_, strSrvName_, euAccess_);
                if (hSrv == IntPtr.Zero)
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "OpenService");

                return hSrv;
            }

            /// <summary>
            /// Closes a handle to a service control manager or service object
            /// </summary>
            /// <param name="hService"></param>
            /// <returns></returns>
            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport("advapi32.dll", EntryPoint = "CloseServiceHandle", SetLastError = true)]
            public static extern bool Close(IntPtr hService);

            /// <summary>
            /// 删除服务
            /// </summary>
            /// <param name="hService"></param>
            /// <returns></returns>
            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport("advapi32.dll", EntryPoint = "DeleteService", SetLastError = true)]
            public static extern bool Delete(IntPtr hService);
            #endregion

            /// <summary>
            /// 安装服务，如果出错会抛出Win32Exception
            /// </summary>
            /// <param name="strSrvName_"></param>
            /// <param name="strExeFilePath_">程序名（包括路径），服务器运行时所需的参数也要包括在内</param>
            /// <param name="aryDependencies_">依赖服务列表</param>
            /// <param name="euType_"></param>
            /// <param name="euStart_"></param>
            /// <param name="euError_"></param>
            public static void Install(string strSrvName_, string strExeFilePath_, string[] aryDependencies_ = null, ServiceType euType_ = ServiceType.FormService, StartMode euStart_ = StartMode.AutoStart, ErrorControl euError_ = ErrorControl.Normal)
            {
                IntPtr hManger = IntPtr.Zero, hService = IntPtr.Zero;
                try 
                {
                    hManger = OpenSCManager();
                    hService = Create(hManger, strSrvName_, strExeFilePath_, aryDependencies_, ServiceAccess.All, euType_, euStart_, euError_);
                    // ChangeServiceConfig2 to set description
                }
                finally
                {
                    if (hService != IntPtr.Zero)
                        Close(hService);
                    if (hManger != IntPtr.Zero)
                        Close(hManger);
                }
            }

            /// <summary>
            /// 卸载服务：如果出错会抛出Win32Exception
            /// </summary>
            /// <param name="strSrvName_"></param>
            public static void Uninstall(string strSrvName_)
            {
                IntPtr hManger = IntPtr.Zero, hService = IntPtr.Zero;
                try
                {
                    hManger = OpenSCManager();
                    hService = Open(hManger, strSrvName_);
                    try
                    {
                        Control(hService, ServiceControl.Stop);
                    }
                    catch{}

                    if(!Delete(hService))
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "DeleteService");
                }
                finally
                {
                    if (hService != IntPtr.Zero)
                        Close(hService);
                    if (hManger != IntPtr.Zero)
                        Close(hManger);
                }
            }

            /// <summary>
            /// 判断指定的服务是否存在
            /// </summary>
            /// <param name="strSrvName_"></param>
            /// <returns></returns>
            public static bool HasExists(string strSrvName_)
            {
                bool bExists = false;
                IntPtr hManger = IntPtr.Zero, hService = IntPtr.Zero;
                try
                {
                    hManger = OpenSCManager();
                    hService = Open(hManger, strSrvName_, ServiceAccess.Read);
                    bExists = true;
                }
                catch (Win32Exception) { }
                finally
                {
                    if (hService != IntPtr.Zero)
                        Close(hService);
                    if (hManger != IntPtr.Zero)
                        Close(hManger);
                }

                return bExists;
            }
            
            #region "Control Service"
            /// <summary>
            /// Control code
            /// </summary>
            [Flags]
            public enum ServiceControl : uint
            {
                /// <summary>
                /// Notifies a paused service that it should resume
                /// </summary>
                Continue = 0x03,
                /// <summary>
                /// Notifies a service that it should report its current status information to the service control manager
                /// </summary>
                Interrogate = 0x04,
                /// <summary>
                /// Notifies a network service that there is a new component for binding
                /// </summary>
                NetbindAdd = 0x07,
                /// <summary>
                /// Notifies a network service that one of its bindings has been disabled
                /// </summary>
                NetbindDisable = 0x0A,
                /// <summary>
                /// Notifies a network service that a disabled binding has been enabled
                /// </summary>
                NetbindEnable = 0x09,
                /// <summary>
                /// Notifies a network service that a component for binding has been removed
                /// </summary>
                NetbindRemove = 0x08,
                /// <summary>
                /// Notifies a service that its startup parameters have changed
                /// </summary>
                ParamChange = 0x06,
                /// <summary>
                /// Notifies a service that it should pause
                /// </summary>
                Pause = 0x02,
                /// <summary>
                /// Notifies a service that it should stop
                /// </summary>
                Stop = 0x01,
            }

            /// <summary>
            /// current state of the service
            /// </summary>
            public enum ServiceState : uint
            {
                /// <summary>
                /// The service continue is pending
                /// </summary>
                ContinuePending = 0x05,
                /// <summary>
                /// The service pause is pending
                /// </summary>
                PausePending = 0x06,
                /// <summary>
                /// The service is paused
                /// </summary>
                Paused = 0x07,
                /// <summary>
                /// The service is running
                /// </summary>
                Running = 0x04,
                /// <summary>
                /// The service is starting
                /// </summary>
                StartPending = 0x02,
                /// <summary>
                /// The service is stopping
                /// </summary>
                StopPending = 0x03,
                /// <summary>
                /// The service is not running
                /// </summary>
                Stopped = 0x01,
            }

            /// <summary>
            /// status information for a service
            /// </summary>
            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct ServiceStatus
            {
                /// <summary>
                /// Size
                /// </summary>
                public static readonly int SizeOf = Marshal.SizeOf(typeof(ServiceStatus));
                /// <summary>
                /// The type of service
                /// </summary>
                public ServiceType dwServiceType;
                /// <summary>
                /// The current state of the service
                /// </summary>
                public ServiceState dwCurrentState;
                /// <summary>
                /// The control codes the service accepts and processes in its handler function 
                /// </summary>
                public uint dwControlsAccepted;
                /// <summary>
                /// The error code the service uses to report an error that occurs when it is starting or stopping
                /// </summary>
                public uint dwWin32ExitCode;
                /// <summary>
                /// A service-specific error code that the service returns when an error occurs while the service is starting or stopping
                /// </summary>
                public uint dwServiceSpecificExitCode;
                /// <summary>
                /// The check-point value the service increments periodically to report its progress during a lengthy start, stop, pause, or continue operation
                /// </summary>
                public uint dwCheckPoint;
                /// <summary>
                /// The estimated time required for a pending start, stop, pause, or continue operation, in milliseconds
                /// </summary>
                public uint dwWaitHint;
            }

            /// <summary>
            /// Sends a control code to a service
            /// </summary>
            /// <param name="hService"></param>
            /// <param name="dwControl"></param>
            /// <param name="lpServiceStatus"></param>
            /// <returns></returns>
            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport("advapi32.dll", EntryPoint = "ControlService", SetLastError = true)]
            public static extern bool Control(IntPtr hService, ServiceControl dwControl, ref ServiceStatus lpServiceStatus);

            /// <summary>
            /// Sends a control code to a service
            /// </summary>
            /// <param name="hService"></param>
            /// <param name="dwControl"></param>
            /// <returns></returns>
            public static ServiceStatus Control(IntPtr hService, ServiceControl dwControl)
            {
                ServiceStatus srvStatus = new ServiceStatus();
                if (!Control(hService, dwControl, ref srvStatus))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "StopService");

                return srvStatus;
            }

            /// <summary>
            /// 获取服务的状态
            /// </summary>
            /// <param name="hService"></param>
            /// <param name="dwServiceStatus"></param>
            /// <returns></returns>
            [DllImport("advapi32.dll", EntryPoint = "QueryServiceStatus", SetLastError = true)]
            public static extern bool QueryStatus(IntPtr hService, ref ServiceStatus dwServiceStatus);

            /// <summary>
            /// 获取服务的当前状态
            /// </summary>
            /// <param name="strSrvName_"></param>
            /// <returns></returns>
            public static ServiceState QueryState(string strSrvName_)
            {
                IntPtr hManger = IntPtr.Zero, hService = IntPtr.Zero;
                try
                {
                    hManger = OpenSCManager();
                    hService = Open(hManger, strSrvName_);

                    ServiceStatus srvStatus = new ServiceStatus();
                    if (!QueryStatus(hService, ref srvStatus))
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "QueryServiceStatus");

                    return srvStatus.dwCurrentState;
                }
                finally
                {
                    if (hService != IntPtr.Zero)
                        Close(hService);
                    if (hManger != IntPtr.Zero)
                        Close(hManger);
                }
            }

            /// <summary>
            /// Starts a service
            /// </summary>
            /// <param name="hService"></param>
            /// <param name="nArgCount_"></param>
            /// <param name="strArgs_"></param>
            /// <returns></returns>
            [DllImport("advapi32", CharSet=CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool StartServiceW(IntPtr hService, int nArgCount_, string[] strArgs_);

            /// <summary>
            /// 启动服务
            /// </summary>
            /// <param name="hService_"></param>
            /// <param name="strArgs_">服务启动时所需的参数，如果不为null，则第一个参数必须服务名称</param>
            /// <returns></returns>
            public static bool Start(IntPtr hService_, string[] strArgs_)
            {
                int nCount = 0;
                if(strArgs_ != null)
                    nCount = strArgs_.Length;

                return StartServiceW(hService_, nCount, strArgs_);
            }

            /// <summary>
            /// 启动服务：出错抛出Win32Exception
            /// </summary>
            /// <param name="strSrvName_">服务名</param>
            /// <param name="strArgs_">服务启动时所需的参数，如果不为null，则第一个参数必须服务名称</param>
            public static void Start(string strSrvName_, string[] strArgs_=null)
            {
                IntPtr hManger = IntPtr.Zero, hService = IntPtr.Zero;
                try
                {
                    hManger = OpenSCManager();
                    hService = Open(hManger, strSrvName_);
                    if (!Start(hService, strArgs_))
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "StartService");
                }
                finally
                {
                    if (hService != IntPtr.Zero)
                        Close(hService);
                    if (hManger != IntPtr.Zero)
                        Close(hManger);
                }
            }

            /// <summary>
            /// 停止服务：如果出错抛出Win32Exception
            /// </summary>
            /// <param name="strSrvName_"></param>
            /// <param name="bWait_">是否等待服务退出：如果是，则等待（最长30秒）；否则直接完成</param>
            public static void Stop(string strSrvName_, bool bWait_=false)
            {
                IntPtr hManger = IntPtr.Zero, hService = IntPtr.Zero;
                try
                {
                    hManger = OpenSCManager();
                    hService = Open(hManger, strSrvName_);
                    ServiceStatus srvStatus = new ServiceStatus();
                    if(!QueryStatus(hService, ref srvStatus))
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "QueryServiceStatus");
                    if (srvStatus.dwCurrentState == ServiceState.Stopped)
                        return;

                    // Stop now
                    if(!Control(hService, ServiceControl.Stop, ref srvStatus))
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "StopService");

                    if (bWait_)
                    {
                        int nCount = 30;
                        while (srvStatus.dwCurrentState != ServiceState.Stopped)
                        {
                            if (--nCount <= 0)
                                throw new Win32Exception((int)SHErrorCode.WaitTimeOut, "StopService");

                            Thread.Sleep(100);
                            QueryStatus(hService, ref srvStatus);
                        }
                    }
                }
                finally
                {
                    if (hService != IntPtr.Zero)
                        Close(hService);
                    if (hManger != IntPtr.Zero)
                        Close(hManger);
                }
            }
            #endregion

            #region "Service Config"
            //public enum ServiceConfig2InfoLevel
            //{
            //    SERVICE_CONFIG_DESCRIPTION = 1,
            //    SERVICE_CONFIG_FAILURE_ACTIONS = 2
            //}

            //[StructLayout(LayoutKind.Sequential)]
            //public class SERVICE_DESCRIPTION
            //{
            //    [MarshalAs(UnmanagedType.LPWStr)]
            //    public string lpDescription;
            //    public SERVICE_DESCRIPTION()
            //    {

            //    }
            //}


            //[StructLayout(LayoutKind.Sequential)]
            //public struct SERVICE_FAILURE_ACTIONS
            //{
            //    public int dwResetPeriod;
            //    [MarshalAs(UnmanagedType.LPTStr)]
            //    public string lpRebootMsg;
            //    [MarshalAs(UnmanagedType.LPTStr)]
            //    public string lpCommand;
            //    public int cActions;
            //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
            //    public SC_ACTION[] actions;
            //}

            //[StructLayout(LayoutKind.Sequential)]
            //public class SC_ACTION
            //{
            //    public SC_ACTION_TYPE type;
            //    public uint dwDelay;
            //    public SC_ACTION()
            //    {

            //    }
            //}

            //public enum SC_ACTION_TYPE
            //{
            //    None,
            //    RestartService,
            //    RebootComputer,
            //    Run_Command
            //}

            //[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            //private static extern bool ChangeServiceConfig2(IntPtr hService, ServiceConfig2InfoLevel dwInfoLevel, ref SERVICE_FAILURE_ACTIONS failureAction);
            //[return: MarshalAs(UnmanagedType.Bool)]
            //[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            //public static extern bool ChangeServiceConfig2(IntPtr hService, ServiceConfig2InfoLevel dwInfoLevel, IntPtr lpInfo);
            //[return: MarshalAs(UnmanagedType.Bool)]
            //[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            //private static extern bool ChangeServiceConfig2(IntPtr hService, ServiceConfig2InfoLevel dwInfoLevel, ref SERVICE_DESCRIPTION serviceDescription);

            #endregion

        } // Service
    } // XWin
}
