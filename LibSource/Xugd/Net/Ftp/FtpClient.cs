using System;
using System.Text;
using System.Text.RegularExpressions;

using System.IO;
using System.Net;
using System.Collections.Generic;

namespace SHCre.Xugd.Net.Ftp
{
    /// <summary>
    /// FTP�ͻ��˲����ࡣ 
    /// ���нӿ��д��ݵ��ļ�����·���������'/'��'\'��ʼ����Ϊ����·����
    /// ����Ϊ���·����ʹ��CurrentDir+ָ��·����Ϊȫ·����
    /// �ļ��ϴ�������ʱ����ͨ���趨�ص���������ȡ��ʵ�Ľ�����Ϣ��
    /// </summary>
    public class FtpClient
    {
        Uri _uriFtp;
        string _strFtpUser;
        string _strFtpPsw;

        /// <summary>
        /// ���캯�����˿�Ĭ��Ϊ21��
        /// </summary>
        /// <param name="strAddr_">FTP�������ĵ�ַ��IP��ַ��������</param>
        /// <param name="strUser_">��¼�˺����������������¼����Ϊ�գ�</param>
        /// <param name="strPsw_">��¼����</param>
        public FtpClient(string strAddr_, string strUser_, string strPsw_)
            : this(strAddr_, 21, strUser_, strPsw_)
        {
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="strAddr_">FTP�������ĵ�ַ��IP��ַ��������</param>
        /// <param name="nPort_">FTP�������˿�</param>
        /// <param name="strUser_">��¼�˺����������������¼����Ϊ�գ�</param>
        /// <param name="strPsw_">��¼����</param>
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
        /// ���캯��
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
        /// �����ܷ����ӣ���������׳��쳣
        /// </summary>
        public void ConnectTest()
        {
            IsFolderExists("/");
        }

        /// <summary>
        /// �ܷ����ӣ��ɹ�����true������false
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
        /// ��ȡ��ǰ·��
        /// </summary>
        public string CurrentDir
        {
            get
            {
                return _strCurDirectory;
            }
        }

        /// <summary>
        /// �ж�strParent_��strSub_�ĸ�Ŀ¼������ͬ
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
        /// �趨��ǰ·���������'/'��'\'��ʼ����Ϊ����·��������Ϊ���·������
        /// ����ʱ�����Զ�������ӦĿ¼���������ʧ�ܣ����׳��쳣�����Ҳ������赱ǰ·����
        /// �����ɹ��󣬻����赱ǰ·��Ϊ�µ�·����
        /// </summary>
        /// <param name="strPath_">Ҫ�趨�ĵ�ǰ·��</param>
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

            // strFile����Ǿ���·������'\/'��ʼ�����Զ���������
            // ���򣬷��ص�ǰ·��+strFile��Ϊ����·��
            return Path.Combine(_strCurDirectory, strFile_);
        }
        #endregion

        #region "Callback Functions info"
        /// <summary>
        /// �ص�״̬
        /// </summary>
        public enum CallbackStatus
        {
            /// <summary>
            /// ��Ч
            /// </summary>
            Invalid = 0,
            /// <summary>
            /// ��ʼ
            /// </summary>
            Begin,
            /// <summary>
            /// ǰ��һ��
            /// </summary>
            Step,
            /// <summary>
            /// ��ɣ�������
            /// </summary>
            End
        };

        /// <summary>
        /// �ص�����ί�У����ȡ����ǰ�������򷵻�false����Ӧ�ĺ������׳�OperationCanceledException�쳣��
        /// </summary>
        /// <param name="euStatus_">��ǰ״̬</param>
        /// <param name="nCurStep_">��ǰ����</param>
        /// <param name="nTotalStep_">�ܵĽ���</param>
        /// <returns>���ȡ����ǰ�������򷵻�false�����򣬷���true</returns>
        public delegate bool CallbackFun(CallbackStatus euStatus_, int nCurStep_, int nTotalStep_);

        /// <summary>
        /// �趨�ص�����
        /// </summary>
        /// <param name="funCallback_">Ҫ�趨�Ļص�����</param>
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
        /// ��ȡָ��·�����ļ����ļ����б�
        /// </summary>
        /// <param name="strPath_">·��</param>
        /// <returns>��ȡ���ļ����б�</returns>
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
        /// ��ȡָ��·�����ļ���ϸ��Ϣ
        /// </summary>
        /// <param name="strPath_">·��</param>
        /// <returns>�ļ�����ϸ��Ϣ�б�</returns>
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
        /// �ϴ������ļ���FTP�������ϣ�
        /// ����������ǣ���Ftp��ͬ���ļ��Ѵ��ڣ����׳�IOException�쳣
        /// </summary>
        /// <param name="strLocalFile">�����ļ���ȫ·����</param>
        /// <param name="strFtpFile_">Ŀ�꣨FTP�������ϣ��ļ������δ�趨���Ա����ļ�����ΪĿ���ļ���</param>
        /// <param name="bOverWrite_">����ļ��Ѵ��ڣ��Ƿ񸲸�</param>
        public void Upload(string strLocalFile, string strFtpFile_, bool bOverWrite_)
        {
            //copy to FI
            FileInfo fInfo = new FileInfo(strLocalFile);
            Upload(fInfo, strFtpFile_, bOverWrite_);
        }

        /// <summary>
        /// �ϴ������ļ���FTP��������
        /// </summary>
        /// <param name="fInfo">�����ļ���Ϣ</param>
        /// <param name="strFtpFile_">Ŀ�꣨FTP�������ϣ��ļ������δ�趨���Ա����ļ�����ΪĿ���ļ���</param>
        /// <param name="bOverWrite_">����ļ��Ѵ��ڣ��Ƿ񸲸�</param>
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
        /// �ϴ�һ���������ݵ�FTP�������ϣ�������Ϊ�ļ���
        /// </summary>
        /// <param name="byContents_">�ļ�����</param>
        /// <param name="strFtpFile_">Ŀ�꣨FTP�������ϣ��ļ�</param>
        /// <param name="bOverWrite_">����ļ��Ѵ��ڣ��Ƿ񸲸�</param>
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
        /// ��FTP�������������ļ�������
        /// </summary>
        /// <param name="strFtpFile_">ԭ�ļ���FTP�������ϣ�</param>
        /// <param name="strLocalFile">�����ļ���ȫ·����</param>
        /// <param name="bOverWrite">��������ļ��Ѵ��ڣ��Ƿ񸲸�</param>
        public void Download(string strFtpFile_, string strLocalFile, bool bOverWrite)
        {
            FileInfo fInfo = new FileInfo(strLocalFile);
            this.Download(strFtpFile_, fInfo, bOverWrite);
        }

        /// <summary>
        /// ��FTP�������������ļ�������
        /// </summary>
        /// <param name="dFtpFile_">ԭ�ļ���Ϣ��FTP�������ϣ�</param>
        /// <param name="strLocalFile">�����ļ���ȫ·����</param>
        /// <param name="bOverWrite">��������ļ��Ѵ��ڣ��Ƿ񸲸�</param>
        public void Download(FileDetail dFtpFile_, string strLocalFile, bool bOverWrite)
        {
            this.Download(dFtpFile_.FullName, strLocalFile, bOverWrite);
        }

        /// <summary>
        /// ��FTP�������������ļ�������
        /// </summary>
        /// <param name="dFtpFile_">ԭ�ļ���Ϣ��FTP�������ϣ�</param>
        /// <param name="fiLocal">�����ļ���Ϣ</param>
        /// <param name="bOverWrite">��������ļ��Ѵ��ڣ��Ƿ񸲸�</param>
        public void Download(FileDetail dFtpFile_, FileInfo fiLocal, bool bOverWrite)
        {
            this.Download(dFtpFile_.FullName, fiLocal, bOverWrite);
        }

        /// <summary>
        /// ��FTP�������������ļ�������
        /// </summary>
        /// <param name="strFtpFile_">ԭ�ļ���FTP�������ϣ�</param>
        /// <param name="fiLocalFile_">�����ļ���Ϣ</param>
        /// <param name="bOverWrite">��������ļ��Ѵ��ڣ��Ƿ񸲸�</param>
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
        /// ��FTP�������������ļ������飨�ļ�����С��64K����
        /// �ļ�����64K�����׳�NotSupportedException("File too large(Must less than 64K)")�쳣
        /// </summary>
        /// <param name="strFtpFile_">ԭ�ļ���FTP�������ϣ�</param>
        /// <returns>�����ļ����ݵ�����</returns>
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
        /// �ж�FTP��������ָ���ļ��Ƿ����
        /// </summary>
        /// <param name="strFile">Ҫ�жϵ��ļ���</param>
        /// <returns>���ڣ�����true�����򣬷���false</returns>
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
        /// ��ȡFTP��������ָ���ļ��Ĵ�С
        /// </summary>
        /// <param name="strFile">Ҫ��ȡ���ļ�</param>
        /// <returns>�ļ��Ĵ�С���ֽڣ�</returns>
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
        /// ������FTP��������ָ�����ļ�
        /// </summary>
        /// <param name="strOldName">ԭ�ļ���</param>
        /// <param name="strNewName">���ļ���</param>
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
        /// ɾ��FTP�������ϵ��ļ�
        /// </summary>
        /// <param name="strFile">Ҫɾ�����ļ���</param>
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
        /// �ж��ļ����Ƿ����
        /// </summary>
        /// <param name="strFolder_">Ҫ�жϵ��ļ���</param>
        /// <returns>��ţ�����true�����򣬷���false</returns>
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
        /// �����ļ��У�����ͬʱ�����༶������ڵ�ǰĿ¼�´�������Ŀ¼��
        /// test\test1\test1��Ҳ�ɴӸ�Ŀ¼����'\root\root1'��
        /// </summary>
        /// <param name="strFolder_">Ҫ������Ŀ¼</param>
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
        /// ɾ��ָ����Ŀ¼�����ɾ����Ŀ¼�ǵ�ǰĿ¼��һ���֣�strFolderΪ����·������
        /// ɾ����ɹ��󣬵�ǰĿ¼���Զ���Ϊ��Ŀ¼��"/"����
        /// </summary>
        /// <param name="strFolder_">Ҫɾ����Ŀ¼</param>
        /// <param name="bForce_">����ļ��зǿգ��Ƿ�ǿ��ɾ��</param>
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
        /// ��ʾFTP���������ļ����ļ��нڵ���Ϣ���б�
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
            /// ȫ��������·����
            /// </summary>
            public string FullName
            {
                get
                {
                    return Path.Combine(_strPath, _strFilename);
                }
            }

            /// <summary>
            /// �ļ���
            /// </summary>
            public string FileName
            {
                get
                {
                    return _strFilename;
                }
            }

            /// <summary>
            /// ����·��
            /// </summary>
            public string FilePath
            {
                get
                {
                    return _strPath;
                }
            }

            /// <summary>
            /// ���ͣ��ļ�/�ļ��У�
            /// </summary>
            public DetailTypes FileType
            {
                get
                {
                    return _euFileType;
                }
            }

            /// <summary>
            /// �ļ���С
            /// </summary>
            public long Size
            {
                get
                {
                    return _lSize;
                }
            }

            /// <summary>
            /// ʱ��
            /// </summary>
            public DateTime FileDateTime
            {
                get
                {
                    return _dtFileTime;
                }
            }

            /// <summary>
            /// Ȩ��
            /// </summary>
            public string Permission
            {
                get
                {
                    return _strPermission;
                }
            }

            /// <summary>
            /// �ļ���չ��������'.'��
            /// </summary>
            public string FileExt
            {
                get
                {
                    return Path.GetExtension(_strFilename);
                }
            }

            /// <summary>
            /// �ļ�������������չ����
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
            /// ��ʾ�ļ�/�ļ�����
            /// </summary>
            public enum DetailTypes
            {
                /// <summary>
                /// ��Ч
                /// </summary>
                Invalid,
                /// <summary>
                /// �ļ�
                /// </summary>
                File,
                /// <summary>
                /// �ļ���
                /// </summary>
                Folder
            }

            /// <summary>
            /// �ڵ�Ĺ��캯��
            /// </summary>
            /// <param name="strLine">��FTP�������ϻ�ȡ����һ����Ϣ</param>
            /// <param name="strPath">����·��</param>
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
        /// ��Žӿ���Ϣ���б�
        /// </summary>
        public class DetailList : List<FileDetail>
        {
            /// <summary>
            /// Ĭ�Ϲ��캯��
            /// </summary>
            public DetailList()
            {
            }

            /// <summary>
            /// �ڵ��б��캯��
            /// </summary>
            /// <param name="strDirDetails">Ŀ¼�½ڵ���ϸ��Ϣ</param>
            /// <param name="strPath">����·��</param>
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
            /// ��ȡĿ¼������ָ���ļ���Ϣ�б�
            /// </summary>
            /// <param name="strExt">�ļ�����չ��(����'.'����".exe")</param>
            /// <returns>�ļ���Ϣ�б�</returns>
            public DetailList GetFiles(string strExt)
            {
                return this.GetFileOrDir(FileDetail.DetailTypes.File, strExt);
            }

            /// <summary>
            /// ��ȡĿ¼�������ļ���Ϣ�б�
            /// </summary>
            /// <returns>�ļ���Ϣ�б�</returns>
            public DetailList GetFiles()
            {
                return this.GetFiles(string.Empty);
            }

            /// <summary>
            /// ��ȡĿ¼�������ļ�����Ϣ
            /// </summary>
            /// <returns>�ļ�����Ϣ�б�</returns>
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
            /// �ж��ļ�/�ļ����Ƿ����
            /// </summary>
            /// <param name="strFileName">�ļ���</param>
            /// <returns>���ڣ�����true�����򷵻�false</returns>
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
            /// ��ȡָ���ļ�������
            /// </summary>
            /// <param name="strName_">�ļ���</param>
            /// <returns>�����ڣ�����valid���ļ�������File���ļ��У�����Folder</returns>
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
