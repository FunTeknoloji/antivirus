using Newtonsoft.Json;
using System.IO;

namespace AdvancedAntivirus.Models
{
    public class UserSettings
    {
        public bool RealTimeProtection { get; set; } = true;
        public string VirusTotalApiKey { get; set; } = string.Empty;
        public bool DarkMode { get; set; } = true;
        public string InstallPath { get; set; } = string.Empty;
        public bool IsSetupComplete { get; set; } = false;

        private static readonly string SettingsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        public static UserSettings Load()
        {
            try
            {
                if (File.Exists(SettingsFile))
                {
                    var json = File.ReadAllText(SettingsFile);
                    return JsonConvert.DeserializeObject<UserSettings>(json) ?? new UserSettings();
                }
            }
            catch { }
            return new UserSettings();
        }

        public void Save()
        {
            try
            {
                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(SettingsFile, json);
            }
            catch { }
        }
    }
}
