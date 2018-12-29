using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SHCre.Xugd.Net;
using SHCre.Xugd.Net.Xmpp;

namespace TestForm
{
    public partial class FrmImLogin : Form
    {
        XmppClient _imClient = null;
        public FrmImLogin(XmppClient imCon_)
        {
            InitializeComponent();

            _imClient = imCon_;
        }

        private void FrmImLogin_Load(object sender, EventArgs e)
        {
            this.txtDomain.Text = "sh-202.cti";
            this.txtIp.Text = "192.168.1.202";
            this.txtPort.Text = "5222";
            this.txtName.Text = "cretest";
            this.txtPsw.Text = "xgdxgd";
        }

        public DateTime LoginStart {get; private set;}

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try 
            {
                XmppConfig imConfig = new XmppConfig();
                imConfig.Init();
                imConfig.LoginInfo.Set(this.txtIp.Text,
                    this.txtDomain.Text,
                    this.txtName.Text,
                    this.txtPsw.Text,
                    int.Parse(this.txtPort.Text)
                    );
                _imClient.SetServerInfo(imConfig);

                this.btnLogin.Enabled = false;
                LoginStart = DateTime.Now;
                _imClient.LoginSync();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                this.btnLogin.Enabled = true;
            }

            this.DialogResult = DialogResult.OK;
        }
    }
}
