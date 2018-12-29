using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using SHCre.Xugd.Config;
using SHCre.Xugd.Net;
using SHCre.Xugd.Extension;
using SHCre.Xugd.Common;
using System.IO;
using SHCre.Xugd.Net.Xmpp;
using System.Threading;
using SHCre.Xugd.Data;

namespace RedundantSrv
{
    public partial class FrmSrv : Form
    {
        XStandbyServerConfig _conServer = null;
        public FrmSrv()
        {
            InitializeComponent();
        }

        private void FrmSrv_Load(object sender, EventArgs e)
        {
            string strFile = "XStandbySrv.xml";
            _conServer = XConFile.Read<XStandbyServerConfig>(strFile);
            if(_conServer == null){
                _conServer = new XStandbyServerConfig();
                _conServer.Init();
                XConFile.Write(strFile, _conServer);
            }
            
            InitSrvInfo();
            InitRedServer();
        }

        string _strText;
        void InitSrvInfo()
        {
            // Add server
            XmppConfig conXmpp = _conServer.LocalServer;
            ListViewItem lvItem = new ListViewItem("Server");
            lvItem.SubItems.Add(string.Format("{0} at {1}", conXmpp.LoginInfo.UserName, conXmpp.LoginInfo.PrintAddr()));
            this.lvwSrvs.Items.Add(lvItem);
            
            _strText = this.Text = "Srv: " + conXmpp.LoginInfo.PrintAddr();

            // Add sync
            conXmpp = _conServer.SyncServer.XmppServer;
            lvItem = new ListViewItem("Sync");
            lvItem.SubItems.Add(string.Format("{0} at {1}", conXmpp.LoginInfo.UserName, conXmpp.LoginInfo.PrintAddr()));
            this.lvwSrvs.Items.Add(lvItem);            
        }

        enum TestCode
        {
            ClientLogin = 1,
            MasterChange = 2,
            SendSync,
            SendClient,
        }

        XStandbyServer<TestCode> _redSrv = null;
        private void InitRedServer()
        {
            _redSrv = new XStandbyServer<TestCode>(TestCode.ClientLogin, TestCode.MasterChange);
            _redSrv.SetServerInfo(_conServer);

            _redSrv.OnMasterChanged += new Action<bool>(_redSrv_OnMasterChanged);
            _redSrv.OnExcept += _redSrv_OnError;
            _redSrv.OnRequestArrival += new Action<XDataComm<TestCode>.RequestArrivalArgs, bool>(_redSrv_OnRequestArrival);
            _redSrv.OnClientLogin += new Action<string>(_redSrv_OnClientLogin);
            _redSrv.OnClientLogout += new Action<string>(_redSrv_OnClientLogout);
        }

        void _redSrv_OnClientLogout(string obj)
        {
            AddInfo("Client Logout: " + obj);
        }

        void _redSrv_OnClientLogin(string obj)
        {
            AddInfo("Client Login: " + obj);
        }

        void _redSrv_OnRequestArrival(XDataComm<FrmSrv.TestCode>.RequestArrivalArgs arg1, bool arg2)
        {
            string strReceive = string.Format("[{0}]{1}->{2}[{3}]", arg2 ? "Sync" : "Client", arg1.From,
                arg1.RequestData.DataJson, arg1.RequestData.DataType);
            AddInfo(strReceive);
        }

        private void AddInfo(string strInfo_)
        {
            if(InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(AddInfo), strInfo_);
                return;
            }

            if (this.chkSlient.Checked) return;

            if (!string.IsNullOrEmpty(strInfo_))
                strInfo_ = XTime.GetTimeString(DateTime.Now, true) + " " + strInfo_;
            ListViewItem lvItem = new ListViewItem(strInfo_);
            this.lvwInfo.Items.Add(lvItem);
            this.lvwInfo.EnsureVisible(lvwInfo.Items.Count - 1);
        }

        void _redSrv_OnError(Exception ex_, string strInfo)
        {
            AddInfo(string.Format("{0}: {1}", strInfo, ex_.ToString()));
        }

        void _redSrv_OnMasterChanged(bool obj)
        {
            string strSrv = obj ? "IsMaster" : "NotMaster";
            AddInfo("MasterChange: " + strSrv);
            AddTitle(strSrv);
        }

        private void AddTitle(string strSrv)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(AddTitle), strSrv);
                return;
            }

            this.Text = _strText + " - " + strSrv;
        }

        int nTimes = 0;
        bool _bStopRequest = false;
        void AutoTestThread(){
            while(!_bStopRequest){
                try
                {
                    AddInfo("");
                    ++nTimes;
                    AddInfo(nTimes.ToString() + " - To Start");
                    _redSrv.Start();
                    AddInfo("Start success: " + (_redSrv.IsMasterServer ? "IsMaster" : "NotMaster"));
                   

                    Thread.Sleep(XTime.Second2Interval(XRandom.GetInt(20, 40)));

                    AddInfo("   To stop");
                    _redSrv.Stop();
                    AddInfo("stopped");
                    Thread.Sleep(XTime.Second2Interval(XRandom.GetInt(1,5)));
                }
                catch (Exception ex)
                {
                    this.btnStart.Enabled = true;
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.btnStart.Enabled = false;
            this.btnStop.Enabled = true;

            _bStopRequest = false;
            XThread.StartThread(AutoTestThread);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.lvwInfo.Items.Clear();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.btnSend.Enabled = true;
            this.btnStart.Enabled = true;
            this.btnStop.Enabled = false;

            _bStopRequest = true;
        }

        private void FrmSrv_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_redSrv != null)
                _redSrv.Stop();
        }

        int SendCount = 0;
        class Send2Srv
        {
            public Send2Srv(int nIndex_)
            {
                Value = string.Format("[{0}]:{1}", nIndex_, XRandom.GetString(10));
            }

            public string Value;
        }

        void SendMsgThread(){
            while (!_bStopRequest)
            {
                try
                {
                    if (!_redSrv.IsServerStarted)
                        _redSrv.Start();
                    ++nTimes;


                    var dataSend = new Send2Srv(++SendCount);
                    _redSrv.Send2Server(TestCode.SendSync, dataSend);
                    AddInfo(nTimes.ToString() + " ->Sync: " + dataSend.Value);

                    Thread.Sleep(XRandom.GetInt(100, 500));
                }
                catch (Exception ex)
                {
                    this.btnSend.Enabled = true;
                    MessageBox.Show(ex.ToString());
                }
            }

            _redSrv.Stop();
        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            this.btnSend.Enabled = false;
            this.btnStop.Enabled = true;
            _bStopRequest = false;

            XThread.StartThread(SendMsgThread);
        }
    }
}
