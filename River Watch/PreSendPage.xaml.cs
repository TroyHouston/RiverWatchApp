using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using River_Watch.Resources;
using System.Device.Location;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Maps.Controls;
using System.Windows.Shapes;
using System.Windows.Media;



namespace River_Watch
{

    public partial class PreSendPage : PhoneApplicationPage
    {
        MapLayer polLayer = null;
        MapOverlay polMarker = null;
        private const int mapZoomLevel = 15; //1-20, 15 is good.

        // Constructor
        public PreSendPage()
        {
            InitializeComponent();
            setMapLoc(new GeoCoordinate(-41.193599,174.932816));
            setPollutionImage(@"/Assets/placeholderPhoto.png");
            adaptToTheme((Visibility)Resources["PhoneLightThemeVisibility"] == System.Windows.Visibility.Visible);
        }

        private void adaptToTheme(bool isLight)
        {
            ImageBrush themeBackBrush = new ImageBrush();
            ImageBrush themeConfirmBrush = new ImageBrush();
            if(isLight)
            {
                themeBackBrush.ImageSource = new BitmapImage(new Uri(@"/Assets/Tiles/light.appbar.navigate.previous.png", UriKind.Relative));
                confirmButton.Background = themeConfirmBrush;
                themeConfirmBrush.ImageSource = new BitmapImage(new Uri(@"/Assets/Tiles/light.appbar.navigate.next.png", UriKind.Relative));
                backButton.Background = themeBackBrush;
            }
            else
            {
                themeBackBrush.ImageSource = new BitmapImage(new Uri(@"/Assets/Tiles/appbar.navigate.previous.png", UriKind.Relative));
                confirmButton.Background = themeConfirmBrush;
                themeConfirmBrush.ImageSource = new BitmapImage(new Uri(@"/Assets/Tiles/appbar.navigate.next.png", UriKind.Relative));
                backButton.Background = themeBackBrush;
            }
        }

        private void setMapLoc(GeoCoordinate latLong){
            miniMap.Center = latLong;
            miniMap.ZoomLevel = mapZoomLevel;

            polLayer = new MapLayer();
            polMarker = new MapOverlay();
            Canvas can = new Canvas();

            Ellipse mark = new Ellipse();
            mark.Fill = new SolidColorBrush(Colors.Red);
            mark.Opacity = 0.8;
            mark.Height = 10;
            mark.Width = 10;
            can.Children.Add(mark);

            polMarker.Content = can;
            polMarker.PositionOrigin = new Point(0.5, 0.5);
            polMarker.GeoCoordinate = latLong;
            polLayer.Add(polMarker);
            miniMap.Layers.Add(polLayer);
        }

        private void setPollutionImage(String imagePath)
        {
            pollutionPic.Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/UploadPage.xaml", UriKind.Relative));
        }

        private void tagButton_Click(object sender, RoutedEventArgs e)
        {
            

        }

    }
}