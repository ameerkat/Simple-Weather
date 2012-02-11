using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.Collections.Generic;

namespace WeatherHelper
{
    public static class SettingsHelper
    {
        static IsolatedStorageSettings settings = 
            IsolatedStorageSettings.ApplicationSettings;

        public static void setVar(string varname, object value)
        {
            if (settings.Contains(varname))
            {
                settings[varname] = value;
            }
            else
            {
                settings.Add(varname, value);
            }
            settings.Save();
        }

        public static object getVar(string varname)
        {
            // dictionary for default values
            Dictionary<string, object> dd = new Dictionary<string, object>();
            dd.Add("locationEnabled", false);
            dd.Add("tileEnabled", true);
            dd.Add("zip", "");
            dd.Add("tempFormat", 0); // F by default
            dd.Add("firstRun", true);
            dd.Add("autoloadEnabled", true);
            dd.Add("refreshInterval", 20);

            if (settings.Contains(varname))
            {
                return settings[varname];
            }
            else if (dd.ContainsKey(varname))
            {
                return dd[varname];
            }
            else
            {
                return null;
            }
        }
    }
}
