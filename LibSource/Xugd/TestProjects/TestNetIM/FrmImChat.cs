using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SHCre.Xugd.Net;
using SHCre.Xugd.Common;
using System.Threading;
using SHCre.Xugd.Net.Xmpp;
using SHCre.Xugd.Data;

namespace TestForm
{
    public partial class FrmImChat : Form
    {
        class SendInfo
        {
            public string Info;
            public int Number;
            public DateTime Time;
        }

        enum SendType
        {
            Code = 0x1,
            RetCode = Code | 0x1000,
        };

        XmppClient _imChat;
        XDataComm<SendType> _dataComm;


        public FrmImChat()
        {
            InitializeComponent();
        }

        private void FrmImChat_Load(object sender, EventArgs e)
        {
            _imChat = new XmppClient();
            _imChat.OnDisconnected += new Action<bool>(_imChat_OnClose);
            _imChat.OnDataReceived += new Action<XReceiveDataArgs>(_imChat_OnDataReceived);
            _imChat.OnExcept += _imChat_OnError;
            _imChat.OnFriendLogin += new Action<string>(_imChat_OnFriendLogin);
            _imChat.OnFriendLogout += new Action<string>(_imChat_OnFriendLogout);

            _dataComm = new XDataComm<SendType>(_imChat);
            _dataComm.OnRequestArrival += new Action<XDataComm<SendType>.RequestArrivalArgs>(_dataComm_OnRequestArrival);
            _dataComm.OnResponseMismatch += new Action<XDataComm<SendType>.ResponseMismatchArgs>(_dataComm_OnResponseMismatch);
            _dataComm.ResponseWaitSeconds = 30;

            IMLogin();
        }

        void _dataComm_OnResponseMismatch(XDataComm<FrmImChat.SendType>.ResponseMismatchArgs argData_)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<XDataComm<FrmImChat.SendType>.ResponseMismatchArgs>(_dataComm_OnResponseMismatch), argData_);
                return;
            }

            AddInfo(string.Format("ResponseMismatch: From {0}, Index {1}, Type {2}, Data {3}",
                argData_.From, argData_.DataIndex, argData_.DataType, argData_.ResponseData));
        }

        void _dataComm_OnRequestArrival(XDataComm<FrmImChat.SendType>.RequestArrivalArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<XDataComm<SendType>.RequestArrivalArgs>(_dataComm_OnRequestArrival), e);
                return;
            }

            // AddInfo(string.Format("Request from {0}, Index:{1}, Type:{2}, Data:{3}", e.From, e.RequestData.DataIndex, e.RequestData.DataType, e.RequestData.DataJson));
            if (e.RequestData.DataType == SendType.Code)
            {
                var reData = e.RequestData.GetData<SendInfo>();
                AddInfo(string.Format("Request from {0}, Index:{1}, Type:{2}, info:{3}, number:{4}, time:{5}",
                    e.From, e.RequestData.DataIndex, e.RequestData.DataType, reData.Info, reData.Number, reData.Time));

                if (e.RequestData.NeedResponse)
                {
                    XThread.StartThread(() =>
                    {
                        try
                        {

                            Thread.Sleep(XTime.Second2Interval(XRandom.GetInt(1, 60)));
                            reData.Info += ":Response";
                            e.InvokeResponse(SendType.RetCode, reData, null);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                    });
                }
            }
            else
            {
                AddInfo(string.Format("ComDataRequst: Invalid request type {0}, Index {1}", e.RequestData.DataType, e.RequestData.DataIndex));
            }
        }

        void _imChat_OnFriendLogout(string obj)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(_imChat_OnFriendLogout), obj);
                return;
            }

            this.lstFriends.Items.RemoveByKey(obj);
        }

        void _imChat_OnFriendLogin(string obj)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(_imChat_OnFriendLogin), new object[] { obj });
                return;
            }

            this.lstFriends.Items.RemoveByKey(obj);

            ListViewItem lvItem = new ListViewItem(obj);
            lvItem.Name = obj;
            this.lstFriends.Items.Add(lvItem);
        }

        void _imChat_OnError(Exception obj, string strInfo_)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Exception, string>(_imChat_OnError), new object[] { obj, strInfo_ });
                return;
            }

            // AddInfo("OnError: " + obj.ToString());
            MessageBox.Show(string.Format("{0}: {1}", strInfo_, obj.ToString()));
        }

        void _imChat_OnDataReceived(XReceiveDataArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<XReceiveDataArgs>(_imChat_OnDataReceived), e);
                return;
            }

            AddInfo(string.Format("{0}:{1}", e.From, e.Data));
        }

        void _imChat_OnClose(bool bClose_)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<bool>(_imChat_OnClose), bClose_);
                return;
            }

            _dataComm.Stop();
            this.lstFriends.Items.Clear();
            AddInfo("Logout");
        }

        private void AddInfo(string strInfo_)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(AddInfo), strInfo_);
                return;
            }

            this.lvwInfo.Items.Add(new ListViewItem(strInfo_));
            this.lvwInfo.EnsureVisible(lvwInfo.Items.Count - 1);
        }

        private void IMLogin()
        {
            FrmImLogin frmLogin = new FrmImLogin(_imChat);
            if (frmLogin.ShowDialog() == DialogResult.OK)
            {
                DateTime dtNow = DateTime.Now;
                AddInfo(string.Format("Login: {0}-{1}, User {2}s", 
                    XTime.GetTimeString(frmLogin.LoginStart),
                    XTime.GetTimeString(dtNow),
                    (dtNow-frmLogin.LoginStart).TotalSeconds));
                _dataComm.Start();
                this.Text = _imChat.UserJid;
            }
        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IMLogin();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                _imChat.LogoutSync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void mainToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            //this.logoutToolStripMenuItem.Enabled = _imChat.IsLogged;
            //this.loginToolStripMenuItem.Enabled = !_imChat.IsLogged;
        }

        private void clearInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.lvwInfo.Items.Clear();
        }

        private void lstFriends_DoubleClick(object sender, EventArgs e)
        {
            if (this.lstFriends.SelectedItems.Count == 0)
                return;

            this.txtReceiver.Text = this.lstFriends.SelectedItems[0].Name;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            _imChat.SendAsync(this.txtReceiver.Text, this.rtxtSend.Text, (arg) =>
                {
                    if (arg.IsSuccess)
                    {
                        AddInfo("Send Success");
                    }
                    else
                    {
                        MessageBox.Show(arg.Result.ToString());
                    }
                });
        }

        int SendCount = 0;
        private void btnSendClass_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtReceiver.Text))
            {
                MessageBox.Show("input receiver");
                return;
            }

            this.btnSendClass.Enabled = false;
            int i = SendCount++;
            SendInfo sInfo = new SendInfo();
            sInfo.Info = XRandom.GetString(10);
            sInfo.Number = XRandom.GetInt();
            sInfo.Time = DateTime.Now;

            bool bResponse = XRandom.GetBool();
            _dataComm.SendAsyn(this.txtReceiver.Text, SendType.Code, sInfo,
                (xarg) =>
                {
                    AddInfo(string.Empty);
                    AddInfo(string.Format("{6}: Send[{5}]: to {0}, Type {1}, Info {2}, Number {3}, Time {4}",
                        this.txtReceiver.Text, SendType.Code, sInfo.Info, sInfo.Number,
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

            this.btnSendClass.Enabled = true;
        }

        private void setLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmLogLevel frmLevel = new FrmLogLevel();
            if(frmLevel.ShowDialog() == DialogResult.OK){
                _imChat.SetLogLevel(frmLevel.LogLevel);
            }
        }
    } // class
}
