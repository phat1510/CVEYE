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

        // Macro name and function:
        // M90: set machine home
        // M91: set speed mode
        // M92: valve testing
        // M93: go to working position
        // M999: general purpose

        private IMach4 mach3;
        private IMyScriptObject scriptObject;
        ushort MacroLineNumber = 0;
        byte LastItem = 0;
        bool CombineFlag = true;

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
        private byte pt_width;
        private byte pt_height;
        private float square_size;
        private VectorOfPointF corners = new VectorOfPointF();
        private static Mat[] frame_array_buffer;
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
        ushort RoiSize;
        private double RawTmpSize = 430;
        private double PaintingPointScale = 0;
        const double Chess31_DPR = 1.4; // diameter per roi size
        public static double _accuracy;

        private double _dp;
        private double _minDist;
        private double _param01;
        private double _param02;
        private int _minRadius;
        private int _maxRadius;

        private VectorOfPointF corner_set = new VectorOfPointF();
        #endregion

        #region IO

        private string mainDirectory;
        private string mach3Directory;
        private static string macroDirectory;
        public static XDocument SysData;
        private XDocument DispensingData;
        private static TextWriter macroData;
        private XElement getItem;

        // Set data file path
        private string DataPath;
        private string Chess_31_DataPath = "_chess31database.xml";
        private string Chess_29_DataPath = "_chess29database.xml";
        //... for further data paths

        // System file path
        private static string systemPath = "_system.xml";

        #endregion

        #region Miscellaneous

        // Object parameters
        float InnerCircleChessRadius;// = 12.0f;
        float OuterCircleChessRadius;
        float ObjectHeight;
        float SheetThickness;
        //float DrippingHeight;

        // 
        private double xdelta;
        private double ydelta;
        private double _xoffset;
        private double _yoffset;

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
        private bool machStart = false;
        private bool cameraOriginOffset = false;
        double cameraTempOriX = 0;
        double cameraTempOriY = 0;

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

            LoadSetData();

            Init_XML();

            Init_Directory();

            Init_UI();

            InitBackgroundWorker();
        }

        // Set up the BackgroundWorker object by attaching event handlers
        private void InitBackgroundWorker()
        {
            backgroundWorker.DoWork +=
                new DoWorkEventHandler(backgroundWorker_DoWork);

            backgroundWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);

            backgroundWorker.ProgressChanged +=
                new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
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

        private void CheckOutputStatus()
        {

            OutputLed1.BackColor = scriptObject.IsOutputActive(7) ? Color.Lime : Color.LightGray;
            OutputLed2.BackColor = scriptObject.IsOutputActive(8) ? Color.Lime : Color.LightGray;
            OutputLed3.BackColor = scriptObject.IsOutputActive(9) ? Color.Lime : Color.LightGray;
            OutputLed4.BackColor = scriptObject.IsOutputActive(10) ? Color.Lime : Color.LightGray;
            OutputLed5.BackColor = scriptObject.IsOutputActive(11) ? Color.Lime : Color.LightGray;
            OutputLed6.BackColor = scriptObject.IsOutputActive(12) ? Color.Lime : Color.LightGray;
            OutputLed7.BackColor = scriptObject.IsOutputActive(13) ? Color.Lime : Color.LightGray;
            OutputLed8.BackColor = scriptObject.IsOutputActive(14) ? Color.Lime : Color.LightGray;

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

                        //
                        if (scriptObject.IsActive(25) != 0) // press E-stop
                        {
                            //
                            active = true;
                            resetBlinking = true;
                            lockCylinder.Text = "Khóa khay";

                            //
                            CombineFlag = true;

                            //
                            EnableButton(true);

                            //
                            processLog.Items.Clear();
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

                        // Check setting machine zero completed
                        if (!machineZero)
                            CheckMachineZero();

                        // Check painting completed
                        if (painting)
                            CheckPaintingCompleted(zDRO.Text);

                        // Checking for macro combination
                        if (!CombineFlag)
                            MacroCombination(LastItem, zDRO.Text);

                        // Toggle RESET button
                        ResetToggling();

                        // Camera origin offset calibration
                        if (cameraOriginOffset)
                        {
                            xdelta = double.Parse(xDRO.Text) - cameraTempOriX;
                            ydelta = double.Parse(yDRO.Text) - cameraTempOriY;

                            deltaX.Text = xdelta.ToString();
                            deltaY.Text = ydelta.ToString();
                        }

                        CheckOutputStatus();
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

            if ((zSafe == 30.0) && (!scriptObject.IsOutputActive((short)(GetValveNum(item_color.Text) + 6))))
            {
                //status_label.Text = "Painting Completed"; 
                StatusLabel.Text = "Hoàn Tất Phun Sơn";

                lockCylinder.Text = "Khóa khay";

                EnableButton(true);

                processLog.Items.Clear();

                // disable painting flag
                painting = false;
            }
        }

        /// <summary>
        /// x, y ~20000mm/min or 2000rpm
        /// z ~12000mm/min or 2400rpm
        /// </summary>
        public void HighSpeedMode()
        {
            // Build Set speed macro
            using (macroData = new StreamWriter(Path.Combine(macroDirectory, @"M91.m1s")))
            {
                macroData.WriteLine("SetParam(\"VelocitiesX\", 340)");
                macroData.WriteLine("SetParam(\"VelocitiesY\", 340)");
                macroData.WriteLine("SetParam(\"VelocitiesZ\", 200)");

                macroData.Close();
            }
            scriptObject.Code("M91");
            lowSpeed = false;
        }

        /// <summary>
        /// x,y ~ 1000mm/min
        /// z ~ 500 mm/min
        /// </summary>
        private void LowSpeedMode()
        {
            // Build Set speed macro
            using (macroData = new StreamWriter(Path.Combine(macroDirectory, @"M91.m1s")))
            {
                macroData.WriteLine("SetParam(\"VelocitiesX\", 17)");
                macroData.WriteLine("SetParam(\"VelocitiesY\", 17)");
                macroData.WriteLine("SetParam(\"VelocitiesZ\", 9)");

                macroData.Close();
            }
            scriptObject.Code("M91");
            lowSpeed = true;
        }

        /// <summary>
        /// x, y, z ~ 50 mm/min
        /// </summary>
        private void SuperLowSpeedMode()
        {
            // Build Set speed macro
            using (macroData = new StreamWriter(Path.Combine(macroDirectory, @"M91.m1s")))
            {
                macroData.WriteLine("SetParam(\"VelocitiesX\", 0.8)");
                macroData.WriteLine("SetParam(\"VelocitiesY\", 0.8)");
                macroData.WriteLine("SetParam(\"VelocitiesZ\", 0.8)");

                macroData.Close();
            }
            scriptObject.Code("M91");
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
                StatusLabel.Text = "Đang Kết Nối Camera...";
                StatusLabel.Refresh();

                if (_capture == null)
                {
                    try
                    {
                        XElement CameraCalibrationWindow = SysData.Element("System").Element("CameraCalibrationWindow");
                        int camIndex = int.Parse(CameraCalibrationWindow.Element("CameraID").Attribute("ID").Value);
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
                        StatusLabel.Text = "Đã kết nối camera";
                        StatusLabel.Refresh();

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

        private void LoadSetData()
        {
            //
            processLog.Items.Clear();

            // Select data path file
            SetSelection(itemsSet.Text);

            // Load data file
            DispensingData = XDocument.Load(DataPath);
            XElement SetData = DispensingData.Element("Field").Element("SetData");

            // Load object parameters
            RoiSize = ushort.Parse(SetData.Attribute("RoiSize").Value);
            OuterCircleChessRadius = float.Parse(SetData.Attribute("ItemOuterDia").Value) / 2;
            InnerCircleChessRadius = float.Parse(SetData.Attribute("ItemInnnerDia").Value) / 2;
            ObjectHeight = float.Parse(SetData.Attribute("ItemHeight").Value);
            SheetThickness = float.Parse(SetData.Attribute("SheetThickness").Value);

            //
            PaintingPointScale = RoiSize / RawTmpSize;

            //            
            processLog.Items.Add("Bán kính mẫu:   " + OuterCircleChessRadius.ToString());
            processLog.Items.Add("Bán kính vòng trong:   " + InnerCircleChessRadius.ToString());
            processLog.Items.Add("Chiều cao mẫu:   " + ObjectHeight.ToString());

            LoadObjectData();
        }

        private void LoadObjectData()
        {
            // View the previous working template
            ViewTemplate(itemsSet.Text, tmp_item_name.Text);
            CheckTmpSize();
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

        private void LoadOriginal(string value)
        {
            XElement MainWindow = SysData.Element("System").Element("MainWindow");
            XElement _offset = MainWindow;

            switch (value)
            {
                case "Đỏ":
                    _offset = MainWindow.Element("Offset").Element("G54");
                    processLog.Items.Add("Đã tải lại gốc camera G54");
                    break;
                case "Đen":
                    _offset = MainWindow.Element("Offset").Element("G55");
                    processLog.Items.Add("Đã tải lại gốc camera G55");
                    break;

            }
            _xoffset = double.Parse(_offset.Attribute("x").Value);
            _yoffset = -double.Parse(_offset.Attribute("y").Value);
        }

        private void ReadSystemData()
        {
            // Load system xml
            SysData = XDocument.Load(systemPath);

            XElement MainWindow = SysData.Element("System").Element("MainWindow");

            // Last item info
            itemsSet.Text = MainWindow.Element("Template").Attribute("LastSet").Value;
            tmp_item_name.Text = MainWindow.Element("Template").Attribute("LastItem").Value;
            item_color.Text = MainWindow.Element("Template").Attribute("LastColor").Value;

            // Get last machine-camera offset value
            calib_xOffset.Text = MainWindow.Element("CameraOffset").Attribute("X").Value;
            calib_yOffset.Text = MainWindow.Element("CameraOffset").Attribute("Y").Value;

            // Get camera accuracy
            _accuracy = double.Parse(SysData.Element("System").Element("CameraCalibrationWindow").Element("Accuracy").Attribute("Value").Value);

            //
            valveNum.Text = (GetValveNum(item_color.Text) - 3).ToString();

            // Load Painting Conditions
            xySpeed.Text        = MainWindow.Element("PaintingCondition").Attribute("xySpeed").Value;
            zSpeed.Text         = MainWindow.Element("PaintingCondition").Attribute("zSpeed").Value;
            zSafe.Text          = MainWindow.Element("PaintingCondition").Attribute("zSafe").Value;
            zReturn.Text        = MainWindow.Element("PaintingCondition").Attribute("zReturn").Value;
            zDrip.Text          = MainWindow.Element("PaintingCondition").Attribute("zDrip").Value;
            deepOffset.Text     = MainWindow.Element("PaintingCondition").Attribute("offset").Value;
            circleSpeed.Text    = MainWindow.Element("PaintingCondition").Attribute("circleSpeed").Value;

            // Load Offset values
            XElement Offset = MainWindow.Element("Offset");
            for (int i = 0; i < 5; i++)
            {
                string Gxx = "G" + (54 + i).ToString();
                OffsetCoor.Rows.Add(Gxx,
                    Offset.Element(Gxx).Attribute("x").Value,
                    Offset.Element(Gxx).Attribute("y").Value,
                    Offset.Element(Gxx).Attribute("z").Value);
            }

            // Load camera calibration data
            XElement CalibrationParameter = MainWindow.Element("CalibrationParameter");
            calib_xOffset.Text = CalibrationParameter.Attribute("calib_xOffset").Value;
            calib_yOffset.Text = CalibrationParameter.Attribute("calib_yOffset").Value;
            calib_cornerWidth.Text = CalibrationParameter.Attribute("calib_cornerWidth").Value;
            calib_cornerHeight.Text = CalibrationParameter.Attribute("calib_cornerHeight").Value;
            calib_noOfSample.Text = CalibrationParameter.Attribute("calib_noOfSample").Value;
            calib_squareSize.Text = CalibrationParameter.Attribute("calib_squareSize").Value;
            camID.Text = SysData.Element("System").Element("CameraCalibrationWindow").Element("CameraID").Attribute("ID").Value;

            // Load image processing data
            XElement ImageProcessing = SysData.Element("System").Element("ImageProcessingWindow");

            // Load center correction data
            cannyThresh.Value = decimal.Parse(ImageProcessing.Element("MachingCorrection").Attribute("cannyThresh").Value);
            correctionRange.Value = decimal.Parse(ImageProcessing.Element("MachingCorrection").Attribute("correctionRange").Value);
            ErrConstraint.Value = decimal.Parse(ImageProcessing.Element("MachingCorrection").Attribute("ErrConstraint").Value);

            // Load Hough transform data 
            ReadHoughData(itemsSet.Text);

            // Load Gaussian filter data
            gaussian_sig.Value = decimal.Parse(ImageProcessing.Element("ImageFiltering").Attribute("gaussian_sig").Value);
            G_blur.CheckState = (decimal.Parse(ImageProcessing.Element("ImageFiltering").Attribute("G_blur").Value) == 1) ? CheckState.Checked : CheckState.Unchecked;
            BlockSize.Value = decimal.Parse(ImageProcessing.Element("ImageFiltering").Attribute("BlockSize").Value);

            // Load perspective transform data
            ReadPersData(itemsSet.Text);

            // View template image
            templateField.Image = tmpBgr.Bitmap;
            templateField.Refresh();
        }

        private void ReadHoughData(string set)
        {
            XElement ImageProcessing = SysData.Element("System").Element("ImageProcessingWindow");
            XElement SetHoughData = ImageProcessing;

            switch (set)
            {
                case "Cờ 31":
                    SetHoughData = ImageProcessing.Element("HoughCirclesDetector").Element("Chess31");
                    break;
                case "Cờ 29":
                    SetHoughData = ImageProcessing.Element("HoughCirclesDetector").Element("Chess29");
                    break;
            }

            houge_dp.Value = decimal.Parse(SetHoughData.Attribute("dp").Value);
            houge_minDist.Value = decimal.Parse(SetHoughData.Attribute("minDist").Value);
            houge_param1.Value = decimal.Parse(SetHoughData.Attribute("houge_param1").Value);
            houge_param2.Value = decimal.Parse(SetHoughData.Attribute("houge_param2").Value);
            min_ra.Value = decimal.Parse(SetHoughData.Attribute("min_ra").Value);
            max_ra.Value = decimal.Parse(SetHoughData.Attribute("max_ra").Value);
        }

        private void ReadPersData(string set)
        {
            XElement ImageProcessing = SysData.Element("System").Element("ImageProcessingWindow");
            XElement SetPersData = ImageProcessing;

            switch (set)
            {
                case "Cờ 31":
                    SetPersData = ImageProcessing.Element("PSTTransform").Element("Chess31");
                    break;
                case "Cờ 29":
                    SetPersData = ImageProcessing.Element("PSTTransform").Element("Chess29");
                    break;
            }

            cnl1.Value = decimal.Parse(SetPersData.Attribute("edge1").Value);
            cnl2.Value = decimal.Parse(SetPersData.Attribute("edge2").Value);
            cnl3.Value = decimal.Parse(SetPersData.Attribute("edge3").Value);
            cnl4.Value = decimal.Parse(SetPersData.Attribute("edge4").Value);
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

            MainWindow.Element("Template").Attribute("LastSet").Value = itemsSet.Text;
            MainWindow.Element("Template").Attribute("LastItem").Value = tmp_item_name.Text;
            MainWindow.Element("Template").Attribute("LastColor").Value = item_color.Text;
            MainWindow.Element("CameraOffset").Attribute("X").Value = calib_xOffset.Text;
            MainWindow.Element("CameraOffset").Attribute("Y").Value = calib_yOffset.Text;

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

                        Size pt_size = new Size(pt_width, pt_height);
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

                                StatusLabel.Text = "Frame: " + frame_buffer_savepoint.ToString() + "/" + frame_array_buffer.Length.ToString();
                                StatusLabel.Refresh();
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

                            MessageBox.Show("Cannot detect corners." +
                                Environment.NewLine + "Please disable undistortion and recheck corners set.", "CVEye");

                            // Return to calibrated mode
                            currentMode = Mode.Calibrated;
                        }

                        // View raw image
                        pattern_field.Image = img_capture.Bitmap;
                        pattern_field.Refresh();
                    }

                    if (currentMode == Mode.Caluculating_Intrinsics)
                    {
                        // Update status
                        StatusLabel.Text = "Caluculating Intrinsics...";
                        StatusLabel.Refresh();

                        //
                        Size pt_size = new Size(pt_width, pt_height);

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

                            StatusLabel.Text = "Corner Set: " + (k + 1).ToString() + "/" + frame_array_buffer.Length.ToString();
                            StatusLabel.Refresh();
                        }

                        StatusLabel.Text = "Getting Camera Matrices";
                        StatusLabel.Refresh();

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
                        StatusLabel.Text = "Saving Data";
                        StatusLabel.Refresh();
                        Thread.Sleep(1000);

                        FileStorage fs = new FileStorage("_camera.xml", FileStorage.Mode.Write);
                        fs.Write(cameraMat, "Camera_Matrix");
                        fs.Write(distCoeffsMat, "Distortion_Coefficients");

                        StatusLabel.Text = "Calibration Completed";
                        StatusLabel.Refresh();
                    }

                    if (currentMode == Mode.Calibrated)
                    {
                        // Apply Calibration
                        Mat undist_frame = new Mat();
                        if (enableCamUndist.CheckState == CheckState.Checked)
                        {
                            CvInvoke.Undistort(frame_raw, undist_frame, cameraMat, distCoeffsMat);
                            img_capture_undist = undist_frame.ToImage<Bgr, byte>();
                        }
                        else
                            img_capture_undist = frame_raw.ToImage<Bgr, byte>();

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

        private void CalibrationStart()
        {
            // Load data
            SysData = XDocument.Load(systemPath);
            XElement CalibrationParameter = SysData.Element("System").Element("MainWindow").Element("CalibrationParameter");

            // Read corner size
            pt_width = byte.Parse(CalibrationParameter.Attribute("calib_cornerWidth").Value);
            pt_height = byte.Parse(CalibrationParameter.Attribute("calib_cornerHeight").Value);

            // Load number of samples
            byte byteNum = byte.Parse(CalibrationParameter.Attribute("calib_noOfSample").Value);
            frame_array_buffer = new Mat[byteNum];

            // Load square size in mm
            square_size = float.Parse(CalibrationParameter.Attribute("calib_squareSize").Value);

            //
            corners_object_list = new MCvPoint3D32f[frame_array_buffer.Length][];
            corners_point_list = new PointF[frame_array_buffer.Length][];
            corners_point_vector = new VectorOfPointF[frame_array_buffer.Length];

            //
            start_calib = true;

            // Set saving frame mode
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
                    (float)((double.Parse(element.Attribute("X").Value) * PaintingPointScale - RoiSize / 2) * Math.Cos(angle)) -
                    (float)(((double.Parse(element.Attribute("Y").Value) * PaintingPointScale - RoiSize / 2) * Math.Sin(angle))) +
                    objectCenter.X;
                affine_painting_points[point_num].Y =
                    (float)((double.Parse(element.Attribute("X").Value) * PaintingPointScale - RoiSize / 2) * Math.Sin(angle)) +
                    (float)(((double.Parse(element.Attribute("Y").Value) * PaintingPointScale - RoiSize / 2) * Math.Cos(angle))) +
                    objectCenter.Y;

                // For checking
                draw_image_bgr.Draw(new Cross2DF(affine_painting_points[point_num], 2, 2), new Bgr(Color.Red), 1);

                // Compute machine points
                affine_painting_points[point_num].X = (float)xCompensate(affine_painting_points[point_num]);
                affine_painting_points[point_num].Y = (float)yCompensate(affine_painting_points[point_num]);

                // Matching camera coordinate with machine coordinate
                affine_painting_points[point_num] = RotateFOV(affine_painting_points[point_num], 0.8);

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
                            int.Parse(ImageProcessingWindow.Element("ImageFiltering").Attribute("BlockSize").Value),
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
                        CvInvoke.AdaptiveThreshold(mat_gray,
                            img_threshold,
                            255,
                            AdaptiveThresholdType.GaussianC,
                            ThresholdType.Binary,
                            int.Parse(ImageProcessingWindow.Element("ImageFiltering").Attribute("BlockSize").Value),
                            1.5);
                    }

                    return mat_gray;
                }
            }
        }

        private void SetSelection(string value)
        {
            switch (value)
            {
                case "Cờ 31":
                    DataPath = Chess_31_DataPath;
                    processLog.Items.Add("Đã tải bộ cờ 31.");
                    break;
                case "Cờ 29":
                    DataPath = Chess_29_DataPath;
                    processLog.Items.Add("Đã tải bộ cờ 29.");
                    break;
            }
        }

        private void LoadImageProcessingData(string value)
        {
            // Call ImageProcessingWindow node
            XElement ImageProcessingWindow = SysData.Element("System").Element("ImageProcessingWindow");
            XElement SetHoughData = ImageProcessingWindow.Element("HoughCirclesDetector").Element("Chess31");

            switch (value)
            {
                case "Cờ 31":
                    SetHoughData = ImageProcessingWindow.Element("HoughCirclesDetector").Element("Chess31");
                    break;
                case "Cờ 29":
                    SetHoughData = ImageProcessingWindow.Element("HoughCirclesDetector").Element("Chess29");
                    break;
            }

            // Assigment
            _dp = double.Parse(SetHoughData.Attribute("dp").Value);
            _minDist = double.Parse(SetHoughData.Attribute("minDist").Value);
            _param01 = double.Parse(SetHoughData.Attribute("houge_param1").Value);
            _param02 = double.Parse(SetHoughData.Attribute("houge_param2").Value);
            _minRadius = int.Parse(SetHoughData.Attribute("min_ra").Value);
            _maxRadius = int.Parse(SetHoughData.Attribute("max_ra").Value);
        }

        public void DetectClone()
        {
            //
            detectionWatch = Stopwatch.StartNew();

            // Call ImageProcessingWindow node
            LoadImageProcessingData(itemsSet.Text);

            // Update progress bar
            Progress.Value = 3;

            img_items = img_capture_undist.Clone(); // => should be fixed
            using (Mat img_gray = Frame_Analyze())
            {
                // Update progress bar
                Progress.Value = 5;

                // Circle Hough Transform
                CircleF[] img_circles = CvInvoke.HoughCircles(img_gray, HoughType.Gradient, _dp, _minDist,
                    _param01, _param02, _minRadius, _maxRadius);

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
                    if (true)
                    {
                        // Cal ROI location
                        Point roi_location = new Point((int)Math.Round(circle.Center.X) - RoiSize / 2,
                                        (int)Math.Round(circle.Center.Y) - RoiSize / 2);

                        Mat getMat = new Mat();

                        // Data allocation
                        byte[] roiData = new byte[RoiSize * RoiSize];
                        GCHandle handle = GCHandle.Alloc(roiData, GCHandleType.Pinned);
                        using (Image<Gray, byte> src = img_threshold.ToImage<Gray, byte>())
                        using (Image<Gray, byte> roi_getting_img = GetSourceROI(src, roi_location, RoiSize))
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

                        int numOfpixels = RoiSize * RoiSize;

                        // Find maching angle
                        for (int cnt = 0; cnt < 360; cnt = cnt + 2)
                        {
                            // Sum of pixels value, update each cycle
                            int pixel_sum = 0;

                            // Template rotation 
                            using (Mat tmp_rot = new Mat())
                            {
                                //

                                CvInvoke.GetRotationMatrix2D(new PointF(RoiSize / 2, RoiSize / 2), cnt, 1, tmp_dst);
                                CvInvoke.WarpAffine(tmp_raw, tmp_rot, tmp_dst, tmp_raw.Size); // ~100 ms => it takes almost scanning time, we should improve

                                //
                                using (Image<Gray, byte> tmp_rot_img = tmp_rot.ToImage<Gray, byte>())
                                using (Mat _tmp_rot = tmp_rot_img.Mat)
                                {
                                    byte[] tmpData = new byte[RoiSize * RoiSize];
                                    GCHandle _handle = GCHandle.Alloc(tmpData, GCHandleType.Pinned);
                                    CvInvoke.BitwiseNot(_tmp_rot, _tmp_rot);
                                    using (Mat tempMat = new Mat(_tmp_rot.Size, DepthType.Cv8U, 1, _handle.AddrOfPinnedObject(), tmp_rot.Width))
                                    {
                                        CvInvoke.BitwiseNot(_tmp_rot, tempMat);
                                        //CvInvoke.Imwrite("data/tmp" + cnt.ToString() + ".jpg", tempMat); // Sure!!!
                                    }
                                    _handle.Free();

                                    // Get maching value
                                    for (int i = 0; i < numOfpixels; i++) // Taking 15 - 20 ms
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
                // Add object data to their lists
                _angleList.Add(get_angle);
                _centerList.Add(get_circle.Center);

                // Show detected items
                Draw_Matching(img_items, get_circle, get_angle);

                // Show object radius
                processLog.Items.Add(get_circle.Radius);

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
            XElement PaintingCondition = SysData.Element("System").Element("MainWindow").Element("PaintingCondition");

            // Get current item painting points
            getItem = DispensingData.Element("Field")
                .Elements("Item")
                .Where(x => x.Element("Name").Value == tmp_item_name.Text)
                .Single();

            // List painting points to list
            List<XElement> point_list = getItem.Element("Points").Elements("Point").ToList();
            
            // Item sorting
            _centerList = SortByDistance(_centerList, _angleList);

            // Reset macro combination position
            LastItem = 0;
            MacroLineNumber = 0;
            ushort MaxLine = 18000;

            //
            using (macroData)
            {
                #region Painting macro building

                for (byte pointidx = 0; pointidx < _centerList.Count; pointidx++)
                {
                    if (MacroLineNumber <= MaxLine)
                    {
                        // Machine coordinate (G54, G55)
                        //^ Y axis
                        //|  <Image area>
                        //|  <Image area>
                        //L---------> X axis
                        //O

                        // Calculate center coordinate
                        double cen_X = xCompensate(_centerList[pointidx]);
                        double cen_Y = yCompensate(_centerList[pointidx]);
                        PointF cenPoint = new PointF((float)cen_X, (float)cen_Y);

                        // Global painting points rotation
                        double dirCom = 0.8;
                        cenPoint = RotateFOV(cenPoint, dirCom);

                        cen_X = Math.Round(cenPoint.X * _accuracy, 2) + _xoffset;
                        cen_Y = Math.Round(cenPoint.Y * _accuracy, 2) + _yoffset;

                        // Invert y axis direction
                        cen_Y = -cen_Y;

                        // Points set affine transformation
                        Real_PointsWarpAffine(point_list, _centerList[pointidx], angleListSort[pointidx], img_items);

                        // Building painting procedure
                        CompletedPaintingProcess(affine_painting_points, (float)cen_X, (float)cen_Y, PaintingCondition, macroData);

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
                    else
                    {
                        //
                        LastItem = pointidx;

                        // Disable selection piston
                        macroData.WriteLine("' Macro combination condition");
                        macroData.WriteLine("DeactivateSignal(OUTPUT6)");

                        // Return to z macro combination point
                        macroData.WriteLine("Code \"G00 Z" + 13.4 + "\"");
                        Wait(50, macroData);

                        //processLog.Items.Add("Checking combination condition...");

                        MacroLineNumber = (ushort)(MacroLineNumber + 3);

                        // Terminating loop
                        break;
                    }
                }

                #endregion

                #region Complete Painting Process

                // Do not finish the process if number of macro lines exceed maximum value
                if (LastItem == 0)
                {
                    // Return to z safe
                    macroData.WriteLine("Code \"G00 Z" + PaintingCondition.Attribute("zSafe").Value + "\"");
                    Wait(50, macroData);

                    // Disable dispensing valve channel
                    macroData.WriteLine("DeactivateSignal(OUTPUT" + GetValveNum(item_color.Text) + ")");

                    // Disable selection piston
                    macroData.WriteLine("' Disable level piston");
                    macroData.WriteLine("DeactivateSignal(OUTPUT6)");

                    // Disable frame holding piston
                    macroData.WriteLine("' Disable clamping piston");
                    macroData.WriteLine("DeactivateSignal(OUTPUT8)");

                    //  Return home
                    macroData.WriteLine("' Return home");
                    macroData.WriteLine("Code \"G90 G54 G00 X-90 Y-400\"");

                    MacroLineNumber = (ushort)(MacroLineNumber + 8);

                    //processLog.Items.Add("No need to combine macro.");
                }

                // Close all macros
                macroData.Close();

                #endregion
            }

            // Save detected items result
            CvInvoke.Imwrite("result/dots.jpg", img_items);
            //CvInvoke.Imwrite("result/dots.jpg", img_threshold);

            // View result
            pattern_field.Image = img_items.Bitmap;
            pattern_field.Refresh();
            
            // Update working status
            StatusLabel.Text = "Hoàn tất quét ảnh";
            StatusLabel.Refresh();

            // Detection status
            detected = true;

            // 
            startDetection.Enabled = true;
            turnCamera.Enabled = true;
            captureImg.Enabled = true;
            stopDetection.Enabled = false;

            //
            timerDROupdate.Enabled = true;

            // Show processing times
            //processLog.Items.Add(detectionWatch.ElapsedMilliseconds);
            //processLog.Items.Add("Số dòng lệnh: " + MacroLineNumber);
            //processLog.Items.Add("Chuyển tại mẫu: " + LastItem);

            //
            Progress.Visible = false;
        }

        private void MacroCombination(byte lastitem, string value)
        {
            double zSafe;
            zSafe = Math.Round(double.Parse(value), 1);

            if ((zSafe == 13.4) && (!scriptObject.IsOutputActive(12)))
            {
                //processLog.Items.Add("Building combined macro...");

                // Reset combination flag
                CombineFlag = true;

                // Load database
                XElement PaintingCondition = SysData.Element("System").Element("MainWindow").Element("PaintingCondition");

                // List painting points to list
                List<XElement> point_list = getItem.Element("Points").Elements("Point").ToList();                

                // Init new mach3 macro file
                if (macroData != null)
                    macroData.Close();
                macroData = new StreamWriter(Path.Combine(macroDirectory, @"M999.m1s"));

                //
                using (macroData)
                {
                    #region Painting macro building

                    macroData.WriteLine("ActivateSignal(OUTPUT6)");

                    for (byte pointidx = LastItem; pointidx < _centerList.Count; pointidx++)
                    {
                        // Machine coordinate (G54, G55)
                        //^ Y axis
                        //|  <Image area>
                        //|  <Image area>
                        //L---------> X axis
                        //O

                        // Calculate center coordinate
                        double cen_X = xCompensate(_centerList[pointidx]);
                        double cen_Y = yCompensate(_centerList[pointidx]);
                        PointF cenPoint = new PointF((float)cen_X, (float)cen_Y);

                        // Global painting points rotation
                        double dirCom = 0.8;
                        cenPoint = RotateFOV(cenPoint, dirCom);

                        cen_X = Math.Round(cenPoint.X * _accuracy, 3) + _xoffset;
                        cen_Y = Math.Round(cenPoint.Y * _accuracy, 3) + _yoffset;

                        // Invert y axis direction
                        cen_Y = -cen_Y;

                        // Points set affine transformation
                        Real_PointsWarpAffine(point_list, _centerList[pointidx], angleListSort[pointidx], img_items);

                        // Building painting procedure
                        CompletedPaintingProcess(affine_painting_points, (float)cen_X, (float)cen_Y, PaintingCondition, macroData);

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
                    macroData.WriteLine("Code \"G00 Z" + PaintingCondition.Attribute("zSafe").Value + "\"");
                    Wait(50, macroData);

                    // Disable dispensing valve channel
                    macroData.WriteLine("DeactivateSignal(OUTPUT" + GetValveNum(item_color.Text) + ")");

                    // Disable selection piston
                    macroData.WriteLine("' Disable level piston");
                    macroData.WriteLine("DeactivateSignal(OUTPUT6)");

                    // Disable frame holding piston
                    macroData.WriteLine("' Disable clamping piston");
                    macroData.WriteLine("DeactivateSignal(OUTPUT8)");

                    //  Return home
                    macroData.WriteLine("' Return home");
                    macroData.WriteLine("Code \"G90 G54 G00 X-90 Y-400\"");

                    // Close all macros
                    macroData.Close();

                    // Update no. of macro lines
                    MacroLineNumber = (ushort)(MacroLineNumber + 8);

                    #endregion
                }

                Thread.Sleep(100);

                GetMach3Instance();

                if (scriptObject != null)
                {
                    // Run painting macro
                    scriptObject.Code("M999");
                }

                //processLog.Items.Add("Macro is combined successfully.");

                // Reset macro combination position
                LastItem = 0;
                MacroLineNumber = 0;
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

        private Image<Gray, byte> GetSourceROI(Image<Gray, byte> src, Point ROILocation, int RoiSize)
        {
            using (src)
            {
                // Set ROI to image
                Rectangle roi_rec = new Rectangle(ROILocation, new Size(RoiSize, RoiSize));
                CvInvoke.cvSetImageROI(src.Ptr, roi_rec);
                src.ROI = CvInvoke.cvGetImageROI(src.Ptr);

                // Create ROI background
                using (Image<Bgr, byte> roi_background = new Image<Bgr, byte>(src.Size))
                {
                    Mat roi_getting = new Mat();
                    roi_background.SetValue(255);
                    roi_background.Draw(
                        new CircleF(new Point(roi_background.Rows / 2, roi_background.Cols / 2), RoiSize / 2),
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

        private  void PixelsCompensation(Image<Bgr, byte> sample)
        {
            SysData = XDocument.Load(systemPath);
            XElement section;
            XElement CalibrationParameter = SysData.Element("System").Element("MainWindow").Element("CalibrationParameter");

            SysData.Element("System").Element("CameraCalibrationWindow").Element("xCompensation").RemoveAll();
            SysData.Element("System").Element("CameraCalibrationWindow").Element("yCompensation").RemoveAll();

            // Read corner size
            pt_width = byte.Parse(CalibrationParameter.Attribute("calib_cornerWidth").Value);
            pt_height = byte.Parse(CalibrationParameter.Attribute("calib_cornerHeight").Value);
                        
            Mat sample_frame = new Mat();
            
            //
            Size sample_size = new Size(pt_width, pt_height);

            CameraData.Items.Clear();

            // int cornerPosOri = pt_width * pt_height / 2 - pt_width / 2; using if pt is even
            int cornerPosOri = pt_width * (pt_height / 2 + 1) - (pt_width / 2 + 1);
            CameraData.Items.Add("Camera Origin No.:  " + cornerPosOri.ToString());
            CornerPos.Text = cornerPosOri.ToString();

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

                // Calculate system accuracy
                float squareSize = ((corner_set[cornerPosOri + 1].X - corner_set[cornerPosOri].X) +
                    (corner_set[cornerPosOri - pt_width + 1].X - corner_set[cornerPosOri - pt_width].X) +
                    (corner_set[cornerPosOri + pt_width + 1].X - corner_set[cornerPosOri + pt_width].X)) / 3;
                _accuracy = 10 / squareSize;

                sample.Draw(new CircleF(corner_set[cornerPosOri], 25), new Bgr(Color.Blue), 5);
                //sample.Draw(new CircleF(corner_set[cornerPosOri + pt_width], 15), new Bgr(Color.GreenYellow), 5);
                //sample.Draw(new CircleF(corner_set[cornerPosOri - pt_width], 15), new Bgr(Color.GreenYellow), 5);

                //SysData.Element("System").Element("CameraCalibrationWindow").Add(new XElement("Accuracy", new XAttribute("Value", _accuracy)));
                SysData.Element("System").Element("CameraCalibrationWindow").Element("Accuracy").Attribute("Value").Value = _accuracy.ToString();

                // New variable
                PointF[] rowComPoint = new PointF[2];
                PointF[] colComPoint = new PointF[2];

                using (StreamWriter logStream = new StreamWriter("history.txt"))
                {
                    //
                    section = SysData.Element("System").Element("CameraCalibrationWindow").Element("yCompensation");
                  
                    float xsegEnd = 0;

                    // Y axis compensation data
                    for (int col = 0; col < pt_width; col++) // column scan from 0 to col width -1                
                    {
                        // Add new column segment
                        section.Add(new XElement("Column", new XElement("Num", col)));

                        //byte rowSegNum = (byte)(pt_height / 2 - 1); //when pt_height is even
                        byte rowSegNum = (byte)(pt_height / 2); //when pt_height is odd
                        byte colSegNum = (byte)(pt_width / 2);

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
                            sample.Draw(cornerPos.ToString(),
                                new Point((int)corner_set[cornerPos].X - 10, (int)corner_set[cornerPos].Y - 10),
                                FontFace.HersheyPlain,
                                1.2,
                                new Bgr(Color.Red),
                                1);
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

                        byte rowSegNum = (byte)(pt_height / 2 - 1);
                        byte colSegNum = (byte)(pt_width / 2);

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

                    if (EnableSaveCompsData.CheckState == CheckState.Checked)
                    {
                        SysData.Save(systemPath);
                        SysData = XDocument.Load(systemPath);
                    }

                    double xCom = xCompensate(corner_set[cornerPosOri]);
                    double yCom = yCompensate(corner_set[cornerPosOri]);

                    sample.Draw(new CircleF(new PointF((float)xCom, (float)yCom), 15), new Bgr(Color.GreenYellow), 5);
                }

                //
                CameraData.Items.Add("Number of rows:    " + pt_height.ToString());
                CameraData.Items.Add("Number of cols:    " + pt_width.ToString());
                CameraData.Items.Add("Acc.:   " + _accuracy.ToString());
            }
            else
                MessageBox.Show("Cannot Find Corners.", "CVEye");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Points">The vector of global painting points</param>
        /// <param name="cenX">X object center</param>
        /// <param name="cenY">Y object center</param>
        /// <param name="Parameters">Painting conditions data</param>
        /// <param name="macro"> Output macro</param>
        private void CompletedPaintingProcess(PointF[] Points, float cenX, float cenY, XElement Parameters, TextWriter macro)
        {
            double X, Y; //mm
            float startpoint_X = cenX - InnerCircleChessRadius; //mm
            float startpoint_Y = cenY; //mm

            List<XElement> point_list = getItem.Element("Points").Elements("Point").ToList();

            // Get painting condition
            float ZReturnValue      = SheetThickness + ObjectHeight + float.Parse(Parameters.Attribute("zReturn").Value);
            float ZOffset           = float.Parse(Parameters.Attribute("offset").Value);
            int CirclePaintingSpeed = int.Parse(Parameters.Attribute("circleSpeed").Value);
            int NormalPaintingSpeed = int.Parse(Parameters.Attribute("xySpeed").Value);

            int j = 0;

            foreach (XElement point in point_list)
            {
                // Convert pixel to mm
                X = Math.Round(Points[j].X * _accuracy + _xoffset, 2);
                Y = -Math.Round(Points[j].Y * _accuracy + _yoffset, 2);

                byte pointMode = byte.Parse(point.Attribute("T").Value);

                //--------------------- Build macro01-------------------

                // with the first point of each item
                if (j == 0)
                {
                    // with the first point of the first item
                    if (first_item)
                    {
                        // macro01 initialization
                        //macro.WriteLine("' Initialization");
                        macro.WriteLine("Code \"G90 " + GetValveHome(item_color.Text) + " G00" + "\"");
                        MacroLineNumber++;

                        // Select valve position
                        //macro.WriteLine("' Select valve");
                        macro.WriteLine("ActivateSignal(OUTPUT" + GetValveNum(item_color.Text) + ")");
                        MacroLineNumber++;

                        // Move to the first point of inner circle
                        //macro.WriteLine("' Circle starting point");
                        macro.WriteLine("Code \"G00 X" + startpoint_X + " Y" + startpoint_Y + "\"");
                        MacroLineNumber++;
                        Wait(2000, macro);

                        // Turn on 15mm piston
                        //macro.WriteLine("' Turning on level piston");
                        macro.WriteLine("ActivateSignal(OUTPUT6)");
                        MacroLineNumber++;

                        // Select painting mode 03
                        JettingModeSelectingMacro(3, macro);

                        // Dispense paint
                        //macro.WriteLine("' Starting dispensing");
                        Paint_Drip(ZOffset, macro);

                        // Circular interpolation
                        //macro.WriteLine("' Circular interpolation");
                        CircleDivider(new PointF(cenX, cenY), InnerCircleChessRadius, 4, CirclePaintingSpeed, macro);
                        Wait(50, macro);

                        // Select painting mode 01
                        JettingModeSelectingMacro(pointMode, macro);

                        // Go to z return
                        //macro.WriteLine("' Moving to z return");
                        macroData.WriteLine("Code \"G00 Z" + ZReturnValue + "\"");
                        MacroLineNumber++;

                        // Move to the first point of inner circle
                        //macro.WriteLine("' Moving to painting point of pattern");
                        macro.WriteLine("Code \"G01 X" + X + " Y" + Y + " F" + NormalPaintingSpeed + "\"");
                        MacroLineNumber++;
                        Wait(50, macro);
                    }
                    else
                    {
                        // Move to the first point of inner circle
                        //macro.WriteLine("' Circle starting point");
                        macro.WriteLine("Code \"G00 X" + startpoint_X + " Y" + startpoint_Y + "\"");
                        MacroLineNumber++;
                        Wait(500, macro);

                        // Select painting mode 03
                        JettingModeSelectingMacro(3, macro);

                        // Dispense paint
                        //macro.WriteLine("' Starting dispensing");
                        Paint_Drip(ZOffset, macro);

                        // Circular interpolation
                        //macro.WriteLine("' Circular interpolation");
                        CircleDivider(new PointF(cenX, cenY), InnerCircleChessRadius, 4, CirclePaintingSpeed, macro);
                        Wait(50, macro);

                        // Select painting mode 01
                        JettingModeSelectingMacro(pointMode, macro);

                        // Go to z return
                        //macro.WriteLine("' Moving to z return");
                        macro.WriteLine("Code \"G00 Z" + ZReturnValue + "\"");
                        MacroLineNumber++;

                        // First point of non-first item
                        //macro.WriteLine("' Moving to painting point of pattern");
                        macro.WriteLine("Code \"G01 X" + X + " Y" + Y + " F" + NormalPaintingSpeed + "\"");
                        MacroLineNumber++;
                        Wait(50, macro);
                    }
                }
                else
                {
                    JettingModeSelectingMacro(pointMode, macro);

                    // Go to painting point of template
                    //macro.WriteLine("' Moving to painting point of pattern");
                    macro.WriteLine("Code \"G01 X" + X + " Y" + Y + " F" + NormalPaintingSpeed + "\"");
                    MacroLineNumber++;
                    //Wait(50);
                }

                // Dispense paint
                Paint_Drip(0, macro);

                // Rapid moving to Z return
                //macro.WriteLine("' Moving to z return");
                macro.WriteLine("Code \"Z" + ZReturnValue + "\"");
                MacroLineNumber++;

                // Disable the first item flag
                first_item = false;

                j++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="factor"></param>
        /// <param name="speed"></param>
        /// <param name="macro"></param>
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
                MacroLineNumber++;

                // Update start point of the arc
                startPoint.X = endPoint.X;
                startPoint.Y = endPoint.Y;
            }
        }

        private void Paint_Drip(float offset, TextWriter macro)
        {
            XElement PaintingCondition = SysData.Element("System").Element("MainWindow").Element("PaintingCondition");

            float z_value = SheetThickness + ObjectHeight + float.Parse(PaintingCondition.Attribute("zDrip").Value) + offset;

            // Moving to Z injecting height with specific speed
            if (!first_item)
                macro.WriteLine("Code \"G00 Z" + z_value + "\"");
            else
                macro.WriteLine("Code \"Z" + z_value + "\"");

            //
            MacroLineNumber++;

            // Waiting for moving completed
            //Wait(firstpointofitem ? 100 : 50);
            Wait(50, macro);

            // Drip paint
            macro.WriteLine("ActivateSignal(OUTPUT7" + ")");
            macro.WriteLine("Sleep(100)");
            macro.WriteLine("DeactivateSignal(OUTPUT7" + ")");
            MacroLineNumber = (ushort)(MacroLineNumber + 3);
        }

        private void JettingModeSelectingMacro(byte mode, TextWriter macro)
        {
            switch (mode)
            {
                case 1: // 0 0
                    macro.WriteLine("' Painting mode 01");
                    macro.WriteLine("DeactivateSignal(OUTPUT1" + ")");
                    macro.WriteLine("DeactivateSignal(OUTPUT2" + ")");
                    MacroLineNumber = (ushort)(MacroLineNumber + 3);
                    break;
                case 2: // 0 1
                    macro.WriteLine("' Painting mode 02");
                    macro.WriteLine("DeactivateSignal(OUTPUT1" + ")");
                    macro.WriteLine("ActivateSignal(OUTPUT2" + ")");
                    MacroLineNumber = (ushort)(MacroLineNumber + 3);
                    break;
                case 3: // 1 0
                    macro.WriteLine("' Painting mode 03");
                    macro.WriteLine("ActivateSignal(OUTPUT1" + ")");
                    macro.WriteLine("DeactivateSignal(OUTPUT2" + ")");
                    MacroLineNumber = (ushort)(MacroLineNumber + 3);
                    break;
                case 4: // 1 1
                    macro.WriteLine("' Painting mode 04");
                    macro.WriteLine("ActivateSignal(OUTPUT1" + ")");
                    macro.WriteLine("ActivateSignal(OUTPUT2" + ")");
                    MacroLineNumber = (ushort)(MacroLineNumber + 3);
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
            MacroLineNumber = (ushort)(MacroLineNumber + 3);
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

        private void BrowseAFile()
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
                Progress.Visible = true;

                // View captured image
                pattern_field.Image = img_capture_undist.Bitmap;
                pattern_field.Refresh();

                // Clear listbox
                processLog.Items.Clear();

                // Disable timer
                timerDROupdate.Enabled = false;

                //
                stopDetection.Enabled = true;
                startDetection.Enabled = false;
                turnCamera.Enabled = false;
                captureImg.Enabled = false;

                // Reset first item flag
                first_item = true;

                // Reset macro combine flag
                CombineFlag = false;

                // Init new mach3 macro01
                if (macroData != null)
                    macroData.Close();
                macroData = new StreamWriter(Path.Combine(macroDirectory, @"M999.m1s"));

                // Reload XML
                SysData = XDocument.Load(systemPath);
                DispensingData = XDocument.Load(DataPath);

                // Update working status
                StatusLabel.Text = "Đang quét ảnh...";
                pattern_field.Refresh(); // to remove the old text on camera window
                StatusLabel.Refresh();

                // Start detecting
                DetectClone();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void stopDetection_Click(object sender, EventArgs e)
        {
            Progress.Visible = false;

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
                        StatusLabel.Text = "Camera Bật";
                        StatusLabel.Refresh();

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
                            CvInvoke.Imwrite("result/pattern_field.jpg", img_capture_undist);

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
                            StatusLabel.Text = "Đã Chụp Ảnh";
                            StatusLabel.Refresh();

                            // 
                            switchCamera = false;
                        }
                        else MessageBox.Show("Chưa Mở Camera", "CVEye");
                    }
                    else
                    {
                        //status_label.Text = "No Camera Data";
                        StatusLabel.Text = "Không Có Ảnh";
                        StatusLabel.Refresh();
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
            //
            CombineFlag = true;

            //
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

                        using (macroData = new StreamWriter(Path.Combine(macroDirectory, @"M999.m1s")))
                        {
                            // Load changing prosition coordinate
                            string x = SysData.Element("System").Element("MainWindow").Element("MaintanancePosition").Attribute("x").Value.ToString();
                            string y = SysData.Element("System").Element("MainWindow").Element("MaintanancePosition").Attribute("y").Value.ToString();
                            string z = SysData.Element("System").Element("MainWindow").Element("MaintanancePosition").Attribute("z").Value.ToString();
                            string speed = "5000";
                            macroData.WriteLine("Code \"G90 G54 G01 Z" + z + " F" + speed + "\"");
                            macroData.WriteLine("Code \"X" + x + " Y" + y + "\"");
                            macroData.Close();
                        }
                        scriptObject.Code("M999");
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

                            StatusLabel.Text = "Đang sơn...";
                            StatusLabel.Refresh();
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

        private void RadioOneDrop_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioOneDrop.Checked)
            {
                ValveOneClick.Text = "Xả sơn";

                GetMach3Instance();

                if (scriptObject != null)
                {
                    using (macroData = new StreamWriter(Path.Combine(macroDirectory, @"M92.m1s")))
                    {
                        // Select jetting mode
                        JettingModeSelectingMacro(1, macroData);
                        macroData.WriteLine("Sleep 100");

                        macroData.Close();
                    }
                    scriptObject.Code("M92");
                }
            }
        }

        private void RadioContDrop_CheckedChanged(object sender, EventArgs e)
        {
            if (RadioContDrop.Checked)
            {
                GetMach3Instance();

                if (scriptObject != null)
                {
                    using (macroData = new StreamWriter(Path.Combine(macroDirectory, @"M92.m1s")))
                    {
                        // Select jetting mode
                        JettingModeSelectingMacro(4, macroData);
                        macroData.WriteLine("Sleep 100");

                        macroData.Close();
                    }
                    scriptObject.Code("M92");
                }
            }
        }

        private void ValveOneClick_Click(object sender, EventArgs e)
        {
            if (RadioContDrop.Checked || RadioOneDrop.Checked)
            {
                GetMach3Instance();

                if (scriptObject != null)
                {
                    // Build dot-on-demand macro
                    using (macroData = new StreamWriter(Path.Combine(macroDirectory, @"M92.m1s")))
                    {
                        // Select channels
                        short channel = (short)(GetValveNum(item_color.Text));
                        macroData.WriteLine("DeactivateSignal(OUTPUT4)");
                        macroData.WriteLine("DeactivateSignal(OUTPUT5)");
                        macroData.WriteLine("ActivateSignal(OUTPUT" + channel.ToString() + ")");
                        macroData.WriteLine("ActivateSignal(OUTPUT7)");
                        macroData.WriteLine("Sleep 100");
                        macroData.WriteLine("DeactivateSignal(OUTPUT7)");

                        macroData.Close();
                    }
                    scriptObject.Code("M92");

                    if (RadioContDrop.Checked)
                        ValveOneClick.Text = (ValveOneClick.Text == "Xả sơn") ? "Dừng xả" : "Xả sơn";
                }
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
            
        }

        private void paintingPointToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ExitMainForm()
        {
            Close();
        }

        private void OpenPaintingPointWindow()
        {
            ConfigPaintingPoints Con_Painting_Point = new ConfigPaintingPoints();
            Con_Painting_Point.ShowDialog();
        }
        #endregion

        #region Other events

        private void Set_Changed(object sender, EventArgs e)
        {
            try
            {
                // Reload objects set data
                LoadSetData();

                // Read Hough transform data
                ReadHoughData(itemsSet.Text);

                // Read Pers transform data
                ReadPersData(itemsSet.Text);

                // View template image
                templateField.Image = tmpBgr.Bitmap;
                templateField.Refresh();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Name_Changed(object sender, EventArgs e)
        {
            try
            {
                // Reload object data
                LoadObjectData();

                // Reload painting points from Xml
                getItem = DispensingData.Element("Field")
                    .Elements("Item")
                    .Where(x => x.Element("Name").Value == tmp_item_name.Text)
                    .Single();

                // View template image
                templateField.Image = tmpBgr.Bitmap;
                templateField.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ColorChanged(object sender, EventArgs e)
        {
            try
            {
                // Reload camera origin offset
                LoadOriginal(item_color.Text);

                // Reload selecting valve piston macro
                ResetSelectingPiston();

                // Update valve number
                valveNum.Text = (GetValveNum(item_color.Text) - 3).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ResetSelectingPiston()
        {
            GetMach3Instance();

            if (scriptObject != null)
            {
                using (macroData = new StreamWriter(Path.Combine(macroDirectory, @"M999.m1s")))
                {
                    // Select jetting mode
                    macroData.WriteLine("DeactivateSignal(OUTPUT4)");
                    macroData.WriteLine("DeactivateSignal(OUTPUT5)");

                    macroData.Close();
                }
                scriptObject.Code("M999");
            }
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

            KeyPreview = true;
        }

        // Hotkey
        private void CVEye_KeyDown(object sender, KeyEventArgs e)
        {
            //
            if (e.KeyCode == Keys.Down)
            {
                //ValveOneClick.PerformClick();
            }

            //
            if (e.Control == true && e.KeyCode == Keys.V)
            {
                //ValveSwitching.PerformClick();
            }
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
                    LowSpeedMode();
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

        /// <summary>
        /// Find object (chess) in sample image and extract to raw template
        /// </summary>
        private void ExtractChess()
        {
            try
            {
                using (Image<Bgr, byte> SampleImage = img_capture_undist.Clone())
                {
                    //
                    Mat imgGray = Frame_Analyze();

                    // ROI radius
                    int RawRoiRa = (int)RawROISize.Value;
                    CircleF RawCircle;

                    // Circle Hough Transform
                    CircleF[] img_circles = CvInvoke.HoughCircles(
                        imgGray, // Array of circles data
                        HoughType.Gradient,
                        1.7,
                        100,
                        60,
                        45,
                        (int)TmpMinRa.Value,
                        (int)TmpMaxRa.Value);

                    RawCircle = img_circles[0];

                    SampleImage.Draw(RawCircle, new Bgr(Color.GreenYellow), 3);
                    SampleImage.Draw(new Rectangle(
                        new Point((int)RawCircle.Center.X - RawRoiRa, (int)RawCircle.Center.Y - RawRoiRa),
                        new Size(2 * RawRoiRa, 2 * RawRoiRa)), new Bgr(Color.Blue), 3);

                    pattern_field.Image = SampleImage.Bitmap;
                    pattern_field.Refresh();

                    // Get raw roi from image
                    Rectangle roi_rec = new Rectangle(
                        new Point((int)RawCircle.Center.X - RawRoiRa, (int)RawCircle.Center.Y - RawRoiRa),
                        new Size(2 * RawRoiRa, 2 * RawRoiRa));
                    CvInvoke.cvSetImageROI(img_capture_undist.Ptr, roi_rec);
                    img_capture_undist.ROI = CvInvoke.cvGetImageROI(img_capture_undist.Ptr);

                    // Resize roi to roi standard size and save to drive
                    CvInvoke.Resize(img_capture_undist, img_capture_undist, new Size(430, 430));
                    CvInvoke.Imwrite("data/tmp_" + RawRoiRa.ToString() + "_" + RawCircle.Radius.ToString()
                        + DateTime.Now.ToFileTime() + ".jpg", img_capture_undist);
                    CvInvoke.cvResetImageROI(img_capture_undist.Ptr);

                    imgGray.Dispose();
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void buildTemplate_Click(object sender, EventArgs e)
        {
            ExtractChess();
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
                            159,
                            4.5);

                        CvInvoke.Imwrite("result/bin" + DateTime.Now.ToFileTime() + ".jpg", srcTmp);
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

        private void CamCompensate_Click(object sender, EventArgs e)
        {
            using (Image<Bgr, byte> drawImg = img_capture_undist.Clone())
            {
                //
                PixelsCompensation(drawImg);

                //
                Draw_Grid(drawImg);

                //
                pattern_field.Image = drawImg.Bitmap;
                pattern_field.Refresh();

                //
                CvInvoke.Imwrite("result/comResult.jpg", drawImg);
            }
        }

        private void CamCalibrate_Click(object sender, EventArgs e)
        {
            CalibrationStart();
        }

        private void SavePCData_Click(object sender, EventArgs e)
        {
            // Reload XML file
            SysData = XDocument.Load("_system.xml");

            // Painting Condition node
            XElement PaintingCondition = SysData.Element("System").Element("MainWindow").Element("PaintingCondition");
            PaintingCondition.Attribute("xySpeed").Value        = xySpeed.Text;
            PaintingCondition.Attribute("zSpeed").Value         = zSpeed.Text;
            PaintingCondition.Attribute("zSafe").Value          = zSafe.Text;
            PaintingCondition.Attribute("zReturn").Value        = zReturn.Text;
            PaintingCondition.Attribute("zDrip").Value          = zDrip.Text;
            PaintingCondition.Attribute("offset").Value         = deepOffset.Text;
            PaintingCondition.Attribute("circleSpeed").Value    = circleSpeed.Text;

            // Offset node
            XElement _Offset = SysData.Element("System").Element("MainWindow").Element("Offset");
            for (int i = 0; i < 5; i++)
            { 
                string Gxx = "G" + (54 + i).ToString();
                _Offset.Element(Gxx).Attribute("x").Value = OffsetCoor.Rows[i].Cells[1].Value.ToString();
                _Offset.Element(Gxx).Attribute("y").Value = OffsetCoor.Rows[i].Cells[2].Value.ToString();
                _Offset.Element(Gxx).Attribute("z").Value = OffsetCoor.Rows[i].Cells[3].Value.ToString();

            }

            SysData.Save("_system.xml");
        }

        private void ResetPCData_Click(object sender, EventArgs e)
        {
            SysData = XDocument.Load(systemPath);
            XElement MainWindow = SysData.Element("System").Element("MainWindow");

            // Load Painting Conditions
            xySpeed.Text        = MainWindow.Element("PaintingCondition").Attribute("xySpeed").Value;
            zSpeed.Text         = MainWindow.Element("PaintingCondition").Attribute("zSpeed").Value;
            zSafe.Text          = MainWindow.Element("PaintingCondition").Attribute("zSafe").Value;
            zReturn.Text        = MainWindow.Element("PaintingCondition").Attribute("zReturn").Value;
            zDrip.Text          = MainWindow.Element("PaintingCondition").Attribute("zDrip").Value;
            deepOffset.Text     = MainWindow.Element("PaintingCondition").Attribute("offset").Value;
            circleSpeed.Text    = MainWindow.Element("PaintingCondition").Attribute("circleSpeed").Value;

            // Load Offset values
            XElement Offset = MainWindow.Element("Offset");
            OffsetCoor.Rows.Clear();
            for (int i = 0; i < 5; i++)
            {
                string Gxx = "G" + (54 + i).ToString();
                OffsetCoor.Rows.Add(Gxx,
                    Offset.Element(Gxx).Attribute("x").Value,
                    Offset.Element(Gxx).Attribute("y").Value,
                    Offset.Element(Gxx).Attribute("z").Value);
            }
        }

        private void SaveCamData_Click(object sender, EventArgs e)
        {
            SysData = XDocument.Load(systemPath);
            XElement CalibrationParameter = SysData.Element("System").Element("MainWindow").Element("CalibrationParameter");

            CalibrationParameter.Attribute("calib_xOffset").Value = calib_xOffset.Text;
            CalibrationParameter.Attribute("calib_yOffset").Value = calib_yOffset.Text;
            CalibrationParameter.Attribute("calib_cornerWidth").Value = calib_cornerWidth.Text;
            CalibrationParameter.Attribute("calib_cornerHeight").Value = calib_cornerHeight.Text;
            CalibrationParameter.Attribute("calib_noOfSample").Value = calib_noOfSample.Text;
            CalibrationParameter.Attribute("calib_squareSize").Value = calib_squareSize.Text;
            SysData.Element("System").Element("CameraCalibrationWindow").Element("CameraID").Attribute("ID").Value = camID.Text;

            // Save data
            SysData.Save("_system.xml");

        }

        private void ImgProcSave_Click(object sender, EventArgs e)
        {
            SysData = XDocument.Load(systemPath);
            XElement ImageProcessing = SysData.Element("System").Element("ImageProcessingWindow");

            //
            ImageProcessing.Element("MachingCorrection").Attribute("cannyThresh").Value = cannyThresh.Value.ToString();
            ImageProcessing.Element("MachingCorrection").Attribute("correctionRange").Value = correctionRange.Value.ToString();
            ImageProcessing.Element("MachingCorrection").Attribute("ErrConstraint").Value = ErrConstraint.Value.ToString();

            //
            SaveHoughParameters(itemsSet.Text);

            //
            ImageProcessing.Element("ImageFiltering").Attribute("gaussian_sig").Value = gaussian_sig.Value.ToString();
            ImageProcessing.Element("ImageFiltering").Attribute("G_blur").Value = ((G_blur.CheckState == CheckState.Checked) ? 1 : 0).ToString();
            ImageProcessing.Element("ImageFiltering").Attribute("BlockSize").Value = BlockSize.Value.ToString();

            //
            SavePersParameters(itemsSet.Text);

            // Save document
            SysData.Save("_system.xml");
            SysData = XDocument.Load(systemPath);
        }

        private void SaveHoughParameters(string value)
        {
            XElement ImageProcessing = SysData.Element("System").Element("ImageProcessingWindow");

            XElement SetHoughData = ImageProcessing;

            switch (value)
            {
                case "Cờ 31":
                    SetHoughData = ImageProcessing.Element("HoughCirclesDetector").Element("Chess31");
                    break;
                case "Cờ 29":
                    SetHoughData = ImageProcessing.Element("HoughCirclesDetector").Element("Chess29");
                    break;
            }

            SetHoughData.Attribute("dp").Value              = houge_dp.Value.ToString();
            SetHoughData.Attribute("minDist").Value         = houge_minDist.Value.ToString();
            SetHoughData.Attribute("houge_param1").Value    = houge_param1.Value.ToString();
            SetHoughData.Attribute("houge_param2").Value    = houge_param2.Value.ToString();
            SetHoughData.Attribute("min_ra").Value          = min_ra.Value.ToString();
            SetHoughData.Attribute("max_ra").Value          = max_ra.Value.ToString();
        }

        private void SavePersParameters(string value)
        {
            XElement ImageProcessing = SysData.Element("System").Element("ImageProcessingWindow");

            XElement SetPersData = ImageProcessing;

            switch (value)
            {
                case "Cờ 31":
                    SetPersData = ImageProcessing.Element("PSTTransform").Element("Chess31");
                    break;
                case "Cờ 29":
                    SetPersData = ImageProcessing.Element("PSTTransform").Element("Chess29");
                    break;
            }

            SetPersData.Attribute("edge1").Value = cnl1.Value.ToString();
            SetPersData.Attribute("edge2").Value = cnl2.Value.ToString();
            SetPersData.Attribute("edge3").Value = cnl3.Value.ToString();
            SetPersData.Attribute("edge4").Value = cnl4.Value.ToString();
        }

        // Check the template size and ROI size to resize the template
        private void CheckTmpSize()
        {
            if (tmp_raw.Width > RoiSize)
            {
                // Resize template image to match the ROI size
                CvInvoke.Resize(tmp_raw, tmp_raw, new Size(RoiSize, RoiSize));
            }
        }

        private void ZDripEqualReturn_CheckStateChanged(object sender, EventArgs e)
        {
            if (ZDripEqualReturn.CheckState == CheckState.Checked)
                zDrip.Text = zReturn.Text;
        }

        private void zReturn_TextChanged(object sender, EventArgs e)
        {
            if (ZDripEqualReturn.CheckState == CheckState.Checked)
                zDrip.Value = zReturn.Value;
        }

        private void getOriginal_Click(object sender, EventArgs e)
        {
            cameraOriginOffset = false;

            SysData = XDocument.Load(systemPath);
            XElement MainWindow = SysData.Element("System").Element("MainWindow");

            MainWindow.Element("Offset").Element("G54").Attribute("x").Value = xdelta.ToString();
            MainWindow.Element("Offset").Element("G54").Attribute("y").Value = ydelta.ToString();

            // Save document
            SysData.Save(systemPath);

            using (macroData = new StreamWriter(Path.Combine(macroDirectory, @"M999.m1s")))
            {
                // Build temporary camera origin
                macroData.WriteLine("Code \"G90 G54 G01 X" + (cameraTempOriX + xdelta) + " Y" + (cameraTempOriY + ydelta) + " F5000\"");
                CameraData.Items.Add("Exact origin is exported.");

                macroData.Close();
            }


        }

        private void GotoOriginal_Click(object sender, EventArgs e)
        {
            SysData = XDocument.Load(systemPath);

            double alpha = double.Parse(SysData.Element("System").Element("CameraCalibrationWindow").
                Element("FOVRotationAngle").Attribute("alpha").Value);

            // Create a macro to get Camera origin
            using (macroData = new StreamWriter(Path.Combine(macroDirectory, @"M999.m1s")))
            {
                //-------------On the camera frame-------------

                int _position = int.Parse(CornerPos.Text);

                double xCom = corner_set[_position].X;
                double yCom = corner_set[_position].Y;

                xCom = xCompensate(corner_set[_position]);
                yCom = yCompensate(corner_set[_position]);

                // Global painting points rotation
                PointF RotatedPoint = new PointF((float)xCom, (float)yCom);
                RotatedPoint = RotateFOV(RotatedPoint, alpha);

                //-------------On the machine frame-------------
                double cen_X = Math.Round(RotatedPoint.X * _accuracy, 2);
                double cen_Y = Math.Round(-RotatedPoint.Y * _accuracy, 2);

                //
                XElement MainWindow = SysData.Element("System").Element("MainWindow");

                // Start origin setting
                cameraOriginOffset = true;

                cameraTempOriX = cen_X;
                cameraTempOriY = cen_Y;

                LoadOriginal(item_color.Text);
                cameraTempOriX = cen_X + _xoffset;
                cameraTempOriY = cen_Y - _yoffset;

                // Build temporary camera origin
                macroData.WriteLine("Code \"G90 G54 G01 X" + cameraTempOriX + " Y" + cameraTempOriY + " F5000\"");
                macroData.WriteLine("Code \"Z12.1\"");

                CameraData.Items.Add("Temporary center of grid is exported.");

                macroData.Close();
            }

            GetMach3Instance();

            if (scriptObject != null)
            {
                scriptObject.Code("M999");
            }
        }

        private void OpenImage_Click(object sender, EventArgs e)
        {
            BrowseAFile();
        }

        private void PaintingPointIcon_Click(object sender, EventArgs e)
        {
            OpenPaintingPointWindow();
        }

        // Reload template
        private void ViewTemplate(string set, string value)
        {
            switch (set)
            {
                case "Cờ 31":
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
                    break;
                case "Cờ 29":
                    switch (value)
                    {
                        case "Tướng 01":
                            tmp_raw = CvInvoke.Imread("pt_data_8bit/03/cc31.jpg");
                            tmpBgr = new Image<Bgr, byte>("pattern_data/03/cc31.jpg");
                            break;
                        case "Sĩ 01":
                            tmp_raw = CvInvoke.Imread("pt_data_8bit/03/cc32.jpg");
                            tmpBgr = new Image<Bgr, byte>("pattern_data/03/cc32.jpg");
                            break;
                        case "Tượng 01":
                            tmp_raw = CvInvoke.Imread("pt_data_8bit/03/cc33.jpg");
                            tmpBgr = new Image<Bgr, byte>("pattern_data/03/cc33.jpg");
                            break;
                        case "Xe 01":
                            tmp_raw = CvInvoke.Imread("pt_data_8bit/03/cc34.jpg");
                            tmpBgr = new Image<Bgr, byte>("pattern_data/03/cc34.jpg");
                            break;
                        case "Pháo 01":
                            tmp_raw = CvInvoke.Imread("pt_data_8bit/03/cc35.jpg");
                            tmpBgr = new Image<Bgr, byte>("pattern_data/03/cc35.jpg");
                            break;
                        case "Ngựa 01":
                            tmp_raw = CvInvoke.Imread("pt_data_8bit/03/cc36.jpg");
                            tmpBgr = new Image<Bgr, byte>("pattern_data/03/cc36.jpg");
                            break;
                        case "Chốt 01":
                            tmp_raw = CvInvoke.Imread("pt_data_8bit/03/cc37.jpg");
                            tmpBgr = new Image<Bgr, byte>("pattern_data/03/cc37.jpg");
                            break;
                        case "Tướng 02":
                            tmp_raw = CvInvoke.Imread("pt_data_8bit/04/cc41.jpg");
                            tmpBgr = new Image<Bgr, byte>("pattern_data/04/cc41.jpg");
                            break;
                        case "Sĩ 02":
                            tmp_raw = CvInvoke.Imread("pt_data_8bit/04/cc42.jpg");
                            tmpBgr = new Image<Bgr, byte>("pattern_data/04/cc42.jpg");
                            break;
                        case "Tượng 02":
                            tmp_raw = CvInvoke.Imread("pt_data_8bit/04/cc43.jpg");
                            tmpBgr = new Image<Bgr, byte>("pattern_data/04/cc43.jpg");
                            break;
                        case "Xe 02":
                            tmp_raw = CvInvoke.Imread("pt_data_8bit/04/cc44.jpg");
                            tmpBgr = new Image<Bgr, byte>("pattern_data/04/cc44.jpg");
                            break;
                        case "Pháo 02":
                            tmp_raw = CvInvoke.Imread("pt_data_8bit/04/cc45.jpg");
                            tmpBgr = new Image<Bgr, byte>("pattern_data/04/cc45.jpg");
                            break;
                        case "Ngựa 02":
                            tmp_raw = CvInvoke.Imread("pt_data_8bit/04/cc46.jpg");
                            tmpBgr = new Image<Bgr, byte>("pattern_data/04/cc46.jpg");
                            break;
                        case "Chốt 02":
                            tmp_raw = CvInvoke.Imread("pt_data_8bit/04/cc47.jpg");
                            tmpBgr = new Image<Bgr, byte>("pattern_data/04/cc47.jpg");
                            break;
                    }
                    break;
            }
        }

        #endregion
    }
}
