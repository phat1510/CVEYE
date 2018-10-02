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
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Mach4;

namespace CVEYEV1
{
    public partial class CVEye : Form
    {
        #region Mach3

        private IMach4 mach3;
        private IMyScriptObject scriptObject;

        #endregion

        #region User32.dll

        [DllImport("USER32.DLL")]

        public static extern bool SetForegroundWindow(IntPtr CVEyeWindow);

        [DllImport("USER32.DLL", EntryPoint = "FindWindow", SetLastError = true)]

        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        #endregion

        #region Camera Calibration

        public static bool cameraOn = false;
        public static bool start_calib = false;
        private bool find_chessboard;
        private VideoCapture _capture;
        private const int screen_height = 1944;
        private const int screen_width = 2592;
        private const int pt_width = 46;
        private const int pt_height = 34;
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

        public static MCvPoint3D32f[][] corners_object_list;
        public static PointF[][] corners_point_list;
        public static VectorOfPointF[] corners_point_vector;
        Mat[] rvecs, tvecs;
        Mat cameraMat = new Mat(3, 3, DepthType.Cv64F, 1);
        Mat distCoeffsMat = new Mat(14, 1, DepthType.Cv64F, 1);

        #endregion

        #region Image processing

        public static Image<Bgr, byte> img_capture_undist;
        private Mat img_threshold = new Mat();
        private Image<Bgr, byte> img_items;
        private Image<Bgr, byte> tmpBgr;

        public Mat tmp_raw = new Mat();     
        private PointF[] affine_painting_points;
        const byte ROIsize = 123;
        const double phi31Dia_ROIscale = 1.366667;
        public static double _accuracy;

        #endregion

        #region IO

        private string mainDirectory;
        private string mach3Directory;
        private static string macroDirectory;
        public static XDocument SysData;
        private static TextWriter macro01, macro02;
        private XElement getItem;
        private  XDocument dispensingData;
        private string data_path = "_database.xml";
        private static string systemPath = "_system.xml";

        #endregion

        #region Miscellaneous

        const float in_circle_radius = 12.0f;

        // 
        public static double x_pixel_offset;
        public static double y_pixel_offset;

        private bool switchCamera = false;
        private bool initCamera = false;
        private bool lowSpeed = true;
        private bool xZero = false;
        private bool yZero = false;
        private bool zZero = false;
        private bool machineZero = false;
        private bool painting = false;
        private bool detected = false;
        private bool machinecoord = true;
        private bool resetBlinking = false;
        private bool first_item = true;
        //private bool firstpointofitem = false;
        private bool machStart = false;
        private bool macro02On = false;
        private bool macro02Used = false;

        // E-stop toggling
        private bool hightolow = false;
        private bool preactive = false;
        private bool active = false;
        int temp = 0;

        // Template localization
        List<double> angleListSort = new List<double>();
        List<double> _angleList;
        List<PointF> _centerList;       
        double get_angle = 0;
        CircleF get_circle = new CircleF();

        Stopwatch detectionWatch;

        #endregion

        public CVEye()
        {
            InitializeComponent();

            InitBackgroundWorker();

            Init_XML();

            Init_Directory();

            Init_UI();
        }

        // Set up the BackgroundWorker object by 
        // attaching event handlers. 
        private void InitBackgroundWorker()
        {
            backgroundWorker.DoWork +=
            new DoWorkEventHandler(backgroundWorker_DoWork);

            backgroundWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);

            backgroundWorker.ProgressChanged +=
                new ProgressChangedEventHandler(
            backgroundWorker_ProgressChanged);
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

                // Find the CVEye window
                IntPtr CVEyeWindow = FindWindowByCaption(IntPtr.Zero, "CVEye (Beta)");

                //
                while (scriptObject == null)
                {
                    GetMach3Instance();
                    // Send CVEye window to front
                    if (!SetForegroundWindow(CVEyeWindow))
                        SetForegroundWindow(CVEyeWindow);

                    // Wait mach3 starting completed
                    Thread.Sleep(500);
                }
                
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
                if (machStart)
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
                            lockCylinder.Text = "Khóa khay";
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

                            //
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

                        // Checking for macro02 running
                        if (!scriptObject.IsOutputActive(12) && !scriptObject.IsOutputActive(11) && macro02Used)
                        {
                            scriptObject.Code("M998");
                            macro02Used = false;
                        }
                    }
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

                lockCylinder.Text = "Khóa khay";

                EnableButton(true);

                // disable painting flag
                painting = false;
            }
        }
        public void HighSpeedMode()
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
                    try
                    {
                        XElement ImageProcessingWindow = SysData.Element("System").Element("ImageProcessingWindow");
                        int camIndex = int.Parse(ImageProcessingWindow.Element("CameraID").Attribute("ID").Value);
                        _capture = new VideoCapture(camIndex);
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
            SysData = XDocument.Load(systemPath);

            XElement MainWindow = SysData.Element("System").Element("MainWindow");

            // Last item info
            tmp_item_name.Text = MainWindow.Element("Template").Attribute("LastItem").Value;
            item_color.Text = MainWindow.Element("Template").Attribute("LastColor").Value;

            // Get last machine-camera offset value
            xOffset.Value = decimal.Parse(MainWindow.Element("CameraOffset").Attribute("X").Value);
            yOffset.Value = decimal.Parse(MainWindow.Element("CameraOffset").Attribute("Y").Value);

            // Get camera accuracy
            _accuracy = double.Parse(SysData.Element("System").Element("CameraCalibrationWindow").Element("Accuracy").Attribute("Value").Value);
            
            // Compute camera offset value
            x_pixel_offset = (double)xOffset.Value / _accuracy;
            y_pixel_offset = (double)yOffset.Value / _accuracy;

            //
            valveNum.Text = (GetValveNum(item_color.Text) - 3).ToString();

            // View the previous working template
            ViewTemplate(tmp_item_name.Text);
            CheckTmpSize();

            // View template image
            templateField.Image = tmpBgr.Bitmap;
            templateField.Refresh();
        }

        private void WriteSystemData()
        {
            SysData = XDocument.Load(systemPath);

            bool clear_XML = false;
            if (clear_XML)
            {
                // Setting
                XmlTextWriter writer = new XmlTextWriter(systemPath, System.Text.Encoding.UTF8);
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

            // Update data of main window
            XElement MainWindow = SysData.Element("System").Element("MainWindow");

            MainWindow.Element("Template").Attribute("LastItem").Value = tmp_item_name.Text;
            MainWindow.Element("Template").Attribute("LastColor").Value = item_color.Text;
            MainWindow.Element("CameraOffset").Attribute("X").Value = xOffset.Value.ToString();
            MainWindow.Element("CameraOffset").Attribute("Y").Value = yOffset.Value.ToString();

            // Save document
            SysData.Save(systemPath);
        }

        private void Init_UI()
        {
            ledX.BackColor = Color.Gray;
            ledY.BackColor = Color.Gray;
            ledZ.BackColor = Color.Gray;

            stopDetection.Enabled = false;
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
                        //img_capture_undist = frame_raw.ToImage<Bgr, byte>();

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
        public void Real_PointsWarpAffine(List<XElement> point_list, PointF objectCenter, double objectDir, Image<Bgr, byte> draw_image_bgr)
        {
            affine_painting_points = new PointF[20];

            // Invert rotation direction
            double angle = Math.Round((360 - objectDir) * Math.PI / 180, 3);

            // Reset number of points
            int point_num = 0;

            // Transformation loop
            foreach (XElement element in point_list)
            {
                // Rotation and translation of painting points set
                affine_painting_points[point_num].X =
                    (float)((double.Parse(element.Attribute("X").Value) - ROIsize / 2) * Math.Cos(angle)) - (float)(((double.Parse(element.Attribute("Y").Value) - ROIsize / 2) * Math.Sin(angle))) + objectCenter.X;
                affine_painting_points[point_num].Y =
                    (float)((double.Parse(element.Attribute("X").Value) - ROIsize / 2) * Math.Sin(angle)) + (float)(((double.Parse(element.Attribute("Y").Value) - ROIsize / 2) * Math.Cos(angle))) + objectCenter.Y;

                // For checking
                draw_image_bgr.Draw(new Cross2DF(affine_painting_points[point_num], 2, 2), new Bgr(Color.Red), 1);

                // Compute machine points
                affine_painting_points[point_num].X = (float)xCompensate(affine_painting_points[point_num]);
                affine_painting_points[point_num].Y = (float)yCompensate(affine_painting_points[point_num]);

                // Matching camera coordinate with machine coordinate
                affine_painting_points[point_num] = RotateFOV(affine_painting_points[point_num], 0.5);

                point_num++;
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
            //
            XElement ImageProcessingWindow = SysData.Element("System").Element("ImageProcessingWindow");

            using (Image<Bgr, byte> sourceImg = img_capture_undist.Clone())
            {
                Mat homograyphy = new Mat();

                // Coordinates of quadrangle vertices
                PointF[] src = new PointF[4];
                PointF[] dst = new PointF[4];

                float leftEdge = float.Parse(ImageProcessingWindow.Element("PSTTransform").Attribute("edge1").Value);
                float rightEdge = float.Parse(ImageProcessingWindow.Element("PSTTransform").Attribute("edge3").Value);
                float topEdge = float.Parse(ImageProcessingWindow.Element("PSTTransform").Attribute("edge2").Value);
                float botEdge = float.Parse(ImageProcessingWindow.Element("PSTTransform").Attribute("edge4").Value);

                using (Image<Bgr, byte> imgHalfLeft = img_capture_undist.Clone())
                {
                    // Half left perspective transform
                    src[0] = new PointF(leftEdge, 0);
                    src[1] = new PointF(screen_width / 2, 0);
                    src[2] = new PointF(screen_width / 2, screen_height);
                    src[3] = new PointF(leftEdge, screen_height);
                    dst[0] = new PointF(0, 0);
                    dst[1] = new PointF(screen_width / 2, 0);
                    dst[2] = new PointF(screen_width / 2, screen_height);
                    dst[3] = new PointF(0, screen_height);
                    homograyphy = CvInvoke.GetPerspectiveTransform(src, dst);
                    CvInvoke.WarpPerspective(img_capture_undist, imgHalfLeft, homograyphy, img_capture_undist.Size);

                    // Set ROI to image
                    Rectangle roi_rec = new Rectangle(new Point(0, 0), new Size(screen_width / 2, screen_height));
                    CvInvoke.cvSetImageROI(imgHalfLeft.Ptr, roi_rec);

                    // Half right perspective transform
                    src[0] = new PointF(screen_width / 2, 0);
                    src[1] = new PointF(screen_width - rightEdge, 0);
                    src[2] = new PointF(screen_width - rightEdge, screen_height);
                    src[3] = new PointF(screen_width / 2, screen_height);
                    dst[0] = new PointF(screen_width / 2, 0);
                    dst[1] = new PointF(screen_width, 0);
                    dst[2] = new PointF(screen_width, screen_height);
                    dst[3] = new PointF(screen_width / 2, screen_height);
                    homograyphy = CvInvoke.GetPerspectiveTransform(src, dst);
                    CvInvoke.WarpPerspective(img_capture_undist, sourceImg, homograyphy, img_capture_undist.Size);
                    homograyphy.Dispose();

                    // Merge half left and half right
                    CvInvoke.cvSetImageROI(sourceImg.Ptr, roi_rec);
                    imgHalfLeft.CopyTo(sourceImg);
                    CvInvoke.cvResetImageROI(sourceImg.Ptr);
                }

                using (Image<Bgr, byte> imgHalfTop = sourceImg.Clone())
                {
                    // Half top perspective transform
                    src[0] = new PointF(0, topEdge);
                    src[1] = new PointF(screen_width, topEdge);
                    src[2] = new PointF(screen_width, screen_height / 2);
                    src[3] = new PointF(0, screen_height / 2);
                    dst[0] = new PointF(0, 0);
                    dst[1] = new PointF(screen_width, 0);
                    dst[2] = new PointF(screen_width, screen_height / 2);
                    dst[3] = new PointF(0, screen_height / 2);
                    homograyphy = CvInvoke.GetPerspectiveTransform(src, dst);
                    CvInvoke.WarpPerspective(sourceImg, imgHalfTop, homograyphy, sourceImg.Size);

                    // Set ROI to image
                    Rectangle roi_rec = new Rectangle(new Point(0, 0), new Size(screen_width, screen_height / 2));
                    CvInvoke.cvSetImageROI(imgHalfTop.Ptr, roi_rec);

                    // Half bot perspective transform
                    src[0] = new PointF(0, screen_height / 2);
                    src[1] = new PointF(screen_width, screen_height / 2);
                    src[2] = new PointF(screen_width, screen_height - botEdge);
                    src[3] = new PointF(0, screen_height - botEdge);
                    dst[0] = new PointF(0, screen_height / 2);
                    dst[1] = new PointF(screen_width, screen_height / 2);
                    dst[2] = new PointF(screen_width, screen_height);
                    dst[3] = new PointF(0, screen_height);
                    homograyphy = CvInvoke.GetPerspectiveTransform(src, dst);
                    CvInvoke.WarpPerspective(sourceImg, sourceImg, homograyphy, sourceImg.Size);
                    homograyphy.Dispose();

                    // Merge half left and half right
                    CvInvoke.cvSetImageROI(sourceImg.Ptr, roi_rec);
                    imgHalfTop.CopyTo(sourceImg);
                    CvInvoke.cvResetImageROI(sourceImg.Ptr);
                }

                //
                Mat mat_gray = new Mat();
                using (Mat img_blur = new Mat(), img_hist = new Mat(), img_raw = sourceImg.Mat)
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

                    return mat_gray;
                }
            }
        }

        public void DetectClone()
        {
            //
            detectionWatch = Stopwatch.StartNew();

            // Load database
            dispensingData = XDocument.Load(data_path);

            // Call ImageProcessingWindow node
            XElement ImageProcessingWindow = SysData.Element("System").Element("ImageProcessingWindow");

            // Update progress bar
            Progress.Value = 3;

            img_items = img_capture_undist.Clone(); // => should be fixed
            using (Mat img_gray = Frame_Analyze())
            {
                // Update progress bar
                Progress.Value = 5;

                // Circle Hough Transform
                CircleF[] img_circles = CvInvoke.HoughCircles(
                    img_gray, // Array of circles data
                    HoughType.Gradient,
                    1.7,
                    100,
                    int.Parse(ImageProcessingWindow.Element("HoughCirclesDetector").Attribute("houge_param1").Value),
                    int.Parse(ImageProcessingWindow.Element("HoughCirclesDetector").Attribute("houge_param2").Value),
                    int.Parse(ImageProcessingWindow.Element("HoughCirclesDetector").Attribute("min_ra").Value),
                    int.Parse(ImageProcessingWindow.Element("HoughCirclesDetector").Attribute("max_ra").Value));

                if (img_circles.Length != 0)
                {
                    // Display number of items
                    processLog.Items.Add("Số mẫu tìm được:  " + img_circles.Length.ToString() + " mẫu.");
                    processLog.Refresh();

                    // Update progress bar
                    Progress.Value = 10;

                    // Renew data
                    _angleList = new List<double>();
                    _centerList = new List<PointF>();
                    get_angle = 0;
                    get_circle = new CircleF();

                    // Arguments of async process
                    List<object> arguments = new List<object>();
                    arguments.Add(img_circles);
                    arguments.Add(img_items); // => should be fixed

                    // Run async work
                    backgroundWorker.RunWorkerAsync(arguments);
                }
                else
                {
                    //
                    stopDetection.Enabled = false;
                    startDetection.Enabled = true;
                    turnCamera.Enabled = true;
                    captureImg.Enabled = true;

                    //
                    MessageBox.Show("Không tìm thấy mẫu. Thêm số lượng mẫu hoặc chụp ảnh lại.", "CVEye");
                }

            }
        }

        private void mainLoop(CircleF[] img_circles, Image<Bgr, byte> img_items, BackgroundWorker worker, DoWorkEventArgs e)
        {
            List<object> arguments = new List<object>();

            for (int circle_num = 0; circle_num < img_circles.Length; circle_num++) // take the most memory
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                }
                else
                {
                    //
                    temp = circle_num;

                    // Getting each circle
                    CircleF circle = img_circles[circle_num];

                    // Inside painting mode
                    if (Inside.Checked)
                    {
                        // Cal ROI location
                        Point roi_location = new Point((int)Math.Round(circle.Center.X) - ROIsize / 2,
                                        (int)Math.Round(circle.Center.Y) - ROIsize / 2);

                        Mat getMat = new Mat();

                        // Data allocation
                        byte[] roiData = new byte[ROIsize * ROIsize];
                        GCHandle handle = GCHandle.Alloc(roiData, GCHandleType.Pinned);
                        using (Image<Gray, byte> src = img_threshold.ToImage<Gray, byte>())
                        using (Image<Gray, byte> roi_getting_img = GetSourceROI(src, roi_location, ROIsize))
                        {
                            CvInvoke.BitwiseNot(roi_getting_img, roi_getting_img);

                            using (Mat tempMat = new Mat(roi_getting_img.Size, DepthType.Cv8U, 1, handle.AddrOfPinnedObject(), roi_getting_img.Width))
                            {
                                CvInvoke.BitwiseNot(roi_getting_img, tempMat);
                                getMat = tempMat.Clone();
                            }

                            //
                            handle.Free();
                        }

                        #region Estimate pattern direction [Input: tmp_raw - Output: get_angle] 

                        // Maching value
                        int get_min = 0;
                        //double get_angle = 0;
                        //CircleF get_circle = new CircleF();

                        // 2D Rotation matrix
                        Matrix<double> tmp_dst = new Matrix<double>(3, 3);

                        int numOfpixels = ROIsize * ROIsize;

                        // Find maching angle
                        for (int cnt = 0; cnt < 360; cnt = cnt + 2)
                        {
                            // Sum of pixels value, update each cycle
                            int pixel_sum = 0;
                            
                            // Template rotation 
                            using (Mat tmp_rot = new Mat())
                            {
                                //

                                CvInvoke.GetRotationMatrix2D(new PointF(ROIsize / 2, ROIsize / 2), cnt, 1, tmp_dst);
                                CvInvoke.WarpAffine(tmp_raw, tmp_rot, tmp_dst, tmp_raw.Size); // ~100 ms => it takes almost scanning time, we should improve

                                //
                                using (Image<Gray, byte> tmp_rot_img = tmp_rot.ToImage<Gray, byte>())
                                using (Mat _tmp_rot = tmp_rot_img.Mat)
                                {
                                    byte[] tmpData = new byte[ROIsize * ROIsize];
                                    GCHandle _handle = GCHandle.Alloc(tmpData, GCHandleType.Pinned);
                                    CvInvoke.BitwiseNot(_tmp_rot, _tmp_rot);
                                    using (Mat tempMat = new Mat(_tmp_rot.Size, DepthType.Cv8U, 1, _handle.AddrOfPinnedObject(), tmp_rot.Width))
                                    {
                                        CvInvoke.BitwiseNot(_tmp_rot, tempMat);
                                        //CvInvoke.Imwrite("data/tmp" + cnt.ToString() + ".jpg", tempMat); // Sure!!!
                                    }
                                    _handle.Free();

                                    // Get maching value
                                    for (int i = 0; i < 15129; i++) // Taking 15 - 20 ms
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

                        // For center correction, not yet implemented
                        get_circle = circle;
                    }
                    else
                    {
                        //
                        get_circle = circle;

                        // Add object data to their lists
                        _angleList.Add(0);
                        _centerList.Add(get_circle.Center);
                        float inradius = 82f; //pixels
                        img_items.Draw(new CircleF(get_circle.Center, inradius), new Bgr(Color.GreenYellow), 3);

                    }

                    // Call ProcessChange Event
                    worker.ReportProgress(10 + ((circle_num + 1) * 100 / img_circles.Length) * 90 / 100);
                }                
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            List<object> genericlist = e.Argument as List<object>;

            mainLoop((CircleF[])genericlist[0], (Image<Bgr, byte>)genericlist[1], worker, e);
        }


        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (Inside.Checked)
                {
                    // Add object data to their lists
                    _angleList.Add(get_angle);
                    _centerList.Add(get_circle.Center);

                    // Show detected items
                    Draw_Matching(img_items, get_circle, get_angle);
                    processLog.Items.Add(get_circle.Radius);
                }

                // Update progess bar
                Progress.Value = e.ProgressPercentage;

                //
                templateField.Refresh();

            }
            catch (Exception ex)
            {
                //
                MessageBox.Show(ex.Message, "CVEye");

                //
                backgroundWorker.CancelAsync();

                //
                stopDetection.Enabled = false;
                startDetection.Enabled = true;
                turnCamera.Enabled = true;
                captureImg.Enabled = true;

                //
                timerDROupdate.Enabled = true;
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Load database
            XElement Parameters = SysData.Element("System").Element("PaintingConditionWindow").Element("Parameters");

            // Get current item painting points
            getItem = dispensingData.Element("Field")
                .Elements("Item")
                .Where(x => x.Element("Name").Value == tmp_item_name.Text)
                .Single();

            // List painting points to list
            List<XElement> point_list = getItem.Element("Points").Elements("Point").ToList();


            // Item sorting
            _centerList = SortByDistance(_centerList, _angleList);

            using (macro01)
            using (macro02)
            {
                #region Painting macro building

                TextWriter lastmacro = macro01;

                for (int pointidx = 0; pointidx < _centerList.Count; pointidx++)
                {
                    // Machine coordinate (G54, G55)
                    //^ Y axis
                    //|
                    //|
                    //L---------> X axis
                    //O

                    // Calculate center coordinate
                    double cen_X = xCompensate(_centerList[pointidx]) + x_pixel_offset;
                    double cen_Y = yCompensate(_centerList[pointidx]) + y_pixel_offset;
                    PointF cenPoint = new PointF((float)cen_X, (float)cen_Y);

                    // Global painting points rotation
                    double dirCom = 0.5;
                    cenPoint = RotateFOV(cenPoint, dirCom);

                    cen_X = Math.Round(cenPoint.X * _accuracy, 3);
                    cen_Y = Math.Round(cenPoint.Y * _accuracy, 3);

                    // Invert y axis direction
                    cen_Y = -cen_Y;

                    // Points set affine transformation
                    Real_PointsWarpAffine(point_list, _centerList[pointidx], angleListSort[pointidx], img_items);

                    // Build running procedure
                    if (pointidx < 80)
                        CompletedPaintingProcess(affine_painting_points, (float)cen_X, (float)cen_Y, Parameters, macro01);
                    else
                    {                        
                        if (pointidx == 80)
                        {
                            macro02On = true;
                            macro02Used = true;
                            macro02.WriteLine("DeactivateSignal(OUTPUT5)");
                            macro02.WriteLine("DeactivateSignal(OUTPUT6)");
                        }
                        CompletedPaintingProcess(affine_painting_points, (float)cen_X, (float)cen_Y, Parameters, macro02);
                        lastmacro = macro02;
                    }

                    // Draw the object tracking line
                    if (enableLine.CheckState == CheckState.Checked)
                    {
                        if (pointidx > 0)
                            // Draw point to point
                            img_items.Draw(new LineSegment2DF(_centerList[pointidx - 1], _centerList[pointidx]), new Bgr(Color.Red), 2);
                        else
                            // Draw the start point
                            img_items.Draw(new CircleF(_centerList[pointidx], 10), new Bgr(Color.Red), 2);
                    }
                }

                #endregion

                #region Complete Painting Process

                // Return to z safe
                lastmacro.WriteLine("Code \"G00 Z" + Parameters.Attribute("zSafe").Value + "\"");
                Wait(50, lastmacro);

                // Disable dispensing valve channel
                lastmacro.WriteLine("DeactivateSignal(OUTPUT" + GetValveNum(item_color.Text) + ")");

                // Disable selection piston
                lastmacro.WriteLine("' Disable level piston");
                lastmacro.WriteLine("DeactivateSignal(OUTPUT6)");

                // Disable frame holding piston
                lastmacro.WriteLine("' Disable clamping piston");
                lastmacro.WriteLine("DeactivateSignal(OUTPUT8)");

                //  Return home
                lastmacro.WriteLine("' Return home");
                lastmacro.WriteLine("Code \"G90 G54 G00 X-90 Y-400\"");
                lastmacro.Close();
                macro01.Close();
                macro02.Close();

                #endregion
            }

            // Save detected items result
            CvInvoke.Imwrite("result/dots.jpg", img_items);
            CvInvoke.Imwrite("result/dots.jpg", img_threshold);

            // View result
            pattern_field.Image = img_items.Bitmap;
            pattern_field.Refresh();
            
            // Update working status
            status_label.Text = "Hoàn tất quét ảnh";
            status_label.Refresh();

            // Detection status
            detected = true;

            // 
            startDetection.Enabled = true;
            turnCamera.Enabled = true;
            captureImg.Enabled = true;
            stopDetection.Enabled = false;

            //
            timerDROupdate.Enabled = true;

            processLog.Items.Add(detectionWatch.ElapsedMilliseconds);
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
            dispensingData = XDocument.Load(data_path);

            getItem = dispensingData.Element("Field")
                .Elements("Item")
                .Where(x => x.Element("Name").Value == tmp_item_name.Text)
                .Single();

            getItem.Element("PixelsPosition").RemoveAll();

            dispensingData.Save(data_path);
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

        public static bool CheckInside(PointF point)
        {
            bool inside = false;

            if ((point.Y <= screen_height) && (point.Y >= 0) && (point.X <= screen_width) && (point.X >= 0))
            {
                inside = true;
            }

            return inside;
        }

        // Compensate x axis value
        public static double xCompensate(PointF point)
        {
            double value = 0;

            if (CheckInside(point))
            {
                XElement dataX;

                // Get left node
                dataX = SysData.Element("System")
                        .Element("CameraCalibrationWindow")
                        .Element("xCompensation");

                //
                dataX = dataX.Elements("Row").
                    Where(j => ((float.Parse(j.Element("rowSegment").Attribute("start").Value) <= point.Y) &&
                    (float.Parse(j.Element("rowSegment").Attribute("end").Value) > point.Y))).
                    Single();

                //
                dataX = dataX.Elements("colSegment").Where(j => ((float.Parse(j.Attribute("start").Value) <= point.X) &&
                (float.Parse(j.Attribute("stop").Value) > point.X))).
                Single();

                // Compute compensated value
                value = point.X - ((point.X - float.Parse(dataX.Attribute("start").Value)) * double.Parse(dataX.Attribute("slope").Value) +
                                 double.Parse(dataX.Attribute("intercept").Value));
                //logStream.WriteLine(value);
            }

            else MessageBox.Show("Out of compensation range", "CVEye");

            return value;
        }

        // Compensate y axis value
        public static double yCompensate(PointF point)
        {
            double value = 0;

            if (CheckInside(point))
            {
                XElement dataX;

                // Get left node
                dataX = SysData.Element("System")
                        .Element("CameraCalibrationWindow")
                        .Element("yCompensation");

                //
                dataX = dataX.Elements("Column").
                    Where(j => ((float.Parse(j.Element("colSegment").Attribute("start").Value) <= point.X) &&
                    (float.Parse(j.Element("colSegment").Attribute("end").Value) > point.X))).
                    Single();

                //
                dataX = dataX.Elements("rowSegment").Where(j => ((float.Parse(j.Attribute("start").Value) <= point.Y) &&
                (float.Parse(j.Attribute("stop").Value) > point.Y))).
                Single();

                // Compute compensated value
                value = point.Y - ((point.Y - float.Parse(dataX.Attribute("start").Value)) * double.Parse(dataX.Attribute("slope").Value) +
                                 double.Parse(dataX.Attribute("intercept").Value));
                //logStream.WriteLine(value);
            }

            else MessageBox.Show("Out of compensation range", "CVEye");

            return value;
        }

        public static void PixelsCompensation(Image<Bgr, byte> sample)
        {
            SysData = XDocument.Load(systemPath);
            XElement section;
            SysData.Element("System").Element("CameraCalibrationWindow").RemoveAll();
            SysData.Element("System").Element("CameraCalibrationWindow").Add(new XElement("xCompensation"), new XElement("yCompensation"));


            VectorOfPointF corner_set = new VectorOfPointF();
            Mat sample_frame = new Mat();
            
            //
            Size sample_size = new Size(pt_width, pt_height);

            int cornerPosOri = pt_width * pt_height / 2 - pt_width / 2;

            // Convert BGR to GRAY image
            CvInvoke.CvtColor(sample, sample_frame, ColorConversion.Bgr2Gray);

            // Find corners on chess board image
            if (CvInvoke.FindChessboardCorners(sample_frame, sample_size, corner_set))
            {
                //
                CvInvoke.CornerSubPix(sample_frame, corner_set, new Size(11, 11), new Size(-1, -1), new MCvTermCriteria(30, 0.05));

                // Initialize some variables
                PointF[] com_point = new PointF[2]; // compensation point
                double slope, intercept;


                float squareSize = ((corner_set[cornerPosOri + 1].X - corner_set[cornerPosOri].X) +
                    (corner_set[cornerPosOri + 1 - 48].X - corner_set[cornerPosOri - 48].X) +
                    (corner_set[cornerPosOri + 1 - 96].X - corner_set[cornerPosOri - 96].X)) / 3;
                _accuracy = 10 / squareSize;

                SysData.Element("System").Element("CameraCalibrationWindow").Add(new XElement("Accuracy", new XAttribute("Value", _accuracy)));

                // New variable
                PointF[] rowComPoint = new PointF[2];
                PointF[] colComPoint = new PointF[2];

                using (StreamWriter logStream = new StreamWriter("history.txt"))
                {

                    //
                    section = SysData.Element("System").Element("CameraCalibrationWindow").Element("yCompensation");

                    // Center of corner array
                    
                    float xsegEnd = 0;

                    // Y axis compensation data
                    for (int col = 0; col < pt_width; col++) // column scan                
                    {
                        // Add new column segment
                        section.Add(new XElement("Column", new XElement("Num", col)));

                        byte rowSegNum = pt_height / 2 - 1;
                        byte colSegNum = pt_width / 2;

                        // Segment value
                        float xsegStart = (col == 0) ? 0 : xsegEnd;
                        xsegEnd = (col == pt_width - 1) ? 2592 : corner_set[cornerPosOri].X + squareSize * col - squareSize * colSegNum;

                        // Create segments length
                        section.Elements("Column").Where(j => j.Element("Num").Value == col.ToString()).Single().Add(new XElement("colSegment",
                            new XAttribute("start", xsegStart),
                            new XAttribute("end", xsegEnd)));

                        //
                        sample.Draw(new LineSegment2DF(new PointF(corner_set[cornerPosOri].X + squareSize * col - squareSize * colSegNum, 0),
                            new PointF(corner_set[cornerPosOri].X + squareSize * col - squareSize * colSegNum, 1944)), new Bgr(Color.Blue), 1);

                        byte rowIndex = 0;

                        for (int row = 0; row < pt_height; row++) // row scan
                        {
                            // Corner index of array
                            int cornerPos = pt_width * row + col;

                            // Calculate stretch error
                            rowComPoint[rowIndex].X = corner_set[cornerPosOri].Y + squareSize * row - squareSize * rowSegNum; // ref
                            float err = corner_set[cornerPos].Y - rowComPoint[rowIndex].X;
                            rowComPoint[rowIndex].Y = err;

                            if (col == 1)
                                sample.Draw(new LineSegment2DF(new PointF(0, rowComPoint[rowIndex].X),
                                    new PointF(2592, rowComPoint[rowIndex].X)),
                                    new Bgr(Color.Blue), 1);

                            // Error law
                            if (rowIndex > 0)
                            {
                                // a = (y0 - y1)/(x0 - x1)
                                slope = (rowComPoint[rowIndex - 1].Y - rowComPoint[rowIndex].Y) / (0 - squareSize);

                                // b = (x0y1 - x1y0)/(x0 - x1)
                                intercept = (0 * rowComPoint[rowIndex].Y - squareSize * rowComPoint[rowIndex - 1].Y) / (0 - squareSize);

                                slope = Math.Round(slope, 4);
                                intercept = Math.Round(intercept, 4);

                                // Test
                                //logStream.WriteLine(((row == 1) ? 0 : rowComPoint[point_index - 1].X) + " " + slope + "   " + intercept + "    " + ((row == pt_height - 1) ? 1944 : rowComPoint[point_index].X));                          

                                section.Elements("Column").Where(j => j.Element("Num").Value == col.ToString()).Single().
                                    Add(new XElement("rowSegment",
                                    new XAttribute("start", (row == 1) ? 0 : rowComPoint[rowIndex - 1].X),
                                    new XAttribute("slope", slope),
                                    new XAttribute("intercept", intercept),
                                    new XAttribute("stop", (row == pt_height - 1) ? 1944 : rowComPoint[rowIndex].X)));

                                // Update previous value
                                rowComPoint[rowIndex - 1] = rowComPoint[rowIndex];
                                rowIndex = 0;
                            }

                            // Draw something
                            sample.Draw(new Cross2DF(corner_set[cornerPos], 10, 10), new Bgr(Color.Red), 2);
                            //sample.Draw(cornerPos.ToString(),
                            //    new Point((int)corner_set[cornerPos].X - 10, (int)corner_set[cornerPos].Y - 10),
                            //    FontFace.HersheyPlain,
                            //    1.2,
                            //    new Bgr(Color.GreenYellow),
                            //    1);
                            rowIndex++;
                        }
                        // Reset var
                        rowComPoint = new PointF[2];
                    }

                    //
                    section = SysData.Element("System").Element("CameraCalibrationWindow").Element("xCompensation");

                    float ysegEnd = 0;

                    // X axis compensation data
                    for (int row = 0; row < pt_height; row++)
                    {
                        // Add new column segment
                        section.Add(new XElement("Row", new XElement("Num", row)));

                        byte rowSegNum = pt_height / 2 - 1;
                        byte colSegNum = pt_width / 2;

                        float ysegStart = (row == 0) ? 0 : ysegEnd;
                        ysegEnd = (row == pt_height - 1) ? 1944 : corner_set[cornerPosOri].Y + squareSize * row - squareSize * rowSegNum;

                        // Create segments length
                        section.Elements("Row").Where(j => j.Element("Num").Value == row.ToString()).Single().Add(new XElement("rowSegment",
                            new XAttribute("start", ysegStart),
                            new XAttribute("end", ysegEnd)));


                        byte colIndex = 0;
                        for (int col = 0; col < pt_width; col++)
                        {
                            // Corner index of array
                            int cornerPos = pt_width * row + col;

                            // Calculate stretch error
                            colComPoint[colIndex].X = corner_set[cornerPosOri].X + squareSize * col - squareSize * colSegNum; // ref
                            float err = corner_set[cornerPos].X - colComPoint[colIndex].X;
                            colComPoint[colIndex].Y = err;

                            // display error
                            //logStream.WriteLine(err);

                            // Error law
                            if (colIndex > 0)
                            {
                                // a = (y0 - y1)/(x0 - x1)
                                slope = (colComPoint[colIndex - 1].Y - colComPoint[colIndex].Y) / (0 - squareSize);

                                // b = (x0y1 - x1y0)/(x0 - x1)
                                intercept = (0 * colComPoint[colIndex].Y - squareSize * colComPoint[colIndex - 1].Y) / (0 - squareSize);

                                slope = Math.Round(slope, 4);
                                intercept = Math.Round(intercept, 4);

                                section.Elements("Row").Where(j => j.Element("Num").Value == row.ToString()).Single().
                                    Add(new XElement("colSegment",
                                    new XAttribute("start", (col == 1) ? 0 : colComPoint[colIndex - 1].X),
                                    new XAttribute("slope", slope),
                                    new XAttribute("intercept", intercept),
                                    new XAttribute("stop", (col == pt_width - 1) ? 2592 : colComPoint[colIndex].X)));


                                // Update previous value
                                colComPoint[colIndex - 1] = colComPoint[colIndex];
                                colIndex = 0;
                            }

                            colIndex++;
                        }

                        // Reset value
                        colComPoint = new PointF[2];
                    }

                    SysData.Save(systemPath);
                    SysData = XDocument.Load(systemPath);

                    using (macro01 = new StreamWriter(Path.Combine(macroDirectory, @"M999.m1s")))
                    {

                        for (int cornerPos = cornerPosOri; cornerPos < cornerPosOri + 1; cornerPos++)
                        //for (int row = 0; row < pt_height; row++)
                        {
                            //for (int col = 0; col < pt_width; col++)
                            {
                                //int cornerPos = pt_width * row + col;
                                double xCom = xCompensate(corner_set[cornerPos]);
                                double yCom = yCompensate(corner_set[cornerPos]);
                                sample.Draw(new CircleF(new PointF((float)xCom, (float)yCom), 10), new Bgr(Color.GreenYellow), 3);

                                // For G54 calibration

                                // Calculate center coordinate
                                double cen_X = xCompensate(corner_set[cornerPos]) + x_pixel_offset;
                                double cen_Y = yCompensate(corner_set[cornerPos]) + y_pixel_offset;

                                PointF cenPoint = new PointF((float)cen_X, (float)cen_Y);

                                // Global painting points rotation
                                cenPoint = RotateFOV(cenPoint, 0.5);

                                cen_X = Math.Round(cenPoint.X * _accuracy, 3);
                                cen_Y = Math.Round(cenPoint.Y * _accuracy, 3);

                                // Invert y axis direction
                                cen_Y = -cen_Y;

                                macro01.WriteLine("Code \"G90 G54 G01 X" + cen_X + " Y" + cen_Y + " F5000\"");
                            }
                        }

                        macro01.Close();
                    }
                }

                MessageBox.Show("Compensation Completed."
                    + Environment.NewLine + "Number of rows:    " + pt_height.ToString()
                    + Environment.NewLine + "Number of cols:    " + pt_width.ToString()
                    + Environment.NewLine + "Acc.:   " + _accuracy.ToString(), "CVEye");
            }
            else
                MessageBox.Show("Cannot Find Corners.", "CVEye");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Points"> The vector of global painting points </param>
        /// <param name="cenX"> X object center </param>
        /// <param name="cenY"> Y object center</param>
        /// <param name="Parameters"> Painting conditions data </param>
        private void CompletedPaintingProcess(PointF[] Points, float cenX, float cenY, XElement Parameters, TextWriter macro)
        {
            double X, Y; //mm
            float startpoint_X = cenX - in_circle_radius; //mm
            float startpoint_Y = cenY; //mm

            // Get painting points from XML
            getItem = dispensingData.Element("Field")
                            .Elements("Item")
                            .Where(x => x.Element("Name").Value == tmp_item_name.Text)
                            .Single();

            List<XElement> point_list = getItem.Element("Points").Elements("Point").ToList();

            int j = 0;

            foreach (XElement element in point_list)
            {
                // Convert pixel to mm
                X =  Math.Round((Points[j].X + x_pixel_offset) * _accuracy, 3); //
                Y = -Math.Round((Points[j].Y + y_pixel_offset) * _accuracy, 3); //

                //--------------------- Build macro01-------------------

                // with the first point of each item
                if (j == 0)
                {
                    // with the first point of the first item
                    if (first_item)
                    {
                        // macro01 initialization
                        macro.WriteLine("' Initialization");
                        macro.WriteLine("Code \"G90 " + GetValveHome(item_color.Text) + " G00" + "\"");

                        // Select valve position
                        macro.WriteLine("' Select valve");
                        macro.WriteLine("ActivateSignal(OUTPUT" + GetValveNum(item_color.Text) + ")");

                        // Move to the first point of inner circle
                        macro.WriteLine("' Circle starting point");
                        macro.WriteLine("Code \"G00 X" + startpoint_X + " Y" + startpoint_Y + "\"");
                        Wait(2000, macro);

                        // Turn on 15mm piston
                        macro.WriteLine("' Turning on level piston");
                        macro.WriteLine("ActivateSignal(OUTPUT6)");

                        // Select painting mode 03
                        JettingModeSelectingMacro(3, macro);

                        // Dispense paint
                        macro.WriteLine("' Starting dispensing");
                        Paint_Drip(decimal.Parse(Parameters.Attribute("offset").Value), macro);

                        // Circular interpolation
                        macro.WriteLine("' Circular interpolation");
                        CircleDivider(new PointF(cenX, cenY), in_circle_radius, 4, int.Parse(Parameters.Attribute("circleSpeed").Value), macro);
                        Wait(50, macro);

                        // Select painting mode 01
                        JettingModeSelectingMacro(1, macro);

                        // Go to z return
                        macro.WriteLine("' Moving to z return");
                        macro01.WriteLine("Code \"G00 Z" + Parameters.Attribute("zReturn").Value + "\"");

                        // Move to the first point of inner circle
                        macro.WriteLine("' Moving to painting point of pattern");
                        macro.WriteLine("Code \"G01 X" + X + " Y" + Y + " F" + Parameters.Attribute("xySpeed").Value + "\"");
                        Wait(50, macro);
                    }
                    else
                    {
                        // Move to the first point of inner circle
                        macro.WriteLine("' Circle starting point");
                        macro.WriteLine("Code \"G00 X" + startpoint_X + " Y" + startpoint_Y + "\"");
                        Wait(500, macro);

                        if (macro02On)
                        {
                            macro.WriteLine("ActivateSignal(OUTPUT6)");
                            macro.WriteLine("ActivateSignal(OUTPUT5)");
                            macro02On = false;
                        }

                        // Select painting mode 03
                        JettingModeSelectingMacro(3, macro);

                        // Dispense paint
                        macro.WriteLine("' Starting dispensing");
                        Paint_Drip(decimal.Parse(Parameters.Attribute("offset").Value), macro);

                        // Circular interpolation
                        macro.WriteLine("' Circular interpolation");
                        CircleDivider(new PointF(cenX, cenY), in_circle_radius, 4, int.Parse(Parameters.Attribute("circleSpeed").Value), macro);
                        Wait(50, macro);

                        // Select painting mode 01
                        JettingModeSelectingMacro(1, macro);

                        // Go to z return
                        macro.WriteLine("' Moving to z return");
                        macro.WriteLine("Code \"G00 Z" + Parameters.Attribute("zReturn").Value + "\"");

                        // First point of non-first item
                        macro.WriteLine("' Moving to painting point of pattern");
                        macro.WriteLine("Code \"G01 X" + X + " Y" + Y + " F" + Parameters.Attribute("xySpeed").Value + "\"");
                        Wait(50, macro);
                    }

                    //firstpointofitem = true;
                }
                else
                {
                    // Exception of "Ngựa 02"
                    if (tmp_item_name.Text == "Ngựa 02" && j == 15)
                        JettingModeSelectingMacro(2, macro);

                    // Go to painting point of template
                    macro.WriteLine("' Moving to painting point of pattern");
                    macro.WriteLine("Code \"G01 X" + X + " Y" + Y + " F" + Parameters.Attribute("xySpeed").Value + "\"");
                    //Wait(50);

                    //firstpointofitem = false;
                }

                // Dispense paint
                Paint_Drip(0, macro);

                // Rapid moving to Z return
                macro.WriteLine("' Moving to z return");
                macro.WriteLine("Code \"Z" + Parameters.Attribute("zReturn").Value + "\"");

                // Disable the first item flag
                first_item = false;

                j++;
            }
        }

        private void CircleDivider(PointF center, float radius, byte factor, int speed, TextWriter macro)
        {
            // Start point of the circle
            PointF startPoint = new PointF(center.X - radius, center.Y);

            // G02
            for (byte i = 0; i < factor; i++)
            {
                PointF endPoint = new PointF(); // End point of an arc
                endPoint.X = (float)(center.X - radius + radius * (1 - Math.Cos((i + 1) * (360 / factor) * Math.PI / 180)));
                endPoint.Y = (float)(center.Y - radius * Math.Sin((i + 1) * (360 / factor) * Math.PI / 180));

                // Command macro01                
                macro.WriteLine("Code \"G03 X" + endPoint.X + " Y" + endPoint.Y + " I" + (center.X - startPoint.X) + " J" + (center.Y - startPoint.Y) + ((i == 0) ? " F" + speed.ToString() : "") + "\"");

                // Update start point of the arc
                startPoint.X = endPoint.X;
                startPoint.Y = endPoint.Y;
            }
        }

        private void Paint_Drip(decimal offset, TextWriter macro)
        {
            XElement Parameters = SysData.Element("System").Element("PaintingConditionWindow").Element("Parameters");

            // Moving to Z injecting deep with specific speed
            if (!first_item)
                macro.WriteLine("Code \"G00 Z" + (decimal.Parse(Parameters.Attribute("zDrip").Value) + offset) + "\"");
            else
                macro.WriteLine("Code \"Z" + (decimal.Parse(Parameters.Attribute("zDrip").Value) + offset) + "\"");


            // Waiting for moving completed
            //Wait(firstpointofitem ? 100 : 50);
            Wait(50, macro);

            // Drip paint
            macro.WriteLine("ActivateSignal(OUTPUT7" + ")");
            macro.WriteLine("Sleep(100)");
            macro.WriteLine("DeactivateSignal(OUTPUT7" + ")");

        }

        private void JettingModeSelectingMacro(byte mode, TextWriter macro)
        {
            switch (mode)
            {
                case 1: // 0 0
                    macro.WriteLine("' Painting mode 01");
                    macro.WriteLine("DeactivateSignal(OUTPUT1" + ")");
                    macro.WriteLine("DeactivateSignal(OUTPUT2" + ")");
                    break;
                case 2: // 0 1
                    macro.WriteLine("' Painting mode 02");
                    macro.WriteLine("DeactivateSignal(OUTPUT1" + ")");
                    macro.WriteLine("ActivateSignal(OUTPUT2" + ")");
                    break;
                case 3: // 1 0
                    macro.WriteLine("' Painting mode 03");
                    macro.WriteLine("ActivateSignal(OUTPUT1" + ")");
                    macro.WriteLine("DeactivateSignal(OUTPUT2" + ")");
                    break;
                case 4: // 1 1
                    macro.WriteLine("' Painting mode 04");
                    macro.WriteLine("ActivateSignal(OUTPUT1" + ")");
                    macro.WriteLine("ActivateSignal(OUTPUT2" + ")");
                    break;
            }
        }

        private void JettingModeSelecting(byte mode)
        {
            switch (mode)
            {
                case 1: // 0 0
                    scriptObject.DeActivateSignal(7);
                    scriptObject.DeActivateSignal(8);
                    break;
                case 2: // 0 1
                    scriptObject.DeActivateSignal(7);
                    scriptObject.ActivateSignal(8);
                    break;
                case 3: // 1 0
                    scriptObject.ActivateSignal(7);
                    scriptObject.DeActivateSignal(8);
                    break;
                case 4: // 1 1
                    scriptObject.ActivateSignal(7);
                    scriptObject.ActivateSignal(8);
                    break;
            }
        }

        private void Wait(int time, TextWriter macro)
        {
            // Wait for moving completed
            macro.WriteLine("While (IsMoving())");
            macro.WriteLine("Sleep(" + time.ToString() + ")");
            macro.WriteLine("Wend");
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
            ValveOneClick.Enabled = status;
            ReplacePosition.Enabled = status;
            TurnPiston.Enabled = status;
        }
        
            #region Click events

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

        private void DetectClick(object sender, EventArgs e)
        {
            try
            {
                // Disable timer
                timerDROupdate.Enabled = false;

                //
                stopDetection.Enabled = true;
                startDetection.Enabled = false;
                turnCamera.Enabled = false;
                captureImg.Enabled = false;

                // reset first item flag
                first_item = true;

                // View captured image
                pattern_field.Image = img_capture_undist.Bitmap;
                pattern_field.Refresh();

                // Clear listbox
                processLog.Items.Clear();

                // Init new mach3 macro01
                if (macro01 != null)
                    macro01.Close();
                macro01 = new StreamWriter(Path.Combine(macroDirectory, @"M999.m1s"));

                // Init new mach3 macro01
                if (macro02 != null)
                    macro02.Close();
                macro02 = new StreamWriter(Path.Combine(macroDirectory, @"M998.m1s"));

                // Reload XML
                SysData = XDocument.Load(systemPath);

                // Update working status
                status_label.Text = "Đang quét ảnh...";
                pattern_field.Refresh(); // to remove the old text on camera window
                status_label.Refresh();

                // Start detecting
                DetectClone();
                //Detect_Pattern();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void stopDetection_Click(object sender, EventArgs e)
        {
            //
            backgroundWorker.CancelAsync();

            //
            stopDetection.Enabled = false;
            startDetection.Enabled = true;
            turnCamera.Enabled = true;
            captureImg.Enabled = true;

            //
            timerDROupdate.Enabled = true;
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
                if (lockCylinder.Text != "Khóa khay")
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
                else
                    MessageBox.Show("Bạn chưa khóa khay.", "CVEye");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
                else MessageBox.Show("Vui lòng thiết lập gốc máy.", "CVEye");
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

                        scriptObject.Code("M666");
                    }
                }
                else MessageBox.Show("Vui lòng thiết lập gốc máy.", "CVEye");
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
                    if (machineZero)
                    {
                        GetMach3Instance();

                        if (scriptObject != null)
                        {
                            // Update working status
                            painting = true;
                            detected = false;

                            // Disable some button
                            EnableButton(false);

                            // Run painting macro
                            scriptObject.Code("M999");

                            status_label.Text = "Đang sơn...";
                            status_label.Refresh();
                        }
                    }
                    else MessageBox.Show("Vui lòng thiết lập gốc máy.", "CVEye");
                }
                else MessageBox.Show("Chưa quét ảnh.", "CVEye");
            }
            else return;
        }

        private void lockCylinder_Click(object sender, EventArgs e)
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

        private void ValveOneClick_Click(object sender, EventArgs e)
        {
            GetMach3Instance();

            if (scriptObject != null)
            {
                // Disale all channels
                scriptObject.DeActivateSignal(10);
                scriptObject.DeActivateSignal(11);

                // Select jetting mode
                JettingModeSelecting(1); // 30 ms valve-on time

                // Select channels
                short channel = (short)(GetValveNum(item_color.Text) + 6);
                scriptObject.ActivateSignal(channel);
                scriptObject.Code("M92");
            }
        }

        private void ValveSwitching_Click(object sender, EventArgs e)
        {
            GetMach3Instance();

            if (scriptObject != null)
            {
                // Disable all channels
                scriptObject.DeActivateSignal(10);
                scriptObject.DeActivateSignal(11);

                // Select jetting mode
                JettingModeSelecting(4); // switch on-demand

                ValveSwitching.Text = (ValveSwitching.Text == "Xả liên tục") ? "Dừng xả" : "Xả liên tục";

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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void paintingPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigPaintingPoints Con_Painting_Point = new ConfigPaintingPoints();
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
            #endregion

            #region Other events

        private void Name_Changed(object sender, EventArgs e)
        {
            // Preview current template
            ViewTemplate(tmp_item_name.Text);
            CheckTmpSize();

            // View template image
            templateField.Image = tmpBgr.Bitmap;
            templateField.Refresh();
        }

        private void ColorChanged(object sender, EventArgs e)
        {
            valveNum.Text = (GetValveNum(item_color.Text) - 3).ToString();
        }

        private void CVEye_Shown(object sender, EventArgs e)
        {
            GetMach3Instance();

            //Start Mach3
            if (mach3 == null)
            {
                Init_Mach3();
                machStart = true;
            }
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

        public struct CCStatsOp
        {
            public Rectangle Rectangle;
            public int Area;
        }

        private void buildTemplate1()
        {
            using (OpenFileDialog _src = new OpenFileDialog())
            {
                if (_src.ShowDialog() == DialogResult.OK)
                {
                    using (Image<Gray, byte> srcTmp = new Image<Gray, byte>(_src.FileName))
                    //using (Mat srcTmp = CvInvoke.Imread(_src.FileName, ImreadModes.Grayscale))
                    {
                        // threshold
                        Mat labels = new Mat();
                        Mat stats = new Mat();
                        Mat centroids = new Mat();

                        CvInvoke.Threshold(srcTmp, srcTmp, 200, 255, ThresholdType.Binary);
                        CvInvoke.BitwiseNot(srcTmp, srcTmp);
                        int result = CvInvoke.ConnectedComponentsWithStats(srcTmp, labels, stats, centroids, LineType.FourConnected);
                        processLog.Items.Add(result);

                        var centroidPoints = new MCvPoint2D64f[result];
                        centroids.CopyTo(centroidPoints);

                        CCStatsOp[] statsOp = new CCStatsOp[stats.Rows];
                        stats.CopyTo(statsOp);

                        //foreach (var point in centroidPoints)
                        int i = 0;
                        foreach (var statop in statsOp)
                        {
                            if (statop.Area > 1000)
                            {
                                processLog.Items.Add(statop.Rectangle.Size);
                                CvInvoke.cvSetImageROI(srcTmp.Ptr, statop.Rectangle);
                                srcTmp.ROI = CvInvoke.cvGetImageROI(srcTmp.Ptr);
                                CvInvoke.Imwrite("data/" + statop.Area.ToString() + ".jpg", srcTmp);
                                CvInvoke.cvResetImageROI(srcTmp.Ptr);
                                i++;
                            }
                        }


                        //using (SaveFileDialog _dst = new SaveFileDialog())
                        //{
                        //    if (_dst.ShowDialog() == DialogResult.OK)
                        //    {
                        //        //CvInvoke.Imwrite(_dst.FileName + ".jpg", labels);

                        //        CvInvoke.Imwrite(_dst.FileName + "labels.jpg", labels);
                        //        //CvInvoke.Imwrite(_dst.FileName + "stats.jpg", stats);
                        //        //CvInvoke.Imwrite(_dst.FileName + "centroids.jpg", centroids);
                        //    }
                        //}
                    }
                }
            }
        }

        private void buildTemplate2()
        {
            try
            {
                Mat imgGray = Frame_Analyze();

                // Circle Hough Transform
                CircleF[] img_circles = CvInvoke.HoughCircles(
                    imgGray, // Array of circles data
                    HoughType.Gradient,
                    1.7,
                    100,
                    60,
                    45,
                    350,
                    450);

                Rectangle roi_rec = new Rectangle(new Point((int)img_circles[0].Center.X - 266, (int)img_circles[0].Center.Y - 266), new Size(532, 532));
                CvInvoke.cvSetImageROI(img_capture_undist.Ptr, roi_rec);

                img_capture_undist.ROI = CvInvoke.cvGetImageROI(img_capture_undist.Ptr);
                CvInvoke.Resize(img_capture_undist, img_capture_undist, new Size(430, 430));

                CvInvoke.Imwrite("result/tmp" + DateTime.Now.ToFileTime() + ".jpg", img_capture_undist);
                CvInvoke.cvResetImageROI(img_capture_undist.Ptr);
                img_capture_undist = img_threshold.ToImage<Bgr, byte>();
                CvInvoke.cvSetImageROI(img_capture_undist.Ptr, roi_rec);
                img_capture_undist.ROI = CvInvoke.cvGetImageROI(img_capture_undist.Ptr);
                CvInvoke.Resize(img_capture_undist, img_capture_undist, new Size(ROIsize, ROIsize));

                CvInvoke.Imwrite("result/bintmp" + DateTime.Now.ToFileTime() + ".jpg", img_capture_undist);
                CvInvoke.cvResetImageROI(img_capture_undist.Ptr);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void ConvertBgr28Bit()
        {
            using (OpenFileDialog _src = new OpenFileDialog())
            {
                if (_src.ShowDialog() == DialogResult.OK)
                {
                    // Input BGR image an convert to GRAY
                    using (Image<Gray, byte> srcTmp = new Image<Gray, byte>(_src.FileName))
                    {

                        //CvInvoke.Threshold(srcTmp, srcTmp, 200, 255, ThresholdType.Binary);
                        CvInvoke.AdaptiveThreshold(srcTmp,
                            srcTmp,
                            255,
                            AdaptiveThresholdType.GaussianC,
                            ThresholdType.Binary,
                            95,
                            1.5);

                        using (SaveFileDialog _dst = new SaveFileDialog())
                        {
                            if (_dst.ShowDialog() == DialogResult.OK)
                            {
                                CvInvoke.Imwrite(_dst.FileName + "8bit.jpg", srcTmp);
                            }
                        }

                    }
                }
            }
        }

        private void convertto8bit_Click(object sender, EventArgs e)
        {
            ConvertBgr28Bit();
        }

        private void TimerDROupdate_Tick(object sender, EventArgs e)
        {
            UpdateDRO();
        }

        private void polarTransform()
        {
            using (StreamWriter logStream = new StreamWriter("history.txt", true))
            {
                using (OpenFileDialog _src = new OpenFileDialog())
                {
                    if (_src.ShowDialog() == DialogResult.OK)
                    {
                        using (Image<Gray, byte> src = new Image<Gray, byte>(_src.FileName))
                        {

                            for (int alpha = 0; alpha < 360; alpha = alpha + 1)
                            {
                                int value = 0;
                                for (int r = 0; r < 52; r++)
                                {
                                    double alpha_rad = Math.PI * alpha / 180;
                                    double x = r * Math.Cos(alpha_rad) + src.Rows / 2;
                                    double y = r * Math.Sin(alpha_rad) + src.Cols / 2;
                                    if (src.Data[(int)x, (int)y, 0] == 0)
                                        value++;
                                }
                                logStream.WriteLine(value);
                                value = 0;

                            }
                        }
                    }
                }
            }
        }

        // Check the template size and ROI size to resize the template
        private void CheckTmpSize()
        {
            if (tmp_raw.Width > ROIsize)
            {
                // Resize template image to match the ROI size
                CvInvoke.Resize(tmp_raw, tmp_raw, new Size(ROIsize, ROIsize));
            }
        }

        // Reload template
        private void ViewTemplate(string value)
        {
            switch (value)
            {
                case "Tướng 01":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/01/cc11.jpg");
                    tmpBgr = new Image<Bgr, byte>("pattern_data/01/cc11.jpg");
                    break;
                case "Sĩ 01":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/01/cc12.jpg");
                    tmpBgr = new Image<Bgr, byte>("pattern_data/01/cc12.jpg");
                    break;
                case "Tượng 01":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/01/cc13.jpg");
                    tmpBgr = new Image<Bgr, byte>("pattern_data/01/cc13.jpg");
                    break;
                case "Xe 01":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/01/cc14.jpg");
                    tmpBgr = new Image<Bgr, byte>("pattern_data/01/cc14.jpg");
                    break;
                case "Pháo 01":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/01/cc15.jpg");
                    tmpBgr = new Image<Bgr, byte>("pattern_data/01/cc15.jpg");
                    break;
                case "Ngựa 01":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/01/cc16.jpg");
                    tmpBgr = new Image<Bgr, byte>("pattern_data/01/cc16.jpg");
                    break;
                case "Chốt 01":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/01/cc17.jpg");
                    tmpBgr = new Image<Bgr, byte>("pattern_data/01/cc17.jpg");
                    break;
                case "Tướng 02":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/02/cc21.jpg");
                    tmpBgr = new Image<Bgr, byte>("pattern_data/02/cc21.jpg");
                    break;
                case "Sĩ 02":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/02/cc22.jpg");
                    tmpBgr = new Image<Bgr, byte>("pattern_data/02/cc22.jpg");
                    break;
                case "Tượng 02":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/02/cc23.jpg");
                    tmpBgr = new Image<Bgr, byte>("pattern_data/02/cc23.jpg");
                    break;
                case "Xe 02":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/02/cc24.jpg");
                    tmpBgr = new Image<Bgr, byte>("pattern_data/02/cc24.jpg");
                    break;
                case "Pháo 02":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/02/cc25.jpg");
                    tmpBgr = new Image<Bgr, byte>("pattern_data/02/cc25.jpg");
                    break;
                case "Ngựa 02":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/02/cc26.jpg");
                    tmpBgr = new Image<Bgr, byte>("pattern_data/02/cc26.jpg");
                    break;
                case "Chốt 02":
                    tmp_raw = CvInvoke.Imread("pt_data_8bit/02/cc27.jpg");
                    tmpBgr = new Image<Bgr, byte>("pattern_data/02/cc27.jpg");
                    break;
            }
        }

        #endregion
    }
}
