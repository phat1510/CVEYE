namespace CVEYEV1
{
    partial class ConfigPaintingCondition
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.zSafe = new System.Windows.Forms.NumericUpDown();
            this.zSpeed = new System.Windows.Forms.NumericUpDown();
            this.xySpeed = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.zReturn = new System.Windows.Forms.NumericUpDown();
            this.zDrip = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Cancel = new System.Windows.Forms.Button();
            this.OK = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zSafe)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xySpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zReturn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zDrip)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.zSafe, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.zSpeed, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.xySpeed, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label12, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.zReturn, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.zDrip, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label14, 0, 4);
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(13, 21);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(187, 150);
            this.tableLayoutPanel1.TabIndex = 73;
            // 
            // zSafe
            // 
            this.zSafe.DecimalPlaces = 1;
            this.zSafe.Location = new System.Drawing.Point(96, 63);
            this.zSafe.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.zSafe.Name = "zSafe";
            this.zSafe.Size = new System.Drawing.Size(56, 22);
            this.zSafe.TabIndex = 78;
            this.zSafe.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // zSpeed
            // 
            this.zSpeed.Location = new System.Drawing.Point(96, 33);
            this.zSpeed.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.zSpeed.Name = "zSpeed";
            this.zSpeed.Size = new System.Drawing.Size(56, 22);
            this.zSpeed.TabIndex = 77;
            this.zSpeed.Value = new decimal(new int[] {
            12000,
            0,
            0,
            0});
            // 
            // xySpeed
            // 
            this.xySpeed.Location = new System.Drawing.Point(96, 3);
            this.xySpeed.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.xySpeed.Name = "xySpeed";
            this.xySpeed.Size = new System.Drawing.Size(56, 22);
            this.xySpeed.TabIndex = 73;
            this.xySpeed.Value = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(3, 67);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(48, 16);
            this.label12.TabIndex = 76;
            this.label12.Text = "Z safe:";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "XY Speed:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 16);
            this.label3.TabIndex = 1;
            this.label3.Text = "Z Speed:";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 16);
            this.label4.TabIndex = 75;
            this.label4.Text = "Z return:";
            // 
            // zReturn
            // 
            this.zReturn.DecimalPlaces = 1;
            this.zReturn.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.zReturn.Location = new System.Drawing.Point(96, 93);
            this.zReturn.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.zReturn.Name = "zReturn";
            this.zReturn.Size = new System.Drawing.Size(56, 22);
            this.zReturn.TabIndex = 79;
            this.zReturn.Value = new decimal(new int[] {
            93,
            0,
            0,
            65536});
            // 
            // zDrip
            // 
            this.zDrip.DecimalPlaces = 1;
            this.zDrip.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.zDrip.Location = new System.Drawing.Point(96, 123);
            this.zDrip.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.zDrip.Name = "zDrip";
            this.zDrip.Size = new System.Drawing.Size(56, 22);
            this.zDrip.TabIndex = 82;
            this.zDrip.Value = new decimal(new int[] {
            87,
            0,
            0,
            65536});
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 127);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(45, 16);
            this.label14.TabIndex = 81;
            this.label14.Text = "Z drip:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(210, 184);
            this.groupBox1.TabIndex = 74;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Parameters";
            // 
            // Cancel
            // 
            this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cancel.Location = new System.Drawing.Point(145, 205);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 76;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // OK
            // 
            this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.OK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OK.Location = new System.Drawing.Point(60, 205);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 75;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // ConfigPaintingCondition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(232, 240);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ConfigPaintingCondition";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Painting Condition";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zSafe)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xySpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zReturn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zDrip)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.NumericUpDown zDrip;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button OK;
        public System.Windows.Forms.NumericUpDown zSafe;
        public System.Windows.Forms.NumericUpDown zSpeed;
        public System.Windows.Forms.NumericUpDown xySpeed;
        public System.Windows.Forms.NumericUpDown zReturn;
    }
}