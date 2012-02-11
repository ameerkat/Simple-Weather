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
using System.Collections.Generic;
using System.Globalization;

namespace WeatherHelper
{
    public static class TranslationHelper
    {
        static string langCode = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

        public class Pair<T, U>
        {
            public Pair()
            {
            }

            public Pair(T first, U second)
            {
                this.First = first;
                this.Second = second;
            }

            public T First { get; set; }
            public U Second { get; set; }
        };

        // get localized tile title, can't use string resources because of
        // class reference issues so moved here
        public static Pair<string, int> getTitle()
        {
            Dictionary<string, Pair<string, int>> titleDic = new Dictionary<string, Pair<string, int>>();
            titleDic.Add("ko", new Pair<string, int>("날씨", 20));

            Pair<string, int> _default = new Pair<string, int>();
            _default.First = "Weather"; // default title in english
            _default.Second = 18;
            
            if (titleDic.ContainsKey(langCode))
            {
                return titleDic[langCode];
            }
            else
            {
                return _default;
            }
        }

        public static string translateCondition(string englishCond)
        {
            Dictionary<string, Dictionary<string, string>> d = new Dictionary<string, Dictionary<string, string>>();
            d.Add("ko", new Dictionary<string, string>());

            /*
             * Reference
            "Partly Sunny", "그름많음"
            "Scattered Thunderstorms", "흩어진 뇌우"
            "Showers", "소나기"
            "Scattered Showers", "흩어진 소나기"
            "Rain and Snow", "비또는 눈"
            "Overcast", "흐린"
            "Light Snow", "눈"
            "Freezing Drizzle", "차가운 비"
            "Chance of Rain", "비가 올 확률"
            "Sunny", "맑음"
            "Clear", "맑음"
            "Mostly Sunny", "그름조금"
            "Partly Cloudy", "그름조금"
            "Mostly Cloudy", "그름많음"
            "Chance of Storm", "폭풍우 할 확률"
            "Rain", "비"
            "Chance of Snow", "눈이 올 확률"
            "Cloudy", "흐린"
            "Mist", "안개"
            "Storm", "폭풍"
            "Thunderstorm", "뇌우"
            "Chance of TStorm", "뇌우 할 확률"
            "Sleet", "진눈깨비"
            "Snow", "눈"
            "Icy", "얼음"
            "Dust", "먼지"
            "Fog", "흐림"
            "Smoke", "연기"
            "Haze", "안개"
            "Flurries", "눈 돌풍"
            "Light Rain", "비"
            "Snow Showers", "눈 소나기"
            "Hail", "우박"
            */

            /*
             * ko - Korean
             */
            Dictionary<string, string> d_ko = d["ko"];
            d_ko.Add("Partly Sunny", "그름많음"); // mostly cloudy
            d_ko.Add("Scattered Thunderstorms", "흩어진 뇌우");
            d_ko.Add("Showers", "소나기"); // confirmed
            d_ko.Add("Scattered Showers", "흩어진 소나기"); // confirmed
            d_ko.Add("Rain and Snow", "비또는 눈"); // confirmed
            d_ko.Add("Overcast", "흐린"); //same as cloudy
            d_ko.Add("Light Snow", "눈"); // same as snow
            d_ko.Add("Freezing Drizzle", "차가운 비");
            d_ko.Add("Chance of Rain", "비가 올 확률"); // not sure
            d_ko.Add("Sunny", "맑음");
            d_ko.Add("Clear", "맑음");
            d_ko.Add("Mostly Sunny", "그름조금"); // slightly cloudy
            d_ko.Add("Partly Cloudy", "그름조금"); // slightly cloudy
            d_ko.Add("Mostly Cloudy", "그름많음");
            d_ko.Add("Chance of Storm", "폭풍우 할 확률"); // not sure
            d_ko.Add("Rain", "비");
            d_ko.Add("Chance of Snow", "눈이 올 확률"); // not sure
            d_ko.Add("Cloudy", "흐린");
            d_ko.Add("Mist", "안개");
            d_ko.Add("Storm", "폭풍");
            d_ko.Add("Thunderstorm", "뇌우");
            d_ko.Add("Chance of TStorm", "뇌우 할 확률"); // not sure
            d_ko.Add("Sleet", "진눈깨비");
            d_ko.Add("Snow", "눈");
            d_ko.Add("Icy", "얼음");
            d_ko.Add("Dust", "먼지");
            d_ko.Add("Fog", "흐림");
            d_ko.Add("Smoke", "연기");
            d_ko.Add("Haze", "안개");
            d_ko.Add("Flurries", "눈 돌풍");
            d_ko.Add("Light Rain", "비"); // not sure how to say light in this context
            d_ko.Add("Snow Showers", "눈 소나기");
            d_ko.Add("Hail", "우박");

            /*
             * return code
             */
            if(d.ContainsKey(langCode)){
                Dictionary<string, string> rd = d[langCode];
                if(rd.ContainsKey(englishCond)){
                    return rd[englishCond];
                } else {
                    return englishCond;
                }
                    
            } else {
                return englishCond;
            }
        }
    }
}
