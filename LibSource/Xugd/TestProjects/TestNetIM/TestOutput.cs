using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using SHCre.Xugd.CFile;

namespace TestForm
{
    public partial class TestOutput : Form
    {
        public TestOutput()
        {
            InitializeComponent();
        }

        private void TestOutput_Load(object sender, EventArgs e)
        {
            GetStartPath();
        }

        private void btnGetStartPath_Click(object sender, EventArgs e)
        {
            GetStartPath();
        }

        private void GetStartPath()
        {
            this.lstInfo.Items.Add("App.StartPath: " + Application.StartupPath);
            this.lstInfo.Items.Add("App.ExePath: " + Application.ExecutablePath);

            using (var prc = Process.GetCurrentProcess())
            {
                this.lstInfo.Items.Add("Proc.FileName: " + prc.MainModule.FileName);

                prc.Close();
            }

            this.lstInfo.Items.Add("FullPath: " + XPath.GetFullPath("test.txt"));
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.lstInfo.Items.Clear();
        }
    }
}
