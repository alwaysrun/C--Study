using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.Extension
{
    /// <summary>
    /// 用于把Form程序转为服务的辅助类
    /// </summary>
    partial class XFrmService : ServiceBase
    {
        /// <summary>
        /// 服务对应的Form
        /// </summary>
        public XServiceForm FrmMain { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public XFrmService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public XFrmService(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        /// <summary>
        /// 服务启动
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            Thread thrForm = new Thread(ThreadFormProc);
            thrForm.SetApartmentState(ApartmentState.STA);
            thrForm.Start();
        }

        /// <summary>
        /// 服务退出
        /// </summary>
        protected override void OnStop()
        {
            this.Exit();
        }

        void ThreadFormProc()
        {
            //if(!Debugger.IsAttached)
            {
                // ExceptionRestart = true;
                XProgram.ProgramForm.ServiceName = base.ServiceName;
                XProgram.ProgramForm.HandleAllException();
            }

            Application.EnableVisualStyles();
            Application.Run(this.FrmMain);
            try
            {
                if (XProgram.EnsureExit)
                    XProcess.ExitEnsure(Application.StartupPath, XProgram.ExitWaitSeconds);

                base.Stop();
            }
            catch { }
        }

        void Exit()
        {
            if (this.FrmMain != null)
            {
                this.FrmMain.CanExit = true;
                this.FrmMain.Close();
            }
        }
    }
}
