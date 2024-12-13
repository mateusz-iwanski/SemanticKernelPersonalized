using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelPersonalized.Settings
{
    public class FirecrawlSemanticSettings
    {
        private string _settingsMap { get; set; } = "SemanticKernel->Access->Firecrawl";

        private string _apiKey { get; set; }
        private string _apiUrl { get; set; }
        private string _apiMapUrl { get; set; }

        // It must be in the settings.json file
        public string ApiKey 
        {
            get
            {
                if (string.IsNullOrEmpty(_apiKey)) 
                    throw new ArgumentNullException($"{_settingsMap}->{nameof(ApiKey)} - must be in your settings.json");
                return _apiKey;
            }
            set => _apiKey = value;
        }

        // It must be in the settings.json file
        public string ApiUrl 
        {
            get
            {
                if (string.IsNullOrEmpty(_apiUrl))
                    throw new ArgumentNullException($"{_settingsMap}->{nameof(ApiUrl)} - must be in your settings.json");
                return _apiUrl;
            }
            set => _apiUrl = value;
        }

        // It must be in the settings.json file
        public string ApiMapUrl 
        {
            get
            {
                if (string.IsNullOrEmpty(_apiMapUrl))
                    throw new ArgumentNullException($"{_settingsMap}->{nameof(ApiMapUrl)} - must be in your settings.json");
                return _apiMapUrl;
            }
            set => _apiMapUrl = value;
        }
    }
}
