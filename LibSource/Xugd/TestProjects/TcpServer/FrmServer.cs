using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SHCre.Xugd.Net;
using SHCre.Xugd.Config;
using SHCre.Xugd.Extension;
using SHCre.Xugd.Data;
using SHCre.Xugd.Common;

namespace TcpServer
{
    public partial class FrmServer : Form
    {
        XDataComm<SendType> _dataComm;
        XCommServer _commServer = null;
        XCommServerConfig _conServer = null;

        public FrmServer()
        {
            InitializeComponent();
        }


        private void FrmServer_Load(object sender, EventArgs e)
        {
            _conServer = GetConfig();
            if (_conServer.Protocol == XCommProtocol.Tcp)
                this.rdbTcp.Checked = true;
            else
                this.rdbUdp.Checked = true;

            StartListen();
        }

        private void AddInfo(string strInfo_)
        {
            this.lvwMessage.Items.Add(new ListViewItem(strInfo_));
            this.lvwMessage.EnsureVisible(this.lvwMessage.Items.Count - 1);
        }

        void _tcpServer_OnListenError(XCommListener arg1, Exception arg2)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<XCommListener, Exception>(_tcpServer_OnListenError), arg1, arg2);
                return;
            }

            MessageBox.Show(arg2.ToString());
        }

        void _tcpServer_OnConnectionRemoved(XCommConnection obj)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<XCommConnection>(_tcpServer_OnConnectionRemoved), obj);
                return;
            }

            int nIndex = this.lvwConnects.Items.IndexOfKey(obj.RemoteAddress);
            if (nIndex >= 0)
            {
                this.lvwConnects.Items.RemoveAt(nIndex);
            }
        }


        public enum SendType
        {
            Code = 1,
            RetCode,
        };

        void _tcpServer_OnConnectionAdded(XCommConnection obj)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<XCommConnection>(_tcpServer_OnConnectionAdded), obj);
                return;
            }

            ListViewItem lvItem = new ListViewItem(obj.RemoteAddress);
            lvItem.Name = obj.RemoteAddress;

            this.lvwConnects.Items.Add(lvItem);
        }

        class SendInfo
        {
            public SendInfo()
            {
                Info = string.Empty;
                Number = 0;
                Time = DateTime.Now;
            }
            public string Info;
            public int Number;
            public DateTime Time;
        }

        void dataCom_OnRequestArrival(XDataComm<FrmServer.SendType>.RequestArrivalArgs objRequest)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<XDataComm<SendType>.RequestArrivalArgs>(dataCom_OnRequestArrival), objRequest);
                return;
            }

            var info = objRequest.RequestData.GetData<SendInfo>();
            this.AddInfo(string.Format("Request[{0}]: Info {1}, Number {2}, Time {3}",
                objRequest.RequestData.NeedResponse ? "NeedResponse" : "NoResponse", info.Info, info.Number, info.Time));

            if (objRequest.RequestData.NeedResponse)
            {
                info.Info += ":Response";
                objRequest.InvokeResponse(SendType.RetCode, info, null);
            }
        }

        void obj_OnDataReceived(XCommConnection con_, XReceiveDataArgs data_)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<XCommConnection, XReceiveDataArgs>(obj_OnDataReceived), con_, data_);
                return;
            }

            this.lvwMessage.Items.Add(new ListViewItem("->:" + data_.Data));
        }

        void obj_OnDisconnected(XCommConnection obj)
        {
            // throw new NotImplementedException();
        }

        void obj_OnConnectCompleted(object sender, XAsyncResult e)
        {
            throw new NotImplementedException();
        }

        private XCommServerConfig GetConfig()
        {
            string strFile = "TcpServer.xml";
            XCommServerConfig conListen = XConFile.Read<XCommServerConfig>(strFile);
            if (conListen == null)
            {
                conListen = new XCommServerConfig();
                conListen.Init(5901);

                XConFile.Write(strFile, conListen);
            }

            this.Text = "Listen At: " + conListen.ListenAddresses[0].Port;
            return conListen;
        }

        private void clearInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.lvwMessage.Items.Clear();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartListen();
        }

        private void StartListen()
        {
            this.lvwConnects.Items.Clear();

            if (_commServer != null)
            {
                if (_dataComm != null)
                {
                    _dataComm.Stop();
                    _dataComm = null;
                }
                _commServer.Stop();
                _commServer = null;
            }
            _conServer.Protocol = this.rdbTcp.Checked ? XCommProtocol.Tcp : XCommProtocol.Udp;
            _commServer = _conServer.GetServer();

            _commServer.OnConnectionAdded += new Action<XCommConnection>(_tcpServer_OnConnectionAdded);
            _commServer.OnConnectionRemoved += new Action<XCommConnection>(_tcpServer_OnConnectionRemoved);
            _commServer.OnListenError += new Action<XCommListener, Exception>(_tcpServer_OnListenError);
            _commServer.Start();
            //_commServer.ReceiveBufferSize = 500;
            _dataComm = new XDataComm<SendType>(_commServer);
            _dataComm.OnRequestArrival += new Action<XDataComm<SendType>.RequestArrivalArgs>(dataCom_OnRequestArrival);

            AddInfo("listening");
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_commServer != null)
            {
                if (_dataComm != null)
                {
                    _dataComm.Stop();
                    _dataComm = null;
                }
                _commServer.Stop();
                _commServer = null;
            }
        }
    }
}
