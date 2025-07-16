using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WpfCounterApp;

namespace WpfCounterApp
{
    public static class ThemeManager
    {
        public static readonly ObservableCollection<ThemeData> AvailableThemes = new ObservableCollection<ThemeData>();
        public static readonly string DefaultThemeName = "Dark";

        private static readonly Uri[] InternalThemes = {
            new("/InternalThemes/Light.xaml", UriKind.Relative),
            new("/InternalThemes/Dark.xaml", UriKind.Relative)
        };

        public static void InitializeThemes()
        {
            AvailableThemes.Clear();

            // Add the internal themes
            foreach (Uri uri in InternalThemes)
            {
                try
                {
                    ResourceDictionary themeDictionary = (ResourceDictionary)Application.LoadComponent(uri);

                    string themeName = System.IO.Path.GetFileNameWithoutExtension(uri.OriginalString);

                    SolidColorBrush? primaryColorBrush = null;
                    if (themeDictionary.Contains("PrimaryBackgroundColor") && themeDictionary["PrimaryBackgroundColor"] is SolidColorBrush brush)
                        primaryColorBrush = brush;
                    else
                        primaryColorBrush = new SolidColorBrush(Colors.Black);

                    bool isDefault = themeName == "Dark";

                    if (isDefault)
                        themeName = $"{themeName}*";

                    AvailableThemes.Add(new ThemeData(themeName, isDefault, true, uri, primaryColorBrush));
                    // IsEmbedded is set to true cuz these themes are embedded
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"Error occured loading embedded theme from \n {uri} \n Please consider reporting bug: \n {exception.Message}", "Theme Loading Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // Add the external themes

            // The logic to load default or saved theme
            // But temporarily we will load the default cuz there are no saving logic

            LoadThemeByName(DefaultThemeName);
        }

        public static bool LoadThemeByName(string themeName)
        {
            ThemeData? themeData = AvailableThemes.FirstOrDefault(data => data.Name.StartsWith(themeName, StringComparison.OrdinalIgnoreCase));

            if (themeData != null)
            {
                LoadTheme(themeData);
                return true;
            }
            else
                return false;
        }

        public static bool LoadTheme(ThemeData themeData)
        {
            if (themeData.IsEmbedded)
            {
                ResourceDictionary resourceDictionary = (ResourceDictionary)Application.LoadComponent(themeData.Uri);

                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);

                return true;
            }
            else
                return false;
        }
    }
}
