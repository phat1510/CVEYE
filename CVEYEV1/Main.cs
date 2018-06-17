/*
Project title: Autonomous Vision Dispensing Machine
Author: Phat Do
Starting time:
Ending time:
Decription:
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
//using System.Windows.Forms.DataVisualization.Charting;
using System.Runtime.InteropServices;

// EmguCV library
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.VideoSurveillance;
using Emgu.CV.Features2D;

// Mach library
using Mach4;

namespace CVEYEV1
{
    public partial class CVEye : Form
    {
        #region Subform
        private ConfigPaintingPoints Con_Painting_Point;
        private ConfigImageProcessing Con_Image_Processing;
        #endregion

        #region Mach3
        private IMach4 mach3;
        private IMyScriptObject scriptObject;
        #endregion

        #region User32.dll
        [DllImport("USER32.DLL")]

        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("USER32.DLL", EntryPoint = "FindWindow", SetLastError = true)]

        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        //[DllImport("USER32.DLL")]
        //public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        public const int SW_MAXIMIZE = 3;
        public const int SW_MINIMIZE = 6;
        #endregion

        #region Camera Calibration
        public static bool cameraOn = false;
        public static bool start_calib = false;
        private bool find_chessboard;
        private VideoCapture _capture;
        private const int screen_height = 1944;
        private const int screen_width = 2592;
        private const int pt_width = 50;
        private const int pt_height = 38;
        private float square_size = 10f;
        private Size pt_size = new Size(pt_width, pt_height);
        private VectorOfPointF corners = new VectorOfPointF();
        private Bgr[] line_color_array = new Bgr[pt_width * pt_height];
        private static Mat[] frame_array_buffer = new Mat[30];
        private int frame_buffer_savepoint = 0;        
        public enum Mode
        {
            Caluculating_Intrinsics,
            Calibrated,
            SavingFrames
        }
        public static Mode currentMode = Mode.Calibrated;
        #endregion

        #region Getting camera calibration data
        public static MCvPoint3D32f[][] corners_object_list;
        public static PointF[][] corners_point_list;
        public static VectorOfPointF[] corners_point_vector;
        Mat[] rvecs, tvecs;
        Mat cameraMat = new Mat(3, 3, DepthType.Cv64F, 1);
        Mat distCoeffsMat = new Mat(14, 1, DepthType.Cv64F, 1);
        #endregion

        #region Image Processing
        public static Image<Bgr, byte> img_capture;
        public static Image<Bgr, byte> img_capture_undist;
        public static Image<Bgr, byte> img_draw;
        public Mat tmp_raw = new Mat();
        private Mat img_raw = new Mat();
        private Mat img_gray = new Mat();
        private Mat img_threshold = new Mat();
        #endregion

        #region IO
        private string mainDirectory;
        private string mach3Directory;
        private string macroDirectory;
        public static XDocument SysData;        
        public static TextWriter gcode;
        private bool first_item = true;
        #endregion

        #region Miscellaneous

        const float in_circle_radius = (float)11.9; //mm

        const double x_pixel_offset = -43; // pixels
        const double y_pixel_offset = 9;

        List<double> angleListSort = new List<double>();

        const decimal circle_deep_offset = -(decimal)0.2; //mm

        public static bool first_start01 = true;
        public static bool first_start02 = true;

        private bool lowSpeed = true;
        
        private bool xZero = false;
        private bool yZero = false;
        private bool zZero = false;

        private bool painting = false;
        private bool detected = false;
        private bool machinecoord = true;
        #endregion

        public CVEye()
        {
            InitializeComponent();
            
            Init_XML();

            Init_Directory();

            Init_Camera();

            Init_Subform();

            Init_Graphic();
        }

        private void Init_Directory()
        {
            // Get program directory
            mainDirectory = Directory.GetCurrentDirectory();           

            // Link to directory node
            XElement directory = SysData.Element("System").Element("Directory");
            mach3Directory = directory.Element("Mach3").Attribute("directory").Value;
            macroDirectory = Path.Combine(mach3Directory, @"macros\Mach3Mill");

            // 
            gcode = new StreamWriter(Path.Combine(macroDirectory, @"M999.m1s"));
        }
        private void GetMach3Instance()
        {
            try
            {
                mach3 = (IMach4)Marshal.GetActiveObject("Mach4.Document");
                scriptObject = (IMyScriptObject)mach3.GetScriptDispatch();
            }
            catch
            {
                mach3 = null;
                scriptObject = null;
            }
        }

        private void Init_Mach3()
        {
            try
            {
                // Link to Mach3 directory
                Directory.SetCurrentDirectory(mach3Directory);

                // Start Mach3
                Process.Start("Mach3.lnk");
                Thread.Sleep(1500);

                // Send CVEye window to front
                IntPtr hwnd = FindWindowByCaption(IntPtr.Zero, "CVEye");
                SetForegroundWindow(hwnd);

                // Link to CVEye directory
                Directory.SetCurrentDirectory(mainDirectory);

                GetMach3Instance();
                if (scriptObject != null)
                {
                    // Machine coordinate toggle
                    scriptObject.DoOEMButton(256);

                    // Reset OEM
                    scriptObject.DoOEMButton(1021);
                    
                    // Set to low speed mode
                    LowSpeedMode();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Mach3RunMacros(string macroName)
        {
            GetMach3Instance();

            if (scriptObject != null)
                scriptObject.Code(macroName);
        }

        private void UpdateDRO()
        {
            try
            {
                GetMach3Instance();

                if (scriptObject != null)
                {
                    // Update x y z DROs
                    xDRO.Text = Math.Round(scriptObject.GetOEMDRO(800), 3).ToString("0.000");
                    yDRO.Text = Math.Round(scriptObject.GetOEMDRO(801), 3).ToString("0.000");
                    zDRO.Text = Math.Round(scriptObject.GetOEMDRO(802), 3).ToString("0.000");

                    // Update blended velocity DRO
                    feedrateDRO.Text = scriptObject.GetOEMDRO(813).ToString("0");

                    //// Collect homing conditions
                    //xZero = (xDRO.Text == "0.000") ? true : false;
                    //yZero = (yDRO.Text == "0.000") ? true : false;
                    //zZero = (zDRO.Text == "0.000") ? true : false;

                    //// Reset Motor rapid speed
                    //if (xZero && yZero && zZero && !reset_speed)
                    //{
                    //    //// Reset speed and return working position
                    //    //scriptObject.Code("M93");

                    //    //xZero = false;
                    //    //yZero = false;
                    //    //zZero = false;

                    //    //reset_speed = true;
                    //}

                    machStatus.Text = (scriptObject.IsActive(25) != 0) ? "Emergency Mode" : "Ready";
                    machStatus.Refresh();

                    if (!xZero)
                    {
                        ledX.BackColor = Color.Gray;

                        if (scriptObject.IsActive(2) != 0)
                        {
                            ledX.BackColor = Color.GreenYellow;
                            xZero = true;
                        }
                    }

                    if (!yZero)
                    {
                        ledY.BackColor = Color.Gray;

                        if (scriptObject.IsActive(5) != 0)
                        {
                            ledY.BackColor = Color.GreenYellow;
                            yZero = true;
                        }
                    }

                    if (!zZero)
                    {
                        ledZ.BackColor = Color.Gray;

                        if (scriptObject.IsActive(8) != 0)
                        {
                            ledZ.BackColor = Color.GreenYellow;
                            zZero = true;
                        }
                    }
                    // painting completed condition and enable some buttons !!!!!!!!!!!!!!
                }
            }
            catch
            {
                return;
            }
        }

        private void HighSpeedMode()
        {
            // x, y ~20000mm/min
            // z ~15000mm/min
            scriptObject.Code("M91");
            lowSpeed = false;
        }

        private void LowSpeedMode()
        {
            scriptObject.Code("M94");
            lowSpeed = true;
        }

        private void Init_Camera()
        {
            if (_capture == null)
            {
                //Try to create the capture
                try
                {
                    _capture = new VideoCapture(1);
                    _capture.SetCaptureProperty(CapProp.FrameHeight, screen_height); // pixels
                    _capture.SetCaptureProperty(CapProp.FrameWidth, screen_width); // pixels
                }
                catch (NullReferenceException excpt)
                {
                    //Show errors if there is any
                    MessageBox.Show(excpt.Message);
                }
            }

            if (_capture != null)
            {
                // Add handler for getting camera frame
                Application.Idle += new EventHandler(Frame_Calibration);
                _capture.Start();
            }

            if (_capture.IsOpened)
            {
                cameraOn = true;
                status_label.Text = "Camera On";
                status_label.Refresh();
            }
            else
            {
                status_label.Text = "Camera Off";
                status_label.Refresh();
            }
        }

        private void Init_XML()
        {
            // Read camera calibration matrices
            LoadCameraData();

            // Read system parameters
            ReadSystemData();
        }

        private void LoadCameraData()
        {
            FileNode fn;
            FileStorage fs = new FileStorage("_camera.xml", FileStorage.Mode.Read);
            fn = fs.GetNode("Camera_Matrix");
            fn.ReadMat(cameraMat);
            fn = fs.GetNode("Distortion_Coefficients");
            fn.ReadMat(distCoeffsMat);
        }

        private void ReadSystemData()
        {
            SysData = XDocument.Load("_system.xml");

            tmp_item_name.Text = SysData.Element("System").Element("MainWindow").Element("Template").Attribute("WorkingItem").Value;
            item_color.Text = SysData.Element("System").Element("MainWindow").Element("Template").Attribute("WorkingColor").Value;
            valveNum.Text = (GetValveNum(item_color.Text) - 3).ToString();

            // View the previous working template
            ViewTemplate(tmp_item_name.Text);
        }

        private void WriteSystemData()
        {
            SysData = XDocument.Load("_system.xml");

            bool clear_XML = false;
            if (clear_XML)
            {
                // Setting
                XmlTextWriter writer = new XmlTextWriter("_system.xml", System.Text.Encoding.UTF8);
                writer.WriteStartDocument(true);
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 2;

                // Config database structure
                writer.WriteStartElement("System");
                writer.WriteEndDocument();
                writer.Close();

                // Load new XML file
                SysData.Element("System").Add(new XElement("MainWindow"));

                // Camera Calibration window data
                SysData.Element("System").Add(new XElement("CameraCalibrationWindow", new XElement("Compensation")));
                
                // Painting Points window data
                SysData.Element("System").Add(new XElement("PaintingPointsWindow"));

                // Image Processing window data
                SysData.Element("System").Add(new XElement("ImageProcessingWindow"));

                // Painting Condition window data
                SysData.Element("System").Add(new XElement("PaintingConditionWindow"));
            }

            // Remove all element in "MainWindow" node
            SysData.Element("System").Element("MainWindow").RemoveAll();

            // Update data of main window
            SysData.Element("System").Element("MainWindow").Add(new XElement("Template",
                new XAttribute("WorkingItem", tmp_item_name.Text),
                new XAttribute("WorkingColor", item_color.Text)
                ));

            // Save document
            SysData.Save("_system.xml");
        }

        private void Init_Subform()
        {
            Con_Image_Processing = new ConfigImageProcessing();
            Con_Painting_Point = new ConfigPaintingPoints();
        }

        private void Init_Graphic()
        {
            ledX.BackColor = Color.Gray;
            ledY.BackColor = Color.Gray;
            ledZ.BackColor = Color.Gray;
        }

        // Get frame with camera calibration
        private void Frame_Calibration(object sender, EventArgs e)
        {
            try
            {
                Mat frame_raw = new Mat();
                Mat frame_gray = new Mat();

                // Capture current frame and convert to grayscale
                _capture.Retrieve(frame_raw);
                CvInvoke.CvtColor(frame_raw, frame_gray, ColorConversion.Bgr2Gray);
                img_capture = frame_raw.ToImage<Bgr, byte>();

                // Calibration process
                if (currentMode == Mode.SavingFrames)
                {
                    find_chessboard = CvInvoke.FindChessboardCorners(frame_gray, pt_size, corners);

                    if (find_chessboard)
                    {
                        CvInvoke.CornerSubPix(frame_gray, corners, new Size(11, 11), new Size(-1, -1), new MCvTermCriteria(30, 0.1));

                        if (start_calib)
                        {
                            frame_array_buffer[frame_buffer_savepoint] = frame_gray;
                            frame_buffer_savepoint++;

                            //Check the state of buffer
                            if (frame_buffer_savepoint == frame_array_buffer.Length)
                            {
                                start_calib = false;
                                currentMode = Mode.Caluculating_Intrinsics; //Jump to next step
                            }
                        }
                        // Draw the found corners
                        img_capture.Draw(new CircleF(corners[0], 10), new Bgr(Color.Yellow), 4);
                        for (int i = 1; i < corners.Size; i++)
                        {
                            img_capture.Draw(new LineSegment2DF(corners[i - 1], corners[i]), new Bgr(Color.Blue), 3);
                            img_capture.Draw(new CircleF(corners[i], 10), new Bgr(Color.Yellow), 4);
                        }
                        Thread.Sleep(1000);
                    }
                    corners = new VectorOfPointF();
                    find_chessboard = false;
                }

                if (currentMode == Mode.Caluculating_Intrinsics)
                {
                    for (int k = 0; k < frame_array_buffer.Length; k++)
                    {
                        // Detect corners of each stored frame
                        corners_point_vector[k] = new VectorOfPointF();
                        CvInvoke.FindChessboardCorners(frame_array_buffer[k], pt_size, corners_point_vector[k]);

                        // For accuracy 
                        CvInvoke.CornerSubPix(frame_gray, corners_point_vector[k], new Size(11, 11), new Size(-1, -1), new MCvTermCriteria(30, 0.1));

                        // Fill objects list with the real world mesurments for the intrinsic calculations
                        List<MCvPoint3D32f> object_list = new List<MCvPoint3D32f>();
                        for (int i = 0; i < pt_height; i++)
                        {
                            for (int j = 0; j < pt_width; j++)
                            {
                                object_list.Add(new MCvPoint3D32f(j * square_size, i * square_size, 0.0F));
                            }
                        }

                        corners_object_list[k] = object_list.ToArray();
                        corners_point_list[k] = corners_point_vector[k].ToArray();
                    }

                    double error = CvInvoke.CalibrateCamera(
                        corners_object_list,
                        corners_point_list,
                        frame_gray.Size,
                        cameraMat,
                        distCoeffsMat,
                        CalibType.RationalModel,
                        new MCvTermCriteria(30, 0.05),
                        out rvecs,
                        out tvecs);
                    currentMode = Mode.Calibrated;

                    // Save Camera Calibration Matrices
                    FileStorage fs = new FileStorage("_camera.xml", FileStorage.Mode.Write);
                    fs.Write(cameraMat, "Camera_Matrix");
                    fs.Write(distCoeffsMat, "Distortion_Coefficients");
                }

                if (currentMode == Mode.Calibrated)
                {
                    // Apply Calibration
                    Mat clone_frame = frame_raw.Clone();
                    CvInvoke.Undistort(frame_raw, clone_frame, cameraMat, distCoeffsMat);
                    frame_raw = clone_frame.Clone();
                    img_capture_undist = frame_raw.ToImage<Bgr, byte>();
                    //img_capture = img_capture_undist.Clone(); // uncomment when calibrating
                    img_draw = img_capture_undist.Clone(); // comment when calibrating
                    Draw_Grid(img_draw); // comment when calibrating
                }

                // Show calibrated image
                pattern_field.Image = img_draw.Bitmap;
                //pattern_field.Image = img_capture.Bitmap;

            }
            catch
            {
                //MessageBox.Show(ex.Message);
                return;
            }
        }

        public static void CalibrationStart()
        {
            corners_object_list = new MCvPoint3D32f[frame_array_buffer.Length][];
            corners_point_list = new PointF[frame_array_buffer.Length][];
            corners_point_vector = new VectorOfPointF[frame_array_buffer.Length];
            start_calib = true;
            currentMode = Mode.SavingFrames;
        }

        // Apply affine transformation for painting points
        public void Real_PointsWarpAffine(List<XElement> point_list, PointF rot_center, double rot_angle, Image<Bgr, byte> draw_image_bgr)
        {
            ConfigPaintingPoints.affine_painting_points = new PointF[50];

            // Invert rotation direction
            double angle = Math.Round((360 - rot_angle) * Math.PI / 180, 3);

            // Reset number of points
            ConfigPaintingPoints.point_num = 0;

            foreach (XElement element in point_list)
            {
                // translation -> rotation -> translation
                ConfigPaintingPoints.affine_painting_points[ConfigPaintingPoints.point_num].X = (float)((double.Parse(element.Attribute("X").Value) - ConfigPaintingPoints.real_tmp_size / 2) * Math.Cos(angle)) - (float)(((double.Parse(element.Attribute("Y").Value) - ConfigPaintingPoints.real_tmp_size / 2) * Math.Sin(angle))) + rot_center.X;
                ConfigPaintingPoints.affine_painting_points[ConfigPaintingPoints.point_num].Y = (float)((double.Parse(element.Attribute("X").Value) - ConfigPaintingPoints.real_tmp_size / 2) * Math.Sin(angle)) + (float)(((double.Parse(element.Attribute("Y").Value) - ConfigPaintingPoints.real_tmp_size / 2) * Math.Cos(angle))) + rot_center.Y;

                //draw_image_bgr.Draw(new CircleF(Config_Painting_Point.affine_painting_points[Config_Painting_Point.point_num], 1), new Bgr(Color.Red), 2);
                draw_image_bgr.Draw(new Cross2DF(ConfigPaintingPoints.affine_painting_points[ConfigPaintingPoints.point_num], 2, 2), new Bgr(Color.Red), 1);

                // Compensate x, y coordinate
                ConfigPaintingPoints.affine_painting_points[ConfigPaintingPoints.point_num].X = (float)xCompensate(ConfigPaintingPoints.affine_painting_points[ConfigPaintingPoints.point_num].X);
                ConfigPaintingPoints.affine_painting_points[ConfigPaintingPoints.point_num].Y = (float)yCompensate(ConfigPaintingPoints.affine_painting_points[ConfigPaintingPoints.point_num].Y);

                // Matching camera coordinate with machine coordinate
                ConfigPaintingPoints.affine_painting_points[ConfigPaintingPoints.point_num] = RotateFOV(ConfigPaintingPoints.affine_painting_points[ConfigPaintingPoints.point_num], 0.15);


                ConfigPaintingPoints.point_num++;
            }
        }

        public static PointF RotateFOV(PointF point, double angle)
        {
            PointF result = new PointF();

            angle = Math.Round((360 - angle) * Math.PI / 180, 3);

            result.X = (point.X - screen_width / 2) * (float)Math.Cos(angle) - (point.Y - screen_height / 2) * (float)Math.Sin(angle) + screen_width / 2;
            result.Y = (point.X - screen_width / 2) * (float)Math.Sin(angle) + (point.Y - screen_height / 2) * (float)Math.Cos(angle) + screen_height / 2;

            return result;
        }

        // Image filtering
        public void Frame_Analyze()
        {
            try
            {

                img_raw = img_capture_undist.Mat;
                Mat img_blur = new Mat();
                Mat img_hist = new Mat();

                if (Con_Image_Processing.G_blur.CheckState == CheckState.Checked)
                {
                    // Blurs an image using a Gaussian filter
                    Size ksize = new Size(9, 9);
                    double sigmaY = 0;
                    BorderType bordertype = BorderType.Reflect;
                    CvInvoke.GaussianBlur(img_raw, img_blur, ksize, (double)Con_Image_Processing.gaussian_sig.Value, sigmaY, bordertype);

                    // Convert the image to grayscale
                    CvInvoke.CvtColor(img_blur, img_gray, ColorConversion.Bgr2Gray);

                    // Applies an adaptive threshold to the image
                    CvInvoke.AdaptiveThreshold(img_gray,
                        img_threshold,
                        255,
                        AdaptiveThresholdType.GaussianC,
                        ThresholdType.Binary,
                        95,
                        1.5);
                }
                else
                {
                    // Pyramid down and up filter
                    CvInvoke.PyrDown(img_raw, img_blur, BorderType.Default);
                    CvInvoke.PyrUp(img_blur, img_blur, BorderType.Default);

                    // Convert the image to grayscale
                    CvInvoke.CvtColor(img_blur, img_gray, ColorConversion.Bgr2Gray);

                    CvInvoke.EqualizeHist(img_gray, img_hist);

                    // Applies an adaptive threshold to the image
                    CvInvoke.AdaptiveThreshold(img_hist,
                        img_threshold,
                        255,
                        AdaptiveThresholdType.GaussianC,
                        ThresholdType.Binary,
                        95,
                        1);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void Detect_Pattern()
        {
            try
            {
                // Tick time
                Stopwatch watch = Stopwatch.StartNew();

                // Clear list box
                data_mor.Items.Clear();

                // Initiate processing time
                long rotation_time = watch.ElapsedMilliseconds;

                // Load database
                ConfigPaintingPoints.dispensing_data = XDocument.Load("_database.xml");
                SysData = XDocument.Load("_system.xml");
                XElement ImageProcessingWindow = SysData.Element("System").Element("ImageProcessingWindow");
                XElement Parameters = SysData.Element("System").Element("PaintingConditionWindow").Element("Parameters");


                // Call current item
                ConfigPaintingPoints.get_item = ConfigPaintingPoints.dispensing_data.Element("Field")
                    .Elements("Item")
                    .Where(x => x.Element("Name").Value == tmp_item_name.Text)
                    .Single();

                // List positions to list
                List<XElement> point_list = ConfigPaintingPoints.get_item.Element("Points").Elements("Point").ToList();

                // Get processing time
                data_mor.Items.Add("Load XML:   " + (watch.ElapsedMilliseconds - rotation_time));
                rotation_time = watch.ElapsedMilliseconds;

                // Update progress bar
                Progress.Value = 5;

                // Filter and convert to binary image
                Frame_Analyze();

                // Get processing time
                data_mor.Items.Add("Source processing:   " + (watch.ElapsedMilliseconds - rotation_time));
                rotation_time = watch.ElapsedMilliseconds;

                // Update progress bar
                Progress.Value = 10;

                // 
                Image<Bgr, byte> img_items = img_raw.ToImage<Bgr, byte>();
                Image<Bgr, byte> img_items_bin = img_threshold.ToImage<Bgr, byte>();
                Image<Gray, byte> img_rec = img_threshold.ToImage<Gray, byte>().Copy();

                // Rotation center point
                PointF rot_cen = new PointF(tmp_raw.Rows / 2, tmp_raw.Cols / 2);

                Mat tmp_rot = new Mat();
                Image<Gray, byte> tmp_rot_dir = tmp_rot.ToImage<Gray, byte>();

                List<PointF> centerList = new List<PointF>();
                List<double> angleList = new List<double>();

                // ROI variable
                const int roi_dim = 110;
                Size roi_size = new Size(roi_dim, roi_dim);
                Point roi_location = new Point();
                //Rectangle roi_rec = new Rectangle();
                Mat roi_rot = new Mat();

                Mat mat_edge = new Mat();
                img_gray.CopyTo(mat_edge);
                CvInvoke.Canny(img_gray, mat_edge, (double)Con_Image_Processing.cannyThresh.Value - 25, (double)Con_Image_Processing.cannyThresh.Value + 25, 5, true);
                CvInvoke.BitwiseNot(mat_edge, mat_edge);

                Image<Gray, byte> img_edge = mat_edge.ToImage<Gray, byte>().Copy();
                Image<Bgr, byte> img_edge_clone = new Image<Bgr, byte>(new Size());
                CvInvoke.CvtColor(img_edge, img_edge_clone, ColorConversion.Gray2Bgr);

                // Get processing time
                data_mor.Items.Add("Edge detection:   " + (watch.ElapsedMilliseconds - rotation_time));
                rotation_time = watch.ElapsedMilliseconds;
                // Update progress bar
                Progress.Value = 15;


                // Circle Hough Transform
                CircleF[] img_circles = CvInvoke.HoughCircles(
                    img_gray, // Array of circles data
                    HoughType.Gradient,
                    2,
                    100,
                    int.Parse(ImageProcessingWindow.Element("HougeParam1").Value),
                    int.Parse(ImageProcessingWindow.Element("HougeParam2").Value),
                    int.Parse(ImageProcessingWindow.Element("MinRa").Value),
                    int.Parse(ImageProcessingWindow.Element("MaxRa").Value));

                // Display number of items
                num_of_items.Text = img_circles.Length.ToString();

                // Get processing time
                data_mor.Items.Add("Center detection:   " + (watch.ElapsedMilliseconds - rotation_time));
                rotation_time = watch.ElapsedMilliseconds;

                // Update progress bar
                Progress.Value = 20;

                using (gcode)
                {
                    for (int circle_num = 0; circle_num < img_circles.Length; circle_num++)
                    {
                        // 
                        CircleF circle = img_circles[circle_num];
                        if (Inside.Checked)
                        {
                            Image<Gray, byte> tmp_rot_img = new Image<Gray, byte>(tmp_raw.Size);

                            // Update ROI
                            roi_location = new Point((int)Math.Round(circle.Center.X) - roi_dim / 2,
                                (int)Math.Round(circle.Center.Y) - roi_dim / 2);
                            Image<Gray, byte> roi_getting_img = GetSourceROI(img_rec, roi_location, roi_dim);

                            // Data allocation
                            byte[] roiData = new byte[roi_getting_img.Width * roi_getting_img.Height];
                            GCHandle handle = GCHandle.Alloc(roiData, GCHandleType.Pinned);
                            CvInvoke.BitwiseNot(roi_getting_img, roi_getting_img);

                            using (Mat tempMat = new Mat(roi_getting_img.Size, DepthType.Cv8U, 1, handle.AddrOfPinnedObject(), roi_getting_img.Width))
                            {
                                CvInvoke.BitwiseNot(roi_getting_img, tempMat);
                                //CvInvoke.Imwrite("data/roi" + circle_num.ToString() + ".jpg", tempMat); // Sure!!!
                            }

                            handle.Free();

                            rotation_time = watch.ElapsedMilliseconds;

                            #region Estimate pattern direction [Input: tmp_raw - Output: get_angle] 

                            // 2D Rotation matrix
                            Matrix<double> tmp_dst = new Matrix<double>(3, 3);

                            //
                            int get_min = 0;
                            CircleF get_circle = new CircleF();
                            double get_angle = 0;


                            // Scan pixels
                            for (int cnt = 0; cnt < 360; cnt++)
                            {
                                // Specific value of each position
                                int pixel_sum = 0;

                                // Template rotation                                                        
                                CvInvoke.GetRotationMatrix2D(rot_cen, cnt, 1, tmp_dst);
                                CvInvoke.WarpAffine(tmp_raw, tmp_rot, tmp_dst, tmp_raw.Size); // ~100 ms => it takes almost scanning time, we should improve

                                // Address problem !!!
                                tmp_rot_img = tmp_rot.ToImage<Gray, byte>();
                                tmp_rot = tmp_rot_img.Mat;

                                //// Data allocation
                                byte[] tmpData = new byte[tmp_rot.Width * tmp_rot.Height];
                                GCHandle _handle = GCHandle.Alloc(tmpData, GCHandleType.Pinned);
                                CvInvoke.BitwiseNot(tmp_rot, tmp_rot);
                                using (Mat tempMat = new Mat(tmp_rot.Size, DepthType.Cv8U, 1, _handle.AddrOfPinnedObject(), tmp_rot.Width))
                                {
                                    CvInvoke.BitwiseNot(tmp_rot, tempMat);
                                    //CvInvoke.Imwrite("data/tmp" + cnt.ToString() + ".jpg", tempMat); // Sure!!!
                                }
                                _handle.Free();

                                // Get maching value
                                for (int i = 0; i < 12100; i++) // Taking 15 - 20 ms
                                {
                                    if (tmpData[i] == 0)
                                        pixel_sum += roiData[i];
                                }

                                // Update min value
                                if (cnt == 0)
                                {
                                    get_min = pixel_sum;
                                    get_angle = cnt;
                                    tmp_rot_dir = tmp_rot_img;
                                }
                                else
                                {
                                    if (pixel_sum < get_min)
                                    {
                                        get_min = pixel_sum;
                                        get_angle = cnt;
                                        tmp_rot_dir = tmp_rot_img;
                                    }
                                }
                            }
                            //data_mor.Items.Add("Template " + circle_num.ToString() + " :" + (watch.ElapsedMilliseconds - rotation_time));
                            //rotation_time = watch.ElapsedMilliseconds;
                            #endregion

                            // ====> CONSIDERATION
                            #region Center Correction
                            PointF correctCenter = new PointF();
                            correctCenter = circle.Center;
                            int roi_num = 0;
                            int min_error = 0;
                            int preError = 0;

                            // Correction loop
                            int correctionRage = int.Parse(ImageProcessingWindow.Element("CorrectionRange").Value);
                            for (int p = 0; p < correctionRage; p++)
                            {
                                for (int q = 0; q < correctionRage; q++)
                                {
                                    roi_location = new Point(
                                        (int)Math.Round(get_circle.Center.X) - roi_dim / 2 - (correctionRage / 2 - p),
                                        (int)Math.Round(get_circle.Center.Y) - roi_dim / 2 - (correctionRage / 2 - q));
                                    roi_getting_img = GetSourceROI(img_edge, roi_location, roi_dim);

                                    //CvInvoke.Imwrite("data/roi" + roi_num.ToString() + ".jpg", roi_getting_img);

                                    int error_sum = 0;
                                    for (int alpha = 0; alpha < 360; alpha = alpha + 1)
                                    {
                                        int feedback = 0;
                                        int reference = 0;
                                        int error = 0;
                                        for (int r = 0; r < 50; r++)
                                        {
                                            double alpha_rad = Math.PI * alpha / 180;
                                            double x = r * Math.Cos(alpha_rad) + img_edge.Rows / 2;
                                            double y = r * Math.Sin(alpha_rad) + img_edge.Cols / 2;

                                            if (roi_getting_img.Data[(int)x, (int)y, 0] == 0)
                                                feedback = r;

                                            if (tmp_rot_dir.Data[(int)x, (int)y, 0] == 0)
                                                reference = r;
                                        }

                                        // Calculate error
                                        error = Math.Abs(reference - feedback);
                                        if (error > int.Parse(ImageProcessingWindow.Element("ErrConstraint").Value))
                                            error = int.Parse(ImageProcessingWindow.Element("ErrConstraint").Value);
                                        error_sum += error;
                                    }

                                    if ((p == correctionRage / 2) && (q == correctionRage / 2))
                                        preError = error_sum;

                                    // Update min value
                                    if (roi_num == 0)
                                    {
                                        min_error = error_sum;
                                        correctCenter.X = get_circle.Center.X - ((float)correctionRage / 2 - p);
                                        correctCenter.Y = get_circle.Center.Y - ((float)correctionRage / 2 - q);
                                    }
                                    else
                                    {
                                        if (error_sum < min_error)
                                        {
                                            min_error = error_sum;
                                            correctCenter.X = get_circle.Center.X - ((float)correctionRage / 2 - p);
                                            correctCenter.Y = get_circle.Center.Y - ((float)correctionRage / 2 - q);
                                        }
                                    }
                                    roi_num++;
                                }
                            }
                            #endregion

                            if (correctionRage != 0)
                                data_mor.Items.Add("Error" + (circle_num + 1).ToString() + ": " + min_error.ToString() + " Pre: " + preError.ToString());

                            get_circle = new CircleF(correctCenter, circle.Radius);

                            // Show detected items
                            Draw_Matching(img_items, get_circle, get_angle);

                            // Add data to list
                            angleList.Add(get_angle);
                            centerList.Add(correctCenter);
                        }
                        else
                        {
                            angleList.Add(0);
                            centerList.Add(circle.Center);
                            img_items.Draw(new CircleF(circle.Center, 52), new Bgr(Color.Red), 3);
                        }
                        // Update progress bar
                        Progress.Value = 20 + ((circle_num + 1) * 100 / img_circles.Length) * 80 / 100;
                        Progress.Refresh();
                    }
                    // Points sorting
                    centerList = SortByDistance(centerList, angleList);
                    //centerList = HelixSort(centerList);

                    int t = 0;
                    foreach (PointF centerPoint in centerList)
                    {
                        // Calculate center coordinate
                        double cen_X = xCompensate(centerPoint.X) + x_pixel_offset;
                        double cen_Y = yCompensate(centerPoint.Y) + y_pixel_offset;
                        PointF cenPoint = new PointF((float)cen_X, (float)cen_Y);

                        // Global painting points rotation
                        cenPoint = RotateFOV(cenPoint, 0.15);

                        cen_X = Math.Round(cenPoint.X * ConfigPaintingPoints.real_accuracy, 3);
                        cen_Y = Math.Round(cenPoint.Y * ConfigPaintingPoints.real_accuracy, 3);

                        // Invert y axis direction
                        cen_Y = -cen_Y;

                        if (Inside.Checked)
                        {
                            //Show global painting points
                            Real_PointsWarpAffine(point_list, centerPoint, angleListSort[t], img_items);

                            //Gcode building
                            Running_Procedure(ConfigPaintingPoints.affine_painting_points, (float)cen_X, (float)cen_Y, Parameters);
                        }
                        else
                        {
                            Running_Circle((float)cen_X, (float)cen_Y, Parameters);
                        }

                        if (t > 0)
                            img_items.Draw(new LineSegment2DF(centerList[t - 1], centerList[t]), new Bgr(Color.Red), 2);
                        else
                            img_items.Draw(new CircleF(centerList[t], 10), new Bgr(Color.Red), 2);
                        t++;
                    }

                    #region Complete Painting Process
                    // Waiting for moving completed
                    Wait(100);

                    // Turn off piston
                    gcode.WriteLine("DeactivateSignal(OUTPUT6)");
                    gcode.WriteLine("DeactivateSignal(OUTPUT" + GetValveNum(item_color.Text) + ")");

                    //Return home
                    gcode.WriteLine("Code \"Z" + Parameters.Attribute("zSafe").Value + "\"");
                    gcode.WriteLine("Code \"G00 X0" + " Y0" + "\"");
                    gcode.Close();
                    #endregion

                    // View result
                    pattern_field.Image = img_items.Bitmap;

                    // Save detected items result
                    //CvInvoke.Imwrite("result/1.img_capture_undist.jpg", img_capture_undist);
                    //CvInvoke.Imwrite("result/2.img_gray.jpg", img_gray);
                    CvInvoke.Imwrite("result/3.img_edge_clone.jpg", img_edge_clone);
                    CvInvoke.Imwrite("result/4.img_items.jpg", img_items);
                    //CvInvoke.Imwrite("result/5.img_edge.jpg", mat_edge);
                }
                // Tock timer
                elapsed_time.Text = watch.ElapsedMilliseconds.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private List<PointF> sortedList(List<PointF> pointList)
        {
            float maxx = pointList.Max(point => point.X);
            float minx = pointList.Min(point => point.X);
            float maxy = pointList.Max(point => point.Y);
            float miny = pointList.Min(point => point.Y);

            const int range = 30;
            int initcol = (int)minx / range;
            int initrow = (int)miny / range;
            int poscol = 0;
            int posrow = 0;
            int col = 2 + (int)(maxx - minx) / range;
            int row = 2 + (int)(maxy - miny) / range;
            PointF[,] array = new PointF[col, row];

            foreach (PointF pointElement in pointList)
            {
                poscol = ((int)pointElement.X / range) - initcol;
                posrow = ((int)pointElement.Y / range) - initrow;
                array[poscol, posrow] = pointElement;
            }

            List<PointF> pointList2 = new List<PointF>();
            bool r = false;
            for (int x = 0; x < col; x++)
            {
                for (int y = 0; y < row; y++)
                {
                    if (array[x, r ? row - 1 - y : y].X != 0 && array[x, r ? row - 1 - y : y].Y != 0)
                        pointList2.Add(array[x, r ? row - 1 - y : y]);
                }
                r = !r;
            }

            return pointList2;
        }

        private Image<Gray, byte> GetSourceROI(Image<Gray, byte> src, Point ROILocation, int ROISize)
        {
            // Set ROI to image
            Rectangle roi_rec = new Rectangle(ROILocation, new Size(ROISize, ROISize));
            CvInvoke.cvSetImageROI(src.Ptr, roi_rec);
            src.ROI = CvInvoke.cvGetImageROI(src.Ptr);

            // Create ROi background
            Image<Bgr, byte> roi_background = new Image<Bgr, byte>(src.Size);
            Mat roi_getting = new Mat();
            roi_background.SetValue(255);
            roi_background.Draw(
                new CircleF(new Point(roi_background.Rows / 2, roi_background.Cols / 2), ROISize / 2),
                new Bgr(Color.Black),
                -1); // Line width, input negative value to fill the circle

            // ROI Bitwise
            CvInvoke.CvtColor(roi_background, roi_getting, ColorConversion.Bgr2Gray);
            CvInvoke.BitwiseAnd(src, roi_getting, roi_getting);
            CvInvoke.BitwiseXor(src, roi_getting, roi_getting);

            // Convert to image for pixels access
            Image<Gray, byte> roi_getting_img = roi_getting.ToImage<Gray, byte>();

            //CvInvoke.cvResetImageROI(src.Ptr);
            return roi_getting_img;
        }

        public void TemplateGraph(Mat template)
        {
            ConfigPaintingPoints.dispensing_data = XDocument.Load(ConfigPaintingPoints.data_path);
            ConfigPaintingPoints.get_item = ConfigPaintingPoints.dispensing_data.Element("Field")
                .Elements("Item")
                .Where(x => x.Element("Name").Value == tmp_item_name.Text)
                .Single();
            ConfigPaintingPoints.get_item.Element("PixelsPosition").RemoveAll();

            //int k = 0;
            //int index = 0;

            //for (int i = 0; i < template.Rows; i++)
            //{
            //    for (int j = 0; j < template.Cols; j++)
            //    {
            //        index = (i - 50) * (i - 50) + (j - 50) * (j - 50);
            //        // Check inside pattern and check inside observed circle
            //        if (index < 2500)
            //        {
            //            if (template.ToImage<Gray, byte>().Data[i, j, 0] == 0)
            //            {

            //                Config_Painting_Point.get_item.Element("PixelsPosition").Add(new XElement("Pos",
            //                    new XAttribute("No.", k),
            //                    new XAttribute("Row", i),
            //                    new XAttribute("Col", j)));
            //            }
            //        }
            //        k++;
            //    }
            //}

            ConfigPaintingPoints.dispensing_data.Save(ConfigPaintingPoints.data_path);
        }

        private List<PointF> HelixSort(List<PointF> pointList)
        {
            List<PointF> ouput = new List<PointF>();
            List<double> normList = new List<double>();
            PointF getPoint = new PointF();
            double X, Y, angle;
            double norm;
            double maxnorm = 0;

            
            for (int i = 0; i < 360; i++)
            {
                int k = 0;
                foreach (PointF point in pointList)
                {
                    X = point.X - screen_width / 2;
                    Y = -(point.Y - screen_height / 2);

                    norm = Math.Sqrt(X * X + Y * Y);
                    angle = Math.Acos(X / norm) * 180 / Math.PI;

                    if (angle >= i && angle <= i + 1)
                    {
                        if (k == 0)
                        {
                            maxnorm = norm;
                            getPoint = point;
                        }
                        else
                        {
                            maxnorm = (norm > maxnorm) ? norm : maxnorm;
                            getPoint = point;
                        }
                    }
                    k++;
                }

                if (maxnorm != 0)
                {
                    ouput.Add(getPoint);
                    pointList.Remove(getPoint);
                    maxnorm = 0;
                }
            }

            return ouput;
        }
        


        public List<PointF> SortByDistance(List<PointF> pointList, List<double> angleList)
        {
            List<PointF> output = new List<PointF>();
            angleListSort = new List<double>();
            int nearestPoint = 0;

            // Find the nearest point from start
            nearestPoint = NearestPoint(new Point(0, 0), pointList);
            output.Add(pointList[nearestPoint]);
            angleListSort.Add(angleList[nearestPoint]);

            // Remove start point from input point list
            pointList.Remove(output[0]);
            //angleList.Remove(angleListSort[0]);
            angleList.RemoveAt(nearestPoint);

            int x = 0;
            for (int i = 0; i < pointList.Count + x; i++)
            {
                // Find the nearest point from current point
                nearestPoint = NearestPoint(output[output.Count - 1], pointList);
                output.Add(pointList[nearestPoint]);
                angleListSort.Add(angleList[nearestPoint]);

                // Remove the current point from list
                pointList.Remove(output[output.Count - 1]);
                //angleList.Remove(angleListSort[output.Count - 1]);
                angleList.RemoveAt(nearestPoint);
                x++;
            }

            return output;
        }

        // Find the nearest point from a specific point
        public int NearestPoint(PointF srcPoint, List<PointF> dstPoints)
        {
            KeyValuePair<double, int> smallestDistance = new KeyValuePair<double, int>();
            for (int i = 0; i < dstPoints.Count; i++)
            {
                // Calculate distances form source point to destination points
                double distance = Math.Sqrt(Math.Pow(srcPoint.X - dstPoints[i].X, 2) + Math.Pow(srcPoint.Y - dstPoints[i].Y, 2));

                // Get smallest distance
                if (i == 0)
                {
                    smallestDistance = new KeyValuePair<double, int>(distance, i);
                }
                else
                {
                    if (distance < smallestDistance.Key)
                    {
                        smallestDistance = new KeyValuePair<double, int>(distance, i);
                    }
                }
            }
            return smallestDistance.Value;
        }

        // Compensate x axis value
        public static double xCompensate(double x)
        {
            SysData = XDocument.Load("_system.xml");
            XElement dataX;

            // Get left node
            dataX = SysData.Element("System")
                    .Elements("CameraCalibrationWindow")
                    .Elements("Compensation")
                    .Elements("Section")
                    .Where(j => j.Element("Name").Value == "XAxis")
                    .Single();
            dataX = dataX.Element("Data");
            List<XElement> XList = dataX.Elements("Line").ToList();

            double value = 0;

            // Compensation loop
            foreach (XElement element in XList)
            {
                if (x >= double.Parse(element.Attribute("start").Value) && x < double.Parse(element.Attribute("stop").Value))
                {
                    value = x - (x * double.Parse(element.Attribute("slope").Value) +
                                     double.Parse(element.Attribute("intercept").Value));
                }
            }
            return value;
        }

        // Compensate y axis value
        public static double yCompensate(double y)
        {
            SysData = XDocument.Load("_system.xml");
            XElement dataY;

            // Get YAxis node
            dataY = SysData.Element("System")
                    .Elements("CameraCalibrationWindow")
                    .Elements("Compensation")
                    .Elements("Section")
                    .Where(j => j.Element("Name").Value == "YAxis")
                    .Single();
            dataY = dataY.Element("Data");
            List<XElement> YList = dataY.Elements("Line").ToList();

            double value = 0;

            // Y compensation
            foreach (XElement element in YList)
            {
                if (y >= double.Parse(element.Attribute("start").Value) && y < double.Parse(element.Attribute("stop").Value))
                {
                    value = y - (y * double.Parse(element.Attribute("slope").Value) +
                                    double.Parse(element.Attribute("intercept").Value));
                }
            }

            return value;
        }

        public static void PixelsCompensation(Image<Bgr, byte> sample)
        {
            SysData = XDocument.Load("_system.xml");
            XElement section;

            VectorOfPointF corner_set = new VectorOfPointF();
            Mat sample_frame = new Mat();
            Size sample_size = new Size(50, 38);

            // Convert BGR to GRAY image
            CvInvoke.CvtColor(sample, sample_frame, ColorConversion.Bgr2Gray);

            // Find corners on chess board image
            if (CvInvoke.FindChessboardCorners(sample_frame, sample_size, corner_set))
                CvInvoke.CornerSubPix(sample_frame, corner_set, new Size(11, 11), new Size(-1, -1), new MCvTermCriteria(30, 0.05));

            //
            int ref_num = 1;
            byte point_index = 0;
            PointF[] com_point = new PointF[2]; // compensation point
            double slope, intercept;
            byte getRow = 19;
            byte getCol = 25;

            // Find XAxis node
            section = SysData.Element("System")
            .Elements("CameraCalibrationWindow")
            .Elements("Compensation")
            .Elements("Section")
            .Where(x => x.Element("Name").Value == "XAxis")
            .Single();
            section.Element("Data").RemoveAll();

            for (int i = 50 * getRow; i < 50 * getRow + 50; i++)
            {
                // Draw detected corners from left to right
                sample.Draw(new CircleF(corner_set[i], 8), new Bgr(Color.Red), 2);
                sample.Draw(new LineSegment2DF(new PointF(corner_set[50 * getRow + getCol].X + (float)47.8 * (getCol - ref_num - 1), 0), new PointF(corner_set[50 * getRow + getCol].X + (float)47.8 * (getCol - ref_num - 1), 1944)), ((getCol - ref_num - 1) == 0) ? new Bgr(Color.Red) : new Bgr(Color.Blue), 1);

                // For testing
                //if (i == 50 * getRow)
                //{
                //    // Calculate center coordinate
                //    double cen_X = xCompensate(corner_set[50 * getRow + getCol].X) + x_pixel_offset;
                //    double cen_Y = yCompensate(corner_set[50 * getRow + getCol].Y) + y_pixel_offset;

                //    PointF cenPoint = new PointF((float)cen_X, (float)cen_Y);

                //    // Global painting points rotation
                //    cenPoint = RotateFOV(cenPoint, 0.15);

                //    cen_X = Math.Round(cenPoint.X * ConfigPaintingPoints.real_accuracy, 3);
                //    cen_Y = Math.Round(cenPoint.Y * ConfigPaintingPoints.real_accuracy, 3);

                //    // Invert y axis direction
                //    cen_Y = -cen_Y;

                //    gcode.WriteLine("Code \"G00 X" + cen_X + " Y" + cen_Y + "\"");
                //}
                
                // Calculate stretch error
                com_point[point_index].X = corner_set[50 * getRow + getCol].X + (float)47.8 * (ref_num - 1 - getCol);
                float err = corner_set[i].X - com_point[point_index].X;
                com_point[point_index].Y = err;

                if (point_index > 0)
                {
                    // a = (y0 - y1)/(x0 - x1)
                    slope = (com_point[point_index - 1].Y - com_point[point_index].Y) / (com_point[point_index - 1].X - com_point[point_index].X);

                    // b = (x0y1 - x1y0)/(x0 - x1)
                    intercept = (com_point[point_index - 1].X * com_point[point_index].Y - com_point[point_index].X * com_point[point_index - 1].Y) /
                        (com_point[point_index - 1].X - com_point[point_index].X);

                    slope = Math.Round(slope, 3);
                    intercept = Math.Round(intercept, 3);

                    section.Element("Data").Add(new XElement("Line",
                    new XAttribute("start", (i == 50 * getRow + 1) ? 0 : com_point[point_index - 1].X),
                    new XAttribute("slope", slope),
                    new XAttribute("intercept", intercept),
                    new XAttribute("stop", (i == 50 * getRow + 49) ? 2592 : com_point[point_index].X)));

                    com_point[point_index - 1] = com_point[point_index];
                    point_index = 0;
                }

                ref_num++;
                point_index++;
            }

            // Find YAxis node
            section = SysData.Element("System")
            .Elements("CameraCalibrationWindow")
            .Elements("Compensation")
            .Elements("Section")
            .Where(x => x.Element("Name").Value == "YAxis")
            .Single();
            section.Element("Data").RemoveAll();

            ref_num = 1;
            for (int i = getCol; i < getCol + 50 * 38; i = i + 50)
            {
                // Draw detected corners from top to bottom
                // form getCol to getCol + 50 * 37
                sample.Draw(new CircleF(corner_set[i], 8), new Bgr(Color.Red), 2);
                sample.Draw(new LineSegment2DF(new PointF(0, corner_set[getCol + getRow * 50].Y + (float)47.8 * (getRow - ref_num)), new PointF(2592, corner_set[getCol + getRow * 50].Y + (float)47.8 * (getRow - ref_num))), ((getRow - ref_num) == 0) ? new Bgr(Color.Red) : new Bgr(Color.Blue), 1);

                // Calculate stretch error
                com_point[point_index].X = corner_set[getCol + getRow * 50].Y + (float)47.8 * (ref_num - getRow - 1);
                float err = corner_set[i].Y - com_point[point_index].X;
                com_point[point_index].Y = err;

                if (point_index > 0)
                {
                    // a = (y0 - y1)/(x0 - x1)
                    slope = (com_point[point_index - 1].Y - com_point[point_index].Y) / (com_point[point_index - 1].X - com_point[point_index].X);

                    // b = (x0y1 - x1y0)/(x0 - x1)
                    intercept = (com_point[point_index - 1].X * com_point[point_index].Y - com_point[point_index].X * com_point[point_index - 1].Y) /
                        (com_point[point_index - 1].X - com_point[point_index].X);

                    slope = Math.Round(slope, 3);
                    intercept = Math.Round(intercept, 3);

                    section.Element("Data").Add(new XElement("Line",
                    new XAttribute("start", (i == getCol) ? 0 : com_point[point_index - 1].X),
                    new XAttribute("slope", slope),
                    new XAttribute("intercept", intercept),
                    new XAttribute("stop", (i == getCol + 50 * 38 - 50) ? 1944 : com_point[point_index].X)));

                    com_point[point_index - 1] = com_point[point_index];
                    point_index = 0;
                }

                ref_num++;
                point_index++;
            }

            //SysData.Save("_system.xml");
            gcode.Close();
        }

        private void Running_Procedure(PointF[] Points, float cenX, float cenY, XElement Parameters)
        {
            double X, Y; //mm

            // Get painting points from XML
            ConfigPaintingPoints.get_item = ConfigPaintingPoints.dispensing_data.Element("Field")
                            .Elements("Item")
                            .Where(x => x.Element("Name").Value == tmp_item_name.Text)
                            .Single();

            List<XElement> point_list = ConfigPaintingPoints.get_item.Element("Points").Elements("Point").ToList();

            int j = 0;

            foreach (XElement element in point_list)
            {
                // Convert pixel to mm
                X = Math.Round((Points[j].X - 42.0) * ConfigPaintingPoints.real_accuracy, 3);
                Y = -Math.Round((Points[j].Y + 7.0) * ConfigPaintingPoints.real_accuracy, 3);

                //--------------------- Build Gcode-------------------

                // with the first point of each item
                if (j == 0)
                {
                    // with the first point of the first item
                    if (first_item)
                    {
                        //
                        gcode.WriteLine("Code \"G90 " + GetValveHome(item_color.Text) + " G00" + "\"");

                        //
                        gcode.WriteLine("ActivateSignal(OUTPUT" + GetValveNum(item_color.Text) + ")");

                        // Move to the first point of inside circle
                        gcode.WriteLine("Code \"G00 X" + X + " Y" + Y + "\"");
                        Wait(1000);

                        // Turn on 15mm piston
                        gcode.WriteLine("ActivateSignal(OUTPUT6" + ")");

                        // Rapid moving to Z return
                        gcode.WriteLine("Code \"Z" + Parameters.Attribute("zReturn").Value + "\"");
                    }
                }
                else
                {
                    // Go to painting point of template
                    gcode.WriteLine("Code \"G01 X" + X + " Y" + Y + " F" + Parameters.Attribute("xySpeed").Value + "\"");

                }

                //Drip paint
                Paint_Drip(0);

                // Rapid moving to Z return
                gcode.WriteLine("Code \"Z" + Parameters.Attribute("zReturn").Value + "\"");

                // Disable the first item flag
                first_item = false;

                j++;
            }
        }

        private void Running_Circle(float cenX, float cenY, XElement Parameters)
        {
            //
            float startpoint_X = cenX - in_circle_radius;
            float startpoint_Y = cenY - (float)0.3;

            // with the first point of the first item
            if (first_item)
            {
                // Absolute programming Valve home Rapid positioning
                gcode.WriteLine("Code \"G90 " + GetValveHome(item_color.Text) + " G00" + "\"");

                // Select paiting channel
                gcode.WriteLine("ActivateSignal(OUTPUT" + GetValveNum(item_color.Text) + ")");
                
                // Move to the first point of inside circle
                gcode.WriteLine("Code \"G00 X" + startpoint_X + " Y" + startpoint_Y + "\"");
                Wait(1000);

                // Turn on 15mm piston
                gcode.WriteLine("ActivateSignal(OUTPUT6" + ")");

                //
                Paint_Drip(circle_deep_offset);

                // Circular interpolation
                CircleDivider(new PointF(cenX, cenY + (float)0.3), in_circle_radius, 4);
                Wait(50);

                // Go to z return
                gcode.WriteLine("Code \"G00 Z" + Parameters.Attribute("zReturn").Value + "\"");

                // Disable the first item flag
                first_item = false;
            }
            else
            {
                // Move to the first point of inside circle
                gcode.WriteLine("Code \"X" + startpoint_X + " Y" + startpoint_Y + "\"");
                Wait(100);

                //
                Paint_Drip(circle_deep_offset);

                // Circular interpolation
                CircleDivider(new PointF(cenX, cenY + (float)0.3), in_circle_radius, 4);
                Wait(50);

                // Go to z return
                gcode.WriteLine("Code \"G00 Z" + Parameters.Attribute("zReturn").Value + "\"");
            }
        }

        private void CircleDivider(PointF center, float radius, byte factor)
        {
            // Start point of the circle
            PointF startPoint = new PointF(center.X - radius, center.Y);

            // G02
            for (byte i = 0; i < factor; i++)
            {
                PointF endPoint = new PointF(); // End point of an arc
                endPoint.X = (float)(center.X - radius + radius * (1 - Math.Cos((i + 1) * (360 / factor) * Math.PI / 180)));
                endPoint.Y = (float)(center.Y - radius * Math.Sin((i + 1) * (360 / factor) * Math.PI / 180));

                // Command Gcode                
                gcode.WriteLine("Code \"G03 X" + endPoint.X + " Y" + endPoint.Y + " I" + (center.X - startPoint.X) + " J" + (center.Y - startPoint.Y) + ((i == 0) ? " F6000" : "") + "\"");

                // Update start point of the arc
                startPoint.X = endPoint.X;
                startPoint.Y = endPoint.Y;
            }
        }

        private void Paint_Drip(decimal offset)
        {
            XElement Parameters = SysData.Element("System").Element("PaintingConditionWindow").Element("Parameters");

            // Moving to Z injecting deep with specific speed
            if (!first_item)
                gcode.WriteLine("Code \"G00 Z" + (decimal.Parse(Parameters.Attribute("zDrip").Value) + offset) + "\"");
            else
                gcode.WriteLine("Code \"Z" + (decimal.Parse(Parameters.Attribute("zDrip").Value) + offset) + "\"");


            // Waiting for moving completed
            Wait(first_item ? 100 : 50);

            if (EnableEfd.CheckState == CheckState.Checked)
            {
                // Drip paint
                gcode.WriteLine("ActivateSignal(OUTPUT7" + ")");
                gcode.WriteLine("Sleep(100)");
                gcode.WriteLine("DeactivateSignal(OUTPUT7" + ")");
            }
        }

        private void Wait(int time)
        {
            // Wait for moving completed
            gcode.WriteLine("While (IsMoving())");
            //gcode.WriteLine("Sleep(" + (first_item ? "500" : "50") + ")");
            gcode.WriteLine("Sleep(" + time.ToString() + ")");
            gcode.WriteLine("Wend");
        }
        
        private byte GetValveNum(string colorName)
        {
            byte valve_num = 0;

            switch (colorName)
            {
                case "Red":
                    valve_num = 4;
                    break;
                case "Black":
                    valve_num = 5;
                    break;
            }
            return valve_num;
        }

        private string GetValveHome(string colorName)
        {
            string valve_num = "";

            switch (colorName)
            {
                case "Red":
                    valve_num = "G55";
                    break;
                case "Black":
                    valve_num = "G54";
                    break;
            }
            return valve_num;
        }

        private void Draw_Matching(Image<Bgr, byte> Input, CircleF circle, double angle)
        {
            int line_width = 3;

            if (((int)(circle.Center.X + circle.Radius) < Input.Size.Width + 1 || (int)(circle.Center.X - circle.Radius) > 1) &&
               ((int)(circle.Center.Y + circle.Radius) < Input.Size.Height + 1 || (int)(circle.Center.Y - circle.Radius) > 1))
            {
                LineSegment2DF line = new LineSegment2DF();
                PointF point2 = new PointF(
                    circle.Center.X + (circle.Radius - line_width) * (float)Math.Cos(-angle * Math.PI / 180),
                    circle.Center.Y + (circle.Radius - line_width) * (float)Math.Sin(-angle * Math.PI / 180));
                line.P1 = circle.Center;
                line.P2 = point2;
                Input.Draw(circle, new Bgr(Color.LawnGreen), line_width);
                //Input.Draw(new CircleF(circle.Center, 57), new Bgr(Color.LawnGreen), 1);
                //Input.Draw(line, new Bgr(Color.LawnGreen), line_width);
            }
        }

        public static void Draw_Grid(Image<Bgr, byte> Input)
        {
            const double _scale = 47.8; // pixels per 10 mm
            LineSegment2DF vertical = new LineSegment2DF();
            LineSegment2DF horizontal = new LineSegment2DF();

            // Vertical lines
            for (int i = 1; i < 56; i++)
            {
                vertical.P1 = new PointF(screen_width / 2 + (float)_scale * (i - 28), 0);
                vertical.P2 = new PointF(screen_width / 2 + (float)_scale * (i - 28), screen_height);
                if (i == 28)
                {
                    Input.Draw(vertical, new Bgr(Color.Red), 2);
                }
                else
                {
                    //Input.Draw(vertical, new Bgr(Color.White), 1);
                }
            }

            // Horizontal lines
            for (int i = 0; i < 42; i++)
            {
                horizontal.P1 = new PointF(0, screen_height / 2 + (float)_scale * (i - 20));
                horizontal.P2 = new PointF(screen_width, screen_height / 2 + (float)_scale * (i - 20));
                if (i == 20)
                {
                    Input.Draw(horizontal, new Bgr(Color.Red), 2);
                }
                else
                {
                    //Input.Draw(horizontal, new Bgr(Color.White), 1);
                }

            }
        }

        private void EnableButton(bool status)
        {
            RefAllHome.Enabled = status;
            GotoHome.Enabled = status;
            TestValve.Enabled = status;
            ReplacePosition.Enabled = status;
            TurnPiston.Enabled = status;
            Detect_items.Enabled = status;
        }


        #region Click events
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void paintingPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (first_start01)
                Con_Painting_Point = new ConfigPaintingPoints();
            first_start01 = false;
            Con_Painting_Point.ShowDialog();
        }

        private void imageProcessingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (first_start02)
                Con_Image_Processing = new ConfigImageProcessing();
            first_start02 = false;
            Con_Image_Processing.ShowDialog();
        }

        private void cameraCalibrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigCameraCalibration Con_Camera_Calib = new ConfigCameraCalibration();
            Con_Camera_Calib.ShowDialog();
        }

        private void paintingConditionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigPaintingCondition Con_Painting_Condition = new ConfigPaintingCondition();
            Con_Painting_Condition.ShowDialog();
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            // Open source image
            OpenFileDialog s_file = new OpenFileDialog();
            if (s_file.ShowDialog() == DialogResult.OK)
            {
                img_capture_undist = new Image<Bgr, byte>(s_file.FileName);
                pattern_field.Image = img_capture_undist.Bitmap;
            }
        }

        private void Video_Click(object sender, EventArgs e)
        {

            try
            {
                // Restart camera
                if (_capture.IsOpened)
                {
                    // Add handler for getting camera frame
                    Application.Idle += new EventHandler(Frame_Calibration);
                    _capture.Start();

                    // Reset progress bar
                    Progress.Value = 0;

                    // Update working status
                    status_label.Text = "Camera On";
                    status_label.Refresh();
                }
                else
                {
                    // Update working status
                    status_label.Text = "Camera Off";
                    status_label.Refresh();
                }

                data_mor.Items.Clear();
            }
            catch
            {
                return;
            }

        }

        private void Capture_Click(object sender, EventArgs e)
        {
            try
            {
                if (_capture.IsOpened)
                {
                    // Stop capturing image
                    _capture.Stop();

                    Application.Idle -= new EventHandler(Frame_Calibration);

                    // Save image to disk
                    CvInvoke.Imwrite("pattern_field.jpg", img_capture_undist);

                    // Save image to image lib
                    CvInvoke.Imwrite("image_lib/capture" + DateTime.Now.ToFileTime() + ".jpg", img_capture_undist);

                    // Display
                    pattern_field.Image = img_draw.Bitmap;

                    // Update working status
                    status_label.Text = "Image Captured";
                    status_label.Refresh();
                }
                else
                {
                    status_label.Text = "No Camera Data";
                    status_label.Refresh();
                }
            }
            catch
            {
                return;
            }
        }

        private void DetectClick(object sender, EventArgs e)
        {
            first_item = true;

            pattern_field.Image = img_capture_undist.Bitmap;
            pattern_field.Refresh();

            // Update working status
            status_label.Text = "Detecting...";
            status_label.Refresh();

            gcode.Close();
            gcode = new StreamWriter(Path.Combine(macroDirectory, @"M999.m1s"));

            Detect_Pattern();

            // Update working status
            status_label.Text = "Detected";
            status_label.Refresh();

            detected = true;
        }

        private void ResetClick(object sender, EventArgs e)
        {
            GetMach3Instance();

            if (scriptObject != null)
            {
                // Reset OEM
                scriptObject.DoOEMButton(1021); 
            }
            else MessageBox.Show("Please start Mach3.");

            if (painting)
                painting = false;
        }

        private void RefAllHome_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Bạn muốn thiết lập gốc máy?", "CVEye",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialog == DialogResult.Yes)
            {
                // Reset zero flags
                xZero = false;
                yZero = false;
                zZero = false;                              
                
                GetMach3Instance();

                if (scriptObject != null)
                {
                    // Set machine zero
                    scriptObject.Code("M90");

                    // Low speed mode
                    lowSpeed = true;
                }
            }
            else return;
        }

        private void GotoHome_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Bạn muốn đến vị trí làm việc?", "CVEye",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialog == DialogResult.Yes)
            {
                GetMach3Instance();

                if (scriptObject != null)
                {
                    if (machinecoord)
                    {
                        scriptObject.DoOEMButton(107);
                        machinecoord = false;
                    }

                    scriptObject.Code("M93");

                    // High speed mode
                    lowSpeed = false;
                }
            }
            else return;
        }

        private void ChangingPosition_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Bạn muốn đến vị trí bảo dưỡng?", "CVEye",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialog == DialogResult.Yes)
            {
                GetMach3Instance();

                if (scriptObject != null)
                {
                    if (lowSpeed)
                        HighSpeedMode();

                    scriptObject.Code("G90 G55 G01 X550 Y0 F5000");
                }
            }
            else return;
        }

        private void Run_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Bạn muốn bắt đầu quá trình sơn?", "CVEye",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialog == DialogResult.Yes)
            {
                if (detected)
                {
                    GetMach3Instance();

                    if (scriptObject != null)
                    {
                        // Update working status
                        painting = true;
                        detected = false;
                        status_label.Text = "Đang sơn...";
                        status_label.Refresh();

                        // Disable some button
                        EnableButton(false);

                        // Run painting macro
                        scriptObject.Code("M999");
                    }
                }
                else
                    MessageBox.Show("Chưa quét ảnh.", "CVEye");
            }
            else return;
        }

        private void TestValve_Click(object sender, EventArgs e)
        {
            GetMach3Instance();

            if (scriptObject != null)
            {
                short channel = (short)(GetValveNum(item_color.Text) + 6);
                scriptObject.ActivateSignal(channel);
                scriptObject.Code("M92");
                scriptObject.DeActivateSignal(channel);
            }
        }

        private void TurnPiston_Click(object sender, EventArgs e)
        {
            GetMach3Instance();

            if (scriptObject != null)
            {
                short channel = (short)(GetValveNum(item_color.Text) + 6);
                if (TurnPiston.Text == "Hạ piston")
                {
                    scriptObject.ActivateSignal(channel);
                    scriptObject.ActivateSignal(12);
                    TurnPiston.Text = "Nâng piston";
                }
                else
                {
                    scriptObject.DeActivateSignal(12);
                    scriptObject.DeActivateSignal(channel);
                    Thread.Sleep(10);
                    TurnPiston.Text = "Hạ piston";
                }
            }
        }
        #endregion

        #region Other events
        
        private void Name_Changed(object sender, EventArgs e)
        {
            // Preview current template
            ViewTemplate(tmp_item_name.Text);

        }

        private void ViewTemplate(string value)
        {
            switch (value)
            {
                case "General 01":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/01/gen01.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/01/gen01.jpg").Bitmap;
                    break;
                case "Advisor 01":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/01/ad01.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/01/ad01.jpg").Bitmap;
                    break;
                case "Elephant 01":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/01/ele01.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/01/ele01.jpg").Bitmap;
                    break;
                case "Chariot 01":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/01/cha01.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/01/cha01.jpg").Bitmap;
                    break;
                case "Cannon 01":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/01/can01.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/01/can01.jpg").Bitmap;
                    break;
                case "Horse 01":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/01/hor01.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/01/hor01.jpg").Bitmap;
                    break;
                case "Soldier 01":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/01/sol01.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/01/sol01.jpg").Bitmap;
                    break;
                case "General 02":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/02/gen02.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/02/gen02.jpg").Bitmap;
                    break;
                case "Advisor 02":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/02/ad02.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/02/ad02.jpg").Bitmap;
                    break;
                case "Elephant 02":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/02/ele02.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/02/ele02.jpg").Bitmap;
                    break;
                case "Chariot 02":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/02/cha02.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/02/cha02.jpg").Bitmap;
                    break;
                case "Cannon 02":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/02/can02.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/02/can02.jpg").Bitmap;
                    break;
                case "Horse 02":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/02/hor02.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/02/hor02.jpg").Bitmap;
                    break;
                case "Soldier 02":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/02/sol02.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/02/sol02.jpg").Bitmap;
                    break;
            }
        }

        private void CVEye_Shown(object sender, EventArgs e)
        {
            GetMach3Instance();

            // Start Mach3
            if (mach3 == null)
                Init_Mach3();
        }

        private void CVEye_Load(object sender, EventArgs e)
        {
            timerDROupdate.Enabled = true;
        }
        
        private void CVEye_Closing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Bạn muốn thoát phần mềm?", "CVEye", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialog == DialogResult.No)
                e.Cancel = true;
            else
            {
                GetMach3Instance();

                if (scriptObject != null)
                    scriptObject.Code("M94");
                Thread.Sleep(100);

                // Close Mach3
                if (mach3 != null)
                    mach3.ShutDown();

                WriteSystemData();
            }
        }

        private void ColorChanged(object sender, EventArgs e)
        {
            valveNum.Text = (GetValveNum(item_color.Text) - 3).ToString();
        }

        private void TimerDROupdate_Tick(object sender, EventArgs e)
        {
            UpdateDRO();
        }
        #endregion
    }
}
