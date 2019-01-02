using System;
using System.IO;
using System.Windows.Forms;

using SHCre.Xugd.Config;
using SHCre.Xugd.WinAPI;

namespace SHCre.Xugd.XControl
{
    /// <summary>
    /// 选择文件夹的窗体
    /// </summary>
    public partial class XFolderDlg : Form
    {
        private SHCre.Xugd.CFile.XFileImgs _imgTree;

        int nIndex;
        string _strRootPath;
        string _strCurPath;
        string _strNodeFrm;
        XConReader _xmlReader;

        /// <summary>
        /// 获取当前选中的路径
        /// </summary>
        public string SelectPath
        {
            get
            {
                return (string)this.tvwFolder.SelectedNode.Tag;
            }
        }

        /// <summary>
        /// 构造函数，需要的配置文件内容为：
        ///   &lt;Form Name="SelectFolder"&gt;
        ///    &lt;Title&gt;选择文件夹&lt;/Title&gt;
        ///    &lt;RootName&gt;最顶层目录的名称（如本地安全域）&lt;/RootName&gt;
        ///    &lt;OK&gt;确定&lt;/OK&gt;
        ///    &lt;Cancel&gt;取消&lt;/Cancel&gt;
        ///   &lt;/Form&gt;
        /// </summary>
        /// <param name="conReader_">用于获取配置信息的xml读写实例</param>
        /// <param name="strRootPath_">要选择的路径的根目录（只能选择此目录与子目录）</param>
        /// <param name="strCurPath_">当前路径（打开时，默认选中此路径）</param>
        public XFolderDlg(XConReader conReader_, string strRootPath_, string strCurPath_)
        {
            InitializeComponent();

            _xmlReader = conReader_;
            _strRootPath = strRootPath_;
            _strCurPath = strCurPath_;
            _imgTree = new SHCre.Xugd.CFile.XFileImgs();
        }

        private void FrmSelectFolder_Load(object sender, EventArgs e)
        {
            tvwFolder.ImageList = _imgTree.GetList();
            nIndex = _imgTree.GetDirIndex();
            _strNodeFrm = XConNode.Combine("Form", "Name", "SelectFolder");

            InitFormCaption();
            FillTreeView();
        }

        private void InitFormCaption()
        {
            this.Text = _xmlReader.GetText(XConNode.Combine(_strNodeFrm, "Title"), this.Text);
            btnOk.Text = _xmlReader.GetText(XConNode.Combine(_strNodeFrm, "OK"), btnOk.Text);
            btnCancel.Text = _xmlReader.GetText(XConNode.Combine(_strNodeFrm, "Cancel"), btnCancel.Text);
        }

        private void FillEmptyNode(TreeNodeCollection trParent_, string strPath_)
        {
            string[] strFolders = Directory.GetDirectories(strPath_);
            foreach (string strName in strFolders)
            {
                TreeNode trNode = new TreeNode(Path.GetFileName(strName));
                trNode.Name = trNode.Text;
                trNode.Tag = strName;
                trNode.ImageIndex =
                    trNode.SelectedImageIndex = nIndex;
                if (Directory.GetDirectories(strName).Length > 0)
                {
                    TreeNode emptyNode = new TreeNode(string.Empty);
                    trNode.Nodes.Add(emptyNode);
                }
                trParent_.Add(trNode);
            }
        }

        private TreeNode FillSubNodes(TreeNodeCollection trcParent_, string strPath_, string strName_)
        {
            trcParent_.Clear();
            FillEmptyNode(trcParent_, strPath_);
            TreeNode[] trFound = trcParent_.Find(strName_, false);
            return (trFound.Length > 0) ? trFound[0] : null;
        }

        private void FillTreeView()
        {
            string strRoot = _xmlReader.GetText(XConNode.Combine(_strNodeFrm, "RootName"), Path.GetFileName(_strRootPath));
            TreeNode trRoot = new TreeNode(strRoot);
            trRoot.Name = Path.GetFileName(_strRootPath);
            trRoot.Tag = _strRootPath;
            trRoot.ImageIndex = 
                trRoot.SelectedImageIndex = nIndex;
            tvwFolder.Nodes.Add(trRoot);

            string strPath = _strRootPath;
            TreeNode trCurNode = trRoot;
            if(_strCurPath.Length > (_strRootPath.Length+1))
            {
                string[] strFolders = _strCurPath.Substring(_strRootPath.Length + 1).Split('\\');
                foreach(string strDir in strFolders)
                {
                    trCurNode = FillSubNodes(trCurNode.Nodes, strPath, strDir);
                    if (null == trCurNode)
                        return;
                    strPath = Path.Combine(strPath, strDir);
                }
            }
            FillSubNodes(trCurNode.Nodes, strPath, string.Empty);

            trCurNode.Expand();
            tvwFolder.SelectedNode = trCurNode;
        }

        private void tvwFolder_AfterExpand(object sender, TreeViewEventArgs e)
        {
            TreeNode trExpand=e.Node;

            if((trExpand.Nodes.Count==1) && (trExpand.Nodes[0].Text.Length==0))
            {
                trExpand.Nodes.Clear();
                FillEmptyNode(trExpand.Nodes, (string)trExpand.Tag);
            }
        }

        private void tvwFolder_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.btnOk.Enabled = ((string)this.tvwFolder.SelectedNode.Tag != _strCurPath);
        }
    }
}