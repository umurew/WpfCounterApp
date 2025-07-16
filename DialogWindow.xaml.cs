using System.Windows;
using System.Windows.Input;

namespace WpfCounterApp
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        public bool Confirmed { get; private set; } = false;
        /// This variable is set to false by default.

        public DialogWindow(string? Content)
        {
            InitializeComponent();

            if (Content != null)
                ContentTextBlock.Text = Content;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            Confirmed = true;
            this.Close();
        }

        private void DisapproveButton_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}