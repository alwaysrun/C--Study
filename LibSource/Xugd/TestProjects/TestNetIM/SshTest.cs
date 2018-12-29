using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SHCre.Xugd.Ssh;
using System.IO;
using SHCre.Xugd.CFile;
using System.Xml.Serialization;
using SHCre.Xugd.Config;
using SHCre.Xugd.Common;
using System.Threading;

namespace TestForm
{

    public partial class SshTest : Form
    {
        bool _bIsTransferring = false;
        Xsftp _xFtp = null;
        TestConfig _conTest = null;
        public SshTest()
        {
            InitializeComponent();
        }

        private void SshTest_Load(object sender, EventArgs e)
        {
            _conTest = TestConfig.Read();
            var ssh = _conTest.SFtp;
            this.txtIp.Text = ssh.Address;
            this.txtName.Text = ssh.User;
            this.txtPsw.Text = ssh.Psw;

            var p = _conTest.Paths;
            this.txtLocal.Text = p.LocalPath;
            this.txtRemote.Text = p.RemotePath;
        }

        void AddInfo(string strFormat_, params object[] oParam_)
        {
            var strInfo = string.Format(strFormat_, oParam_); 
            Add2List(strInfo);
        }

        private void Add2List(string strInfo)
        {
            if(this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(Add2List), strInfo);
                return;
            }

            this.lstInfo.Items.Add(strInfo);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            this.lstInfo.Items.Clear();
            if(_xFtp!=null && _xFtp.Connected )
            {
                AddInfo("Has connected");
                return;
            }
            
            // To connect
            TryConnectSSH();
        }

        private void TryConnectSSH()
        {
            if (_xFtp != null && _xFtp.Connected)
                return;

            int nPort = 22;
            int.TryParse(this.txtPort.Text, out nPort);
            var sftp = _conTest.SFtp;
            sftp.Address = this.txtIp.Text;
            sftp.User = this.txtName.Text;
            sftp.Psw = this.txtPsw.Text;
            sftp.Port = nPort;
            _xFtp = new Xsftp(sftp);
            _xFtp.OnFileTransfer += new Action<string, string, Xsftp.TransferMode, int, int>(_xFtp_OnFileTransfer);

            _bIsTransferring = false;
            try
            {
                _xFtp.Connect();
                AddInfo("Connect OK");
                _conTest.Write();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void UpdateTransfered(int nTrans_)
        {
            if(this.InvokeRequired)
            {
                this.BeginInvoke(new Action<int>(UpdateTransfered), nTrans_);
                return;
            }

            this.lblTransfered.Text = nTrans_.ToString();
        }

        void UpdateTotal(int nTotal_)
        {
            if(this.InvokeRequired)
            {
                this.BeginInvoke(new Action<int>(UpdateTotal), nTotal_);
                return;
            }

            this.lblTotal.Text = nTotal_.ToString();
            this.lblTransfered.Text = "0";
        }

        void _xFtp_OnFileTransfer(string strSrc_, string arg2, Xsftp.TransferMode euMode_, int nTrans_, int nTotal_)
        {
            if(euMode_ == Xsftp.TransferMode.Start)
            {
                _bIsTransferring = true;
                UpdateTotal(nTotal_);
                return;
            }
            
            if(euMode_ == Xsftp.TransferMode.End)
            {
                _bIsTransferring = false;
                AddInfo("Transfer({1}) {0}", strSrc_, (nTrans_ == nTotal_) ? "OK" : "Fail");
            }
            UpdateTransfered(nTrans_);
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if(_bIsTransferring)
            {
                MessageBox.Show("File is transferring");
                return;
            }
            if(string.IsNullOrEmpty(this.txtRemote.Text) || string.IsNullOrEmpty(Path.GetExtension(this.txtRemote.Text)))
            {
                MessageBox.Show("Input Remote file");
                return;
            }     
            if(string.IsNullOrEmpty(this.txtLocal.Text))
            {
                MessageBox.Show("Input local Path");
                return;
            }
            string strPath = (this.txtLocal.Text);
            if (Path.GetExtension(strPath).Length > 0)
                strPath = Path.GetDirectoryName(strPath);
            XPath.CreateFullPath(strPath);

            TryConnectSSH();
            XThread.StartThread(() =>
            {
                try
                {
                    _xFtp.Download(this.txtRemote.Text, strPath);

                    //AddInfo("Download {0} OK", this.txtRemote.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    _bIsTransferring = false;
                }
            });
        }

        private void btnUpLoad_Click(object sender, EventArgs e)
        {
            if (_bIsTransferring)
            {
                MessageBox.Show("File is transferring");
                return;
            }
            if (string.IsNullOrEmpty(this.txtLocal.Text) || string.IsNullOrEmpty(Path.GetExtension(this.txtLocal.Text)))
            {
                MessageBox.Show("Input local file");
                return;
            }
            if (string.IsNullOrEmpty(this.txtRemote.Text))
            {
                MessageBox.Show("Input Remote Path");
                return;
            }     
            string strPath = this.txtRemote.Text;
            //if (strPath.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
            //    strPath = Path.GetDirectoryName(strPath);

            TryConnectSSH();
            XThread.StartThread(() =>
            {
                try
                {
                    _xFtp.Upload(this.txtLocal.Text, strPath);
                    //AddInfo("Upload {0} OK", this.txtLocal.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    _bIsTransferring = false;
                }
            });
        }

        private void btnListFiles_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtRemote.Text))
            {
                MessageBox.Show("Input Remote Path");
                return;
            }

            TryConnectSSH();
            try 
            {
                this.lstInfo.Items.Clear();
                var lstFile = _xFtp.GetFileList(this.txtRemote.Text);
                foreach(var fInfo in lstFile)
                {
                    AddInfo("  {1}, {0}, {2}", fInfo.FileName, fInfo.Permission, XTime.GetFullString(fInfo.MTime));
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void SshTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_xFtp != null)
            {
                if (_bIsTransferring)
                    _xFtp.Cancel();
                _xFtp.Close();
                Thread.Sleep(100);
            }
        }

        private void btnIsDir_Click(object sender, EventArgs e)
        {
            TryConnectSSH();
            bool isDir = _xFtp.IsDir(this.txtRemote.Text);
            AddInfo("IsDir: {0}", isDir);
        }

        private void btnIsFile_Click(object sender, EventArgs e)
        {
            TryConnectSSH();
            bool isFile = _xFtp.IsFile(this.txtRemote.Text);
            AddInfo("IsFile: {0}", isFile);
        }

        private void btnLocal_Click(object sender, EventArgs e)
        {
            if(dlgOpen.ShowDialog() == DialogResult.OK)
            {
                this.txtLocal.Text = dlgOpen.FileName;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_bIsTransferring)
                _xFtp.Cancel();
        }
    }

    public class FileConfig
    {
        [XmlAttribute]
        public string LocalPath { get; set; }
        [XmlAttribute]
        public string RemotePath { get; set; }

        public void Init()
        {
            LocalPath = @"d:\WavFiles";
            RemotePath = @"/home/fsdir/recordings/voices";
        }
    }

    public class TestConfig
    {
        const string DefFile = "CreXsftp.xml";
        public XsftpConfig SFtp { get; set; }
        public FileConfig Paths { get; set; }

        public TestConfig()
        {
            SFtp = new XsftpConfig();
            Paths = new FileConfig();
        }

        public static TestConfig Read()
        {
            var conTest = XConFile.Read<TestConfig>(DefFile);
            if(conTest == null)
            {
                conTest = new TestConfig();
                conTest.SFtp.Init("127.0.0.1");
                conTest.Paths.Init();

                conTest.Write();
            }

            return conTest;
        }

        public void Write()
        {
            XConFile.Write(DefFile, this);
        }
    }
}
