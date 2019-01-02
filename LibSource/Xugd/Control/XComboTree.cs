using System;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using SHCre.Xugd.WinAPI;

namespace SHCre.Xugd.XControl
{
    /// <summary>
    /// �ڽڵ�ѡ��ʱ������������ί��
    /// </summary>
    /// <param name="sender">������</param>
    /// <param name="e">ѡ�нڵ���Ϣ</param>
    public delegate void SelectedNodeEventHandler(object sender, TreeViewEventArgs e);

    /// <summary>
    /// ��ַ����Ͽ�ؼ���ʹ�÷�����
    /// ��ʹ��SetRoot�趨��ַ��Ҫ����ĸ�Ŀ¼��
    /// �ڸı�ʱʹ��SetPath�趨��ǰ·��
    /// ΪAfterSelect��Ӵ������̣�����������ѡ��ĳ��ʱ������
    /// </summary>
    public class XAddrCombo : UserControl
    {
        #region Var
        private Panel _pnlBack;
        private Panel _pnlTree;
        private ListView _lvwTxtBox; // ʹ��ListViewģ��Textbox���Ա���ʾͼ��
        private XButtonEx _btnSelect;
        private TreeView _tvwTreeView;
        private XLabelEx _lblSizeGrip;
        private Form _frmComboTree;
        private bool _bInited = false;
        string _strRootPath = string.Empty;
        string _strRootTreeShow = string.Empty;
        string _strRootTextShow = string.Empty;

        private System.Drawing.Point _ptDragOffset;
        SHCre.Xugd.CFile.XFileImgs _imgList = new SHCre.Xugd.CFile.XFileImgs();

        #endregion

        #region ��������
        /// <summary>
        /// ��ȡ�������οؼ��ڵ㼯��
        /// </summary>
        [Description("��ȡ���ĸ���㼯")]
        public TreeNodeCollection Nodes
        {
            get
            {
                return this._tvwTreeView.Nodes;
            }
        }

        /// <summary>
        /// ��ȡ���趨ѡ�еĽڵ㣨TagΪ�ڵ��Ӧ��·����TextΪ��ʾ���ݣ�
        /// </summary>
        [Description("��ȡ���趨��ǰѡ�еĽڵ�")]
        public TreeNode SelectedNode
        {
            get
            {
                return _tvwTreeView.SelectedNode;
            }
        }

        /// <summary>
        /// ��ȡ���趨ͼ���б�
        /// </summary>
        [Description("��ȡ��ѡ��ͼ���б�")]
        private ImageList SmallImageList
        {
            get
            {
                return this._tvwTreeView.ImageList;
            }

            set
            {
                this._lvwTxtBox.SmallImageList =
                    this._tvwTreeView.ImageList = value;
            }
        }

        /// <summary>
        /// ���ؿؼ���Text���ԣ���ȡ���趨�ؼ���Text
        /// </summary>
        public override string Text
        {
            get
            {
                if (this._lvwTxtBox.Items.Count == 0)
                    return string.Empty;

                return this._lvwTxtBox.Items[0].SubItems[0].Text;
            }

            set
            {
                int nIndex = _imgList.GetDirIndex();
                this._lvwTxtBox.Items.Clear();
                this._lvwTxtBox.Items.Add(value, nIndex);
            }
        }

        /// <summary>
        /// �ؼ��ĸ߶�
        /// </summary>
        [Description("�趨�ؼ��ı��򲿷ֵĸ߶�"),
          DefaultValue(21)]
        public int High
        {
            get
            {
                return this._lvwTxtBox.Height;
            }

            set
            {
                this._lvwTxtBox.Height = value;
            }
        }

        /// <summary>
        /// ��ȡ��ǰ�Ƿ��н���
        /// </summary>
        public new bool Focused
        {
            get
            {
                return _lvwTxtBox.Focused;
            }
        }

        #endregion

        /// <summary>
        /// ȱʡ���캯��
        /// </summary>
        public XAddrCombo()
        {
            this.InitializeComponent();

            this._pnlBack = new Panel();
            this._pnlBack.BorderStyle = BorderStyle.Fixed3D;
            this._pnlBack.BackColor = Color.White;
            this._pnlBack.AutoScroll = false;

            this._lvwTxtBox = new ListView();
            this._lvwTxtBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._lvwTxtBox.KeyDown += new KeyEventHandler(OnBoxKeyDown);
            this._lvwTxtBox.HeaderStyle = ColumnHeaderStyle.None;
            this._lvwTxtBox.View = View.Details;
            this._lvwTxtBox.Size = new Size(107, 21);
            this._lvwTxtBox.Columns.Add("Name", 107);
            this._lvwTxtBox.Items.Add("");

            this._btnSelect = new XButtonEx();
            this._btnSelect.Click += new EventHandler(OnBtnClick);
            this._btnSelect.FlatStyle = FlatStyle.Flat;

            this._lblSizeGrip = new XLabelEx();
            this._lblSizeGrip.Size = new Size(9, 9);
            this._lblSizeGrip.BackColor = Color.Transparent;
            this._lblSizeGrip.Cursor = Cursors.SizeNWSE;
            this._lblSizeGrip.MouseMove += new MouseEventHandler(SizeGripMouseMove);
            this._lblSizeGrip.MouseDown += new MouseEventHandler(SizeGripMouseDown);

            this._tvwTreeView = new TreeView();
            this._tvwTreeView.BorderStyle = BorderStyle.None;
            this._tvwTreeView.FullRowSelect = true;
            this._tvwTreeView.ShowPlusMinus = false;
            this._tvwTreeView.ShowLines = false;
            this._tvwTreeView.Location = new Point(0, 0);
            this._tvwTreeView.LostFocus += new EventHandler(TreeViewLostFocus);
            this._tvwTreeView.AfterSelect += new TreeViewEventHandler(AfterSelectNode);
            this._tvwTreeView.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(TreeDoubleClick);

            this._frmComboTree = new Form();
            this._frmComboTree.FormBorderStyle = FormBorderStyle.None;
            this._frmComboTree.StartPosition = FormStartPosition.Manual;
            this._frmComboTree.ShowInTaskbar = false;
            this._frmComboTree.BackColor = System.Drawing.SystemColors.Control;

            this._pnlTree = new Panel();
            this._pnlTree.BorderStyle = BorderStyle.FixedSingle;
            this._pnlTree.BackColor = Color.White;

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            this._pnlTree.Controls.Add(this._lblSizeGrip);
            this._pnlTree.Controls.Add(this._tvwTreeView);
            this._frmComboTree.Controls.Add(this._pnlTree);
            this._pnlBack.Controls.AddRange(new Control[] { _btnSelect, _lvwTxtBox });
            this.Controls.Add(this._pnlBack);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "XComboTree";
            this.Size = new System.Drawing.Size(107, 21);

            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.XComboTree_Layout);
            this.ResumeLayout(false);
        }

        private void XComboTree_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            this.Height = this._lvwTxtBox.Height + 2;
            this._pnlBack.Size = new Size(this.Width, this.Height);

            this._btnSelect.Size = new Size(16, this.Height - 4);
            this._btnSelect.Location = new Point(this.Width - this._btnSelect.Width - 4, 0);

            this._lvwTxtBox.Location = new Point(2, 2);
            this._lvwTxtBox.Width = this.Width - this._btnSelect.Width - 4;
            this._lvwTxtBox.Columns[0].Width = this._lvwTxtBox.Width;

            this._frmComboTree.Size = new Size(this.Width, 200);
            this._pnlTree.Size = this._frmComboTree.Size;
            this._tvwTreeView.Width = this._frmComboTree.Width - this._lblSizeGrip.Width;
            this._tvwTreeView.Height = this._frmComboTree.Height - this._lblSizeGrip.Width;
            this.RelocateGrip();

            _bInited = true;
        }

        /// <summary>
        /// �趨��Ŀ¼����ʾ��Ϣ����ʹ��ǰ����Ҫ���趨��
        /// </summary>
        /// <param name="strRoot_">�ؼ������ĸ�Ŀ¼</param>
        /// <param name="strShowText_">��Ŀ¼����ʾ</param>
        public void SetRoot(string strRoot_, string strShowText_)
        {
            if (string.IsNullOrEmpty(strRoot_) || string.IsNullOrEmpty(strShowText_))
                throw new ArgumentException("Argument is null or empty");

            this._strRootPath = strRoot_.TrimEnd(Path.DirectorySeparatorChar);
            this._strRootTreeShow = strShowText_;
            this._strRootTextShow = XSafeArea.GetShowPath(strRoot_, _strRootPath, _strRootTreeShow); ;

            this.SmallImageList = _imgList.GetList();
            SetPath(this._strRootPath);
        }

        /// <summary>
        /// �趨��ǰ·���������Ǹ�Ŀ¼����Ŀ¼��
        /// </summary>
        /// <param name="strPath_">Ҫ�趨��·��</param>
        public void SetPath(string strPath_)
        {
            if (string.IsNullOrEmpty(strPath_))
                throw new ArgumentException("Argument path is null or empty");
            if (!strPath_.StartsWith(_strRootPath))
                throw new ArgumentException("The path is not the sub folder of the root");

            this.Text = XSafeArea.GetShowPath(strPath_, _strRootPath, _strRootTreeShow);
        }

        private void RelocateGrip()
        {
            this._lblSizeGrip.Top = this._frmComboTree.Height - _lblSizeGrip.Height - 1;
            this._lblSizeGrip.Left = this._frmComboTree.Width - _lblSizeGrip.Width - 1;
        }

        private void OnGetSubNode(TreeNode trNode_, string strPath_)
        {
            string strRelative = strPath_.Substring(this._strRootTextShow.Length).Trim(Path.DirectorySeparatorChar);  
            string[] strSubs = strRelative.Split(Path.DirectorySeparatorChar);
            TreeNode trCur = trNode_;
            int nIndex = _imgList.GetDirIndex();
            string strCur = _strRootPath;
            foreach (string strFolder in strSubs)
            {
                if (strFolder.Length == 0)
                    break;

                TreeNode trSub = new TreeNode(strFolder);
                trSub.ImageIndex =
                    trSub.SelectedImageIndex = nIndex;
                strCur = Path.Combine(strCur, strFolder);
                trSub.Tag = strCur;
                trCur.Nodes.Add(trSub);
                trCur = trSub;
            }
            this._tvwTreeView.SelectedNode = trCur;
        }

        private void ShowInTree()
        {
            if (string.IsNullOrEmpty(this._strRootPath))
                return;

            this._tvwTreeView.Nodes.Clear();
            TreeNode trRoot = new TreeNode(this._strRootTreeShow);
            trRoot.Tag = _strRootPath;
            trRoot.ImageIndex = 
                trRoot.SelectedImageIndex = _imgList.GetDirIndex();
            this._tvwTreeView.Nodes.Add(trRoot);

            if (this.Text.Length > (this._strRootTextShow.Length ))  
                OnGetSubNode(trRoot, this.Text);

            this._tvwTreeView.ExpandAll();
        }

        private void OnBtnClick(object sender, EventArgs e)
        {
            if (!this._frmComboTree.Visible)
            {
                ShowInTree();
                Rectangle Rect = this.RectangleToScreen(this.ClientRectangle);
                this._frmComboTree.Location = new System.Drawing.Point(Rect.X, Rect.Y + this._pnlBack.Height);

                this._frmComboTree.Show();
                this._frmComboTree.BringToFront();
                this._frmComboTree.Show();

                this.RelocateGrip();
            }
            else
            {
                this._frmComboTree.Hide();
            }
        }

        #region �����¼�

        /// <summary>
        /// ����ѡ�����ݺ���
        /// </summary>
        [Description("���οؼ�ѡ�����ݸı�󴥷��Ĳ���"), Category("Behavior")]
        public event SelectedNodeEventHandler AfterSelect;
        /// <summary>
        /// ���״ΰ���ĳ����ʱ����
        /// </summary>
        public new event KeyEventHandler KeyDown;

        #endregion

        #region ˽���¼�����

        private void SizeGripMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int nWidth, nHeight;
                nWidth = Cursor.Position.X - this._frmComboTree.Location.X;
                nWidth = nWidth + this._ptDragOffset.X;
                nHeight = Cursor.Position.Y - this._frmComboTree.Location.Y;
                nHeight = nHeight + this._ptDragOffset.Y;

                if (nWidth < 50)
                    nWidth = 50;
                if (nHeight < 50)
                    nHeight = 50;

                this._frmComboTree.Size = new System.Drawing.Size(nWidth, nHeight);
                this._pnlTree.Size = this._frmComboTree.Size;
                this._tvwTreeView.Size = new System.Drawing.Size(this._frmComboTree.Size.Width - this._lblSizeGrip.Width, this._frmComboTree.Size.Height - this._lblSizeGrip.Width); ;
                RelocateGrip();
            }
        }

        private void SizeGripMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int nOffsetX = System.Math.Abs(Cursor.Position.X - this._frmComboTree.RectangleToScreen(this._frmComboTree.ClientRectangle).Right);
                int nOffsetY = System.Math.Abs(Cursor.Position.Y - this._frmComboTree.RectangleToScreen(this._frmComboTree.ClientRectangle).Bottom);

                this._ptDragOffset = new Point(nOffsetX, nOffsetY);
            }
        }

        // ������������밴��ʱ����������Ϣ
        private void OnBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (KeyDown != null)
            {
                KeyDown(this, e);
            }
        }

        // ʧȥ����ʱ���������οؼ�
        private void TreeViewLostFocus(object sender, EventArgs e)
        {
            try
            {
                if (!this._btnSelect.RectangleToScreen(this._btnSelect.ClientRectangle).Contains(Cursor.Position))
                    this._frmComboTree.Hide();
            }
            catch (ObjectDisposedException) { }
        }

        private void TreeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // just forbid expand or collapse the tree node
        }

        private string GetShowText(TreeNode trNode_)
        {
            if (null == trNode_)
                return string.Empty;
            if (null == trNode_.Parent)
                return trNode_.Text + Path.VolumeSeparatorChar + Path.DirectorySeparatorChar;

            string strParen = GetShowText(trNode_.Parent);
            return Path.Combine(strParen, trNode_.Text);
        }

        private void AfterSelectNode(object sender, TreeViewEventArgs e)
        {
            if (!_bInited)
                return;

            this.Text = GetShowText(e.Node);
            if (AfterSelect != null)
            {
                AfterSelect(this, e);
            }

            this._frmComboTree.Hide();
        }

        #endregion

        #region XLabelEx

        private class XLabelEx : Label
        {
            public XLabelEx()
            {
                this.SetStyle(ControlStyles.UserPaint, true);
                this.SetStyle(ControlStyles.DoubleBuffer, true);
                this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                System.Windows.Forms.ControlPaint.DrawSizeGrip(e.Graphics, System.Drawing.Color.Red, 1, 0, this.Size.Width, this.Size.Height);
            }
        }

        #endregion

        #region XButtonEx

        private class XButtonEx : Button
        {
            ButtonState state;

            public XButtonEx()
            {
                this.SetStyle(ControlStyles.UserPaint, true);
                this.SetStyle(ControlStyles.DoubleBuffer, true);
                this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                state = ButtonState.Pushed;
                base.OnMouseDown(e);
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                state = ButtonState.Normal;
                base.OnMouseUp(e);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                System.Windows.Forms.ControlPaint.DrawComboButton(e.Graphics, 0, 0, this.Width, this.Height, state);
            }
        }
        #endregion

    }
}
