﻿/*
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
        //private const int pt_width = 50;
        //private const int pt_height = 38;
        private const int pt_width = 48;
        private const int pt_height = 36;
        private float square_size = 10f;
        private Size pt_size = new Size(pt_width, pt_height);
        private VectorOfPointF corners = new VectorOfPointF();
        private Bgr[] line_color_array = new Bgr[pt_width * pt_height];
        private static Mat[] frame_array_buffer = new Mat[100];
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
        //public static Image<Bgr, byte> img_capture;
        public static Image<Bgr, byte> img_capture_undist;
        public Mat tmp_raw = new Mat();
        //private Mat img_raw = new Mat();
        //private Mat img_gray = new Mat();
        private Mat img_threshold = new Mat();
        #endregion

        #region IO
        private string mainDirectory;
        private string mach3Directory;
        public static string macroDirectory;
        public static XDocument SysData;
        public static TextWriter gcode;
        #endregion

        #region Miscellaneous

        //const int roi_dim = 110;
        const int roi_dim = 121;

        const float in_circle_radius = (float)11.9; //mm

        const double x_pixel_offset = -43; // pixels
        const double y_pixel_offset = 9;

        List<double> angleListSort = new List<double>();

        public static bool first_start01 = true;

        private bool switchCamera = false;
        private bool initCamera = false;

        private bool lowSpeed = true;

        private bool xZero = false;
        private bool yZero = false;
        private bool zZero = false;
        private bool machineZero = false;

        private bool painting = false;
        private bool detected = false;
        //private bool detecting = false;
        private bool machinecoord = true;
        private bool resetBlinking = false;

        private bool first_item = true;
        private bool firstpointofitem = false;

        // E-stop toggling
        private bool hightolow = false;
        private bool preactive = false;
        private bool active = false;


        #endregion

        public CVEye()
        {
            InitializeComponent();

            Init_XML();

            Init_Directory();

            Init_Form();

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
                //Thread.Sleep(4000);

                GetMach3Instance();
                while (scriptObject == null)
                {
                    GetMach3Instance();
                }

                // Send CVEye window to front
                IntPtr CVEyeWindow = FindWindowByCaption(IntPtr.Zero, "CVEye");
                SetForegroundWindow(CVEyeWindow);

                // Enable main form after Mach3 is ready
                Enabled = true;

                // Link to CVEye directory
                Directory.SetCurrentDirectory(mainDirectory);

                if (scriptObject != null)
                {
                    // Machine coordinate toggle
                    scriptObject.DoOEMButton(256);

                    // set active status
                    if (scriptObject.IsActive(25) != 0)
                        preactive = true;
                    else
                        preactive = false;
                    active = preactive;
                    resetBlinking = true;

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

        // 10 Hz Update DRO and some other task
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

                    //
                    preactive = active; // false = low, true = high
                    if (scriptObject.IsActive(25) != 0) // press E-stop
                    {
                        active = true;
                        resetBlinking = true;
                    }
                    else // release E-stop
                    {
                        active = false;
                    }

                    if ((preactive) && (!active))
                    {
                        hightolow = true;
                    }
                    else
                    {
                        hightolow = false;
                    }

                    if (hightolow)
                    {
                        scriptObject.DoOEMButton(1021);
                        resetBlinking = false;
                    }

                    // Check E-stop toggling
                    //if ((scriptObject.IsActive(25) != 0) || (resetBlinking))
                    if (scriptObject.IsActive(25) != 0)
                    {
                        machStatus.Text = "Chế Độ Khẩn Cấp";
                        machStatus.Refresh();

                        EnableButton(true);
                    }
                    else
                    {
                        machStatus.Text = "Sẵn Sàng";
                        machStatus.Refresh();
                    }

                    // Check setting machine zero completed
                    if (!machineZero)
                        CheckMachineZero();

                    // Check painting completed
                    if (painting)
                        CheckPaintingCompleted(zDRO.Text);

                    // Toggle RESET button
                    ResetToggling();
                }
            }
            catch
            {
                return;
            }
        }

        private void CheckMachineZero()
        {
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

            machineZero = xZero & yZero & zZero;
        }

        private void CheckPaintingCompleted(string value)
        {
            double zSafe;
            zSafe = Math.Round(double.Parse(value), 1);

            if ((zSafe == 25.0) && (!scriptObject.IsOutputActive((short)(GetValveNum(item_color.Text) + 6))))
            {
                //status_label.Text = "Painting Completed"; 
                status_label.Text = "Hoàn Tất Phun Sơn";

                EnableButton(true);

                // disable painting flag
                painting = false;
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

        private void ResetToggling()
        {
            if (resetBlinking)
            {
                Reset.BackColor = (Reset.BackColor == Color.Red) ? Color.Orange : Color.Red;
            }
            else
                Reset.BackColor = Color.Red;
        }

        private void Init_Camera()
        {
            try
            {
                status_label.Text = "Đang Dò Camera...";
                status_label.Refresh();

                if (_capture == null)
                {
                    //Try to create the capture
                    try
                    {
                        _capture = new VideoCapture(1);
                        _capture.SetCaptureProperty(CapProp.FrameHeight, screen_height);
                        _capture.SetCaptureProperty(CapProp.FrameWidth, screen_width);

                        initCamera = true;
                    }
                    catch (NullReferenceException excpt)
                    {
                        //Show errors if there is any
                        MessageBox.Show(excpt.Message);
                    }
                }

                if (_capture != null)
                {
                    if (_capture.IsOpened)
                    {
                        //status_label.Text = "Camera On";
                        status_label.Text = "Camera Bật";
                        status_label.Refresh();

                        // Add handler for getting camera frame
                        Application.Idle += new EventHandler(Frame_Calibration);
                        _capture.Start();

                        cameraOn = true;
                        switchCamera = true;
                    }
                    else
                    {
                        MessageBox.Show("Không Tìm Thấy Camera", "CVEye");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        private void Init_Form()
        {
            Con_Painting_Point = new ConfigPaintingPoints();
            //Enabled = false;
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
                // Capture current frame
                Mat frame_raw = new Mat();
                _capture.Retrieve(frame_raw);

                // Convert to gray mat
                Mat frame_gray = new Mat();
                CvInvoke.CvtColor(frame_raw, frame_gray, ColorConversion.Bgr2Gray);

                // Processing
                using (Image<Bgr, byte> img_capture = frame_raw.ToImage<Bgr, byte>())
                {
                    // Calibration process
                    if (currentMode == Mode.SavingFrames)
                    {
                        //status_label.Text = "Saving Frames";
                        //status_label.Refresh();

                        find_chessboard = CvInvoke.FindChessboardCorners(frame_gray, pt_size, corners);

                        if (find_chessboard)
                        {
                            CvInvoke.CornerSubPix(frame_gray, corners, new Size(11, 11), new Size(-1, -1), new MCvTermCriteria(30, 0.05));

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

                                status_label.Text = "Frame: " + frame_buffer_savepoint.ToString() + "/" + frame_array_buffer.Length.ToString();
                                status_label.Refresh();
                            }

                            // Draw the found corners
                            img_capture.Draw(new CircleF(corners[0], 10), new Bgr(Color.Yellow), 4);
                            for (int i = 1; i < corners.Size; i++)
                            {
                                img_capture.Draw(new LineSegment2DF(corners[i - 1], corners[i]), new Bgr(Color.Blue), 3);
                                img_capture.Draw(new CircleF(corners[i], 10), new Bgr(Color.Yellow), 4);
                            }

                            // Wait for chessboard moving
                            Thread.Sleep(100);
                        }
                        else
                        {
                            corners = new VectorOfPointF();

                            status_label.Text = "Cannot Detect Conners";
                            status_label.Refresh();
                        }                        

                        // View raw image
                        pattern_field.Image = img_capture.Bitmap;
                        pattern_field.Refresh();
                    }

                    if (currentMode == Mode.Caluculating_Intrinsics)
                    {
                        status_label.Text = "Caluculating Intrinsics...";
                        status_label.Refresh();

                        for (int k = 0; k < frame_array_buffer.Length; k++)
                        {
                            // Detect corners of each stored frame
                            corners_point_vector[k] = new VectorOfPointF();
                            CvInvoke.FindChessboardCorners(frame_array_buffer[k], pt_size, corners_point_vector[k]);

                            // For accuracy 
                            CvInvoke.CornerSubPix(frame_gray, corners_point_vector[k], new Size(11, 11), new Size(-1, -1), new MCvTermCriteria(30, 0.05));

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

                            status_label.Text = "Corner Set: " + (k + 1).ToString() + "/" + frame_array_buffer.Length.ToString();
                            status_label.Refresh();
                        }


                        status_label.Text = "Getting Camera Matrices";
                        status_label.Refresh();

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
                        status_label.Text = "Saving Data";
                        status_label.Refresh();
                        Thread.Sleep(1000);

                        FileStorage fs = new FileStorage("_camera.xml", FileStorage.Mode.Write);
                        fs.Write(cameraMat, "Camera_Matrix");
                        fs.Write(distCoeffsMat, "Distortion_Coefficients");

                        status_label.Text = "Calibration Completed";
                        status_label.Refresh();
                    }

                    if (currentMode == Mode.Calibrated)
                    {
                        // Apply Calibration
                        Mat undist_frame = new Mat();
                        CvInvoke.Undistort(frame_raw, undist_frame, cameraMat, distCoeffsMat);
                        img_capture_undist = undist_frame.ToImage<Bgr, byte>();

                        // Draw image center lines
                        using (Image<Bgr, byte> img_draw = img_capture_undist.Clone())
                        {
                            Draw_Grid(img_draw);
                            pattern_field.Image = img_draw.Bitmap;
                            pattern_field.Refresh();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
        public Mat Frame_Analyze()
        {
            XElement ImageProcessingWindow = SysData.Element("System").Element("ImageProcessingWindow");

            Mat mat_gray = new Mat();
            using (Mat img_blur = new Mat(), img_hist = new Mat(), img_raw = img_capture_undist.Mat)
            {
                if (byte.Parse(ImageProcessingWindow.Element("ImageFiltering").Attribute("G_blur").Value) == 1)
                {
                    // Blurs an image using a Gaussian filter
                    Size ksize = new Size(9, 9);
                    double sigmaY = 0;
                    BorderType bordertype = BorderType.Reflect;
                    CvInvoke.GaussianBlur(img_raw, img_blur, ksize,
                        double.Parse(ImageProcessingWindow.Element("ImageFiltering").Attribute("gaussian_sig").Value),
                        sigmaY, bordertype);

                    //// Pyramid down and up filter
                    //PyrFilter(img_capture_undist.Mat);
                    //PyrFilter(img_capture_undist.Mat);
                    //PyrFilter(img_capture_undist.Mat);
                    //PyrFilter(img_capture_undist.Mat);

                    // Convert the image to grayscale
                    CvInvoke.CvtColor(img_blur, mat_gray, ColorConversion.Bgr2Gray);

                    // Applies an adaptive threshold to the image
                    CvInvoke.AdaptiveThreshold(mat_gray,
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
                    CvInvoke.CvtColor(img_blur, mat_gray, ColorConversion.Bgr2Gray);

                    CvInvoke.EqualizeHist(mat_gray, img_hist);

                    // Applies an adaptive threshold to the image
                    CvInvoke.AdaptiveThreshold(img_hist,
                        img_threshold,
                        255,
                        AdaptiveThresholdType.GaussianC,
                        ThresholdType.Binary,
                        95,
                        1);
                }
                // Dilite threshold image
                //var element = CvInvoke.GetStructuringElement(ElementShape.Cross, new Size(3, 3), new Point(-1, -1));
                //CvInvoke.Dilate(img_threshold, img_threshold, element, new Point(-1, -1), 2, BorderType.Reflect, default(MCvScalar));
                //CvInvoke.Erode(img_threshold, img_threshold, element, new Point(-1, -1), 2, BorderType.Reflect, default(MCvScalar));

                return mat_gray;
            }
        }

        public void Detect_Pattern()
        {
            using (StreamWriter logStream = new StreamWriter("history.txt", true))
            {
                logStream.WriteLine(DateTime.Now);
                logStream.WriteLine(tmp_item_name.Text);

                // Tick time
                Stopwatch watch = Stopwatch.StartNew();
                // Initiate processing time
                long currentTime = watch.ElapsedMilliseconds;

                // Load database
                ConfigPaintingPoints.dispensing_data = XDocument.Load("_database.xml");
                XElement ImageProcessingWindow = SysData.Element("System").Element("ImageProcessingWindow");
                XElement Parameters = SysData.Element("System").Element("PaintingConditionWindow").Element("Parameters");

                // Get current item painting points
                ConfigPaintingPoints.get_item = ConfigPaintingPoints.dispensing_data.Element("Field")
                    .Elements("Item")
                    .Where(x => x.Element("Name").Value == tmp_item_name.Text)
                    .Single();

                // List painting points to list
                List<XElement> point_list = ConfigPaintingPoints.get_item.Element("Points").Elements("Point").ToList();

                // Get processing time
                logStream.WriteLine("Load XML:   " + (watch.ElapsedMilliseconds - currentTime));
                currentTime = watch.ElapsedMilliseconds;
                // Update progress bar
                Progress.Value = 3;

                List<PointF> centerList = new List<PointF>();
                List<double> angleList = new List<double>();

                // Rotation center point
                PointF rot_cen = new PointF(tmp_raw.Rows / 2, tmp_raw.Cols / 2);

                // ROI size
                Size roi_size = new Size(roi_dim, roi_dim);

                // ROI location
                Point roi_location = new Point();

                using (Mat img_gray = Frame_Analyze())
                using (Image<Bgr, byte> img_items = img_capture_undist.Clone())
                using (Image<Gray, byte> tmp_rot_dir = new Image<Gray, byte>(roi_dim, roi_dim))
                using (gcode)
                {
                    // Get processing time
                    logStream.WriteLine("Source processing:   " + (watch.ElapsedMilliseconds - currentTime));
                    currentTime = watch.ElapsedMilliseconds;
                    // Update progress bar
                    Progress.Value = 5;

                    // Circle Hough Transform
                    CircleF[] img_circles = CvInvoke.HoughCircles(
                        img_gray, // Array of circles data
                        HoughType.Gradient,
                        2,
                        100,
                        int.Parse(ImageProcessingWindow.Element("HoughCirclesDetector").Attribute("houge_param1").Value),
                        int.Parse(ImageProcessingWindow.Element("HoughCirclesDetector").Attribute("houge_param2").Value),
                        int.Parse(ImageProcessingWindow.Element("HoughCirclesDetector").Attribute("min_ra").Value),
                        int.Parse(ImageProcessingWindow.Element("HoughCirclesDetector").Attribute("max_ra").Value));

                    // Display number of items
                    logStream.WriteLine("Num of Items:      " + img_circles.Length);
                    num_of_items.Text = img_circles.Length.ToString();

                    // Get processing time
                    logStream.WriteLine("Center detection:   " + (watch.ElapsedMilliseconds - currentTime));
                    currentTime = watch.ElapsedMilliseconds;
                    // Update progress bar
                    Progress.Value = 10;

                    // 2D Rotation matrix
                    Matrix<double> tmp_dst = new Matrix<double>(3, 3);

                    double raSum = 0;

                    for (int circle_num = 0; circle_num < img_circles.Length; circle_num++) // take the most memory
                    {
                        // 
                        CircleF circle = img_circles[circle_num];

                        // Inside painting mode
                        if (Inside.Checked)
                        {
                            // Cal ROI location
                            roi_location = new Point((int)Math.Round(circle.Center.X) - roi_dim / 2,
                                            (int)Math.Round(circle.Center.Y) - roi_dim / 2);

                            // Data allocation
                            byte[] roiData = new byte[roi_dim * roi_dim];
                            GCHandle handle = GCHandle.Alloc(roiData, GCHandleType.Pinned);
                            using (Image<Gray, byte> src = img_threshold.ToImage<Gray, byte>())
                            using (Image<Gray, byte> roi_getting_img = GetSourceROI(src, roi_location, roi_dim))
                            {
                                CvInvoke.BitwiseNot(roi_getting_img, roi_getting_img);

                                using (Mat tempMat = new Mat(roi_getting_img.Size, DepthType.Cv8U, 1, handle.AddrOfPinnedObject(), roi_getting_img.Width))
                                {
                                    CvInvoke.BitwiseNot(roi_getting_img, tempMat);
                                    //CvInvoke.Imwrite("data/roi" + circle_num.ToString() + ".jpg", tempMat); // Sure!!!
                                }
                                handle.Free();
                            }

                            #region Estimate pattern direction [Input: tmp_raw - Output: get_angle] 

                            // Maching value
                            int get_min = 0;
                            double get_angle = 0;
                            CircleF get_circle = new CircleF();

                            // Find maching angle
                            for (int cnt = 0; cnt < 360; cnt = cnt + 1)
                            {
                                // Sum of pixels value, update each cycle
                                int pixel_sum = 0;

                                // Template rotation 
                                using (Mat tmp_rot = new Mat())
                                {
                                    CvInvoke.GetRotationMatrix2D(rot_cen, cnt, 1, tmp_dst);
                                    CvInvoke.WarpAffine(tmp_raw, tmp_rot, tmp_dst, tmp_raw.Size); // ~100 ms => it takes almost scanning time, we should improve

                                    using (Image<Gray, byte> tmp_rot_img = tmp_rot.ToImage<Gray, byte>())
                                    using (Mat _tmp_rot = tmp_rot_img.Mat)
                                    {
                                        byte[] tmpData = new byte[roi_dim * roi_dim];
                                        GCHandle _handle = GCHandle.Alloc(tmpData, GCHandleType.Pinned);
                                        CvInvoke.BitwiseNot(_tmp_rot, _tmp_rot);
                                        using (Mat tempMat = new Mat(_tmp_rot.Size, DepthType.Cv8U, 1, _handle.AddrOfPinnedObject(), tmp_rot.Width))
                                        {
                                            CvInvoke.BitwiseNot(_tmp_rot, tempMat);
                                            //CvInvoke.Imwrite("data/tmp" + cnt.ToString() + ".jpg", tempMat); // Sure!!!
                                        }
                                        _handle.Free();

                                        // Get maching value
                                        for (int i = 0; i < 14641; i++) // Taking 15 - 20 ms
                                        {
                                            if (tmpData[i] == 0)
                                                pixel_sum += roiData[i];
                                        }

                                        // Update min value
                                        if (cnt == 0)
                                        {
                                            get_min = pixel_sum;
                                            get_angle = cnt;
                                        }
                                        else
                                        {
                                            if (pixel_sum < get_min)
                                            {
                                                get_min = pixel_sum;
                                                get_angle = cnt;
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion

                            get_circle = new CircleF(circle.Center, circle.Radius);

                            // Show detected items
                            Draw_Matching(img_items, get_circle, get_angle);

                            // Add data to list
                            angleList.Add(get_angle);
                            centerList.Add(circle.Center);

                        }
                        else
                        {
                            angleList.Add(0);
                            centerList.Add(circle.Center);
                            img_items.Draw(new CircleF(circle.Center, 60), new Bgr(Color.Red), 3);
                        }

                        raSum = raSum + circle.Radius;

                        // Update progress bar
                        Progress.Value = 10 + ((circle_num + 1) * 100 / img_circles.Length) * 90 / 100;
                        Progress.Refresh();
                    }

                    double raAvg = raSum / img_circles.Length;
                    double scale = 31 / (2 * raAvg);

                    logStream.WriteLine("raAvg:   " + raAvg);
                    logStream.WriteLine("scale:   " + scale);


                    logStream.WriteLine("Template Maching:   " + (watch.ElapsedMilliseconds - currentTime));
                    currentTime = watch.ElapsedMilliseconds;

                    // Item sorting
                    centerList = SortByDistance(centerList, angleList);
                    //centerList = HelixSort(centerList);

                    #region Macro of Painting Process
                    int t = 0;
                    foreach (PointF centerPoint in centerList)
                    {
                        // Calculate center coordinate
                        double cen_X = xCompensate(centerPoint.X) + x_pixel_offset;
                        double cen_Y = yCompensate(centerPoint.Y) + y_pixel_offset;
                        PointF cenPoint = new PointF((float)cen_X, (float)cen_Y);

                        // Global painting points rotation
                        double dirCom = 0.15;
                        cenPoint = RotateFOV(cenPoint, dirCom);

                        cen_X = Math.Round(cenPoint.X * scale, 3);
                        cen_Y = Math.Round(cenPoint.Y * scale, 3);

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
                            // Build gcode for circle painting
                            Running_Circle((float)cen_X, (float)cen_Y, Parameters);
                        }

                        // Draw the tracking line
                        //if (t > 0)
                        //    // Draw point to point
                        //    img_items.Draw(new LineSegment2DF(centerList[t - 1], centerList[t]), new Bgr(Color.Red), 2);
                        //else
                        //    // Draw the start point
                        //    img_items.Draw(new CircleF(centerList[t], 2), new Bgr(Color.Red), 2);

                        t++;
                    }
                    #endregion

                    #region Complete Painting Process

                    // Return to z safe
                    gcode.WriteLine("Code \"G00 Z" + Parameters.Attribute("zSafe").Value + "\"");
                    Wait(50);

                    // Turn off piston
                    gcode.WriteLine("DeactivateSignal(OUTPUT6)");
                    gcode.WriteLine("DeactivateSignal(OUTPUT" + GetValveNum(item_color.Text) + ")");

                    //Return home
                    gcode.WriteLine("Code \"G90 G54 G00 X-80 Y-430\"");
                    gcode.Close();
                    #endregion

                    logStream.WriteLine("Macro buiding:   " + (watch.ElapsedMilliseconds - currentTime));

                    // View result
                    pattern_field.Image = img_items.Bitmap;
                    pattern_field.Refresh();

                    // Save detected items result
                    //CvInvoke.Imwrite("result/1.img_capture_undist.jpg", img_capture_undist);
                    //CvInvoke.Imwrite("result/2.img_gray.jpg", img_gray);
                    //CvInvoke.Imwrite("result/3.img_edge_clone.jpg", img_edge_clone);
                    CvInvoke.Imwrite("result/4.img_items.jpg", img_items);
                    //CvInvoke.Imwrite("result/5.img_edge.jpg", mat_edge);
                    //CvInvoke.Imwrite("result/6.img_threshold.jpg", img_threshold);
                }

                // Tock timer
                logStream.WriteLine("Total:     " + watch.ElapsedMilliseconds);
                logStream.WriteLine("_________________________");
                elapsed_time.Text = watch.ElapsedMilliseconds.ToString();
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
            using (src)
            {
                // Set ROI to image
                Rectangle roi_rec = new Rectangle(ROILocation, new Size(ROISize, ROISize));
                CvInvoke.cvSetImageROI(src.Ptr, roi_rec);
                src.ROI = CvInvoke.cvGetImageROI(src.Ptr);

                // Create ROI background
                using (Image<Bgr, byte> roi_background = new Image<Bgr, byte>(src.Size))
                {
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
            }
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
            Size sample_size = new Size(pt_width, pt_height);

            // Convert BGR to GRAY image
            CvInvoke.CvtColor(sample, sample_frame, ColorConversion.Bgr2Gray);

            // Find corners on chess board image
            if (CvInvoke.FindChessboardCorners(sample_frame, sample_size, corner_set))
                CvInvoke.CornerSubPix(sample_frame, corner_set, new Size(11, 11), new Size(-1, -1), new MCvTermCriteria(30, 0.05));
            else
                MessageBox.Show("Cannot Find Corners", "CVEye");

            // Initialize some variables
            int ref_num = 1;
            byte point_index = 0;
            PointF[] com_point = new PointF[2]; // compensation point
            double slope, intercept;

            //byte getCol = 25;
            //byte getRow = 19;  

            float squareSize = (float)52.08;
            byte getCol = pt_width / 2;
            byte getRow = pt_height / 2;

            // Find XAxis node
            //section = SysData.Element("System")
            //.Elements("CameraCalibrationWindow")
            //.Elements("Compensation")
            //.Elements("Section")
            //.Where(x => x.Element("Name").Value == "XAxis")
            //.Single();
            //section.Element("Data").RemoveAll();

            section = SysData.Element("System")
            .Elements("CameraCalibrationWindow")
            .Elements("Compensation")
            .Elements("Section")
            .Where(x => x.Element("Name").Value == "XAxis")
            .Single();
            section.Element("Data").RemoveAll();


            // New variable
            PointF[] rowComPoint = new PointF[2];
            PointF[] colComPoint = new PointF[2];

            using (StreamWriter logStream = new StreamWriter("history.txt"))
            {
                // Scan all corner points
                for (int col = 0; col < pt_width; col++) // column scan                
                {
                    if (col >= 1)
                    {
                        logStream.WriteLine("Column:    " + col);

                        for (int row = 0; row < pt_height; row++) // row scan
                        {
                            // Corner index of array
                            byte rowOri = pt_height / 2;
                            int cornerPosOri = pt_width * rowOri + col;
                            int cornerPos = pt_width * row + col;

                            // Calculate stretch error
                            rowComPoint[point_index].X = corner_set[cornerPosOri].Y + squareSize * row - squareSize * rowOri; // ref
                            float err = corner_set[cornerPos].Y - rowComPoint[point_index].X;
                            rowComPoint[point_index].Y = err;

                            if (point_index > 0)
                            {
                                // a = (y0 - y1)/(x0 - x1)
                                slope = (rowComPoint[point_index - 1].Y - rowComPoint[point_index].Y) / (0 - squareSize);

                                // b = (x0y1 - x1y0)/(x0 - x1)
                                intercept = (0 * rowComPoint[point_index].Y - squareSize * rowComPoint[point_index - 1].Y) / (0 - squareSize);

                                slope = Math.Round(slope, 4);
                                intercept = Math.Round(intercept, 4);

                                // Draw something
                                sample.Draw(new Cross2DF(corner_set[cornerPos], 10, 10), new Bgr(Color.Red), 2);
                                sample.Draw(cornerPos.ToString(), new Point((int)corner_set[cornerPos].X - 10, (int)corner_set[cornerPos].Y - 10), FontFace.HersheyPlain, 1.2, new Bgr(Color.GreenYellow), 1);

                                // Test
                                //logStream.WriteLine(((row == 1) ? 0 : rowComPoint[point_index - 1].X) + " " + slope + "   " + intercept + "    " + ((row == pt_height - 1) ? 1944 : rowComPoint[point_index].X));

                                //section.Element("Data").Add(new XElement("Segment",
                                //    new XAttribute("start", (row == 1) ? 0 : rowComPoint[point_index - 1].X)),
                                //    new XAttribute("slope", slope),
                                //    new XAttribute("intercept", intercept),
                                //    new XAttribute("stop", ((row == pt_height - 1) ? 1944 : rowComPoint[point_index].X)));
                                //SysData.Save("_system.xml");

                                // Update previous value
                                rowComPoint[point_index - 1] = rowComPoint[point_index];
                                point_index = 0;
                            }
                            point_index++;
                        }
                    }

                    // Reset var
                    rowComPoint = new PointF[2];
                    point_index = 0;
                }
            }

            

            return;

            for (int i = pt_width * getRow; i < pt_width * getRow + pt_width; i++)
            {
                // Draw detected corners from left to right
                //sample.Draw(new CircleF(corner_set[i], 8), new Bgr(Color.Red), 2);
                sample.Draw(new Cross2DF(corner_set[i], 10, 10), new Bgr(Color.Red), 2);
                sample.Draw(new LineSegment2DF(new PointF(corner_set[pt_width * getRow + getCol].X + squareSize * (getCol - ref_num), 0), new PointF(corner_set[pt_width * getRow + getCol].X + squareSize * (getCol - ref_num), 1944)), ((getCol - ref_num) == 0) ? new Bgr(Color.Red) : new Bgr(Color.Blue), 1);

                // Calculate stretch error
                com_point[point_index].X = corner_set[pt_width * getRow + getCol].X + squareSize * (ref_num - getCol - 1);
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
                    new XAttribute("start", (i == pt_width * getRow + 1) ? 0 : com_point[point_index - 1].X),
                    new XAttribute("slope", slope),
                    new XAttribute("intercept", intercept),
                    new XAttribute("stop", (i == pt_width * getRow + (pt_width - 1)) ? 2592 : com_point[point_index].X)));

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
            point_index = 0;
            //for (int i = getCol; i < getCol + 50 * 38; i = i + 50)
            for (int i = getCol; i < getCol + pt_width * pt_height; i = i + pt_width)
            {
                // Draw detected corners from top to bottom
                // from getCol to getCol + 50 * 37
                //sample.Draw(new CircleF(corner_set[i], 8), new Bgr(Color.Red), 2);
                sample.Draw(new Cross2DF(corner_set[i], 10, 10), new Bgr(Color.Red), 2);
                sample.Draw(new LineSegment2DF(new PointF(0, corner_set[getCol + getRow * pt_width].Y + squareSize * (getRow - ref_num)), new PointF(2592, corner_set[getCol + getRow * pt_width].Y + squareSize * (getRow - ref_num))), ((getRow - ref_num) == 0) ? new Bgr(Color.Red) : new Bgr(Color.Blue), 1);

                // Calculate stretch error
                com_point[point_index].X = corner_set[getCol + getRow * pt_width].Y + squareSize * (ref_num - getRow - 1);
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
                    new XAttribute("start", (i == getCol + pt_width) ? 0 : com_point[point_index - 1].X),
                    new XAttribute("slope", slope),
                    new XAttribute("intercept", intercept),
                    new XAttribute("stop", (i == getCol + pt_width * pt_height - pt_width) ? 1944 : com_point[point_index].X)));

                    com_point[point_index - 1] = com_point[point_index];
                    point_index = 0;
                }

                ref_num++;
                point_index++;
            }

            SysData.Save("_system.xml");

            // For G54 calibration

            // Calculate center coordinate
            double cen_X = xCompensate(corner_set[pt_width * getRow + getCol].X) + x_pixel_offset;
            double cen_Y = yCompensate(corner_set[pt_width * getRow + getCol].Y) + y_pixel_offset;

            PointF cenPoint = new PointF((float)cen_X, (float)cen_Y);

            // Global painting points rotation
            cenPoint = RotateFOV(cenPoint, 0.15);

            cen_X = Math.Round(cenPoint.X * ConfigPaintingPoints.real_accuracy, 3);
            cen_Y = Math.Round(cenPoint.Y * ConfigPaintingPoints.real_accuracy, 3);

            // Invert y axis direction
            cen_Y = -cen_Y;

            gcode = new StreamWriter(Path.Combine(macroDirectory, @"M999.m1s"));
            gcode.WriteLine("Code \"G90 G54 G01 X" + cen_X + " Y" + cen_Y + " F5000\"");
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
                X = Math.Round((Points[j].X + x_pixel_offset) * ConfigPaintingPoints.real_accuracy, 3); //
                Y = -Math.Round((Points[j].Y + y_pixel_offset) * ConfigPaintingPoints.real_accuracy, 3); //

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
                    }
                    else
                    {
                        // First point of non-first item
                        gcode.WriteLine("Code \"G01 X" + X + " Y" + Y + " F" + Parameters.Attribute("xySpeed").Value + "\"");
                        Wait(500);
                    }

                    firstpointofitem = true;
                }
                else
                {
                    // Go to painting point of template
                    gcode.WriteLine("Code \"G01 X" + X + " Y" + Y + " F" + Parameters.Attribute("xySpeed").Value + "\"");
                    //Wait(50);

                    firstpointofitem = false;
                }

                // Dispense paint
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
            float startpoint_Y = cenY;

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

                // Dispense paint
                Paint_Drip(decimal.Parse(Parameters.Attribute("offset").Value));

                // Circular interpolation
                CircleDivider(new PointF(cenX, cenY), in_circle_radius, 4, int.Parse(Parameters.Attribute("circleSpeed").Value));
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

                // Dispense paint
                Paint_Drip(decimal.Parse(Parameters.Attribute("offset").Value));

                // Circular interpolation
                CircleDivider(new PointF(cenX, cenY), in_circle_radius, 4, int.Parse(Parameters.Attribute("circleSpeed").Value));
                Wait(50);

                // Go to z return
                gcode.WriteLine("Code \"G00 Z" + Parameters.Attribute("zReturn").Value + "\"");
            }
        }

        private void CircleDivider(PointF center, float radius, byte factor, int speed)
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
                gcode.WriteLine("Code \"G03 X" + endPoint.X + " Y" + endPoint.Y + " I" + (center.X - startPoint.X) + " J" + (center.Y - startPoint.Y) + ((i == 0) ? " F" + speed.ToString() : "") + "\"");

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
            Wait(firstpointofitem ? 100 : 50);

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
            gcode.WriteLine("Sleep(" + time.ToString() + ")");
            gcode.WriteLine("Wend");
        }
        
        private byte GetValveNum(string colorName)
        {
            byte valve_num = 0;

            switch (colorName)
            {
                case "Đen":
                    valve_num = 4;
                    break;
                case "Đỏ":
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
                case "Đỏ":
                    valve_num = "G54";
                    break;
                case "Đen":
                    valve_num = "G55";
                    break;
            }
            return valve_num;
        }

        private void Draw_Matching(Image<Bgr, byte> Input, CircleF circle, double angle)
        {
            int line_width = 2;

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
            const double _scale = 52.08; // pixels per 10 mm
            LineSegment2DF vertical = new LineSegment2DF();
            LineSegment2DF horizontal = new LineSegment2DF();

            // Vertical lines
            byte vline_num = 48;
            for (int i = 1; i < vline_num; i++)
            {
                vertical.P1 = new PointF(screen_width / 2 + (float)_scale * (i - vline_num / 2), 0);
                vertical.P2 = new PointF(screen_width / 2 + (float)_scale * (i - vline_num / 2), screen_height);
                if (i == vline_num / 2)
                {
                    Input.Draw(vertical, new Bgr(Color.Red), 2);
                }
                else
                {
                    //Input.Draw(vertical, new Bgr(Color.White), 1);
                }
            }

            // Horizontal lines
            byte hline_num = 36;
            for (int i = 0; i < hline_num; i++)
            {
                horizontal.P1 = new PointF(0, screen_height / 2 + (float)_scale * (i - hline_num / 2 - 1));
                horizontal.P2 = new PointF(screen_width, screen_height / 2 + (float)_scale * (i - hline_num / 2 - 1));
                if (i == hline_num / 2 + 1)
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
            ConfigImageProcessing Con_Image_Processing = new ConfigImageProcessing();
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
            using (OpenFileDialog s_file = new OpenFileDialog())
            {
                if (s_file.ShowDialog() == DialogResult.OK)
                {
                    img_capture_undist = new Image<Bgr, byte>(s_file.FileName);
                    pattern_field.Image = img_capture_undist.Bitmap;
                }
            }
        }

        private void Video_Click(object sender, EventArgs e)
        {
            if (initCamera)
            {
                // Restart camera
                if (_capture.IsOpened)
                {
                    if (!switchCamera) // to prevent more than one click when camera is on
                    {


                        // Add handler for getting camera frame
                        Application.Idle += new EventHandler(Frame_Calibration);
                        _capture.Start();

                        // Reset progress bar
                        Progress.Value = 0;

                        // Update working status
                        //status_label.Text = "Camera On";
                        status_label.Text = "Camera Bật";
                        status_label.Refresh();

                        switchCamera = true;
                    }
                    else MessageBox.Show("Camera Đang Mở", "CVEye");

                }
                else
                {
                    MessageBox.Show("Không Tìm Thấy Camera", "CVEye");
                }

            }
            else Init_Camera();
        }

        private void Capture_Click(object sender, EventArgs e)
        {
            try
            {
                if (_capture != null)
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
                        using (Image<Bgr, byte> img_draw = img_capture_undist.Clone())
                        {
                            Draw_Grid(img_draw);
                            pattern_field.Image = img_draw.Bitmap;
                            pattern_field.Refresh();
                        }

                        // Update working status
                        status_label.Text = "Đã Chụp Ảnh";
                        status_label.Refresh();

                        // 
                        switchCamera = false;
                    }
                    else MessageBox.Show("Chưa Mở Camera", "CVEye");
                }
                else
                {
                    //status_label.Text = "No Camera Data";
                    status_label.Text = "Không Có Ảnh";
                    status_label.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DetectClick(object sender, EventArgs e)
        {
            try
            {
                first_item = true; // reset first item flag

                // View captured image
                pattern_field.Image = img_capture_undist.Bitmap;
                pattern_field.Refresh();

                // Create new mach3 macro
                if (gcode != null)
                    gcode.Close();
                gcode = new StreamWriter(Path.Combine(macroDirectory, @"M999.m1s"));

                // Reload XML
                SysData = XDocument.Load("_system.xml");

                // Update working status
                //status_label.Text = "Detecting...";

                status_label.Text = "Đang quét ảnh...";
                pattern_field.Refresh(); // to remove the old text on camera window
                status_label.Refresh();

                //detecting = true;

                // Start detecting
                Detect_Pattern();

                // Update working status
                //status_label.Text = "Detected";
                status_label.Text = "Hoàn tất quét ảnh";
                status_label.Refresh();

                detected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ResetClick(object sender, EventArgs e)
        {
            GetMach3Instance();

            if (scriptObject != null)
            {
                // Reset OEM
                //scriptObject.DoOEMButton(1021);

                //resetBlinking = (resetBlinking) ? false : true;
            }
            //else MessageBox.Show("Can not connect to Mach3.");

            // update painting status
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
                machineZero = false;

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
                if (machineZero)
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
                else MessageBox.Show("Bạn chưa thiết lập gốc máy.", "CVEye");
            }
            else return;
        }

        private void ChangingPosition_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Bạn muốn đến vị trí bảo dưỡng?", "CVEye",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialog == DialogResult.Yes)
            {
                if (machineZero)
                {
                    GetMach3Instance();

                    if (scriptObject != null)
                    {
                        if (lowSpeed)
                            HighSpeedMode();

                        scriptObject.Code("G90 G54 G01 X420 Y-430 F6000");
                    }
                }
                else MessageBox.Show("Bạn chưa thiết lập gốc máy.", "CVEye");
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
                        //EnableButton(false);

                        // Run painting macro
                        scriptObject.Code("M999");
                        Thread.Sleep(500);
                    }
                }
                else MessageBox.Show("Chưa quét ảnh.", "CVEye");
            }
            else return;
        }

        private void lockCylinderClick(object sender, EventArgs e)
        {
            GetMach3Instance();

            if (scriptObject != null)
            {
                if (lockCylinder.Text == "Khóa khay")
                {
                    scriptObject.ActivateSignal(17);
                    lockCylinder.Text = "Mở khóa khay";
                }
                else
                {
                    scriptObject.DeActivateSignal(17);
                    lockCylinder.Text = "Khóa khay";
                }
            }
        }

        private void TestValve_Click(object sender, EventArgs e)
        {
            GetMach3Instance();

            if (scriptObject != null)
            {
                scriptObject.DeActivateSignal(10);
                scriptObject.DeActivateSignal(11);

                short channel = (short)(GetValveNum(item_color.Text) + 6);
                scriptObject.ActivateSignal(channel);
                scriptObject.Code("M92");
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
            Template.Refresh();
        }

        private void ViewTemplate(string value)
        {
            switch (value)
            {
                //case "General 01":
                case "Tướng 01":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/01/gen01.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/01/gen01.jpg").Bitmap;
                    break;
                //case "Advisor 01":
                case "Sĩ 01":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/01/ad01.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/01/ad01.jpg").Bitmap;
                    break;
                //case "Elephant 01":
                case "Tượng 01":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/01/ele01.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/01/ele01.jpg").Bitmap;
                    break;
                //case "Chariot 01":
                case "Xe 01":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/01/cha01.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/01/cha01.jpg").Bitmap;
                    break;
                //case "Cannon 01":
                case "Pháo 01":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/01/can01.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/01/can01.jpg").Bitmap;
                    break;
                //case "Horse 01":
                case "Ngựa 01":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/01/hor01.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/01/hor01.jpg").Bitmap;
                    break;
                //case "Soldier 01":
                case "Chốt 01":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/01/sol01.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/01/sol01.jpg").Bitmap;
                    break;
                //case "General 02":
                case "Tướng 02":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/02/gen02.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/02/gen02.jpg").Bitmap;
                    break;
                //case "Advisor 02":
                case "Sĩ 02":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/02/ad02.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/02/ad02.jpg").Bitmap;
                    break;
                //case "Elephant 02":
                case "Tượng 02":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/02/ele02.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/02/ele02.jpg").Bitmap;
                    break;
                //case "Chariot 02":
                case "Xe 02":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/02/cha02.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/02/cha02.jpg").Bitmap;
                    break;
                //case "Cannon 02":
                case "Pháo 02":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/02/can02.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/02/can02.jpg").Bitmap;
                    break;
                //case "Horse 02":
                case "Ngựa 02":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/02/hor02.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/02/hor02.jpg").Bitmap;
                    break;
                //case "Soldier 02":
                case "Chốt 02":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/02/sol02.jpg");
                    Template.Image = new Image<Bgr, byte>("pattern_data/02/sol02.jpg").Bitmap;
                    break;
            }
        }

        private void CVEye_Shown(object sender, EventArgs e)
        {
            //GetMach3Instance();

            ////Start Mach3
            //if (mach3 == null)
            //    Init_Mach3();
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

        private void button3_Click(object sender, EventArgs e)
        {
            //using (img_capture_undist)
            {
                //CvInvoke.Resize(img_capture_undist, img_capture_undist, new Size(121, 121));
                //CvInvoke.Threshold(img_capture_undist, img_threshold, 200, 255, ThresholdType.Binary);
                ////img_threshold.ConvertTo(img_threshold, DepthType.Cv8U);
                //CvInvoke.Imwrite("result/6.img_threshold.jpg", img_threshold);
                CvInvoke.PyrDown(img_capture_undist, img_capture_undist, BorderType.Default);
                CvInvoke.Imwrite("result/PyrDown.jpg", img_capture_undist);
            }

        }

        private void PyrFilter(Mat src)
        {
            CvInvoke.PyrDown(src, src, BorderType.Default);
            CvInvoke.PyrUp(src, src, BorderType.Default);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (Image<Bgr, byte> drawImg = CVEye.img_capture_undist.Clone())
            {
                //
                CVEye.PixelsCompensation(drawImg);
                CVEye.Draw_Grid(drawImg);
                pattern_field.Image = drawImg.Bitmap;
                pattern_field.Refresh();
                CvInvoke.Imwrite("result/4.img_items.jpg", drawImg);
            }
        }

        private void TimerDROupdate_Tick(object sender, EventArgs e)
        {
            UpdateDRO();
        }
        #endregion
    }
}
