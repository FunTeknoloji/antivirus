using System.IO;

namespace AdvancedAntivirus.Services
{
    public class HeuristicEngine : IScanEngine
    {
        public async Task<ScanReport> ScanFileAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                var fileInfo = new FileInfo(filePath);

                // Basit sezgisel kontroller
                // 1. Çift uzantı kontrolü (örneğin image.jpg.exe)
                if (filePath.EndsWith(".exe.exe") || filePath.EndsWith(".jpg.exe") || filePath.EndsWith(".pdf.exe"))
                {
                    return new ScanReport { FilePath = filePath, Result = ScanResult.Infected, ThreatName = "Heuristic.DoubleExtension", Engine = "Heuristic" };
                }

                // 2. Çok küçük boyutlu çalıştırılabilir dosyalar (Sıklıkla downloader/dropper olabilir)
                if (fileInfo.Extension.Equals(".exe", StringComparison.OrdinalIgnoreCase) && fileInfo.Length < 1024 * 5)
                {
                    return new ScanReport { FilePath = filePath, Result = ScanResult.Infected, ThreatName = "Heuristic.SuspiciousSmallExe", Engine = "Heuristic" };
                }

                return new ScanReport { FilePath = filePath, Result = ScanResult.Clean, Engine = "Heuristic" };
            });
        }
    }
}
