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
        }

        private void CameraCalibrate_Click(object sender, EventArgs e)
        {
            CVEye.CalibrationStart();
        }

        private void CompensateClick(object sender, EventArgs e)
        {
            using (Image<Bgr, byte> drawImg = CVEye.img_capture_undist.Clone())
            {
                CVEye.PixelsCompensation(drawImg);
                CVEye.Draw_Grid(drawImg);
                CvInvoke.Imwrite("result/comResult.jpg", drawImg);
            }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
