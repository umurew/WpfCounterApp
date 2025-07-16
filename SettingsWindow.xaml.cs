using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfCounterApp
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            ThemeListBox.ItemsSource = ThemeManager.AvailableThemes;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => this.DragMove();

        private void ExitButton_Click(object sender, RoutedEventArgs e) => this.Close();

        private void DeleteThemeButton_Click(object sender, RoutedEventArgs e)
        {
            MessageTextBlock.Inlines.Clear();
            ThemeData themeData = (ThemeData)ThemeListBox.SelectedItem;

            if (themeData.IsEmbedded)
                MessageTextBlock.SetTextBlockInline("You cannot delete embedded themes.", Symbols.Warning, Brushes.Orange);
            else
            {
                MessageTextBlock.SetTextBlockInline($"Custom themes are not supported yet.", Symbols.SadFace, Brushes.Blue);

                // Logic for removing the theme is not added currently because external themes are not supported yet.
                // So there is actually nothing to remove.
            }
        }

        private void ApplyThemeButton_Click(object sender, RoutedEventArgs e) => ThemeManager.LoadTheme((ThemeData)ThemeListBox.SelectedItem);

        private void EditThemeButton_Click(object sender, RoutedEventArgs e) => MessageTextBlock.SetTextBlockInline("Custom themes are not supported yet.", Symbols.SadFace, Brushes.Blue);

        private void AddThemeButton_Click(object sender, RoutedEventArgs e) => MessageTextBlock.SetTextBlockInline("Custom themes are not supported yet.", Symbols.SadFace, Brushes.Blue);

        private void DeleteThemeButton_LostFocus(object sender, RoutedEventArgs e) => MessageTextBlock.Inlines.Clear();

        private void ApplyThemeButton_LostFocus(object sender, RoutedEventArgs e) => MessageTextBlock.Inlines.Clear();

        private void EditThemeButton_LostFocus(object sender, RoutedEventArgs e) => MessageTextBlock.Inlines.Clear();

        private void AddThemeButton_LostFocus(object sender, RoutedEventArgs e) => MessageTextBlock.Inlines.Clear();
    }
}
