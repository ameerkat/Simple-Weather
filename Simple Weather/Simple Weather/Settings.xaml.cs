using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using System.Collections.Generic;
using WeatherHelper;

namespace Simple_Weather
{
    public partial class Settings : PhoneApplicationPage
    {
        static IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

        public Settings()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(SettingsPage_Loaded);
        }

        private void loadVars()
        {
            zipTextbox.Text = (string)SettingsHelper.getVar("zip");
            locationToggle.IsChecked = (bool)SettingsHelper.getVar("locationEnabled");
            zipTextbox.IsEnabled = !(bool)SettingsHelper.getVar("locationEnabled");
            tileToggle.IsChecked = (bool)SettingsHelper.getVar("tileEnabled");
            temp_format.SelectedIndex = (int)SettingsHelper.getVar("tempFormat");
            downloadToggle.IsChecked = (bool)SettingsHelper.getVar("autoloadEnabled");
            refreshTextbox.Text = ((int)SettingsHelper.getVar("refreshInterval")).ToString();
            refreshTextbox.IsEnabled = (bool)SettingsHelper.getVar("autoloadEnabled");
            
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            // @todo set all the items as callbacks on loaded instead of in the xaml
            // then we can get rid of the if(loaded) checks
            loadVars();
            // register events here to avoid confliction before loading
            locationToggle.Checked += locationToggle_Checked;
            locationToggle.Unchecked += locationToggle_Unchecked;
            downloadToggle.Checked += downloadToggle_Checked;
            downloadToggle.Unchecked += downloadToggle_Unchecked;
            refreshTextbox.TextChanged += refreshTextbox_TextChanged;
            tileToggle.Checked += tileToggle_Checked;
            tileToggle.Unchecked += tileToggle_Unchecked;
            zipTextbox.TextChanged += zipTextbox_TextChanged;
            temp_format.SelectionChanged += temp_format_SelectionChanged;
        }

        private void locationToggle_Checked(object sender, RoutedEventArgs e)
        {
            SettingsHelper.setVar("locationEnabled", locationToggle.IsChecked);
            zipTextbox.IsEnabled = false;
        }

        private void locationToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            SettingsHelper.setVar("locationEnabled", locationToggle.IsChecked);
            zipTextbox.IsEnabled = true;
        }

        private void tileToggle_Checked(object sender, RoutedEventArgs e)
        {
            SettingsHelper.setVar("tileEnabled", tileToggle.IsChecked);
            string taskName = "UpdateTile";
            string description = AppResources.task_description;
            TaskHelper.AddTask(taskName, description);
        }

        private void tileToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            SettingsHelper.setVar("tileEnabled", tileToggle.IsChecked);
            // remove the task
            string taskName = "UpdateTile";
            TaskHelper.RemoveTask(taskName);
        }

        private void downloadToggle_Checked(object sender, RoutedEventArgs e)
        {
            SettingsHelper.setVar("autoloadEnabled", downloadToggle.IsChecked);
            refreshTextbox.IsEnabled = true;
        }

        private void downloadToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            SettingsHelper.setVar("autoloadEnabled", downloadToggle.IsChecked);
            refreshTextbox.IsEnabled = false;
        }

        private void zipTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SettingsHelper.setVar("zip", zipTextbox.Text);
        }

        private void refreshTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int refreshTextboxInterval = 0;
            if (int.TryParse(refreshTextbox.Text, out refreshTextboxInterval))
            {
                SettingsHelper.setVar("refreshInterval", refreshTextboxInterval);
            }
            
        }

        private void temp_format_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SettingsHelper.setVar("tempFormat", temp_format.SelectedIndex);
        }
    }
}