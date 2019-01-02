using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SHCre.Xugd.WinAPI;
using System.Security.Permissions;

namespace SHCre.Xugd.Extension
{
    /// <summary>
    /// 服务的基类
    /// </summary>
    partial class XServiceForm : Form
    {
        /// <summary>
        /// WndProc中请求结束Session消息ID
        /// </summary>
        const int MsgQueryEndSession = 0x11;
        /// <summary>
        /// WndProc中退出消息ID
        /// </summary>
        const int MsgQuit = 0x12;

        private NotifyIcon _iconNoitfy;

        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool IsHide { get; set; }
        /// <summary>
        /// 是否是服务
        /// </summary>
        public bool IsService { get; set; }
        /// <summary>
        /// 能否退出
        /// </summary>
        public bool CanExit { get; set; }
        /// <summary>
        /// 是否隐藏托盘
        /// </summary>
        public bool IsHideTrayIcon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public XServiceForm()
        {
            InitializeComponent();

            base.Disposed += new EventHandler(ServiceForm_Disposed);
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
        public void ToExit()
        {
            this.CanExit = true;
            this.Close();
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
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message msg_)
        {
            // In service, this may not enabled
            if (msg_.Msg == MsgQuit || msg_.Msg == MsgQueryEndSession)
            {
                //if(msg_.Msg == MsgQueryEndSession)
                //{
                //    // 如果是关机或注销（为防止注销或未成功关机而引起的退出），尝试重启
                //    XProcess.StartService(Application.StartupPath, ProgramForm.ServiceName, 60);
                //}
                this.CanExit = true;
                base.Close();
            }

            base.WndProc(ref msg_);
        }

        /// <summary>
        /// 
        /// </summary>
        protected void ShowTrayIcon()
        {
            if (!this.IsHideTrayIcon)
            {
                if (this._iconNoitfy == null)
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
                                    this.CanExit = true;
                                    base.Close();
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
    }
}
