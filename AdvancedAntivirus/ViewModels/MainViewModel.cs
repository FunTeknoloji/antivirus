using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using AdvancedAntivirus.Models;
using AdvancedAntivirus.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdvancedAntivirus.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly AntivirusService _antivirusService;
        private readonly RealTimeMonitor _realTimeMonitor;
        private readonly QuarantineService _quarantineService;

        [ObservableProperty]
        private object? _currentView;

        [ObservableProperty]
        private string _statusText = "Sistem Güvende";

        [ObservableProperty]
        private bool _isScanning;

        public ObservableCollection<ScanReport> RecentThreats { get; } = new();

        public MainViewModel()
        {
            var settings = UserSettings.Load();
            _antivirusService = new AntivirusService(settings.VirusTotalApiKey);
            _quarantineService = new QuarantineService();
            _realTimeMonitor = new RealTimeMonitor(_antivirusService);

            _realTimeMonitor.OnThreatDetected += (report) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    StatusText = "TEHDİT ALGILANDI!";
                    RecentThreats.Insert(0, report);
                    _quarantineService.QuarantineFile(report.FilePath);
                });
            };

            if (settings.RealTimeProtection)
            {
                _realTimeMonitor.Start();
            }

            CurrentView = new DashboardViewModel();
        }

        [RelayCommand]
        private void ShowDashboard() => CurrentView = new DashboardViewModel();

        [RelayCommand]
        private void ShowScan() => CurrentView = new ScanViewModel(this);

        [RelayCommand]
        private void ShowQuarantine() => CurrentView = new QuarantineViewModel();

        [RelayCommand]
        private void ShowSettings() => CurrentView = new SettingsViewModel();

        public async Task StartScan(string path)
        {
            IsScanning = true;
            StatusText = "Tarama Yapılıyor...";

            var results = await _antivirusService.FullScanFileAsync(path);
            foreach (var report in results)
            {
                if (report.Result == ScanResult.Infected)
                {
                    RecentThreats.Insert(0, report);
                    _quarantineService.QuarantineFile(report.FilePath);
                }
            }

            IsScanning = false;
            StatusText = RecentThreats.Count > 0 ? "Tehditler Temizlendi" : "Sistem Güvende";
        }
    }

    public class DashboardViewModel : ObservableObject { }

    public partial class ScanViewModel : ObservableObject
    {
        private readonly MainViewModel _mainVm;

        public ScanViewModel(MainViewModel mainVm)
        {
            _mainVm = mainVm;
        }

        [RelayCommand]
        private async Task QuickScan()
        {
            // Simüle edilen hızlı tarama - masaüstünü tarayalım
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            await _mainVm.StartScan(desktop);
        }
    }

    public class QuarantineViewModel : ObservableObject { }
    public class SettingsViewModel : ObservableObject { }
}
