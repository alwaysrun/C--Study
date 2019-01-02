using System;
using System.Drawing;
using System.Windows.Forms;
using SHCre.Xugd.WinAPI;

namespace SHCre.Xugd.Program
{
    /// <summary>
    /// 用于在托盘中显示的类：双击托盘图标后会还原
    /// </summary>
    public class XFormTray
    {
        private Form _frmParent;
        private Icon _iconTray;
        private string _strText;
        private NotifyIcon _iconNoitfy;
        private ContextMenu _menuContext;
        private WhenShowInTray _euShowMode;

        /// <summary>
        /// 点击关闭时是否退出；
        /// 只有在设定了显示时机为ShowTrayTimeMode.WhenClosing；
        /// 且自定义托盘右键菜单时才需要处理（在真正要退出时设为true）
        /// </summary>
        public bool ExitWhenClose {get; set;}

        /// <summary>
        /// 什么时候在托盘中显示
        /// </summary>
        public enum WhenShowInTray
        {
            /// <summary>
            /// 由调用者通过调用ShowInTray来手动处理
            /// </summary>
            None,
            /// <summary>
            /// 最小化时
            /// </summary>
            Minimize,
            /// <summary>
            /// 点击关闭按钮时：如果ExitWhenClose为true，则退出；否则，显示托盘（隐藏主窗体）
            /// </summary>
            Closing,
        }
        
        private bool _bAlwaysShow = false;
        /// <summary>
        /// 是否总是显示托盘：
        /// 默认情况下，如果主窗体显示时，托盘会隐藏。
        /// 如果设定了总是显示，则在程序退出时要调用QuitTray来取消托盘显示。
        /// </summary>
        public bool AlwaysShowTray 
        {
            get { return _bAlwaysShow; } 
            set
            {
                if (_bAlwaysShow == value)
                    return;

                _bAlwaysShow = value;
                if (_bAlwaysShow)
                {
                    ShowTray();
                }
                else
                {
                    if (_frmParent.Visible)
                        QuitTray();
                }
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="frmShow_">要显示的主窗体</param>
        /// <param name="euMode_">何时显示在托盘中</param>
        /// <param name="iconTray_">在托盘中显示的图标，不设定则使用窗体的主图标</param>
        /// <param name="strIconText_">在托盘中鼠标指向时显示的文本，不设定在使用窗体的标题</param>
        /// <param name="conMenu_">右击托盘图片时弹出的菜单</param>
        public XFormTray(Form frmShow_, WhenShowInTray euMode_=WhenShowInTray.Minimize, Icon iconTray_ = null, string strIconText_=null, ContextMenu conMenu_=null)
        {
            if(conMenu_ == null)
            {
                conMenu_ = BuildDefContextMenu();
            }
            Init(frmShow_, euMode_, iconTray_, strIconText_, conMenu_);

            HookFormEvent();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="frmShow_">要显示的主窗体</param>
        /// <param name="strShowMenuText_">托盘菜单中的‘显示’菜单标题</param>
        /// <param name="strQuitMenuText_">托盘菜单中的‘退出’菜单标题</param>
        /// <param name="euMode_">何时显示在托盘中</param>
        /// <param name="iconTray_">在托盘中显示的图标，不设定则使用窗体的主图标</param>
        /// <param name="strIconText_">在托盘中鼠标指向时显示的文本，不设定在使用窗体的标题</param>
        public XFormTray(Form frmShow_, string strShowMenuText_, string strQuitMenuText_, WhenShowInTray euMode_ = WhenShowInTray.Minimize, Icon iconTray_ = null, string strIconText_ = null)
        {
            Init(frmShow_, euMode_, iconTray_, strIconText_, BuildDefContextMenu(strShowMenuText_, strQuitMenuText_));

            HookFormEvent();
        }

        private void Init(Form frmShow_, WhenShowInTray euMode_, Icon iconTray_, string strIconText_, ContextMenu conMenu_)
        {
            _frmParent = frmShow_;

            if (iconTray_ == null)
                _iconTray = _frmParent.Icon;
            else
                _iconTray = iconTray_;

            if (string.IsNullOrEmpty(strIconText_))
                _strText = _frmParent.Text;
            else
                _strText = strIconText_;

            _euShowMode = euMode_;
            _menuContext = conMenu_;
        }

        private ContextMenu BuildDefContextMenu(string strShowMenuText_ = null, string strQuitMenuText_ = null)
        {
            if (string.IsNullOrEmpty(strShowMenuText_))
                strShowMenuText_ = "&Show";
            if (string.IsNullOrEmpty(strQuitMenuText_))
                strQuitMenuText_ = "&Quit";

            return new ContextMenu(new MenuItem[]
                {
                    new MenuItem(strShowMenuText_, (zo, ze)=>{
                            XWin.ShowAtTop(_frmParent);
                            if (!_bAlwaysShow)
                                _iconNoitfy.Visible = false;
                        }),
                    new MenuItem(strQuitMenuText_, (zo, ze)=>
                        {
                            _iconNoitfy.Visible = false;
                            ExitWhenClose = true;
                            _frmParent.Close();
                        }),
                });
        }

        private void HookFormEvent()
        {
            ExitWhenClose = true;

            if(_euShowMode== WhenShowInTray.Minimize)
            {
                _frmParent.Resize += (sz, ez) =>
                    {
                        if (_frmParent.WindowState == FormWindowState.Minimized)
                        {
                            ShowInTray();
                        }
                    };
            }
            else if(_euShowMode == WhenShowInTray.Closing)
            {
                ExitWhenClose = false;
                _frmParent.FormClosing += (sz, ez) =>
                    {
                        ShowInTray();
                        ez.Cancel = true;
                        return;
                    };
            }
        }

        private void ShowTray()
        {
            if (this._iconNoitfy == null)
            {
                this._iconNoitfy = new NotifyIcon();
                this._iconNoitfy.Icon = _iconTray;
                this._iconNoitfy.Text = _strText;
                this._iconNoitfy.DoubleClick += new EventHandler(IconNoitfy_DoubleClick);
                this._iconNoitfy.ContextMenu = _menuContext;
            }

            this._iconNoitfy.Visible = true;
        }

        /// <summary>
        /// 在托盘中显示，并隐藏主窗体
        /// </summary>
        public void ShowInTray()
        {
            ShowTray();
            _frmParent.Visible = false;
        }

        /// <summary>
        /// 在退出时调用
        /// </summary>
        public void QuitTray()
        {
            if (this._iconNoitfy != null)
                this._iconNoitfy.Visible = false;
        }

        void IconNoitfy_DoubleClick(object sender, EventArgs e)
        {
            //_frmParent.Visible = true;
            XWin.ShowAtTop(_frmParent);

            if (!_bAlwaysShow)
                this._iconNoitfy.Visible = false;
        }
    }
}
