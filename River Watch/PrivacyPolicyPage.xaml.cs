using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace River_Watch
{
    public partial class PrivacyPage : PhoneApplicationPage
    {
        public PrivacyPage()
        {
            InitializeComponent();

            //Set the privacy policy text
            privacyText.Text = 
                "This is the privacy policy placeholder" +
                "This is the privacy policy placeholder." +
                " This is the privacy policy placeholder." +
                " This is the privacy policy placeholder.";
        }
    }
}