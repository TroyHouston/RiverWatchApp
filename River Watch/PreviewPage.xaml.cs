using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Media;

namespace River_Watch
{
    public partial class PreviewPage : PhoneApplicationPage
    {
        private Popup tagsPopUp;
        private TagsPage tagsUserControl;
        private SubmitEvent submit;
        CameraCaptureTask cameraCaptureTask;
        Stream currentPhotoStream;

        public PreviewPage()
        {
            InitializeComponent();
            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_Completed);

            //Set preview picture
            //var currentPhotoStream = PhoneApplicationService.Current.State["currentStream"];
            //picture = (Stream)currentPhotoStream;
            //BitmapImage bmp = new BitmapImage();
            //bmp.SetSource(picture);
            //previewImage.Source = bmp;

            //Set up and initialize the tags page
            tagsPopUp = new Popup();
            submit = new SubmitEvent();
            tagsPopUp.VerticalOffset = 80;
            tagsUserControl = new TagsPage();
            tagsPopUp.Child = tagsUserControl;
            //Set functionality for the buttons on the tags page
            tagsUserControl.Add.Click += (s, args) =>
            {
                tagsPopUp.IsOpen = false;
            };
            tagsUserControl.Cancel.Click += (s, args) =>
            {
                tagsPopUp.IsOpen = false;
            };
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string msg = "";

            if (NavigationContext.QueryString.TryGetValue("msg", out msg))
            {
                if (msg.Equals(Constants.MAIN_PAGE))
                {
                    // Query strings appear permanent until removed
                    // We only want this to run when we came directly from the main page
                    NavigationContext.QueryString.Remove("msg");

                    if (NavigationContext.QueryString.TryGetValue("src", out msg))
                    {
                        if (msg.Equals("camera"))
                        {
                            cameraCaptureTask.Show();
                        }
                    }

                }
            }
            else
            {
                // We only want the switching navigation to appear if the camera isn't coming up
                //Checks the current theme being used
                base.OnNavigatedTo(e);
                var theme = (Visibility)Resources["PhoneLightThemeVisibility"];
                ImageBrush themeOkayButton = new ImageBrush();
                ImageBrush themeCancelButton = new ImageBrush();
                if (theme == System.Windows.Visibility.Visible)
                {
                    // Change the UI for Light theme
                    System.Diagnostics.Debug.WriteLine("Light");
                    themeOkayButton.ImageSource = new BitmapImage(new Uri(@"/Assets/Tiles/light.appbar.check.png", UriKind.Relative));
                    themeCancelButton.ImageSource = new BitmapImage(new Uri(@"/Assets/Tiles/light.appbar.close.png", UriKind.Relative));
                    uploadButton.Background = themeOkayButton;
                    cancelButton.Background = themeCancelButton;
                }
                else
                {
                    // Change the UI for Dark theme
                    System.Diagnostics.Debug.WriteLine("Dark");
                    themeOkayButton.ImageSource = new BitmapImage(new Uri(@"/Assets/Tiles/appbar.check.png", UriKind.Relative));
                    themeCancelButton.ImageSource = new BitmapImage(new Uri(@"/Assets/Tiles/appbar.close.png", UriKind.Relative));
                    uploadButton.Background = themeOkayButton;
                    cancelButton.Background = themeCancelButton;
                }
            }
           
        }

        private void tagButton_Click(object sender, RoutedEventArgs e) {
            //Toggle popup visibility
            if (tagsPopUp.IsOpen == false)
            {
                tagsPopUp.IsOpen = true;
            }
            else {
                tagsPopUp.IsOpen = false; 
            }

        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            //Hide popup when back key pressed
            if (tagsPopUp.IsOpen)
            {
                tagsPopUp.IsOpen = false;
                e.Cancel = true;
            }
        }

        private void upload_Click(object sender, RoutedEventArgs e)
        {
            tagsPopUp.IsOpen = false;
            List<String> tags = tagsUserControl.getTags();
            //No tags selected
            if (tags.Count == 0){
                System.Diagnostics.Debug.WriteLine("No tags selected.");
                MessageBox.Show("No tags selected.");
            }
            else if (String.IsNullOrEmpty(descriptionTextBox.Text))
            {
                System.Diagnostics.Debug.WriteLine("No description given.");
                MessageBox.Show("No description given.");
            }
            else
            {
                //Test the list of tags
                for (int i = 0; i < tags.Count; i++)
                {
                    System.Diagnostics.Debug.WriteLine("tag selected: " + tags[i]);
                }
                //Submit the photo goes here along with the tags...
                System.Diagnostics.Debug.WriteLine("SENDING IMAGE TO SERVER...");

                var lat = PhoneApplicationService.Current.State["latitude"];
                var lon = PhoneApplicationService.Current.State["longitude"];

                if (lat == null || lon == null)
                {
                    MessageBox.Show("GPS coordinates were not found.");
                    return;
                }

                System.Diagnostics.Debug.WriteLine((double)lat);
                System.Diagnostics.Debug.WriteLine((double)lon);

                if (submit.send(currentPhotoStream, tags, (double)lat, (double)lon, nameTextBox.Text, descriptionTextBox.Text))
                {
                    MessageBox.Show("Sending...");
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }
            }

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            tagsPopUp.IsOpen = false;
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                BitmapImage bmp = new BitmapImage();
                currentPhotoStream = e.ChosenPhoto;
                bmp.SetSource(currentPhotoStream);
                previewImage.Source = bmp;
            }
            else
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }


    }
}