namespace WpfCounterApp
{
    public class ThemeData
    {
        public required string Name { get; set; }
        public bool IsDefault { get; set; }
        public bool IsBuiltIn { get; set; }
        public required Uri Uri { get; set; }
        public string PrimaryColor { get; set; }
    }
}