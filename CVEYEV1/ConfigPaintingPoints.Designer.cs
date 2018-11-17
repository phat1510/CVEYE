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
            this.TmpImageBox = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.item_color = new System.Windows.Forms.ComboBox();
            this.item_name = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Data_Grid = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label7 = new System.Windows.Forms.Label();
            this.SetName = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fastMoving = new System.Windows.Forms.CheckBox();
            this.xPlus = new System.Windows.Forms.Button();
            this.yPlus = new System.Windows.Forms.Button();
            this.yMinus = new System.Windows.Forms.Button();
            this.xMinus = new System.Windows.Forms.Button();
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
            this.AddRow = new System.Windows.Forms.Button();
            this.DeleteRow = new System.Windows.Forms.Button();
            this.PointNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.patternColor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.jettingMode = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.TmpImageBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Data_Grid)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.jettingMode)).BeginInit();
            this.SuspendLayout();
            // 
            // TmpImageBox
            // 
            this.TmpImageBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.TmpImageBox.Location = new System.Drawing.Point(12, 31);
            this.TmpImageBox.Name = "TmpImageBox";
            this.TmpImageBox.Size = new System.Drawing.Size(430, 430);
            this.TmpImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.TmpImageBox.TabIndex = 69;
            this.TmpImageBox.TabStop = false;
            this.TmpImageBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Template_Click);
            this.TmpImageBox.MouseEnter += new System.EventHandler(this.pattern_field_MouseEnter);
            this.TmpImageBox.MouseLeave += new System.EventHandler(this.pattern_field_MouseLeave);
            this.TmpImageBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pattern_field_MouseMove);
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 37);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 16);
            this.label5.TabIndex = 71;
            this.label5.Text = "Tên mẫu:";
            // 
            // item_color
            // 
            this.item_color.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.item_color.FormattingEnabled = true;
            this.item_color.Items.AddRange(new object[] {
            "Đỏ",
            "Đen"});
            this.item_color.Location = new System.Drawing.Point(103, 63);
            this.item_color.Name = "item_color";
            this.item_color.Size = new System.Drawing.Size(134, 24);
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
            this.item_name.Location = new System.Drawing.Point(103, 33);
            this.item_name.Name = "item_name";
            this.item_name.Size = new System.Drawing.Size(134, 24);
            this.item_name.TabIndex = 70;
            this.item_name.Text = "Tướng 01";
            this.item_name.TextChanged += new System.EventHandler(this.Name_Changed);
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(3, 67);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 16);
            this.label6.TabIndex = 72;
            this.label6.Text = "Màu:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 16);
            this.label3.TabIndex = 77;
            this.label3.Text = "Mode:";
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
            this.patternColor});
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
            this.Data_Grid.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.Data_Grid_CellEnter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(448, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 16);
            this.label2.TabIndex = 76;
            this.label2.Text = "Dữ liệu điểm sơn:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 16);
            this.label4.TabIndex = 78;
            this.label4.Text = "Hình mẫu:";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.item_color, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.item_name, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.SetName, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.jettingMode, 1, 3);
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 21);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(240, 120);
            this.tableLayoutPanel1.TabIndex = 80;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(3, 7);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(28, 16);
            this.label7.TabIndex = 87;
            this.label7.Text = "Bộ:";
            // 
            // SetName
            // 
            this.SetName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SetName.FormattingEnabled = true;
            this.SetName.Items.AddRange(new object[] {
            "Cờ 31",
            "Cờ 29"});
            this.SetName.Location = new System.Drawing.Point(103, 3);
            this.SetName.Name = "SetName";
            this.SetName.Size = new System.Drawing.Size(134, 24);
            this.SetName.TabIndex = 88;
            this.SetName.Text = "Cờ 31";
            this.SetName.TextChanged += new System.EventHandler(this.SetName_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 467);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(430, 151);
            this.groupBox1.TabIndex = 81;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Thông số mẫu";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.fastMoving);
            this.panel1.Controls.Add(this.xPlus);
            this.panel1.Controls.Add(this.yPlus);
            this.panel1.Controls.Add(this.yMinus);
            this.panel1.Controls.Add(this.xMinus);
            this.panel1.Location = new System.Drawing.Point(261, 21);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(163, 120);
            this.panel1.TabIndex = 104;
            // 
            // fastMoving
            // 
            this.fastMoving.AutoSize = true;
            this.fastMoving.Location = new System.Drawing.Point(3, 94);
            this.fastMoving.Name = "fastMoving";
            this.fastMoving.Size = new System.Drawing.Size(119, 20);
            this.fastMoving.TabIndex = 106;
            this.fastMoving.Text = "Thay đổi nhanh";
            this.fastMoving.UseVisualStyleBackColor = true;
            // 
            // xPlus
            // 
            this.xPlus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xPlus.Location = new System.Drawing.Point(108, 27);
            this.xPlus.Name = "xPlus";
            this.xPlus.Size = new System.Drawing.Size(50, 37);
            this.xPlus.TabIndex = 105;
            this.xPlus.Text = "X+";
            this.xPlus.UseVisualStyleBackColor = true;
            this.xPlus.Click += new System.EventHandler(this.xPlus_Click);
            // 
            // yPlus
            // 
            this.yPlus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yPlus.Location = new System.Drawing.Point(56, 51);
            this.yPlus.Name = "yPlus";
            this.yPlus.Size = new System.Drawing.Size(50, 37);
            this.yPlus.TabIndex = 102;
            this.yPlus.Text = "Y+";
            this.yPlus.UseVisualStyleBackColor = true;
            this.yPlus.Click += new System.EventHandler(this.yPlus_Click);
            // 
            // yMinus
            // 
            this.yMinus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yMinus.Location = new System.Drawing.Point(56, 4);
            this.yMinus.Name = "yMinus";
            this.yMinus.Size = new System.Drawing.Size(50, 37);
            this.yMinus.TabIndex = 103;
            this.yMinus.Text = "Y-";
            this.yMinus.UseVisualStyleBackColor = true;
            this.yMinus.Click += new System.EventHandler(this.yMinus_Click);
            // 
            // xMinus
            // 
            this.xMinus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xMinus.Location = new System.Drawing.Point(3, 27);
            this.xMinus.Name = "xMinus";
            this.xMinus.Size = new System.Drawing.Size(50, 37);
            this.xMinus.TabIndex = 104;
            this.xMinus.Text = "X-";
            this.xMinus.UseVisualStyleBackColor = true;
            this.xMinus.Click += new System.EventHandler(this.xMinus_Click);
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
            this.SaveDatabase.Location = new System.Drawing.Point(273, 622);
            this.SaveDatabase.Name = "SaveDatabase";
            this.SaveDatabase.Size = new System.Drawing.Size(163, 28);
            this.SaveDatabase.TabIndex = 92;
            this.SaveDatabase.Text = "Save";
            this.SaveDatabase.UseVisualStyleBackColor = true;
            this.SaveDatabase.Click += new System.EventHandler(this.SaveDatabase_Click);
            // 
            // ClearTracking
            // 
            this.ClearTracking.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClearTracking.Location = new System.Drawing.Point(12, 622);
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
            // AddRow
            // 
            this.AddRow.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddRow.Location = new System.Drawing.Point(451, 622);
            this.AddRow.Name = "AddRow";
            this.AddRow.Size = new System.Drawing.Size(80, 28);
            this.AddRow.TabIndex = 105;
            this.AddRow.Text = "Add row";
            this.AddRow.UseVisualStyleBackColor = true;
            this.AddRow.Click += new System.EventHandler(this.AddRow_Click);
            // 
            // DeleteRow
            // 
            this.DeleteRow.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeleteRow.Location = new System.Drawing.Point(537, 622);
            this.DeleteRow.Name = "DeleteRow";
            this.DeleteRow.Size = new System.Drawing.Size(80, 28);
            this.DeleteRow.TabIndex = 106;
            this.DeleteRow.Text = "Delete";
            this.DeleteRow.UseVisualStyleBackColor = true;
            this.DeleteRow.Click += new System.EventHandler(this.DeleteRow_Click);
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
            this.colT.HeaderText = "Mode";
            this.colT.Name = "colT";
            // 
            // patternColor
            // 
            this.patternColor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.patternColor.HeaderText = "Color";
            this.patternColor.Name = "patternColor";
            // 
            // jettingMode
            // 
            this.jettingMode.Location = new System.Drawing.Point(103, 93);
            this.jettingMode.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.jettingMode.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.jettingMode.Name = "jettingMode";
            this.jettingMode.Size = new System.Drawing.Size(134, 22);
            this.jettingMode.TabIndex = 89;
            this.jettingMode.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // ConfigPaintingPoints
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(864, 682);
            this.Controls.Add(this.DeleteRow);
            this.Controls.Add(this.AddRow);
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
            this.Controls.Add(this.TmpImageBox);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ConfigPaintingPoints";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Config Painting Point";
            this.Load += new System.EventHandler(this.ConfigPaintingPoints_Load);
            this.Shown += new System.EventHandler(this.ConfigPaintingPoints_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.TmpImageBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Data_Grid)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.jettingMode)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox TmpImageBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox item_color;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label x_lb;
        private System.Windows.Forms.Label y_lb;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button SaveDatabase;
        private System.Windows.Forms.Button ClearTracking;
        private System.Windows.Forms.TextBox x_pos_lb;
        private System.Windows.Forms.TextBox y_pos_lb;
        public System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox item_name;
        private System.Windows.Forms.DataGridView Data_Grid;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel status;
        private System.Windows.Forms.Button yPlus;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox SetName;
        private System.Windows.Forms.Button xPlus;
        private System.Windows.Forms.Button xMinus;
        private System.Windows.Forms.Button yMinus;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button AddRow;
        private System.Windows.Forms.Button DeleteRow;
        private System.Windows.Forms.CheckBox fastMoving;
        private System.Windows.Forms.DataGridViewTextBoxColumn PointNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn colX;
        private System.Windows.Forms.DataGridViewTextBoxColumn colY;
        private System.Windows.Forms.DataGridViewTextBoxColumn colC;
        private System.Windows.Forms.DataGridViewTextBoxColumn colT;
        private System.Windows.Forms.DataGridViewTextBoxColumn patternColor;
        private System.Windows.Forms.NumericUpDown jettingMode;
    }
}