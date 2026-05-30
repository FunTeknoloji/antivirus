using Xunit;
using AdvancedAntivirus.Services;
using System.IO;

namespace AdvancedAntivirus.Tests
{
    public class AntivirusTests
    {
        [Fact]
        public async Task LocalHashEngine_DetectsMaliciousHash()
        {
            var engine = new LocalHashEngine();
            string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".txt");
            File.WriteAllText(tempFile, "password"); // Hash of "password" is in our local DB

            var report = await engine.ScanFileAsync(tempFile);

            Assert.Equal(ScanResult.Infected, report.Result);
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }

        [Fact]
        public async Task HeuristicEngine_DetectsDoubleExtension()
        {
            var engine = new HeuristicEngine();
            var report = await engine.ScanFileAsync("test.jpg.exe");

            Assert.Equal(ScanResult.Infected, report.Result);
            Assert.Equal("Heuristic.DoubleExtension", report.ThreatName);
        }
    }
}
