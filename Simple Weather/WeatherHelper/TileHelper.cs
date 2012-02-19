using System;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using Microsoft.Phone.Shell;
using System.Collections.Generic;

namespace WeatherHelper
{
    public static class TileHelper
    {
        static IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();

        public static string spaces(int numSpaces)
        {
            string s = "";
            while (numSpaces > 0)
            {
                s += " ";
                --numSpaces;
            }
            return s;
        }
        public static void updateTile(string backgroundPath, string frontText)
        {        
            Uri backgroundUri = new Uri(backgroundPath, UriKind.Relative);
            MemoryStream ms = new MemoryStream();
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                Grid grid = new Grid();
                StreamResourceInfo info = Application.GetResourceStream(backgroundUri);
                info.Stream.CopyTo(ms);
                /*BitmapImage imageSource = new BitmapImage(backgroundUri);
                grid.Background = new ImageBrush {
                    ImageSource = imageSource,
                    Opacity = 1,
                };*/
                WriteableBitmap imgSrc = new WriteableBitmap(1, 1);
                imgSrc.SetSource(info.Stream);
                Image img = new Image();
                img.Source = imgSrc;
                grid.Children.Add(img);
                /*TextBlock text = new TextBlock() { FontSize = 20, Foreground = new SolidColorBrush(Colors.White) };
                text.Text = frontText;
                text.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                text.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                Thickness th = new Thickness();
                th.Top = 143;
                th.Left = 133;
                text.Margin = th;
                grid.Children.Add(text);
                 */
                grid.Arrange(new Rect(0, 0, 173, 173)); // force render

                WriteableBitmap wbmp = new WriteableBitmap(173, 173);
                Canvas can = new Canvas();
                can.Background = (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];
                can.Width = 173;
                can.Height = 173;
                wbmp.Render(can, null);
                wbmp.Render(grid, null);
                wbmp.Invalidate();
                //wbmp.SaveJpeg(ms, 173, 173, 0, 100);
            });
            using (var stream = isolatedStorage.CreateFile("/Shared/ShellContent/tile.png"))
            {
                ms.CopyTo(stream);
            }
            
            TranslationHelper.Pair<string, int> title_info = TranslationHelper.getTitle();
            string tile_title = title_info.First;
            int tile_width = title_info.Second;

            string title = tile_title + spaces(tile_width - tile_title.Length - frontText.Length) + frontText;
            Uri iconUri = new Uri("isostore:/Shared/ShellContent/tile.png", UriKind.Absolute);
            ShellTile TileToFind = ShellTile.ActiveTiles.First();
            if (TileToFind != null)
            {
                StandardTileData NewTileData = new StandardTileData
                {
                    Title = title,
                    Count = 0,
                    BackgroundImage = backgroundUri,
                };
                TileToFind.Update(NewTileData);
            }
        }
    }
}
