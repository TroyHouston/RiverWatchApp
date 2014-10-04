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
    public partial class TagsPage : UserControl
    {

        private List<String> tags = new List<String>();

        public TagsPage()
        {
            InitializeComponent();
        }

        public List<String> getTags()
        {
            //Returns all the tags that the user selected
            return tags;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            tags.Clear();
            foreach (var child in this.LayoutGrid.Children)
            {
                if (child is CheckBox)
                {
                    var checkBox = child as CheckBox;
                    //Add all the tags selected 
                    if (checkBox.IsChecked == true)
                    {
                        String tagName = (checkBox.Content).ToString();
                        tags.Add(tagName);
                    }
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            //Restore UI of the tags to the previous state. 
            foreach (var child in this.LayoutGrid.Children)
            {
                if (child is CheckBox)
                {
                    var checkBox = child as CheckBox;
                    //Untick all the new tags selected
                    if (!tags.Contains((checkBox.Content).ToString()))
                    {
                        checkBox.IsChecked = false;
                    }
                }
            }
        }
    }
}

