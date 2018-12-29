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
using SHCre.Xugd.Common;
using SHCre.Xugd.Data;
using SHCre.Xugd.Net.Xmpp;

namespace RedundantClient
{
    public partial class FrmClient : Form
    {
        //public class ServerAddress
        //{
        //    [XmlAttribute]
        //    public string SrvId { get; set; }
        //    public string Domain { get; set; }
        //    public string IP { get; set; }
        //    public int Port { get; set; }
        //    public string SrvUser { get; set; }
        //}

        //public class ClientInfo
        //{
        //    public ClientInfo()
        //    {
        //    }
        //    public ClientInfo(string strName_, string strPsw_)
        //    {
        //        UserName = strName_;
        //        Password = strPsw_;
        //    }
        //    [XmlAttribute]
        //    public string UserName { get; set; }
        //    [XmlAttribute]
        //    public string Password { get; set; }

        //    public override string ToString()
        //    {
        //        return UserName;
        //    }
        //}

        //public class ServerAddressInfos
        //{
        //    public List<ServerAddress> SrvAddrs { get; set; }
        //    public List<ClientInfo> Clients { get; set; }

        //    public ServerAddressInfos()
        //    {
        //        SrvAddrs = new List<ServerAddress>();
        //        Clients = new List<ClientInfo>();
        //    }

        //    public static ServerAddressInfos Read()
        //    {
        //        ServerAddressInfos addrInfo = null;
        //        try
        //        {
        //            addrInfo = XConFile.Read<ServerAddressInfos>("RedundantClient.xml");
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.ToString());
        //        }

        //        if (addrInfo == null)
        //        {
        //            addrInfo = new ServerAddressInfos();
        //            addrInfo.SrvAddrs.Add(new ServerAddress()
        //            {
        //                SrvId = "1",
        //                Domain = "crecti.net",
        //                IP = "192.168.1.200",
        //                Port = 5222,
        //                SrvUser = "ctisrv",
        //            });
        //            addrInfo.SrvAddrs.Add(new ServerAddress()
        //            {
        //                SrvId = "2",
        //                Domain = "crecti.net",
        //                IP = "192.168.1.202",
        //                Port = 5222,
        //                SrvUser = "ctisrv",
        //            });
        //            addrInfo.Clients.Add(new ClientInfo("cc2001", "CreCTI"));
        //            addrInfo.Clients.Add(new ClientInfo("cc2002", "CreCTI"));
        //                addrInfo.Clients.Add(new ClientInfo("cc2003", "CreCTI"));
        //                addrInfo.Clients.Add(new ClientInfo("cc2004", "CreCTI"));
        //            addrInfo.Write();
        //        }

        //        return addrInfo;
        //    }

        //    public void Write()
        //    {
        //        XConFile.Write("RedundantClient.xml", this);
        //    }
        //}
        //ServerAddressInfos _srvAddrInfo;

        XStandbyClientConfig _conClient;

        public FrmClient()
        {
            InitializeComponent();
        }

        private void FrmClient_Load(object sender, EventArgs e)
        {
            string strFile = "XStandbyClient.xml";
            _conClient = XConFile.Read<XStandbyClientConfig>(strFile);
            if(_conClient == null){
                _conClient = new XStandbyClientConfig();
                _conClient.Init();
                XConFile.Write(strFile, _conClient);
            }
            if(_conClient.StandbyServer.Count(z=>z.Enabled) == 0)
            {
                MessageBox.Show("No server config");
                this.Close();
                return;
            }

            InitSrvInfo();
            InitCbUsers();
        }

        void InitSrvInfo()
        {
            string strText = string.Empty;
            _conClient.StandbyServer.ForEach(z =>
            {
                ListViewItem lvItem = new ListViewItem(string.Format("User {0} -> [{1}] {2}", z.XmppServer.LoginInfo.UserName, z.ServerName, z.XmppServer.LoginInfo.PrintAddr()));
                this.lvwSrvs.Items.Add(lvItem);
            });
        }

        void InitCbUsers()
        {
            //_srvAddrInfo.Clients.ForEach(z => this.cbUsers.Items.Add(z));
            //this.cbUsers.SelectedIndex = 0;
        }

        enum TestCode
        {
            ClientLogin = 1,
            MasterChange = 2,
            SendSync,
            SendClient,
        }

        XStandbyClient<TestCode> _redClient = null;
        void InitClient()
        {
            if(_redClient != null)
            {
                _redClient.LogoutSync();
                _redClient = null;
            }

            _redClient = new XStandbyClient<TestCode>(TestCode.ClientLogin, TestCode.MasterChange);

            _redClient.AddClientServer(_conClient);

            _redClient.OnExcept += _redClient_OnError;
            _redClient.OnMasterChanged += new Action<XDataComm<TestCode>>(_redClient_OnMasterChanged);
            _redClient.OnRequestArrival += new Action<XDataComm<TestCode>, XDataComm<TestCode>.RequestArrivalArgs>(_redClient_OnRequestArrival);
            _redClient.OnSrvConnect += new Action<XDataComm<TestCode>>(_redClient_OnLoginServer);
        }

        void _redClient_OnLoginServer(XDataComm<FrmClient.TestCode> obj)
        {
            AddInfo("Login " + obj.NetComm.RemoteAddress);
        }

        void _redClient_OnRequestArrival(XDataComm<TestCode> dataComm_, XDataComm<FrmClient.TestCode>.RequestArrivalArgs obj)
        {
            AddInfo(string.Format("[{0}]{1}->:{2}", obj.RequestData.DataType, obj.From, obj.RequestData.DataJson));
        }

        void _redClient_OnMasterChanged(XDataComm<FrmClient.TestCode> obj)
        {
            if (obj == null)
                AddInfo("No master");
            else
                AddInfo(obj.NetComm.RemoteAddress + " is master");
        }

        void _redClient_OnError(Exception ex_, string strInfo)
        {
            AddInfo(string.Format("{0}: {1}", strInfo, ex_.ToString()));
        }

        private void cbUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Text = this.cbUsers.SelectedItem.ToString();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                this.btnStart.Enabled = false;
                InitClient();
                _redClient.LoginSync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                this.btnStart.Enabled = true;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            this.btnStop.Enabled = false;
            _redClient.LogoutSync();
            this.btnStop.Enabled = true;
        }

        int SendCount = 0;
        class Send2Srv
        {
            public Send2Srv(int nIndex_)
            {
                Value = string.Format("[{0}]:{1}{{<<", nIndex_, XRandom.GetString(10));
            }

            public string Value;
        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            if(_redClient == null)
            {
                MessageBox.Show("Login first");
                return;
            }

            var dataSend = new Send2Srv(++SendCount);
            bool bAll = XRandom.GetBool();
            AddInfo(string.Format("[{0}]:{1}", bAll ? "All" : "Master", dataSend.Value));

            if (bAll)
                _redClient.Send2AllServer(TestCode.SendClient, dataSend, (zop) =>
                    {
                        if (zop.IsSuccess)
                            AddInfo("->[All]: " + dataSend.Value);
                        else
                            MessageBox.Show(zop.Result.ToString());
                    }, false);
            else
                _redClient.Send2MasterOnly(TestCode.SendClient, dataSend, (zop) =>
                    {
                        if (zop.IsSuccess)
                            AddInfo("->[Master]: " + dataSend.Value);
                        else
                            MessageBox.Show(zop.Result.ToString());
                    }, false);
        }

        private void AddInfo(string strInfo_)
        {
            if(InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(AddInfo), strInfo_);
                return;
            }

            strInfo_ = XTime.GetTimeString(DateTime.Now, true) + " " + strInfo_;
            this.lvwInfo.Items.Add(new ListViewItem(strInfo_));
            this.lvwInfo.EnsureVisible(this.lvwInfo.Items.Count - 1);
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.lvwInfo.Items.Clear();
        }
    }
}
