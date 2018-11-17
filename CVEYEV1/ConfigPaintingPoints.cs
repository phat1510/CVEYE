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
        private Image<Bgr, byte> TmpImg;
        private bool mouse_enter;
        private double x_real_pos, y_real_pos;
        private int x_axis, y_axis;

        private byte ChessSize;
        private double raw_tmp_size = 430;

        private double scale = 0;
        private double real_accuracy = 0;

        // XML
        private string DataPath;
        private string Chess_31_DataPath = "_chess31database.xml";
        private string Chess_29_DataPath = "_chess29database.xml";
        private XDocument DispensingData;
        private XElement GetItem;

        // Painting points array
        private Point[] painting_points = new Point[25];

        // Number of painting points
        private int point_num = 0;

        // Flags
        private bool clear = false;

        // Graphic
        Graphics drawing;

        public ConfigPaintingPoints()
        {
            InitializeComponent();

            InitValue();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitValue()
        {
            // 
            drawing = TmpImageBox.CreateGraphics();

            // 
            real_accuracy = CVEye._accuracy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void LoadTmpChess31(string value)
        {
            switch (value)
            {
                case "Tướng 01":
                    ReloadTemplate("pattern_data/01/cc11.jpg");
                    break;
                case "Sĩ 01":
                    ReloadTemplate("pattern_data/01/cc12.jpg");
                    break;
                case "Tượng 01":
                    ReloadTemplate("pattern_data/01/cc13.jpg");
                    break;
                case "Xe 01":
                    ReloadTemplate("pattern_data/01/cc14.jpg");
                    break;
                case "Pháo 01":
                    ReloadTemplate("pattern_data/01/cc15.jpg");
                    break;
                case "Ngựa 01":
                    ReloadTemplate("pattern_data/01/cc16.jpg");
                    break;
                case "Chốt 01":
                    ReloadTemplate("pattern_data/01/cc17.jpg");
                    break;
                case "Tướng 02":
                    ReloadTemplate("pattern_data/02/cc21.jpg");
                    break;
                case "Sĩ 02":
                    ReloadTemplate("pattern_data/02/cc22.jpg");
                    break;
                case "Tượng 02":
                    ReloadTemplate("pattern_data/02/cc23.jpg");
                    break;
                case "Xe 02":
                    ReloadTemplate("pattern_data/02/cc24.jpg");
                    break;
                case "Pháo 02":
                    ReloadTemplate("pattern_data/02/cc25.jpg");
                    break;
                case "Ngựa 02":
                    ReloadTemplate("pattern_data/02/cc26.jpg");
                    break;
                case "Chốt 02":
                    ReloadTemplate("pattern_data/02/cc27.jpg");
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private void LoadTmpChess29(string value)
        {
            switch (value)
            {
                case "Tướng 01":
                    ReloadTemplate("pattern_data/03/cc31.jpg");
                    break;
                case "Sĩ 01":
                    ReloadTemplate("pattern_data/03/cc32.jpg");
                    break;
                case "Tượng 01":
                    ReloadTemplate("pattern_data/03/cc33.jpg");
                    break;
                case "Xe 01":
                    ReloadTemplate("pattern_data/03/cc34.jpg");
                    break;
                case "Pháo 01":
                    ReloadTemplate("pattern_data/03/cc35.jpg");
                    break;
                case "Ngựa 01":
                    ReloadTemplate("pattern_data/03/cc36.jpg");
                    break;
                case "Chốt 01":
                    ReloadTemplate("pattern_data/03/cc37.jpg");
                    break;
                case "Tướng 02":
                    ReloadTemplate("pattern_data/04/cc41.jpg");
                    break;
                case "Sĩ 02":
                    ReloadTemplate("pattern_data/04/cc42.jpg");
                    break;
                case "Tượng 02":
                    ReloadTemplate("pattern_data/04/cc43.jpg");
                    break;
                case "Xe 02":
                    ReloadTemplate("pattern_data/04/cc44.jpg");
                    break;
                case "Pháo 02":
                    ReloadTemplate("pattern_data/04/cc45.jpg");
                    break;
                case "Ngựa 02":
                    ReloadTemplate("pattern_data/04/cc46.jpg");
                    break;
                case "Chốt 02":
                    ReloadTemplate("pattern_data/04/cc47.jpg");
                    break;
            }
        }

        /// <summary>
        /// Load selected template
        /// </summary>
        /// <param name="tmp_path"></param>
        private void ReloadTemplate(string tmp_path)
        {
            TmpImg = new Image<Bgr, byte>(tmp_path);
        }

        /// <summary>
        /// Load painting points data, view on datagrid and template picture box
        /// </summary>
        private void LoadXml()
        {
            try
            {
                // Select data path
                SetSelection(SetName.Text);

                // View template
                TmpImageBox.Image = TmpImg.Bitmap;
                TmpImageBox.Refresh();

                // Load data of set
                DispensingData = XDocument.Load(DataPath);

                // 
                ChessSize = byte.Parse(DispensingData.Element("Field").Element("SetData").Attribute("RoiSize").Value);
                scale = ChessSize / raw_tmp_size;

                // Call current item
                GetItem = DispensingData.Element("Field")
                    .Elements("Item")
                    .Where(x => x.Element("Name").Value == item_name.Text)
                    .Single();

                List<XElement> point_list = GetItem.Element("Points").Elements("Point").ToList();

                PointF[] draw_point = new PointF[2];
                int t = 0;
                int k = 1;

                // Clear grid data
                Data_Grid.Rows.Clear();

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
                        drawing.DrawLine(new Pen(Color.GreenYellow, 1.5f), draw_point[t - 1], draw_point[t]);
                        draw_point[t - 1] = draw_point[t];
                    }
                    else
                    {
                        draw_point[t] = new PointF((float)(float.Parse(element.Attribute("X").Value) / scale), (float)(float.Parse(element.Attribute("Y").Value) / scale));
                        t++;
                    }

                    drawing.DrawString(k.ToString(), new Font("Arial", 10.5f), new SolidBrush(Color.White), new Point((int)draw_point[0].X, (int)draw_point[0].Y));
                    k++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DrawPointsFromGrid()
        {
            byte pointinx = 0;
            PointF[] draw_point = new PointF[2];

            for (int idx = 0; idx < Data_Grid.RowCount - 1; idx++)
            {
                // Preview point tracking
                if (pointinx == 1)
                {
                    draw_point[pointinx] = new PointF((float)(float.Parse(Data_Grid.Rows[idx].Cells[1].Value.ToString()) / scale), (float)(float.Parse(Data_Grid.Rows[idx].Cells[2].Value.ToString()) / scale));
                    drawing.DrawLine(new Pen(Color.GreenYellow, 1.5f), draw_point[pointinx - 1], draw_point[pointinx]);
                    draw_point[pointinx - 1] = draw_point[pointinx];
                }
                else
                {
                    draw_point[pointinx] = new PointF(float.Parse(Data_Grid.Rows[0].Cells[1].Value.ToString()) / (float)scale, float.Parse(Data_Grid.Rows[0].Cells[2].Value.ToString()) / (float)scale);
                    pointinx++;
                }

                drawing.DrawString((idx + 1).ToString(), new Font("Arial", 10.5f), new SolidBrush(Color.White), new Point((int)draw_point[0].X, (int)draw_point[0].Y));

                byte _rect_size = 2;
                Rectangle _rect = new Rectangle(new Point((int)draw_point[0].X - _rect_size, (int)draw_point[0].Y - _rect_size), new Size(2 * _rect_size, 2 * _rect_size));
                drawing.DrawEllipse(new Pen(Color.GreenYellow, 1.5f), _rect);
            }
        }

        /// <summary>
        /// Collect painting points
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Template_Click(object sender, MouseEventArgs e)
        {
            if (clear || point_num == 0)
            {
                point_num++;

                clear = true;

                Data_Grid.Rows.Add(point_num, x_real_pos, y_real_pos, "-", jettingMode.Value, "-");
            }
        }

        /// <summary>
        /// Draw mouse position on template picture box
        /// </summary>
        /// <param name="e"></param>
        private void ViewPixelInfo(MouseEventArgs e)
        {
            TmpImageBox.Image = TmpImg.Bitmap;
            TmpImageBox.Refresh();

            LineSegment2DF vertical = new LineSegment2DF();
            LineSegment2DF horizontal = new LineSegment2DF();

            // Cursor in picture box position
            x_axis = e.Location.X + 1;
            y_axis = e.Location.Y + 1;
            CalculateLocalPosition();

            //
            DrawPointsFromGrid();

            vertical.P1 = new Point(x_axis, 0);
            vertical.P2 = new Point(x_axis, TmpImageBox.Height);
            horizontal.P1 = new Point(0, y_axis);
            horizontal.P2 = new Point(TmpImageBox.Width, y_axis);

            drawing.DrawLine(new Pen(Color.Blue, 1.5f), vertical.P1, vertical.P2);
            drawing.DrawLine(new Pen(Color.Blue, 1.5f), horizontal.P1, horizontal.P2);

        }

        /// <summary>
        /// 
        /// </summary>
        public void CalculateLocalPosition()
        {
            // Calculate real painting point local coordinate
            x_real_pos = Math.Round(x_axis * scale, 1);
            y_real_pos = Math.Round(y_axis * scale, 1);

            // Show value
            x_pos_lb.Text = x_real_pos.ToString();
            y_pos_lb.Text = y_real_pos.ToString();
        }

        private void ClearTracking_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialog = MessageBox.Show("Bạn muốn xóa toàn bộ điểm sơn?", "CVEye",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialog == DialogResult.Yes)
                {
                    painting_points = new Point[25];

                    // Refresh picture box
                    TmpImageBox.Image = TmpImg.Bitmap;
                    TmpImageBox.Refresh();

                    // Reset number of points
                    point_num = 0;

                    // Clear datagridview
                    Data_Grid.Rows.Clear();

                    // Remove current item
                    GetItem = DispensingData.Element("Field")
                    .Elements("Item")
                    .Where(x => x.Element("Name").Value == item_name.Text)
                    .Single();

                    if (GetItem != null)
                        GetItem.Element("Points").RemoveAll();
                    else
                        MessageBox.Show("Nothing to clear");
                    //
                    DispensingData.Save(DataPath);

                    // Set clear flag
                    clear = true;

                    //
                    status.Text = "Đã xóa toàn bộ điểm sơn";
                }
                else return;
            }
            catch (Exception)
            {
                MessageBox.Show("Exception thrown");

                // Refresh picture box
                TmpImageBox.Image = TmpImg.Bitmap;
                TmpImageBox.Refresh();
                point_num = 0;

                // Clear datagridview
                Data_Grid.Rows.Clear();
            }
        }

        private void UpdateDatabase()
        {
            // Reload
            DispensingData = XDocument.Load(DataPath);

            // Remove current item
            GetItem = DispensingData.Element("Field")
            .Elements("Item")
            .Where(x => x.Element("Name").Value == item_name.Text)
            .Single();
            GetItem.Element("Points").RemoveAll();

            if (status.Text != "Cleared")
                point_num = Data_Grid.RowCount - 1;

            for (int k = 0; k < point_num; k++)
            {
                float x = float.Parse(Data_Grid.Rows[k].Cells[1].Value.ToString());
                x = (float)Math.Round(x, 1);
                float y = float.Parse(Data_Grid.Rows[k].Cells[2].Value.ToString());
                y = (float)Math.Round(y, 1);

                double __scale = 1;

                GetItem.Element("Points").Add(new XElement("Point",
                    new XAttribute("X", Math.Round(x / __scale, 1)),
                    new XAttribute("Y", Math.Round(y / __scale, 1)),
                    new XAttribute("Z", "-"),
                    new XAttribute("T", Data_Grid.Rows[k].Cells[4].Value),
                    new XAttribute("C", "-")));
            }

            // Save data to XML
            DispensingData.Save(DataPath);
        }

        /// <summary>
        /// 
        /// </summary>
        private void ViewCurrentPoint()
        {
            try
            {
                if (Data_Grid.RowCount > 1)
                {
                    // View template image in background
                    TmpImageBox.Image = TmpImg.Bitmap;
                    TmpImageBox.Refresh();

                    // Initial condition
                    PointF[] draw_point = new PointF[2];
                    int pointinx = 0;

                    for (int idx = 0; idx < Data_Grid.RowCount - 1; idx++)
                    {
                        // Preview point tracking
                        if (pointinx == 1)
                        {
                            draw_point[pointinx] = new PointF((float)(float.Parse(Data_Grid.Rows[idx].Cells[1].Value.ToString()) / scale), (float)(float.Parse(Data_Grid.Rows[idx].Cells[2].Value.ToString()) / scale));
                            drawing.DrawLine(new Pen(Color.GreenYellow, 1.5f), draw_point[pointinx - 1], draw_point[pointinx]);
                            draw_point[pointinx - 1] = draw_point[pointinx];
                        }
                        else
                        {
                            draw_point[pointinx] = new PointF(float.Parse(Data_Grid.Rows[0].Cells[1].Value.ToString()) / (float)scale, float.Parse(Data_Grid.Rows[0].Cells[2].Value.ToString()) / (float)scale);
                            pointinx++;
                        }

                        byte _rect_size = 4;
                        Rectangle _rect = new Rectangle(new Point((int)draw_point[0].X - _rect_size, (int)draw_point[0].Y - _rect_size), new Size(2 * _rect_size, 2 * _rect_size));

                        if (Data_Grid.CurrentRow.Cells[0].Value.ToString() == (idx + 1).ToString())
                            drawing.DrawEllipse(new Pen(Color.GreenYellow, 3f), _rect);
                        else
                        {
                            byte _rect_size_small = 2;
                            Rectangle _rect_small = new Rectangle(new Point((int)draw_point[0].X - _rect_size_small, (int)draw_point[0].Y - _rect_size_small), new Size(2 * _rect_size_small, 2 * _rect_size_small));
                            drawing.DrawEllipse(new Pen(Color.GreenYellow, 1.5f), _rect_small);
                            drawing.DrawString((idx + 1).ToString(), new Font("Arial", 10.5f), new SolidBrush(Color.White), new Point((int)draw_point[0].X, (int)draw_point[0].Y));
                        }
                    }
                }

                status.Text = "Point " + (Data_Grid.CurrentCell.RowIndex + 1).ToString() + " is selected";
            }
            catch (Exception ex)
            {
                status.Text = ex.Message;
            }
        }

        /// <summary>
        /// Select painting points data file path of a set
        /// </summary>
        /// <param name="value"></param>
        private void SetSelection(string value)
        {
            switch (value)
            {
                case "Cờ 31":
                    // Reload template image file
                    LoadTmpChess31(item_name.Text);

                    // Update XML file path
                    DataPath = Chess_31_DataPath;

                    // Update status
                    status.Text = "Đã tải dữ liệu cờ 31";
                    break;
                case "Cờ 29":
                    // Update XML file path
                    DataPath = Chess_29_DataPath;

                    // Reload template file
                    LoadTmpChess29(item_name.Text);

                    // Update status
                    status.Text = "Đã tải dữ liệu cờ 29";
                    break;                    
            }
        }

        //----------------------------------
        //----------------------------------
        //----------------------------------
        //********** Form events ***********
        //----------------------------------
        //----------------------------------
        //----------------------------------

        private void Name_Changed(object sender, EventArgs e)
        {
            LoadXml();
            point_num = 0;
        }

        private void SetName_TextChanged(object sender, EventArgs e)
        {
            LoadXml();
            point_num = 0;
        }

        private void SaveDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                // Update new painting points
                UpdateDatabase();

                //
                LoadXml();

                //
                clear = false;
                point_num = 0;
                status.Text = "Đã lưu";
            }
            catch (Exception ex)
            {
                status.Text = ex.Message;
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

        private void ConfigPaintingPoints_Shown(object sender, EventArgs e)
        {
            LoadXml();
        }

        private void AddRow_Click(object sender, EventArgs e)
        {
            try
            {
                // View template image in background
                TmpImageBox.Image = TmpImg.Bitmap;
                TmpImageBox.Refresh();

                Data_Grid.Rows.Insert(Data_Grid.CurrentCell.RowIndex,
                    Data_Grid.CurrentRow.Cells[0].Value,
                    Data_Grid.CurrentRow.Cells[1].Value,
                    Data_Grid.CurrentRow.Cells[2].Value,
                    Data_Grid.CurrentRow.Cells[3].Value,
                    Data_Grid.CurrentRow.Cells[4].Value,
                    Data_Grid.CurrentRow.Cells[5].Value);

                for (int i = 0; i < Data_Grid.RowCount - 1; i++)
                {
                    Data_Grid.Rows[i].Cells[0].Value = i + 1;
                }

                DrawPointsFromGrid();
            }
            catch (Exception ex)
            {
                status.Text = ex.Message;
            }
        }

        private void DeleteRow_Click(object sender, EventArgs e)
        {
            try
            {
                // View template image in background
                TmpImageBox.Image = TmpImg.Bitmap;
                TmpImageBox.Refresh();

                Data_Grid.Rows.Remove(Data_Grid.CurrentRow);

                for (int i = 0; i < Data_Grid.RowCount - 1; i++)
                {
                    Data_Grid.Rows[i].Cells[0].Value = i + 1;
                }

                DrawPointsFromGrid();
            }
            catch (Exception ex)
            {
                status.Text = ex.Message;
            }
        }

        private void Data_Grid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void Data_Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ViewCurrentPoint();
        }

        private void pattern_field_MouseMove(object sender, MouseEventArgs e)
        {
            if (clear)
                ViewPixelInfo(e);
        }

        private void ConfigPaintingPoints_Load(object sender, EventArgs e)
        {
        }

        private void yPlus_Click(object sender, EventArgs e)
        {
            float value;
            value = float.Parse(Data_Grid.CurrentRow.Cells[2].Value.ToString());

            if (fastMoving.CheckState == CheckState.Checked)
                value += 1;
            else
                value += 0.2f;

            value = (float)Math.Round(value, 1);
            Data_Grid.CurrentRow.Cells[2].Value = value.ToString();

            TmpImageBox.Image = TmpImg.Bitmap;
            TmpImageBox.Refresh();

            DrawPointsFromGrid();
        }

        private void yMinus_Click(object sender, EventArgs e)
        {
            float value;
            value = float.Parse(Data_Grid.CurrentRow.Cells[2].Value.ToString());

            if (fastMoving.CheckState == CheckState.Checked)
                value -= 1;
            else
                value -= 0.2f;

            value = (float)Math.Round(value, 1);
            Data_Grid.CurrentRow.Cells[2].Value = value.ToString();

            TmpImageBox.Image = TmpImg.Bitmap;
            TmpImageBox.Refresh();

            DrawPointsFromGrid();
        }

        private void xPlus_Click(object sender, EventArgs e)
        {
            float value;
            value = float.Parse(Data_Grid.CurrentRow.Cells[1].Value.ToString());

            if (fastMoving.CheckState == CheckState.Checked)
                value += 1;
            else
                value += 0.2f;

            value = (float)Math.Round(value, 1);
            Data_Grid.CurrentRow.Cells[1].Value = value.ToString();

            TmpImageBox.Image = TmpImg.Bitmap;
            TmpImageBox.Refresh();

            DrawPointsFromGrid();
        }

        private void xMinus_Click(object sender, EventArgs e)
        {
            float value;
            value = float.Parse(Data_Grid.CurrentRow.Cells[1].Value.ToString());

            if (fastMoving.CheckState == CheckState.Checked)
                value -= 1;
            else
                value -= 0.2f;

            value = (float)Math.Round(value, 1);
            Data_Grid.CurrentRow.Cells[1].Value = value.ToString();

            TmpImageBox.Image = TmpImg.Bitmap;
            TmpImageBox.Refresh();

            DrawPointsFromGrid();
        }

        private void pattern_field_MouseLeave(object sender, EventArgs e)
        {
            mouse_enter = !mouse_enter;
        }
        
        private void pattern_field_MouseEnter(object sender, EventArgs e)
        {
            mouse_enter = true;
        }
    }
}
