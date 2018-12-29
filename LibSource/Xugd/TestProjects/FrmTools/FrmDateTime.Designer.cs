namespace FrmTools
{
    partial class FrmDateTime
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
            this.txtDtLong = new System.Windows.Forms.TextBox();
            this.btnLong2Dt = new System.Windows.Forms.Button();
            this.txtDtTime = new System.Windows.Forms.TextBox();
            this.grpTime = new System.Windows.Forms.GroupBox();
            this.grpPrecision = new System.Windows.Forms.GroupBox();
            this.rdbMicroSecond = new System.Windows.Forms.RadioButton();
            this.rdbSecond = new System.Windows.Forms.RadioButton();
            this.grpZone = new System.Windows.Forms.GroupBox();
            this.rdbUniversal = new System.Windows.Forms.RadioButton();
            this.rdbLocal = new System.Windows.Forms.RadioButton();
            this.grpTime.SuspendLayout();
            this.grpPrecision.SuspendLayout();
            this.grpZone.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtDtLong
            // 
            this.txtDtLong.Location = new System.Drawing.Point(13, 156);
            this.txtDtLong.Name = "txtDtLong";
            this.txtDtLong.Size = new System.Drawing.Size(213, 25);
            this.txtDtLong.TabIndex = 0;
            // 
            // btnLong2Dt
            // 
            this.btnLong2Dt.Location = new System.Drawing.Point(252, 156);
            this.btnLong2Dt.Name = "btnLong2Dt";
            this.btnLong2Dt.Size = new System.Drawing.Size(102, 23);
            this.btnLong2Dt.TabIndex = 1;
            this.btnLong2Dt.Text = "Convert";
            this.btnLong2Dt.UseVisualStyleBackColor = true;
            this.btnLong2Dt.Click += new System.EventHandler(this.btnLong2Dt_Click);
            // 
            // txtDtTime
            // 
            this.txtDtTime.Location = new System.Drawing.Point(13, 201);
            this.txtDtTime.Name = "txtDtTime";
            this.txtDtTime.ReadOnly = true;
            this.txtDtTime.Size = new System.Drawing.Size(341, 25);
            this.txtDtTime.TabIndex = 2;
            // 
            // grpTime
            // 
            this.grpTime.Controls.Add(this.grpZone);
            this.grpTime.Controls.Add(this.grpPrecision);
            this.grpTime.Controls.Add(this.btnLong2Dt);
            this.grpTime.Controls.Add(this.txtDtTime);
            this.grpTime.Controls.Add(this.txtDtLong);
            this.grpTime.Location = new System.Drawing.Point(14, 21);
            this.grpTime.Name = "grpTime";
            this.grpTime.Size = new System.Drawing.Size(448, 258);
            this.grpTime.TabIndex = 4;
            this.grpTime.TabStop = false;
            this.grpTime.Text = "Linux时间戳(1970-1-1到现在时间差)";
            // 
            // grpPrecision
            // 
            this.grpPrecision.Controls.Add(this.rdbSecond);
            this.grpPrecision.Controls.Add(this.rdbMicroSecond);
            this.grpPrecision.Location = new System.Drawing.Point(13, 28);
            this.grpPrecision.Name = "grpPrecision";
            this.grpPrecision.Size = new System.Drawing.Size(157, 101);
            this.grpPrecision.TabIndex = 3;
            this.grpPrecision.TabStop = false;
            this.grpPrecision.Text = "精度";
            // 
            // rdbMicroSecond
            // 
            this.rdbMicroSecond.AutoSize = true;
            this.rdbMicroSecond.Checked = true;
            this.rdbMicroSecond.Location = new System.Drawing.Point(18, 25);
            this.rdbMicroSecond.Name = "rdbMicroSecond";
            this.rdbMicroSecond.Size = new System.Drawing.Size(58, 19);
            this.rdbMicroSecond.TabIndex = 0;
            this.rdbMicroSecond.TabStop = true;
            this.rdbMicroSecond.Text = "微秒";
            this.rdbMicroSecond.UseVisualStyleBackColor = true;
            // 
            // rdbSecond
            // 
            this.rdbSecond.AutoSize = true;
            this.rdbSecond.Location = new System.Drawing.Point(18, 63);
            this.rdbSecond.Name = "rdbSecond";
            this.rdbSecond.Size = new System.Drawing.Size(43, 19);
            this.rdbSecond.TabIndex = 1;
            this.rdbSecond.Text = "秒";
            this.rdbSecond.UseVisualStyleBackColor = true;
            // 
            // grpZone
            // 
            this.grpZone.Controls.Add(this.rdbUniversal);
            this.grpZone.Controls.Add(this.rdbLocal);
            this.grpZone.Location = new System.Drawing.Point(203, 28);
            this.grpZone.Name = "grpZone";
            this.grpZone.Size = new System.Drawing.Size(215, 101);
            this.grpZone.TabIndex = 4;
            this.grpZone.TabStop = false;
            this.grpZone.Text = "时区";
            // 
            // rdbUniversal
            // 
            this.rdbUniversal.AutoSize = true;
            this.rdbUniversal.Location = new System.Drawing.Point(18, 63);
            this.rdbUniversal.Name = "rdbUniversal";
            this.rdbUniversal.Size = new System.Drawing.Size(128, 19);
            this.rdbUniversal.TabIndex = 1;
            this.rdbUniversal.Text = "协调时间(UTC)";
            this.rdbUniversal.UseVisualStyleBackColor = true;
            // 
            // rdbLocal
            // 
            this.rdbLocal.AutoSize = true;
            this.rdbLocal.Checked = true;
            this.rdbLocal.Location = new System.Drawing.Point(18, 25);
            this.rdbLocal.Name = "rdbLocal";
            this.rdbLocal.Size = new System.Drawing.Size(58, 19);
            this.rdbLocal.TabIndex = 0;
            this.rdbLocal.TabStop = true;
            this.rdbLocal.Text = "本地";
            this.rdbLocal.UseVisualStyleBackColor = true;
            // 
            // FrmDateTime
            // 
            this.AcceptButton = this.btnLong2Dt;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 291);
            this.Controls.Add(this.grpTime);
            this.Name = "FrmDateTime";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DateTime Convert";
            this.grpTime.ResumeLayout(false);
            this.grpTime.PerformLayout();
            this.grpPrecision.ResumeLayout(false);
            this.grpPrecision.PerformLayout();
            this.grpZone.ResumeLayout(false);
            this.grpZone.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtDtLong;
        private System.Windows.Forms.Button btnLong2Dt;
        private System.Windows.Forms.TextBox txtDtTime;
        private System.Windows.Forms.GroupBox grpTime;
        private System.Windows.Forms.GroupBox grpZone;
        private System.Windows.Forms.RadioButton rdbUniversal;
        private System.Windows.Forms.RadioButton rdbLocal;
        private System.Windows.Forms.GroupBox grpPrecision;
        private System.Windows.Forms.RadioButton rdbSecond;
        private System.Windows.Forms.RadioButton rdbMicroSecond;
    }
}

