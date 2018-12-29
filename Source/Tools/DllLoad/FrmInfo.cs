using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;


namespace SHCre.Tools.DllLoad
{
    public partial class FrmInfo : Form
    {
        public FrmInfo()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if(this.txtFile.Text.Length == 0)
            {
                MessageBox.Show("Input the file to load.");
                return;
            }
            if(!File.Exists(this.txtFile.Text))
            {
                MessageBox.Show("File not exits, reinput please");
                return;
            }

            try
            {
                Assembly ass = Assembly.LoadFrom(this.txtFile.Text);
                MessageBox.Show("Load Success");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if( dlgOpenFile.ShowDialog() == DialogResult.OK )
            {
                this.txtFile.Text = dlgOpenFile.FileName;
            }
        }
    }
}
