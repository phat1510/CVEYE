using System;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CVEYEV1
{
    public partial class ConfigPaintingCondition : Form
    {
        public ConfigPaintingCondition()
        {
            InitializeComponent();

            LoadXML();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            // Reload XML file
            CVEye.SysData = XDocument.Load("_system.xml");

            // Update new data
            CVEye.SysData.Element("System").Element("PaintingConditionWindow").RemoveAll();
            CVEye.SysData.Element("System").Element("PaintingConditionWindow").Add(new XElement("Parameters",
                new XAttribute("xySpeed", xySpeed.Value),
                new XAttribute("zSpeed", zSpeed.Value),
                new XAttribute("zSafe", zSafe.Value),
                new XAttribute("zReturn", zReturn.Value),
                new XAttribute("zDrip", zDrip.Value),
                new XAttribute("offset", offset.Value)
                ));

            CVEye.SysData.Save("_system.xml");

            Close();
        }

        private void LoadXML()
        {
            CVEye.SysData = XDocument.Load("_system.xml");
            XElement Parameters = CVEye.SysData.Element("System").Element("PaintingConditionWindow").Element("Parameters");
            xySpeed.Value = decimal.Parse(Parameters.Attribute("xySpeed").Value);
            zSpeed.Value = decimal.Parse(Parameters.Attribute("zSpeed").Value);
            zSafe.Value = decimal.Parse(Parameters.Attribute("zSafe").Value);
            zReturn.Value = decimal.Parse(Parameters.Attribute("zReturn").Value);
            zDrip.Value = decimal.Parse(Parameters.Attribute("zDrip").Value);
            offset.Value = decimal.Parse(Parameters.Attribute("offset").Value);
        }
        
        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
            //CVEye.first_start04 = true;
        }
    }
}
