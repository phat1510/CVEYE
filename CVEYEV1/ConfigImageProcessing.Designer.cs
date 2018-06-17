namespace CVEYEV1
{
    partial class ConfigImageProcessing
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
            this.OK = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.gaussian_sig = new System.Windows.Forms.NumericUpDown();
            this.houge_param1 = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.houge_param2 = new System.Windows.Forms.NumericUpDown();
            this.G_blur = new System.Windows.Forms.CheckBox();
            this.Cancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.min_ra = new System.Windows.Forms.NumericUpDown();
            this.max_ra = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.ErrConstraint = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.correctionRange = new System.Windows.Forms.NumericUpDown();
            this.cannyThresh = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gaussian_sig)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.houge_param1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.houge_param2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.min_ra)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.max_ra)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ErrConstraint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.correctionRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cannyThresh)).BeginInit();
            this.SuspendLayout();
            // 
            // OK
            // 
            this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.OK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OK.Location = new System.Drawing.Point(318, 200);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 0;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(3, 7);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 16);
            this.label7.TabIndex = 41;
            this.label7.Text = "Canny threshold:";
            // 
            // gaussian_sig
            // 
            this.gaussian_sig.DecimalPlaces = 1;
            this.gaussian_sig.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gaussian_sig.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.gaussian_sig.Location = new System.Drawing.Point(168, 54);
            this.gaussian_sig.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.gaussian_sig.Name = "gaussian_sig";
            this.gaussian_sig.Size = new System.Drawing.Size(49, 22);
            this.gaussian_sig.TabIndex = 46;
            // 
            // houge_param1
            // 
            this.houge_param1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.houge_param1.Location = new System.Drawing.Point(158, 3);
            this.houge_param1.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.houge_param1.Name = "houge_param1";
            this.houge_param1.Size = new System.Drawing.Size(49, 22);
            this.houge_param1.TabIndex = 42;
            this.houge_param1.Value = new decimal(new int[] {
            65,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(108, 16);
            this.label4.TabIndex = 45;
            this.label4.Text = "Gaussian sigma:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(143, 16);
            this.label3.TabIndex = 43;
            this.label3.Text = "Accumulator threshold:";
            // 
            // houge_param2
            // 
            this.houge_param2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.houge_param2.Location = new System.Drawing.Point(158, 33);
            this.houge_param2.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.houge_param2.Name = "houge_param2";
            this.houge_param2.Size = new System.Drawing.Size(49, 22);
            this.houge_param2.TabIndex = 44;
            this.houge_param2.Value = new decimal(new int[] {
            40,
            0,
            0,
            0});
            // 
            // G_blur
            // 
            this.G_blur.AutoSize = true;
            this.G_blur.Checked = true;
            this.G_blur.CheckState = System.Windows.Forms.CheckState.Checked;
            this.G_blur.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.G_blur.Location = new System.Drawing.Point(9, 24);
            this.G_blur.Name = "G_blur";
            this.G_blur.Size = new System.Drawing.Size(110, 20);
            this.G_blur.TabIndex = 60;
            this.G_blur.Text = "Gaussian Blur";
            this.G_blur.UseVisualStyleBackColor = true;
            // 
            // Cancel
            // 
            this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cancel.Location = new System.Drawing.Point(403, 200);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 61;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(250, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(230, 152);
            this.groupBox1.TabIndex = 62;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Hough Circles Detector";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 71.55963F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.44037F));
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.max_ra, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.houge_param2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.houge_param1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.min_ra, 1, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 25);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(218, 120);
            this.tableLayoutPanel1.TabIndex = 65;
            // 
            // min_ra
            // 
            this.min_ra.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.min_ra.Location = new System.Drawing.Point(158, 63);
            this.min_ra.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.min_ra.Name = "min_ra";
            this.min_ra.Size = new System.Drawing.Size(49, 22);
            this.min_ra.TabIndex = 66;
            this.min_ra.Value = new decimal(new int[] {
            73,
            0,
            0,
            0});
            // 
            // max_ra
            // 
            this.max_ra.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.max_ra.Location = new System.Drawing.Point(158, 93);
            this.max_ra.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.max_ra.Name = "max_ra";
            this.max_ra.Size = new System.Drawing.Size(49, 22);
            this.max_ra.TabIndex = 65;
            this.max_ra.Value = new decimal(new int[] {
            76,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 16);
            this.label2.TabIndex = 66;
            this.label2.Text = "Min Circle Radius:";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 16);
            this.label1.TabIndex = 65;
            this.label1.Text = "Max Circle Radius:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.G_blur);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.gaussian_sig);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(12, 139);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(230, 85);
            this.groupBox2.TabIndex = 63;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Image Filtering";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tableLayoutPanel2);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(230, 121);
            this.groupBox3.TabIndex = 64;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Matching Correction";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 71.55963F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.44037F));
            this.tableLayoutPanel2.Controls.Add(this.ErrConstraint, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label8, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.correctionRange, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.cannyThresh, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label9, 0, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(6, 25);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(218, 90);
            this.tableLayoutPanel2.TabIndex = 66;
            // 
            // ErrConstraint
            // 
            this.ErrConstraint.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ErrConstraint.Location = new System.Drawing.Point(158, 63);
            this.ErrConstraint.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.ErrConstraint.Name = "ErrConstraint";
            this.ErrConstraint.Size = new System.Drawing.Size(49, 22);
            this.ErrConstraint.TabIndex = 65;
            this.ErrConstraint.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(3, 67);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 16);
            this.label6.TabIndex = 65;
            this.label6.Text = "Error Constraint:";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(3, 7);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(107, 16);
            this.label8.TabIndex = 41;
            this.label8.Text = "Canny threshold:";
            // 
            // correctionRange
            // 
            this.correctionRange.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.correctionRange.Location = new System.Drawing.Point(158, 33);
            this.correctionRange.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.correctionRange.Name = "correctionRange";
            this.correctionRange.Size = new System.Drawing.Size(49, 22);
            this.correctionRange.TabIndex = 44;
            this.correctionRange.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // cannyThresh
            // 
            this.cannyThresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cannyThresh.Location = new System.Drawing.Point(158, 3);
            this.cannyThresh.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.cannyThresh.Name = "cannyThresh";
            this.cannyThresh.Size = new System.Drawing.Size(49, 22);
            this.cannyThresh.TabIndex = 42;
            this.cannyThresh.Value = new decimal(new int[] {
            475,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(3, 37);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(116, 16);
            this.label9.TabIndex = 43;
            this.label9.Text = "Correction Range:";
            // 
            // ConfigImageProcessing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 235);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ConfigImageProcessing";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Image Processing Parameters";
            ((System.ComponentModel.ISupportInitialize)(this.gaussian_sig)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.houge_param1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.houge_param2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.min_ra)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.max_ra)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ErrConstraint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.correctionRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cannyThresh)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.NumericUpDown gaussian_sig;
        public System.Windows.Forms.NumericUpDown houge_param1;
        public System.Windows.Forms.NumericUpDown houge_param2;
        public System.Windows.Forms.CheckBox G_blur;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.NumericUpDown min_ra;
        public System.Windows.Forms.NumericUpDown max_ra;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        public System.Windows.Forms.NumericUpDown ErrConstraint;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        public System.Windows.Forms.NumericUpDown correctionRange;
        public System.Windows.Forms.NumericUpDown cannyThresh;
        private System.Windows.Forms.Label label9;
    }
}