using System.IO;

namespace AdvancedAntivirus.Services
{
    public class QuarantineService
    {
        private readonly string _quarantinePath;

        public QuarantineService()
        {
            _quarantinePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Quarantine");
            if (!Directory.Exists(_quarantinePath))
            {
                Directory.CreateDirectory(_quarantinePath);
            }
        }

        public void QuarantineFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) return;

                string fileName = Path.GetFileName(filePath);
                string destination = Path.Combine(_quarantinePath, fileName + ".quarantine");

                // Şifreleme eklenebilir veya sadece taşıma yapılabilir
                // Dosyayı taşıyarak erişimi engelliyoruz
                File.Move(filePath, destination, true);

                // Dosya izinlerini değiştirme (Okuma/Yazma engelleme) simüle edilebilir
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Quarantine failed: {ex.Message}");
            }
        }

        public void RestoreFile(string quarantinedFileName, string originalPath)
        {
             string source = Path.Combine(_quarantinePath, quarantinedFileName);
             if (File.Exists(source))
             {
                 File.Move(source, originalPath.Replace(".quarantine", ""), true);
             }
        }
    }
}
