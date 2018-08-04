/*
Project: Autonomous Chinese Chess Panting Maching
Program name: Setting Dispensing Points
Author: Phat Do
Date created: 
Decription:21/03/2018
*/

// System library
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

// EmguCV library
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;


namespace CVEYEV1
{
    public partial class ConfigPaintingPoints : Form
    {
        // 
        private Image<Bgr,byte> template_img;

        private double mouse_val;
        private bool mouse_enter;

        private double x_real_pos, y_real_pos;
        private int x_axis, y_axis;
        private double real_tmp_size = 121;
        private double raw_tmp_size = 430;
        private double scale = 0;
        public static string data_path = "_database.xml";

        public static Point[] painting_points;
        //public static PointF[] affine_painting_points;

        Image<Bgr, byte> points_tracking;
        public static int point_num = 0;

        public static XDocument dispensing_data;
        public static XElement get_item;

        public static double real_accuracy = 0;

        private bool clear =  false;
        private bool first_start = false;

        public ConfigPaintingPoints()
        {
            InitializeComponent();

            Init_Image();

            Init_Xml();

            painting_points = new Point[25];
            //affine_painting_points = new PointF[25];

            scale = real_tmp_size / raw_tmp_size;
            real_accuracy = CVEye._accuracy;

        }

        private void Init_Image()
        {
            template_img = new Image<Bgr, byte>("pattern_data/01/gen01.jpg");
            //template_view.Image = template_img.Bitmap;
            points_tracking = template_img.Clone();
        }

        private void Init_Xml()
        {
            // Load up-to-date XML database
            LoadXml();   
        }

        // Load points data to data table
        private void LoadXml()
        {
            try
            {
                points_tracking = template_img.Clone();
                PointF[] draw_point = new PointF[2];
                int t = 0;
                int k = 1;

                // Clear and load points data
                Data_Grid.Rows.Clear();
                dispensing_data = XDocument.Load(data_path);

                // Call current item
                get_item = dispensing_data.Element("Field")
                    .Elements("Item")
                    .Where(x => x.Element("Name").Value == item_name.Text)
                    .Single();

                List<XElement> point_list = get_item.Element("Points").Elements("Point").ToList();
                
                foreach (XElement element in point_list)
                {
                    Data_Grid.Rows.Add(k,
                        element.Attribute("X").Value,
                        element.Attribute("Y").Value,
                        element.Attribute("Z").Value,
                        element.Attribute("T").Value,
                        element.Attribute("C").Value);                                       

                    // Preview point tracking
                    if (t == 1)
                    {
                        draw_point[t] = new PointF((float)(float.Parse(element.Attribute("X").Value) / scale), (float)(float.Parse(element.Attribute("Y").Value) / scale));
                        points_tracking.Draw(new LineSegment2DF(draw_point[t - 1], draw_point[t]),
                            new Bgr(System.Drawing.Color.Cyan), 1);
                        draw_point[t - 1] = draw_point[t];
                    }
                    else
                    {
                        draw_point[t] = new PointF((float)(float.Parse(element.Attribute("X").Value) / scale), (float)(float.Parse(element.Attribute("Y").Value) / scale));
                        t++;
                    }

                    points_tracking.Draw(k.ToString(), new Point((int)draw_point[0].X, (int)draw_point[0].Y), FontFace.HersheySimplex, 0.5, new Bgr(System.Drawing.Color.YellowGreen));

                    k++;

                }

                template_view.Image = points_tracking.Bitmap;
                template_view.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Reload_Template(string tmp_path)
        {
            template_img = new Image<Bgr, byte>(tmp_path);
            template_view.Image = new Image<Bgr, byte>(tmp_path).Bitmap;
            points_tracking = template_img.Clone();
        }

        private void Name_Changed(object sender, EventArgs e)
        {
            LoadXml();
            view_template(item_name.Text);
            point_num = 0;
        }

        private void ViewPixelInfo(MouseEventArgs e)
        {
            Image<Bgr, byte> template_clone;
            template_clone = points_tracking.Clone();

            LineSegment2DF vertical = new LineSegment2DF();
            LineSegment2DF horizontal = new LineSegment2DF();

            // Cursor in picture box position
            x_axis = e.Location.X + 1;
            y_axis = e.Location.Y + 1;

            CalculateLocalPosition();

            vertical.P1 = new Point(x_axis, 0);
            vertical.P2 = new Point(x_axis, template_view.Height);
            horizontal.P1 = new Point(0, y_axis);
            horizontal.P2 = new Point(template_view.Width, y_axis);

            template_clone.Draw(vertical, new Bgr(System.Drawing.Color.Blue), 1);
            template_clone.Draw(horizontal, new Bgr(System.Drawing.Color.Blue), 1);

            double needle_cir;
            needle_cir = ((double)needle_dia.Value / real_accuracy) / scale;

            template_clone.Draw(new CircleF(new PointF(x_axis, y_axis), (float)needle_cir), new Bgr(System.Drawing.Color.GreenYellow), 1);

            template_view.Image = template_clone.Bitmap;
        }

        // Get local painting points
        private void Template_Click(object sender, MouseEventArgs e)
        {
            if (clear || point_num == 0)
            {  
                // Drawing
                painting_points[point_num] = new Point(x_axis, y_axis);

                if (point_num > 0)
                {
                    points_tracking.Draw(new LineSegment2DF(painting_points[point_num - 1], painting_points[point_num]),
                        new Bgr(System.Drawing.Color.Cyan), 1);
                }
                clear = true;
                point_num++;

                Data_Grid.Rows.Add(point_num, x_real_pos, y_real_pos, "-", "-", item_color.Text);
            }
        }

        private void CalculateLocalPosition()
        {
            // Calculate real painting point local coordinate
            x_real_pos = Math.Round(x_axis * scale, 3);
            y_real_pos = Math.Round(y_axis * scale, 3);

            // Show value
            x_pos_lb.Text = Convert.ToString(x_real_pos);
            y_pos_lb.Text = Convert.ToString(y_real_pos);
        }

        private void template_view_MouseWheel(object sender, MouseEventArgs e)
        {
            if (mouse_enter)
            {
                mouse_val += e.Delta;
            }
        }

        private void ClearTracking_Click(object sender, EventArgs e)
        {
            try
            {
                painting_points = new Point[100];
                // Refresh picture box
                template_view.Image = template_img.Bitmap;
                points_tracking = template_img.Clone();
                point_num = 0;

                // Clear datagridview
                Data_Grid.Rows.Clear();

                // Remove current item
                get_item = dispensing_data.Element("Field")
                .Elements("Item")
                .Where(x => x.Element("Name").Value == item_name.Text)
                .Single();

                if (get_item !=null)
                    get_item.Element("Points").RemoveAll();
                else
                    MessageBox.Show("Nothing to clear");

                dispensing_data.Save(data_path);
                clear = true;
                status.Text = "Cleared";
            }
            catch (Exception)
            {
                MessageBox.Show("Exception thrown");

                // Refresh picture box
                template_view.Image = template_img.Bitmap;
                points_tracking = template_img.Clone();
                point_num = 0;

                // Clear datagridview
                Data_Grid.Rows.Clear();
            }
        }

        private void UpdateDatabase()
        {
            // Save dispensing data to XML
            dispensing_data = XDocument.Load(data_path);

            // Remove current item
            get_item = dispensing_data.Element("Field")
            .Elements("Item")
            .Where(x => x.Element("Name").Value == item_name.Text)
            .Single();
            get_item.Element("Points").RemoveAll();

            for (int k = 0; k < point_num; k++)
            {
                get_item.Element("Points").Add(new XElement("Point",
                    new XAttribute("X", Data_Grid.Rows[k].Cells[1].Value),
                    new XAttribute("Y", Data_Grid.Rows[k].Cells[2].Value),
                    new XAttribute("Z", "-"),
                    new XAttribute("T", "-"),
                    new XAttribute("C", Data_Grid.Rows[k].Cells[5].Value)));
            }

            // Save data to XML
            dispensing_data.Save(data_path);
        }

        private void CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (first_start) // if does not use this condition, this event will run when program starts
            {
                // Save dispensing data to XML
                dispensing_data = XDocument.Load(data_path);

                // Remove current item
                get_item = dispensing_data.Element("Field")
                .Elements("Item")
                .Where(x => x.Element("Name").Value == item_name.Text)
                .Single();
                get_item.Element("Points").RemoveAll();

                for (int k = 0; k < Data_Grid.RowCount - 1; k++)
                {
                    get_item.Element("Points").Add(new XElement("Point",
                        new XAttribute("X", Data_Grid.Rows[k].Cells[1].Value),
                        new XAttribute("Y", Data_Grid.Rows[k].Cells[2].Value),
                        new XAttribute("Z", "-"),
                        new XAttribute("T", "-"),
                        new XAttribute("C", Data_Grid.Rows[k].Cells[5].Value)));
                }

                // Save data to XML
                dispensing_data.Save(data_path);

                LoadXml();

                status.Text = "Updated";
            }
        }

        private void SaveDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                // Update new painting points
                UpdateDatabase();

                // Refresh picture box
                points_tracking = template_img.Clone();
                point_num = 0;
                status.Text = "Saved";

            }
            catch (Exception)
            {
                MessageBox.Show("Exception thrown");
            }

        }

        private void pattern_field_MouseEnter(object sender, EventArgs e)
        {
            mouse_enter = true;
            MouseWheel += template_view_MouseWheel;
        }

        private void pattern_field_MouseMove(object sender, MouseEventArgs e)
        {
            //Draw_Cross(e);
            ViewPixelInfo(e);
        }

        private void pattern_field_MouseLeave(object sender, EventArgs e)
        {
            mouse_enter = !mouse_enter;
        }

        private void ItemNameChange(object sender, EventArgs e)
        {
            LoadXml();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CellClick(object sender, EventArgs e)
        {
            first_start = true;
        }

        private void _FromClosing(object sender, FormClosingEventArgs e)
        {
            CVEye.first_start01 = true;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
            CVEye.first_start01 = true;
        }

        private void view_template(string value)
        {
            switch (value)
            {
                //case "General 01":
                case "Tướng 01":
                    Reload_Template("pattern_data/01/gen01.jpg");
                    break;
                //case "Advisor 01":
                case "Sĩ 01":
                    Reload_Template("pattern_data/01/ad01.jpg");
                    break;
                //case "Elephant 01":
                case "Tượng 01":
                    Reload_Template("pattern_data/01/ele01.jpg");
                    break;
                //case "Chariot 01":
                case "Xe 01":
                    Reload_Template("pattern_data/01/cha01.jpg");
                    break;
                //case "Cannon 01":
                case "Pháo 01":
                    Reload_Template("pattern_data/01/can01.jpg");
                    break;
                //case "Horse 01":
                case "Ngựa 01":
                    Reload_Template("pattern_data/01/hor01.jpg");
                    break;
                //case "Soldier 01":
                case "Chốt 01":
                    Reload_Template("pattern_data/01/sol01.jpg");
                    break;
                //case "General 02":
                case "Tướng 02":
                    Reload_Template("pattern_data/02/gen02.jpg");
                    break;
                //case "Advisor 02":
                case "Sĩ 02":
                    Reload_Template("pattern_data/02/ad02.jpg");
                    break;
                //case "Elephant 02":
                case "Tượng 02":
                    Reload_Template("pattern_data/02/ele02.jpg");
                    break;
                //case "Chariot 02":
                case "Xe 02":
                    Reload_Template("pattern_data/02/cha02.jpg");
                    break;
                //case "Cannon 02":
                case "Pháo 02":
                    Reload_Template("pattern_data/02/can02.jpg");
                    break;
                //case "Horse 02":
                case "Ngựa 02":
                    Reload_Template("pattern_data/02/hor02.jpg");
                    break;
                //case "Soldier 02":
                case "Chốt 02":
                    Reload_Template("pattern_data/02/sol02.jpg");
                    break;
            }
        }
    }
}
