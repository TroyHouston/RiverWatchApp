using System;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using River_Watch.Resources;
using Windows.Devices.Geolocation;
using System.Windows.Media;
using System.Windows.Media.Imaging;



namespace River_Watch
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        // Asks user if app may get location data.
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //Checks the current theme being used
            base.OnNavigatedTo(e);
            var theme = (Visibility)Resources["PhoneLightThemeVisibility"];
            ImageBrush themeCameraBrush = new ImageBrush();
            ImageBrush themeFolderBrush = new ImageBrush();
            if (theme == System.Windows.Visibility.Visible)
            {
                // Change the UI for Light theme
                System.Diagnostics.Debug.WriteLine("Light");
                themeFolderBrush.ImageSource = new BitmapImage(new Uri(@"/Assets/Tiles/light.appbar.folder.open.png", UriKind.Relative));
                themeCameraBrush.ImageSource = new BitmapImage(new Uri(@"/Assets/Tiles/light.appbar.camera.png", UriKind.Relative));
                folderButton.Background = themeFolderBrush;
                cameraButton.Background = themeCameraBrush;
            }
            else
            {
                // Change the UI for Dark theme
                System.Diagnostics.Debug.WriteLine("Dark");
                themeFolderBrush.ImageSource = new BitmapImage(new Uri(@"/Assets/Tiles/appbar.folder.open.png", UriKind.Relative));
                themeCameraBrush.ImageSource = new BitmapImage(new Uri(@"/Assets/Tiles/appbar.camera.png", UriKind.Relative));
                folderButton.Background = themeFolderBrush;
                cameraButton.Background = themeCameraBrush;
            }


            if (IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
            {
                // User has opted in or out of Location
                return;
            }
            else
            {
                MessageBoxResult result =
                    MessageBox.Show("This app accesses your phone's location. Is that ok?", "Location", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK) {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = true;
                }
                else {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;
                }

                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }
        
        private void Camera_Click(object sender, RoutedEventArgs e)
        {
     
        }

        private void Folder_Click(object sender, RoutedEventArgs e)
        {
            
        }

        // Gets the current GeoLocation. Prints to console.
        private async void getGeoLocation()
        {
            if ((bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] != true)
            {
                // The user has opted out of Location.
                return;
            }

            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(0),
                    timeout: TimeSpan.FromSeconds(10)
                    );

                // Print Geolocation to console
                System.Diagnostics.Debug.WriteLine(geoposition.Coordinate.Latitude.ToString("0.00"));
                System.Diagnostics.Debug.WriteLine(geoposition.Coordinate.Longitude.ToString("0.00"));
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location master switch is off
                    System.Diagnostics.Debug.WriteLine("location  is disabled in phone settings.");
                }
                //else
                {
                    // something else happened acquring the location
                }
            }
        }


        
        

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}