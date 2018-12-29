namespace TestForm
{
    partial class FrmImChat
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
            this.msMain = new System.Windows.Forms.MenuStrip();
            this.mainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnSendClass = new System.Windows.Forms.Button();
            this.lstFriends = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.btnSend = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.rtxtSend = new System.Windows.Forms.RichTextBox();
            this.txtReceiver = new System.Windows.Forms.TextBox();
            this.lvwInfo = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.setLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.msMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // msMain
            // 
            this.msMain.BackColor = System.Drawing.SystemColors.ControlDark;
            this.msMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainToolStripMenuItem,
            this.clearInfoToolStripMenuItem,
            this.setLevelToolStripMenuItem});
            this.msMain.Location = new System.Drawing.Point(0, 0);
            this.msMain.Name = "msMain";
            this.msMain.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.msMain.Size = new System.Drawing.Size(740, 28);
            this.msMain.TabIndex = 7;
            this.msMain.Text = "msMain";
            // 
            // mainToolStripMenuItem
            // 
            this.mainToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loginToolStripMenuItem,
            this.logoutToolStripMenuItem});
            this.mainToolStripMenuItem.Name = "mainToolStripMenuItem";
            this.mainToolStripMenuItem.Size = new System.Drawing.Size(57, 24);
            this.mainToolStripMenuItem.Text = "Main";
            this.mainToolStripMenuItem.DropDownOpening += new System.EventHandler(this.mainToolStripMenuItem_DropDownOpening);
            // 
            // loginToolStripMenuItem
            // 
            this.loginToolStripMenuItem.Name = "loginToolStripMenuItem";
            this.loginToolStripMenuItem.Size = new System.Drawing.Size(131, 24);
            this.loginToolStripMenuItem.Text = "Login";
            this.loginToolStripMenuItem.Click += new System.EventHandler(this.loginToolStripMenuItem_Click);
            // 
            // logoutToolStripMenuItem
            // 
            this.logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
            this.logoutToolStripMenuItem.Size = new System.Drawing.Size(131, 24);
            this.logoutToolStripMenuItem.Text = "Logout";
            this.logoutToolStripMenuItem.Click += new System.EventHandler(this.logoutToolStripMenuItem_Click);
            // 
            // clearInfoToolStripMenuItem
            // 
            this.clearInfoToolStripMenuItem.Name = "clearInfoToolStripMenuItem";
            this.clearInfoToolStripMenuItem.Size = new System.Drawing.Size(86, 24);
            this.clearInfoToolStripMenuItem.Text = "ClearInfo";
            this.clearInfoToolStripMenuItem.Click += new System.EventHandler(this.clearInfoToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 28);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnSendClass);
            this.splitContainer1.Panel1.Controls.Add(this.lstFriends);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.btnSend);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.rtxtSend);
            this.splitContainer1.Panel1.Controls.Add(this.txtReceiver);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lvwInfo);
            this.splitContainer1.Size = new System.Drawing.Size(740, 526);
            this.splitContainer1.SplitterDistance = 235;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 8;
            // 
            // btnSendClass
            // 
            this.btnSendClass.Location = new System.Drawing.Point(545, 201);
            this.btnSendClass.Margin = new System.Windows.Forms.Padding(4);
            this.btnSendClass.Name = "btnSendClass";
            this.btnSendClass.Size = new System.Drawing.Size(113, 29);
            this.btnSendClass.TabIndex = 16;
            this.btnSendClass.Text = "Send Class";
            this.btnSendClass.UseVisualStyleBackColor = true;
            this.btnSendClass.Click += new System.EventHandler(this.btnSendClass_Click);
            // 
            // lstFriends
            // 
            this.lstFriends.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lstFriends.Location = new System.Drawing.Point(16, 32);
            this.lstFriends.Margin = new System.Windows.Forms.Padding(4);
            this.lstFriends.Name = "lstFriends";
            this.lstFriends.Size = new System.Drawing.Size(292, 196);
            this.lstFriends.TabIndex = 9;
            this.lstFriends.UseCompatibleStateImageBehavior = false;
            this.lstFriends.View = System.Windows.Forms.View.Details;
            this.lstFriends.DoubleClick += new System.EventHandler(this.lstFriends_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 213;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 15);
            this.label1.TabIndex = 10;
            this.label1.Text = "Friends:";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(361, 201);
            this.btnSend.Margin = new System.Windows.Forms.Padding(4);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(135, 29);
            this.btnSend.TabIndex = 14;
            this.btnSend.Text = "SendMsg";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(333, 12);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 15);
            this.label2.TabIndex = 11;
            this.label2.Text = "Receiver:";
            // 
            // rtxtSend
            // 
            this.rtxtSend.Location = new System.Drawing.Point(336, 45);
            this.rtxtSend.Margin = new System.Windows.Forms.Padding(4);
            this.rtxtSend.Name = "rtxtSend";
            this.rtxtSend.Size = new System.Drawing.Size(371, 148);
            this.rtxtSend.TabIndex = 13;
            this.rtxtSend.Text = "";
            // 
            // txtReceiver
            // 
            this.txtReceiver.Location = new System.Drawing.Point(443, 10);
            this.txtReceiver.Margin = new System.Windows.Forms.Padding(4);
            this.txtReceiver.Name = "txtReceiver";
            this.txtReceiver.Size = new System.Drawing.Size(264, 25);
            this.txtReceiver.TabIndex = 12;
            this.txtReceiver.Text = "creclient@sh-202.cti";
            // 
            // lvwInfo
            // 
            this.lvwInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.lvwInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwInfo.Location = new System.Drawing.Point(0, 0);
            this.lvwInfo.Margin = new System.Windows.Forms.Padding(4);
            this.lvwInfo.Name = "lvwInfo";
            this.lvwInfo.ShowItemToolTips = true;
            this.lvwInfo.Size = new System.Drawing.Size(740, 286);
            this.lvwInfo.TabIndex = 15;
            this.lvwInfo.UseCompatibleStateImageBehavior = false;
            this.lvwInfo.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Info";
            this.columnHeader2.Width = 800;
            // 
            // setLevelToolStripMenuItem
            // 
            this.setLevelToolStripMenuItem.Name = "setLevelToolStripMenuItem";
            this.setLevelToolStripMenuItem.Size = new System.Drawing.Size(83, 24);
            this.setLevelToolStripMenuItem.Text = "SetLevel";
            this.setLevelToolStripMenuItem.Click += new System.EventHandler(this.setLevelToolStripMenuItem_Click);
            // 
            // FrmImChat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 554);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.msMain);
            this.MainMenuStrip = this.msMain;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmImChat";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmImChat";
            this.Load += new System.EventHandler(this.FrmImChat_Load);
            this.msMain.ResumeLayout(false);
            this.msMain.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip msMain;
        private System.Windows.Forms.ToolStripMenuItem mainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loginToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearInfoToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnSendClass;
        private System.Windows.Forms.ListView lstFriends;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox rtxtSend;
        private System.Windows.Forms.TextBox txtReceiver;
        private System.Windows.Forms.ListView lvwInfo;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ToolStripMenuItem setLevelToolStripMenuItem;
    }
}