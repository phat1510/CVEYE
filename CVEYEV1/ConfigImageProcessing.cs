using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace CVEYEV1
{
    public partial class ConfigImageProcessing : Form
    {
        public ConfigImageProcessing()
        {
            InitializeComponent();

            LoadXML();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            // Load XML file
            CVEye.SysData = XDocument.Load("_system.xml");

            // Update new data

            CVEye.SysData.Element("System").Element("ImageProcessingWindow").Element("MachingCorrection").Attribute("cannyThresh").Value = cannyThresh.Value.ToString();
            CVEye.SysData.Element("System").Element("ImageProcessingWindow").Element("MachingCorrection").Attribute("correctionRange").Value = correctionRange.Value.ToString();
            CVEye.SysData.Element("System").Element("ImageProcessingWindow").Element("MachingCorrection").Attribute("ErrConstraint").Value = ErrConstraint.Value.ToString();

            CVEye.SysData.Element("System").Element("ImageProcessingWindow").Element("HoughCirclesDetector").Attribute("houge_param1").Value = houge_param1.Value.ToString();
            CVEye.SysData.Element("System").Element("ImageProcessingWindow").Element("HoughCirclesDetector").Attribute("houge_param2").Value = houge_param2.Value.ToString();
            CVEye.SysData.Element("System").Element("ImageProcessingWindow").Element("HoughCirclesDetector").Attribute("min_ra").Value = min_ra.Value.ToString();
            CVEye.SysData.Element("System").Element("ImageProcessingWindow").Element("HoughCirclesDetector").Attribute("max_ra").Value = max_ra.Value.ToString();

            CVEye.SysData.Element("System").Element("ImageProcessingWindow").Element("ImageFiltering").Attribute("gaussian_sig").Value = gaussian_sig.Value.ToString();
            CVEye.SysData.Element("System").Element("ImageProcessingWindow").Element("ImageFiltering").Attribute("G_blur").Value = ((G_blur.CheckState == CheckState.Checked) ? 1 : 0).ToString();

            CVEye.SysData.Element("System").Element("ImageProcessingWindow").Element("PSTTransform").Attribute("edge1").Value = cnl1.Value.ToString();
            CVEye.SysData.Element("System").Element("ImageProcessingWindow").Element("PSTTransform").Attribute("edge2").Value = cnl2.Value.ToString();
            CVEye.SysData.Element("System").Element("ImageProcessingWindow").Element("PSTTransform").Attribute("edge3").Value = cnl3.Value.ToString();
            CVEye.SysData.Element("System").Element("ImageProcessingWindow").Element("PSTTransform").Attribute("edge4").Value = cnl4.Value.ToString();

            // Save document
            CVEye.SysData.Save("_system.xml");

            Close();
        }

        private void LoadXML()
        {
            CVEye.SysData = XDocument.Load("_system.xml");
            XElement ImageProcessingWindow = CVEye.SysData.Element("System").Element("ImageProcessingWindow");
            cannyThresh.Value       = decimal.Parse(ImageProcessingWindow.Element("MachingCorrection").Attribute("cannyThresh").Value);
            correctionRange.Value   = decimal.Parse(ImageProcessingWindow.Element("MachingCorrection").Attribute("correctionRange").Value);
            ErrConstraint.Value     = decimal.Parse(ImageProcessingWindow.Element("MachingCorrection").Attribute("ErrConstraint").Value);
            houge_param1.Value      = decimal.Parse(ImageProcessingWindow.Element("HoughCirclesDetector").Attribute("houge_param1").Value);
            houge_param2.Value      = decimal.Parse(ImageProcessingWindow.Element("HoughCirclesDetector").Attribute("houge_param2").Value);
            min_ra.Value            = decimal.Parse(ImageProcessingWindow.Element("HoughCirclesDetector").Attribute("min_ra").Value);
            max_ra.Value            = decimal.Parse(ImageProcessingWindow.Element("HoughCirclesDetector").Attribute("max_ra").Value);
            gaussian_sig.Value      = decimal.Parse(ImageProcessingWindow.Element("ImageFiltering").Attribute("gaussian_sig").Value);
            G_blur.CheckState       = (decimal.Parse(ImageProcessingWindow.Element("ImageFiltering").Attribute("G_blur").Value) == 1) ? CheckState.Checked : CheckState.Unchecked;
            cnl1.Value = decimal.Parse(ImageProcessingWindow.Element("PSTTransform").Attribute("edge1").Value);
            cnl2.Value = decimal.Parse(ImageProcessingWindow.Element("PSTTransform").Attribute("edge2").Value);
            cnl3.Value = decimal.Parse(ImageProcessingWindow.Element("PSTTransform").Attribute("edge3").Value);
            cnl4.Value = decimal.Parse(ImageProcessingWindow.Element("PSTTransform").Attribute("edge4").Value);
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
