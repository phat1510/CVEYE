/*
Project: Autonomous Xiangqi Panting Maching
Program name:
Author: Phat Do
Date created: 
Decription:
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.VideoSurveillance;

namespace CVEYEV1
{
    public partial class CVEye : Form
    {
        private VideoCapture _capture;

        private string s_name = "pattern_field.jpg";
        private string t_name = "template.jpg";
        private Image<Bgr, byte> img_raw;
        private Image<Bgr, byte> tmp_raw;
        
        private double mouse_val;
        private bool mouse_enter;
        
        public CVEye()
        {
            InitializeComponent();

            // Enable mousewheel operation and 
            // add handler for pattern_field_MouseWheel.
            MouseWheel += pattern_field_MouseWheel;

            //Initialize_Camera();

            Initial_Image();
        }

        //---------------------Functions-------------------------------//
        //-------------------------------------------------------------//

        public void Initial_Image()
        {
            //Load images
            img_raw = new Image<Bgr, byte>(s_name);
            tmp_raw = new Image<Bgr, byte>(t_name);
            pattern_field.Image = img_raw.Bitmap;
        }

        private void Initialize_Camera()
        {
            if (_capture == null)
            {
                try   //Try to create the capture
                {
                    _capture = new VideoCapture();
                }
                catch (NullReferenceException excpt)
                {   //Show errors if there is any
                    MessageBox.Show(excpt.Message);
                }
            }

            if (_capture != null)
            {
                // Add handler for Image_Capture
                _capture.ImageGrabbed += Image_Capture;
                _capture.Start();
            }
        }

        private void Image_Capture(object sender, EventArgs e)
        {
            Mat image = new Mat();
            _capture.Retrieve(image);
            pattern_field.Image = image.Bitmap;
        }

        public void Frame_Analyze()
        {
            try
            {
                /*----------------------------Filters----------------------------*/

                // 
                Mat img_pyr = new Mat();
                CvInvoke.PyrDown(img_raw, img_pyr);
                CvInvoke.PyrUp(img_pyr, img_pyr);
                
                // Convert the image to grayscale
                Mat img_gray = new Mat();
                CvInvoke.CvtColor(img_pyr, img_gray, ColorConversion.Bgr2Gray);

                // Equalize histogram
                Mat img_hist = new Mat();
                CvInvoke.EqualizeHist(img_gray, img_hist);
         
                // Image threshold
                Mat img_threshold = new Mat();
                CvInvoke.AdaptiveThreshold(img_hist, img_threshold, 255, AdaptiveThresholdType.GaussianC, ThresholdType.Binary, 95, 1);

                // Circle Hough Transform
                CircleF[] img_circles = CvInvoke.HoughCircles(img_gray, HoughType.Gradient, 2, 25, 150, 110); // array of circle data

                /*----------------------------Template Method----------------------------*/
                // Template rotation
                PointF rot_cen = new PointF(tmp_raw.Rows / 2, tmp_raw.Cols / 2);
                Mat tmp_rot = new Mat();
                Mat tmp_dst = new Mat();

                CvInvoke.GetRotationMatrix2D(rot_cen, -50, 1, tmp_rot);
                CvInvoke.WarpAffine(tmp_raw, tmp_dst, tmp_rot, tmp_raw.Size);

                /*----------------------------Drawing Method----------------------------*/
                // Draw detected circles
                foreach (CircleF circle in img_circles)
                {
                    img_raw.Draw(circle, new Bgr(Color.Red), 2);
                }

                pattern_field.Image = img_threshold.Bitmap;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            
        }
        
        private void pattern_field_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                Cursor.Current = Cursors.Hand;
            }
            else
            {
                Cursor.Current = Cursors.Cross; // Cursor shape
            }
            X_Axis.Text = "X:   " + e.Location.X.ToString();
            Y_Axis.Text = "Y:   " + e.Location.Y.ToString();
            Invalidate();
        }

        private void pattern_field_MouseWheel(object sender, MouseEventArgs e)
        {
            if (mouse_enter)
            {
                mouse_val += e.Delta;
            }

        }

        private void pattern_field_MouseEnter(object sender, EventArgs e)
        {
            mouse_enter = true;
        }

        private void pattern_field_MouseLeave(object sender, EventArgs e)
        {
            mouse_enter = !mouse_enter;
        }
        //---------------------Click Events-------------------------------//
        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Analyze_Click(object sender, EventArgs e)
        {
            Frame_Analyze();
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            OpenFileDialog s_file = new OpenFileDialog();
            if (s_file.ShowDialog() == DialogResult.OK)
            {
                Image<Bgr, byte> s_raw = new Image<Bgr, byte>(s_file.FileName);
                pattern_field.Image = s_raw.Bitmap;
            }
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            img_raw = new Image<Bgr, byte>(s_name);
            pattern_field.Image = img_raw.Bitmap;
        }

        private void Capture_Click(object sender, EventArgs e)
        {

        }


    }
}
