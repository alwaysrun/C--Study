using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SHCre.Xugd.Net;
using SHCre.Xugd.Data;
using SHCre.Xugd.Common;
using SHCre.Xugd.Config;
using System.Threading;

namespace TcpClient
{
    public partial class FrmClient : Form
    {
        public enum SendType
        {
            Code = 1,
            RetCode,
        };

        XCommClientConfig _conClient;
        XCommConnection _Connection;
        XDataComm<SendType> _dataComm;

        public FrmClient()
        {
            InitializeComponent();
        }

        private void FrmClient_Load(object sender, EventArgs e)
        {
            string strFile = "TcpClient.xml";
            _conClient = XConFile.Read<XCommClientConfig>(strFile);
            if(_conClient == null)
            {
                _conClient = new XCommClientConfig();
                _conClient.Init("192.168.1.100", 5901);

                XConFile.Write(strFile, _conClient);
            }

            this.txtIP.Text = _conClient.RemoteAddress.Address;
            this.txtPort.Text = _conClient.RemoteAddress.Port.ToString();
            if (_conClient.Protocol == XCommProtocol.Tcp)
                this.rdbTcp.Checked = true;
            else
                this.rdbUdp.Checked = true;

            SetSendButton(false);
        }

        private void AddInfo(string strInfo_)
        {
            if(InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(AddInfo), strInfo_);
                return;
            }

            this.lvwInfo.Items.Add(new ListViewItem(strInfo_));
            this.lvwInfo.EnsureVisible(this.lvwInfo.Items.Count - 1);
        }

        private void ConnSuccess()
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action(ConnSuccess));
                return;
            }

            AddInfo("Connect to " + _Connection.RemoteAddress + " success");
            this.Text = string.Format("Client[{0}]", _Connection.LocalAddress);
            SetSendButton(true);
        }

        private void SetSendButton(bool bEndble_)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<bool>(SetSendButton), bEndble_);
                return;
            }

            this.btnSend.Enabled =
                this.btnSendMulti.Enabled = 
                this.btnSendOne.Enabled = bEndble_;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            int nPort = int.Parse(this.txtPort.Text);
            _conClient.RemoteAddress.Address = this.txtIP.Text;
            _conClient.RemoteAddress.Port = nPort;
            _conClient.Protocol = this.rdbTcp.Checked ? XCommProtocol.Tcp : XCommProtocol.Udp;

            if(_Connection != null)
            {
                if (_dataComm != null)
                {
                    _dataComm.Stop();
                    _dataComm = null;
                }
                _Connection.Close();
                _Connection = null;
            }

            _Connection = _conClient.GetClient();
            _Connection.OnConnected += new Action(_Connection_OnConnected);
            _Connection.OnDisconnected += new Action<bool>(_Connection_OnDisconnected);

            _dataComm = new XDataComm<SendType>(_Connection);
            _dataComm.OnRequestArrival += new Action<XDataComm<SendType>.RequestArrivalArgs>(_comData_OnRequestArrival);
            _dataComm.OnResponseMismatch += new Action<XDataComm<SendType>.ResponseMismatchArgs>(_comData_OnResponseMismatch);
            _dataComm.Start();

            _Connection.ConnectAsync((zarg) =>
                {
                    if(zarg.IsSuccess)
                    {
                        //ConnSuccess();
                    }
                    else
                    {
                        MessageBox.Show(zarg.Result.ToString());
                    }
                });
        }

        void _Connection_OnDisconnected(bool obj)
        {
            AddInfo("Disconnect");
            SetSendButton(false);
        }

        void _Connection_OnConnected()
        {
            ConnSuccess();
            SetSendButton(true);
        }

        void _comData_OnResponseMismatch(XDataComm<FrmClient.SendType>.ResponseMismatchArgs obj)
        {
            AddInfo(string.Format("!!!Response Mismatch: Index {0}, Type {1}, Data {2}", obj.DataIndex, obj.DataType, obj.ResponseData));
        }

        void _comData_OnRequestArrival(XDataComm<FrmClient.SendType>.RequestArrivalArgs obj)
        {
            AddInfo(string.Format("Request: From {0}, Type {1}, Data {2}", obj.From, obj.RequestData.DataType, obj.RequestData.DataJson));
        }

        private void clearInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.lvwInfo.Items.Clear();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if(this.rtxtInfo.TextLength == 0)
            {
                MessageBox.Show("Input info");
                return;
            }

            _Connection.SendAsync(this.rtxtInfo.Text, (zop) =>
                {
                    if(zop.IsSuccess)
                    {
                        AddInfo("Send success");
                    }
                    else
                    {
                        MessageBox.Show(zop.Result.ToString());
                    }
                });
        }

        class SendInfo
        {
            public string Info;
            public int Number;
            public DateTime Time;
        }

        int SendCount = 0;
        private void btnSendMulti_Click(object sender, EventArgs e)
        {
            XThread.StartThread(() =>
                {
                    SetSendButton(false);
                    for(int i=0 ; i<100; ++i)
                    {
                        SendData();
                        SendData();
                        SendData();
                        SendData();
                        SendData();

                        Thread.Sleep(XRandom.GetInt(10, 100));
                    }
                    SetSendButton(true);
                });

            //this.btnJson.Enabled = true;
        }

        private void SendData()
        {
            // this.btnJson.Enabled = false;
            int i = SendCount++;
            SendInfo sInfo = new SendInfo();
            sInfo.Info = XRandom.GetString(10);
            sInfo.Number = XRandom.GetInt();
            sInfo.Time = DateTime.Now;

            bool bResponse = XRandom.GetBool();
            _dataComm.SendAsyn(string.Empty, SendType.Code, sInfo,
                (xarg) =>
                {
                    AddInfo(string.Empty);
                    AddInfo(string.Format("{6}: Send[{5}]: {0}Type {1}, Info {2}, Number {3}, Time {4}",
                        string.Empty, SendType.Code, sInfo.Info, sInfo.Number,
                        sInfo.Time, bResponse ? "NeedResponse" : "NoResponse", i + 1));

                    if (xarg.IsSuccess)
                    {
                        if (bResponse)
                        {
                            var rData = xarg.ResponseData.GetData<SendInfo>();
                            AddInfo(string.Format("Response: Index {0}, Type {1}, Info {2}, Number {3}, Time {4}",
                                xarg.ResponseData.DataIndex, xarg.ResponseData.DataType, rData.Info, rData.Number, rData.Time));
                        }
                        else
                        {
                            AddInfo("Send success");
                        }
                    }
                    else
                    {
                        if (xarg.Result is TimeoutException)
                            AddInfo("Send Fail: " + xarg.Result.Message);
                        else
                            MessageBox.Show(xarg.Result.ToString());
                    }
                },
                bResponse);
        }

        private void btnSendOne_Click(object sender, EventArgs e)
        {
            SendData();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (_Connection != null)
            {
                if (_dataComm != null)
                {
                    _dataComm.Stop();
                    _dataComm = null;
                }
                _Connection.Close();
                _Connection = null;
            }
        }
    }
}
