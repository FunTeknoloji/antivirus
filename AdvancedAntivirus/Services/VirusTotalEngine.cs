using RestSharp;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Security.Cryptography;

namespace AdvancedAntivirus.Services
{
    public class VirusTotalEngine : IScanEngine
    {
        private readonly string _apiKey;
        private readonly RestClient _client;

        public VirusTotalEngine(string apiKey)
        {
            _apiKey = apiKey;
            _client = new RestClient("https://www.virustotal.com/api/v3/");
        }

        public async Task<ScanReport> ScanFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(_apiKey)) return new ScanReport { Result = ScanResult.Unknown, Engine = "VirusTotal (No API Key)" };

                // Get File Hash
                using var sha256 = SHA256.Create();
                using var stream = File.OpenRead(filePath);
                var hashBytes = await sha256.ComputeHashAsync(stream);
                var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                var request = new RestRequest($"files/{hash}");
                request.AddHeader("x-apikey", _apiKey);

                var response = await _client.ExecuteGetAsync(request);

                if (response.IsSuccessful && response.Content != null)
                {
                    var json = JObject.Parse(response.Content);
                    var stats = json["data"]?["attributes"]?["last_analysis_stats"];

                    if (stats != null)
                    {
                        int malicious = stats["malicious"]?.Value<int>() ?? 0;
                        if (malicious > 0)
                        {
                            return new ScanReport { FilePath = filePath, Result = ScanResult.Infected, ThreatName = $"Cloud.Detected({malicious} engines)", Engine = "VirusTotal" };
                        }
                    }
                    return new ScanReport { FilePath = filePath, Result = ScanResult.Clean, Engine = "VirusTotal" };
                }

                return new ScanReport { FilePath = filePath, Result = ScanResult.Unknown, Engine = "VirusTotal" };
            }
            catch
            {
                return new ScanReport { Result = ScanResult.Error, Engine = "VirusTotal" };
            }
        }
    }
}
