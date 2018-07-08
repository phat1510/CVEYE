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

            // Remove node data
            CVEye.SysData.Element("System").Element("PaintingConditionWindow").RemoveAll();

            // Updata new data
            CVEye.SysData.Element("System").Element("PaintingConditionWindow").Add(new XElement("Parameters",
                new XAttribute("xySpeed", xySpeed.Value),
                new XAttribute("zSpeed", zSpeed.Value),
                new XAttribute("zSafe", zSafe.Value),
                new XAttribute("zReturn", zReturn.Value),
                new XAttribute("zDrip", zDrip.Value),
                new XAttribute("offset", deepOffset.Value),
                new XAttribute("circleSpeed", circleSpeed.Value)
                ));

            CVEye.SysData.Element("System").Element("PaintingConditionWindow").Add(new XElement("Offset"));
            for (int i = 0; i < 5; i++)
            {
                string Gxx = "G" + (54 + i).ToString();
                CVEye.SysData.Element("System").Element("PaintingConditionWindow").Element("Offset").Add(new XElement(
                    Gxx,
                    new XAttribute("x", OffsetCoor.Rows[i].Cells[1].Value),
                    new XAttribute("y", OffsetCoor.Rows[i].Cells[2].Value),
                    new XAttribute("z", OffsetCoor.Rows[i].Cells[3].Value)));
            }

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
            deepOffset.Value = decimal.Parse(Parameters.Attribute("offset").Value);
            circleSpeed.Value = decimal.Parse(Parameters.Attribute("circleSpeed").Value);

            XElement Offset = CVEye.SysData.Element("System").Element("PaintingConditionWindow").Element("Offset");
            for (int i = 0; i < 5; i++)
            {
                string Gxx = "G" + (54 + i).ToString();
                OffsetCoor.Rows.Add(Gxx,
                    Offset.Element(Gxx).Attribute("x").Value,
                    Offset.Element(Gxx).Attribute("y").Value,
                    Offset.Element(Gxx).Attribute("z").Value);
            }
        }
        
        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
            //CVEye.first_start04 = true;
        }
    }
}
