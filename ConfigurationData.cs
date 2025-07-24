namespace WpfCounterApp
{
    public class ConfigurationData
    {
        public Language Language { get; set; } = Language.English;
        public string Theme { get; set; } = ThemeManager.DefaultThemeName;
        public ThemeData[] UserThemes { get; set; } = [];
        public string UserThemesDirectory { get; set; } = "./themes";
        public bool Topmost { get; set; } = false;
        public bool LockWindowSize { get; set; } = true;
    }
}