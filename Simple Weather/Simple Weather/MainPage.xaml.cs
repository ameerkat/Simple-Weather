using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Microsoft.Phone.Controls;
using RestSharp;
using WeatherHelper;
using Microsoft.Phone.Shell;

namespace Simple_Weather
{
    public partial class MainPage : PhoneApplicationPage
    {
        GeoCoordinateWatcher watcher;
        IsolatedStorageSettings settings;
        IsolatedStorageFile xmlCache;
        RestRequestAsyncHandle rrah;
        bool pivotItemsCreated;
        string lastDatePivotItemsCreated;
        
        // Constructor
        public MainPage()
        {    
            //progress.IsHitTestVisible = false;
            InitializeComponent();
            rrah = null;
            settings = IsolatedStorageSettings.ApplicationSettings;
            xmlCache = IsolatedStorageFile.GetUserStoreForApplication();
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // function that returns a day of the week as a string into the localized
        // string version
        public string getLocalDay(string day)
        {
            Dictionary<string, string> td = new Dictionary<string, string>();
            // long day names
            td.Add("sunday", AppResources.day_of_week_sunday);
            td.Add("monday", AppResources.day_of_week_monday);
            td.Add("tuesday", AppResources.day_of_week_tuesday);
            td.Add("wednesday", AppResources.day_of_week_wednesday);
            td.Add("thursday", AppResources.day_of_week_thursday);
            td.Add("friday", AppResources.day_of_week_friday);
            td.Add("saturday", AppResources.day_of_week_saturday);
            // short day names
            td.Add("sun", AppResources.day_of_week_sunday);
            td.Add("mon", AppResources.day_of_week_monday);
            td.Add("tue", AppResources.day_of_week_tuesday);
            td.Add("wed", AppResources.day_of_week_wednesday);
            td.Add("thu", AppResources.day_of_week_thursday);
            td.Add("fri", AppResources.day_of_week_friday);
            td.Add("sat", AppResources.day_of_week_saturday);
            if (td.ContainsKey(day.ToLower()))
            {
                return td[day.ToLower()];
            }
            else
            {
                // error invalid day
                return "err";
            }
        }

        private void refresh_from_xml(string xml_content)
        {
            int temp_format = WeatherHelper.WeatherHelper.FAREN;
            if (settings.Contains("tempFormat"))
            {
                temp_format = (int)settings["tempFormat"];
            }
            refresh_from_xml(xml_content, temp_format);
        }

        private void create_pivot_items(int lookahead, DateTime from)
        {
            string today = from.DayOfWeek.ToString();
            if (!pivotItemsCreated || (pivotItemsCreated && lastDatePivotItemsCreated != today))
            {
                pivotItemsCreated = true;
                lastDatePivotItemsCreated = today;

                // clear old pivot items if any
                while (appPivot.Items.Count > 1)
                {
                    // allways remove at first element because they shift over
                    appPivot.Items.RemoveAt(1);
                }

                // dynamically add new pivot page

                // includes today
                int lookahead_count = 4;
                string last_day = today.ToLower();

                while (lookahead_count > 0)
                {
                    PivotItem p_temp = new PivotItem();
                    p_temp.Header = getLocalDay(last_day);
                    appPivot.Items.Add(p_temp);
                    last_day = WeatherHelper.WeatherHelper.getNextDay(last_day);
                    --lookahead_count;
                }
            }
        }

        private void refresh_from_xml(string xml_content, int temp_format)
        {
            WeatherHelper.WeatherHelper.CurrentConditions current = new WeatherHelper.WeatherHelper.CurrentConditions();
            try
            {
                XElement root = XElement.Parse(xml_content);
                IEnumerable<XElement> forecast_info = from c in root.Elements().Elements("forecast_information") select c;
                IEnumerable<XElement> current_conditions = from c in root.Elements().Elements("current_conditions").Elements() select c;
                IEnumerable<XElement> forecast_conditions = from c in root.Elements().Elements("forecast_conditions") select c;
                IEnumerable<XElement> error_check = from c in root.Elements().Elements("problem_cause") select c;
                if (error_check.Count() > 0)
                {
                    current_condition.Text = AppResources.error_xml_error;
                    return;
                }
                foreach (XElement cd in current_conditions)
                {
                    current.setByName(cd.Name.LocalName, cd.Attribute("data").Value);
                }
                string localDateString = forecast_info.Elements("forecast_date").First().Attribute("data").Value;
                //Debug.WriteLine(localDateString);
                string[] localDateSplit = localDateString.Split(new char[] {'-'});
                // parse the local date time according to forecast
                DateTime localDate = DateTime.Now;
                if (localDateSplit.Length == 3)
                {
                    int year = 0, month = 0, day = 0;
                    if (int.TryParse(localDateSplit[0], out year) && int.TryParse(localDateSplit[1], out month) && int.TryParse(localDateSplit[2], out day))
                    {
                        localDate = new DateTime(year, month, day);
                    }
                }
                
                // try to dynamically add new pivot page
                // includes today
                int lookahead_count = 4;
                create_pivot_items(lookahead_count, localDate);

                string current_temp_text = "";
                if (temp_format == WeatherHelper.WeatherHelper.FAREN)
                {
                    current_temp_text = current.temp_f + WeatherHelper.WeatherHelper.DEGREE;
                }
                else
                {
                    current_temp_text = current.temp_c + WeatherHelper.WeatherHelper.DEGREE;
                }
                current_temp.Text = current_temp_text;
                current_wind.Text = current.wind.Replace("Wind", AppResources.wind);
                current_humidity.Text = current.humidity.Replace("Humidity", AppResources.humidity);
                current_condition.Text = WeatherHelper.TranslationHelper.translateCondition(current.condition);
                var v = (Visibility)Resources["PhoneLightThemeVisibility"];
                string local_image_folder = "dark";
                if (v == Visibility.Visible)
                {
                    local_image_folder = "light";
                }
                Uri imageUri = new Uri(WeatherHelper.WeatherHelper.BASE_ICON_PATH + local_image_folder + "/" + WeatherHelper.WeatherHelper.getLocalImageName(current.icon), UriKind.Relative);
                string iconPath = WeatherHelper.WeatherHelper.BASE_ICON_PATH + "tile/" + WeatherHelper.WeatherHelper.getLocalImageName(current.icon);
                current_image.Source = new BitmapImage(imageUri);
                current_image.Height = 200;
                List<WeatherHelper.WeatherHelper.ForecastConditions> forecast_list = new List<WeatherHelper.WeatherHelper.ForecastConditions>();
                // update tile
                TileHelper.updateTile(iconPath, current_temp_text);
                foreach (XElement fc in forecast_conditions)
                {
                    IEnumerable<XElement> cond = fc.Elements();
                    WeatherHelper.WeatherHelper.ForecastConditions fc_temp = new WeatherHelper.WeatherHelper.ForecastConditions();
                    foreach (XElement cd in cond)
                    {
                        fc_temp.setByName(cd.Name.LocalName, cd.Attribute("data").Value);
                    }
                    forecast_list.Add(fc_temp);
                    foreach (PivotItem pi in appPivot.Items)
                    {
                        //Debug.WriteLine(pi.Header.ToString());
                        //Debug.WriteLine(getLocalDay(fc_temp.day_of_week.Substring(0, 3).ToLower()));
                        if (pi.Header.ToString() == getLocalDay(fc_temp.day_of_week.Substring(0, 3).ToLower()))
                        {
                            string today = localDate.DayOfWeek.ToString();
                            // debug
                            /*Debug.WriteLine(today);
                            Debug.WriteLine(fc_temp.day_of_week);*/
                            if (fc_temp.day_of_week.Substring(0, 3).ToLower() == today.Substring(0, 3).ToLower())
                            {
                                pi.Header = AppResources.day_of_week_today;
                            }
                            StackPanel sp = new StackPanel();
                            TextBlock tb_cond = new TextBlock();
                            TextBlock tb_high = new TextBlock();
                            TextBlock tb_low = new TextBlock();
                            tb_high.FontSize = 64;
                            tb_low.FontSize = 52;
                            tb_high.Foreground = Application.Current.Resources["PhoneAccentBrush"] as SolidColorBrush;
                            tb_low.Foreground = Application.Current.Resources["PhoneAccentBrush"] as SolidColorBrush;
                            tb_cond.FontSize = 24;
                            tb_high.TextAlignment = TextAlignment.Center;
                            tb_low.TextAlignment = TextAlignment.Center;
                            tb_cond.TextAlignment = TextAlignment.Center;
                            Thickness tb_cond_margin = new Thickness();
                            Thickness tb_high_margin = new Thickness();
                            tb_cond_margin.Top = 25;
                            tb_high_margin.Top = 10;
                            tb_cond.Margin = tb_cond_margin;
                            tb_high.Margin = tb_high_margin;
                            if (temp_format == WeatherHelper.WeatherHelper.FAREN)
                            {
                                tb_high.Text = fc_temp.high + WeatherHelper.WeatherHelper.DEGREE;
                                tb_low.Text = fc_temp.low + WeatherHelper.WeatherHelper.DEGREE;
                            }
                            else
                            {
                                tb_high.Text = WeatherHelper.WeatherHelper.f_to_c(fc_temp.high) + WeatherHelper.WeatherHelper.DEGREE;
                                tb_low.Text = WeatherHelper.WeatherHelper.f_to_c(fc_temp.low) + WeatherHelper.WeatherHelper.DEGREE;
                            }
                            tb_cond.Text = WeatherHelper.TranslationHelper.translateCondition(fc_temp.condition);
                            Image tb_image = new Image();
                            Uri tb_uri = null;
                            if (fc_temp.icon != null)
                            {
                                tb_uri = new Uri(WeatherHelper.WeatherHelper.BASE_ICON_PATH + local_image_folder + "/" + WeatherHelper.WeatherHelper.getLocalImageName(fc_temp.icon), UriKind.Relative);
                            }
                            else
                            {
                                //tb_uri = new Uri(WeatherHelper.WeatherHelper.BASE_ICON_PATH + local_image_folder + "/" + WeatherHelper.WeatherHelper.getLocalImageNameByCondition(fc_temp.condition), UriKind.Relative);
                            }
                            if (tb_uri != null)
                            {
                                tb_image.Source = new BitmapImage(tb_uri);
                            }
                            tb_image.Height = 200;
                            sp.Children.Add(tb_image);
                            sp.Children.Add(tb_cond);
                            sp.Children.Add(tb_high);
                            sp.Children.Add(tb_low);
                            pi.Content = sp;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                current_condition.Text = AppResources.error_general_exception + " : " + e.Message;
            }
        }

        private void execute_refresh(RestClient client, RestRequest request)
        {
            progress.Visibility = Visibility.Visible;
            // get visibility

            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = true; // enable stop
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = false; // disable refresh

            rrah = client.ExecuteAsync(request, (response) =>
            {
                if (response.ResponseStatus != ResponseStatus.Completed || response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    //current_condition.Text = "Could not load latest data, make sure you are connected to the internet.";
                    DateTime last_updated = DateTime.Now;
                    if (settings.Contains("lastUpdated"))
                    {
                         last_updated = (DateTime)settings["lastUpdated"];
                    }
                    refresh_from_cache(last_updated);
                    // lastUpdatedText.Text = AppResources.last_updated + " " + AppResources.never; // override
                } else {
                    int temp_format = WeatherHelper.WeatherHelper.FAREN;
                    if (settings.Contains("tempFormat"))
                    {
                        temp_format = (int)settings["tempFormat"];
                    }
                    current_condition.Text = response.StatusDescription;
                    var resource = response.Content;
                    // parse out the info
                    // update from info
                    if (settings.Contains("lastUpdated"))
                    {
                        settings["lastUpdated"] = DateTime.Now;
                    }
                    else
                    {
                        settings.Add("lastUpdated", DateTime.Now);
                    }
                    lastUpdatedText.Text = String.Format(AppResources.last_updated + " 0 " + AppResources.minutes_ago);
                    using (StreamWriter writer = new StreamWriter(new IsolatedStorageFileStream("xmlCache.xml", FileMode.Create, FileAccess.Write, xmlCache)))
                    {
                        writer.Write(resource);
                        writer.Close();
                    }
                    refresh_from_xml(resource, temp_format);
                }
                if (watcher != null)
                {
                    watcher.Stop();
                }
                progress.Visibility = Visibility.Collapsed;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false; // disable stop
                ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = true; // enable refresh
                rrah = null;
            });
        }

        private void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            // update when we get the position changed
            RestClient client = new RestClient();
            client.BaseUrl = "http://www.google.com";
            RestRequest request = new RestRequest();
            // encode from the latitude and longitude
            request.Resource = String.Format("ig/api?weather=,,,{0}", WeatherHelper.WeatherHelper.encodeLatLon(e.Position.Location.Latitude, e.Position.Location.Longitude));
            execute_refresh(client, request);
        }

        private void load_from_zip(string zip)
        {
            RestClient client = new RestClient();
            client.BaseUrl = "http://www.google.com";
            RestRequest request = new RestRequest();
            // encode from the latitude and longitude
            request.Resource = String.Format("ig/api?weather={0}", zip);
            execute_refresh(client, request);
        }

        private void start_weather_load(bool force = false)
        {
            // check to see if we need to load or not
            if (!force && !(bool)SettingsHelper.getVar("autoloadEnabled"))
            {
                if (settings.Contains("lastUpdated"))
                {
                    DateTime last_updated = (DateTime)settings["lastUpdated"];
                    lastUpdatedText.Text = String.Format(AppResources.last_updated + " {0} " + AppResources.minutes_ago, last_updated); // override
                    refresh_from_cache(last_updated);
                    return;
                }
                else
                {
                    return;
                }
            }
            if (settings.Contains("lastUpdated") && !force)
            {
                DateTime last_updated = (DateTime)settings["lastUpdated"];
                lastUpdatedText.Text = String.Format(AppResources.last_updated + " {0} " + AppResources.minutes_ago, last_updated); // override
                refresh_from_cache(last_updated); // automatically load from cache
                int compare = DateTime.Compare(last_updated.AddMinutes((double)((int)SettingsHelper.getVar("refreshInterval"))), DateTime.Now);
                if (compare > 0 && xmlCache.FileExists("xmlCache.xml"))
                {
                    return;
                }
            }
            if (settings.Contains("locationEnabled"))
            {
                if ((bool)settings["locationEnabled"])
                {
                    if (watcher == null)
                    {
                        watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
                        watcher.MovementThreshold = 512;
                        watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
                    }
                    watcher.Start();
                }
                else
                {
                    // load using zip code
                    if (settings.Contains("zip"))
                    {
                        string zip_code = (string)settings["zip"];
                        if (zip_code != null && zip_code != "")
                        {
                            load_from_zip(zip_code);
                        }
                        else
                        {
                            // error! can't load without zip and turned off location
                            current_condition.Text = AppResources.error_no_location_info;
                        }
                    }
                    else
                    {
                        // error! can't load without zip and turned off location
                        current_condition.Text = AppResources.error_no_location_info;
                    }
                }
            }
            else
            {
                // check for gps status
                if (watcher == null)
                {
                    watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
                    watcher.MovementThreshold = 512;
                    watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
                }
                if (watcher.Status != GeoPositionStatus.Disabled)
                {
                    watcher.Start();
                }
                else
                {
                    if (settings.Contains("zip"))
                    {
                        string zip_code = (string)settings["zip"];
                        if (zip_code != null && zip_code != "")
                        {
                            load_from_zip(zip_code);
                        }
                        else
                        {
                            // error! can't load without zip and turned off location (disabled on the device)
                            current_condition.Text = AppResources.error_no_location_disabled;
                        }
                    }
                }
            }
        }

        private void refresh_from_cache(DateTime last_updated)
        {
            // load from cache data
            if(xmlCache.FileExists("xmlCache.xml")){
                using (StreamReader reader = new StreamReader(xmlCache.OpenFile("xmlCache.xml", FileMode.Open, FileAccess.Read)))
                {
                    string xml_content = reader.ReadToEnd();
                    refresh_from_xml(xml_content);
                    lastUpdatedText.Text = String.Format(AppResources.last_updated + " {0} " + AppResources.minutes_ago, (int)(DateTime.Now - last_updated).TotalMinutes);
                }
            } else {
                current_condition.Text = "cached data does not exist, connect to the internet to get weather data.";
            }
            
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Add menu buttons


            // add the scheduled task to update the live tile
            string taskName = "UpdateTile";
            string description = AppResources.task_description;
            TaskHelper.RemoveTask(taskName);
            if ((bool)SettingsHelper.getVar("tileEnabled"))
            {
                TaskHelper.AddTask(taskName, description);
            }

            // clear tile for debugging
            /* ShellTile TileToFind = ShellTile.ActiveTiles.First();
            if (TileToFind != null)
            {
                StandardTileData NewTileData = new StandardTileData
                {
                    Title = "cleared",
                    Count = 0,
                    BackgroundImage = new Uri("", UriKind.Relative),
                    BackTitle = "",
                    BackBackgroundImage = new Uri("", UriKind.Relative),
                    BackContent = ""
                };
                TileToFind.Update(NewTileData);
            }

            // debug by launching the task right away
            ScheduledActionService.LaunchForTest(taskName, TimeSpan.FromMilliseconds(500));*/

            if (!(bool)SettingsHelper.getVar("firstRun"))
            {
                    start_weather_load();
            }
            else
            {
                SettingsHelper.setVar("firstRun", false);
                MessageBoxResult mbr = MessageBox.Show(AppResources.msg_box_first_run_content,
                    AppResources.msg_box_first_run_title, MessageBoxButton.OKCancel);
                if (mbr == MessageBoxResult.Cancel)
                {
                    SettingsHelper.setVar("locationEnabled", false);
                    NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
                }
                else
                {
                    start_weather_load();
                }
            }
        }

        private void SettingsMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        private void button_refresh_Click(object sender, EventArgs e)
        {
            start_weather_load(true);
        }

        private void refresh_stop_Click(object sender, EventArgs e)
        {
            if (rrah != null)
            {
                rrah.Abort();
                ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false; // disable stop
                ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = true; // enable refresh
            }
        }
    }
}
