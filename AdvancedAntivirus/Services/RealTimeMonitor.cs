using System.IO;

namespace AdvancedAntivirus.Services
{
    public class RealTimeMonitor
    {
        private readonly FileSystemWatcher _watcher;
        private readonly AntivirusService _antivirusService;
        public event Action<ScanReport>? OnThreatDetected;

        public RealTimeMonitor(AntivirusService antivirusService)
        {
            _antivirusService = antivirusService;
            _watcher = new FileSystemWatcher
            {
                Path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
            };

            _watcher.Created += OnChanged;
            _watcher.Changed += OnChanged;
            _watcher.Renamed += OnChanged;
        }

        public void Start() => _watcher.EnableRaisingEvents = true;
        public void Stop() => _watcher.EnableRaisingEvents = false;

        private async void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (Directory.Exists(e.FullPath)) return;

            var reports = await _antivirusService.FullScanFileAsync(e.FullPath);
            foreach (var report in reports)
            {
                if (report.Result == ScanResult.Infected)
                {
                    OnThreatDetected?.Invoke(report);
                    break;
                }
            }
        }
    }
}
