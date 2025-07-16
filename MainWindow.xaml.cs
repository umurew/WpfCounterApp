using System.Windows;
using System.Windows.Input;

namespace WpfCounterApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int Counter;
        private const string DefaultThemeName = "Dark";

        // Default theme name used at the start as a constant
        // Should be in format where only file name is used without extension
        //
        // For example: "Light" instead of "Light.xaml"

        public MainWindow()
        {
            InitializeComponent();
            InitializeCounter();
            ThemeManager.InitializeThemes();
        }

        private void InitializeCounter()
        {
            Counter = 0;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            DisplayLabel.Content = Counter.ToString();
        }

        private void ResetCounter() => InitializeCounter();

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
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

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new();
            settingsWindow.ShowDialog();
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