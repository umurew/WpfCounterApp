using System.Windows.Media;

namespace WpfCounterApp
{
    public class ThemeData
    {
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public bool IsEmbedded { get; set; }
        public Uri Uri { get; set; }
        public SolidColorBrush PrimaryColor { get; set; }

        public ThemeData(string name, bool isDefault, bool isEmbedded, Uri uri, SolidColorBrush primaryColor)
        {
            Name = name;
            IsDefault = isDefault;
            IsEmbedded = isEmbedded;
            Uri = uri;
            PrimaryColor = primaryColor;
        }
    }
}