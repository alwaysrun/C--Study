using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using SHCre.Xugd.WinAPI;
using SHCre.Xugd.CFile;

namespace SHCre.Xugd.XControl
{
    /// <summary>
    /// ������ļ���Ϣ����
    /// </summary>
    public class XOpenedFiles
    {
        /// <summary>
        /// �Ѵ򿪣�����δ�����½��̵��ļ�
        /// </summary>
        private List<string> _lstOpenFiles = new List<string>();

        /// <summary>
        /// �Ѵ򿪣��������½��̵��ļ�
        /// </summary>
        private Dictionary<string, int> _dicOpenedFiles = new Dictionary<string, int>();

        /// <summary>
        /// ���ļ�����¼�������ʧ����򿪡��ļ��򿪷�ʽ��
        /// </summary>
        /// <param name="strFile_">�ļ�����ȫ·����</param>
        /// <returns>�ɹ��򿪣�����true����ʧ�ܣ�����false</returns>
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
                else // ʹ���Ѵ򿪵Ľ��̴��ļ���Office���һ������������ģ�
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
        /// �ر��Ѵ򿪵��ļ������ܱ�֤���ж��ܹرգ�
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
        /// ��ȡ�Ѵ򿪵��ļ��б�
        /// </summary>
        /// <returns>�Ѵ򿪵��ļ��б�</returns>
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
        /// �����ѹرյ��ļ���һ����ö�ʱ����������ر��ļ��Ƚ϶�ʱ��������
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
        /// ��������Ѵ򿪵��ļ���Ϣ
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
