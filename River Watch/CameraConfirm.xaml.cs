using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;

namespace River_Watch
{
    public partial class CameraConfirm : PhoneApplicationPage
    {
        CameraCaptureTask cameraCaptureTask;
        Stream currentPhotoStream;

        public CameraConfirm()
        {
            InitializeComponent();

            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_Completed);
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

                    if (NavigationContext.QueryString.TryGetValue("src", out msg)) {
                        if (msg.Equals("camera")) {
                            cameraCaptureTask.Show();
                        }
                        else if (msg.Equals("folder")) { 
                            // display folder? 
                        }
                    }
                    
                }
            }
            else
            {
                // We only want the switching navigation to appear if the camera isn't coming up
                base.OnNavigatedTo(e);
            }
        }


        void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                //MessageBox.Show(e.ChosenPhoto.Length.ToString());
                BitmapImage bmp = new BitmapImage();
                currentPhotoStream = e.ChosenPhoto;
                bmp.SetSource(currentPhotoStream);
                returnedImage.Source = bmp;
            }
        }

        private void yesButton_Click(object sender, RoutedEventArgs e)
        {
            // Attempt to send the image to the server
            System.Diagnostics.Debug.WriteLine("SENDING IMAGE TO SERVER...");

            SubmitEvent submit = new SubmitEvent();
            System.Diagnostics.Debug.WriteLine(submit.createJSONSubmit());

            //Go to preview Page - passing it the picture
            PhoneApplicationService.Current.State["currentStream"] = currentPhotoStream;
            NavigationService.Navigate(new Uri("/PreviewPage.xaml", UriKind.Relative));

            //submit.send(currentPhotoStream);
        }

        private void noButton_Click(object sender, RoutedEventArgs e)
        {
            cameraCaptureTask.Show();
        }
    }
}