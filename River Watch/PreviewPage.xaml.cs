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

namespace River_Watch
{
    public partial class PreviewPage : PhoneApplicationPage
    {
 
        private Popup tagsPopUp;
        private TagsPage tags;

        public PreviewPage()
        {
            InitializeComponent();

            //Set preview picture
            var currentPhotoStream = PhoneApplicationService.Current.State["currentStream"];
            Stream picture = (Stream)currentPhotoStream;
            BitmapImage bmp = new BitmapImage();
            bmp.SetSource(picture);
            previewImage.Source = bmp;
            //Set up and initialize the tags page
            tagsPopUp = new Popup();
            tagsPopUp.VerticalOffset = 80;
            tags = new TagsPage();
            tagsPopUp.Child = tags;
            //Set functionality for the buttons on the tags page
            tags.Add.Click += (s, args) =>
            {
                tagsPopUp.IsOpen = false;
            };
            tags.Cancel.Click += (s, args) =>
            {
                tagsPopUp.IsOpen = false;
            };
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

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            tagsPopUp.IsOpen = false;
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


    }
}