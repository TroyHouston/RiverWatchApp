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
                MessageBoxResult result =
                    MessageBox.Show("This app accesses your phone's location & something private data. Is that ok?", "Location & Privacy", MessageBoxButton.OKCancel);

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
            getGeoLocation();

            // Clear previous geolocation stored
            PhoneApplicationService.Current.State["latitude"] = null;
            PhoneApplicationService.Current.State["longitude"] = null;

            if (locationOn)
                NavigationService.Navigate(new Uri("/CameraConfirm.xaml?msg=" + Constants.MAIN_PAGE + "&src=" + "camera", UriKind.Relative));
        }

        private void Folder_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Background.xaml", UriKind.RelativeOrAbsolute));
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
            locationOn = true;

            try
            {
                // Could make this public
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(0),
                    timeout: TimeSpan.FromSeconds(10)
                    );

                // Print Geolocation to console 
                lat = geoposition.Coordinate.Latitude;
                PhoneApplicationService.Current.State["latitude"] = lat;
                lon = geoposition.Coordinate.Longitude;
                PhoneApplicationService.Current.State["longitude"] = lon;

                System.Diagnostics.Debug.WriteLine(geoposition.Coordinate.Latitude.ToString("0.00"));
                System.Diagnostics.Debug.WriteLine(geoposition.Coordinate.Longitude.ToString("0.00"));

                MessageBox.Show("Geoposition found - lat: " + lat + " long: " + lon, "Location", MessageBoxButton.OKCancel);
                
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location master switch is off
                    System.Diagnostics.Debug.WriteLine("location  is disabled in phone settings.");
                    MessageBoxResult ask =
                    MessageBox.Show("Your Location seems to be turned off. Please turn it on to use this application", "Settings", MessageBoxButton.OK);
                    locationOn = false;
                    if (ask == MessageBoxResult.OK)
                    {                       
                        // What happens when you say OK. Preferable not go to another screen. Add a global bool?
                    }
                }
                else
                {
                    // something else happened acquring the location
                    // Simply retry?
                    //getGeoLocation();
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