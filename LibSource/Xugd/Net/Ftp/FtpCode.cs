using System;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Sockets;

namespace SHCre.Xugd.Net.Ftp
{
    /// <summary>
    /// Ftp客户端接口类
    /// </summary>
    class FtpCode
    {
        #region 构造函数
        /// <summary>
        /// 缺省构造函数
        /// </summary>
        public FtpCode()
        {
            _strFtpAddress = string.Empty;
            _strFtpPath = string.Empty;
            _strFtpUser = string.Empty;
            _strFtpPsw = string.Empty;
            _nFtpPort = 21;
            _bFtpConnected = false;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strAddr_">FTP服务器IP地址</param>
        /// <param name="strPath_">当前服务器目录</param>
        /// <param name="strUser_">登录用户账号</param>
        /// <param name="strPsw_">登录用户密码</param>
        /// <param name="nPort_">FTP服务器端口</param>
        public FtpCode(string strAddr_, string strPath_, string strUser_, string strPsw_, int nPort_)
        {
            _strFtpAddress = strAddr_;
            _strFtpPath = strPath_;
            _strFtpUser = strUser_;
            _strFtpPsw = strPsw_;
            _nFtpPort = nPort_;
            Connect();
        }
        #endregion

        #region 登录字段、属性

        private string _strFtpAddress;
        /// <summary>
        /// FTP服务器IP地址
        /// </summary>
        public string FtpAddress
        {
            get
            {
                return _strFtpAddress;
            }
            set
            {
                _strFtpAddress = value;
            }
        }

        private int _nFtpPort;
        /// <summary>
        /// FTP服务器端口
        /// </summary>
        public int FtpPort
        {
            get
            {
                return _nFtpPort;
            }
            set
            {
                _nFtpPort = value;
            }
        }

        private string _strFtpPath;
        /// <summary>
        /// 当前服务器目录
        /// </summary>
        public string FtpPath
        {
            get
            {
                return _strFtpPath;
            }
            set
            {
                _strFtpPath = value;
            }
        }

        private string _strFtpUser;
        /// <summary>
        /// 登录用户账号
        /// </summary>
        public string FtpUser
        {
            set
            {
                _strFtpUser = value;
            }
        }

        private string _strFtpPsw;
        /// <summary>
        /// 用户登录密码
        /// </summary>
        public string FtpPassword
        {
            set
            {
                _strFtpPsw = value;
            }
        }

        private bool _bFtpConnected;
        /// <summary>
        /// 是否登录
        /// </summary>
        public bool Connected
        {
            get
            {
                return _bFtpConnected;
            }
        }
        #endregion

        #region 链接
        /// <summary>
        /// 建立连接 
        /// </summary>
        public void Connect()
        {
            _sockControl = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(FtpAddress), _nFtpPort);
            // 链接
            _sockControl.Connect(ipEnd);

            // 获取应答码
            ReadReply();
            if (_nReplyCode != 220)
            {
                DisConnect();
                throw new IOException(_strReply.Substring(4));
            }

            // 登录
            SendCommand("USER " + _strFtpUser);
            if (!(_nReplyCode == 331 || _nReplyCode == 230))
            {
                CloseSocketConnect();//关闭连接
                throw new IOException(_strReply.Substring(4));
            }
            if (_nReplyCode != 230)
            {
                SendCommand("PASS " + _strFtpPsw);
                if (!(_nReplyCode == 230 || _nReplyCode == 202))
                {
                    CloseSocketConnect();//关闭连接
                    throw new IOException(_strReply.Substring(4));
                }
            }
            _bFtpConnected = true;

            // 切换到初始目录
            if (!string.IsNullOrEmpty(_strFtpPath))
            {
                ChDir(_strFtpPath);
            }
        }


        /// <summary>
        /// 关闭连接
        /// </summary>
        public void DisConnect()
        {
            if (_sockControl != null)
            {
                SendCommand("QUIT");
            }
            CloseSocketConnect();
        }

        #endregion

        #region 传输模式

        /// <summary>
        /// 传输模式:二进制类型、ASCII类型
        /// </summary>
        public enum TransferType
        {
            /// <summary>
            /// 二进制类型
            /// </summary>
            Binary,
            /// <summary>
            /// ASCII类型
            /// </summary>
            ASCII
        };

        /// <summary>
        /// 设置传输模式
        /// </summary>
        /// <param name="ttType">传输模式</param>
        public void SetTransferType(TransferType ttType)
        {
            if (ttType == TransferType.Binary)
            {
                SendCommand("TYPE I");//binary类型传输
            }
            else
            {
                SendCommand("TYPE A");//ASCII类型传输
            }
            if (_nReplyCode != 200)
            {
                throw new IOException(_strReply.Substring(4));
            }
            else
            {
                _euType = ttType;
            }
        }


        /// <summary>
        /// 获得传输模式
        /// </summary>
        /// <returns>传输模式</returns>
        public TransferType GetTransferType()
        {
            return _euType;
        }

        #endregion

        #region 文件操作
        /// <summary>
        /// 获得文件列表
        /// </summary>
        /// <param name="strMask">文件名的匹配字符串</param>
        /// <returns></returns>
        public string[] Dir(string strMask)
        {
            // 建立链接
            if (!_bFtpConnected)
            {
                Connect();
            }

            //建立进行数据连接的socket
            Socket socketData = CreateDataSocket();

            //传送命令
            SendCommand("LIST " + strMask);

            //分析应答代码
            if (!(_nReplyCode == 150 || _nReplyCode == 125 || _nReplyCode == 226))
            {
                throw new IOException(_strReply.Substring(4));
            }

            //获得结果
            _strReplyMsg = string.Empty;
            while (true)
            {
                int iBytes = socketData.Receive(byBuffer, byBuffer.Length, 0);
                _strReplyMsg += GB2312.GetString(byBuffer, 0, iBytes);
                if (iBytes < byBuffer.Length)
                {
                    break;
                }
            }
            char[] seperator = { '\n' };
            string[] strsFileList = _strReplyMsg.Split(seperator);
            socketData.Close();//数据socket关闭时也会有返回码
            if (_nReplyCode != 226)
            {
                ReadReply();
                if (_nReplyCode != 226)
                {
                    throw new IOException(_strReply.Substring(4));
                }
            }
            return strsFileList;
        }


        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="strFileName">文件名</param>
        /// <returns>文件大小</returns>
        public long GetFileSize(string strFileName)
        {
            if (!_bFtpConnected)
            {
                Connect();
            }
            SendCommand("SIZE " + Path.GetFileName(strFileName));
            long lSize = 0;
            if (_nReplyCode == 213)
            {
                lSize = Int64.Parse(_strReply.Substring(4));
            }
            else
            {
                throw new IOException(_strReply.Substring(4));
            }
            return lSize;
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="strFileName">待删除文件名</param>
        public void Delete(string strFileName)
        {
            if (!_bFtpConnected)
            {
                Connect();
            }
            SendCommand("DELE " + strFileName);
            if (_nReplyCode != 250)
            {
                throw new IOException(_strReply.Substring(4));
            }
        }


        /// <summary>
        /// 重命名(如果新文件名与已有文件重名,将覆盖已有文件)
        /// </summary>
        /// <param name="strOldFileName">旧文件名</param>
        /// <param name="strNewFileName">新文件名</param>
        public void Rename(string strOldFileName, string strNewFileName)
        {
            if (!_bFtpConnected)
            {
                Connect();
            }
            SendCommand("RNFR " + strOldFileName);
            if (_nReplyCode != 350)
            {
                throw new IOException(_strReply.Substring(4));
            }
            //  如果新文件名与原有文件重名,将覆盖原有文件
            SendCommand("RNTO " + strNewFileName);
            if (_nReplyCode != 250)
            {
                throw new IOException(_strReply.Substring(4));
            }
        }
        #endregion

        #region 上传和下载
        /// <summary>
        /// 下载一批文件
        /// </summary>
        /// <param name="strFileNameMask">文件名的匹配字符串</param>
        /// <param name="strFolder">本地目录(不得以\结束)</param>
        public void Get(string strFileNameMask, string strFolder)
        {
            if (!_bFtpConnected)
            {
                Connect();
            }
            string[] strFiles = Dir(strFileNameMask);
            foreach (string strFile in strFiles)
            {
                if (!strFile.Equals(string.Empty))//一般来说strFiles的最后一个元素可能是空字符串
                {
                    if (strFile.LastIndexOf(".") > -1)
                    {
                        Get(strFile.Replace("\r", string.Empty), strFolder, strFile.Replace("\r", string.Empty));
                    }
                }
            }
        }


        /// <summary>
        /// 下载目录
        /// </summary>
        /// <param name="strRemoteFileName">要下载的文件名</param>
        /// <param name="strFolder">本地目录(不得以\结束)</param>
        /// <param name="strLocalFileName">保存在本地时的文件名</param>
        public void Get(string strRemoteFileName, string strFolder, string strLocalFileName)
        {
            if (strLocalFileName.StartsWith("-r"))
            {
                string[] infos = strLocalFileName.Split(' ');
                strRemoteFileName = strLocalFileName = infos[infos.Length - 1];

                if (!_bFtpConnected)
                {
                    Connect();
                }
                SetTransferType(TransferType.Binary);
                if (strLocalFileName.Equals(string.Empty))
                {
                    strLocalFileName = strRemoteFileName;
                }
                if (!File.Exists(strLocalFileName))
                {
                    Stream st = File.Create(strLocalFileName);
                    st.Close();
                }

                FileStream output = new
                    FileStream(strFolder + "\\" + strLocalFileName, FileMode.Create);
                Socket socketData = CreateDataSocket();
                SendCommand("RETR " + strRemoteFileName);
                if (!(_nReplyCode == 150 || _nReplyCode == 125
                || _nReplyCode == 226 || _nReplyCode == 250))
                {
                    throw new IOException(_strReply.Substring(4));
                }
                while (true)
                {
                    int iBytes = socketData.Receive(byBuffer, byBuffer.Length, 0);
                    output.Write(byBuffer, 0, iBytes);
                    if (iBytes <= 0)
                    {
                        break;
                    }
                }
                output.Close();
                if (socketData.Connected)
                {
                    socketData.Close();
                }
                if (!(_nReplyCode == 226 || _nReplyCode == 250))
                {
                    ReadReply();
                    if (!(_nReplyCode == 226 || _nReplyCode == 250))
                    {
                        throw new IOException(_strReply.Substring(4));
                    }
                }
            }
        }

        /// <summary>
        /// 下载一个文件
        /// </summary>
        /// <param name="strRemoteFileName">要下载的文件名</param>
        /// <param name="strFolder">本地目录(不得以\结束)</param>
        /// <param name="strLocalFileName">保存在本地时的文件名</param>
        public void GetFile(string strRemoteFileName, string strFolder, string strLocalFileName)
        {
            if (!_bFtpConnected)
            {
                Connect();
            }
            SetTransferType(TransferType.Binary);
            if (strLocalFileName.Equals(string.Empty))
            {
                strLocalFileName = strRemoteFileName;
            }
            if (!File.Exists(strLocalFileName))
            {
                Stream st = File.Create(strLocalFileName);
                st.Close();
            }

            FileStream output = new
                FileStream(strFolder + "\\" + strLocalFileName, FileMode.Create);
            Socket socketData = CreateDataSocket();
            SendCommand("RETR " + strRemoteFileName);
            if (!(_nReplyCode == 150 || _nReplyCode == 125
            || _nReplyCode == 226 || _nReplyCode == 250))
            {
                throw new IOException(_strReply.Substring(4));
            }
            while (true)
            {
                int iBytes = socketData.Receive(byBuffer, byBuffer.Length, 0);
                output.Write(byBuffer, 0, iBytes);
                if (iBytes <= 0)
                {
                    break;
                }
            }
            output.Close();
            if (socketData.Connected)
            {
                socketData.Close();
            }
            if (!(_nReplyCode == 226 || _nReplyCode == 250))
            {
                ReadReply();
                if (!(_nReplyCode == 226 || _nReplyCode == 250))
                {
                    throw new IOException(_strReply.Substring(4));
                }
            }
        }

        /// <summary>
        /// 下载一个文件
        /// </summary>
        /// <param name="strRemoteFileName">要下载的文件名</param>
        /// <param name="strFolder">本地目录(不得以\结束)</param>
        /// <param name="strLocalFileName">保存在本地时的文件名</param>
        /// <param name="size">文件大小</param>
        public void GetBrokenFile(string strRemoteFileName, string strFolder, string strLocalFileName, long size)
        {
            if (!_bFtpConnected)
            {
                Connect();
            }
            SetTransferType(TransferType.Binary);



            FileStream output = new
                FileStream(strFolder + "\\" + strLocalFileName, FileMode.Append);
            Socket socketData = CreateDataSocket();
            SendCommand("REST " + size.ToString());
            SendCommand("RETR " + strRemoteFileName);
            if (!(_nReplyCode == 150 || _nReplyCode == 125
            || _nReplyCode == 226 || _nReplyCode == 250))
            {
                throw new IOException(_strReply.Substring(4));
            }

            //int byteYu = (int)size % 512;
            //int byteChu = (int)size / 512;
            //byte[] tempBuffer = new byte[byteYu];
            //for (int i = 0; i < byteChu; i++)
            //{
            //    socketData.Receive(buffer, buffer.Length, 0);
            //}

            //socketData.Receive(tempBuffer, tempBuffer.Length, 0);

            //socketData.Receive(buffer, byteYu, 0);
            while (true)
            {
                int iBytes = socketData.Receive(byBuffer, byBuffer.Length, 0);
                //totalBytes += iBytes;

                output.Write(byBuffer, 0, iBytes);
                if (iBytes <= 0)
                {
                    break;
                }
            }
            output.Close();
            if (socketData.Connected)
            {
                socketData.Close();
            }
            if (!(_nReplyCode == 226 || _nReplyCode == 250))
            {
                ReadReply();
                if (!(_nReplyCode == 226 || _nReplyCode == 250))
                {
                    throw new IOException(_strReply.Substring(4));
                }
            }
        }



        /// <summary>
        /// 上传一批文件
        /// </summary>
        /// <param name="strFolder">本地目录(不得以\结束)</param>
        /// <param name="strFileNameMask">文件名匹配字符(可以包含*和?)</param>
        public void Put(string strFolder, string strFileNameMask)
        {
            string[] strFiles = Directory.GetFiles(strFolder, strFileNameMask);
            foreach (string strFile in strFiles)
            {
                //strFile是完整的文件名(包含路径)
                Put(strFile);
            }
        }


        /// <summary>
        /// 上传一个文件
        /// </summary>
        /// <param name="strFileName">本地文件名</param>
        public void Put(string strFileName)
        {
            if (!_bFtpConnected)
            {
                Connect();
            }
            Socket socketData = CreateDataSocket();
            SendCommand("STOR " + Path.GetFileName(strFileName));
            if (!(_nReplyCode == 125 || _nReplyCode == 150))
            {
                throw new IOException(_strReply.Substring(4));
            }
            FileStream input = new
            FileStream(strFileName, FileMode.Open);
            int iBytes = 0;
            while ((iBytes = input.Read(byBuffer, 0, byBuffer.Length)) > 0)
            {
                socketData.Send(byBuffer, iBytes, 0);
            }
            input.Close();
            if (socketData.Connected)
            {
                socketData.Close();
            }
            if (!(_nReplyCode == 226 || _nReplyCode == 250))
            {
                ReadReply();
                if (!(_nReplyCode == 226 || _nReplyCode == 250))
                {
                    throw new IOException(_strReply.Substring(4));
                }
            }
        }

        #endregion

        #region 目录操作
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="strDirName">目录名</param>
        public void MkDir(string strDirName)
        {
            if (!_bFtpConnected)
            {
                Connect();
            }
            SendCommand("MKD " + strDirName);
            if (_nReplyCode != 257)
            {
                throw new IOException(_strReply.Substring(4));
            }
        }


        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="strDirName">目录名</param>
        public void RmDir(string strDirName)
        {
            if (!_bFtpConnected)
            {
                Connect();
            }
            SendCommand("RMD " + strDirName);
            if (_nReplyCode != 250)
            {
                throw new IOException(_strReply.Substring(4));
            }
        }


        /// <summary>
        /// 改变目录
        /// </summary>
        /// <param name="strDirName">新的工作目录名</param>
        public void ChDir(string strDirName)
        {
            if (strDirName.Equals(".") || strDirName.Equals(string.Empty))
            {
                return;
            }
            if (!_bFtpConnected)
            {
                Connect();
            }
            SendCommand("CWD " + strDirName);
            if (_nReplyCode != 250)
            {
                throw new IOException(_strReply.Substring(4));
            }
            this._strFtpPath = strDirName;
        }

        #endregion

        #region 内部变量
        /// <summary>
        /// 服务器返回的应答信息(包含应答码)
        /// </summary>
        private string _strReplyMsg;
        /// <summary>
        /// 服务器返回的应答信息(包含应答码)
        /// </summary>
        private string _strReply;
        /// <summary>
        /// 服务器返回的应答码
        /// </summary>
        private int _nReplyCode;
        /// <summary>
        /// 进行控制连接的socket
        /// </summary>
        private Socket _sockControl;
        /// <summary>
        /// 传输模式
        /// </summary>
        private TransferType _euType;
        /// <summary>
        /// 接收和发送数据的缓冲区
        /// </summary>
        private static int BLOCK_SIZE = 512;
        byte[] byBuffer = new byte[BLOCK_SIZE];
        /// <summary>
        /// 编码方式(为防止出现中文乱码采用 GB2312编码方式)
        /// </summary>
        Encoding GB2312 = Encoding.GetEncoding("gb2312");
        #endregion

        #region 内部函数
        /// <summary>
        /// 将一行应答字符串记录在strReply和strMsg
        /// 应答码记录在iReplyCode
        /// </summary>
        private void ReadReply()
        {
            _strReplyMsg = string.Empty;
            _strReply = ReadLine();
            _nReplyCode = Int32.Parse(_strReply.Substring(0, 3));
        }

        /// <summary>
        /// 建立进行数据连接的socket
        /// </summary>
        /// <returns>数据连接socket</returns>
        private Socket CreateDataSocket()
        {
            SendCommand("PASV");
            if (_nReplyCode != 227)
            {
                throw new IOException(_strReply.Substring(4));
            }
            int index1 = _strReply.IndexOf('(');
            int index2 = _strReply.IndexOf(')');
            string ipData =
            _strReply.Substring(index1 + 1, index2 - index1 - 1);
            int[] parts = new int[6];
            int len = ipData.Length;
            int partCount = 0;
            string buf = string.Empty;
            for (int i = 0; i < len && partCount <= 6; i++)
            {
                char ch = Char.Parse(ipData.Substring(i, 1));
                if (Char.IsDigit(ch))
                    buf += ch;
                else if (ch != ',')
                {
                    throw new IOException("Malformed PASV strReply: " +
                    _strReply);
                }
                if (ch == ',' || i + 1 == len)
                {
                    try
                    {
                        parts[partCount++] = Int32.Parse(buf);
                        buf = string.Empty;
                    }
                    catch (Exception)
                    {
                        throw new IOException("Malformed PASV strReply: " +
                         _strReply);
                    }
                }
            }
            string ipAddress = parts[0] + "." + parts[1] + "." +
            parts[2] + "." + parts[3];
            int port = (parts[4] << 8) + parts[5];
            Socket s = new
            Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new
            IPEndPoint(IPAddress.Parse(ipAddress), port);
            try
            {
                s.Connect(ep);
            }
            catch (Exception)
            {
                throw new IOException("Can't connect to remote server");
            }
            return s;
        }


        /// <summary>
        /// 关闭socket连接(用于登录以前)
        /// </summary>
        private void CloseSocketConnect()
        {
            if (_sockControl != null)
            {
                _sockControl.Close();
                _sockControl = null;
            }
            _bFtpConnected = false;
        }

        /// <summary>
        /// 读取Socket返回的所有字符串
        /// </summary>
        /// <returns>包含应答码的字符串行</returns>
        private string ReadLine()
        {
            while (true)
            {
                int iBytes = _sockControl.Receive(byBuffer, byBuffer.Length, 0);
                _strReplyMsg += GB2312.GetString(byBuffer, 0, iBytes);
                if (iBytes < byBuffer.Length)
                {
                    break;
                }
            }

            char[] seperator = { '\n' };
            string[] strMsgs = _strReplyMsg.Split(seperator);
            if ( strMsgs.Length > 2)
            {
                _strReplyMsg = strMsgs[strMsgs.Length - 2];
                //seperator[0]是10,换行符是由13和10组成的,分隔后10后面虽没有字符串,
                //但也会分配为空字符串给后面(也是最后一个)字符串数组,
                //所以最后一个mess是没用的空字符串
                //但为什么不直接取mess[0],因为只有最后一行字符串应答码与信息之间有空格
            }
            else
            {
                _strReplyMsg = strMsgs[0];
            }
            if (!_strReplyMsg.Substring(3, 1).Equals(" "))//返回字符串正确的是以应答码(如220开头,后面接一空格,再接问候字符串)
            {
                return ReadLine();
            }

            return _strReplyMsg;
        }


        /// <summary>
        /// 发送命令并获取应答码和最后一行应答字符串
        /// </summary>
        /// <param name="strCommand">命令</param>
        private void SendCommand(String strCommand)
        {
            byte[] cmdBytes = GB2312.GetBytes(strCommand + "\r\n");
            _sockControl.Send(cmdBytes, cmdBytes.Length, 0);
            ReadReply();
        }
        #endregion
    }
}