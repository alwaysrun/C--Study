using System;
using System.Text;
using System.Text.RegularExpressions;

using System.IO;
using System.Net;
using System.Collections.Generic;

namespace SHCre.Xugd.Net.Ftp
{
    /// <summary>
    /// FTP客户端操作类。 
    /// 所有接口中传递的文件名与路径：如果以'/'或'\'开始，则为绝对路径；
    /// 否则，为相对路径，使用CurrentDir+指定路径作为全路径。
    /// 文件上传与下载时，可通过设定回调函数来获取真实的进度信息。
    /// </summary>
    public class FtpClient
    {
        Uri _uriFtp;
        string _strFtpUser;
        string _strFtpPsw;

        /// <summary>
        /// 构造函数（端口默认为21）
        /// </summary>
        /// <param name="strAddr_">FTP服务器的地址（IP地址或域名）</param>
        /// <param name="strUser_">登录账号名（如果是匿名登录，可为空）</param>
        /// <param name="strPsw_">登录口令</param>
        public FtpClient(string strAddr_, string strUser_, string strPsw_)
            : this(strAddr_, 21, strUser_, strPsw_)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strAddr_">FTP服务器的地址（IP地址或域名）</param>
        /// <param name="nPort_">FTP服务器端口</param>
        /// <param name="strUser_">登录账号名（如果是匿名登录，可为空）</param>
        /// <param name="strPsw_">登录口令</param>
        public FtpClient(string strAddr_, int nPort_, string strUser_, string strPsw_)
        {
            if (strAddr_.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase))
                _uriFtp = new Uri(string.Format("{0}:{1}", strAddr_, nPort_));
            else
                _uriFtp = new Uri(string.Format("ftp://{0}:{1}", strAddr_, nPort_));

            _strFtpUser = strUser_;
            _strFtpPsw = strPsw_;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ftpConfig_"></param>
        public FtpClient(XFtpConfig ftpConfig_)
            :this(ftpConfig_.Address, ftpConfig_.Port, ftpConfig_.UserName, ftpConfig_.Password)
        {
            if (!string.IsNullOrEmpty(ftpConfig_.RootPath))
            {                
                SetCurDir(ftpConfig_.RootPath);
            }
        }

        /// <summary>
        /// 测试能否连接，如果出错抛出异常
        /// </summary>
        public void ConnectTest()
        {
            IsFolderExists("/");
        }

        /// <summary>
        /// 能否连接：成功返回true；否则false
        /// </summary>
        /// <returns></returns>
        public bool CanConnect()
        {
            try 
            {
                IsFolderExists("/");
                return true;
            }
            catch{}

            return false;
        }

        private string GetUserName(string strUser_)
        {
            return (string.IsNullOrEmpty(strUser_) ? "anonymous" : strUser_);
        }

        private FtpWebRequest Connect(string strPath_)
        {
            FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(_uriFtp, strPath_));
            if(!string.IsNullOrEmpty(_strFtpUser))
                ftpRequest.Credentials = new NetworkCredential(_strFtpUser, _strFtpPsw);
            ftpRequest.KeepAlive = false;

            return ftpRequest;
        }

        private string GetResponseString(FtpWebRequest ftpRequest_)
        {
            //Get the result, streaming to a string
            string strRet = string.Empty;
            using (FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest_.GetResponse())
            {
                using (StreamReader srReponse = new StreamReader(ftpResponse.GetResponseStream(), Encoding.Default))
                {
                    strRet = srReponse.ReadToEnd();

                    srReponse.Close();
                }

                ftpResponse.Close();
            }

            return strRet;
        }

        #region "Path deal"
        private string _strCurDirectory = "/";
        /// <summary>
        /// 获取当前路径
        /// </summary>
        public string CurrentDir
        {
            get
            {
                return _strCurDirectory;
            }
        }

        /// <summary>
        /// 判断strParent_是strSub_的父目录，或相同
        /// </summary>
        /// <param name="strParent_"></param>
        /// <param name="strSub_"></param>
        /// <returns></returns>
        private bool IsParent(string strParent_, string strSub_)
        {
            if (strParent_.Length > strSub_.Length)
                return false;

            if( strParent_.Length == strSub_.Length )
            {
                return string.Equals(strParent_, strSub_, StringComparison.OrdinalIgnoreCase);
            }

            char chSeparator = strSub_[strParent_.Length];
            if ((chSeparator != '\\') && (chSeparator != '/'))
                return false;

            return strSub_.StartsWith(strParent_, StringComparison.OrdinalIgnoreCase);
        }

        private string AdjustDir(string strPath_)
        {
            return strPath_.Replace('\\', '/');
        }

        /// <summary>
        /// 设定当前路径（如果以'/'或'\'开始，则为绝对路径；否则为相对路径）；
        /// 重试时，会自动创建对应目录（如果创建失败，会抛出异常，并且不会重设当前路径；
        /// 创建成功后，会重设当前路径为新的路径）
        /// </summary>
        /// <param name="strPath_">要设定的当前路径</param>
        public void SetCurDir(string strPath_)
        {
            if (string.IsNullOrEmpty(strPath_))
                throw new ArgumentException("Invalid path");

            CreateFolder(strPath_);
            _strCurDirectory = AdjustDir(Path.Combine(_strCurDirectory, strPath_));
        }

        private string GetFullPath(string strFile_)
        {
            if (string.IsNullOrEmpty(strFile_))
                return _strCurDirectory;

            // strFile如果是绝对路径（以'\/'开始），自动返回自身；
            // 否则，返回当前路径+strFile作为完整路径
            return Path.Combine(_strCurDirectory, strFile_);
        }
        #endregion

        #region "Callback Functions info"
        /// <summary>
        /// 回调状态
        /// </summary>
        public enum CallbackStatus
        {
            /// <summary>
            /// 无效
            /// </summary>
            Invalid = 0,
            /// <summary>
            /// 开始
            /// </summary>
            Begin,
            /// <summary>
            /// 前进一步
            /// </summary>
            Step,
            /// <summary>
            /// 完成（结束）
            /// </summary>
            End
        };

        /// <summary>
        /// 回调函数委托：如果取消当前操作，则返回false（对应的函数会抛出OperationCanceledException异常）
        /// </summary>
        /// <param name="euStatus_">当前状态</param>
        /// <param name="nCurStep_">当前进度</param>
        /// <param name="nTotalStep_">总的进度</param>
        /// <returns>如果取消当前操作，则返回false；否则，返回true</returns>
        public delegate bool CallbackFun(CallbackStatus euStatus_, int nCurStep_, int nTotalStep_);

        /// <summary>
        /// 设定回调函数
        /// </summary>
        /// <param name="funCallback_">要设定的回调函数</param>
        public void SetCallback(CallbackFun funCallback_)
        {
            _funCallback = funCallback_;
        }

        int _nCurSteps = 0;
        int _nTotalSteps = 0;
        CallbackFun _funCallback = null;
        private void ProcBegin(int nTotalStep_)
        {
            if (_funCallback != null)
            {
                _nCurSteps = 0;
                _nTotalSteps = nTotalStep_;
                if (!_funCallback(CallbackStatus.Begin, _nCurSteps, _nTotalSteps))
                    throw new OperationCanceledException("User cancelled");
            }
        }

        private void ProcStep()
        {
            if( _funCallback != null )
            {
                ++_nCurSteps;
                if (!_funCallback(CallbackStatus.Step, _nCurSteps, _nTotalSteps))
                    throw new OperationCanceledException("User cancelled");
            }
        }

        private void ProcEnd()
        {
            if( _funCallback != null )
            {
                _funCallback(CallbackStatus.End, _nTotalSteps, _nTotalSteps);
            }
        }
        #endregion

        /// <summary>
        /// 获取指定路径下文件与文件名列表
        /// </summary>
        /// <param name="strPath_">路径</param>
        /// <returns>获取的文件名列表</returns>
        public List<string> ListDir(string strPath_)
        {
            FtpWebRequest ftpRequest = Connect(GetFullPath(strPath_));
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            string strReponse = GetResponseString(ftpRequest);
            //replace CRLF to CR, remove last instance
            strReponse = strReponse.Replace("\r\n", "\r").TrimEnd('\r');

            string[] strAllFile = strReponse.Split('\r');
            List<string> lstFiles = new List<string>(strAllFile.Length);
            foreach(string strFile in strAllFile)
            {
                if (!string.IsNullOrEmpty(strFile))
                    lstFiles.Add(strFile);
            }

            return lstFiles;
        }

        /// <summary>
        /// 获取指定路径下文件详细信息
        /// </summary>
        /// <param name="strPath_">路径</param>
        /// <returns>文件的详细信息列表</returns>
        public DetailList ListDetail(string strPath_)
        {
            string strFullPath = GetFullPath(strPath_);
            FtpWebRequest ftpRequest = Connect(strFullPath);
            ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            string strDetails = GetResponseString(ftpRequest);
            strDetails = strDetails.Replace("\r\n", "\r").TrimEnd('\r');

            return new DetailList(strDetails, strFullPath);
        }


        const int Data_BufferSize = 1024;
        #region "Upload: File transfer TO ftp server"
        
        /// <summary>
        /// 上传本地文件到FTP服务器上：
        /// 如果不允许覆盖，且Ftp上同名文件已存在，则抛出IOException异常
        /// </summary>
        /// <param name="strLocalFile">本地文件（全路径）</param>
        /// <param name="strFtpFile_">目标（FTP服务器上）文件，如果未设定则以本地文件名作为目标文件名</param>
        /// <param name="bOverWrite_">如果文件已存在，是否覆盖</param>
        public void Upload(string strLocalFile, string strFtpFile_, bool bOverWrite_)
        {
            //copy to FI
            FileInfo fInfo = new FileInfo(strLocalFile);
            Upload(fInfo, strFtpFile_, bOverWrite_);
        }

        /// <summary>
        /// 上传本地文件到FTP服务器上
        /// </summary>
        /// <param name="fInfo">本地文件信息</param>
        /// <param name="strFtpFile_">目标（FTP服务器上）文件，如果未设定则以本地文件名作为目标文件名</param>
        /// <param name="bOverWrite_">如果文件已存在，是否覆盖</param>
        public void Upload(FileInfo fInfo, string strFtpFile_, bool bOverWrite_)
        {            
            // check source
            if (!fInfo.Exists)
            {
                throw (new FileNotFoundException("File " + fInfo.Name + " not found"));
            }

            //copy the dFile specified to target dFile: target dFile can be full strPath or just strFile (uses current dir)
            // check target
            string strTargetFile;
            if (string.IsNullOrEmpty(strFtpFile_))
            {
                //Blank target: use source strFile & current dir
                strTargetFile = Path.Combine(_strCurDirectory, fInfo.Name);
            }
            else
            {
                strTargetFile = GetFullPath(strFtpFile_);
            }

            //////////////////////////////////////////////////////////////////////////
            // Ftp upload
            FtpWebRequest ftpRequest = BuildUploadRequest(strTargetFile, bOverWrite_);
            //Notify FTP of the expected size
            ftpRequest.ContentLength = fInfo.Length;

            //create byte array to store
            int nReadLen;
            byte[] byContent = new byte[Data_BufferSize];

            ProcBegin((int)(fInfo.Length / Data_BufferSize + 1));
            using (FileStream fs = fInfo.OpenRead())
            {
                //open request to send
                using (Stream rs = ftpRequest.GetRequestStream())
                {
                    do
                    {
                        nReadLen = fs.Read(byContent, 0, byContent.Length);
                        if (nReadLen == 0)
                            break;

                        rs.Write(byContent, 0, nReadLen);
                        ProcStep();
                    } while (true);
                    rs.Close();
                }

                fs.Close();
            }
            ProcEnd();

            ftpRequest = null;
        }

        private FtpWebRequest BuildUploadRequest(string strTargetFile, bool bOverWrite_)
        {
            if (!bOverWrite_)
            {
                if (IsFileExists(strTargetFile))
                    throw new IOException(string.Format("Ftp.Upload: File {0} existed", strTargetFile));
            }

            // Assure the path existed
            string strParentDir = Path.GetDirectoryName(strTargetFile);
            CreateFolder(strParentDir);

            FtpWebRequest ftpRequest = Connect(strTargetFile);

            //Set request to upload a dFile in binary
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile; // : WebRequestMethods.Ftp.UploadFileWithUniqueName;
            ftpRequest.UseBinary = true;
            return ftpRequest;
        }

        /// <summary>
        /// 上传一个数组内容到FTP服务器上，并保存为文件。
        /// </summary>
        /// <param name="byContents_">文件内容</param>
        /// <param name="strFtpFile_">目标（FTP服务器上）文件</param>
        /// <param name="bOverWrite_">如果文件已存在，是否覆盖</param>
        public void Upload(byte[] byContents_, string strFtpFile_, bool bOverWrite_)
        {
            if ((byContents_ == null) || (byContents_.Length == 0))
                throw new ArgumentException("Invalid File contents");

            // check target
            string strTargetFile;
            if (string.IsNullOrEmpty(strFtpFile_))
            {
                throw new ArgumentException("Must set target file");
            }
            else
            {
                strTargetFile = GetFullPath(strFtpFile_);
            }

            //////////////////////////////////////////////////////////////////////////
            // Ftp upload
            FtpWebRequest ftpRequest = BuildUploadRequest(strTargetFile, bOverWrite_);
            //Notify FTP of the expected size
            ftpRequest.ContentLength = byContents_.Length;

            //open request to send
            using (Stream rs = ftpRequest.GetRequestStream())
            {
                rs.Write(byContents_, 0, byContents_.Length);

                rs.Close();
            }

            ftpRequest = null;
        }
        #endregion

        #region "Download: File transfer FROM ftp server"
        
        /// <summary>
        /// 从FTP服务器上下载文件到本地
        /// </summary>
        /// <param name="strFtpFile_">原文件（FTP服务器上）</param>
        /// <param name="strLocalFile">本地文件（全路径）</param>
        /// <param name="bOverWrite">如果本地文件已存在，是否覆盖</param>
        public void Download(string strFtpFile_, string strLocalFile, bool bOverWrite)
        {
            FileInfo fInfo = new FileInfo(strLocalFile);
            this.Download(strFtpFile_, fInfo, bOverWrite);
        }

        /// <summary>
        /// 从FTP服务器上下载文件到本地
        /// </summary>
        /// <param name="dFtpFile_">原文件信息（FTP服务器上）</param>
        /// <param name="strLocalFile">本地文件（全路径）</param>
        /// <param name="bOverWrite">如果本地文件已存在，是否覆盖</param>
        public void Download(FileDetail dFtpFile_, string strLocalFile, bool bOverWrite)
        {
            this.Download(dFtpFile_.FullName, strLocalFile, bOverWrite);
        }

        /// <summary>
        /// 从FTP服务器上下载文件到本地
        /// </summary>
        /// <param name="dFtpFile_">原文件信息（FTP服务器上）</param>
        /// <param name="fiLocal">本地文件信息</param>
        /// <param name="bOverWrite">如果本地文件已存在，是否覆盖</param>
        public void Download(FileDetail dFtpFile_, FileInfo fiLocal, bool bOverWrite)
        {
            this.Download(dFtpFile_.FullName, fiLocal, bOverWrite);
        }

        /// <summary>
        /// 从FTP服务器上下载文件到本地
        /// </summary>
        /// <param name="strFtpFile_">原文件（FTP服务器上）</param>
        /// <param name="fiLocalFile_">本地文件信息</param>
        /// <param name="bOverWrite">如果本地文件已存在，是否覆盖</param>
        public void Download(string strFtpFile_, FileInfo fiLocalFile_, bool bOverWrite)
        {
            //1. check target
            if (fiLocalFile_.Exists && !(bOverWrite))
            {
                throw new IOException(string.Format("Ftp.Download: file {0} existed", fiLocalFile_.FullName));
            }

            //2. check source
            string strSrc;
            if (string.IsNullOrEmpty(strFtpFile_))
            {
                throw (new ArgumentException("File not specified"));
            }
            else
            {
                strSrc = GetFullPath(strFtpFile_);
            }

            long lSize = GetFileSize(strSrc);
            ProcBegin((int)(lSize / Data_BufferSize + 2));

            //3. perform copy
            FtpWebRequest ftpRequest = Connect(strSrc);

            //Set request to download a dFile in binary mode
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            ftpRequest.UseBinary = true;

            //open request and get ftpResponse stream
            ProcStep();
            using (FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse())
            {
                using (Stream rs = ftpResponse.GetResponseStream())
                {
                    //loop to read & write to file
                    using (FileStream fs = fiLocalFile_.Create())
                    {
                        try
                        {
                            byte[] byBuffer = new byte[Data_BufferSize];
                            int nReadLen = 0;
                            do
                            {
                                nReadLen = rs.Read(byBuffer, 0, byBuffer.Length);
                                if (nReadLen == 0)
                                    break;

                                //System.Diagnostics.Trace.WriteLine("FtpClient-=-Download(): Read Length: " + nReadLen.ToString());
                                fs.Write(byBuffer, 0, nReadLen);
                                ProcStep();
                            } while (true);

                            fs.Flush();
                            fs.Close();
                        }
                        catch (Exception)
                        {
                            //catch error and delete dFile only partially downloaded
                            fs.Close();
                            //delete target dFile as it's incomplete
                            fiLocalFile_.Delete();

                            throw;
                        }
                    }

                    rs.Close();
                }

                ftpResponse.Close();
            }
            ProcEnd();

            ftpRequest = null;
        }

        /// <summary>
        /// 从FTP服务器上下载文件到数组（文件必须小于64K）：
        /// 文件超过64K，则抛出NotSupportedException("File too large(Must less than 64K)")异常
        /// </summary>
        /// <param name="strFtpFile_">原文件（FTP服务器上）</param>
        /// <returns>包含文件内容的数组</returns>
        public byte[] Download(string strFtpFile_)
        {
            const int MaxFile_Size = 64*1024;

            //2. check source
            string strSrc;
            if (string.IsNullOrEmpty(strFtpFile_))
            {
                throw (new ArgumentException("File not specified"));
            }
            else
            {
                strSrc = GetFullPath(strFtpFile_);
            }

            long lSize = GetFileSize(strSrc);
            if (lSize > MaxFile_Size)
                throw new NotSupportedException("File too large(Must less than 64K)");

            //3. perform copy
            FtpWebRequest ftpRequest = Connect(strSrc);

            //Set request to download a dFile in binary mode
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            ftpRequest.UseBinary = true;

            byte[] byBuffer = null;
            using (FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse())
            {
                using (Stream rs = ftpResponse.GetResponseStream())
                {
                    //loop to read 
                    int nReadLen = 0;
                    int nReadOffset = 0;
                    int nToRead = (int)lSize;
                    byBuffer = new byte[nToRead];
                    do
                    {
                        nReadLen = rs.Read(byBuffer, nReadOffset, nToRead);
                        if (nReadLen == 0)
                            break;

                        nReadOffset += nReadLen;
                        nToRead -= nReadLen;
                    } while (true);

                    rs.Close();
                }

                ftpResponse.Close();
            }

            ftpRequest = null;
            return byBuffer;
        }
        #endregion

        #region "File deal functions"
        /// <summary>
        /// 判断FTP服务器上指定文件是否存在
        /// </summary>
        /// <param name="strFile">要判断的文件名</param>
        /// <returns>存在，返回true；否则，返回false</returns>
        public bool IsFileExists(string strFile)
        {
            //Try to obtain file size: if we get error msg containing "550"
            //the file does not exist
            try
            {
                GetFileSize(strFile);
                return true;
            }
            catch (Exception ex)
            {
                //only handle expected not-found exception
                if (ex is WebException)
                {
                    //file does not exist/no rights error = 550
                    if (ex.Message.Contains("550"))
                    {
                        return false;
                    }
                }

                throw;
            }
        }

        /// <summary>
        /// 获取FTP服务器上指定文件的大小
        /// </summary>
        /// <param name="strFile">要获取的文件</param>
        /// <returns>文件的大小（字节）</returns>
        public long GetFileSize(string strFile)
        {
            FtpWebRequest ftpRequest = Connect(GetFullPath(strFile));
            //Try to get info on file/dir?
            ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;

            long lSize;
            using (FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse())
            {
                lSize = ftpResponse.ContentLength;
                ftpResponse.Close();
            }

            return lSize;
        }

        /// <summary>
        /// 重命名FTP服务器上指定的文件
        /// </summary>
        /// <param name="strOldName">原文件名</param>
        /// <param name="strNewName">新文件名</param>
        public void RenameFile(string strOldName, string strNewName)
        {
            if (string.IsNullOrEmpty(strNewName))
                throw new ArgumentException("New name is invalid");

            //Does file exist?
            string strSource = GetFullPath(strOldName);
            if (!IsFileExists(strSource))
            {
                throw (new FileNotFoundException("File " + strSource + " not found"));
            }

            //build target name, ensure it does not exist
            string strTarget = GetFullPath(strNewName);
            if (string.Equals(strTarget, strSource, StringComparison.OrdinalIgnoreCase) )
            {
                return;
            }
            else if (IsFileExists(strTarget))
            {
                throw new IOException(string.Format("Ftp.RenameFile: NewName {0} existed",  strTarget));
            }

            FtpWebRequest ftpRequest = Connect(strSource);
            //Set request to delete
            ftpRequest.Method = WebRequestMethods.Ftp.Rename;
            ftpRequest.RenameTo = strTarget;

            //get ftpResponse but ignore it
            GetResponseString(ftpRequest);
        }

        /// <summary>
        /// 删除FTP服务器上的文件
        /// </summary>
        /// <param name="strFile">要删除的文件名</param>
        public void DeleteFile(string strFile)
        {
            FtpWebRequest ftpRequest = Connect(GetFullPath(strFile));
            //Set request to delete
            ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;

            //get ftpResponse but ignore it
            GetResponseString(ftpRequest);
        }
        #endregion

        #region "Folder deal functions"
        private void CreateDir(string strName_)
        {
            try
            {
                // Not existed, create now
                FtpWebRequest ftpRequest = Connect(GetFullPath(strName_));
                //Set request to MkDir
                ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                //get ftpResponse but ignore it
                GetResponseString(ftpRequest);
            }
            catch(Exception ex)
            {
                if( ex is WebException)
                {
                    if (IsFileExists(strName_))
                        throw new IOException(string.Format("Ftp.CreateDir: {0} is a file", GetFullPath(strName_)));
                }

                throw;
            }
        }

        /// <summary>
        /// 判断文件夹是否存在
        /// </summary>
        /// <param name="strFolder_">要判断的文件夹</param>
        /// <returns>存放，返回true；否则，返回false</returns>
        public bool IsFolderExists(string strFolder_)
        {
            if (string.IsNullOrEmpty(strFolder_))
                throw new ArgumentException("Folder can not empty");

            char chEnd = strFolder_[strFolder_.Length-1];
            if( chEnd != '\\' && chEnd != '/' )
                strFolder_ += "/";

            try 
            {
                FtpWebRequest ftpRequest = Connect(GetFullPath(strFolder_));
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                GetResponseString(ftpRequest);

                return true;
            }
            catch (Exception ex)
            {
                //only handle expected not-found exception
                if (ex is WebException)
                {
                    //file does not exist/no rights error = 550
                    if (ex.Message.Contains("550"))
                    {
                        return false;
                    }
                }

                throw;
            }
        }

        /// <summary>
        /// 创建文件夹，可以同时创建多级（如可在当前目录下创建三级目录，
        /// test\test1\test1；也可从根目录创建'\root\root1'）
        /// </summary>
        /// <param name="strFolder_">要创建的目录</param>
        public void CreateFolder(string strFolder_)
        {
            if (IsFolderExists(strFolder_))
                return;

            // Recursive to create each one
            string strParent = Path.GetDirectoryName(strFolder_);
            if (!string.IsNullOrEmpty(strParent ))
                CreateFolder(strParent);

            // Create the folder now
            if ((strFolder_ == "\\") || (strFolder_ == "/"))
                return;

            CreateDir(strFolder_);
        }

        private void RemoveFolder(string strFolder_)
        {
            //perform remove
            FtpWebRequest ftpRequest = Connect(GetFullPath(strFolder_));
            //Set request to RmDir
            ftpRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;

            //get ftpResponse but ignore it
            GetResponseString(ftpRequest);
        }

        private void RemoveChildren(string strFolder_)
        {
            DetailList dirInfo = ListDetail(strFolder_);
            foreach(FileDetail diFolder in dirInfo.GetDirectories())
            {
                RemoveChildren(Path.Combine(strFolder_, diFolder.FileName));
            }

            foreach(FileDetail diFile in dirInfo.GetFiles())
            {
                DeleteFile(Path.Combine(strFolder_, diFile.FileName));
            }

            RemoveFolder(strFolder_);
        }

        /// <summary>
        /// 删除指定的目录：如果删除的目录是当前目录的一部分（strFolder为绝对路径），
        /// 删除后成功后，当前目录会自动设为根目录（"/"）。
        /// </summary>
        /// <param name="strFolder_">要删除的目录</param>
        /// <param name="bForce_">如果文件夹非空，是否强制删除</param>
        public void DeleteFolder(string strFolder_, bool bForce_)
        {
            if (bForce_)
                RemoveChildren(strFolder_);
            else
                RemoveFolder(strFolder_);

            if( (strFolder_[0] == '\\') || (strFolder_[0] == '/') )
            {
                string strPath = AdjustDir(strFolder_);
                if (IsParent(strPath, _strCurDirectory))
                    _strCurDirectory = "/";
            }
        }
        #endregion

        /// <summary>
        /// 表示FTP服务器上文件与文件夹节点信息的列表
        /// </summary>
        /// <remarks>
        /// This class is used to parse the results from a detailed
        /// directory list from FTP. It supports most formats of
        /// </remarks>
        public class FileDetail
        {
            //Stores extended info about FTP dFile
            private long _lSize;
            private string _strFilename;
            private string _strPath;
            private string _strPermission;
            private DateTime _dtFileTime;
            private DetailTypes _euFileType;

            #region "Properties"
            /// <summary>
            /// 全名（包括路径）
            /// </summary>
            public string FullName
            {
                get
                {
                    return Path.Combine(_strPath, _strFilename);
                }
            }

            /// <summary>
            /// 文件名
            /// </summary>
            public string FileName
            {
                get
                {
                    return _strFilename;
                }
            }

            /// <summary>
            /// 所在路径
            /// </summary>
            public string FilePath
            {
                get
                {
                    return _strPath;
                }
            }

            /// <summary>
            /// 类型（文件/文件夹）
            /// </summary>
            public DetailTypes FileType
            {
                get
                {
                    return _euFileType;
                }
            }

            /// <summary>
            /// 文件大小
            /// </summary>
            public long Size
            {
                get
                {
                    return _lSize;
                }
            }

            /// <summary>
            /// 时间
            /// </summary>
            public DateTime FileDateTime
            {
                get
                {
                    return _dtFileTime;
                }
            }

            /// <summary>
            /// 权限
            /// </summary>
            public string Permission
            {
                get
                {
                    return _strPermission;
                }
            }

            /// <summary>
            /// 文件扩展名（包括'.'）
            /// </summary>
            public string FileExt
            {
                get
                {
                    return Path.GetExtension(_strFilename);
                }
            }

            /// <summary>
            /// 文件名（不包括扩展名）
            /// </summary>
            public string NameOnly
            {
                get
                {
                    return Path.GetFileNameWithoutExtension(_strFilename);
                }
            }
            #endregion

            /// <summary>
            /// 表示文件/文件类型
            /// </summary>
            public enum DetailTypes
            {
                /// <summary>
                /// 无效
                /// </summary>
                Invalid,
                /// <summary>
                /// 文件
                /// </summary>
                File,
                /// <summary>
                /// 文件夹
                /// </summary>
                Folder
            }

            /// <summary>
            /// 节点的构造函数
            /// </summary>
            /// <param name="strLine">从FTP服务器上获取到的一行信息</param>
            /// <param name="strPath">所在路径</param>
            public FileDetail(string strLine, string strPath)
            {
                //parse strLine
                Match m = GetMatchingRegex(strLine);
                if (m == null)
                {
                    //failed
                    throw (new InvalidDataException("Unable to parse line: " + strLine));
                }
                else
                {
                    _strFilename = m.Groups["name"].Value;
                    _strPath = strPath;

                    Int64.TryParse(m.Groups["size"].Value, out _lSize);
                    //_size = System.Convert.ToInt32(m.Groups["size"].Value);

                    _strPermission = m.Groups["permission"].Value;
                    string strDir = m.Groups["strDir"].Value;
                    if (strDir != string.Empty && strDir != "-")
                    {
                        _euFileType = DetailTypes.Folder;
                    }
                    else
                    {
                        _euFileType = DetailTypes.File;
                    }

                    try
                    {
                        _dtFileTime = DateTime.Parse(m.Groups["timestamp"].Value);
                    }
                    catch (Exception)
                    {
                        _dtFileTime = Convert.ToDateTime(null);
                    }

                }
            }

            private Match GetMatchingRegex(string strLine)
            {
                Regex rx;
                Match m;
                for (int i = 0; i <= _ParseFormats.Length - 1; i++)
                {
                    rx = new Regex(_ParseFormats[i]);
                    m = rx.Match(strLine);
                    if (m.Success)
                    {
                        return m;
                    }
                }
                return null;
            }

            #region "Regular expressions for parsing LIST results"
            /// <summary>
            /// List of REGEX formats for different FTP server listing formats
            /// </summary>
            /// <remarks>
            /// The first three are various UNIX/LINUX formats, fourth is for MS FTP
            /// in detailed mode and the last for MS FTP in 'DOS' mode.
            /// </remarks>
            private static string[] _ParseFormats = new string[] { 
            "(?<strDir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})\\s+\\d+\\s+\\w+\\s+\\w+\\s+(?<size>\\d+)\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{4})\\s+(?<name>.+)", 
            "(?<strDir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})\\s+\\d+\\s+\\d+\\s+(?<size>\\d+)\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{4})\\s+(?<name>.+)", 
            "(?<strDir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})\\s+\\d+\\s+\\d+\\s+(?<size>\\d+)\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{1,2}:\\d{2})\\s+(?<name>.+)", 
            "(?<strDir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})\\s+\\d+\\s+\\w+\\s+\\w+\\s+(?<size>\\d+)\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{1,2}:\\d{2})\\s+(?<name>.+)", 
            "(?<strDir>[\\-d])(?<permission>([\\-r][\\-w][\\-xs]){3})(\\s+)(?<size>(\\d+))(\\s+)(?<ctbit>(\\w+\\s\\w+))(\\s+)(?<size2>(\\d+))\\s+(?<timestamp>\\w+\\s+\\d+\\s+\\d{2}:\\d{2})\\s+(?<name>.+)", 
            "(?<timestamp>\\d{2}\\-\\d{2}\\-\\d{2}\\s+\\d{2}:\\d{2}[Aa|Pp][mM])\\s+(?<strDir>\\<\\w+\\>){0,1}(?<size>\\d+){0,1}\\s+(?<name>.+)" };
            #endregion
        }

        /// <summary>
        /// 存放接口信息的列表
        /// </summary>
        public class DetailList : List<FileDetail>
        {
            /// <summary>
            /// 默认构造函数
            /// </summary>
            public DetailList()
            {
            }

            /// <summary>
            /// 节点列表构造函数
            /// </summary>
            /// <param name="strDirDetails">目录下节点详细信息</param>
            /// <param name="strPath">所在路径</param>
            public DetailList(string strDirDetails, string strPath)
            {
                string[] strAllLine = strDirDetails.Replace("\n", string.Empty).Split('\r');
                this.Capacity = strAllLine.Length;
                foreach (string strLine in strAllLine)
                {
                    //parse
                    if (strLine != string.Empty)
                    {
                        this.Add(new FileDetail(strLine, strPath));
                    }
                }
            }

            /// <summary>
            /// 获取目录下所有指定文件信息列表
            /// </summary>
            /// <param name="strExt">文件的扩展名(包括'.'，如".exe")</param>
            /// <returns>文件信息列表</returns>
            public DetailList GetFiles(string strExt)
            {
                return this.GetFileOrDir(FileDetail.DetailTypes.File, strExt);
            }

            /// <summary>
            /// 获取目录下所有文件信息列表
            /// </summary>
            /// <returns>文件信息列表</returns>
            public DetailList GetFiles()
            {
                return this.GetFiles(string.Empty);
            }

            /// <summary>
            /// 获取目录下所有文件夹信息
            /// </summary>
            /// <returns>文件夹信息列表</returns>
            public DetailList GetDirectories()
            {
                return this.GetFileOrDir(FileDetail.DetailTypes.Folder, string.Empty);
            }

            //internal: share use function for GetDirectories/Files
            private DetailList GetFileOrDir(FileDetail.DetailTypes euType, string strExt)
            {
                DetailList lstResult = new DetailList();
                foreach (FileDetail dEntry in this)
                {
                    if (dEntry.FileType == euType)
                    {
                        if (strExt == string.Empty)
                        {
                            lstResult.Add(dEntry);
                        }
                        else if (string.Equals(strExt, dEntry.FileExt, StringComparison.OrdinalIgnoreCase))
                        {
                            lstResult.Add(dEntry);
                        }
                    }
                }

                return lstResult;
            }

            /// <summary>
            /// 判断文件/文件夹是否存在
            /// </summary>
            /// <param name="strFileName">文件名</param>
            /// <returns>存在，返回true；否则返回false</returns>
            public bool IsExists(string strFileName)
            {
                foreach (FileDetail dEntry in this)
                {
                    if (string.Equals(dEntry.FileName, strFileName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// 获取指定文件的类型
            /// </summary>
            /// <param name="strName_">文件名</param>
            /// <returns>不存在，返回valid；文件，返回File；文件夹，返回Folder</returns>
            public FileDetail.DetailTypes GetType(string strName_)
            {
                foreach(FileDetail dEntry in this)
                {
                    if (string.Equals(dEntry.FileName, strName_, StringComparison.OrdinalIgnoreCase))
                        return dEntry.FileType;
                }

                return FileDetail.DetailTypes.Invalid;
            }
        }
    }
}
