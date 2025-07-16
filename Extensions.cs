using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls;

namespace WpfCounterApp
{
    internal static class TextBlockExtensions
    {
        internal static void SetTextBlockInline(this TextBlock textBlock, string messageText, string? symbolString, SolidColorBrush? symbolColor)
        {
            textBlock.Inlines.Clear();

            Run symbol;
            Run text = new() { Text = $" {messageText}" };

            if (symbolString != null)
            {
                symbol = new() { Text = symbolString };

                if (symbolColor != null)
                    symbol.Foreground = symbolColor;

                textBlock.Inlines.Add(symbol);
            }

            textBlock.Inlines.Add(text);
        }
    }
}