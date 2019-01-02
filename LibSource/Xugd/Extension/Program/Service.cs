using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.Extension
{
    partial class XProgram
    {
        /// <summary>
        /// 服务
        /// </summary>
        internal class Service:ServiceBase
        {
            private IContainer _conComponents;

            /// <summary>
            /// 服务对应的Form
            /// </summary>
            public ServiceForm FrmMain { get; set; }
            ///// <summary>
            ///// 是否强制保证退出，如果是则等待ExitWaitSeconds后，直接杀掉进程
            ///// </summary>
            //public bool EnsureExit { get; set; }
            ///// <summary>
            ///// 强制退出或重启前等待的时间
            ///// </summary>
            //public int ExitWaitSeconds { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public Service()
            {
                this.InitializeComponent();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="con_"></param>
            public Service(IContainer con_)
            {
                con_.Add(this);
                this.InitializeComponent();
            }

            void InitializeComponent()
            {
                this._conComponents = new Container();
                this.ServiceName = "XProgram.Service";
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
                    ProgramForm.ServiceName = base.ServiceName;
                    ProgramForm.HandleAllException();
                }

                Application.EnableVisualStyles();
                Application.Run(this.FrmMain);
                try
                {
                    if (EnsureExit)
                        XProcess.ExitEnsure(Application.StartupPath, ExitWaitSeconds);

                    base.Stop();
                }
                catch{}
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="bDisposing_"></param>
            protected override void Dispose(bool bDisposing_)
            {
                if (bDisposing_ && this._conComponents != null)
                {
                    this._conComponents.Dispose();
                    this._conComponents = null;
                }

                base.Dispose(bDisposing_);
            }

            void Exit()
            {
                if(this.FrmMain != null)
                {
                    this.FrmMain.ToExit("XProgram.Service.Exit");
                }
            }
        } // Service
    } // XProgram
}
