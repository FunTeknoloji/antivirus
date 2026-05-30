using System.IO;
using System.Security.Cryptography;

namespace AdvancedAntivirus.Services
{
    public class LocalHashEngine : IScanEngine
    {
        // Örnek bir veritabanı (Gerçekte bu binlerce kayıtlı bir dosya veya DB olur)
        private static readonly HashSet<string> MaliciousHashes = new()
        {
            "E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855", // Empty file test hash
            "5E884898DA28047151D0E56F8DC6292773603D0D6AABBDD62A11EF721D1542D8"  // "password" hash
        };

        public async Task<ScanReport> ScanFileAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) return new ScanReport { Result = ScanResult.Error };

                using var sha256 = SHA256.Create();
                using var stream = File.OpenRead(filePath);
                var hashBytes = await sha256.ComputeHashAsync(stream);
                var hash = BitConverter.ToString(hashBytes).Replace("-", "");

                if (MaliciousHashes.Contains(hash))
                {
                    return new ScanReport { FilePath = filePath, Result = ScanResult.Infected, ThreatName = "Malware.Local.Hash", Engine = "LocalHash" };
                }

                return new ScanReport { FilePath = filePath, Result = ScanResult.Clean, Engine = "LocalHash" };
            }
            catch
            {
                return new ScanReport { Result = ScanResult.Error };
            }
        }
    }
}
