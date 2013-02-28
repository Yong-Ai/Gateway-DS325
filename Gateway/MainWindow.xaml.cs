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


namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
    }
}
