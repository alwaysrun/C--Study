namespace TestForm
{
    partial class SshTest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SshTest));
            this.txtPsw = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtIp = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtLocal = new System.Windows.Forms.TextBox();
            this.txtRemote = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUpLoad = new System.Windows.Forms.Button();
            this.lstInfo = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnListFiles = new System.Windows.Forms.Button();
            this.btnIsDir = new System.Windows.Forms.Button();
            this.btnIsFile = new System.Windows.Forms.Button();
            this.lblOperate = new System.Windows.Forms.Label();
            this.lblTransfered = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.btnLocal = new System.Windows.Forms.Button();
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtPsw
            // 
            this.txtPsw.Location = new System.Drawing.Point(323, 59);
            this.txtPsw.Margin = new System.Windows.Forms.Padding(4);
            this.txtPsw.Name = "txtPsw";
            this.txtPsw.Size = new System.Drawing.Size(171, 25);
            this.txtPsw.TabIndex = 3;
            this.txtPsw.Text = "gat!@#$";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(244, 62);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 15);
            this.label5.TabIndex = 18;
            this.label5.Text = "Password";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(65, 59);
            this.txtName.Margin = new System.Windows.Forms.Padding(4);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(165, 25);
            this.txtName.TabIndex = 2;
            this.txtName.Text = "cre";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 62);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 15);
            this.label4.TabIndex = 16;
            this.label4.Text = "Name";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(303, 13);
            this.txtPort.Margin = new System.Windows.Forms.Padding(4);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(191, 25);
            this.txtPort.TabIndex = 1;
            this.txtPort.Text = "22";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(256, 16);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 15);
            this.label3.TabIndex = 14;
            this.label3.Text = "Port";
            // 
            // txtIp
            // 
            this.txtIp.Location = new System.Drawing.Point(49, 13);
            this.txtIp.Margin = new System.Windows.Forms.Padding(4);
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(181, 25);
            this.txtIp.TabIndex = 0;
            this.txtIp.Text = "10.196.14.5";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 16);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 15);
            this.label2.TabIndex = 12;
            this.label2.Text = "IP";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 15);
            this.label1.TabIndex = 20;
            this.label1.Text = "Local";
            // 
            // txtLocal
            // 
            this.txtLocal.Location = new System.Drawing.Point(91, 107);
            this.txtLocal.Margin = new System.Windows.Forms.Padding(4);
            this.txtLocal.Name = "txtLocal";
            this.txtLocal.Size = new System.Drawing.Size(364, 25);
            this.txtLocal.TabIndex = 4;
            this.txtLocal.Text = "d:\\wavfiles";
            // 
            // txtRemote
            // 
            this.txtRemote.Location = new System.Drawing.Point(91, 140);
            this.txtRemote.Margin = new System.Windows.Forms.Padding(4);
            this.txtRemote.Name = "txtRemote";
            this.txtRemote.Size = new System.Drawing.Size(403, 25);
            this.txtRemote.TabIndex = 5;
            this.txtRemote.Text = "/home/fsdir/recordings/voices";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 143);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 15);
            this.label6.TabIndex = 22;
            this.label6.Text = "Remote";
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(133, 172);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(83, 29);
            this.btnDown.TabIndex = 6;
            this.btnDown.Text = "DownLoad";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUpLoad
            // 
            this.btnUpLoad.Location = new System.Drawing.Point(219, 172);
            this.btnUpLoad.Name = "btnUpLoad";
            this.btnUpLoad.Size = new System.Drawing.Size(77, 29);
            this.btnUpLoad.TabIndex = 7;
            this.btnUpLoad.Text = "UpLoad";
            this.btnUpLoad.UseVisualStyleBackColor = true;
            this.btnUpLoad.Click += new System.EventHandler(this.btnUpLoad_Click);
            // 
            // lstInfo
            // 
            this.lstInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lstInfo.Location = new System.Drawing.Point(21, 207);
            this.lstInfo.Name = "lstInfo";
            this.lstInfo.Size = new System.Drawing.Size(473, 179);
            this.lstInfo.TabIndex = 26;
            this.lstInfo.UseCompatibleStateImageBehavior = false;
            this.lstInfo.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Info";
            this.columnHeader1.Width = 467;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(21, 172);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(72, 29);
            this.btnConnect.TabIndex = 27;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnListFiles
            // 
            this.btnListFiles.Location = new System.Drawing.Point(407, 172);
            this.btnListFiles.Name = "btnListFiles";
            this.btnListFiles.Size = new System.Drawing.Size(87, 29);
            this.btnListFiles.TabIndex = 28;
            this.btnListFiles.Text = "ListFile";
            this.btnListFiles.UseVisualStyleBackColor = true;
            this.btnListFiles.Click += new System.EventHandler(this.btnListFiles_Click);
            // 
            // btnIsDir
            // 
            this.btnIsDir.Location = new System.Drawing.Point(342, 392);
            this.btnIsDir.Name = "btnIsDir";
            this.btnIsDir.Size = new System.Drawing.Size(73, 29);
            this.btnIsDir.TabIndex = 29;
            this.btnIsDir.Text = "IsDir";
            this.btnIsDir.UseVisualStyleBackColor = true;
            this.btnIsDir.Click += new System.EventHandler(this.btnIsDir_Click);
            // 
            // btnIsFile
            // 
            this.btnIsFile.Location = new System.Drawing.Point(421, 392);
            this.btnIsFile.Name = "btnIsFile";
            this.btnIsFile.Size = new System.Drawing.Size(73, 29);
            this.btnIsFile.TabIndex = 30;
            this.btnIsFile.Text = "IsFile";
            this.btnIsFile.UseVisualStyleBackColor = true;
            this.btnIsFile.Click += new System.EventHandler(this.btnIsFile_Click);
            // 
            // lblOperate
            // 
            this.lblOperate.Location = new System.Drawing.Point(23, 391);
            this.lblOperate.Name = "lblOperate";
            this.lblOperate.Size = new System.Drawing.Size(84, 27);
            this.lblOperate.TabIndex = 31;
            this.lblOperate.Text = "Progress:";
            this.lblOperate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTransfered
            // 
            this.lblTransfered.Location = new System.Drawing.Point(124, 391);
            this.lblTransfered.Name = "lblTransfered";
            this.lblTransfered.Size = new System.Drawing.Size(98, 27);
            this.lblTransfered.TabIndex = 32;
            this.lblTransfered.Text = "0";
            this.lblTransfered.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTotal
            // 
            this.lblTotal.Location = new System.Drawing.Point(235, 391);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(98, 27);
            this.lblTotal.TabIndex = 33;
            this.lblTotal.Text = "0";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(220, 399);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(15, 15);
            this.label9.TabIndex = 34;
            this.label9.Text = "/";
            // 
            // btnLocal
            // 
            this.btnLocal.Location = new System.Drawing.Point(462, 110);
            this.btnLocal.Name = "btnLocal";
            this.btnLocal.Size = new System.Drawing.Size(39, 23);
            this.btnLocal.TabIndex = 35;
            this.btnLocal.Text = "...";
            this.btnLocal.UseVisualStyleBackColor = true;
            this.btnLocal.Click += new System.EventHandler(this.btnLocal_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(302, 172);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 29);
            this.btnCancel.TabIndex = 36;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // SshTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 421);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnLocal);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.lblTransfered);
            this.Controls.Add(this.lblOperate);
            this.Controls.Add(this.btnIsFile);
            this.Controls.Add(this.btnIsDir);
            this.Controls.Add(this.btnListFiles);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.lstInfo);
            this.Controls.Add(this.btnUpLoad);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.txtRemote);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtLocal);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPsw);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtIp);
            this.Controls.Add(this.label2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SshTest";
            this.Text = "Sftp";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SshTest_FormClosing);
            this.Load += new System.EventHandler(this.SshTest_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtPsw;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtIp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtLocal;
        private System.Windows.Forms.TextBox txtRemote;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUpLoad;
        private System.Windows.Forms.ListView lstInfo;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnListFiles;
        private System.Windows.Forms.Button btnIsDir;
        private System.Windows.Forms.Button btnIsFile;
        private System.Windows.Forms.Label lblOperate;
        private System.Windows.Forms.Label lblTransfered;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnLocal;
        private System.Windows.Forms.OpenFileDialog dlgOpen;
        private System.Windows.Forms.Button btnCancel;
    }
}