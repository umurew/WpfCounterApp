using System.Runtime.CompilerServices;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace WpfCounterApp
{
    public static class ConfigurationManager
    {
        public static readonly string DataPath = "./configuration.json";
        internal static ConfigurationData Data { get; set; } = new();

        public static void Initialize()
        {
            if (!File.Exists(DataPath))
                WriteData(Data, DataPath);

            Data = ReadData(DataPath);
        }

        public static ConfigurationData ReadData(string dataPath,
            [CallerLineNumber] int _CallerLineNumber = 0,
            [CallerMemberName] string _CallerMemberName = "ThemeManager.cs")
        {
            if (!File.Exists(dataPath))
                return new ConfigurationData();

            string text = File.ReadAllText(dataPath);

            try
            {
                ConfigurationData? data = JsonSerializer.Deserialize<ConfigurationData>(text);

                if (data is null)
                    throw new NullReferenceException("Deserialization returned null.");

                if (ValidateData(data, dataPath))
                    return data;
            }
            catch (Exception exception)
            {
                if (exception is JsonException)
                    MessageBox.Show($"ArgumentNullException occured at {_CallerMemberName} : Line {_CallerLineNumber}\n\t{exception.Message}", "ArgumentNullException", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                else if (exception is JsonException)
                    MessageBox.Show($"JsonException occured at {_CallerMemberName} : Line {_CallerLineNumber}\n\t{exception.Message}", "JsonException", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                else if (exception is JsonException)
                    MessageBox.Show($"NotSupportedException occured at {_CallerMemberName} : Line {_CallerLineNumber}\n\t{exception.Message}", "NotSupportedException", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                else if (exception is JsonException)
                    MessageBox.Show($"NullReferenceException occured at {_CallerMemberName} : Line {_CallerLineNumber}\n\t{exception.Message}", "NullReferenceException", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            MessageBoxResult result = MessageBox.Show("Be in the acknnowledge of that your configuration will be restored to defaults when closed.\nDo you want to save the current configuration as a file?",
                "Restore Defaults - Save First?",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation);

            if (result == MessageBoxResult.Yes)
                File.Move(dataPath, dataPath + ".backup");

            return new ConfigurationData();
        }

        public static void WriteData(ConfigurationData data, string dataPath,
            [CallerLineNumber] int _CallerLineNumber = 0,
            [CallerMemberName] string _CallerMemberName = "ThemeManager.cs")
        {
            try
            {
                using FileStream stream = File.Create(dataPath);
                JsonSerializer.Serialize(stream, data);
            }
            catch (Exception exception)
            {
                if (exception is ArgumentNullException)
                    MessageBox.Show($"ArgumentNullException occured at {_CallerMemberName} : Line {_CallerLineNumber}\n\t{exception.Message}", "ArgumentNullException", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                else if (exception is NotSupportedException)
                    MessageBox.Show($"NotSupportedException occured at {_CallerMemberName} : Line {_CallerLineNumber}\n\t{exception.Message}", "NotSupportedException", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                else if (exception is JsonException)
                    MessageBox.Show($"JsonException occured at {_CallerMemberName} : Line {_CallerLineNumber}\n\t{exception.Message}", "JsonException", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        public static bool ValidateData(ConfigurationData data, string path)
        {
            bool _Debug = false;

            using (JsonDocument document = JsonDocument.Parse(File.ReadAllText(path)))
            {
                if (!document.RootElement.TryGetProperty("Language", out _) &&
                       document.RootElement.TryGetProperty("Theme", out _) &&
                       document.RootElement.TryGetProperty("UserThemes", out _) &&
                       document.RootElement.TryGetProperty("UserThemesDirectory", out _) &&
                       document.RootElement.TryGetProperty("Topmost", out _) &&
                       document.RootElement.TryGetProperty("LockWindowSize", out _))
                {
                    MessageBox.Show("Your configuration file seems to be missing some required data.\nDid you mess with it manually?\n\n" +
                        "To fix this, either delete the config file and restart the app, or restore a valid version.",
                        "Invalid Configuration",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return false;
                }
            }

            if (!Enum.IsDefined(typeof(Language), data.Language))
            {
                if (_Debug)
                    MessageBox.Show("Language is invalid.");
                return false;
            }

            if (!ThemeManager.Exists(data.Theme))
            {
                if (_Debug)
                    MessageBox.Show($"Theme '{data.Theme}' does not exist.");
                return false;
            }

            if (data.UserThemes == null)
            {
                if (_Debug)
                    MessageBox.Show("UserThemes is null.");
                return false;
            }

            return true;
        }
    }
}