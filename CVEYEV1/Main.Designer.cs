namespace CVEYEV1
{
    partial class CVEye
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
            this.components = new System.ComponentModel.Container();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.paintingPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.paintingConditionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.imageProcessingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.cameraCalibrationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.pattern_field = new System.Windows.Forms.PictureBox();
            this.Detect_items = new System.Windows.Forms.Button();
            this.Template = new System.Windows.Forms.PictureBox();
            this.num_of_items = new System.Windows.Forms.TextBox();
            this.elapsed_time = new System.Windows.Forms.TextBox();
            this.tmp_item_name = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.item_color = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.painting_times = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.Outside = new System.Windows.Forms.RadioButton();
            this.Inside = new System.Windows.Forms.RadioButton();
            this.run = new System.Windows.Forms.Button();
            this.Reset = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Progress = new System.Windows.Forms.ProgressBar();
            this.data_mor = new System.Windows.Forms.ListBox();
            this.status_label = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ledZ = new System.Windows.Forms.PictureBox();
            this.ledY = new System.Windows.Forms.PictureBox();
            this.ledX = new System.Windows.Forms.PictureBox();
            this.EnableEfd = new System.Windows.Forms.CheckBox();
            this.GotoHome = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.feedrateDRO = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.TurnPiston = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.valveNum = new System.Windows.Forms.Label();
            this.zDRO = new System.Windows.Forms.Label();
            this.yDRO = new System.Windows.Forms.Label();
            this.xDRO = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ReplacePosition = new System.Windows.Forms.Button();
            this.RefAllHome = new System.Windows.Forms.Button();
            this.TestValve = new System.Windows.Forms.Button();
            this.timerDROupdate = new System.Windows.Forms.Timer(this.components);
            this.machStatus = new System.Windows.Forms.Label();
            this.lockCylinder = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pattern_field)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Template)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ledZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ledY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ledX)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(9, 21);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(104, 30);
            this.button2.TabIndex = 8;
            this.button2.Text = "Chụp ảnh";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Capture_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(119, 21);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 30);
            this.button1.TabIndex = 1;
            this.button1.Text = "Bật camera";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Video_Click);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "Số mẫu tìm được:";
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(3, 37);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(96, 16);
            this.label9.TabIndex = 8;
            this.label9.Text = "Thời gian xử lý:";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(39, 21);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(127, 22);
            this.toolStripMenuItem2.Text = "Browse...";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.Browse_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(124, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.paintingPointToolStripMenuItem,
            this.paintingConditionToolStripMenuItem,
            this.toolStripSeparator2,
            this.imageProcessingToolStripMenuItem,
            this.toolStripSeparator3,
            this.cameraCalibrationToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(58, 21);
            this.toolsToolStripMenuItem.Text = "Config";
            // 
            // paintingPointToolStripMenuItem
            // 
            this.paintingPointToolStripMenuItem.Name = "paintingPointToolStripMenuItem";
            this.paintingPointToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.paintingPointToolStripMenuItem.Text = "Painting Points";
            this.paintingPointToolStripMenuItem.Click += new System.EventHandler(this.paintingPointToolStripMenuItem_Click);
            // 
            // paintingConditionToolStripMenuItem
            // 
            this.paintingConditionToolStripMenuItem.Name = "paintingConditionToolStripMenuItem";
            this.paintingConditionToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.paintingConditionToolStripMenuItem.Text = "Painting Condition";
            this.paintingConditionToolStripMenuItem.Click += new System.EventHandler(this.paintingConditionToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(185, 6);
            // 
            // imageProcessingToolStripMenuItem
            // 
            this.imageProcessingToolStripMenuItem.Name = "imageProcessingToolStripMenuItem";
            this.imageProcessingToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.imageProcessingToolStripMenuItem.Text = "Image Processing";
            this.imageProcessingToolStripMenuItem.Click += new System.EventHandler(this.imageProcessingToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(185, 6);
            // 
            // cameraCalibrationToolStripMenuItem
            // 
            this.cameraCalibrationToolStripMenuItem.Name = "cameraCalibrationToolStripMenuItem";
            this.cameraCalibrationToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.cameraCalibrationToolStripMenuItem.Text = "Camera Calibration";
            this.cameraCalibrationToolStripMenuItem.Click += new System.EventHandler(this.cameraCalibrationToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manualToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(47, 21);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // manualToolStripMenuItem
            // 
            this.manualToolStripMenuItem.Name = "manualToolStripMenuItem";
            this.manualToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.manualToolStripMenuItem.Text = "Manual";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1350, 25);
            this.menuStrip1.TabIndex = 28;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // pattern_field
            // 
            this.pattern_field.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pattern_field.Location = new System.Drawing.Point(12, 36);
            this.pattern_field.Margin = new System.Windows.Forms.Padding(0);
            this.pattern_field.Name = "pattern_field";
            this.pattern_field.Size = new System.Drawing.Size(880, 660);
            this.pattern_field.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pattern_field.TabIndex = 56;
            this.pattern_field.TabStop = false;
            // 
            // Detect_items
            // 
            this.Detect_items.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Detect_items.Location = new System.Drawing.Point(9, 326);
            this.Detect_items.Name = "Detect_items";
            this.Detect_items.Size = new System.Drawing.Size(210, 30);
            this.Detect_items.TabIndex = 58;
            this.Detect_items.Text = "Quét ảnh";
            this.Detect_items.UseVisualStyleBackColor = true;
            this.Detect_items.Click += new System.EventHandler(this.DetectClick);
            // 
            // Template
            // 
            this.Template.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.Template.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Template.Location = new System.Drawing.Point(6, 78);
            this.Template.Name = "Template";
            this.Template.Size = new System.Drawing.Size(216, 216);
            this.Template.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Template.TabIndex = 0;
            this.Template.TabStop = false;
            // 
            // num_of_items
            // 
            this.num_of_items.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.num_of_items.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.num_of_items.Location = new System.Drawing.Point(138, 4);
            this.num_of_items.Name = "num_of_items";
            this.num_of_items.ReadOnly = true;
            this.num_of_items.Size = new System.Drawing.Size(66, 22);
            this.num_of_items.TabIndex = 66;
            // 
            // elapsed_time
            // 
            this.elapsed_time.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.elapsed_time.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.elapsed_time.Location = new System.Drawing.Point(138, 34);
            this.elapsed_time.Name = "elapsed_time";
            this.elapsed_time.ReadOnly = true;
            this.elapsed_time.Size = new System.Drawing.Size(66, 22);
            this.elapsed_time.TabIndex = 70;
            // 
            // tmp_item_name
            // 
            this.tmp_item_name.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tmp_item_name.BackColor = System.Drawing.SystemColors.Window;
            this.tmp_item_name.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tmp_item_name.FormattingEnabled = true;
            this.tmp_item_name.Items.AddRange(new object[] {
            "Tướng 01",
            "Sĩ 01",
            "Tượng 01",
            "Xe 01",
            "Pháo 01",
            "Ngựa 01",
            "Chốt 01",
            "Tướng 02",
            "Sĩ 02",
            "Tượng 02",
            "Xe 02",
            "Pháo 02",
            "Ngựa 02",
            "Chốt 02"});
            this.tmp_item_name.Location = new System.Drawing.Point(3, 31);
            this.tmp_item_name.Name = "tmp_item_name";
            this.tmp_item_name.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tmp_item_name.Size = new System.Drawing.Size(132, 24);
            this.tmp_item_name.TabIndex = 65;
            this.tmp_item_name.Text = "Tướng 01";
            this.tmp_item_name.TextChanged += new System.EventHandler(this.Name_Changed);
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(141, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 16);
            this.label6.TabIndex = 67;
            this.label6.Text = "Màu:";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 16);
            this.label5.TabIndex = 66;
            this.label5.Text = "Tên:";
            // 
            // item_color
            // 
            this.item_color.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.item_color.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.item_color.FormattingEnabled = true;
            this.item_color.Items.AddRange(new object[] {
            "Đỏ",
            "Đen"});
            this.item_color.Location = new System.Drawing.Point(141, 31);
            this.item_color.Name = "item_color";
            this.item_color.Size = new System.Drawing.Size(72, 24);
            this.item_color.TabIndex = 68;
            this.item_color.Text = "Đỏ";
            this.item_color.TextChanged += new System.EventHandler(this.ColorChanged);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63.88889F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.11111F));
            this.tableLayoutPanel2.Controls.Add(this.item_color, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label5, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label6, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.tmp_item_name, 0, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(6, 17);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(216, 56);
            this.tableLayoutPanel2.TabIndex = 69;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel4);
            this.groupBox1.Controls.Add(this.Outside);
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Controls.Add(this.Template);
            this.groupBox1.Controls.Add(this.Inside);
            this.groupBox1.Controls.Add(this.Detect_items);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(907, 108);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 465);
            this.groupBox1.TabIndex = 65;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Thông tin mẫu";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 62.03209F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37.96791F));
            this.tableLayoutPanel4.Controls.Add(this.painting_times, 1, 2);
            this.tableLayoutPanel4.Controls.Add(this.label17, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.elapsed_time, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.label9, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.num_of_items, 1, 0);
            this.tableLayoutPanel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel4.Location = new System.Drawing.Point(6, 367);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(212, 90);
            this.tableLayoutPanel4.TabIndex = 78;
            // 
            // painting_times
            // 
            this.painting_times.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.painting_times.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.painting_times.Location = new System.Drawing.Point(138, 64);
            this.painting_times.Name = "painting_times";
            this.painting_times.ReadOnly = true;
            this.painting_times.Size = new System.Drawing.Size(66, 22);
            this.painting_times.TabIndex = 72;
            this.painting_times.Visible = false;
            // 
            // label17
            // 
            this.label17.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(3, 67);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(92, 16);
            this.label17.TabIndex = 71;
            this.label17.Text = "Thời gian sơn:";
            this.label17.Visible = false;
            // 
            // Outside
            // 
            this.Outside.AutoSize = true;
            this.Outside.Location = new System.Drawing.Point(102, 300);
            this.Outside.Name = "Outside";
            this.Outside.Size = new System.Drawing.Size(78, 20);
            this.Outside.TabIndex = 80;
            this.Outside.Text = "Sơn viền";
            this.Outside.UseVisualStyleBackColor = true;
            // 
            // Inside
            // 
            this.Inside.AutoSize = true;
            this.Inside.Checked = true;
            this.Inside.Location = new System.Drawing.Point(18, 300);
            this.Inside.Name = "Inside";
            this.Inside.Size = new System.Drawing.Size(74, 20);
            this.Inside.TabIndex = 79;
            this.Inside.TabStop = true;
            this.Inside.Text = "Sơn chữ";
            this.Inside.UseVisualStyleBackColor = true;
            // 
            // run
            // 
            this.run.BackColor = System.Drawing.Color.Lime;
            this.run.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.run.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.run.Location = new System.Drawing.Point(907, 668);
            this.run.Name = "run";
            this.run.Size = new System.Drawing.Size(230, 50);
            this.run.TabIndex = 67;
            this.run.Text = "RUN";
            this.run.UseVisualStyleBackColor = false;
            this.run.Click += new System.EventHandler(this.Run_Click);
            // 
            // Reset
            // 
            this.Reset.BackColor = System.Drawing.Color.Red;
            this.Reset.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Reset.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Reset.Location = new System.Drawing.Point(1143, 668);
            this.Reset.Name = "Reset";
            this.Reset.Size = new System.Drawing.Size(195, 50);
            this.Reset.TabIndex = 68;
            this.Reset.Text = "RESET";
            this.Reset.UseVisualStyleBackColor = false;
            this.Reset.Click += new System.EventHandler(this.ResetClick);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(907, 36);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(230, 66);
            this.groupBox3.TabIndex = 71;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Điều khiển camera";
            // 
            // Progress
            // 
            this.Progress.Location = new System.Drawing.Point(601, 704);
            this.Progress.Name = "Progress";
            this.Progress.Size = new System.Drawing.Size(280, 22);
            this.Progress.TabIndex = 72;
            // 
            // data_mor
            // 
            this.data_mor.FormattingEnabled = true;
            this.data_mor.Location = new System.Drawing.Point(396, 627);
            this.data_mor.Name = "data_mor";
            this.data_mor.Size = new System.Drawing.Size(496, 69);
            this.data_mor.TabIndex = 73;
            this.data_mor.Visible = false;
            // 
            // status_label
            // 
            this.status_label.AutoSize = true;
            this.status_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status_label.Location = new System.Drawing.Point(9, 707);
            this.status_label.Name = "status_label";
            this.status_label.Size = new System.Drawing.Size(49, 16);
            this.status_label.TabIndex = 74;
            this.status_label.Text = "Ready";
            // 
            // groupBox4
            // 
            this.groupBox4.Location = new System.Drawing.Point(12, 700);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(880, 2);
            this.groupBox4.TabIndex = 76;
            this.groupBox4.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ledZ);
            this.groupBox2.Controls.Add(this.ledY);
            this.groupBox2.Controls.Add(this.ledX);
            this.groupBox2.Controls.Add(this.EnableEfd);
            this.groupBox2.Controls.Add(this.GotoHome);
            this.groupBox2.Controls.Add(this.groupBox7);
            this.groupBox2.Controls.Add(this.groupBox6);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.tableLayoutPanel3);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.TurnPiston);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.tableLayoutPanel1);
            this.groupBox2.Controls.Add(this.ReplacePosition);
            this.groupBox2.Controls.Add(this.RefAllHome);
            this.groupBox2.Controls.Add(this.TestValve);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(1143, 36);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(195, 626);
            this.groupBox2.TabIndex = 72;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Điều khiển bàn máy";
            // 
            // ledZ
            // 
            this.ledZ.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ledZ.Location = new System.Drawing.Point(5, 201);
            this.ledZ.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.ledZ.Name = "ledZ";
            this.ledZ.Size = new System.Drawing.Size(20, 40);
            this.ledZ.TabIndex = 87;
            this.ledZ.TabStop = false;
            // 
            // ledY
            // 
            this.ledY.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ledY.Location = new System.Drawing.Point(5, 161);
            this.ledY.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.ledY.Name = "ledY";
            this.ledY.Size = new System.Drawing.Size(20, 40);
            this.ledY.TabIndex = 86;
            this.ledY.TabStop = false;
            // 
            // ledX
            // 
            this.ledX.BackColor = System.Drawing.SystemColors.Control;
            this.ledX.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ledX.Location = new System.Drawing.Point(5, 121);
            this.ledX.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.ledX.Name = "ledX";
            this.ledX.Size = new System.Drawing.Size(20, 40);
            this.ledX.TabIndex = 79;
            this.ledX.TabStop = false;
            // 
            // EnableEfd
            // 
            this.EnableEfd.AutoSize = true;
            this.EnableEfd.Checked = true;
            this.EnableEfd.CheckState = System.Windows.Forms.CheckState.Checked;
            this.EnableEfd.Location = new System.Drawing.Point(9, 292);
            this.EnableEfd.Name = "EnableEfd";
            this.EnableEfd.Size = new System.Drawing.Size(100, 20);
            this.EnableEfd.TabIndex = 85;
            this.EnableEfd.Text = "Enable EFD";
            this.EnableEfd.UseVisualStyleBackColor = true;
            // 
            // GotoHome
            // 
            this.GotoHome.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.GotoHome.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GotoHome.Location = new System.Drawing.Point(9, 64);
            this.GotoHome.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.GotoHome.Name = "GotoHome";
            this.GotoHome.Size = new System.Drawing.Size(176, 30);
            this.GotoHome.TabIndex = 84;
            this.GotoHome.Text = "Vị trí làm việc";
            this.GotoHome.UseVisualStyleBackColor = false;
            this.GotoHome.Click += new System.EventHandler(this.GotoHome_Click);
            // 
            // groupBox7
            // 
            this.groupBox7.Location = new System.Drawing.Point(9, 471);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(180, 2);
            this.groupBox7.TabIndex = 83;
            this.groupBox7.TabStop = false;
            // 
            // groupBox6
            // 
            this.groupBox6.Location = new System.Drawing.Point(9, 392);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(180, 2);
            this.groupBox6.TabIndex = 82;
            this.groupBox6.TabStop = false;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 481);
            this.label16.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(102, 16);
            this.label16.TabIndex = 81;
            this.label16.Text = "Bảo dưỡng van:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 402);
            this.label15.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(98, 16);
            this.label15.TabIndex = 80;
            this.label15.Text = "Điều khiển van:";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.tableLayoutPanel3.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58.65922F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.34078F));
            this.tableLayoutPanel3.Controls.Add(this.feedrateDRO, 0, 0);
            this.tableLayoutPanel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel3.Location = new System.Drawing.Point(9, 339);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(180, 40);
            this.tableLayoutPanel3.TabIndex = 78;
            // 
            // feedrateDRO
            // 
            this.feedrateDRO.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.feedrateDRO.AutoSize = true;
            this.feedrateDRO.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.feedrateDRO.Location = new System.Drawing.Point(150, 5);
            this.feedrateDRO.Name = "feedrateDRO";
            this.feedrateDRO.Size = new System.Drawing.Size(26, 29);
            this.feedrateDRO.TabIndex = 77;
            this.feedrateDRO.Text = "0";
            this.feedrateDRO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 320);
            this.label14.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(112, 16);
            this.label14.TabIndex = 79;
            this.label14.Text = "Tốc độ (mm/min):";
            // 
            // TurnPiston
            // 
            this.TurnPiston.BackColor = System.Drawing.Color.Gold;
            this.TurnPiston.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.TurnPiston.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TurnPiston.Location = new System.Drawing.Point(9, 550);
            this.TurnPiston.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.TurnPiston.Name = "TurnPiston";
            this.TurnPiston.Size = new System.Drawing.Size(176, 30);
            this.TurnPiston.TabIndex = 72;
            this.TurnPiston.Text = "Hạ piston";
            this.TurnPiston.UseVisualStyleBackColor = false;
            this.TurnPiston.Click += new System.EventHandler(this.TurnPiston_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 102);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 16);
            this.label4.TabIndex = 78;
            this.label4.Text = "Vị trí (mm):";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 61.25F));
            this.tableLayoutPanel1.Controls.Add(this.valveNum, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.zDRO, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.yDRO, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.xDRO, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label12, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 3);
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(28, 121);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(161, 160);
            this.tableLayoutPanel1.TabIndex = 77;
            // 
            // valveNum
            // 
            this.valveNum.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.valveNum.AutoSize = true;
            this.valveNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.valveNum.Location = new System.Drawing.Point(131, 124);
            this.valveNum.Name = "valveNum";
            this.valveNum.Size = new System.Drawing.Size(26, 29);
            this.valveNum.TabIndex = 80;
            this.valveNum.Text = "0";
            this.valveNum.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // zDRO
            // 
            this.zDRO.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.zDRO.AutoSize = true;
            this.zDRO.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.zDRO.Location = new System.Drawing.Point(86, 83);
            this.zDRO.Name = "zDRO";
            this.zDRO.Size = new System.Drawing.Size(71, 29);
            this.zDRO.TabIndex = 79;
            this.zDRO.Text = "0.000";
            this.zDRO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // yDRO
            // 
            this.yDRO.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.yDRO.AutoSize = true;
            this.yDRO.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yDRO.Location = new System.Drawing.Point(86, 44);
            this.yDRO.Name = "yDRO";
            this.yDRO.Size = new System.Drawing.Size(71, 29);
            this.yDRO.TabIndex = 78;
            this.yDRO.Text = "0.000";
            this.yDRO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // xDRO
            // 
            this.xDRO.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.xDRO.AutoSize = true;
            this.xDRO.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xDRO.Location = new System.Drawing.Point(86, 5);
            this.xDRO.Name = "xDRO";
            this.xDRO.Size = new System.Drawing.Size(71, 29);
            this.xDRO.TabIndex = 77;
            this.xDRO.Text = "0.000";
            this.xDRO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(4, 83);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(33, 29);
            this.label12.TabIndex = 76;
            this.label12.Text = "Z:";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "X:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(4, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 29);
            this.label3.TabIndex = 1;
            this.label3.Text = "Y:";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(4, 118);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 41);
            this.label7.TabIndex = 77;
            this.label7.Text = "Van:";
            // 
            // ReplacePosition
            // 
            this.ReplacePosition.BackColor = System.Drawing.Color.Gold;
            this.ReplacePosition.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ReplacePosition.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReplacePosition.Location = new System.Drawing.Point(9, 507);
            this.ReplacePosition.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.ReplacePosition.Name = "ReplacePosition";
            this.ReplacePosition.Size = new System.Drawing.Size(176, 30);
            this.ReplacePosition.TabIndex = 71;
            this.ReplacePosition.Text = "Vị trí bảo dưỡng";
            this.ReplacePosition.UseVisualStyleBackColor = false;
            this.ReplacePosition.Click += new System.EventHandler(this.ChangingPosition_Click);
            // 
            // RefAllHome
            // 
            this.RefAllHome.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.RefAllHome.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RefAllHome.Location = new System.Drawing.Point(9, 21);
            this.RefAllHome.Name = "RefAllHome";
            this.RefAllHome.Size = new System.Drawing.Size(176, 30);
            this.RefAllHome.TabIndex = 70;
            this.RefAllHome.Text = "Thiết lập gốc máy";
            this.RefAllHome.UseVisualStyleBackColor = false;
            this.RefAllHome.Click += new System.EventHandler(this.RefAllHome_Click);
            // 
            // TestValve
            // 
            this.TestValve.BackColor = System.Drawing.Color.Gold;
            this.TestValve.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.TestValve.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TestValve.Location = new System.Drawing.Point(9, 428);
            this.TestValve.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.TestValve.Name = "TestValve";
            this.TestValve.Size = new System.Drawing.Size(176, 30);
            this.TestValve.TabIndex = 69;
            this.TestValve.Text = "Phun thử";
            this.TestValve.UseVisualStyleBackColor = false;
            this.TestValve.Click += new System.EventHandler(this.TestValve_Click);
            // 
            // timerDROupdate
            // 
            this.timerDROupdate.Tick += new System.EventHandler(this.TimerDROupdate_Tick);
            // 
            // machStatus
            // 
            this.machStatus.AutoSize = true;
            this.machStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.machStatus.Location = new System.Drawing.Point(186, 707);
            this.machStatus.Name = "machStatus";
            this.machStatus.Size = new System.Drawing.Size(0, 16);
            this.machStatus.TabIndex = 78;
            // 
            // lockCylinder
            // 
            this.lockCylinder.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lockCylinder.Location = new System.Drawing.Point(907, 612);
            this.lockCylinder.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.lockCylinder.Name = "lockCylinder";
            this.lockCylinder.Size = new System.Drawing.Size(230, 40);
            this.lockCylinder.TabIndex = 81;
            this.lockCylinder.Text = "Khóa khay";
            this.lockCylinder.UseVisualStyleBackColor = true;
            this.lockCylinder.Click += new System.EventHandler(this.lockCylinderClick);
            // 
            // CVEye
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1350, 730);
            this.Controls.Add(this.lockCylinder);
            this.Controls.Add(this.machStatus);
            this.Controls.Add(this.data_mor);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.Progress);
            this.Controls.Add(this.Reset);
            this.Controls.Add(this.status_label);
            this.Controls.Add(this.run);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pattern_field);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "CVEye";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CVEye";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CVEye_Closing);
            this.Load += new System.EventHandler(this.CVEye_Load);
            this.Shown += new System.EventHandler(this.CVEye_Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pattern_field)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Template)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ledZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ledY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ledX)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Detect_items;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ToolStripMenuItem imageProcessingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem paintingPointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manualToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.PictureBox Template;
        private System.Windows.Forms.TextBox elapsed_time;
        private System.Windows.Forms.TextBox num_of_items;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ComboBox item_color;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button run;
        private System.Windows.Forms.Button Reset;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ProgressBar Progress;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ToolStripMenuItem cameraCalibrationToolStripMenuItem;
        public System.Windows.Forms.ComboBox tmp_item_name;
        public System.Windows.Forms.PictureBox pattern_field;
        public System.Windows.Forms.ListBox data_mor;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolStripMenuItem paintingConditionToolStripMenuItem;
        private System.Windows.Forms.Button RefAllHome;
        private System.Windows.Forms.Button TestValve;
        private System.Windows.Forms.Button ReplacePosition;
        private System.Windows.Forms.Button TurnPiston;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label feedrateDRO;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TextBox painting_times;
        private System.Windows.Forms.Timer timerDROupdate;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button GotoHome;
        public System.Windows.Forms.Label status_label;
        private System.Windows.Forms.CheckBox EnableEfd;
        public System.Windows.Forms.RadioButton Inside;
        public System.Windows.Forms.RadioButton Outside;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        public System.Windows.Forms.Label machStatus;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label valveNum;
        private System.Windows.Forms.Label zDRO;
        private System.Windows.Forms.Label yDRO;
        private System.Windows.Forms.Label xDRO;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.PictureBox ledZ;
        private System.Windows.Forms.PictureBox ledY;
        private System.Windows.Forms.PictureBox ledX;
        private System.Windows.Forms.Button lockCylinder;
    }
}

