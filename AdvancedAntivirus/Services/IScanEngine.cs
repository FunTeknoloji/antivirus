using System.Security.Cryptography;
using System.Text;

namespace AdvancedAntivirus.Services
{
    public enum ScanResult
    {
        Clean,
        Infected,
        Unknown,
        Error
    }

    public class ScanReport
    {
        public string FilePath { get; set; } = string.Empty;
        public ScanResult Result { get; set; }
        public string ThreatName { get; set; } = string.Empty;
        public string Engine { get; set; } = string.Empty;
    }

    public interface IScanEngine
    {
        Task<ScanReport> ScanFileAsync(string filePath);
    }
}
