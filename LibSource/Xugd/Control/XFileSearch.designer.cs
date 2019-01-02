namespace SHCre.Xugd.XControl
{
    partial class XFileSearch
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XFileSearch));
            this.grpCondition = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtSearchHere = new System.Windows.Forms.TextBox();
            this.lblSearchHere = new System.Windows.Forms.Label();
            this.txtContext = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.lblContext = new System.Windows.Forms.Label();
            this.lblResults = new System.Windows.Forms.Label();
            this.proBar = new System.Windows.Forms.ProgressBar();
            this.lvwResults = new System.Windows.Forms.ListView();
            this.grpCondition.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpCondition
            // 
            this.grpCondition.Controls.Add(this.btnBrowse);
            this.grpCondition.Controls.Add(this.txtSearchHere);
            this.grpCondition.Controls.Add(this.lblSearchHere);
            this.grpCondition.Controls.Add(this.txtContext);
            this.grpCondition.Controls.Add(this.btnSearch);
            this.grpCondition.Controls.Add(this.lblContext);
            this.grpCondition.Location = new System.Drawing.Point(8, 11);
            this.grpCondition.Name = "grpCondition";
            this.grpCondition.Size = new System.Drawing.Size(537, 84);
            this.grpCondition.TabIndex = 0;
            this.grpCondition.TabStop = false;
            this.grpCondition.Text = "Search Conditions";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnBrowse.Location = new System.Drawing.Point(480, 55);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(37, 23);
            this.btnBrowse.TabIndex = 4;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtSearchHere
            // 
            this.txtSearchHere.Location = new System.Drawing.Point(134, 57);
            this.txtSearchHere.Name = "txtSearchHere";
            this.txtSearchHere.ReadOnly = true;
            this.txtSearchHere.Size = new System.Drawing.Size(324, 21);
            this.txtSearchHere.TabIndex = 3;
            // 
            // lblSearchHere
            // 
            this.lblSearchHere.AutoSize = true;
            this.lblSearchHere.Location = new System.Drawing.Point(6, 60);
            this.lblSearchHere.Name = "lblSearchHere";
            this.lblSearchHere.Size = new System.Drawing.Size(71, 12);
            this.lblSearchHere.TabIndex = 3;
            this.lblSearchHere.Text = "Search here";
            // 
            // txtContext
            // 
            this.txtContext.Location = new System.Drawing.Point(134, 28);
            this.txtContext.Name = "txtContext";
            this.txtContext.Size = new System.Drawing.Size(324, 21);
            this.txtContext.TabIndex = 1;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(464, 26);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(65, 23);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // lblContext
            // 
            this.lblContext.AutoSize = true;
            this.lblContext.Location = new System.Drawing.Point(6, 32);
            this.lblContext.Name = "lblContext";
            this.lblContext.Size = new System.Drawing.Size(59, 12);
            this.lblContext.TabIndex = 0;
            this.lblContext.Text = "File Name";
            // 
            // lblResults
            // 
            this.lblResults.AutoSize = true;
            this.lblResults.Location = new System.Drawing.Point(6, 102);
            this.lblResults.Name = "lblResults";
            this.lblResults.Size = new System.Drawing.Size(293, 12);
            this.lblResults.TabIndex = 1;
            this.lblResults.Text = "Search result（Open the folder by double-click）";
            // 
            // proBar
            // 
            this.proBar.Location = new System.Drawing.Point(8, 396);
            this.proBar.Name = "proBar";
            this.proBar.Size = new System.Drawing.Size(537, 13);
            this.proBar.TabIndex = 3;
            // 
            // lvwResults
            // 
            this.lvwResults.FullRowSelect = true;
            this.lvwResults.HideSelection = false;
            this.lvwResults.Location = new System.Drawing.Point(8, 121);
            this.lvwResults.Name = "lvwResults";
            this.lvwResults.Size = new System.Drawing.Size(537, 271);
            this.lvwResults.TabIndex = 2;
            this.lvwResults.TabStop = false;
            this.lvwResults.UseCompatibleStateImageBehavior = false;
            this.lvwResults.View = System.Windows.Forms.View.Details;
            this.lvwResults.DoubleClick += new System.EventHandler(this.lvwResults_DoubleClick);
            // 
            // XFileSearch
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 416);
            this.Controls.Add(this.proBar);
            this.Controls.Add(this.lvwResults);
            this.Controls.Add(this.lblResults);
            this.Controls.Add(this.grpCondition);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "XFileSearch";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Search Files";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSearch_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmSearch_KeyDown);
            this.Load += new System.EventHandler(this.FrmSearch_Load);
            this.grpCondition.ResumeLayout(false);
            this.grpCondition.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpCondition;
        private System.Windows.Forms.Label lblContext;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtSearchHere;
        private System.Windows.Forms.Label lblSearchHere;
        private System.Windows.Forms.TextBox txtContext;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label lblResults;
        private System.Windows.Forms.ListView lvwResults;
        private System.Windows.Forms.ProgressBar proBar;
    }
}