using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace FrmTools
{
    public partial class FrmProcess : Form
    {
        public FrmProcess()
        {
            InitializeComponent();
        }

        private void FrmProcess_Load(object sender, EventArgs e)
        {
            this.txtFileName.Text = Path.Combine(@"E:\Projects\SHCre\Project\LibSource\DotNet\SHCre.Xugd\TestProjects\TestNetIM\bin\Debug", "TestForm.exe");
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(this.txtFileName.Text))
            {
                MessageBox.Show("Input file to start");
                return;
            }

            Process.Start(this.txtFileName.Text);
        }
    }
}
