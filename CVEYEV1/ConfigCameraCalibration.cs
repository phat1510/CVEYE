using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;

namespace CVEYEV1
{
    public partial class ConfigCameraCalibration : Form
    {
        public ConfigCameraCalibration()
        {
            InitializeComponent();
            if (CVEye.cameraOn)
                calib_image.Image = CVEye.img_capture_undist.Bitmap;
        }

        private void CameraCalibrate_Click(object sender, EventArgs e)
        {
            CVEye.CalibrationStart();
        }

        private void CompensateClick(object sender, EventArgs e)
        {
            //
            CVEye.PixelsCompensation(CVEye.img_capture_undist);
            CVEye.Draw_Grid(CVEye.img_capture_undist);
            calib_image.Image = CVEye.img_capture_undist.Bitmap;

            // Save image to image library
            CvInvoke.Imwrite("image_lib/capture" + DateTime.Now.ToFileTime() + ".jpg", CVEye.img_capture_undist);
        }

        private void OK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
