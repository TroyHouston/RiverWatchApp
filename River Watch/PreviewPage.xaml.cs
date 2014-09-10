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

namespace River_Watch
{
    public partial class PreviewPage : PhoneApplicationPage
    {   

        public PreviewPage()
        {
            InitializeComponent();

            //Set preview picture
            var currentPhotoStream = PhoneApplicationService.Current.State["currentStream"];
            Stream picture = (Stream)currentPhotoStream;
            BitmapImage bmp = new BitmapImage();
            bmp.SetSource(picture);
            previewImage.Source = bmp;
        }

        private void tagButton_Click(object sender, RoutedEventArgs e) { 
        

        }

        private void upload_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


    }
}