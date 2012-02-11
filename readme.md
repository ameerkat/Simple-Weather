# Simple Weather
Simple Weather is a Windows Phone 7 application that aims to be a better 
alternative to the official weather application. Simple Weather is
location enabled, features a live tile with weather information and 
simple weather condition icons that integrate into the phones interface.

## TODO
### High Priority
* reorganize code
* consolidate all the forecast data to one screen

### Low Priority
* Finish Korean translation
* add expiration date on the live tile info
* set title to "Weather" and integrate the temp as part of the background image on bottom right
	* not sure what's going on here, might be a race condition

## Icons
![Chance of Rain](https://github.com/ameerkat/Simple-Weather/raw/master/Simple%20Weather/Simple%20Weather/icons/light/chance_of_rain.png)
	
## Guidlines
http://www.silverlightshow.net/items/Windows-Phone-7-Application-Certification-Cheat-Sheet.aspx

## Translation Instructions
Simple Weather is configured for localization, if you want to add a language
though there are a few more steps to take on top of making your own
AppResourse.resx file. In addition there is some custom translation code in
WeatherHelper.TranslationHelper which is aimed at translating the weather
condition strings and providing varying titles for the live tile based on the
phone language. If you want to add another language be sure to update 
TranslationHelper as well.