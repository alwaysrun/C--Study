using System;
using System.Windows.Forms; 

namespace SHCre.Xugd.XControl
{
    /// <summary>
    /// ��Ҫ�����ļ�/�ļ�����ʾ���б���������������
    /// </summary>
    public class XFileView : ListView
    {
        /// <summary>
        /// ���캯�����б�������ΪDetails������ѡ�С�������ʾѡ��״̬������ʾ��ʾ��Ϣ��ToolTips����
        /// ��������˫������ģʽ�����Ż���ʾ������һ������ļ�ʱ����������У���
        /// ʹ�÷�����
        ///  ��XFileView�滻ListView����ͨ��SetSort�趨������Ϣ�������Ҫ���򣩣�
        ///  ������ʱ������ToSort���ɡ�
        /// </summary>
        public XFileView()
        {
            this.View = View.Details;
            this.HideSelection = false;
            this.ShowItemToolTips = true;
            this.FullRowSelect = true;
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
        }

        /// <summary>
        /// �趨���򷽷����趨����������Ҫ���趨���б�����ٵ��á�
        /// �趨���е����򷽷������б�Ϊ���ļ�������С�����͡�ʱ�䣩��
        ///  XSorter.CmpMode[] lstModes = new XSorter.CmpMode[]{XSorter.CmpMode.FileName, XSorter.CmpMode.FileSize, XSorter.CmpMode.FileName, XSorter.CmpMode.FileTime};
        ///  SetSort(delIsForder, lstModes);
        /// </summary>
        /// <param name="delCheck_">�����ж���Ϊ�ļ������ļ��е�ί��</param>
        /// <param name="lstModes_">�趨ÿһ�е����򷽷�</param>
        public void SetSort(XSorter.DelIsFolder delCheck_, params XSorter.CmpMode[] lstModes_)
        {
            XSorter lvSort = new XSorter();
            lvSort.SetFolderCheck(delCheck_);
            lvSort.SetCmpModes(lstModes_);
            if( this.Columns.Count > 0 )
                lvSort.SetColText(this, 0);
            this.ListViewItemSorter = lvSort;
        }

        /// <summary>
        /// ��ʼ����
        /// </summary>
        /// <param name="nClickCol_">Ҫ�������</param>
        public void ToSort(int nClickCol_)
        {
            XSorter lvSort = this.ListViewItemSorter as XSorter;
            if (null == lvSort)
                throw new System.InvalidOperationException("Compare mode not set, use SetSort to set first please");

            lvSort.SetColText(this, nClickCol_);
            this.Sort();
        }

        //////////////////////////////////////////////////////////////////////////
        //      Compare class for sorter
        //////////////////////////////////////////////////////////////////////////        
        /// <summary>
        /// �����б��������
        /// </summary>
        public class XSorter : System.Collections.IComparer
        {
            private const char c_chAsc = '��';
            private const char c_chDes = '��';

            private int _nSortCol;
            private SortOrder _euOrder;

            /// <summary>
            /// ��ȡ��ǰ����ķ�ʽ
            /// </summary>
            public SortOrder Order
            {
                get
                {
                    return _euOrder;
                }
            }

            /// <summary>
            /// ��ȡ��ǰ�������
            /// </summary>
            public int Column
            {
                get
                {
                    return _nSortCol;
                }
            }

            /// <summary>
            /// ����ʱ�ȽϷ�ʽ��������ļ���صģ�������ʱ�ļ���
            /// �����ļ�ǰ�棬Ȼ���ļ������ļ��ֱ��ڲ�����
            /// </summary>
            public enum CmpMode
            {
                /// <summary>
                /// δ֪��ʽ�Ƚϣ�δ�趨����ʽ��ʹ���ַ�����ʽ�Ƚ�
                /// </summary>
                None=0,
                /// <summary>
                /// �ļ����ƱȽϣ�����ͨ���ַ����Ƚϣ��������ļ����ļ��еĶ���ʹ�ô˷�ʽ����
                /// �Ƚ����ļ����ļ��бȽϣ��ٽ����ַ����Ƚ�
                /// </summary>
                FileName=1,
                /// <summary>
                /// �ļ����ͱȽϣ��Ƚ����ļ����ļ��бȽϣ�
                /// �ٽ����ַ����Ƚ�
                /// </summary>
                FileType=1,
                /// <summary>
                /// �ļ�����޸�ʱ��Ƚϣ��Ƚ����ļ������ļ��Ƚϣ�
                /// �ٽ���ʱ��Ƚ�
                /// </summary>
                FileTime,
                /// <summary>
                /// �ļ���С�Ƚϣ��ļ���û�д�С���ļ�����KB���м��㣬
                /// ���м�Ҫ�пո�,ǧ��λʹ�ö��ţ���'1,234 KB')��
                /// Ϊ�˱��ڱȽ���ʹ��SHCre.Xugd.CFile.XFile.FSize2String����ʾ�ļ���С
                /// </summary>
                FileSize,
                /// <summary>
                /// ���ַ�����ʽ���бȽ�
                /// </summary>
                String,
                /// <summary>
                /// �����ֽ��бȽ�
                /// </summary>
                Number,
                /// <summary>
                /// ��ʱ����бȽ�
                /// </summary>
                DateTime,
                /// <summary>
                /// �������ַ���бȽ�
                /// </summary>
                IPV4Addr,
            };

            /// <summary>
            /// �����ж��Ƿ�Ϊ�ļ��е�ί��
            /// </summary>
            /// <param name="lvItem_">��</param>
            /// <returns>�ļ��У�true������false</returns>
            public delegate bool DelIsFolder(ListViewItem lvItem_);
            private DelIsFolder _delIsFolder = new DelIsFolder(delegate(ListViewItem lvItem_) { return false; });
            private CmpMode[] _lstModes = null;

            /// <summary>
            /// �趨�ж��ļ��е�ί��
            /// </summary>
            /// <param name="delCheck_">�ж��ļ��е�ί��</param>
            public void SetFolderCheck(DelIsFolder delCheck_)
            {
                if( null != delCheck_ )
                    _delIsFolder = delCheck_;
            }

            /// <summary>
            /// �趨����ķ�ʽ
            /// </summary>
            /// <param name="lstModes_">����ʽ��ÿһ�ж�Ӧһ����</param>
            public void SetCmpModes(params CmpMode[] lstModes_)
            {
                _lstModes = lstModes_;
            }

            /// <summary>
            /// �趨�б�ͷ
            /// </summary>
            /// <param name="lvFiles_">�б�</param>
            /// <param name="nClickCol_">��ǰ˫������</param>
            public void SetColText(ListView lvFiles_, int nClickCol_)
            {
                if (nClickCol_ == _nSortCol)
                {
                    if (_euOrder == SortOrder.Ascending)
                    {
                        lvFiles_.Columns[nClickCol_].Text = lvFiles_.Columns[nClickCol_].Text.TrimEnd(c_chAsc) + c_chDes.ToString();
                        _euOrder = SortOrder.Descending;
                    }
                    else
                    {
                        lvFiles_.Columns[nClickCol_].Text = lvFiles_.Columns[nClickCol_].Text.TrimEnd(c_chDes) + c_chAsc.ToString();
                        _euOrder = SortOrder.Ascending;
                    }
                }
                else
                {
                    // Sort at a new column, set ascending
                    lvFiles_.Columns[_nSortCol].Text = lvFiles_.Columns[_nSortCol].Text.TrimEnd(c_chAsc, c_chDes);
                    lvFiles_.Columns[nClickCol_].Text += c_chAsc.ToString();
                    _nSortCol = nClickCol_;
                    _euOrder = SortOrder.Ascending;
                }
            }

            /// <summary>
            /// ���캯��
            /// </summary>
            public XSorter()
            {
                _nSortCol = 0;
                _euOrder = SortOrder.None; // In default, not sort
            }

            /// <summary>
            /// ���б��Զ����õıȽϺ���
            /// </summary>
            /// <param name="oFirst_">Ҫ�Ƚϵĵ�һ��</param>
            /// <param name="oSecond_">Ҫ�Ƚϵĵڶ���</param>
            /// <returns>�ȽϽ��</returns>
            public int Compare(object oFirst_, object oSecond_)
            {
                int nResult = 0;
                ListViewItem lvFirst = (ListViewItem)oFirst_;
                ListViewItem lvSecond = (ListViewItem)oSecond_;

                switch (GetCmpMode(_nSortCol))
                {
                    case CmpMode.FileName:  
                        nResult = CompareFileName(lvFirst, lvSecond);
                        break;

                    case CmpMode.FileTime:
                        nResult = CompareFileTime(lvFirst, lvSecond);
                        break;

                    case CmpMode.FileSize:
                        nResult = CompareFileSize(lvFirst.SubItems[_nSortCol].Text, lvSecond.SubItems[_nSortCol].Text);
                        break;

                    case CmpMode.Number:
                        nResult = CompareNumber(lvFirst.SubItems[_nSortCol].Text, lvSecond.SubItems[_nSortCol].Text);
                        break;

                    case CmpMode.DateTime:
                        nResult = CompareTime(lvFirst.SubItems[_nSortCol].Text, lvSecond.SubItems[_nSortCol].Text);
                        break;

                    case CmpMode.IPV4Addr:
                        nResult = CompareIPAddr(lvFirst.SubItems[_nSortCol].Text, lvSecond.SubItems[_nSortCol].Text);
                        break;

                    case CmpMode.String: // go through
                    default:
                        nResult = string.Compare(lvFirst.SubItems[_nSortCol].Text, lvSecond.SubItems[_nSortCol].Text);
                        break;
                }

                if (_euOrder == SortOrder.Descending)
                    nResult = -nResult;

                return nResult;
            }

            private CmpMode GetCmpMode(int nCol_)
            {
                if ((null == _lstModes) || (nCol_ >= _lstModes.Length))
                    return CmpMode.None;

                return _lstModes[nCol_];
            }

            private int IsFolderCheck(ListViewItem lvFirst_, ListViewItem lvSecond_)
            {
                try
                {
                    bool bFirstIsFolder = _delIsFolder(lvFirst_);
                    bool bSecondIsFolder = _delIsFolder(lvSecond_);

                    if (bFirstIsFolder && !bSecondIsFolder)
                    {   // Folder before file
                        return -1;
                    }
                    if (!bFirstIsFolder && bSecondIsFolder)
                    {   // File before folder
                        return 1;
                    }
                }
                catch{}

                return 0;
            }

            private int CompareFileName(ListViewItem lvFirst_, ListViewItem lvSecond_)
            {
                int nCmp = IsFolderCheck(lvFirst_, lvSecond_);
                if (nCmp != 0)
                    return nCmp;

                // both folder, or both file
                return string.Compare(lvFirst_.SubItems[_nSortCol].Text, lvSecond_.SubItems[_nSortCol].Text);
            }

            private int CompareFileTime(ListViewItem lvFirst_, ListViewItem lvSecond_)
            {
                int nCmp = IsFolderCheck(lvFirst_, lvSecond_);
                if (nCmp != 0)
                    return nCmp;

                // both folder, or both file
                return CompareTime(lvFirst_.SubItems[_nSortCol].Text, lvSecond_.SubItems[_nSortCol].Text);
            }

            private int CompareFileSize(string strFirst_, string strSecond_)
            {
                if (strFirst_.Length == 0)
                    return -1;
                if (strSecond_.Length == 0)
                    return 1;

                // both has size(is number)
                string[] strFirstNums = strFirst_.Split();
                string[] strSecondNums = strSecond_.Split();
                int nFirst = int.Parse(strFirstNums[0].Replace(",",""));
                int nSecond = int.Parse(strSecondNums[0].Replace(",", ""));
                if (nFirst > nSecond)
                    return 1;
                if (nFirst < nSecond)
                    return -1;

                return 0;
            }

            private int CompareNumber(string strFirst_, string strSecond_)
            {
                if (strFirst_.Length == 0)
                    return -1;
                if (strSecond_.Length == 0)
                    return 1;

                int nFirst = int.Parse(strFirst_);
                int nSecond = int.Parse(strSecond_);
                if (nFirst > nSecond)
                    return 1;
                if (nFirst < nSecond)
                    return -1;

                return 0;
            }

            private int CompareTime(string strFirst_, string strSecond_)
            {
                if (strFirst_.Length == 0)
                    return -1;
                if (strSecond_.Length == 0)
                    return 1;

                DateTime dtFirst = DateTime.Parse(strFirst_);
                DateTime dtSecond = DateTime.Parse(strSecond_);

                return DateTime.Compare(dtFirst, dtSecond);
            }

//             public bool IsIPv4(string strIPAddr_)
//             {
//                 return System.Text.RegularExpressions.Regex.Match(strIPAddr_, @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$").Success;
//             }

            private int CompareIPAddr(string strFirst_, string strSecond_)
            {
                if (strFirst_.Length == 0)
                    return -1;
                if (strSecond_.Length == 0)
                    return 1;

                string[] strFirstIPs = strFirst_.Split('.');
                string[] strSecondIPs = strSecond_.Split('.');

                for (int i = 0; i < 4; i++)
                {
                    int nFirst = int.Parse(strFirstIPs[i]);
                    int nSecond = int.Parse(strSecondIPs[i]);

                    if ( nFirst > nSecond )
                    {
                        return 1;
                    }
                    else if ( nFirst < nSecond )
                    {
                        return -1;
                    }
                }

                return 0;
            }
        }
        
    }
}
