using System;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Sockets;

namespace SHCre.Xugd.Net.Ftp
{
    /// <summary>
    /// Ftp�ͻ��˽ӿ���
    /// </summary>
    class FtpCode
    {
        #region ���캯��
        /// <summary>
        /// ȱʡ���캯��
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
        /// ���캯��
        /// </summary>
        /// <param name="strAddr_">FTP������IP��ַ</param>
        /// <param name="strPath_">��ǰ������Ŀ¼</param>
        /// <param name="strUser_">��¼�û��˺�</param>
        /// <param name="strPsw_">��¼�û�����</param>
        /// <param name="nPort_">FTP�������˿�</param>
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

        #region ��¼�ֶΡ�����

        private string _strFtpAddress;
        /// <summary>
        /// FTP������IP��ַ
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
        /// FTP�������˿�
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
        /// ��ǰ������Ŀ¼
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
        /// ��¼�û��˺�
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
        /// �û���¼����
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
        /// �Ƿ��¼
        /// </summary>
        public bool Connected
        {
            get
            {
                return _bFtpConnected;
            }
        }
        #endregion

        #region ����
        /// <summary>
        /// �������� 
        /// </summary>
        public void Connect()
        {
            _sockControl = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(FtpAddress), _nFtpPort);
            // ����
            _sockControl.Connect(ipEnd);

            // ��ȡӦ����
            ReadReply();
            if (_nReplyCode != 220)
            {
                DisConnect();
                throw new IOException(_strReply.Substring(4));
            }

            // ��¼
            SendCommand("USER " + _strFtpUser);
            if (!(_nReplyCode == 331 || _nReplyCode == 230))
            {
                CloseSocketConnect();//�ر�����
                throw new IOException(_strReply.Substring(4));
            }
            if (_nReplyCode != 230)
            {
                SendCommand("PASS " + _strFtpPsw);
                if (!(_nReplyCode == 230 || _nReplyCode == 202))
                {
                    CloseSocketConnect();//�ر�����
                    throw new IOException(_strReply.Substring(4));
                }
            }
            _bFtpConnected = true;

            // �л�����ʼĿ¼
            if (!string.IsNullOrEmpty(_strFtpPath))
            {
                ChDir(_strFtpPath);
            }
        }


        /// <summary>
        /// �ر�����
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

        #region ����ģʽ

        /// <summary>
        /// ����ģʽ:���������͡�ASCII����
        /// </summary>
        public enum TransferType
        {
            /// <summary>
            /// ����������
            /// </summary>
            Binary,
            /// <summary>
            /// ASCII����
            /// </summary>
            ASCII
        };

        /// <summary>
        /// ���ô���ģʽ
        /// </summary>
        /// <param name="ttType">����ģʽ</param>
        public void SetTransferType(TransferType ttType)
        {
            if (ttType == TransferType.Binary)
            {
                SendCommand("TYPE I");//binary���ʹ���
            }
            else
            {
                SendCommand("TYPE A");//ASCII���ʹ���
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
        /// ��ô���ģʽ
        /// </summary>
        /// <returns>����ģʽ</returns>
        public TransferType GetTransferType()
        {
            return _euType;
        }

        #endregion

        #region �ļ�����
        /// <summary>
        /// ����ļ��б�
        /// </summary>
        /// <param name="strMask">�ļ�����ƥ���ַ���</param>
        /// <returns></returns>
        public string[] Dir(string strMask)
        {
            // ��������
            if (!_bFtpConnected)
            {
                Connect();
            }

            //���������������ӵ�socket
            Socket socketData = CreateDataSocket();

            //��������
            SendCommand("LIST " + strMask);

            //����Ӧ�����
            if (!(_nReplyCode == 150 || _nReplyCode == 125 || _nReplyCode == 226))
            {
                throw new IOException(_strReply.Substring(4));
            }

            //��ý��
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
            socketData.Close();//����socket�ر�ʱҲ���з�����
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
        /// ��ȡ�ļ���С
        /// </summary>
        /// <param name="strFileName">�ļ���</param>
        /// <returns>�ļ���С</returns>
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
        /// ɾ��
        /// </summary>
        /// <param name="strFileName">��ɾ���ļ���</param>
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
        /// ������(������ļ����������ļ�����,�����������ļ�)
        /// </summary>
        /// <param name="strOldFileName">���ļ���</param>
        /// <param name="strNewFileName">���ļ���</param>
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
            //  ������ļ�����ԭ���ļ�����,������ԭ���ļ�
            SendCommand("RNTO " + strNewFileName);
            if (_nReplyCode != 250)
            {
                throw new IOException(_strReply.Substring(4));
            }
        }
        #endregion

        #region �ϴ�������
        /// <summary>
        /// ����һ���ļ�
        /// </summary>
        /// <param name="strFileNameMask">�ļ�����ƥ���ַ���</param>
        /// <param name="strFolder">����Ŀ¼(������\����)</param>
        public void Get(string strFileNameMask, string strFolder)
        {
            if (!_bFtpConnected)
            {
                Connect();
            }
            string[] strFiles = Dir(strFileNameMask);
            foreach (string strFile in strFiles)
            {
                if (!strFile.Equals(string.Empty))//һ����˵strFiles�����һ��Ԫ�ؿ����ǿ��ַ���
                {
                    if (strFile.LastIndexOf(".") > -1)
                    {
                        Get(strFile.Replace("\r", string.Empty), strFolder, strFile.Replace("\r", string.Empty));
                    }
                }
            }
        }


        /// <summary>
        /// ����Ŀ¼
        /// </summary>
        /// <param name="strRemoteFileName">Ҫ���ص��ļ���</param>
        /// <param name="strFolder">����Ŀ¼(������\����)</param>
        /// <param name="strLocalFileName">�����ڱ���ʱ���ļ���</param>
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
        /// ����һ���ļ�
        /// </summary>
        /// <param name="strRemoteFileName">Ҫ���ص��ļ���</param>
        /// <param name="strFolder">����Ŀ¼(������\����)</param>
        /// <param name="strLocalFileName">�����ڱ���ʱ���ļ���</param>
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
        /// ����һ���ļ�
        /// </summary>
        /// <param name="strRemoteFileName">Ҫ���ص��ļ���</param>
        /// <param name="strFolder">����Ŀ¼(������\����)</param>
        /// <param name="strLocalFileName">�����ڱ���ʱ���ļ���</param>
        /// <param name="size">�ļ���С</param>
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
        /// �ϴ�һ���ļ�
        /// </summary>
        /// <param name="strFolder">����Ŀ¼(������\����)</param>
        /// <param name="strFileNameMask">�ļ���ƥ���ַ�(���԰���*��?)</param>
        public void Put(string strFolder, string strFileNameMask)
        {
            string[] strFiles = Directory.GetFiles(strFolder, strFileNameMask);
            foreach (string strFile in strFiles)
            {
                //strFile���������ļ���(����·��)
                Put(strFile);
            }
        }


        /// <summary>
        /// �ϴ�һ���ļ�
        /// </summary>
        /// <param name="strFileName">�����ļ���</param>
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

        #region Ŀ¼����
        /// <summary>
        /// ����Ŀ¼
        /// </summary>
        /// <param name="strDirName">Ŀ¼��</param>
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
        /// ɾ��Ŀ¼
        /// </summary>
        /// <param name="strDirName">Ŀ¼��</param>
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
        /// �ı�Ŀ¼
        /// </summary>
        /// <param name="strDirName">�µĹ���Ŀ¼��</param>
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

        #region �ڲ�����
        /// <summary>
        /// ���������ص�Ӧ����Ϣ(����Ӧ����)
        /// </summary>
        private string _strReplyMsg;
        /// <summary>
        /// ���������ص�Ӧ����Ϣ(����Ӧ����)
        /// </summary>
        private string _strReply;
        /// <summary>
        /// ���������ص�Ӧ����
        /// </summary>
        private int _nReplyCode;
        /// <summary>
        /// ���п������ӵ�socket
        /// </summary>
        private Socket _sockControl;
        /// <summary>
        /// ����ģʽ
        /// </summary>
        private TransferType _euType;
        /// <summary>
        /// ���պͷ������ݵĻ�����
        /// </summary>
        private static int BLOCK_SIZE = 512;
        byte[] byBuffer = new byte[BLOCK_SIZE];
        /// <summary>
        /// ���뷽ʽ(Ϊ��ֹ��������������� GB2312���뷽ʽ)
        /// </summary>
        Encoding GB2312 = Encoding.GetEncoding("gb2312");
        #endregion

        #region �ڲ�����
        /// <summary>
        /// ��һ��Ӧ���ַ�����¼��strReply��strMsg
        /// Ӧ�����¼��iReplyCode
        /// </summary>
        private void ReadReply()
        {
            _strReplyMsg = string.Empty;
            _strReply = ReadLine();
            _nReplyCode = Int32.Parse(_strReply.Substring(0, 3));
        }

        /// <summary>
        /// ���������������ӵ�socket
        /// </summary>
        /// <returns>��������socket</returns>
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
        /// �ر�socket����(���ڵ�¼��ǰ)
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
        /// ��ȡSocket���ص������ַ���
        /// </summary>
        /// <returns>����Ӧ������ַ�����</returns>
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
                //seperator[0]��10,���з�����13��10��ɵ�,�ָ���10������û���ַ���,
                //��Ҳ�����Ϊ���ַ���������(Ҳ�����һ��)�ַ�������,
                //�������һ��mess��û�õĿ��ַ���
                //��Ϊʲô��ֱ��ȡmess[0],��Ϊֻ�����һ���ַ���Ӧ��������Ϣ֮���пո�
            }
            else
            {
                _strReplyMsg = strMsgs[0];
            }
            if (!_strReplyMsg.Substring(3, 1).Equals(" "))//�����ַ�����ȷ������Ӧ����(��220��ͷ,�����һ�ո�,�ٽ��ʺ��ַ���)
            {
                return ReadLine();
            }

            return _strReplyMsg;
        }


        /// <summary>
        /// ���������ȡӦ��������һ��Ӧ���ַ���
        /// </summary>
        /// <param name="strCommand">����</param>
        private void SendCommand(String strCommand)
        {
            byte[] cmdBytes = GB2312.GetBytes(strCommand + "\r\n");
            _sockControl.Send(cmdBytes, cmdBytes.Length, 0);
            ReadReply();
        }
        #endregion
    }
}