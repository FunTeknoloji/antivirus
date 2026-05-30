using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using AdvancedAntivirus.Models;
using AdvancedAntivirus.Services;

namespace AdvancedAntivirus.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private AntivirusService? _antivirusService;
        private RealTimeMonitor? _realTimeMonitor;
        private readonly QuarantineService _quarantineService;

        [ObservableProperty]
        private object? _currentView;

        [ObservableProperty]
        private string _statusText = "Sistem Güvende";

        [ObservableProperty]
        private bool _isSetupMode;

        public ObservableCollection<ScanReport> RecentThreats { get; } = new();

        public MainViewModel()
        {
            _quarantineService = new QuarantineService();
            var settings = UserSettings.Load();

            if (!settings.IsSetupComplete)
            {
                IsSetupMode = true;
                var setupVm = new SetupViewModel();
                setupVm.OnSetupComplete += InitializeApp;
                CurrentView = setupVm;
            }
            else
            {
                InitializeApp();
            }
        }

        private void InitializeApp()
        {
            var settings = UserSettings.Load();
            settings.IsSetupComplete = true;
            settings.Save();

            IsSetupMode = false;
            _antivirusService = new AntivirusService(settings.VirusTotalApiKey);
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
            if (_antivirusService == null) return;

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
            StatusText = RecentThreats.Count > 0 ? "Tehditler Temizlendi" : "Sistem Güvende";
        }
    }

    public class DashboardViewModel : ObservableObject { }
    public partial class ScanViewModel : ObservableObject
    {
        private readonly MainViewModel _mainVm;
        public ScanViewModel(MainViewModel mainVm) => _mainVm = mainVm;
        [RelayCommand]
        private async Task QuickScan() => await _mainVm.StartScan(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
    }
    public class QuarantineViewModel : ObservableObject { }
    public class SettingsViewModel : ObservableObject { }
}
