namespace CVEYEV1
{
    partial class ConfigPaintingPoints
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.templateImg = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.item_color = new System.Windows.Forms.ComboBox();
            this.item_name = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.inject_time = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Data_Grid = new System.Windows.Forms.DataGridView();
            this.PointNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Color = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.item_height = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.needle_dia = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.x_lb = new System.Windows.Forms.Label();
            this.y_lb = new System.Windows.Forms.Label();
            this.Cancel = new System.Windows.Forms.Button();
            this.OK = new System.Windows.Forms.Button();
            this.SaveDatabase = new System.Windows.Forms.Button();
            this.ClearTracking = new System.Windows.Forms.Button();
            this.x_pos_lb = new System.Windows.Forms.TextBox();
            this.y_pos_lb = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.status = new System.Windows.Forms.ToolStripStatusLabel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.templateImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inject_time)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Data_Grid)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.item_height)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.needle_dia)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // templateImg
            // 
            this.templateImg.Cursor = System.Windows.Forms.Cursors.Default;
            this.templateImg.Location = new System.Drawing.Point(12, 31);
            this.templateImg.Name = "templateImg";
            this.templateImg.Size = new System.Drawing.Size(430, 430);
            this.templateImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.templateImg.TabIndex = 69;
            this.templateImg.TabStop = false;
            this.templateImg.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Template_Click);
            this.templateImg.MouseEnter += new System.EventHandler(this.pattern_field_MouseEnter);
            this.templateImg.MouseLeave += new System.EventHandler(this.pattern_field_MouseLeave);
            this.templateImg.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pattern_field_MouseMove);
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 16);
            this.label5.TabIndex = 71;
            this.label5.Text = "Name:";
            // 
            // item_color
            // 
            this.item_color.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.item_color.FormattingEnabled = true;
            this.item_color.Items.AddRange(new object[] {
            "Đỏ",
            "Đen"});
            this.item_color.Location = new System.Drawing.Point(103, 33);
            this.item_color.Name = "item_color";
            this.item_color.Size = new System.Drawing.Size(94, 24);
            this.item_color.TabIndex = 73;
            this.item_color.Text = "Đỏ";
            // 
            // item_name
            // 
            this.item_name.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.item_name.FormattingEnabled = true;
            this.item_name.Items.AddRange(new object[] {
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
            this.item_name.Location = new System.Drawing.Point(103, 3);
            this.item_name.Name = "item_name";
            this.item_name.Size = new System.Drawing.Size(94, 24);
            this.item_name.TabIndex = 70;
            this.item_name.Text = "Tướng 01";
            this.item_name.SelectedIndexChanged += new System.EventHandler(this.ItemNameChange);
            this.item_name.TextChanged += new System.EventHandler(this.Name_Changed);
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(3, 37);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 16);
            this.label6.TabIndex = 72;
            this.label6.Text = "Color:";
            // 
            // inject_time
            // 
            this.inject_time.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inject_time.Location = new System.Drawing.Point(313, 3);
            this.inject_time.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.inject_time.Name = "inject_time";
            this.inject_time.Size = new System.Drawing.Size(94, 22);
            this.inject_time.TabIndex = 74;
            this.inject_time.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(203, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 16);
            this.label1.TabIndex = 75;
            this.label1.Text = "Injecting Time:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 16);
            this.label3.TabIndex = 77;
            this.label3.Text = "Item height:";
            // 
            // Data_Grid
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Data_Grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.Data_Grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Data_Grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PointNum,
            this.colX,
            this.colY,
            this.colC,
            this.colT,
            this.Color});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Data_Grid.DefaultCellStyle = dataGridViewCellStyle2;
            this.Data_Grid.Location = new System.Drawing.Point(451, 31);
            this.Data_Grid.Name = "Data_Grid";
            this.Data_Grid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.Data_Grid.Size = new System.Drawing.Size(400, 580);
            this.Data_Grid.TabIndex = 77;
            this.Data_Grid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.CellValueChanged);
            this.Data_Grid.Click += new System.EventHandler(this.CellClick);
            this.Data_Grid.MouseEnter += new System.EventHandler(this.Data_Grid_MouseEnter);
            // 
            // PointNum
            // 
            this.PointNum.HeaderText = "";
            this.PointNum.Name = "PointNum";
            this.PointNum.Width = 25;
            // 
            // colX
            // 
            this.colX.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colX.FillWeight = 120F;
            this.colX.HeaderText = "X";
            this.colX.Name = "colX";
            this.colX.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // colY
            // 
            this.colY.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colY.FillWeight = 120F;
            this.colY.HeaderText = "Y";
            this.colY.Name = "colY";
            // 
            // colC
            // 
            this.colC.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colC.FillWeight = 120F;
            this.colC.HeaderText = "Z";
            this.colC.Name = "colC";
            // 
            // colT
            // 
            this.colT.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colT.FillWeight = 120F;
            this.colT.HeaderText = "Time";
            this.colT.Name = "colT";
            // 
            // Color
            // 
            this.Color.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Color.HeaderText = "Color";
            this.Color.Name = "Color";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(448, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 16);
            this.label2.TabIndex = 76;
            this.label2.Text = "Dispensing Data:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 16);
            this.label4.TabIndex = 78;
            this.label4.Text = "Template:";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(3, 67);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(66, 16);
            this.label8.TabIndex = 80;
            this.label8.Text = "Diameter:";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.Controls.Add(this.item_height, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.numericUpDown1, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.item_color, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.item_name, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.inject_time, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label1, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label9, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.needle_dia, 3, 1);
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 21);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(410, 120);
            this.tableLayoutPanel1.TabIndex = 80;
            // 
            // item_height
            // 
            this.item_height.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.item_height.Location = new System.Drawing.Point(103, 93);
            this.item_height.Name = "item_height";
            this.item_height.Size = new System.Drawing.Size(94, 22);
            this.item_height.TabIndex = 86;
            this.item_height.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown1.Location = new System.Drawing.Point(103, 63);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(94, 22);
            this.numericUpDown1.TabIndex = 84;
            this.numericUpDown1.Value = new decimal(new int[] {
            31,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(203, 37);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(78, 16);
            this.label9.TabIndex = 82;
            this.label9.Text = "Needle dia:";
            // 
            // needle_dia
            // 
            this.needle_dia.DecimalPlaces = 1;
            this.needle_dia.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.needle_dia.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.needle_dia.Location = new System.Drawing.Point(313, 33);
            this.needle_dia.Name = "needle_dia";
            this.needle_dia.Size = new System.Drawing.Size(94, 22);
            this.needle_dia.TabIndex = 83;
            this.needle_dia.Value = new decimal(new int[] {
            8,
            0,
            0,
            65536});
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 467);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(430, 151);
            this.groupBox1.TabIndex = 81;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Item parameters";
            // 
            // x_lb
            // 
            this.x_lb.AutoSize = true;
            this.x_lb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.x_lb.Location = new System.Drawing.Point(288, 12);
            this.x_lb.Name = "x_lb";
            this.x_lb.Size = new System.Drawing.Size(19, 16);
            this.x_lb.TabIndex = 88;
            this.x_lb.Text = "X:";
            // 
            // y_lb
            // 
            this.y_lb.AutoSize = true;
            this.y_lb.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.y_lb.Location = new System.Drawing.Point(369, 12);
            this.y_lb.Name = "y_lb";
            this.y_lb.Size = new System.Drawing.Size(20, 16);
            this.y_lb.TabIndex = 89;
            this.y_lb.Text = "Y:";
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cancel.Location = new System.Drawing.Point(771, 622);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(80, 29);
            this.Cancel.TabIndex = 93;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // OK
            // 
            this.OK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OK.Location = new System.Drawing.Point(688, 622);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(80, 29);
            this.OK.TabIndex = 97;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // SaveDatabase
            // 
            this.SaveDatabase.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveDatabase.Location = new System.Drawing.Point(236, 622);
            this.SaveDatabase.Name = "SaveDatabase";
            this.SaveDatabase.Size = new System.Drawing.Size(100, 28);
            this.SaveDatabase.TabIndex = 92;
            this.SaveDatabase.Text = "Save";
            this.SaveDatabase.UseVisualStyleBackColor = true;
            this.SaveDatabase.Click += new System.EventHandler(this.SaveDatabase_Click);
            // 
            // ClearTracking
            // 
            this.ClearTracking.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClearTracking.Location = new System.Drawing.Point(342, 622);
            this.ClearTracking.Name = "ClearTracking";
            this.ClearTracking.Size = new System.Drawing.Size(100, 28);
            this.ClearTracking.TabIndex = 95;
            this.ClearTracking.Text = "Clear";
            this.ClearTracking.UseVisualStyleBackColor = true;
            this.ClearTracking.Click += new System.EventHandler(this.ClearTracking_Click);
            // 
            // x_pos_lb
            // 
            this.x_pos_lb.Location = new System.Drawing.Point(310, 8);
            this.x_pos_lb.Name = "x_pos_lb";
            this.x_pos_lb.Size = new System.Drawing.Size(50, 20);
            this.x_pos_lb.TabIndex = 98;
            // 
            // y_pos_lb
            // 
            this.y_pos_lb.Location = new System.Drawing.Point(392, 8);
            this.y_pos_lb.Name = "y_pos_lb";
            this.y_pos_lb.Size = new System.Drawing.Size(50, 20);
            this.y_pos_lb.TabIndex = 99;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status});
            this.statusStrip1.Location = new System.Drawing.Point(0, 660);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(864, 22);
            this.statusStrip1.TabIndex = 100;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // status
            // 
            this.status.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(0, 17);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(462, 575);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(40, 28);
            this.button1.TabIndex = 101;
            this.button1.Text = "-";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(462, 543);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(40, 28);
            this.button2.TabIndex = 102;
            this.button2.Text = "+";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // ConfigPaintingPoints
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(864, 682);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.y_pos_lb);
            this.Controls.Add(this.x_pos_lb);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.SaveDatabase);
            this.Controls.Add(this.ClearTracking);
            this.Controls.Add(this.y_lb);
            this.Controls.Add(this.x_lb);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Data_Grid);
            this.Controls.Add(this.templateImg);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ConfigPaintingPoints";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Config Painting Point";
            this.Load += new System.EventHandler(this.ConfigPaintingPoints_Load);
            ((System.ComponentModel.ISupportInitialize)(this.templateImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inject_time)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Data_Grid)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.item_height)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.needle_dia)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox templateImg;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox item_color;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label x_lb;
        private System.Windows.Forms.Label y_lb;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button SaveDatabase;
        private System.Windows.Forms.Button ClearTracking;
        public System.Windows.Forms.NumericUpDown inject_time;
        public System.Windows.Forms.NumericUpDown item_height;
        public System.Windows.Forms.NumericUpDown numericUpDown1;
        public System.Windows.Forms.NumericUpDown needle_dia;
        private System.Windows.Forms.TextBox x_pos_lb;
        private System.Windows.Forms.TextBox y_pos_lb;
        public System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox item_name;
        private System.Windows.Forms.DataGridView Data_Grid;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel status;
        private System.Windows.Forms.DataGridViewTextBoxColumn PointNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn colX;
        private System.Windows.Forms.DataGridViewTextBoxColumn colY;
        private System.Windows.Forms.DataGridViewTextBoxColumn colC;
        private System.Windows.Forms.DataGridViewTextBoxColumn colT;
        private System.Windows.Forms.DataGridViewTextBoxColumn Color;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}