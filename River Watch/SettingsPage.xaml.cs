using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace River_Watch
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
            {
                IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;
            }
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("PublishConsent"))
            {
                IsolatedStorageSettings.ApplicationSettings["PublishConsent"] = false;
            }
            InitializeComponent();
            Update_Page();
        }

        private void BtnRevLoc_Click(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;
            IsolatedStorageSettings.ApplicationSettings.Save();
            Update_Page();
        }

        private void BtnRevPub_Click(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["PublishConsent"] = false;
            IsolatedStorageSettings.ApplicationSettings.Save();
            Update_Page();
        }

        private void Update_Page()
        {
            bool loc = (bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"];
            A_Loc_Text.Text = "Access Location";
            if (loc) A_Loc_Text.Text += " - Disabled";
            BtnRevLoc.IsEnabled = loc;
            
            bool pub = (bool)IsolatedStorageSettings.ApplicationSettings["PublishConsent"];
            P_Phot_Text.Text = "Pubish Photos";
            if (pub) P_Phot_Text.Text += " - Disabled";
            BtnRevPub.IsEnabled = loc;
        }
    }
}