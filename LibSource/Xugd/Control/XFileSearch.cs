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
    /// �����ļ������Ĵ�����
    /// </summary>
    public partial class XFileSearch : Form
    {
        #region �������Ͷ���

        private delegate void DelNoParam();
        private delegate void DelAddItem(string strFilePath_, bool bIsFolder_, object oTag_);
        /// <summary>
        /// ˫�����������ļ�ʱ�������Ĳ�����һ���Ƿ��ص������岢����ʾ˫�����ļ�
        /// </summary>
        /// <param name="oTag_">�ļ��ı�ʶ��Ϣ����ȫ·����</param>
        public delegate void DelDoubleClickSearchedFile(object oTag_);
        /// <summary>
        /// �����ļ�������ί��
        /// </summary>
        /// <param name="strPath_">Ҫ������·��</param>
        /// <param name="strPattern_">��ͨ�������������</param>
        /// <param name="strKeyWord_">Ҫ���������ݣ�����ͨ�����</param>
        public delegate void DelSearchFiles(string strPath_, string strPattern_, string strKeyWord_);

        #endregion

        #region ��������
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
        /// ���洰��ı��⣨���ļ�δ�ҵ���δ�趨��������ʱ��������ı��⣩
        /// </summary>
        public string WarningTitle
        {
            set 
            {
                _strWarnTitle = value;
            }
        }

        /// <summary>
        /// ͨ�����Ĭ��Ϊ"*"����������ݿ���������ҪΪ"%"
        /// </summary>
        public string WildCard
        {
            set
            {
                _strWildCard = value;
            }
        }

        /// <summary>
        /// �趨�ļ����������������Ϊnull��ʹ��Ĭ�ϵ��������������������ļ���
        /// </summary>
        /// <param name="delSearch_">����������ί��</param>
        public void SetSearchFile(DelSearchFiles delSearch_)
        {
            if (null == delSearch_)
                _delSearch = new DelSearchFiles(DefSearchFun);
            else
                _delSearch = delSearch_;
        }

        /// <summary>
        /// ��ʾ����
        /// </summary>
        /// <param name="frmOwner_">�����ߴ��壨һ�㴫this���ɣ�</param>
        /// <param name="strCur_">��ǰ·������ʱ��Ĭ�ϵ�����·����</param>
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
        /// ����������б�����ʾ���������ļ�
        /// </summary>
        /// <param name="strFilePath_">�ļ���ȫ·��</param>
        /// <param name="bIsFolder_">�Ƿ����ļ���</param>
        /// <param name="oTag_">�ļ�Ψһ��ʶ�������ļ�һ��Ϊȫ·����</param>
        public void ToAddItem(string strFilePath_, bool bIsFolder_, object oTag_)
        {
            this.Invoke(_delAddItem, strFilePath_, bIsFolder_, oTag_);
        }

        /// <summary>
        /// ���캯������Ҫ����������Ϊ��
        ///   &lt;Form Name="SearchFile"&gt;
        ///    &lt;Title&gt;���ذ�ȫ���ļ�����&lt;/Title&gt;
        ///    &lt;ListView&gt;
        ///     &lt;Name&gt;����&lt;/Name&gt;
        ///     &lt;Path&gt;����·��&lt;/Path&gt;
        ///    &lt;/ListView&gt;
        ///    &lt;Caption&gt;
        ///     &lt;SearchTips&gt;������������������&lt;/SearchTips&gt;
        ///     &lt;ResultTips&gt;���������˫���ɴ��ļ�����Ŀ¼��&lt;/ResultTips&gt;
        ///     &lt;SearchWord&gt;ȫ���򲿷��ļ���&lt;/SearchWord&gt;
        ///     &lt;SearchHere&gt;����������&lt;/SearchHere&gt;
        ///     &lt;Search&gt;����&lt;/Search&gt;
        ///     &lt;RootName&gt;���ذ�ȫ��&lt;/RootName&gt;
        ///    &lt;/Caption&gt;
        ///    &lt;Prompt&gt;
        ///     &lt;NoSearchWord&gt;������Ҫ�������ļ�����&lt;/NoSearchWord&gt;
        ///     &lt;NotFound&gt;û���ҵ������������ļ�����������������������&lt;/NotFound&gt;
        ///     &lt;Searching&gt;�������������Ժ����ԡ�&lt;/Searching&gt;
        ///    &lt;/Prompt&gt;
        ///   &lt;/Form&gt;
        /// </summary>
        /// <param name="conReader_">���ڻ�ȡ������Ϣ��xml��дʵ��</param>
        /// <param name="strRoot_">��·��</param>
        /// <param name="delClick_">˫��ѡ���ļ�ʱ������������ί��</param>
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
            {   // ʹ�õ�һ����Ч���ַ�����Ϊ�ؼ��֣���ƥ�䣨Indexof����ʵ���ļ���
                //������GetDirectories��GetFiles���ص��ļ�����ʱ��ʵ��ƥ��Ķ࣬���Ի�ȡ�ļ���������Indexofƥ��һ�Σ�
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