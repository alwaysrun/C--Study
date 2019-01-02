using System;
using System.ComponentModel;
using System.Security.Permissions;
using System.Windows.Forms;
using SHCre.Xugd.Common;
using SHCre.Xugd.WinAPI;

namespace SHCre.Xugd.Extension
{
    partial class XProgram
    {
        /// <summary>
        /// 可以服务运行的Form
        /// </summary>
        public class ServiceForm:Form
        {
            const int MsgEndSession = 0x0016;
            /// <summary>
            /// WndProc中请求结束Session消息ID
            /// </summary>
            const int MsgQueryEndSession = 0x0011;
            /// <summary>
            /// WndProc中退出消息ID
            /// </summary>
            const int MsgQuit = 0x12;

            #region "Var"
            private IContainer _conComponents;
            private NotifyIcon _iconNoitfy;

            /// <summary>
            /// 是否隐藏
            /// </summary>
            public bool IsHide { get; set; }
            /// <summary>
            /// 是否是服务
            /// </summary>
            public bool IsService {get; set;}
            /// <summary>
            /// 能否退出
            /// </summary>
            public bool CanExit { get; private set; }
            /// <summary>
            /// 退出原因（调用ToExit函数是的参数），已知原因：
            /// ‘XProgram.Service.Exit’：以服务方式运行时，停止了服务；
            /// ‘MsgQuit’：接收到Windows退出消息（0x12）时，程序主动停止；
            /// ‘MsgEndSession’：接收到Session关闭（如关机、注销等）消息（0x0016）时，程序主动停止；
            /// ‘Tray.Quit’：通过托盘中的退出菜单，退出程序。
            /// </summary>
            public string ExitReason {get; private set;}
            /// <summary>
            /// 是否隐藏托盘
            /// </summary>
            public bool IsHideTrayIcon {get;set;}
            #endregion

            #region "Init"
            /// <summary>
            /// 
            /// </summary>
            public ServiceForm()
            {
                this.CanExit = false;
                this.InitializeComponent();
                base.Disposed += new EventHandler(ServiceForm_Disposed);
            }

            void InitializeComponent()
            {
                this._conComponents = new Container();
                base.AutoScaleMode = AutoScaleMode.Font;

                this.Text = "XProgram.ServiceForm";
            }

            void ServiceForm_Disposed(object sender, EventArgs e)
            {
                if (this._iconNoitfy != null)
                {
                    this._iconNoitfy.Dispose();
                    this._iconNoitfy = null;
                }
            }

            /// <summary>
            /// 退出
            /// </summary>
            /// <param name="strReason_">退出原因（会设定到ExitReason中），便于查看</param>
            public void ToExit(string strReason_)
            {
                this.CanExit = true;
                if (string.IsNullOrEmpty(strReason_))
                    strReason_ = "#BeCalled";
                this.ExitReason = strReason_;

                this.Close();
            }

            /// <summary>
            /// 初始设定CanExit
            /// </summary>
            /// <param name="bCanExit_"></param>
            public void InitCanExit(bool bCanExit_)
            {
                this.ExitReason = "Init to " + bCanExit_.ToString();
                this.CanExit = bCanExit_;
            }
            #endregion

            #region "Close/Dispose"
            /// <summary>
            /// 
            /// </summary>
            /// <param name="bDisposing_"></param>
            protected override void Dispose(bool bDisposing_)
            {
                if(bDisposing_ && this._conComponents!=null)
                {
                    this._conComponents.Dispose();
                    this._conComponents = null;
                }

                base.Dispose(bDisposing_);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="e"></param>
            protected override void OnLoad(EventArgs e)
            {
                base.OnLoad(e);

                ShowTrayIcon();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="eArg_"></param>
            protected override void OnClosing(CancelEventArgs eArg_)
            {
                //if(this.IsService && !this.CanExit)
                if (!this.CanExit)
                {
                    eArg_.Cancel = true;
                    base.Visible = false;
                    this.ShowTrayIcon();
                    return;
                }

                if (this._iconNoitfy != null)
                {
                    this._iconNoitfy.Visible = false;
                }
                this.OnServiceQuit();
                base.OnClosing(eArg_);
            }

            /// <summary>
            /// 服务退出时调用
            /// </summary>
            protected virtual void OnServiceQuit()
            {
            }
            #endregion

            /// <summary>
            /// 
            /// </summary>
            /// <param name="e"></param>
            protected override void OnShown(EventArgs e)
            {
                if (this.IsHide)
                    base.Hide();
                else
                    base.Show();
                    
                base.OnShown(e);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="msg_"></param>
            [PermissionSet(SecurityAction.Demand, Name="FullTrust")]
            protected override void WndProc(ref Message msg_)
            {
                // In service, this may not enabled
                if (msg_.Msg == MsgQuit)
                {
                    this.ToExit("MsgQuit");
                }
                else if (msg_.Msg == MsgQueryEndSession)
                {
                    // this.ToExit("MsgQueryEndSession");
                    // 如果是关机或注销（为防止注销或未成功关机而引起的退出），尝试重启
                    // XProcess.StartService(Application.StartupPath, ProgramForm.ServiceName, 60);
                    
                    msg_.Result = (IntPtr)1; // allow shutdown
                }
                else if(msg_.Msg == MsgEndSession)
                {
                    this.ToExit("MsgEndSession");
                }

                base.WndProc(ref msg_);
            }

            /// <summary>
            /// 尝试显示托盘图标
            /// </summary>
            protected void ShowTrayIcon()
            {
                if(!this.IsHideTrayIcon)
                {
                    if(this._iconNoitfy==null)
                    {
                        this._iconNoitfy = new NotifyIcon();
                        this._iconNoitfy.Icon = base.Icon;
                        this._iconNoitfy.Text = this.Text;
                        this._iconNoitfy.DoubleClick += new EventHandler(IconNoitfy_DoubleClick);
                        this._iconNoitfy.ContextMenu = new ContextMenu(new MenuItem[]
                        {
                            new MenuItem("&Show", (zo, ze)=>{
                                    XWin.ShowAtTop(this);
                                    //_iconNoitfy.Visible = false;
                                }),
                            new MenuItem("Quit", (zo, ze)=>
                                {
                                    this._iconNoitfy.Visible = false;
                                    this.ToExit("Tray.Quit");
                                })
                        });
                    }

                    this._iconNoitfy.Visible = true;
                }
            }

            void IconNoitfy_DoubleClick(object sender, EventArgs e)
            {
                XWin.ShowAtTop(this);
                //base.Visible = true;
                //this._iconNoitfy.Visible = false;
            }
        } // ServiceForm
    } // XProgram
}
