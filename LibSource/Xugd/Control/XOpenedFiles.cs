using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using SHCre.Xugd.WinAPI;
using SHCre.Xugd.CFile;

namespace SHCre.Xugd.XControl
{
    /// <summary>
    /// 处理打开文件信息的类
    /// </summary>
    public class XOpenedFiles
    {
        /// <summary>
        /// 已打开，但是未启动新进程的文件
        /// </summary>
        private List<string> _lstOpenFiles = new List<string>();

        /// <summary>
        /// 已打开，且启动新进程的文件
        /// </summary>
        private Dictionary<string, int> _dicOpenedFiles = new Dictionary<string, int>();

        /// <summary>
        /// 打开文件并记录，如果打开失败则打开‘文件打开方式’
        /// </summary>
        /// <param name="strFile_">文件名（全路径）</param>
        /// <returns>成功打开，返回true；打开失败，返回false</returns>
        public bool TryOpenFile(string strFile_)
        {
            bool bOpened = false;
            try
            {
                Process procOpen = Process.Start(strFile_);
                if (procOpen != null)
                {
                    if (_dicOpenedFiles.ContainsKey(strFile_))
                    {
                        _dicOpenedFiles[strFile_] = procOpen.Id;
                    }
                    else
                    {
                        _dicOpenedFiles.Add(strFile_, procOpen.Id);
                    }
                }
                else // 使用已打开的进程打开文件（Office软件一般是这样处理的）
                {
                    if (!_lstOpenFiles.Contains(strFile_) && !_dicOpenedFiles.ContainsKey(strFile_))
                        _lstOpenFiles.Add(strFile_);
                }

                bOpened = true;
            }
            catch
            {
                XShell.ShellExec(strFile_, XShell.SHExecAction.OpenAs);
            }

            return bOpened;
        }

        /// <summary>
        /// 关闭已打开的文件（不能保证所有都能关闭）
        /// </summary>
        public void TryCloseFile()
        {
            foreach (KeyValuePair<string, int> kvFile in _dicOpenedFiles)
            {
                try
                {
                    Process procOpened = Process.GetProcessById(kvFile.Value);
                    if (procOpened.MainWindowTitle.Contains(Path.GetFileName(kvFile.Key)))
                        procOpened.CloseMainWindow();
                    procOpened.Close();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 获取已打开的文件列表
        /// </summary>
        /// <returns>已打开的文件列表</returns>
        public List<string> OpenedFiles()
        {
            List<string> lstOpened = new List<string>(_lstOpenFiles.Count + 1);
            // Files in dictionary
            foreach (KeyValuePair<string, int> kvFile in _dicOpenedFiles)
            {
                if (IsProcessExisted(kvFile.Value, kvFile.Key))
                    lstOpened.Add(kvFile.Key);
            }

            // Files in list
            foreach(string strFile in _lstOpenFiles)
            {
                if (XFile.IsInUse(strFile))
                    lstOpened.Add(strFile);
            }

            return lstOpened;
        }

        /// <summary>
        /// 清理已关闭的文件（一般最好定时进行清理，或关闭文件比较多时进行清理）
        /// </summary>
        public void RemoveClosedFile()
        {
            List<string> lstClosed = new List<string>(_dicOpenedFiles.Count);
            // Remove closed file in dictionary
            foreach (KeyValuePair<string, int> kvFile in _dicOpenedFiles)
            {
                if (!IsProcessExisted(kvFile.Value, kvFile.Key))
                    lstClosed.Add(kvFile.Key);
            }
            foreach(string strClosed in lstClosed)
            {
                _dicOpenedFiles.Remove(strClosed);
            }

            // Files in list
            _lstOpenFiles.RemoveAll(new Predicate<string>(delegate(string strFile_)
                {
                    return !XFile.IsInUse(strFile_);
                }));
        }

        /// <summary>
        /// 情况所有已打开的文件信息
        /// </summary>
        public void Clear()
        {
            _dicOpenedFiles.Clear();
            _lstOpenFiles.Clear();
        }

        private bool IsProcessExisted(int strProcId_, string strFile_)
        {
            bool bExisted = false;
            try
            {
                Process procOpened = Process.GetProcessById(strProcId_);
                if (procOpened.MainWindowTitle.Contains(Path.GetFileName(strFile_)))
                    bExisted = true;
                procOpened.Close();
            }
            catch
            {
            }

            return bExisted;
        }
    }
}
