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

/*
using Iisu;
using Iisu.Data;
*/

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
        /*
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
        */
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
            myMap.Mode = new RoadMode();
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
                        if (skeleton.TrackingState == SkeletonTrackingState.Tracked &&
                            skeleton.TrackingId == this.nearestId)
                        {
                            var scaledJointLeft = skeleton.Joints[JointType.HandLeft].ScaleTo(900, 540, .8f, .8f);
                            var scaledJointRight = skeleton.Joints[JointType.HandRight].ScaleTo(900, 540, .8f, .8f);


                            //SetEllipsePosition(leftEllipse, scaledJointLeft);
                            //SetEllipsePosition(rightEllipse, scaledJointRight);
                        }
                    }

                }
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
                //set to near mode
                this.Sensor.SkeletonStream.EnableTrackingInNearRange = true;
                this.Sensor.DepthStream.Range = DepthRange.Near; // Depth in near range enabled
                this.Sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                this.Sensor.SkeletonStream.Enable();

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
