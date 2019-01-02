using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using SHCre.Xugd.Config;
using SHCre.Xugd.WinAPI;

namespace SHCre.Xugd.XControl
{
    /// <summary>
    /// 用于文件搜索的窗体类
    /// </summary>
    public partial class XFileSearch : Form
    {
        #region 辅助类型定义

        private delegate void DelNoParam();
        private delegate void DelAddItem(string strFilePath_, bool bIsFolder_, object oTag_);
        /// <summary>
        /// 双击搜索到的文件时，触发的操作：一般是返回到主窗体并且显示双击的文件
        /// </summary>
        /// <param name="oTag_">文件的标识信息（如全路径）</param>
        public delegate void DelDoubleClickSearchedFile(object oTag_);
        /// <summary>
        /// 搜索文件操作的委托
        /// </summary>
        /// <param name="strPath_">要搜索的路径</param>
        /// <param name="strPattern_">带通配符的搜索内容</param>
        /// <param name="strKeyWord_">要搜索的内容（不带通配符）</param>
        public delegate void DelSearchFiles(string strPath_, string strPattern_, string strKeyWord_);

        #endregion

        #region 变量定义
        string _strRootName;
        XConReader _xmlReader;
        private string _strNodeFrm;
        private string _strNodePrompt;
        private string _strNodeCaption;
        private string _strRootPath;
        private string _strWarnTitle = "Warning";
        private string _strWildCard = "*";
        private char[] _chWildeCards = new char[] { '*', '?' };
        private Mutex _synSearch;
        private SHCre.Xugd.CFile.XFileImgs _imgList;

        DelDoubleClickSearchedFile _delDbClickSerachFile = null;
        DelSearchFiles _delSearch = null;
        DelAddItem _delAddItem = null;
        #endregion

        /// <summary>
        /// 警告窗体的标题（如文件未找到、未设定搜索内容时给出警告的标题）
        /// </summary>
        public string WarningTitle
        {
            set 
            {
                _strWarnTitle = value;
            }
        }

        /// <summary>
        /// 通配符：默认为"*"，如果是数据库搜索则需要为"%"
        /// </summary>
        public string WildCard
        {
            set
            {
                _strWildCard = value;
            }
        }

        /// <summary>
        /// 设定文件的搜索函数，如果为null则使用默认的搜索函数（搜索本地文件）
        /// </summary>
        /// <param name="delSearch_">用于搜索的委托</param>
        public void SetSearchFile(DelSearchFiles delSearch_)
        {
            if (null == delSearch_)
                _delSearch = new DelSearchFiles(DefSearchFun);
            else
                _delSearch = delSearch_;
        }

        /// <summary>
        /// 显示窗体
        /// </summary>
        /// <param name="frmOwner_">调用者窗体（一般传this即可）</param>
        /// <param name="strCur_">当前路径（打开时，默认的搜索路径）</param>
        public void ToShow(IWin32Window frmOwner_, string strCur_)
        {
            int nStartX = ((Form)frmOwner_).Width - this.Width - (this.Width - this.ClientRectangle.Width);
            int nStartY = ((Form)frmOwner_).Height - this.Height - (this.Height - this.ClientRectangle.Height);
            Point stPos = new Point(nStartX, nStartY);
            this.Location = ((Form)frmOwner_).PointToScreen(stPos);
            if( !this.Visible )
                this.Show(frmOwner_);
            SetSearchPath(strCur_);
        }

        /// <summary>
        /// 在搜索结果列表中显示搜索到的文件
        /// </summary>
        /// <param name="strFilePath_">文件的全路径</param>
        /// <param name="bIsFolder_">是否是文件夹</param>
        /// <param name="oTag_">文件唯一标识（本地文件一般为全路径）</param>
        public void ToAddItem(string strFilePath_, bool bIsFolder_, object oTag_)
        {
            this.Invoke(_delAddItem, strFilePath_, bIsFolder_, oTag_);
        }

        /// <summary>
        /// 构造函数，需要的配置内容为：
        ///   &lt;Form Name="SearchFile"&gt;
        ///    &lt;Title&gt;本地安全域—文件搜索&lt;/Title&gt;
        ///    &lt;ListView&gt;
        ///     &lt;Name&gt;名称&lt;/Name&gt;
        ///     &lt;Path&gt;所在路径&lt;/Path&gt;
        ///    &lt;/ListView&gt;
        ///    &lt;Caption&gt;
        ///     &lt;SearchTips&gt;按以下条件进行搜索&lt;/SearchTips&gt;
        ///     &lt;ResultTips&gt;搜索结果（双击可打开文件所在目录）&lt;/ResultTips&gt;
        ///     &lt;SearchWord&gt;全部或部分文件名&lt;/SearchWord&gt;
        ///     &lt;SearchHere&gt;在这里搜索&lt;/SearchHere&gt;
        ///     &lt;Search&gt;搜索&lt;/Search&gt;
        ///     &lt;RootName&gt;本地安全域&lt;/RootName&gt;
        ///    &lt;/Caption&gt;
        ///    &lt;Prompt&gt;
        ///     &lt;NoSearchWord&gt;请输入要搜索的文件名！&lt;/NoSearchWord&gt;
        ///     &lt;NotFound&gt;没有找到符合条件的文件，请重新输入搜索条件！&lt;/NotFound&gt;
        ///     &lt;Searching&gt;正在搜索，请稍后重试。&lt;/Searching&gt;
        ///    &lt;/Prompt&gt;
        ///   &lt;/Form&gt;
        /// </summary>
        /// <param name="conReader_">用于获取配置信息的xml读写实例</param>
        /// <param name="strRoot_">根路径</param>
        /// <param name="delClick_">双击选中文件时，触发操作的委托</param>
        public XFileSearch(XConReader conReader_, string strRoot_, DelDoubleClickSearchedFile delClick_)
        {
            InitializeComponent();

            this.TopLevel = true;
            _xmlReader = conReader_;
            _strRootPath = strRoot_;
            _delDbClickSerachFile = delClick_;
            _imgList = new SHCre.Xugd.CFile.XFileImgs();
            _delAddItem = new DelAddItem(AddItemToResults);
            SetSearchFile(null);
        }

        private void FrmSearch_Load(object sender, EventArgs e)
        {
            lvwResults.SmallImageList = _imgList.GetList();
            _strNodeFrm = XConNode.Combine("Form", "Name", "SearchFile");
            _strNodePrompt = XConNode.Combine(_strNodeFrm, "Prompt");
            _strNodeCaption = XConNode.Combine(_strNodeFrm, "Caption");

            this.Text = _xmlReader.GetText(XConNode.Combine(_strNodeFrm, "Title"), this.Text);

            string strNodeList = XConNode.Combine(_strNodeFrm, "ListView");
            lvwResults.Columns.Add(_xmlReader.GetText(XConNode.Combine(strNodeList, "Name"), "Name"), 200, HorizontalAlignment.Left);
            lvwResults.Columns.Add(_xmlReader.GetText(XConNode.Combine(strNodeList, "Path"), "Path"), 330, HorizontalAlignment.Left);

            grpCondition.Text = _xmlReader.GetText(XConNode.Combine(_strNodeCaption, "SearchTips"), grpCondition.Text);
            lblContext.Text = _xmlReader.GetText(XConNode.Combine(_strNodeCaption, "SearchWord"), lblContext.Text);
            lblSearchHere.Text = _xmlReader.GetText(XConNode.Combine(_strNodeCaption, "SearchHere"), lblSearchHere.Text);
            btnSearch.Text = _xmlReader.GetText(XConNode.Combine(_strNodeCaption, "Search"), btnSearch.Text);
            lblResults.Text = _xmlReader.GetText(XConNode.Combine(_strNodeCaption, "ResultTips"), lblResults.Text);

            _strRootName = _xmlReader.GetText(XConNode.Combine(_strNodeCaption, "RootName"), Path.GetFileName(_strRootName));

            _synSearch = new Mutex();
            txtContext.Select();
        }

        private string GetSearchWord(string strInput_)
        {
            return _strWildCard + strInput_ + _strWildCard;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (!TryLock(true))
                return;

            string strKeys = this.txtContext.Text.Trim();
            if(strKeys.Length == 0)
            {
                MessageBox.Show(this, _xmlReader.GetText(XConNode.Combine(_strNodePrompt, "NoSearchWord"), "Input the search key-word please!"),
                    _strWarnTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.txtContext.Select();
                return;
            }

            strKeys = strKeys.Trim(_chWildeCards);
            string[] oParams = new string[] { this.txtSearchHere.Tag.ToString(), GetSearchWord(strKeys), strKeys};
            Thread thrSearch = new Thread(new ParameterizedThreadStart(SearchFileThread));
            thrSearch.IsBackground = true;
            thrSearch.Start(oParams);
        }

        private bool TryLock(bool bShowTip_)
        {
            if (!_synSearch.WaitOne(0, false))
            {
                if (bShowTip_)
                    MessageBox.Show(this, _xmlReader.GetText(XConNode.Combine(_strNodePrompt, "Searching"), "Is searching, try later!"),
                        _strWarnTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            _synSearch.ReleaseMutex();
            return true;
        }
    
        private string GetFilePath(string strFile_)
        {
            string strDir = Path.GetDirectoryName(strFile_);
            string strPath = _strRootName + Path.VolumeSeparatorChar + Path.DirectorySeparatorChar;
            if (strDir.Length > _strRootPath.Length)
                strPath += strDir.Substring(_strRootPath.Length + 1);

            return strPath;
        }

        private void DefSearchFun(string strSearchPath_, string strPattern_, string strKeyWord_)
        {
            string[] strKeys = strKeyWord_.Split(_chWildeCards);
            strKeyWord_ = string.Empty;
            foreach(string str in strKeys)
            {   // 使用第一个有效的字符串作为关键字，来匹配（Indexof）真实的文件名
                //（由于GetDirectories与GetFiles返回的文件名有时比实际匹配的多，所以获取文件名后，再用Indexof匹配一次）
                if(str.Length > 0 )
                {
                    strKeyWord_ = str;
                    break;
                }
            }

            string[] strDirs = Directory.GetDirectories(strSearchPath_, strPattern_, SearchOption.AllDirectories);
            foreach (string strFolder in strDirs)
            {
                string strName = Path.GetFileName(strFolder);
                if (-1 == strName.IndexOf(strKeyWord_, StringComparison.OrdinalIgnoreCase))
                    continue;

                ToAddItem(strFolder, true, strFolder);
            }

            string[] strFiles = Directory.GetFiles(strSearchPath_, strPattern_, SearchOption.AllDirectories);
            foreach (string strF in strFiles)
            {
                string strName = Path.GetFileName(strF);
                if (-1 == strName.IndexOf(strKeyWord_, StringComparison.OrdinalIgnoreCase))
                    continue;

                ToAddItem(strF, false, strF);
            }
        }

        private void SearchFileThread(object oParam_)
        {
            try
            {
                _synSearch.WaitOne();

                string[] oLocParams = (string[])oParam_;
                this.Invoke(new DelNoParam(SearchBegin));

                _delSearch(oLocParams[0], oLocParams[1], oLocParams[2]);
            }
            catch(Exception) {}
            finally
            {
                _synSearch.ReleaseMutex();
                if( !this.IsDisposed )
                    this.Invoke(new DelNoParam(SearchEnd));
            }
        }

        private void AddItemToResults(string strFilePath_, bool bIsFolder_, object oTag_)
        {
            ListViewItem lvItem = new ListViewItem(Path.GetFileName(strFilePath_));
            lvItem.SubItems.Add(GetFilePath(strFilePath_));
            lvItem.Tag = oTag_;
            if (bIsFolder_)
                lvItem.ImageIndex = _imgList.GetDirIndex();
            else
                lvItem.ImageIndex = _imgList.GetFileIndex(strFilePath_);

            this.lvwResults.Items.Add(lvItem);
        }

        private void SearchBegin()
        {
            this.lvwResults.Items.Clear();

            proBar.Style = ProgressBarStyle.Marquee;
            proBar.Value = 0;
        }

        private void SearchEnd()
        {
            proBar.Style = ProgressBarStyle.Blocks;
            proBar.Value = 0;
            txtContext.Select();

            if (this.lvwResults.Items.Count == 0)
            {
                string strMsg = _xmlReader.GetText(XConNode.Combine(_strNodePrompt, "NotFound"), "No file found, research please!");
                MessageBox.Show(this, strMsg, _strWarnTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void lvwResults_DoubleClick(object sender, EventArgs e)
        {
            if (lvwResults.SelectedItems.Count == 0)
                return;

            if (null != _delDbClickSerachFile)
                _delDbClickSerachFile(lvwResults.SelectedItems[0].Tag.ToString());
        }

        private void SetSearchPath(string strPath_)
        {
            if(string.IsNullOrEmpty(strPath_))
            {
                strPath_ = _strRootPath;
            }

            this.txtSearchHere.Text = XSafeArea.GetShowPath(strPath_, _strRootPath, _strRootName);
            this.txtSearchHere.Tag = strPath_;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (!TryLock(true))
                return;

            XFolderDlg frmDir = new XFolderDlg(_xmlReader, _strRootPath, (string)this.txtSearchHere.Tag);
            if(frmDir.ShowDialog()==DialogResult.OK)
            {
                SetSearchPath(frmDir.SelectPath);
            }
        }

        private void FrmSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (TryLock(false))
                    this.Hide();
            }
        }

        private void FrmSearch_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!TryLock(true))
            {
                e.Cancel = true;
            }
        }
    }
}