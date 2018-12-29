using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SHCre.Xugd.CFile;

namespace TestForm
{
    public partial class FrmLogLevel : Form
    {
        public XLogSimple.LogLevels LogLevel;

        public FrmLogLevel()
        {
            InitializeComponent();
            LogLevel = XLogSimple.LogLevels.None;
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            if (rdbtnDebug.Checked)
                LogLevel = XLogSimple.LogLevels.Debug;
            else if (rdbtnInfo.Checked)
                LogLevel = XLogSimple.LogLevels.Info;
            else
                LogLevel = XLogSimple.LogLevels.Error;

            this.DialogResult = DialogResult.OK;
        }
    }
}
