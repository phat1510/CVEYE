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
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

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
        private Image<Bgr, byte> points_tracking;

        private double mouse_val;
        private bool mouse_enter;

        private double x_real_pos, y_real_pos;
        private int x_axis, y_axis;
        private double real_tmp_size = 121;
        private double raw_tmp_size = 430;
        private double scale = 0;
        private string data_path = "_database.xml";

        private Point[] painting_points = new Point[25];

        private int point_num = 0;

        private XDocument dispensing_data;
        private XElement get_item;

        private double real_accuracy = 0;

        private bool clear =  false;
        private bool first_start = false;
        //private bool gridEnter = false;

        public ConfigPaintingPoints()
        {
            InitializeComponent();
            
            scale = real_tmp_size / raw_tmp_size;
            real_accuracy = CVEye._accuracy;
        }

        // Load points data to data table
        private void LoadXml()
        {
            try
            {
                //
                LoadTmp(item_name.Text);

                // Clear and load points data
                Data_Grid.Rows.Clear();
                dispensing_data = XDocument.Load(data_path);

                // Call current item
                get_item = dispensing_data.Element("Field")
                    .Elements("Item")
                    .Where(x => x.Element("Name").Value == item_name.Text)
                    .Single();

                List<XElement> point_list = get_item.Element("Points").Elements("Point").ToList();

                PointF[] draw_point = new PointF[2];
                int t = 0;
                int k = 1;

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
                            new Bgr(Color.Cyan), 1);
                        draw_point[t - 1] = draw_point[t];
                    }
                    else
                    {
                        draw_point[t] = new PointF((float)(float.Parse(element.Attribute("X").Value) / scale), (float)(float.Parse(element.Attribute("Y").Value) / scale));
                        t++;
                    }

                    points_tracking.Draw(k.ToString(), new Point((int)draw_point[0].X, (int)draw_point[0].Y), FontFace.HersheySimplex, 0.5, new Bgr(Color.YellowGreen));

                    k++;
                }

                // 
                templateImg.Image = points_tracking.Bitmap;
                templateImg.Refresh();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Name_Changed(object sender, EventArgs e)
        {
            LoadXml();
            point_num = 0;
        }

        private void ViewPixelInfo(MouseEventArgs e)
        {
            using (Image<Bgr, byte> tmp = points_tracking.Clone())
            {
                LineSegment2DF vertical = new LineSegment2DF();
                LineSegment2DF horizontal = new LineSegment2DF();

                // Cursor in picture box position
                x_axis = e.Location.X + 1;
                y_axis = e.Location.Y + 1;

                CalculateLocalPosition();

                vertical.P1 = new Point(x_axis, 0);
                vertical.P2 = new Point(x_axis, templateImg.Height);
                horizontal.P1 = new Point(0, y_axis);
                horizontal.P2 = new Point(templateImg.Width, y_axis);

                tmp.Draw(vertical, new Bgr(Color.Blue), 1);
                tmp.Draw(horizontal, new Bgr(Color.Blue), 1);

                double needle_cir;
                needle_cir = ((double)needle_dia.Value / real_accuracy) / scale;

                tmp.Draw(new CircleF(new PointF(x_axis, y_axis), (float)needle_cir / 2), new Bgr(Color.GreenYellow), 1);

                templateImg.Image = tmp.Bitmap;
                templateImg.Refresh();
            }
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
                        new Bgr(Color.Cyan), 1);
                }
                clear = true;
                point_num++;

                Data_Grid.Rows.Add(point_num, x_real_pos, y_real_pos, "-", "-", item_color.Text);
            }
        }

        public void CalculateLocalPosition()
        {
            // Calculate real painting point local coordinate
            x_real_pos = Math.Round(x_axis * scale, 1);
            y_real_pos = Math.Round(y_axis * scale, 1);

            // Show value
            x_pos_lb.Text = x_real_pos.ToString();
            y_pos_lb.Text = y_real_pos.ToString();
        }

        private void DataGridMouseWheel(object sender, MouseEventArgs e)
        {
            using (Image<Bgr, byte> tmp = points_tracking.Clone())
            {
                mouse_val += e.Delta / 120;
                status.Text = mouse_val.ToString();

                Data_Grid.CurrentCell.Value = Math.Round(float.Parse(Data_Grid.CurrentCell.Value.ToString()) + (float)e.Delta / 240, 1).ToString();
                Data_Grid.Refresh();

                int pointinx = 0;
                PointF[] draw_point = new PointF[2];
                for (int idx = 0; idx < Data_Grid.RowCount - 1; idx++)
                {
                    // Preview point tracking
                    if (pointinx == 1)
                    {
                        draw_point[pointinx] = new PointF((float)(float.Parse(Data_Grid.Rows[idx].Cells[1].Value.ToString()) / scale), (float)(float.Parse(Data_Grid.Rows[idx].Cells[2].Value.ToString()) / scale));
                        tmp.Draw(new LineSegment2DF(draw_point[pointinx - 1], draw_point[pointinx]),
                            new Bgr(Color.Cyan), 1);
                        draw_point[pointinx - 1] = draw_point[pointinx];
                    }
                    else
                    {
                        draw_point[pointinx] = new PointF(float.Parse(Data_Grid.Rows[0].Cells[1].Value.ToString()) / (float)scale, float.Parse(Data_Grid.Rows[0].Cells[2].Value.ToString()) / (float)scale);
                        //draw_point[pointinx] = new PointF(0, 0);
                        pointinx++;
                    }

                    tmp.Draw((idx + 1).ToString(), new Point((int)draw_point[0].X, (int)draw_point[0].Y), FontFace.HersheySimplex, 0.5, new Bgr(Color.YellowGreen));
                }

                // 
                templateImg.Image = tmp.Bitmap;
                templateImg.Refresh();
            }
        }

        private void ClearTracking_Click(object sender, EventArgs e)
        {
            try
            {
                painting_points = new Point[25];

                // Refresh picture box
                templateImg.Image = template_img.Bitmap;
                points_tracking = template_img.Clone();
                point_num = 0;

                // Clear datagridview
                Data_Grid.Rows.Clear();

                // Remove current item
                get_item = dispensing_data.Element("Field")
                .Elements("Item")
                .Where(x => x.Element("Name").Value == item_name.Text)
                .Single();

                if (get_item != null)
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
                templateImg.Image = template_img.Bitmap;
                points_tracking = template_img.Clone();
                point_num = 0;

                // Clear datagridview
                Data_Grid.Rows.Clear();
            }
        }

        private void UpdateDatabase()
        {
            // Reload
            dispensing_data = XDocument.Load(data_path);

            // Remove current item
            get_item = dispensing_data.Element("Field")
            .Elements("Item")
            .Where(x => x.Element("Name").Value == item_name.Text)
            .Single();
            get_item.Element("Points").RemoveAll();

            if (status.Text != "Cleared")
                point_num = Data_Grid.RowCount - 1;

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

        private void ViewCurrentPoint()
        {
            using (Image<Bgr, byte> tmp = points_tracking.Clone())
            {
                if (Data_Grid.RowCount > 1)
                {
                    PointF currentPoint = new PointF(float.Parse(Data_Grid.CurrentRow.Cells[1].Value.ToString()) / (float)scale,
                        float.Parse(Data_Grid.CurrentRow.Cells[2].Value.ToString()) / (float)scale);

                    tmp.Draw(new CircleF(currentPoint, 5), new Bgr(Color.GreenYellow), 2);

                    templateImg.Image = tmp.Bitmap;
                    templateImg.Refresh();
                }
            }

            status.Text = "Cell is selected";

            first_start = true;
        }

        #region Datagrid Events

        private void CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (first_start) // if does not use this condition, this event will run when program starts
            {
                using (Image<Bgr, byte> tmp = template_img.Clone())
                {
                    PointF[] draw_point = new PointF[2];
                    int t = 0;

                    for (int k = 0; k < Data_Grid.RowCount - 1; k++)
                    {
                        float currentPointX = (float)(float.Parse(Data_Grid.Rows[k].Cells[1].Value.ToString()) / scale);
                        float currentPointY = (float)(float.Parse(Data_Grid.Rows[k].Cells[2].Value.ToString()) / scale);

                        PointF currentPoint = new PointF(currentPointX, currentPointY);

                        // Preview point tracking
                        if (t == 1)
                        {
                            draw_point[t] = currentPoint;

                            tmp.Draw(new LineSegment2DF(draw_point[t - 1], draw_point[t]),
                                new Bgr(Color.Cyan), 1);
                            draw_point[t - 1] = draw_point[t];
                        }
                        else
                        {
                            draw_point[t] = currentPoint;
                            t++;
                        }

                        tmp.Draw((k + 1).ToString(), new Point((int)draw_point[0].X, (int)draw_point[0].Y), FontFace.HersheySimplex, 0.5, new Bgr(Color.YellowGreen));
                    }

                    // 
                    templateImg.Image = tmp.Bitmap;
                    templateImg.Refresh();
                }
            }
        }

        private void CellClick(object sender, EventArgs e)
        {
            ViewCurrentPoint();
        }

        private void Data_Grid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            //ViewCurrentPoint();
        }

        #endregion

        private void ItemNameChange(object sender, EventArgs e)
        {
            LoadXml();
        }

        private void SaveDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                // Update new painting points
                UpdateDatabase();

                LoadXml();

                // Refresh picture box
                //points_tracking = template_img.Clone();
                point_num = 0;
                status.Text = "Saved";

            }
            catch (Exception)
            {
                MessageBox.Show("Exception thrown");
            }

        }

        private void OK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void pattern_field_MouseEnter(object sender, EventArgs e)
        {
            mouse_enter = true;
        }

        private void pattern_field_MouseMove(object sender, MouseEventArgs e)
        {
            if (status.Text == "Cleared")
                ViewPixelInfo(e);
        }

        private void pattern_field_MouseLeave(object sender, EventArgs e)
        {
            mouse_enter = !mouse_enter;
        }

        private void ConfigPaintingPoints_Load(object sender, EventArgs e)
        {
            LoadXml();
        }

        private void Reload_Template(string tmp_path)
        {
            template_img = new Image<Bgr, byte>(tmp_path);
            points_tracking = template_img.Clone();
        }

        private void LoadTmp(string value)
        {
            switch (value)
            {
                //case "General 01":
                case "Tướng 01":
                    Reload_Template("pattern_data/01/cc11.jpg");
                    break;
                //case "Advisor 01":
                case "Sĩ 01":
                    Reload_Template("pattern_data/01/cc12.jpg");
                    break;
                //case "Elephant 01":
                case "Tượng 01":
                    Reload_Template("pattern_data/01/cc13.jpg");
                    break;
                //case "Chariot 01":
                case "Xe 01":
                    Reload_Template("pattern_data/01/cc14.jpg");
                    break;
                //case "Cannon 01":
                case "Pháo 01":
                    Reload_Template("pattern_data/01/cc15.jpg");
                    break;
                //case "Horse 01":
                case "Ngựa 01":
                    Reload_Template("pattern_data/01/cc16.jpg");
                    break;
                //case "Soldier 01":
                case "Chốt 01":
                    Reload_Template("pattern_data/01/cc17.jpg");
                    break;
                //case "General 02":
                case "Tướng 02":
                    Reload_Template("pattern_data/02/cc21.jpg");
                    break;
                //case "Advisor 02":
                case "Sĩ 02":
                    Reload_Template("pattern_data/02/cc22.jpg");
                    break;
                //case "Elephant 02":
                case "Tượng 02":
                    Reload_Template("pattern_data/02/cc23.jpg");
                    break;
                //case "Chariot 02":
                case "Xe 02":
                    Reload_Template("pattern_data/02/cc24.jpg");
                    break;
                //case "Cannon 02":
                case "Pháo 02":
                    Reload_Template("pattern_data/02/cc25.jpg");
                    break;
                //case "Horse 02":
                case "Ngựa 02":
                    Reload_Template("pattern_data/02/cc26.jpg");
                    break;
                //case "Soldier 02":
                case "Chốt 02":
                    Reload_Template("pattern_data/02/cc27.jpg");
                    break;
            }
        }
    }
}
