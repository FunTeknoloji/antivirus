namespace AdvancedAntivirus.Services
{
    public class AntivirusService
    {
        private readonly List<IScanEngine> _engines = new();

        public AntivirusService(string vtApiKey)
        {
            _engines.Add(new LocalHashEngine());
            _engines.Add(new HeuristicEngine());
            _engines.Add(new VirusTotalEngine(vtApiKey));
            // ClamAV entegrasyonu genelde yerel bir servis gerektirir,
            // burada placeholder olarak diğerlerini kullanıyoruz.
        }

        public async Task<List<ScanReport>> FullScanFileAsync(string filePath)
        {
            var reports = new List<ScanReport>();
            foreach (var engine in _engines)
            {
                var report = await engine.ScanFileAsync(filePath);
                reports.Add(report);
                if (report.Result == ScanResult.Infected) break; // Bir motor bulduysa dur (hız için)
            }
            return reports;
        }
    }
}
