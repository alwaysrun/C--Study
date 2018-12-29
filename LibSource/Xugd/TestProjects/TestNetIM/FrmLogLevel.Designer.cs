namespace TestForm
{
    partial class FrmLogLevel
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
            this.btnSet = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdbtnError = new System.Windows.Forms.RadioButton();
            this.rdbtnInfo = new System.Windows.Forms.RadioButton();
            this.rdbtnDebug = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSet
            // 
            this.btnSet.Location = new System.Drawing.Point(94, 185);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(102, 29);
            this.btnSet.TabIndex = 0;
            this.btnSet.Text = "Set";
            this.btnSet.UseVisualStyleBackColor = true;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdbtnDebug);
            this.groupBox1.Controls.Add(this.rdbtnInfo);
            this.groupBox1.Controls.Add(this.rdbtnError);
            this.groupBox1.Location = new System.Drawing.Point(29, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(265, 149);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Log-Level";
            // 
            // rdbtnError
            // 
            this.rdbtnError.AutoSize = true;
            this.rdbtnError.Location = new System.Drawing.Point(25, 25);
            this.rdbtnError.Name = "rdbtnError";
            this.rdbtnError.Size = new System.Drawing.Size(68, 19);
            this.rdbtnError.TabIndex = 0;
            this.rdbtnError.TabStop = true;
            this.rdbtnError.Text = "Error";
            this.rdbtnError.UseVisualStyleBackColor = true;
            // 
            // rdbtnInfo
            // 
            this.rdbtnInfo.AutoSize = true;
            this.rdbtnInfo.Location = new System.Drawing.Point(25, 59);
            this.rdbtnInfo.Name = "rdbtnInfo";
            this.rdbtnInfo.Size = new System.Drawing.Size(108, 19);
            this.rdbtnInfo.TabIndex = 0;
            this.rdbtnInfo.TabStop = true;
            this.rdbtnInfo.Text = "Infomation";
            this.rdbtnInfo.UseVisualStyleBackColor = true;
            // 
            // rdbtnDebug
            // 
            this.rdbtnDebug.AutoSize = true;
            this.rdbtnDebug.Checked = true;
            this.rdbtnDebug.Location = new System.Drawing.Point(25, 96);
            this.rdbtnDebug.Name = "rdbtnDebug";
            this.rdbtnDebug.Size = new System.Drawing.Size(68, 19);
            this.rdbtnDebug.TabIndex = 0;
            this.rdbtnDebug.TabStop = true;
            this.rdbtnDebug.Text = "Debug";
            this.rdbtnDebug.UseVisualStyleBackColor = true;
            // 
            // FrmLogLevel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 232);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSet);
            this.Name = "FrmLogLevel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FrmLogLevel";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSet;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdbtnDebug;
        private System.Windows.Forms.RadioButton rdbtnInfo;
        private System.Windows.Forms.RadioButton rdbtnError;
    }
}