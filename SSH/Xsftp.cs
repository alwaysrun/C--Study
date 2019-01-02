using System;
using System.Collections.Generic;
using System.Text;
using Tamir.SharpSsh;
using System.IO;
using System.Threading;
using SHCre.Xugd.Common;
using SHCre.Xugd.CFile;

namespace SHCre.Xugd.Ssh
{
    /// <summary>
    /// 通过SSH的sftp进行文件传输（使用22端口）:
    /// Connect
    /// [SetBaseDir]
    /// Upload/Download
    /// Close
    /// </summary>
    public class Xsftp : Sftp
    {
        const string PathSeperator = @"/";
        string _strBaseDir = string.Empty;
        bool _bTransferring = false;
        bool _bTransferOver = false;
        int _nFtpPort = 0;
        ManualResetEvent _evtTransferWait = new ManualResetEvent(false);

        /// <summary>
        /// 传输回调Transfer(Source, Destination, Mode, TransferredSize, TotalSize)
        /// </summary>
        public event Action<string, string, TransferMode, int, int> OnFileTransfer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strHost_">主机（IP地址）</param>
        /// <param name="strUser_">用户名</param>
        /// <param name="strPsw_">密码</param>
        public Xsftp(string strHost_, string strUser_, string strPsw_)
            : base(strHost_, strUser_, strPsw_)
        {
            base.OnTransferStart += new FileTransferEvent(Xsftp_OnTransferStart);
            base.OnTransferProgress += new FileTransferEvent(Xsftp_OnTransferProgress);
            base.OnTransferEnd += new FileTransferEvent(Xsftp_OnTransferEnd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conFtp_"></param>
        public Xsftp(XsftpConfig conFtp_)
            : this(conFtp_.Address, conFtp_.User, conFtp_.Psw)
        {
            _nFtpPort = conFtp_.Port;
            if (!string.IsNullOrEmpty(conFtp_.BasePath))
                SetBaseDir(conFtp_.BasePath);
        }

        /// <summary>
        /// 连接
        /// </summary>
        public override void Connect()
        {
            if (_nFtpPort == 0)
                base.Connect();
            else
                Connect(_nFtpPort);

            if(Connected && !string.IsNullOrEmpty(_strBaseDir))
            {
                // todo: check whether is a valid dir.
            }
        }

        /// <summary>
        /// 设定远端的基目录（所有上传文件都在此目录即子目录下）
        /// </summary>
        /// <param name="strDir_"></param>
        public void SetBaseDir(string strDir_)
        {
            if (string.IsNullOrEmpty(strDir_))
                throw new ArgumentException("Dir can not empty");

            strDir_ = XPath.Win2LinuxPath(strDir_);
            if(Connected)
            {
                if (!IsDir(strDir_))
                    throw new ArgumentException("It not a valid path");
            }

            _strBaseDir = strDir_;
            if (!_strBaseDir.EndsWith(PathSeperator))
                _strBaseDir += PathSeperator;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="strLocalFile_">要上传的文件</param>
        /// <param name="strRemote_">上传目的地: 若以‘/’开始则为绝对路径；
        /// 否则为相对路径（相对SetBaseDir设定的目录）；若是带扩展名，则看作目标文件名；否则看作路径（使用源文件名）</param>
        public void Upload(string strLocalFile_, string strRemote_)
        {
            if (!File.Exists(strLocalFile_))
                throw new ArgumentException("Local file not exists");

            string strDest = string.Empty;
            if (!string.IsNullOrEmpty(strRemote_))
            {
                strRemote_ = XPath.Win2LinuxPath(strRemote_);
                if (strRemote_.StartsWith(PathSeperator))
                {
                    strDest = strRemote_;
                }
                else
                {
                    strDest = _strBaseDir + strRemote_;
                }

                var strExt = Path.GetExtension(strLocalFile_);
                if (strDest.EndsWith(strExt, StringComparison.OrdinalIgnoreCase))
                { // Consider is file
                    string strDestPath = Path.GetDirectoryName(strDest);
                    strDestPath = XPath.Win2LinuxPath(strDestPath);
                    if (!IsDir(strDestPath))
                        Mkdir(strDestPath); 
                }
                else
                {
                    if (!IsDir(strDest))
                        Mkdir(strDest);
                }
            }
            else
            {
                strDest = _strBaseDir;
            }

            Put(strLocalFile_, strDest);
        }

        /// <summary>
        /// 同步传输文件（最多等待WaitSecond秒）
        /// </summary>
        /// <param name="strLocalFile_"></param>
        /// <param name="strRemotePath_">若是带扩展名，则看作目标文件名；否则看作路径（使用源文件名）</param>
        /// <param name="nWaitSecond_"></param>
        /// <returns></returns>
        public bool UploadSync(string strLocalFile_, string strRemotePath_, int nWaitSecond_=30)
        {
            if (_bTransferring)
                throw new XConflictException("Another file is Transferring");

            try
            {
                _bTransferring = true;
                _bTransferOver = false;
                _evtTransferWait.Reset();
                
                Upload(strLocalFile_, strRemotePath_);
                if (_evtTransferWait.WaitOne(nWaitSecond_ * 1000))
                {
                    return _bTransferOver;
                }
            }
            finally
            {
                _bTransferring = false;
            }

            return false;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="strRemoteFile_">要下载的文件</param>
        /// <param name="strLocal_">文件存放路径</param>
        public void Download(string strRemoteFile_, string strLocal_)
        {
            string strSource = XPath.Win2LinuxPath(strRemoteFile_);
            if (!strSource.StartsWith(PathSeperator))
            {
                strSource = _strBaseDir + strRemoteFile_;
            }
            Get(strSource, strLocal_);
        }

        /// <summary>
        /// 同步传输文件（最多等待WaitSecond秒）
        /// </summary>
        /// <param name="strRemoteFile_"></param>
        /// <param name="strLocal_"></param>
        /// <param name="nWaitSecond_"></param>
        /// <returns></returns>
        public bool DownloadSync(string strRemoteFile_, string strLocal_, int nWaitSecond_ = 30)
        {
            if (_bTransferring)
                throw new XConflictException("Another file is Transferring");

            try
            {
                _bTransferring = true;
                _bTransferOver = false;
                _evtTransferWait.Reset();

                Download(strRemoteFile_, strLocal_);
                if (_evtTransferWait.WaitOne(nWaitSecond_ * 1000))
                {
                    return _bTransferOver;
                }
            }
            finally
            {
                _bTransferring = false;
            }

            return false;
        }
        
        /// <summary>
        /// 判断是否是文件夹
        /// </summary>
        /// <param name="strDir"></param>
        /// <returns></returns>
        public override bool IsDir(string strDir)
        {
            return base.IsDir(strDir);
        }

        /// <summary>
        /// 判断是否是文件
        /// </summary>
        /// <param name="strPath_"></param>
        /// <returns></returns>
        public override bool IsFile(string strPath_)
        {
            return base.IsFile(strPath_);
        }

		/// <summary>
		/// 创建目录
		/// </summary>
		/// <param name="strDir_"></param>
		public override void Mkdir(string strDir_)
		{
            base.Mkdir(strDir_);
		}

		/// <summary>
		/// 获取指定目录下的文件列表
		/// </summary>
		/// <param name="strPath_"></param>
		/// <returns></returns>
        public override List<XFtpFileInfo> GetFileList(string strPath_)
        {
            return base.GetFileList(strPath_);
        }

        /// <summary>
        /// 传送模式
        /// </summary>
        public enum TransferMode
        {
            /// <summary>
            /// 开始
            /// </summary>
            Start,
            /// <summary>
            /// 进度
            /// </summary>
            Progress,
            /// <summary>
            /// 完成
            /// </summary>
            End,
        }
        void Xsftp_OnTransferEnd(string src, string dst, int transferredBytes, int totalBytes, string message)
        {
            _bTransferOver = transferredBytes == totalBytes;
            _evtTransferWait.Set();

            if (OnFileTransfer != null)
                OnFileTransfer(src, dst, TransferMode.End, transferredBytes, totalBytes);
        }

        void Xsftp_OnTransferProgress(string src, string dst, int transferredBytes, int totalBytes, string message)
        {
            if (OnFileTransfer != null)
                OnFileTransfer(src, dst, TransferMode.Progress, transferredBytes, totalBytes);
        }

        void Xsftp_OnTransferStart(string src, string dst, int transferredBytes, int totalBytes, string message)
        {
            if (OnFileTransfer != null)
                OnFileTransfer(src, dst, TransferMode.Start, transferredBytes, totalBytes);
        }
    } // class
}
