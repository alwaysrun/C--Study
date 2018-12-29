namespace NetworkInfo
{
    partial class FrmNetwork
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmNetwork));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnPing = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCount = new System.Windows.Forms.TextBox();
            this.txtPingPort = new System.Windows.Forms.TextBox();
            this.txtPingIP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnTcping = new System.Windows.Forms.Button();
            this.txtListenPort = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPortStatus = new System.Windows.Forms.Button();
            this.btnTcpConnect = new System.Windows.Forms.Button();
            this.btnUdpListener = new System.Windows.Forms.Button();
            this.btnTcpListener = new System.Windows.Forms.Button();
            this.lstInfo = new System.Windows.Forms.ListView();
            this.btnStop = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnStop);
            this.splitContainer1.Panel1.Controls.Add(this.btnPing);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.txtCount);
            this.splitContainer1.Panel1.Controls.Add(this.txtPingPort);
            this.splitContainer1.Panel1.Controls.Add(this.txtPingIP);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.btnTcping);
            this.splitContainer1.Panel1.Controls.Add(this.txtListenPort);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.btnPortStatus);
            this.splitContainer1.Panel1.Controls.Add(this.btnTcpConnect);
            this.splitContainer1.Panel1.Controls.Add(this.btnUdpListener);
            this.splitContainer1.Panel1.Controls.Add(this.btnTcpListener);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lstInfo);
            this.splitContainer1.Size = new System.Drawing.Size(742, 435);
            this.splitContainer1.SplitterDistance = 92;
            this.splitContainer1.TabIndex = 0;
            // 
            // btnPing
            // 
            this.btnPing.Location = new System.Drawing.Point(537, 55);
            this.btnPing.Name = "btnPing";
            this.btnPing.Size = new System.Drawing.Size(51, 35);
            this.btnPing.TabIndex = 13;
            this.btnPing.Text = "Ping";
            this.btnPing.UseVisualStyleBackColor = true;
            this.btnPing.Click += new System.EventHandler(this.btnPing_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(490, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(22, 15);
            this.label4.TabIndex = 12;
            this.label4.Text = "次";
            // 
            // txtCount
            // 
            this.txtCount.Location = new System.Drawing.Point(423, 60);
            this.txtCount.Name = "txtCount";
            this.txtCount.Size = new System.Drawing.Size(61, 25);
            this.txtCount.TabIndex = 5;
            this.txtCount.Text = "4";
            this.txtCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtPingPort
            // 
            this.txtPingPort.Location = new System.Drawing.Point(248, 60);
            this.txtPingPort.Name = "txtPingPort";
            this.txtPingPort.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtPingPort.Size = new System.Drawing.Size(48, 25);
            this.txtPingPort.TabIndex = 3;
            this.txtPingPort.Text = "80";
            // 
            // txtPingIP
            // 
            this.txtPingIP.Location = new System.Drawing.Point(92, 60);
            this.txtPingIP.Name = "txtPingIP";
            this.txtPingIP.Size = new System.Drawing.Size(139, 25);
            this.txtPingIP.TabIndex = 2;
            this.txtPingIP.Text = "127.0.0.1";
            this.txtPingIP.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(231, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(22, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "Ping地址：";
            // 
            // btnTcping
            // 
            this.btnTcping.Location = new System.Drawing.Point(299, 55);
            this.btnTcping.Name = "btnTcping";
            this.btnTcping.Size = new System.Drawing.Size(118, 35);
            this.btnTcping.TabIndex = 4;
            this.btnTcping.Text = "Ping指定端口";
            this.btnTcping.UseVisualStyleBackColor = true;
            this.btnTcping.Click += new System.EventHandler(this.btnTcping_Click);
            // 
            // txtListenPort
            // 
            this.txtListenPort.Location = new System.Drawing.Point(92, 20);
            this.txtListenPort.Name = "txtListenPort";
            this.txtListenPort.Size = new System.Drawing.Size(78, 25);
            this.txtListenPort.TabIndex = 0;
            this.txtListenPort.Text = "0";
            this.txtListenPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "指定端口：";
            // 
            // btnPortStatus
            // 
            this.btnPortStatus.Location = new System.Drawing.Point(176, 13);
            this.btnPortStatus.Name = "btnPortStatus";
            this.btnPortStatus.Size = new System.Drawing.Size(118, 35);
            this.btnPortStatus.TabIndex = 1;
            this.btnPortStatus.Text = "查询端口状态";
            this.btnPortStatus.UseVisualStyleBackColor = true;
            this.btnPortStatus.Click += new System.EventHandler(this.btnPortStatus_Click);
            // 
            // btnTcpConnect
            // 
            this.btnTcpConnect.Location = new System.Drawing.Point(348, 11);
            this.btnTcpConnect.Name = "btnTcpConnect";
            this.btnTcpConnect.Size = new System.Drawing.Size(137, 35);
            this.btnTcpConnect.TabIndex = 6;
            this.btnTcpConnect.Text = "Tcp Connection";
            this.btnTcpConnect.UseVisualStyleBackColor = true;
            this.btnTcpConnect.Click += new System.EventHandler(this.btnTcpConnect_Click);
            // 
            // btnUdpListener
            // 
            this.btnUdpListener.Location = new System.Drawing.Point(612, 11);
            this.btnUdpListener.Name = "btnUdpListener";
            this.btnUdpListener.Size = new System.Drawing.Size(118, 35);
            this.btnUdpListener.TabIndex = 8;
            this.btnUdpListener.Text = "Udp Listener";
            this.btnUdpListener.UseVisualStyleBackColor = true;
            this.btnUdpListener.Click += new System.EventHandler(this.btnUdpListener_Click);
            // 
            // btnTcpListener
            // 
            this.btnTcpListener.Location = new System.Drawing.Point(488, 11);
            this.btnTcpListener.Name = "btnTcpListener";
            this.btnTcpListener.Size = new System.Drawing.Size(118, 35);
            this.btnTcpListener.TabIndex = 7;
            this.btnTcpListener.Text = "Tcp Listener";
            this.btnTcpListener.UseVisualStyleBackColor = true;
            this.btnTcpListener.Click += new System.EventHandler(this.btnTcpListener_Click);
            // 
            // lstInfo
            // 
            this.lstInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstInfo.FullRowSelect = true;
            this.lstInfo.GridLines = true;
            this.lstInfo.HideSelection = false;
            this.lstInfo.Location = new System.Drawing.Point(0, 0);
            this.lstInfo.MultiSelect = false;
            this.lstInfo.Name = "lstInfo";
            this.lstInfo.ShowItemToolTips = true;
            this.lstInfo.Size = new System.Drawing.Size(742, 339);
            this.lstInfo.TabIndex = 0;
            this.lstInfo.UseCompatibleStateImageBehavior = false;
            this.lstInfo.View = System.Windows.Forms.View.Details;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(688, 55);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(51, 35);
            this.btnStop.TabIndex = 14;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // FrmNetwork
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(742, 435);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmNetwork";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CreNetworkInfo";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnTcpListener;
        private System.Windows.Forms.ListView lstInfo;
        private System.Windows.Forms.Button btnUdpListener;
        private System.Windows.Forms.Button btnTcpConnect;
        private System.Windows.Forms.TextBox txtListenPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPortStatus;
        private System.Windows.Forms.Button btnTcping;
        private System.Windows.Forms.TextBox txtPingPort;
        private System.Windows.Forms.TextBox txtPingIP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCount;
        private System.Windows.Forms.Button btnPing;
        private System.Windows.Forms.Button btnStop;
    }
}

