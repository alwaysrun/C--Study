using System;
using System.Collections.Generic;
using System.Text;
using SHCre.Xugd.CFile;
using System.Xml.Serialization;
using SHCre.Xugd.Config;
using SHCre.Xugd.Common;

namespace SHCre.Tools.ProcMonitor
{
    public enum ExitHandleMode
    {
        /// <summary>
        /// 不错任何处理
        /// </summary>
        None,
        /// <summary>
        /// 重新启动程序
        /// </summary>
        RestartExe,
        /// <summary>
        /// 重启操作系统
        /// </summary>
        RestartOS,
        /// <summary>
        /// 通过CreateProcessAsUser启动程序（只有服务有权限）
        /// </summary>
        RestartProc,
    }

    /// <summary>
    /// 内存检测配置
    /// </summary>
    public class MemCheckConfig
    {
        /// <summary>
        /// 进程名，用于查找要检测的程序
        /// </summary>
        [XmlAttribute]
        public string ProcName { get; set; }
        /// <summary>
        /// 检测的时间
        /// </summary>
        [XmlAttribute]
        public double CheckAtClock { get; set; }
        /// <summary>
        /// 最大允许值
        /// </summary>
        [XmlAttribute]
        public int MaxSizeInMB { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public MemCheckConfig()
        {
            ProcName = string.Empty;
            CheckAtClock = 2.5;    // default is 2:30
            MaxSizeInMB = 800;
        }
    }

    public class MonitorConfig
    {
        /// <summary>
        /// 进程名，用于查找要检测的程序
        /// </summary>
        [XmlAttribute]
        public string ProcName { get; set; }
        /// <summary>
        /// 如果要监视的程序未启动，多长时间检测一次
        /// </summary>
        [XmlAttribute]
        public int CheckIntervalMinutes { get; set; }
        /// <summary>
        /// 如果程序未启动，等待多长时间（WaitCheckCountBeforeAsExit*CheckIntervalMinutes）
        /// 后即认为程序已退掉，然后做ExitHandle
        /// </summary>
        [XmlAttribute]
        public int WaitCheckCountBeforeAsExit { get; set; }
        /// <summary>
        /// 如果监视的程序退出，如何处理
        /// </summary>
        [XmlAttribute]
        public string ExitHandle { get; set; }

        /// <summary>
        /// 退出处理
        /// </summary>
        [XmlIgnore]
        public ExitHandleMode ExitMode 
        { 
            get
            {
                var euMode = ExitHandleMode.RestartExe;
                try 
                {
                    euMode = XConvert.Name2Enum<ExitHandleMode>(ExitHandle);
                }
                catch{}

                return euMode;
            }
        }

        /// <summary>
        /// 文件名（包括完整路径）
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// 监视程序对应的文件名
        /// </summary>
        [XmlIgnore]
        public string ProcFile { get; set; }

        /// <summary>
        /// 启动程序的名，如果不需要启动则不需要
        /// </summary>
        public string RestartCmd { get; set; }

        /// <summary>
        /// 在程序重启前，等待时间
        /// </summary>
        [XmlAttribute]
        public int WaitSecondBeforeRestart { get; set; }

        public MonitorConfig()
        {
            CheckIntervalMinutes = 1;
            WaitCheckCountBeforeAsExit = 3;
            ExitHandle = XConvert.Enum2Name(ExitHandleMode.RestartExe);
            ProcName = string.Empty;
            File = string.Empty;
            WaitSecondBeforeRestart = 0;
        }
    }

    public class ProcConfig
    {
        /// <summary>
        /// 只有OS启动多长时间后，才会重启系统；
        /// 如果小于指定的时间程序退掉，则重启程序，而非重启系统。
        /// </summary>
        public int RestartOsOnlyAfterMinute { get; set; }
        public string ExitHandleChoice { get;set; }
        public List<MonitorConfig> ToMonitors { get; set; }
        public List<MemCheckConfig> ToCheckMems { get; set; }
        public XLogConfig LogConfig { get; set; }

        public ProcConfig()
        {
            RestartOsOnlyAfterMinute = 10;
            LogConfig = new XLogConfig();
            ToMonitors = new List<MonitorConfig>();
            ToCheckMems = new List<MemCheckConfig>();
        }

        public static ProcConfig Read(string strFile_=null)
        {
            if (string.IsNullOrEmpty(strFile_))
                strFile_ = "CreProcMonitor.xml";

            ProcConfig conProc = XConFile.Read<ProcConfig>(strFile_);
            if(conProc == null)
            {
                conProc = new ProcConfig()
                {
                    ExitHandleChoice = XString.GetEnumNames<ExitHandleMode>(),
                };
                conProc.ToMonitors.Add(new MonitorConfig()
                    {
                        ProcName = String.Empty,
                        File = String.Empty,
                        RestartCmd = String.Empty,
                    });

                conProc.ToCheckMems.Add(new MemCheckConfig());
                conProc.LogConfig.InitDayLog("Proc");

                conProc.Write(strFile_);
            }

            return conProc;
        }

        public void Write(string strFile_=null)
        {
            if (string.IsNullOrEmpty(strFile_))
                strFile_ = "CreProcMonitor.xml";

            XConFile.Write(strFile_, this);
        }
    }
}
