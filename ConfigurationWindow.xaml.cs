using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfCounterApp
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        private bool _Initialized = false;

        public ConfigurationWindow()
        {
            InitializeComponent();
            ThemeListBox.ItemsSource = ThemeManager.AvailableThemes;

            foreach (Language language in Enum.GetValues(typeof(Language)))
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem() { Content = language };
                comboBoxItem.SetResourceReference(ComboBoxItem.ForegroundProperty, "PrimaryForegroundColor");

                if (ConfigurationManager.Data.Language == language)
                    comboBoxItem.IsSelected = true;

                LanguageComboBox.Items.Add(comboBoxItem);
            }

            _Initialized = true;

            TopmostCheckBox.IsChecked = ConfigurationManager.Data.Topmost;
            LockWindowSizeCheckBox.IsChecked = ConfigurationManager.Data.LockWindowSize;

            if (ConfigurationManager.Data.LockWindowSize)
            {
                foreach (Window window in App.Current.Windows)
                    window.ResizeMode = ResizeMode.NoResize;
            }
            else
            {
                foreach (Window window in App.Current.Windows)
                    window.ResizeMode = ResizeMode.CanResizeWithGrip;
            }
        }

        private object[] GetCallerDetails(
            [CallerLineNumber] int _CallerLineNumber = 0,
            [CallerMemberName] string _CallerMemberName = "ConfigurationWindow.cs")
        {
            return [_CallerLineNumber, _CallerMemberName];
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            var Offset = RestoreDefaultsButtonPopup.HorizontalOffset;
            RestoreDefaultsButtonPopup.HorizontalOffset += 1;
            RestoreDefaultsButtonPopup.HorizontalOffset = Offset;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void ExitButton_Click(object sender, RoutedEventArgs e) => this.Close();

        private void DeleteThemeButton_Click(object sender, RoutedEventArgs e)
        {
            object[] details = GetCallerDetails();
            int _CallerLineNumber = details[0] is int lineNumber ? lineNumber : 0;
            string _CallerMemberName = details[1] is string name ? name : "ConfigurationWindow.cs";

            MessageTextBlock.Inlines.Clear();

            if (ThemeListBox.SelectedItem is null)
            {
                MessageTextBlock.SetTextBlockInline("Select a theme first.", Symbols.Warning, Brushes.Orange);
                return;
            }

            ThemeData themeData = (ThemeData)ThemeListBox.SelectedItem;

            if (themeData.IsBuiltIn)
                MessageTextBlock.SetTextBlockInline("You cannot delete built-in themes.", Symbols.Warning, Brushes.Orange);
            else
            {
                if (!ThemeManager.Exists(themeData.Name))
                {
                    MessageTextBlock.SetTextBlockInline("Selected theme doesn't exist.", Symbols.QuestionMark, Brushes.Gray);
                    return;
                }

                try
                {
                    File.Delete(themeData.Uri.AbsolutePath);
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

                    MessageTextBlock.SetTextBlockInline("Couldn't delete theme.", Symbols.Error, Brushes.Red);
                    return;
                }

                ThemeData? defaultTheme = ThemeListBox.Items
                    .OfType<ThemeData>()
                    .FirstOrDefault(query => query.Name == ThemeManager.DefaultThemeName);

                if (defaultTheme != null)
                    ThemeListBox.SelectedItem = defaultTheme;
                else
                    MessageTextBlock.SetTextBlockInline($"Unexpected condition occured. Restarting may help?", Symbols.Warning, Brushes.Orange);

                ThemeManager.AvailableThemes.Remove(themeData);

                ConfigurationManager.Data.UserThemes = ConfigurationManager.Data.UserThemes
                    .Where(queryData => queryData.Uri.AbsolutePath != themeData.Uri.AbsolutePath)
                    .ToArray();

                ConfigurationManager.Data.Theme = ThemeManager.DefaultThemeName;
                MessageTextBlock.SetTextBlockInline($"Deleted {themeData.Name}", Symbols.Success, Brushes.Green);
            }
        }

        private void ThemeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ThemeData? themeData = (ThemeData)ThemeListBox.SelectedItem;
            if (themeData is null)
            {
                MessageTextBlock.SetTextBlockInline($"Selected theme doesn't actually exist.", Symbols.QuestionMark, Brushes.Gray);
                return;
            }

            bool result = ThemeManager.LoadUsingData(themeData);

            if (result)
                MessageTextBlock.SetTextBlockInline($"Successfully loaded {themeData.Name}", Symbols.Success, Brushes.Green);
            else
                MessageTextBlock.SetTextBlockInline($"Failed to load {themeData.Name}", Symbols.Error, Brushes.Red);
        }

        private void EditThemeButton_Click(object sender, RoutedEventArgs e) => MessageTextBlock.SetTextBlockInline("Theme Editor is not implemented yet.", Symbols.Info, Brushes.Blue);

        // AddThemeButton_Click has things to do
        private void AddThemeButton_Click(object sender, RoutedEventArgs e)
        {
            // Open theme editor window
            // Get the window arguments
            // Pass them to Add()

            ThemeManager.Add(new ThemeData
            {
                Name = "Randomm",
                IsBuiltIn = false,
                IsDefault = false,
                PrimaryColor = Colors.Azure.ToString(),
                Uri = new Uri(Path.GetFullPath($"{ConfigurationManager.Data.UserThemesDirectory}/Randomm.xaml"))
            });
        }

        private void DeleteThemeButton_LostFocus(object sender, RoutedEventArgs e) => MessageTextBlock.Inlines.Clear();

        private void EditThemeButton_LostFocus(object sender, RoutedEventArgs e) => MessageTextBlock.Inlines.Clear();

        private void AddThemeButton_LostFocus(object sender, RoutedEventArgs e) => MessageTextBlock.Inlines.Clear();

        private void RestoreDefaultsButton_Click(object sender, RoutedEventArgs e)
        {
            if (RestoreDefaultsButtonPopup.IsOpen)
            {
                ToolTipService.SetIsEnabled(RestoreDefaultsButton, true);
                RestoreDefaultsButton.Content = Symbols.RestoreDefaults;
                RestoreDefaultsButtonPopup.IsOpen = false;

                ConfigurationManager.Data = new();

                ThemeManager.LoadUsingName(ConfigurationManager.Data.Theme);
                TopmostCheckBox.IsChecked = ConfigurationManager.Data.Topmost;
                LockWindowSizeCheckBox.IsChecked = ConfigurationManager.Data.LockWindowSize;

                var mainWindow = App.Current.Windows
                    .OfType<MainWindow>()
                    .FirstOrDefault();

                if (mainWindow != null)
                {
                    mainWindow.Height = 300;
                    mainWindow.Width = 400;
                }

                var configurationWindow = App.Current.Windows
                    .OfType<ConfigurationWindow>()
                    .FirstOrDefault();

                if (configurationWindow != null)
                {
                    configurationWindow.Height = 450;
                    configurationWindow.Width = 500;
                }
            }
            else
            {
                ToolTipService.SetIsEnabled(RestoreDefaultsButton, false);
                RestoreDefaultsButton.Content = Symbols.QuestionMark;
                RestoreDefaultsButtonPopup.IsOpen = true;
            }
        }

        private void RestoreDefaultsButton_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!RestoreDefaultsButtonPopup.IsOpen) return;

            ToolTipService.SetIsEnabled(RestoreDefaultsButton, true);
            RestoreDefaultsButton.Content = Symbols.RestoreDefaults;
            RestoreDefaultsButtonPopup.IsOpen = false;
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LanguageComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content is Language language)
                ConfigurationManager.Data.Language = language;
        }

        private void LockWindowSizeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!_Initialized) return;

            ConfigurationManager.Data.LockWindowSize = true;

            foreach (Window window in App.Current.Windows)
                window.ResizeMode = ResizeMode.NoResize;
        }

        private void LockWindowSizeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!_Initialized) return;

            ConfigurationManager.Data.LockWindowSize = false;

            foreach (Window window in App.Current.Windows)
                window.ResizeMode = ResizeMode.CanResizeWithGrip;
        }

        private void TopmostCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ConfigurationManager.Data.Topmost = true;

            foreach (Window window in App.Current.Windows)
                window.Topmost = true;
        }

        private void TopmostCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ConfigurationManager.Data.Topmost = false;

            foreach (Window window in App.Current.Windows)
                window.Topmost = false;
        }
    }
}