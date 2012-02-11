using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using Microsoft.Phone.Scheduler;
using RestSharp;
using WeatherHelper;

namespace UpdateSimpleWeatherTile
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        GeoCoordinateWatcher watcher;
        IsolatedStorageSettings settings;
        private static volatile bool _classInitialized;

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
            settings = IsolatedStorageSettings.ApplicationSettings;
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        private void execute_refresh(RestClient client, RestRequest request)
        {
            client.ExecuteAsync(request, (response) =>
            {
                if (response.ResponseStatus != ResponseStatus.Completed || response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    // fail silently, no network connection
                }
                else
                {
                    var resource = response.Content;

                    /*
                     * cache results
                     */
                    if (settings.Contains("lastUpdated"))
                    {
                        settings["lastUpdated"] = DateTime.Now;
                    }
                    else
                    {
                        settings.Add("lastUpdated", DateTime.Now);
                    }
                    using (StreamWriter writer = new StreamWriter(new IsolatedStorageFileStream("xmlCache.xml", FileMode.Create, FileAccess.Write, IsolatedStorageFile.GetUserStoreForApplication())))
                    {
                        writer.Write(resource);
                        writer.Close();
                    }

                    int temp_format = WeatherHelper.WeatherHelper.FAREN;
                    if (settings.Contains("tempFormat"))
                    {
                        temp_format = (int)settings["tempFormat"];
                    }
                    WeatherHelper.WeatherHelper.CurrentConditions current = new WeatherHelper.WeatherHelper.CurrentConditions();
                    IEnumerable<XElement> current_conditions = from c in XElement.Parse(resource).Elements().Elements("current_conditions").Elements() select c;
                    IEnumerable<XElement> forecast_conditions = from c in XElement.Parse(resource).Elements().Elements("forecast_conditions") select c;
                    IEnumerable<XElement> error_check = from c in XElement.Parse(resource).Elements().Elements("problem_cause") select c;
                    if (error_check.Count() > 0)
                    {
                        NotifyComplete();
                    }
                    foreach (XElement cd in current_conditions)
                    {
                        current.setByName(cd.Name.LocalName, cd.Attribute("data").Value);
                    }
                    string iconPath = WeatherHelper.WeatherHelper.BASE_ICON_PATH + "tile/" + WeatherHelper.WeatherHelper.getLocalImageName(current.icon);
                    // update live tile
                    string current_temp_text = "";
                    if (temp_format == WeatherHelper.WeatherHelper.FAREN)
                    {
                        current_temp_text = current.temp_f + WeatherHelper.WeatherHelper.DEGREE;
                    }
                    else
                    {
                        current_temp_text = current.temp_c + WeatherHelper.WeatherHelper.DEGREE;
                    }
                    TileHelper.updateTile(iconPath, current_temp_text);
                    if (watcher != null)
                    {
                        watcher.Stop();
                    }
                }
                NotifyComplete();
            });
        }

        private void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
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


        private void start_weather_load()
        {
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
                            // silently fail
                        }
                    }
                    else
                    {
                        // error! can't load without zip and turned off location
                        // silently fail
                    }
                }
            }
            else
            {
                if (watcher == null)
                {
                    watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
                    watcher.MovementThreshold = 512;
                    watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
                }
                watcher.Start();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            //TODO: Add code to perform your task in background
           start_weather_load();
        }
    }
}