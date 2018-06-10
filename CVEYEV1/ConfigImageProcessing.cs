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
            CVEye.SysData = XDocument.Load("_system.xml");
            CVEye.SysData.Element("System").Element("ImageProcessingWindow").RemoveAll();
            CVEye.SysData.Element("System").Element("ImageProcessingWindow").Add(
                new XElement("CannyThresh", cannyThresh.Value),
                new XElement("CorrectionRange", correctionRange.Value),
                new XElement("ErrConstraint", ErrConstraint.Value),
                new XElement("HougeParam1", houge_param1.Value),
                new XElement("HougeParam2", houge_param2.Value),
                new XElement("MaxRa", max_ra.Value),
                new XElement("MinRa", min_ra.Value),
                new XElement("GaussianSigma", gaussian_sig.Value),
                new XElement("Gblur", ((G_blur.CheckState == CheckState.Checked) ? 1 : 0)));
            CVEye.SysData.Save("_system.xml");

            Close();
        }

        private void LoadXML()
        {
            CVEye.SysData = XDocument.Load("_system.xml");
            XElement ImageProcessingWindow = CVEye.SysData.Element("System").Element("ImageProcessingWindow");
            cannyThresh.Value = decimal.Parse(ImageProcessingWindow.Element("CannyThresh").Value);
            correctionRange.Value = decimal.Parse(ImageProcessingWindow.Element("CorrectionRange").Value);
            ErrConstraint.Value = decimal.Parse(ImageProcessingWindow.Element("ErrConstraint").Value);
            houge_param1.Value = decimal.Parse(ImageProcessingWindow.Element("HougeParam1").Value);
            houge_param2.Value = decimal.Parse(ImageProcessingWindow.Element("HougeParam2").Value);
            max_ra.Value = decimal.Parse(ImageProcessingWindow.Element("MaxRa").Value);
            min_ra.Value = decimal.Parse(ImageProcessingWindow.Element("MinRa").Value);
            gaussian_sig.Value = decimal.Parse(ImageProcessingWindow.Element("GaussianSigma").Value);
            G_blur.CheckState = (decimal.Parse(ImageProcessingWindow.Element("Gblur").Value) == 1) ? CheckState.Checked : CheckState.Unchecked;

        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
            CVEye.first_start02 = true;
        }

        private void _FromClosing(object sender, FormClosingEventArgs e)
        {
            CVEye.first_start02 = true;
        }
    }
}
