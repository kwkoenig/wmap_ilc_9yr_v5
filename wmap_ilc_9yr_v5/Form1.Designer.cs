namespace wmap_ilc_9yr_v5
{
    partial class wmap_ilc_9yr_v5
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(wmap_ilc_9yr_v5));
            this.label1 = new System.Windows.Forms.Label();
            this.cbBasePixel = new System.Windows.Forms.ComboBox();
            this.cbScale = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkColor = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.chkRotate = new System.Windows.Forms.CheckBox();
            this.cbNextGrab = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnGrab = new System.Windows.Forms.Button();
            this.btnToggle = new System.Windows.Forms.Button();
            this.lblShowing = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.txtMin = new System.Windows.Forms.TextBox();
            this.txtMax = new System.Windows.Forms.TextBox();
            this.lblMin = new System.Windows.Forms.Label();
            this.lblMax = new System.Windows.Forms.Label();
            this.chkReverse = new System.Windows.Forms.CheckBox();
            this.numericUpDownForN = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cbDiagonals = new System.Windows.Forms.ComboBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bMPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pNGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jPGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label7 = new System.Windows.Forms.Label();
            this.cbFindType = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.nudCol = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.nudRow = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.nudWidth = new System.Windows.Forms.NumericUpDown();
            this.btnGo = new System.Windows.Forms.Button();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.nudHeight = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.btnOverlap = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.nudTolerance = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownForN)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCol)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTolerance)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 30);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Base Pixel:";
            // 
            // cbBasePixel
            // 
            this.cbBasePixel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBasePixel.FormattingEnabled = true;
            this.cbBasePixel.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11"});
            this.cbBasePixel.Location = new System.Drawing.Point(74, 27);
            this.cbBasePixel.Margin = new System.Windows.Forms.Padding(2);
            this.cbBasePixel.Name = "cbBasePixel";
            this.cbBasePixel.Size = new System.Drawing.Size(44, 21);
            this.cbBasePixel.TabIndex = 2;
            this.cbBasePixel.SelectedIndexChanged += new System.EventHandler(this.cbBasePixel_SelectedIndexChanged);
            // 
            // cbScale
            // 
            this.cbScale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbScale.FormattingEnabled = true;
            this.cbScale.Items.AddRange(new object[] {
            "to the Nth",
            "linear",
            "log",
            "exp"});
            this.cbScale.Location = new System.Drawing.Point(226, 26);
            this.cbScale.Margin = new System.Windows.Forms.Padding(2);
            this.cbScale.Name = "cbScale";
            this.cbScale.Size = new System.Drawing.Size(78, 21);
            this.cbScale.TabIndex = 6;
            this.cbScale.SelectedIndexChanged += new System.EventHandler(this.CbScale_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(187, 30);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Scale:";
            // 
            // chkColor
            // 
            this.chkColor.AutoSize = true;
            this.chkColor.Checked = true;
            this.chkColor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkColor.Location = new System.Drawing.Point(135, 29);
            this.chkColor.Margin = new System.Windows.Forms.Padding(2);
            this.chkColor.Name = "chkColor";
            this.chkColor.Size = new System.Drawing.Size(49, 17);
            this.chkColor.TabIndex = 9;
            this.chkColor.Text = "color";
            this.chkColor.UseVisualStyleBackColor = true;
            this.chkColor.CheckedChanged += new System.EventHandler(this.Render_Required);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(207, 64);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 13);
            this.label4.TabIndex = 15;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(8, 113);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(512, 512);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::wmap_ilc_9yr_v5.Properties.Resources.BasePixelGrid;
            this.pictureBox2.Location = new System.Drawing.Point(529, 113);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(518, 518);
            this.pictureBox2.TabIndex = 17;
            this.pictureBox2.TabStop = false;
            // 
            // chkRotate
            // 
            this.chkRotate.AutoSize = true;
            this.chkRotate.Location = new System.Drawing.Point(583, 32);
            this.chkRotate.Name = "chkRotate";
            this.chkRotate.Size = new System.Drawing.Size(79, 17);
            this.chkRotate.TabIndex = 18;
            this.chkRotate.Text = "Rotate 180";
            this.chkRotate.UseVisualStyleBackColor = true;
            this.chkRotate.CheckedChanged += new System.EventHandler(this.chkRotate_CheckedChanged);
            // 
            // cbNextGrab
            // 
            this.cbNextGrab.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbNextGrab.FormattingEnabled = true;
            this.cbNextGrab.Items.AddRange(new object[] {
            "0",
            "1"});
            this.cbNextGrab.Location = new System.Drawing.Point(74, 60);
            this.cbNextGrab.Margin = new System.Windows.Forms.Padding(2);
            this.cbNextGrab.Name = "cbNextGrab";
            this.cbNextGrab.Size = new System.Drawing.Size(44, 21);
            this.cbNextGrab.TabIndex = 20;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 63);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Next Grab:";
            // 
            // btnGrab
            // 
            this.btnGrab.Location = new System.Drawing.Point(135, 58);
            this.btnGrab.Name = "btnGrab";
            this.btnGrab.Size = new System.Drawing.Size(65, 23);
            this.btnGrab.TabIndex = 21;
            this.btnGrab.Text = "grab";
            this.btnGrab.UseVisualStyleBackColor = true;
            this.btnGrab.Click += new System.EventHandler(this.btnGrab_Click);
            // 
            // btnToggle
            // 
            this.btnToggle.Enabled = false;
            this.btnToggle.Location = new System.Drawing.Point(219, 58);
            this.btnToggle.Name = "btnToggle";
            this.btnToggle.Size = new System.Drawing.Size(65, 23);
            this.btnToggle.TabIndex = 22;
            this.btnToggle.Text = "toggle";
            this.btnToggle.UseVisualStyleBackColor = true;
            this.btnToggle.Click += new System.EventHandler(this.btnToggle_Click);
            // 
            // lblShowing
            // 
            this.lblShowing.AutoSize = true;
            this.lblShowing.Location = new System.Drawing.Point(5, 94);
            this.lblShowing.Name = "lblShowing";
            this.lblShowing.Size = new System.Drawing.Size(51, 13);
            this.lblShowing.TabIndex = 23;
            this.lblShowing.Text = "Showing:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnReset);
            this.groupBox2.Controls.Add(this.txtMin);
            this.groupBox2.Controls.Add(this.txtMax);
            this.groupBox2.Controls.Add(this.lblMin);
            this.groupBox2.Controls.Add(this.lblMax);
            this.groupBox2.Location = new System.Drawing.Point(376, 19);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(190, 66);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(132, 14);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(50, 44);
            this.btnReset.TabIndex = 5;
            this.btnReset.Text = "Set to Data";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // txtMin
            // 
            this.txtMin.Location = new System.Drawing.Point(73, 40);
            this.txtMin.Name = "txtMin";
            this.txtMin.Size = new System.Drawing.Size(52, 20);
            this.txtMin.TabIndex = 3;
            this.txtMin.Text = "-0.888";
            this.txtMin.TextChanged += new System.EventHandler(this.txtMin_TextChanged);
            // 
            // txtMax
            // 
            this.txtMax.Location = new System.Drawing.Point(73, 11);
            this.txtMax.Name = "txtMax";
            this.txtMax.Size = new System.Drawing.Size(52, 20);
            this.txtMax.TabIndex = 2;
            this.txtMax.Text = "-0.888";
            this.txtMax.TextChanged += new System.EventHandler(this.txtMax_TextChanged);
            // 
            // lblMin
            // 
            this.lblMin.AutoSize = true;
            this.lblMin.Location = new System.Drawing.Point(7, 42);
            this.lblMin.Name = "lblMin";
            this.lblMin.Size = new System.Drawing.Size(66, 13);
            this.lblMin.TabIndex = 1;
            this.lblMin.Text = "Min (-0.888):";
            // 
            // lblMax
            // 
            this.lblMax.AutoSize = true;
            this.lblMax.Location = new System.Drawing.Point(7, 14);
            this.lblMax.Name = "lblMax";
            this.lblMax.Size = new System.Drawing.Size(66, 13);
            this.lblMax.TabIndex = 0;
            this.lblMax.Text = "Max (0.888):";
            // 
            // chkReverse
            // 
            this.chkReverse.AutoSize = true;
            this.chkReverse.Location = new System.Drawing.Point(583, 62);
            this.chkReverse.Name = "chkReverse";
            this.chkReverse.Size = new System.Drawing.Size(131, 17);
            this.chkReverse.TabIndex = 25;
            this.chkReverse.Text = "Reverse Hot and Cold";
            this.chkReverse.UseVisualStyleBackColor = true;
            this.chkReverse.CheckedChanged += new System.EventHandler(this.chkReverseCheckedChanged);
            // 
            // numericUpDownForN
            // 
            this.numericUpDownForN.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownForN.Location = new System.Drawing.Point(331, 27);
            this.numericUpDownForN.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.numericUpDownForN.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownForN.Name = "numericUpDownForN";
            this.numericUpDownForN.Size = new System.Drawing.Size(37, 20);
            this.numericUpDownForN.TabIndex = 27;
            this.numericUpDownForN.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownForN.ValueChanged += new System.EventHandler(this.NumericUpDownForN_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(311, 31);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(18, 13);
            this.label5.TabIndex = 28;
            this.label5.Text = "N:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(402, 633);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 30;
            this.label6.Text = "Diagonals";
            // 
            // cbDiagonals
            // 
            this.cbDiagonals.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDiagonals.FormattingEnabled = true;
            this.cbDiagonals.Items.AddRange(new object[] {
            "Black",
            "White",
            "Off"});
            this.cbDiagonals.Location = new System.Drawing.Point(460, 629);
            this.cbDiagonals.Margin = new System.Windows.Forms.Padding(2);
            this.cbDiagonals.Name = "cbDiagonals";
            this.cbDiagonals.Size = new System.Drawing.Size(60, 21);
            this.cbDiagonals.TabIndex = 29;
            this.cbDiagonals.SelectedIndexChanged += new System.EventHandler(this.CbDiagonals_SelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1053, 24);
            this.menuStrip1.TabIndex = 31;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveAsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bMPToolStripMenuItem,
            this.pNGToolStripMenuItem,
            this.jPGToolStripMenuItem});
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.saveAsToolStripMenuItem.Text = "Save As";
            // 
            // bMPToolStripMenuItem
            // 
            this.bMPToolStripMenuItem.Name = "bMPToolStripMenuItem";
            this.bMPToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.bMPToolStripMenuItem.Text = "BMP";
            this.bMPToolStripMenuItem.Click += new System.EventHandler(this.BMPToolStripMenuItem_Click);
            // 
            // pNGToolStripMenuItem
            // 
            this.pNGToolStripMenuItem.Name = "pNGToolStripMenuItem";
            this.pNGToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.pNGToolStripMenuItem.Text = "PNG";
            this.pNGToolStripMenuItem.Click += new System.EventHandler(this.PNGToolStripMenuItem_Click);
            // 
            // jPGToolStripMenuItem
            // 
            this.jPGToolStripMenuItem.Name = "jPGToolStripMenuItem";
            this.jPGToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.jPGToolStripMenuItem.Text = "JPG";
            this.jPGToolStripMenuItem.Click += new System.EventHandler(this.JPGToolStripMenuItem_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(766, 31);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 13);
            this.label7.TabIndex = 32;
            this.label7.Text = "Find # of";
            // 
            // cbFindType
            // 
            this.cbFindType.FormattingEnabled = true;
            this.cbFindType.Items.AddRange(new object[] {
            "max color",
            "min color"});
            this.cbFindType.Location = new System.Drawing.Point(816, 28);
            this.cbFindType.Name = "cbFindType";
            this.cbFindType.Size = new System.Drawing.Size(71, 21);
            this.cbFindType.TabIndex = 33;
            this.cbFindType.Text = "max color";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(889, 31);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 13);
            this.label8.TabIndex = 34;
            this.label8.Text = "pixels in";
            // 
            // nudCol
            // 
            this.nudCol.Location = new System.Drawing.Point(934, 56);
            this.nudCol.Maximum = new decimal(new int[] {
            511,
            0,
            0,
            0});
            this.nudCol.Name = "nudCol";
            this.nudCol.Size = new System.Drawing.Size(41, 20);
            this.nudCol.TabIndex = 42;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(910, 59);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(21, 13);
            this.label9.TabIndex = 36;
            this.label9.Text = "col";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(978, 59);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(24, 13);
            this.label10.TabIndex = 38;
            this.label10.Text = "row";
            // 
            // nudRow
            // 
            this.nudRow.Location = new System.Drawing.Point(1005, 56);
            this.nudRow.Maximum = new decimal(new int[] {
            511,
            0,
            0,
            0});
            this.nudRow.Name = "nudRow";
            this.nudRow.Size = new System.Drawing.Size(41, 20);
            this.nudRow.TabIndex = 43;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(981, 31);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(18, 13);
            this.label11.TabIndex = 39;
            this.label11.Text = "by";
            // 
            // nudWidth
            // 
            this.nudWidth.Location = new System.Drawing.Point(935, 28);
            this.nudWidth.Maximum = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.nudWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudWidth.Name = "nudWidth";
            this.nudWidth.Size = new System.Drawing.Size(41, 20);
            this.nudWidth.TabIndex = 40;
            this.nudWidth.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(769, 81);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(51, 23);
            this.btnGo.TabIndex = 44;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.BtnGo_Click);
            // 
            // txtResults
            // 
            this.txtResults.Location = new System.Drawing.Point(828, 83);
            this.txtResults.Name = "txtResults";
            this.txtResults.ReadOnly = true;
            this.txtResults.Size = new System.Drawing.Size(169, 20);
            this.txtResults.TabIndex = 45;
            // 
            // nudHeight
            // 
            this.nudHeight.Location = new System.Drawing.Point(1005, 28);
            this.nudHeight.Maximum = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.nudHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudHeight.Name = "nudHeight";
            this.nudHeight.Size = new System.Drawing.Size(41, 20);
            this.nudHeight.TabIndex = 41;
            this.nudHeight.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(767, 59);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(138, 13);
            this.label12.TabIndex = 44;
            this.label12.Text = "region with top left corner at";
            // 
            // btnOverlap
            // 
            this.btnOverlap.Enabled = false;
            this.btnOverlap.Location = new System.Drawing.Point(303, 58);
            this.btnOverlap.Name = "btnOverlap";
            this.btnOverlap.Size = new System.Drawing.Size(65, 23);
            this.btnOverlap.TabIndex = 46;
            this.btnOverlap.Text = "overlap";
            this.btnOverlap.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(582, 86);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(133, 13);
            this.label13.TabIndex = 47;
            this.label13.Text = "overlap and find tolerance:";
            // 
            // nudTolerance
            // 
            this.nudTolerance.Location = new System.Drawing.Point(718, 83);
            this.nudTolerance.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudTolerance.Name = "nudTolerance";
            this.nudTolerance.Size = new System.Drawing.Size(41, 20);
            this.nudTolerance.TabIndex = 48;
            // 
            // wmap_ilc_9yr_v5
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1053, 658);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.nudTolerance);
            this.Controls.Add(this.btnOverlap);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.nudHeight);
            this.Controls.Add(this.txtResults);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.nudWidth);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.nudRow);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.nudCol);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cbFindType);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbDiagonals);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numericUpDownForN);
            this.Controls.Add(this.chkReverse);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.lblShowing);
            this.Controls.Add(this.btnToggle);
            this.Controls.Add(this.btnGrab);
            this.Controls.Add(this.cbNextGrab);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chkRotate);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.chkColor);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbScale);
            this.Controls.Add(this.cbBasePixel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "wmap_ilc_9yr_v5";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "wmap_ilc_9yr_v5";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownForN)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCol)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTolerance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbBasePixel;
        private System.Windows.Forms.ComboBox cbScale;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkColor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.CheckBox chkRotate;
        private System.Windows.Forms.ComboBox cbNextGrab;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnGrab;
        private System.Windows.Forms.Button btnToggle;
        private System.Windows.Forms.Label lblShowing;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtMax;
        private System.Windows.Forms.Label lblMin;
        private System.Windows.Forms.Label lblMax;
        private System.Windows.Forms.TextBox txtMin;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.CheckBox chkReverse;
        private System.Windows.Forms.NumericUpDown numericUpDownForN;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbDiagonals;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bMPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pNGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jPGToolStripMenuItem;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cbFindType;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown nudCol;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown nudRow;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown nudWidth;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.NumericUpDown nudHeight;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnOverlap;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown nudTolerance;
    }
}











