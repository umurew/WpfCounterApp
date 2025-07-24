using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace WpfCounterApp
{
    public static class ThemeManager
    {
        public static readonly ObservableCollection<ThemeData> AvailableThemes = [];
        public static readonly string DefaultThemeName = "Light";

        private static readonly Uri[] builtInThemes = {
            new("/builtInThemes/Light.xaml", UriKind.Relative),
            new("/builtInThemes/Dark.xaml", UriKind.Relative)
        };

        public static void Initialize(
            [CallerLineNumber] int _CallerLineNumber = 0,
            [CallerMemberName] string _CallerMemberName = "ThemeManager.cs")
        {
            AvailableThemes.Clear();

            foreach (Uri uri in builtInThemes)
            {
                try
                {
                    string name = Path.GetFileNameWithoutExtension(uri.OriginalString);

                    ResourceDictionary themeDictionary = (ResourceDictionary)Application.LoadComponent(uri);
                    string primaryColor = string.Empty;

                    if (themeDictionary.Contains("PrimaryBackgroundColor") && themeDictionary["PrimaryBackgroundColor"] is SolidColorBrush brush)
                        primaryColor = brush.Color.ToString();
                    else
                        primaryColor = Colors.Black.ToString();

                    bool isDefault = name == DefaultThemeName;
                    bool isBuiltIn = true;

                    AvailableThemes.Add(new ThemeData()
                    {
                        Name = name,
                        IsBuiltIn = isBuiltIn,
                        IsDefault = isDefault,
                        PrimaryColor = primaryColor,
                        Uri = uri
                    });
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"Exception occured at {_CallerMemberName} : Line {_CallerLineNumber}\n\t{exception.Message}", "Exception", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        public static void InitializeUserThemes(
            [CallerLineNumber] int _CallerLineNumber = 0,
            [CallerMemberName] string _CallerMemberName = "ThemeManager.cs")
        {
            foreach (ThemeData themeData in ConfigurationManager.Data.UserThemes)
            {
                try
                {
                    AvailableThemes.Add(themeData);
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"Exception occured at {_CallerMemberName} : Line {_CallerLineNumber}\n\t{exception.Message}", "Exception", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
        }

        public static bool LoadUsingName(string themeName)
        {
            ThemeData? themeData = AvailableThemes.FirstOrDefault(data => data.Name.StartsWith(themeName, StringComparison.OrdinalIgnoreCase));

            if (themeData is not null)
                return LoadUsingData(themeData);
            else
                return false;
        }

        public static bool LoadUsingData(ThemeData themeData,
            [CallerLineNumber] int _CallerLineNumber = 0,
            [CallerMemberName] string _CallerMemberName = "ThemeManager.cs")
        {
            try
            {
                if (themeData.IsBuiltIn)
                {
                    ResourceDictionary resourceDictionary = (ResourceDictionary)Application.LoadComponent(themeData.Uri);

                    Application.Current.Resources.MergedDictionaries.Clear();
                    Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);

                    ConfigurationManager.Data.Theme = themeData.Name;

                    return true;
                }
                //else if (!themeData.IsBuiltIn)
                //{
                //    ResourceDictionary resourceDictionary = (ResourceDictionary)Application.LoadComponent(themeData.Uri);

                //    Application.Current.Resources.MergedDictionaries.Clear();
                //    Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);

                //    ConfigurationManager.Data.Theme = themeData.Name;

                //    return true;
                //}
                //
                // At this point, i am not usre that built-in and user themes differ
                else
                    return false;
            }
            catch (Exception exception)
            {
                if (exception is ArgumentException)
                    MessageBox.Show($"ArgumentException occured at {_CallerMemberName} : Line {_CallerLineNumber}\n\t{exception.Message}", "ArgumentException", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                else if (exception is ArgumentNullException)
                    MessageBox.Show($"ArgumentNullException occured at {_CallerMemberName} : Line {_CallerLineNumber}\n\t{exception.Message}", "ArgumentNullException", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                else if (exception is DirectoryNotFoundException)
                    MessageBox.Show($"DirectoryNotFoundException occured at {_CallerMemberName} : Line {_CallerLineNumber}\n\t{exception.Message}", "DirectoryNotFoundException", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                else if (exception is IOException)
                    MessageBox.Show($"IOException occured at {_CallerMemberName} : Line {_CallerLineNumber}\n\t{exception.Message}", "IOException", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                else if (exception is NotSupportedException)
                    MessageBox.Show($"NotSupportedException occured at {_CallerMemberName} : Line {_CallerLineNumber}\n\t{exception.Message}", "NotSupportedException", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                else if (exception is PathTooLongException)
                    MessageBox.Show($"PathTooLongException occured at {_CallerMemberName} : Line {_CallerLineNumber}\n\t{exception.Message}", "PathTooLongException", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                else if (exception is UnauthorizedAccessException)
                    MessageBox.Show($"UnauthorizedAccessException occured at {_CallerMemberName} : Line {_CallerLineNumber}\n\t{exception.Message}", "UnauthorizedAccessException", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                return false;
            }
        }

        public static bool Add(ThemeData themeData,
            [CallerLineNumber] int _CallerLineNumber = 0,
            [CallerMemberName] string _CallerMemberName = "ThemeManager.cs")
        {
            bool _Debug = false;

            if (!Directory.Exists($"{ConfigurationManager.Data.UserThemesDirectory}"))
                Directory.CreateDirectory($"{ConfigurationManager.Data.UserThemesDirectory}");

            if (File.Exists(themeData.Uri.AbsolutePath))
            {
                MessageBox.Show($"A theme named \"{themeData.Name}\" already exists.\n" +
                    "You can't link two themes to the same file. Please choose a different name or remove the existing one.",
                    "Duplicate Theme Name",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                return false;
            }

            ConfigurationManager.Data.UserThemes = ConfigurationManager.Data.UserThemes.Append(themeData).ToArray();
            AvailableThemes.Add(themeData);

            // A temporary snippet to prevent exceptions.
            // :: At this point, because there is no actual content in file, i don't know if it's existance matters.
            if (!File.Exists(themeData.Uri.AbsolutePath))
                File.Create(themeData.Uri.AbsolutePath);

            if (_Debug)
            {
                string sum = "";

                sum = sum + $"AbsolutePath :: {themeData.Uri.AbsolutePath}\n";
                sum = sum + $"AbsoluteUri :: {themeData.Uri.AbsoluteUri}\n";
                sum = sum + $"Authority :: {themeData.Uri.Authority}\n";
                sum = sum + $"DnsSafeHost :: {themeData.Uri.DnsSafeHost}\n";
                sum = sum + $"Fragment :: {themeData.Uri.Fragment}\n";
                sum = sum + $"Host :: {themeData.Uri.Host}\n";
                sum = sum + $"HostNameType :: {themeData.Uri.HostNameType}\n";
                sum = sum + $"IdnHost :: {themeData.Uri.IdnHost}\n";
                sum = sum + $"IsAbsoluteUri :: {themeData.Uri.IsAbsoluteUri}\n";
                sum = sum + $"IsDefaultPort :: {themeData.Uri.IsDefaultPort}\n";
                sum = sum + $"IsFile :: {themeData.Uri.IsFile}\n";
                sum = sum + $"IsLoopback :: {themeData.Uri.IsLoopback}\n";
                sum = sum + $"IsUnc :: {themeData.Uri.IsUnc}\n";
                sum = sum + $"LocalPath :: {themeData.Uri.LocalPath}\n";
                sum = sum + $"OriginalString :: {themeData.Uri.OriginalString}\n";
                sum = sum + $"PathAndQuery :: {themeData.Uri.PathAndQuery}\n";
                sum = sum + $"Port :: {themeData.Uri.Port}\n";
                sum = sum + $"Query :: {themeData.Uri.Query}\n";
                sum = sum + $"Scheme :: {themeData.Uri.Scheme}\n";
                sum = sum + $"Segments :: {themeData.Uri.Segments}\n";

                MessageBox.Show(sum, "Add() :: ThemeData.Uri Key Pairs");
            }
            if (_Debug)
            {
                string sum = "";

                sum = sum + $"Language :: {ConfigurationManager.Data.Language}\n";
                sum = sum + $"LockWindowSize :: {ConfigurationManager.Data.LockWindowSize}\n";
                sum = sum + $"Theme :: {ConfigurationManager.Data.Theme}\n";
                sum = sum + $"Topmost :: {ConfigurationManager.Data.Topmost}\n";
                sum = sum + $"UserThemes :: {ConfigurationManager.Data.UserThemes}\n";

                foreach (ThemeData query in ConfigurationManager.Data.UserThemes)
                {
                    sum = sum + $"\tName: {query.Name}\n";
                    sum = sum + $"\t\tIsBuiltIn :: {query.IsBuiltIn}\n";
                    sum = sum + $"\t\tIsDefault :: {query.IsDefault}\n";
                    sum = sum + $"\t\tPrimaryColor :: {query.PrimaryColor}\n";
                    sum = sum + $"\t\tUri :: {query.Uri}\n";
                    sum = sum + $"\t\tAbsolutePath :: {query.Uri.AbsolutePath}\n";
                }

                sum = sum + $"UserThemesDirectory :: {ConfigurationManager.Data.UserThemesDirectory}\n";

                MessageBox.Show(sum, "Add() :: ConfigurationManager.Data Key Pairs");
            }

            // No eror-handlings because this method already covers all possibilities up.
            ThemeManager.LoadUsingData(themeData);

            return true;
        }

        public static bool Exists(string themeName)
        {
            if (AvailableThemes.ToArray().Length <= 0)
                ThemeManager.Initialize();

            ThemeData? themeData = AvailableThemes.FirstOrDefault(data => data.Name.StartsWith(themeName, StringComparison.OrdinalIgnoreCase));

            if (themeData != null)
                return true;
            else
                return false;
        }
    }
}