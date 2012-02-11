using System.Linq;
using System;

namespace WeatherHelper
{
    public class WeatherHelper
    {
        public static string DEGREE = "°";
        public static string BASE_ICON_PATH = "icons/";
        public static int FAREN = 0;
        public static int CELSI = 1;

        public static int f_to_c(int f)
        {
            return (int)((f - 32) * 5.0 / 9);
        }

        public static string f_to_c(string f)
        {
            try
            {
                int _f = int.Parse(f);
                return f_to_c(_f).ToString();
            }
            catch (Exception)
            {
                // nothing
                return "0";
            }
        }

        public static string getLocalImageName(string remoteImage)
        {
            if (remoteImage != null)
            {
                char[] sep1 = { '/' };
                char[] sep2 = { '.' };
                return remoteImage.Split(sep1).Last().Split(sep2).First() + ".png";
            }
            return "";
        }

        public static string encodeLatLon(double lat, double lon)
        {
            return ((int)(lat * 1000000)).ToString() + "," + ((int)(lon * 1000000)).ToString();
        }

        public static string getLocalImageNameByCondition(string condition)
        {
            string img = condition.ToLower().Replace(' ', '_') + ".png";
            // manual override
            if (img == "chance_of_showers.png")
            {
                img = "chance_of_rain.png";
            }
            else if (img == "clear.png")
            {
                img = "sunny.png";
            }
            return img;
        }

        public class CurrentConditions
        {
            public string condition { get; set; }
            public string temp_f { get; set; }
            public string temp_c { get; set; }
            public string humidity { get; set; }
            public string wind { get; set; }
            public string icon { get; set; }
            public void setByName(string name, string value)
            {
                if (name == "condition")
                {
                    this.condition = value;
                }
                else if (name == "temp_f")
                {
                    this.temp_f = value;
                }
                else if (name == "temp_c")
                {
                    this.temp_c = value;
                }
                else if (name == "humidity")
                {
                    this.humidity = value;
                }
                else if (name == "wind_condition")
                {
                    this.wind = value;
                }
                else if (name == "icon")
                {
                    this.icon = value;
                }
            }
        }

        public class ForecastConditions
        {
            public string condition { get; set; }
            public string day_of_week { get; set; }
            public string low { get; set; }
            public string high { get; set; }
            public string icon { get; set; }
            public void setByName(string name, string value)
            {
                if (name == "condition")
                {
                    this.condition = value;
                }
                else if (name == "day_of_week")
                {
                    this.day_of_week = value;
                }
                else if (name == "low")
                {
                    this.low = value;
                }
                else if (name == "high")
                {
                    this.high = value;
                }
                else if (name == "icon")
                {
                    this.icon = value;
                }
            }
        }

        // get next day
        public static string getNextDay(string dayOfWeek)
        {
            string[] daysOfTheWeek = { "sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday" };
            string refDay = dayOfWeek.ToLower();
            for (int i = 0; i < daysOfTheWeek.Length; ++i)
            {
                if (daysOfTheWeek[i] == refDay)
                {
                    return daysOfTheWeek[i + 1];
                }
            }
            return "invalid";
        }
        
    }
}
