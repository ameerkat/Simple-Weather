# Simple Weather
Simple Weather is a Windows Phone 7 application that aims to be a better 
alternative to the official weather application. Simple Weather is
location enabled, features a live tile with weather information and 
simple weather condition icons that integrate into the phones interface.

[WP7 App Store Link](http://www.windowsphone.com/en-US/apps/9ee30e8b-a9f3-4ea2-9055-34852f9a85da)

## TODO
### High Priority
* reorganize code
* consolidate all the forecast data to one screen
* fix bugs
	* detect location sets to off when visiting settings screen for first time even if enabled
	* background update doesn't seem to update the timestamp on cached data

### Low Priority
* finish Korean translation
* update screenshots
* add expiration date on the live tile info
* set title to "Weather" and integrate the temp as part of the background image on bottom right
	* not sure what's going on here, might be a race condition

## Icons
![Icon Samples](https://github.com/ameerkat/Simple-Weather/raw/master/Screenshots/icon_samples.png)
	
## Screenshots
![Live Tile English](https://github.com/ameerkat/Simple-Weather/raw/master/Screenshots/small/tileEnglish.png)
![Live Tile Korean](https://github.com/ameerkat/Simple-Weather/raw/master/Screenshots/small/tileKorean.png)
![Current Conditions English](https://github.com/ameerkat/Simple-Weather/raw/master/Screenshots/small/screen1.png)
![Current Conditions Korean](https://github.com/ameerkat/Simple-Weather/raw/master/Screenshots/small/demoKorean.png)
![Light Theme Demo](https://github.com/ameerkat/Simple-Weather/raw/master/Screenshots/small/lightTheme.png)

## Translation Instructions
Simple Weather is configured for localization, if you want to add a language
though there are a few more steps to take on top of making your own
AppResourse.resx file. In addition there is some custom translation code in
WeatherHelper.TranslationHelper which is aimed at translating the weather
condition strings and providing varying titles for the live tile based on the
phone language. If you want to add another language be sure to update 
TranslationHelper as well.

### Language Support
* English (Complete)
* Korean (In Progress)
