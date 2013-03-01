using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.Maps.MapControl.WPF;


using Iisu;
using Iisu.Data;


using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using System.IO;


namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        // iisu handle
        private IHandle iisuHandle;

        // iisu device
        private IDevice device;

        // validates iisu DataHandles
        private bool m_valid = false;


        // ensures we get a new frame
        private int m_lastFrameID = -1;

        // variables we're concerned with
        private IDataHandle<float> zoomStage;
        private IDataHandle<bool> hand1_closed;
        private IDataHandle<bool> hand2_closed;

        private IDataHandle<float> h1x;
        private IDataHandle<float> h1y;

        private IDataHandle<float> h2x;
        private IDataHandle<float> h2y;
        
        private KinectSensor Sensor;

        /// <summary>
        /// There is currently no connected sensor.
        /// </summary>
        private bool isDisconnectedField = true;

        /// <summary>
        /// Any message associated with a failure to connect.
        /// </summary>
        private string disconnectedReasonField;

        /// <summary>
        /// Array to receive skeletons from sensor, resize when needed.
        /// </summary>
        private Microsoft.Kinect.Skeleton[] skeletons = new Microsoft.Kinect.Skeleton[0];

        /// <summary>
        /// Time until skeleton ceases to be highlighted.
        /// </summary>
        private DateTime highlightTime = DateTime.MinValue;

        /// <summary>
        /// The ID of the skeleton to highlight.
        /// </summary>
        private int highlightId = -1;

        /// <summary>
        /// The ID if the skeleton to be tracked.
        /// </summary>
        private int nearestId = -1;

        /// <summary>
        /// The index of the current image.
        /// </summary>
        private int indexField = 1;

        public MainWindow()
        {
            InitializeComponent();

            initDS();
            CompositionTarget.Rendering += UpdateColor;
            myMap.Mode = new RoadMode();
            stadiumImg.Margin = new Thickness(1100d, stadiumImg.Margin.Top, stadiumImg.Margin.Right, stadiumImg.Margin.Bottom);
            museumImg.Margin = new Thickness(1100d, museumImg.Margin.Top, museumImg.Margin.Right, museumImg.Margin.Bottom);
            myMap.Margin = new Thickness(90d, mapImg.Margin.Top, mapImg.Margin.Right, mapImg.Margin.Bottom);
            browser.Margin = new Thickness(1100d, browser.Margin.Top, browser.Margin.Right, browser.Margin.Bottom);
            myMap.Center = new Location(55.396172, 10.39079);
            myMap.ZoomLevel = 10.0;
        }


        protected void UpdateColor(object sender, EventArgs e)
        {
            updateDS();
        }

        void initDS()
        {
            IContext context = Iisu.Iisu.Context;

            // get iisu handle
            iisuHandle = context.CreateHandle();

            // create iisu device
            device = iisuHandle.InitializeDevice();

            try
            {
                // register event listener
                device.EventManager.RegisterEventListener("SYSTEM.Error", new OnErrorDelegate(onError));

                // launch IID script
                device.CommandManager.SendCommand("IID.loadGraph", "gateway-v2.iid");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(0);
            }

            device.Start();

        }



        private delegate void OnErrorDelegate(String name, Iisu.Error e);
        private void onError(String name, Iisu.Error e)
        {
            Console.WriteLine("OH NO!" + e.Message);
        }



        private void restaurant_Click(object sender, RoutedEventArgs e)  //click version
        {

            

        }

        
        private void rect_LeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image r = (Image)sender;
            DataObject dataObj = new DataObject(r.Tag);
            DragDrop.DoDragDrop(r, dataObj, DragDropEffects.Move);

        }



        void displayEat()
        {
            myMap.Margin = new Thickness(1100d, mapImg.Margin.Top, mapImg.Margin.Right, mapImg.Margin.Bottom);
            stadiumImg.Margin = new Thickness(1100d, stadiumImg.Margin.Top, stadiumImg.Margin.Right, stadiumImg.Margin.Bottom);
            museumImg.Margin = new Thickness(1100d, museumImg.Margin.Top, museumImg.Margin.Right, museumImg.Margin.Bottom);
            browser.Margin = new Thickness(90d, browser.Margin.Top, browser.Margin.Right, browser.Margin.Bottom);
            browser.Navigate(new Uri("http://www.just-eat.dk/area/5000-odense-c"));
        }


        void displayMuseum()
        {
            myMap.Margin = new Thickness(1100d, mapImg.Margin.Top, mapImg.Margin.Right, mapImg.Margin.Bottom);
            browser.Margin = new Thickness(1100d, browser.Margin.Top, browser.Margin.Right, browser.Margin.Bottom);
            stadiumImg.Margin = new Thickness(1100d, stadiumImg.Margin.Top, stadiumImg.Margin.Right, stadiumImg.Margin.Bottom);
            museumImg.Margin = new Thickness(90d, museumImg.Margin.Top, museumImg.Margin.Right, museumImg.Margin.Bottom);
        }


        void displayStadion()
        {
            myMap.Margin = new Thickness(1100d, mapImg.Margin.Top, mapImg.Margin.Right, mapImg.Margin.Bottom);
            museumImg.Margin = new Thickness(1100d, museumImg.Margin.Top, museumImg.Margin.Right, museumImg.Margin.Bottom);
            browser.Margin = new Thickness(1100d, browser.Margin.Top, browser.Margin.Right, browser.Margin.Bottom);
            stadiumImg.Margin = new Thickness(90d, stadiumImg.Margin.Top, stadiumImg.Margin.Right, stadiumImg.Margin.Bottom);
        }


        void displayMaps()
        {
            stadiumImg.Margin = new Thickness(1100d, stadiumImg.Margin.Top, stadiumImg.Margin.Right, stadiumImg.Margin.Bottom);
            museumImg.Margin = new Thickness(1100d, museumImg.Margin.Top, museumImg.Margin.Right, museumImg.Margin.Bottom);
            myMap.Margin = new Thickness(90d, mapImg.Margin.Top, mapImg.Margin.Right, mapImg.Margin.Bottom);
            browser.Margin = new Thickness(1100d, browser.Margin.Top, browser.Margin.Right, browser.Margin.Bottom);
            myMap.Center = new Location(55.396172, 10.39079);
            myMap.ZoomLevel = 10.0;
        }



        private void Target_Drop(object sender, DragEventArgs e)
        {
            String btnTag = (String)e.Data.GetData(typeof(String));

            string mapStr = "Map";
            string restaurantStr = "Restaurant";
            string museumStr = "Museum";
            string stadiumStr = "Stadium";

            if (mapStr.Equals(btnTag))
            {
               
                stadiumImg.Margin = new Thickness(1100d, stadiumImg.Margin.Top, stadiumImg.Margin.Right, stadiumImg.Margin.Bottom);
                museumImg.Margin = new Thickness(1100d, museumImg.Margin.Top, museumImg.Margin.Right, museumImg.Margin.Bottom);
                myMap.Margin = new Thickness(90d, mapImg.Margin.Top, mapImg.Margin.Right, mapImg.Margin.Bottom);
                browser.Margin = new Thickness(1100d, browser.Margin.Top, browser.Margin.Right, browser.Margin.Bottom);
                myMap.Center = new Location(55.396172, 10.39079);
                myMap.ZoomLevel = 10.0;
            }
            else if (restaurantStr.Equals(btnTag))
            {
                myMap.Margin = new Thickness(1100d, mapImg.Margin.Top, mapImg.Margin.Right, mapImg.Margin.Bottom);
                stadiumImg.Margin = new Thickness(1100d, stadiumImg.Margin.Top, stadiumImg.Margin.Right, stadiumImg.Margin.Bottom);
                museumImg.Margin = new Thickness(1100d, museumImg.Margin.Top, museumImg.Margin.Right, museumImg.Margin.Bottom);
                browser.Margin = new Thickness(90d, browser.Margin.Top, browser.Margin.Right, browser.Margin.Bottom);
                browser.Navigate(new Uri("http://www.just-eat.dk/area/5000-odense-c"));
            }

            else if (museumStr.Equals(btnTag))
            {
                myMap.Margin = new Thickness(1100d, mapImg.Margin.Top, mapImg.Margin.Right, mapImg.Margin.Bottom);
                browser.Margin = new Thickness(1100d, browser.Margin.Top, browser.Margin.Right, browser.Margin.Bottom);
                stadiumImg.Margin = new Thickness(1100d, stadiumImg.Margin.Top, stadiumImg.Margin.Right, stadiumImg.Margin.Bottom);
                museumImg.Margin = new Thickness(90d, museumImg.Margin.Top, museumImg.Margin.Right, museumImg.Margin.Bottom);

            }

            else if (stadiumStr.Equals(btnTag))
            {
                myMap.Margin = new Thickness(1100d, mapImg.Margin.Top, mapImg.Margin.Right, mapImg.Margin.Bottom);
                museumImg.Margin = new Thickness(1100d, museumImg.Margin.Top, museumImg.Margin.Right, museumImg.Margin.Bottom);
                browser.Margin = new Thickness(1100d, browser.Margin.Top, browser.Margin.Right, browser.Margin.Bottom);
                stadiumImg.Margin = new Thickness(90d, stadiumImg.Margin.Top, stadiumImg.Margin.Right, stadiumImg.Margin.Bottom);
            }

        }

        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            ZoomIn();
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            ZoomOut();
        }


        void ZoomIn()
        {
            myMap.ZoomLevel = myMap.ZoomLevel + 0.1f;
        }

        void ZoomOut()
        {
            myMap.ZoomLevel = myMap.ZoomLevel - 0.1f;
        }



        void overzoom(float ratio)
        {

            if (ratio < 9 && ratio > -9)
            {

                //myMap.ZoomLevel = 3 + ratio ;
                myMap.ZoomLevel  =+ 3 + ratio;
            }
        }


        void MyMap_ViewChangeOnFrame(object sender, MapEventArgs e)
        {
            //Gets the map that raised this event
            Map myMap = (Map)sender;
            //Gets the bounded rectangle for the current frame
            LocationRect bounds = myMap.BoundingRectangle;
            
        }

        private void myMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Map myMap = (Map)sender;
            //Gets the bounded rectangle for the current frame
            LocationRect bounds = myMap.BoundingRectangle;
        }


        bool overmaps = false;
        bool overfood = false;
        bool overstadium = false;
        bool overmuseum = false;
        long firstframe = -1;


        private void OnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            // Get the frame.
            using (var frame = e.OpenSkeletonFrame())
            {
                // Ensure we have a frame.
                if (frame != null)
                {
                    // Resize the skeletons array if a new size (normally only on first call).
                    if (this.skeletons.Length != frame.SkeletonArrayLength)
                    {
                        this.skeletons = new Microsoft.Kinect.Skeleton[frame.SkeletonArrayLength];
                    }

                    // Get the skeletons.
                    frame.CopySkeletonDataTo(this.skeletons);

                    // Assume no nearest skeleton and that the nearest skeleton is a long way away.
                    var newNearestId = -1;
                    var nearestDistance2 = double.MaxValue;

                    bool __skeletonPresent = false;

                    // Look through the skeletons.
                    foreach (var skeleton in this.skeletons)
                    {
                        // Only consider tracked skeletons.
                        if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            __skeletonPresent = true;
                            // Find the distance squared.
                            var distance2 = (skeleton.Position.X * skeleton.Position.X) +
                                (skeleton.Position.Y * skeleton.Position.Y) +
                                (skeleton.Position.Z * skeleton.Position.Z);

                            // Is the new distance squared closer than the nearest so far?
                            if (distance2 < nearestDistance2)
                            {
                                // Use the new values.
                                newNearestId = skeleton.TrackingId;
                                nearestDistance2 = distance2;
                            }
                        }
                    }


                    //Draw hands
                    foreach (var skeleton in this.skeletons)
                    {
                        // Only consider tracked skeletons.
                        if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            var scaledJointLeft = skeleton.Joints[JointType.HandLeft].ScaleTo(1080, 540, .5f, .5f);
                            var scaledJointRight = skeleton.Joints[JointType.HandRight].ScaleTo(1080, 540, .5f, .5f);

                            if (skeleton.Joints[JointType.HandLeft].TrackingState == JointTrackingState.Tracked &&
                                skeleton.Joints[JointType.HandRight].TrackingState == JointTrackingState.Tracked)
                            {


                                SetEllipsePosition(leftEllipse, scaledJointLeft);
                                SetEllipsePosition(rightEllipse, scaledJointRight);

                                //txtblock.Text = "x: " + scaledJointRight.Position.X + "  y: " + scaledJointRight.Position.Y +
                                // "x2: " + scaledJointLeft.Position.X + "  y2: " + scaledJointLeft.Position.Y;


                                // Y e bun la stanga
                                if (scaledJointLeft.Position.Y > 80 && scaledJointLeft.Position.Y < 200 ||
                                    scaledJointRight.Position.Y > 80 && scaledJointRight.Position.Y < 200)
                                {

                                    if (scaledJointLeft.Position.Y > 80 && scaledJointLeft.Position.Y < 200
                                        && scaledJointLeft.Position.X > 30 && scaledJointLeft.Position.X < 190)
                                    {
                                        if (overmaps)
                                        {
                                            if (frame.Timestamp - firstframe > 1000)
                                            {
                                                displayMaps();
                                            }
                                        }
                                        else
                                        {
                                            overmaps = true;
                                            firstframe = frame.Timestamp;
                                        }


                                        //txtblock.Text = frame.Timestamp.ToString();

                                    }
                                    else
                                    {
                                        overmaps = false;
                                    }


                                    if (scaledJointLeft.Position.Y > 80 && scaledJointLeft.Position.Y < 200 && 
                                        scaledJointLeft.Position.X > 280 && scaledJointLeft.Position.X < 340)
                                    {
                                        if (overmuseum)
                                        {
                                            if (frame.Timestamp - firstframe > 1000)
                                            {
                                                displayMuseum();
                                            }
                                        }
                                        else
                                        {
                                            overmuseum = true;
                                            firstframe = frame.Timestamp;
                                        }


                                        //txtblock.Text = frame.Timestamp.ToString();

                                    }
                                    else
                                    {
                                        overmuseum = false;
                                    }


                                    if (scaledJointRight.Position.X > 570 && scaledJointRight.Position.X < 720
                                        && scaledJointRight.Position.Y > 80 && scaledJointRight.Position.Y < 200)
                                    {
                                        if (overfood)
                                        {
                                            if (frame.Timestamp - firstframe > 1000)
                                            {
                                                //displayEat();
                                            }
                                        }
                                        else
                                        {
                                            overfood = true;
                                            firstframe = frame.Timestamp;
                                        }


                                        //txtblock.Text = frame.Timestamp.ToString();

                                    }
                                    else
                                    {
                                        overfood = false;
                                    }

                                    if (scaledJointRight.Position.X > 810 && scaledJointRight.Position.X < 950
                                        && scaledJointRight.Position.Y > 80 && scaledJointRight.Position.Y < 200)
                                    {
                                        if (overstadium)
                                        {
                                            if (frame.Timestamp - firstframe > 1000)
                                            {
                                                displayStadion();
                                            }
                                        }
                                        else
                                        {
                                            overstadium = true;
                                            firstframe = frame.Timestamp;
                                        }


                                        //txtblock.Text = frame.Timestamp.ToString();

                                    }
                                    else
                                    {
                                        overstadium = false;
                                    }


                                }

                                //mapsButton.

                            }
                        }

                    }

                }
            }


            //updateDS();

        }


        void updateDS()
        {
            // make sure that iid parameters are registered
            registerIIDData();

            // only update if object is active (all parameters are properly registered)
            //if (!m_valid)
            //{
            //   return;
            //}

            // the rest of the logic depends on iisu data, so we need to make sure that we have
            // already a new data frame
            int currentFrameID = device.FrameId;
            //if (currentFrameID == m_lastFrameID)
            {
                //  return;
            }

            // remember current frame id
            m_lastFrameID = currentFrameID;


            if (zoomStage.Value != 0)
            {
                overzoom(zoomStage.Value);
            }

            //txtblock.Text = zoomStage.Value.ToString();

            device.ReleaseFrame();
            device.UpdateFrame(true);
        }


        private void registerIIDData()
        {
            if (m_valid)
            {
                return;
            }

            try
            {
                zoomStage = device.RegisterDataHandle<float>("IID.Script.zoomStage");
                hand1_closed = device.RegisterDataHandle<bool>("IID.Script.hand1_closed");
                hand2_closed = device.RegisterDataHandle<bool>("IID.Script.hand2_closed");

                h1x = device.RegisterDataHandle<float>("IID.Script.h1x");
                h1y = device.RegisterDataHandle<float>("IID.Script.h1y");
                h2x = device.RegisterDataHandle<float>("IID.Script.h2x");
                h2y = device.RegisterDataHandle<float>("IID.Script.h2y");

                m_valid = zoomStage.Valid && hand1_closed.Valid && hand2_closed.Valid;
            }
            catch (Exception e)
            {
                //error.Text = "Failed to register iisu data: " + e.Message;
            }
        }



        private void SetEllipsePosition(FrameworkElement ellipse, Joint hoverJoint)
        {
            Canvas.SetLeft(ellipse, hoverJoint.Position.X);
            Canvas.SetTop(ellipse, hoverJoint.Position.Y);
        }



        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            this.InitializeNui();
        }

        private void InitializeNui()
        {
            this.UninitializeNui();

            this.isDisconnectedField = false;
            var index = 0;

            
                try
                {

                    this.Sensor = KinectSensor.KinectSensors[0];
                    this.Sensor.Start();


                }
                catch (IOException ex)
                {
                    this.Sensor = null;

                    this.isDisconnectedField = true;

                }
                catch (InvalidOperationException ex)
                {
                    this.Sensor = null;
                }



            if (this.Sensor != null)
            {

                TransformSmoothParameters smoothingParam = new TransformSmoothParameters();
                {
                    smoothingParam.Smoothing = 0.5f;
                    smoothingParam.Correction = 0.5f;
                    smoothingParam.Prediction = 0.5f;
                    smoothingParam.JitterRadius = 0.05f;
                    smoothingParam.MaxDeviationRadius = 0.04f;
                };

                //set to near mode
                this.Sensor.SkeletonStream.EnableTrackingInNearRange = true;
                this.Sensor.DepthStream.Range = DepthRange.Near; // Depth in near range enabled
                //this.Sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                this.Sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
                this.Sensor.SkeletonStream.Enable(smoothingParam);



                this.Sensor.SkeletonFrameReady += this.OnSkeletonFrameReady;

            }


        }

        /// <summary>
        /// Handle removal of Kinect sensor.
        /// </summary>
        private void UninitializeNui()
        {
            if (this.Sensor != null)
            {
                this.Sensor.SkeletonFrameReady -= this.OnSkeletonFrameReady;

                this.Sensor.Stop();

                this.Sensor = null;
            }
        }

    }
}
