using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using AdvancedAntivirus.Models;

namespace AdvancedAntivirus.ViewModels
{
    public partial class SetupViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _installPath = "C:\\Program Files\\AdvancedAntivirus";

        public event Action? OnSetupComplete;

        [RelayCommand]
        private void CompleteSetup()
        {
            var settings = UserSettings.Load();
            settings.InstallPath = InstallPath;
            settings.Save();
            OnSetupComplete?.Invoke();
        }
    }
}
