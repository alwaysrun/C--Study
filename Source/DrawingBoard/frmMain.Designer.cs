namespace XDrawBoard
{
    partial class FrmMain
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
            this.components = new System.ComponentModel.Container();
            this.tmShow = new System.Windows.Forms.Timer(this.components);
            this.tmAutoHidden = new System.Windows.Forms.Timer(this.components);
            this.lblClose = new System.Windows.Forms.Label();
            this.btnPrintScreen = new System.Windows.Forms.Button();
            this.btnCutScreen = new System.Windows.Forms.Button();
            this.btnDrawBoard = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnPencil = new System.Windows.Forms.Button();
            this.btnBeeLine = new System.Windows.Forms.Button();
            this.picBackgroundColor = new System.Windows.Forms.PictureBox();
            this.btnClearAll = new System.Windows.Forms.Button();
            this.picPenColor = new System.Windows.Forms.PictureBox();
            this.ckbScreenBoard = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnRedo = new System.Windows.Forms.Button();
            this.btnEaraser = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numPenSize = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numOpacity = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbEndCap = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cbStartCap = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cbLineStyle = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cbDrawMethod = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.panBoard = new System.Windows.Forms.Panel();
            this.chkTryErase = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.picBackgroundColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPenColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPenSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOpacity)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panBoard.SuspendLayout();
            this.SuspendLayout();
            // 
            // tmShow
            // 
            this.tmShow.Enabled = true;
            this.tmShow.Interval = 10;
            this.tmShow.Tick += new System.EventHandler(this.tmShow_Tick);
            // 
            // tmAutoHidden
            // 
            this.tmAutoHidden.Enabled = true;
            this.tmAutoHidden.Interval = 1000;
            this.tmAutoHidden.Tick += new System.EventHandler(this.tmHidden_Tick);
            // 
            // lblClose
            // 
            this.lblClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblClose.BackColor = System.Drawing.Color.Transparent;
            this.lblClose.Font = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblClose.Location = new System.Drawing.Point(378, 6);
            this.lblClose.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.lblClose.Name = "lblClose";
            this.lblClose.Size = new System.Drawing.Size(16, 15);
            this.lblClose.TabIndex = 0;
            this.lblClose.Text = "×";
            this.lblClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblClose.Click += new System.EventHandler(this.lblClose_Click);
            this.lblClose.MouseEnter += new System.EventHandler(this.lblClose_MouseEnter);
            this.lblClose.MouseLeave += new System.EventHandler(this.lblClose_MouseLeave);
            // 
            // btnPrintScreen
            // 
            this.btnPrintScreen.Location = new System.Drawing.Point(277, 4);
            this.btnPrintScreen.Margin = new System.Windows.Forms.Padding(4);
            this.btnPrintScreen.Name = "btnPrintScreen";
            this.btnPrintScreen.Size = new System.Drawing.Size(66, 40);
            this.btnPrintScreen.TabIndex = 1;
            this.btnPrintScreen.Tag = "";
            this.btnPrintScreen.Text = "全屏\r\n快照";
            this.btnPrintScreen.UseVisualStyleBackColor = true;
            this.btnPrintScreen.Click += new System.EventHandler(this.btnPrintScreen_Click);
            // 
            // btnCutScreen
            // 
            this.btnCutScreen.Location = new System.Drawing.Point(203, 4);
            this.btnCutScreen.Margin = new System.Windows.Forms.Padding(4);
            this.btnCutScreen.Name = "btnCutScreen";
            this.btnCutScreen.Size = new System.Drawing.Size(66, 40);
            this.btnCutScreen.TabIndex = 1;
            this.btnCutScreen.Tag = "";
            this.btnCutScreen.Text = "屏幕 截图";
            this.btnCutScreen.UseVisualStyleBackColor = true;
            this.btnCutScreen.Click += new System.EventHandler(this.btnCutScreen_Click);
            // 
            // btnDrawBoard
            // 
            this.btnDrawBoard.Location = new System.Drawing.Point(8, 4);
            this.btnDrawBoard.Margin = new System.Windows.Forms.Padding(4);
            this.btnDrawBoard.Name = "btnDrawBoard";
            this.btnDrawBoard.Size = new System.Drawing.Size(68, 40);
            this.btnDrawBoard.TabIndex = 7;
            this.btnDrawBoard.Text = "开启  画板";
            this.btnDrawBoard.UseVisualStyleBackColor = true;
            this.btnDrawBoard.Click += new System.EventHandler(this.btnDrawBoard_Click);
            // 
            // btnUndo
            // 
            this.btnUndo.Location = new System.Drawing.Point(244, 46);
            this.btnUndo.Margin = new System.Windows.Forms.Padding(4);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(60, 30);
            this.btnUndo.TabIndex = 1;
            this.btnUndo.Tag = "";
            this.btnUndo.Text = "撤销";
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnPencil
            // 
            this.btnPencil.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPencil.ImageKey = "(无)";
            this.btnPencil.Location = new System.Drawing.Point(10, 46);
            this.btnPencil.Margin = new System.Windows.Forms.Padding(4);
            this.btnPencil.Name = "btnPencil";
            this.btnPencil.Size = new System.Drawing.Size(60, 30);
            this.btnPencil.TabIndex = 1;
            this.btnPencil.Tag = "Pencil";
            this.btnPencil.Text = "画笔";
            this.btnPencil.UseVisualStyleBackColor = true;
            this.btnPencil.Click += new System.EventHandler(this.btnPencil_Click);
            // 
            // btnBeeLine
            // 
            this.btnBeeLine.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBeeLine.ImageKey = "(无)";
            this.btnBeeLine.Location = new System.Drawing.Point(86, 46);
            this.btnBeeLine.Margin = new System.Windows.Forms.Padding(4);
            this.btnBeeLine.Name = "btnBeeLine";
            this.btnBeeLine.Size = new System.Drawing.Size(60, 30);
            this.btnBeeLine.TabIndex = 1;
            this.btnBeeLine.Tag = "Beeline";
            this.btnBeeLine.Text = "直线";
            this.btnBeeLine.UseVisualStyleBackColor = true;
            this.btnBeeLine.Click += new System.EventHandler(this.btnBeeLine_Click);
            // 
            // picBackgroundColor
            // 
            this.picBackgroundColor.BackColor = System.Drawing.Color.White;
            this.picBackgroundColor.Location = new System.Drawing.Point(244, 197);
            this.picBackgroundColor.Margin = new System.Windows.Forms.Padding(1);
            this.picBackgroundColor.Name = "picBackgroundColor";
            this.picBackgroundColor.Size = new System.Drawing.Size(121, 32);
            this.picBackgroundColor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picBackgroundColor.TabIndex = 2;
            this.picBackgroundColor.TabStop = false;
            this.picBackgroundColor.Click += new System.EventHandler(this.picBackgroundColor_Click);
            // 
            // btnClearAll
            // 
            this.btnClearAll.Location = new System.Drawing.Point(315, 15);
            this.btnClearAll.Margin = new System.Windows.Forms.Padding(4);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(60, 30);
            this.btnClearAll.TabIndex = 8;
            this.btnClearAll.Text = "清空";
            this.btnClearAll.UseVisualStyleBackColor = true;
            this.btnClearAll.Click += new System.EventHandler(this.btnClearAll_Click);
            // 
            // picPenColor
            // 
            this.picPenColor.BackColor = System.Drawing.Color.Black;
            this.picPenColor.Location = new System.Drawing.Point(124, 85);
            this.picPenColor.Margin = new System.Windows.Forms.Padding(4);
            this.picPenColor.Name = "picPenColor";
            this.picPenColor.Size = new System.Drawing.Size(89, 25);
            this.picPenColor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPenColor.TabIndex = 5;
            this.picPenColor.TabStop = false;
            this.picPenColor.Click += new System.EventHandler(this.picPenColor_Click);
            // 
            // ckbScreenBoard
            // 
            this.ckbScreenBoard.AutoSize = true;
            this.ckbScreenBoard.Location = new System.Drawing.Point(15, 207);
            this.ckbScreenBoard.Name = "ckbScreenBoard";
            this.ckbScreenBoard.Size = new System.Drawing.Size(119, 19);
            this.ckbScreenBoard.TabIndex = 10;
            this.ckbScreenBoard.Text = "屏幕做为背景";
            this.ckbScreenBoard.UseVisualStyleBackColor = true;
            this.ckbScreenBoard.CheckedChanged += new System.EventHandler(this.ckbScreenBoard_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(155, 207);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 15);
            this.label3.TabIndex = 11;
            this.label3.Text = "背景及预览";
            // 
            // btnRedo
            // 
            this.btnRedo.Location = new System.Drawing.Point(315, 46);
            this.btnRedo.Margin = new System.Windows.Forms.Padding(4);
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.Size = new System.Drawing.Size(60, 30);
            this.btnRedo.TabIndex = 1;
            this.btnRedo.Tag = "";
            this.btnRedo.Text = "恢复";
            this.btnRedo.UseVisualStyleBackColor = true;
            this.btnRedo.Click += new System.EventHandler(this.btnRedo_Click);
            // 
            // btnEaraser
            // 
            this.btnEaraser.Location = new System.Drawing.Point(162, 46);
            this.btnEaraser.Margin = new System.Windows.Forms.Padding(4);
            this.btnEaraser.Name = "btnEaraser";
            this.btnEaraser.Size = new System.Drawing.Size(60, 30);
            this.btnEaraser.TabIndex = 10;
            this.btnEaraser.Tag = "Eraser";
            this.btnEaraser.Text = "橡皮擦";
            this.btnEaraser.UseVisualStyleBackColor = true;
            this.btnEaraser.Click += new System.EventHandler(this.btnEaraser_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 28);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "宽度";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(92, 26);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 19);
            this.label4.TabIndex = 4;
            this.label4.Text = "px;";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 90);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 15);
            this.label2.TabIndex = 12;
            this.label2.Text = "颜色(点击修改)";
            // 
            // numPenSize
            // 
            this.numPenSize.Location = new System.Drawing.Point(49, 23);
            this.numPenSize.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numPenSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPenSize.Name = "numPenSize";
            this.numPenSize.Size = new System.Drawing.Size(47, 25);
            this.numPenSize.TabIndex = 13;
            this.numPenSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numPenSize.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numPenSize.ValueChanged += new System.EventHandler(this.numPenSize_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(226, 90);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 15);
            this.label6.TabIndex = 14;
            this.label6.Text = "透明度";
            // 
            // numOpacity
            // 
            this.numOpacity.Location = new System.Drawing.Point(282, 85);
            this.numOpacity.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numOpacity.Name = "numOpacity";
            this.numOpacity.Size = new System.Drawing.Size(57, 25);
            this.numOpacity.TabIndex = 16;
            this.numOpacity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numOpacity.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numOpacity.ValueChanged += new System.EventHandler(this.numOpacity_ValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.cbDrawMethod);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.btnEaraser);
            this.groupBox1.Controls.Add(this.btnRedo);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.ckbScreenBoard);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.btnClearAll);
            this.groupBox1.Controls.Add(this.btnBeeLine);
            this.groupBox1.Controls.Add(this.btnPencil);
            this.groupBox1.Controls.Add(this.btnUndo);
            this.groupBox1.Controls.Add(this.picBackgroundColor);
            this.groupBox1.Location = new System.Drawing.Point(3, -3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(386, 234);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbEndCap);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.cbStartCap);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.cbLineStyle);
            this.groupBox2.Controls.Add(this.picPenColor);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.numOpacity);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.numPenSize);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(7, 81);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(376, 115);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "线条信息";
            // 
            // cbEndCap
            // 
            this.cbEndCap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEndCap.Enabled = false;
            this.cbEndCap.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbEndCap.FormattingEnabled = true;
            this.cbEndCap.Items.AddRange(new object[] {
            "默认",
            "方锚头帽",
            "圆锚头帽",
            "菱形锚头帽",
            "箭头状锚头帽"});
            this.cbEndCap.Location = new System.Drawing.Point(256, 54);
            this.cbEndCap.Name = "cbEndCap";
            this.cbEndCap.Size = new System.Drawing.Size(115, 23);
            this.cbEndCap.TabIndex = 23;
            this.cbEndCap.DropDown += new System.EventHandler(this.cbEndCap_DropDown);
            this.cbEndCap.SelectedIndexChanged += new System.EventHandler(this.cbEndCap_SelectedIndexChanged);
            this.cbEndCap.DropDownClosed += new System.EventHandler(this.cbEndCap_DropDownClosed);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(185, 59);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(67, 15);
            this.label11.TabIndex = 22;
            this.label11.Text = "终点样式";
            // 
            // cbStartCap
            // 
            this.cbStartCap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStartCap.Enabled = false;
            this.cbStartCap.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbStartCap.FormattingEnabled = true;
            this.cbStartCap.Items.AddRange(new object[] {
            "默认",
            "方锚头帽",
            "圆锚头帽",
            "菱形锚头帽",
            "箭头状锚头帽"});
            this.cbStartCap.Location = new System.Drawing.Point(78, 54);
            this.cbStartCap.Name = "cbStartCap";
            this.cbStartCap.Size = new System.Drawing.Size(106, 23);
            this.cbStartCap.TabIndex = 21;
            this.cbStartCap.DropDown += new System.EventHandler(this.cbStartCap_DropDown);
            this.cbStartCap.SelectedIndexChanged += new System.EventHandler(this.cbStartCap_SelectedIndexChanged);
            this.cbStartCap.DropDownClosed += new System.EventHandler(this.cbStartCap_DropDownClosed);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 58);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(67, 15);
            this.label10.TabIndex = 20;
            this.label10.Text = "起点样式";
            // 
            // cbLineStyle
            // 
            this.cbLineStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLineStyle.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbLineStyle.FormattingEnabled = true;
            this.cbLineStyle.Items.AddRange(new object[] {
            "默认",
            "划线(---)",
            "点线(....)",
            "划线点(_._.)",
            "划线点点(_..)"});
            this.cbLineStyle.Location = new System.Drawing.Point(254, 23);
            this.cbLineStyle.Name = "cbLineStyle";
            this.cbLineStyle.Size = new System.Drawing.Size(114, 23);
            this.cbLineStyle.TabIndex = 19;
            this.cbLineStyle.SelectedIndexChanged += new System.EventHandler(this.cbLineStyle_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(185, 28);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 15);
            this.label9.TabIndex = 4;
            this.label9.Text = "线条样式";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(212, 88);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(19, 19);
            this.label8.TabIndex = 18;
            this.label8.Text = ";";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(340, 88);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 19);
            this.label5.TabIndex = 4;
            this.label5.Text = "%";
            // 
            // cbDrawMethod
            // 
            this.cbDrawMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDrawMethod.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbDrawMethod.FormattingEnabled = true;
            this.cbDrawMethod.Items.AddRange(new object[] {
            "画笔",
            "直线",
            "矩形框",
            "椭圆框",
            "橡皮擦",
            "填充矩形",
            "填充椭圆"});
            this.cbDrawMethod.Location = new System.Drawing.Point(85, 17);
            this.cbDrawMethod.Name = "cbDrawMethod";
            this.cbDrawMethod.Size = new System.Drawing.Size(117, 23);
            this.cbDrawMethod.TabIndex = 17;
            this.cbDrawMethod.SelectedIndexChanged += new System.EventHandler(this.cbDrawMethod_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 22);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 15);
            this.label7.TabIndex = 4;
            this.label7.Text = "画图工具：";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(244, 14);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(60, 30);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // panBoard
            // 
            this.panBoard.BackColor = System.Drawing.Color.Transparent;
            this.panBoard.Controls.Add(this.groupBox1);
            this.panBoard.Location = new System.Drawing.Point(5, 42);
            this.panBoard.Name = "panBoard";
            this.panBoard.Size = new System.Drawing.Size(395, 230);
            this.panBoard.TabIndex = 12;
            // 
            // chkTryErase
            // 
            this.chkTryErase.AutoSize = true;
            this.chkTryErase.Location = new System.Drawing.Point(79, 23);
            this.chkTryErase.Name = "chkTryErase";
            this.chkTryErase.Size = new System.Drawing.Size(74, 19);
            this.chkTryErase.TabIndex = 13;
            this.chkTryErase.Text = "擦擦看";
            this.chkTryErase.UseVisualStyleBackColor = true;
            this.chkTryErase.CheckedChanged += new System.EventHandler(this.chkTryErase_CheckedChanged);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(400, 275);
            this.Controls.Add(this.chkTryErase);
            this.Controls.Add(this.btnDrawBoard);
            this.Controls.Add(this.btnCutScreen);
            this.Controls.Add(this.btnPrintScreen);
            this.Controls.Add(this.lblClose);
            this.Controls.Add(this.panBoard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmMain";
            this.Opacity = 0.95D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "WhiteBoard";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.MouseEnter += new System.EventHandler(this.frmMain_MouseEnter);
            ((System.ComponentModel.ISupportInitialize)(this.picBackgroundColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPenColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPenSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOpacity)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panBoard.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer tmShow;
        private System.Windows.Forms.Timer tmAutoHidden;
        private System.Windows.Forms.Label lblClose;
        private System.Windows.Forms.Button btnPrintScreen;
        private System.Windows.Forms.Button btnCutScreen;
        private System.Windows.Forms.Button btnDrawBoard;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Button btnPencil;
        private System.Windows.Forms.Button btnBeeLine;
        private System.Windows.Forms.PictureBox picBackgroundColor;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.PictureBox picPenColor;
        private System.Windows.Forms.CheckBox ckbScreenBoard;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRedo;
        private System.Windows.Forms.Button btnEaraser;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numPenSize;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numOpacity;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panBoard;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox cbDrawMethod;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cbLineStyle;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbEndCap;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cbStartCap;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox chkTryErase;
    }
}

