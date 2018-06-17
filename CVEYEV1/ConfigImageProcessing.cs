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
            CVEye.SysData.Element("System").Element("ImageProcessingWindow").RemoveAll();
            CVEye.SysData.Element("System").Element("ImageProcessingWindow").Add(new XElement("MachingCorrection",
                new XAttribute("cannyThresh", cannyThresh.Value),
                new XAttribute("correctionRange", correctionRange.Value),
                new XAttribute("ErrConstraint", ErrConstraint.Value)));
            CVEye.SysData.Element("System").Element("ImageProcessingWindow").Add(new XElement("HoughCirclesDetector",
                new XAttribute("houge_param1", houge_param1.Value),
                new XAttribute("houge_param2", houge_param2.Value),
                new XAttribute("min_ra", min_ra.Value),
                new XAttribute("max_ra", max_ra.Value)));
            CVEye.SysData.Element("System").Element("ImageProcessingWindow").Add(new XElement("ImageFiltering",
                new XAttribute("gaussian_sig", gaussian_sig.Value),
                new XAttribute("G_blur", ((G_blur.CheckState == CheckState.Checked) ? 1 : 0))));
            
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
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
