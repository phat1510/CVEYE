using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// EmguCV library
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.VideoSurveillance;
using Emgu.CV.Features2D;

namespace CVEYEV1
{
    public partial class ConfigCameraCalibration : Form
    {
        public ConfigCameraCalibration()
        {
            InitializeComponent();
            //if (CVEye.cameraOn)
            calib_image.Image = CVEye.img_capture_undist.Bitmap;
        }

        private void CameraCalibrate_Click(object sender, EventArgs e)
        {
            CVEye.CalibrationStart();
        }

        private void CompensateClick(object sender, EventArgs e)
        {
            using (Image<Bgr, byte> drawImg = CVEye.img_capture_undist.Clone())
            {
                //
                CVEye.PixelsCompensation(drawImg);
                CVEye.Draw_Grid(drawImg);
                calib_image.Image = drawImg.Bitmap;

                // Save image to image library
                // CvInvoke.Imwrite("image_lib/capture" + DateTime.Now.ToFileTime() + ".jpg", CVEye.img_capture_undist);
                CvInvoke.Imwrite("result/4.img_items.jpg", drawImg);
            }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
