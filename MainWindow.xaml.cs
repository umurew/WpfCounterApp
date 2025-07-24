using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfCounterApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int Counter;

        public MainWindow()
        {
            InitializeComponent();
            InitializeCounter();

            ThemeManager.Initialize();
            ConfigurationManager.Initialize();
            ThemeManager.InitializeUserThemes();

            ThemeManager.LoadUsingName(ConfigurationManager.Data.Theme);
        }

        private void DecreaseButton_Click(object sender, RoutedEventArgs e)
        {
            Counter = Math.Max(0, Counter - 1);
            DisplayLabel.Content = Counter.ToString();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e) => this.Close();

        private void IncreaseButton_Click(object sender, RoutedEventArgs e)
        {
            Counter++;
            DisplayLabel.Content = Counter.ToString();
        }

        private void InitializeCounter()
        {
            Counter = 0;
            UpdateDisplay();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (Counter == 0) return;

            if (ResetButtonPopup.IsOpen)
            {
                ToolTipService.SetIsEnabled(ResetButton, true);
                ResetButton.Content = Symbols.Reset;
                ResetButtonPopup.IsOpen = false;

                ResetCounter();
            }
            else
            {
                ToolTipService.SetIsEnabled(ResetButton, false);
                ResetButton.Content = Symbols.QuestionMark;
                ResetButtonPopup.IsOpen = true;
            }
        }

        private void ResetButton_LostFocus(object sender, RoutedEventArgs e)
        {
            ToolTipService.SetIsEnabled(ResetButton, true);
            ResetButton.Content = Symbols.Reset;
            ResetButtonPopup.IsOpen = false;
        }

        private void ResetCounter() => InitializeCounter();

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationWindow configurationWindow = new();
            configurationWindow.ShowDialog();
        }

        private void UpdateDisplay() => DisplayLabel.Content = Counter.ToString();

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => ConfigurationManager.WriteData(ConfigurationManager.Data, ConfigurationManager.DataPath);

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            var Offset = ResetButtonPopup.HorizontalOffset;
            ResetButtonPopup.HorizontalOffset += 1;
            ResetButtonPopup.HorizontalOffset = Offset;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();
    }
}