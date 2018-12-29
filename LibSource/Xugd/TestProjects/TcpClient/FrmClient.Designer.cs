namespace TcpClient
{
    partial class FrmClient
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdbUdp = new System.Windows.Forms.RadioButton();
            this.rdbTcp = new System.Windows.Forms.RadioButton();
            this.btnSendOne = new System.Windows.Forms.Button();
            this.btnSendMulti = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.rtxtInfo = new System.Windows.Forms.RichTextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lvwInfo = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.clearInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 28);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.btnSendOne);
            this.splitContainer1.Panel1.Controls.Add(this.btnSendMulti);
            this.splitContainer1.Panel1.Controls.Add(this.btnSend);
            this.splitContainer1.Panel1.Controls.Add(this.rtxtInfo);
            this.splitContainer1.Panel1.Controls.Add(this.btnClose);
            this.splitContainer1.Panel1.Controls.Add(this.btnConnect);
            this.splitContainer1.Panel1.Controls.Add(this.txtPort);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.txtIP);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lvwInfo);
            this.splitContainer1.Size = new System.Drawing.Size(643, 452);
            this.splitContainer1.SplitterDistance = 126;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdbUdp);
            this.groupBox1.Controls.Add(this.rdbTcp);
            this.groupBox1.Location = new System.Drawing.Point(17, 4);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(352, 44);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Protocol";
            // 
            // rdbUdp
            // 
            this.rdbUdp.AutoSize = true;
            this.rdbUdp.Location = new System.Drawing.Point(173, 16);
            this.rdbUdp.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdbUdp.Name = "rdbUdp";
            this.rdbUdp.Size = new System.Drawing.Size(69, 24);
            this.rdbUdp.TabIndex = 0;
            this.rdbUdp.Text = "Udp";
            this.rdbUdp.UseVisualStyleBackColor = true;
            // 
            // rdbTcp
            // 
            this.rdbTcp.AutoSize = true;
            this.rdbTcp.Location = new System.Drawing.Point(41, 19);
            this.rdbTcp.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rdbTcp.Name = "rdbTcp";
            this.rdbTcp.Size = new System.Drawing.Size(69, 24);
            this.rdbTcp.TabIndex = 0;
            this.rdbTcp.Text = "Tcp";
            this.rdbTcp.UseVisualStyleBackColor = true;
            // 
            // btnSendOne
            // 
            this.btnSendOne.Location = new System.Drawing.Point(395, 89);
            this.btnSendOne.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSendOne.Name = "btnSendOne";
            this.btnSendOne.Size = new System.Drawing.Size(100, 29);
            this.btnSendOne.TabIndex = 8;
            this.btnSendOne.Text = "SendOne";
            this.btnSendOne.UseVisualStyleBackColor = true;
            this.btnSendOne.Click += new System.EventHandler(this.btnSendOne_Click);
            // 
            // btnSendMulti
            // 
            this.btnSendMulti.Location = new System.Drawing.Point(271, 89);
            this.btnSendMulti.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSendMulti.Name = "btnSendMulti";
            this.btnSendMulti.Size = new System.Drawing.Size(116, 29);
            this.btnSendMulti.TabIndex = 7;
            this.btnSendMulti.Text = "SendMulti";
            this.btnSendMulti.UseVisualStyleBackColor = true;
            this.btnSendMulti.Click += new System.EventHandler(this.btnSendMulti_Click);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(508, 88);
            this.btnSend.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(125, 29);
            this.btnSend.TabIndex = 6;
            this.btnSend.Text = "Send Input";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // rtxtInfo
            // 
            this.rtxtInfo.Location = new System.Drawing.Point(419, 4);
            this.rtxtInfo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rtxtInfo.Name = "rtxtInfo";
            this.rtxtInfo.Size = new System.Drawing.Size(213, 75);
            this.rtxtInfo.TabIndex = 0;
            this.rtxtInfo.Text = "";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(139, 90);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(89, 29);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(16, 90);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(115, 29);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(311, 59);
            this.txtPort.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(84, 25);
            this.txtPort.TabIndex = 3;
            this.txtPort.Text = "5901";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(251, 62);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port";
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(59, 58);
            this.txtIP.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(159, 25);
            this.txtIP.TabIndex = 1;
            this.txtIP.Text = "192.168.1.100";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 61);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP";
            // 
            // lvwInfo
            // 
            this.lvwInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvwInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwInfo.Location = new System.Drawing.Point(0, 0);
            this.lvwInfo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lvwInfo.Name = "lvwInfo";
            this.lvwInfo.Size = new System.Drawing.Size(643, 321);
            this.lvwInfo.TabIndex = 0;
            this.lvwInfo.UseCompatibleStateImageBehavior = false;
            this.lvwInfo.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Info";
            this.columnHeader1.Width = 476;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearInfoToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(643, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // clearInfoToolStripMenuItem
            // 
            this.clearInfoToolStripMenuItem.Name = "clearInfoToolStripMenuItem";
            this.clearInfoToolStripMenuItem.Size = new System.Drawing.Size(86, 24);
            this.clearInfoToolStripMenuItem.Text = "ClearInfo";
            this.clearInfoToolStripMenuItem.Click += new System.EventHandler(this.clearInfoToolStripMenuItem_Click);
            // 
            // FrmClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 480);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FrmClient";
            this.Text = "CommClient";
            this.Load += new System.EventHandler(this.FrmClient_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView lvwInfo;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem clearInfoToolStripMenuItem;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.RichTextBox rtxtInfo;
        private System.Windows.Forms.Button btnSendMulti;
        private System.Windows.Forms.Button btnSendOne;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdbUdp;
        private System.Windows.Forms.RadioButton rdbTcp;
        private System.Windows.Forms.Button btnClose;
    }
}

