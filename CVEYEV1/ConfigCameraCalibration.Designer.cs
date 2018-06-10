namespace CVEYEV1
{
    partial class ConfigCameraCalibration
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
            this.calib_image = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.calib_data = new System.Windows.Forms.ListBox();
            this.progressCalib = new System.Windows.Forms.ProgressBar();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.calib_image)).BeginInit();
            this.SuspendLayout();
            // 
            // calib_image
            // 
            this.calib_image.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.calib_image.Location = new System.Drawing.Point(14, 14);
            this.calib_image.Margin = new System.Windows.Forms.Padding(5);
            this.calib_image.Name = "calib_image";
            this.calib_image.Size = new System.Drawing.Size(800, 600);
            this.calib_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.calib_image.TabIndex = 77;
            this.calib_image.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(904, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 16);
            this.label1.TabIndex = 79;
            this.label1.Text = "Calibration:";
            // 
            // calib_data
            // 
            this.calib_data.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.calib_data.FormattingEnabled = true;
            this.calib_data.ItemHeight = 16;
            this.calib_data.Location = new System.Drawing.Point(643, 514);
            this.calib_data.Name = "calib_data";
            this.calib_data.Size = new System.Drawing.Size(171, 100);
            this.calib_data.TabIndex = 78;
            // 
            // progressCalib
            // 
            this.progressCalib.Location = new System.Drawing.Point(12, 622);
            this.progressCalib.Name = "progressCalib";
            this.progressCalib.Size = new System.Drawing.Size(146, 28);
            this.progressCalib.TabIndex = 82;
            // 
            // button5
            // 
            this.button5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button5.Location = new System.Drawing.Point(393, 622);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(100, 28);
            this.button5.TabIndex = 83;
            this.button5.Text = "Calibrate";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.CameraCalibrate_Click);
            // 
            // button6
            // 
            this.button6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button6.Location = new System.Drawing.Point(512, 622);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(100, 28);
            this.button6.TabIndex = 84;
            this.button6.Text = "Compensate";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.CompensateClick);
            // 
            // button7
            // 
            this.button7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button7.Location = new System.Drawing.Point(714, 622);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(100, 28);
            this.button7.TabIndex = 85;
            this.button7.Text = "OK";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.OK_Click);
            // 
            // ConfigCameraCalibration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 656);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.progressCalib);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.calib_data);
            this.Controls.Add(this.calib_image);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ConfigCameraCalibration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Camera Calibration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this._FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.calib_image)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.PictureBox calib_image;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ListBox calib_data;
        private System.Windows.Forms.ProgressBar progressCalib;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
    }
}