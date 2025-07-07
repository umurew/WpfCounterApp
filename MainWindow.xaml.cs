using System.Windows;
using System.Windows.Input;

namespace WpfCounterApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ResetCounter();
        }

        private int Counter;
        private int ThemeIndex = 0;

        private void UpdateDisplay()
        {
            DisplayLabel.Content = Counter.ToString();
        }

        private void ResetCounter()
        {
            Counter = 0;
            UpdateDisplay();
        }

        private void DecreaseButton_Click(object sender, RoutedEventArgs e)
        {
            Counter = Math.Max(0, Counter - 1);
            DisplayLabel.Content = Counter.ToString();
        }

        private void IncreaseButton_Click(object sender, RoutedEventArgs e)
        {
            Counter++;
            DisplayLabel.Content = Counter.ToString();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (Counter == 0)
                return;

            DialogWindow dialogWindow = new("Are you sure you want to reset the counter?");
            dialogWindow.ShowDialog();

            if (dialogWindow.Confirmed)
                ResetCounter();
        }

        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            var resourceDictionaries = Application.Current.Resources.MergedDictionaries;
            var currentTheme = resourceDictionaries.FirstOrDefault(dictionary => dictionary.Source != null && dictionary.Source.OriginalString.Contains("/Themes/"));

            int nextThemeIndex = ThemeIndex == 0 ? ThemeIndexs.Dark : ThemeIndexs.Light;
            string themePath = ThemeIndex == 1 ? "/Themes/LightTheme.xaml" : "/Themes/DarkTheme.xaml";

            ThemeIndex = ThemeIndex == 0 ? 1 : 0;
            
            if (ThemeIndex == 0)
                ThemeButton.Content = MDL2Icons.DarkTheme;
            else
                ThemeButton.Content = MDL2Icons.LightTheme;

            if (currentTheme != null)
            {
                currentTheme.Source = new Uri(themePath, UriKind.Relative);
                return;
            }

            ResourceDictionary resourceDictionary = new ResourceDictionary
            {
                Source = new Uri(themePath, UriKind.Relative)
            };

            resourceDictionaries.Add(resourceDictionary);
        }

        private class MDL2Icons
        {
            public static readonly string LightTheme = "\uE706";
            public static readonly string DarkTheme = "\uE708";
        }

        private class ThemeIndexs
        {
            public static readonly int Light = 0;
            public static readonly int Dark = 1;
        }
    }
}