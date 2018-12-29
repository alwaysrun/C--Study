using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SHCre.Xugd.Common;

namespace DateConvert
{
    public partial class FrmDateTime : Form
    {
        public FrmDateTime()
        {
            InitializeComponent();
        }

        private void btnLong2Dt_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(this.txtDtLong.Text))
            {
                MessageBox.Show("Input time to convert");
                return;
            }

            long lCon;
            bool bToSecond = rdbSecond.Checked;
            string strTime = this.txtDtLong.Text.Trim();
            if (bToSecond)
                strTime += "000000";
            try
            {
                lCon = long.Parse(strTime + "0");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            TimeSpan toNow = new TimeSpan(lCon);

            DateTime dtCon = (new DateTime(1970, 1, 1)).Add(toNow);

            bool bToLocal = rdbLocal.Checked;
            if (bToLocal)
                dtCon = TimeZone.CurrentTimeZone.ToLocalTime(dtCon);


            this.txtDtTime.Text = XTime.GetDateString(dtCon, "yyyy-MM-dd HH:mm:ss.ffffff");
        }
    }
}
