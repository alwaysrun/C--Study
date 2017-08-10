namespace XDrawBoard
{
    partial class FrmScreenShot
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FrmScreenShot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(201, 149);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmScreenShot";
            this.ShowInTaskbar = false;
            this.Text = "frmPickScreen";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmScreenShot_FormClosing);
            this.Load += new System.EventHandler(this.frmPickScreen_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmPickScreen_Paint);
            this.DoubleClick += new System.EventHandler(this.frmPickScreen_DoubleClick);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmPickScreen_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmPickScreen_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmPickScreen_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmPickScreen_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion


    }
}