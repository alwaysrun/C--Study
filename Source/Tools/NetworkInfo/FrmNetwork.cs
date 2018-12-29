using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace NetworkInfo
{
    public partial class FrmNetwork : Form
    {
        public FrmNetwork()
        {
            InitializeComponent();
        }

        private void btnTcpListener_Click(object sender, EventArgs e)
        {
            var ipProperty = IPGlobalProperties.GetIPGlobalProperties();
            var tcpPoints = ipProperty.GetActiveTcpListeners();
            this.lstInfo.Items.Clear();
            this.lstInfo.Clear();
            this.lstInfo.Columns.Add("Tcp Listeners", 360);
            foreach (var tp in tcpPoints)
            {
                this.lstInfo.Items.Add(tp.ToString());
            }
        }

        private void btnUdpListener_Click(object sender, EventArgs e)
        {
            var ipProperty = IPGlobalProperties.GetIPGlobalProperties();
            var udpPoints = ipProperty.GetActiveUdpListeners();
            this.lstInfo.Items.Clear();
            this.lstInfo.Clear();
            this.lstInfo.Columns.Add("Udp Listeners", 360);
            foreach (var tp in udpPoints)
            {
                this.lstInfo.Items.Add(tp.ToString());
            }
        }

        private void btnTcpConnect_Click(object sender, EventArgs e)
        {
            var ipProperty = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnects = ipProperty.GetActiveTcpConnections();
            this.lstInfo.Items.Clear();
            this.lstInfo.Clear();
            this.lstInfo.Columns.Add("Local Address", 200);
            this.lstInfo.Columns.Add("Remote Address", 200);
            this.lstInfo.Columns.Add("State", 120);
            foreach (var tp in tcpConnects)
            {
                var lstItem = new ListViewItem(tp.LocalEndPoint.ToString());
                lstItem.SubItems.Add(tp.RemoteEndPoint.ToString());
                lstItem.SubItems.Add(tp.State.ToString());
                this.lstInfo.Items.Add(lstItem);
            }
        }

        void ResetWinCursor(Cursor cur_)
        {
            if(this.InvokeRequired)
            {
                this.BeginInvoke(new Action<Cursor>(ResetWinCursor), cur_);
                return;
            }

            this.Cursor = cur_;
        }

        void AddListItem(ListViewItem lvItem_)
        {
            if(this.InvokeRequired)
            {
                this.BeginInvoke(new Action<ListViewItem>(AddListItem), lvItem_);
                return;
            }

            this.lstInfo.Items.Add(lvItem_);
        }

        private bool ExecuteCmd(string strCmd_, Action<string> actOutput_)
        {
            Process proCmd = new Process();
            // proCmd.StartInfo.FileName = "cmd.exe \"mode con:cols=10 lines=1\"";
            proCmd.StartInfo.FileName = "cmd.exe";
            proCmd.StartInfo.UseShellExecute = false;
            proCmd.StartInfo.RedirectStandardInput = true;
            proCmd.StartInfo.RedirectStandardOutput = true;
            proCmd.StartInfo.RedirectStandardError = true;
            proCmd.StartInfo.CreateNoWindow = true;
            proCmd.StartInfo.ErrorDialog = false;
            //proCmd.OutputDataReceived += (sender, data) => { actOutput_(data.Data); };
            //proCmd.ErrorDataReceived += (sender, data) => { actOutput_(data.Data); };

            try
            {
                if (!proCmd.Start())
                {
                    return false;
                }

                proCmd.StandardInput.WriteLine(strCmd_ + " 1>&2");  // Stdout redirect to Stderr
                proCmd.StandardInput.WriteLine("exit");
                
                string strError = string.Empty;
                while (!proCmd.StandardError.EndOfStream)
                {
                    strError = proCmd.StandardError.ReadLine();
                    actOutput_(strError);
                }

                proCmd.WaitForExit(1000);
                proCmd.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return true;
        }

        private void btnPortStatus_Click(object sender, EventArgs e)
        {
            string strPort = this.txtListenPort.Text.Trim();
            if (strPort == "0")
                strPort = string.Empty;

            this.lstInfo.Clear();
            this.lstInfo.Columns.Add("", 3);
            this.lstInfo.Columns.Add("Protocol", 80);
            this.lstInfo.Columns.Add("Local", 200);
            this.lstInfo.Columns.Add("Remote", 200);
            this.lstInfo.Columns.Add("Status", 120);
            this.lstInfo.Columns.Add("PID", 80);

            this.Cursor = Cursors.WaitCursor;
            string strCmd = "netstat -ano";
            ExecuteCmd(strCmd, z => ShowNetstat(strPort, z));
            if(this.lstInfo.Items.Count <= 1)
            {
                ListViewItem lvItem = new ListViewItem();
                lvItem.SubItems.Add("!!!");
                lvItem.SubItems.Add("Not Found");
                this.lstInfo.Items.Add(lvItem);
            }
            this.Cursor = Cursors.Default;
        }

        private void ShowNetstat(string strPort_, string strOutput_)
        {
            var aryInfo = strOutput_.Split((char[])(null), StringSplitOptions.RemoveEmptyEntries);
            if (aryInfo.Length != 5)
                return;

            if (!string.IsNullOrEmpty(strPort_))
            {
                // 127.0.0.1:80
                // [::]:80
                var aryAddr = aryInfo[1].Split(':');
                if (aryAddr.Length > 1 && aryAddr[aryAddr.Length - 1] != strPort_)
                    return;
            }

            ListViewItem lvItem = new ListViewItem();
            foreach(var item in aryInfo)
            {
                lvItem.SubItems.Add(item);
            }
            this.lstInfo.Items.Add(lvItem);
        }

        private void btnTcping_Click(object sender, EventArgs e)
        {
            int nCount = 4;
            string strAddr = this.txtPingIP.Text;
            string strPort = this.txtPingPort.Text;
            int.TryParse(this.txtCount.Text, out nCount);

            string strPingCmd = Path.Combine(Application.StartupPath, "tcping.exe");
            strPingCmd += string.Format(" -n 1 {0} {1}", strAddr, strPort);
            this.lstInfo.Clear();
            this.lstInfo.Columns.Add("", 5);
            this.lstInfo.Columns.Add("Tips", 300);
            this.lstInfo.Columns.Add("Status", 280);
            this.lstInfo.Columns.Add("Time", 150);
            var lvItem = new ListViewItem(DateTime.Now.ToLongTimeString());
            lvItem.SubItems.Add(string.Format("To Probe {0}:{1} {2}-times", strAddr, strPort, nCount));
            this.lstInfo.Items.Add(lvItem);

            _strPingAddr = string.Format("{0}:{1}", strAddr, strPort);
            Thread thrStart = new Thread(() => StartTcpingThread(nCount, strPingCmd));
            thrStart.IsBackground = true;
            thrStart.Start();
        }

        void StartTcpingThread(int nCount_, string strCmd_)
        {
            ResetWinCursor(Cursors.WaitCursor);
            _bStop = false;

            DateTime dtLast = DateTime.MinValue;
            do 
            {
                dtLast = DateTime.Now;
                ExecuteCmd(strCmd_, ShowTcping);

                if (_bStop) break;
                WaitAtleastOneSecond(dtLast);
            } while (--nCount_ > 0 && !_bStop);

            ResetWinCursor(Cursors.Default);
        }

        private void ShowTcping(string strOutput_)
        {
            var aryInfo = strOutput_.Split(new char[]{'-'}, StringSplitOptions.RemoveEmptyEntries);
            if (aryInfo.Length != 3) return;

            var lvItem = new ListViewItem(DateTime.Now.ToLongTimeString());
            foreach(var item in aryInfo)
            {
                lvItem.SubItems.Add(item);
            }

            AddListItem(lvItem);
        }

        private void btnPing_Click(object sender, EventArgs e)
        {
            int nCount = 4;
            string strAddr = this.txtPingIP.Text;
            int.TryParse(this.txtCount.Text, out nCount);

            this.lstInfo.Clear();
            this.lstInfo.Columns.Add("", 5);
            this.lstInfo.Columns.Add("Status", 300);
            this.lstInfo.Columns.Add("Time(MS)", 100);
            this.lstInfo.Columns.Add("TTL", 100);

            _strPingAddr = strAddr;
            _dtLastPing = DateTime.MinValue;
            _bStop = false;
            var thrPing = new Thread(() => StartPingThread(nCount));
            thrPing.IsBackground = true;
            thrPing.Start();
        }

        string _strPingAddr = string.Empty;
        DateTime _dtLastPing = DateTime.Now;
        private void StartPingThread(int nCount)
        {
            --nCount;
            if (nCount < 0 || _bStop)
            {
                ResetWinCursor(Cursors.Default);
                return;
            }
            WaitAtleastOneSecond(_dtLastPing);

            ResetWinCursor(Cursors.WaitCursor);
            using (var piSend = new Ping())
            {
                piSend.PingCompleted += new PingCompletedEventHandler(piSend_PingCompleted);

                _dtLastPing = DateTime.Now;
                piSend.SendAsync(_strPingAddr, nCount);
            }
        }

        private void WaitAtleastOneSecond(DateTime dtLast_)
        {
            DateTime dtNow = DateTime.Now;
            if ((dtNow - dtLast_).TotalSeconds < 1)
            {
                var dtInternal = 1000 - (int)((dtNow - dtLast_).TotalMilliseconds);
                Thread.Sleep(dtInternal);
            }
        }

        void piSend_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            ListViewItem lvItem = new ListViewItem(DateTime.Now.ToLongTimeString());
            if(e.Cancelled)
            {
                lvItem.SubItems.Add("!!Is Cancelled");
            }
            else if(e.Error != null)
            {
                lvItem.SubItems.Add("!!" + e.Error.ToString());
            }
            else
            {
                PingReply repPing = e.Reply;
                lvItem.SubItems.Add(string.Format("Ping {0}: {1}", _strPingAddr, repPing.Status));
                lvItem.SubItems.Add(repPing.RoundtripTime.ToString());
                if (repPing.Options != null)
                    lvItem.SubItems.Add(repPing.Options.Ttl.ToString());
            }

            AddListItem(lvItem);

            int nCount = (int)e.UserState;
            StartPingThread(nCount);
        }

        bool _bStop = false;
        private void btnStop_Click(object sender, EventArgs e)
        {
            _bStop = true;
        }
    } // class
}
