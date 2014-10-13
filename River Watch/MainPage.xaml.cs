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

        // Stored lat:long coordinates. Updated when camera button is pressed.
        double lat, lon;
        bool locationOn;

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
                themeFolderBrush.ImageSource = new BitmapImage(new Uri(@"/Assets/Tiles/light.appbar.upload.png", UriKind.Relative));
                themeCameraBrush.ImageSource = new BitmapImage(new Uri(@"/Assets/Tiles/light.appbar.camera.png", UriKind.Relative));
                folderButton.Background = themeFolderBrush;
                cameraButton.Background = themeCameraBrush;
            }
            else
            {
                // Change the UI for Dark theme
                System.Diagnostics.Debug.WriteLine("Dark");
                themeFolderBrush.ImageSource = new BitmapImage(new Uri(@"/Assets/Tiles/appbar.upload.png", UriKind.Relative));
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
                // Asks for user privacy and location consent 
                MessageBoxResult result =
                    MessageBox.Show("This app accesses your phone's location and shares this private data. Is that ok?", "Location & Privacy", MessageBoxButton.OKCancel);

                // Set location consent
                if (result == MessageBoxResult.OK) {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = true;
                }
                else {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;
                }

                IsolatedStorageSettings.ApplicationSettings.Save();
            }

            if (IsolatedStorageSettings.ApplicationSettings.Contains("PublishConsent"))
            {
                // User has opted in or out of publishing
            }
            else
            {
                // Ask user for publishing consent 
                MessageBoxResult result =
                    MessageBox.Show("This app will publish content on your behalf. Is that ok?", "Privacy", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    IsolatedStorageSettings.ApplicationSettings["PublishConsent"] = true;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings["PublishConsent"] = false;
                }

                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }
        
        private void Camera_Click(object sender, RoutedEventArgs e)
        {
            // Clear previous geolocation stored
            PhoneApplicationService.Current.State["latitude"] = null;
            PhoneApplicationService.Current.State["longitude"] = null;

            // Get current location.
            getGeoLocation();

            if (locationOn)
                NavigationService.Navigate(new Uri("/PreviewPage.xaml?msg=" + Constants.MAIN_PAGE + "&src=" + "camera", UriKind.Relative));
        }

        private void Folder_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Background.xaml", UriKind.RelativeOrAbsolute));
        }

        // Gets the current GeoLocation. 
        private async void getGeoLocation()
        {
            if ((bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] != true || 
                (bool)IsolatedStorageSettings.ApplicationSettings["PublishConsent"] != true)
            {
                // The user has opted out of Location or Publishing.
                return;
            }

            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;
            locationOn = true;

            try
            {
                // Could make this public
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    // Always check for a new location
                    maximumAge: TimeSpan.FromMinutes(0),
                    // Give phone 20seconds to get the location.
                    timeout: TimeSpan.FromSeconds(15)
                    );

                // Save location.
                lat = geoposition.Coordinate.Latitude;
                PhoneApplicationService.Current.State["latitude"] = lat;
                lon = geoposition.Coordinate.Longitude;
                PhoneApplicationService.Current.State["longitude"] = lon;

                // Code that can be used for physical geolocation test.
                //MessageBox.Show("Geoposition found - lat: " + lat + " long: " + lon, "Location", MessageBoxButton.OK);
                
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location master switch is off
                    System.Diagnostics.Debug.WriteLine("location is disabled in phone settings.");
                    MessageBoxResult ask =
                    MessageBox.Show("Your Location seems to be turned off. Please turn it on to use this application", "Settings", MessageBoxButton.OK);
                    locationOn = false;
                    if (ask == MessageBoxResult.OK)
                    {                       
                        // What happens when you say OK. 
                    }
                }
                else
                {
                }
            }
        }

        //Nav Bar buttons
        private void navBarSettings(object sender, EventArgs e)
        {
           //Go to another page 
           NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.RelativeOrAbsolute));
        }


        private void navBarPolicy(object sender, EventArgs e)
        {

        }

    }
}