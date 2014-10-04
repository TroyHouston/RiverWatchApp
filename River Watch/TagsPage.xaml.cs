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
        //Previous state
        private List<String> prevTags = new List<String>();

        public TagsPage()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //Store all the tags selected by user
            CheckBox currentCheckBoxItem = sender as CheckBox;
            String tagName = (currentCheckBoxItem.Content).ToString();
            tags.Add(tagName);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //Remove checked tags
            CheckBox currentCheckBoxItem = sender as CheckBox;
            String tagName = (currentCheckBoxItem.Content).ToString();
            tags.Remove(tagName);
        }

        public List<String> getTags()
        {
            //Returns all the tags that the user selected
            return tags;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            prevTags = new List<String>(tags);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            tags = new List<String>(prevTags);
            //Restore the UI checklist to previous state
            foreach (var child in this.LayoutGrid.Children)
            {
                if (child is CheckBox)
                {
                    var checkBox = child as CheckBox;
                    //Restore the previous state - untick all the new tags selected
                    if (!tags.Contains((checkBox.Content).ToString())){
                        checkBox.IsChecked = false;
                    }
                }
            }
        }
    }
}

