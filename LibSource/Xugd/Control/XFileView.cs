using System;
using System.Windows.Forms; 

namespace SHCre.Xugd.XControl
{
    /// <summary>
    /// 主要用于文件/文件夹显示的列表，并包含了排序类
    /// </summary>
    public class XFileView : ListView
    {
        /// <summary>
        /// 构造函数：列表类型设为Details，整行选中、总是显示选中状态、并显示提示信息（ToolTips）；
        /// 并启用了双缓冲区模式，来优化显示（所以一般添加文件时，按分组进行）。
        /// 使用方法：
        ///  用XFileView替换ListView，并通过SetSort设定排序信息（如果需要排序）；
        ///  在排序时，调用ToSort即可。
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
        /// 设定排序方法，设定后将启用排序，要在设定了列标题后再调用。
        /// 设定各列的排序方法（设列表为：文件名、大小、类型、时间）：
        ///  XSorter.CmpMode[] lstModes = new XSorter.CmpMode[]{XSorter.CmpMode.FileName, XSorter.CmpMode.FileSize, XSorter.CmpMode.FileName, XSorter.CmpMode.FileTime};
        ///  SetSort(delIsForder, lstModes);
        /// </summary>
        /// <param name="delCheck_">用于判断列为文件还是文件夹的委托</param>
        /// <param name="lstModes_">设定每一列的排序方法</param>
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
        /// 开始排序
        /// </summary>
        /// <param name="nClickCol_">要排序的列</param>
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
        /// 用于列表排序的类
        /// </summary>
        public class XSorter : System.Collections.IComparer
        {
            private const char c_chAsc = 'Δ';
            private const char c_chDes = '▽';

            private int _nSortCol;
            private SortOrder _euOrder;

            /// <summary>
            /// 获取当前排序的方式
            /// </summary>
            public SortOrder Order
            {
                get
                {
                    return _euOrder;
                }
            }

            /// <summary>
            /// 获取当前排序的列
            /// </summary>
            public int Column
            {
                get
                {
                    return _nSortCol;
                }
            }

            /// <summary>
            /// 排序时比较方式：如果是文件相关的，则正序时文件夹
            /// 排在文件前面，然后文件夹与文件分别内部排序
            /// </summary>
            public enum CmpMode
            {
                /// <summary>
                /// 未知方式比较：未设定排序方式，使用字符串方式比较
                /// </summary>
                None=0,
                /// <summary>
                /// 文件名称比较（所有通过字符串比较，且区分文件与文件夹的都可使用此方式）：
                /// 先进行文件与文件夹比较，再进行字符串比较
                /// </summary>
                FileName=1,
                /// <summary>
                /// 文件类型比较：先进行文件与文件夹比较，
                /// 再进行字符串比较
                /// </summary>
                FileType=1,
                /// <summary>
                /// 文件最后修改时间比较：先进行文件夹与文件比较，
                /// 再进行时间比较
                /// </summary>
                FileTime,
                /// <summary>
                /// 文件大小比较：文件夹没有大小，文件则以KB进行计算，
                /// 且中间要有空格,千分位使用逗号（如'1,234 KB')；
                /// 为了便于比较请使用SHCre.Xugd.CFile.XFile.FSize2String来显示文件大小
                /// </summary>
                FileSize,
                /// <summary>
                /// 按字符串方式进行比较
                /// </summary>
                String,
                /// <summary>
                /// 按数字进行比较
                /// </summary>
                Number,
                /// <summary>
                /// 按时间进行比较
                /// </summary>
                DateTime,
                /// <summary>
                /// 按网络地址进行比较
                /// </summary>
                IPV4Addr,
            };

            /// <summary>
            /// 用于判断是否为文件夹的委托
            /// </summary>
            /// <param name="lvItem_">列</param>
            /// <returns>文件夹，true；否则false</returns>
            public delegate bool DelIsFolder(ListViewItem lvItem_);
            private DelIsFolder _delIsFolder = new DelIsFolder(delegate(ListViewItem lvItem_) { return false; });
            private CmpMode[] _lstModes = null;

            /// <summary>
            /// 设定判断文件夹的委托
            /// </summary>
            /// <param name="delCheck_">判断文件夹的委托</param>
            public void SetFolderCheck(DelIsFolder delCheck_)
            {
                if( null != delCheck_ )
                    _delIsFolder = delCheck_;
            }

            /// <summary>
            /// 设定排序的方式
            /// </summary>
            /// <param name="lstModes_">排序方式（每一列对应一个）</param>
            public void SetCmpModes(params CmpMode[] lstModes_)
            {
                _lstModes = lstModes_;
            }

            /// <summary>
            /// 设定列表头
            /// </summary>
            /// <param name="lvFiles_">列表</param>
            /// <param name="nClickCol_">当前双击的列</param>
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
            /// 构造函数
            /// </summary>
            public XSorter()
            {
                _nSortCol = 0;
                _euOrder = SortOrder.None; // In default, not sort
            }

            /// <summary>
            /// 由列表自动调用的比较函数
            /// </summary>
            /// <param name="oFirst_">要比较的第一列</param>
            /// <param name="oSecond_">要比较的第二列</param>
            /// <returns>比较结果</returns>
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
