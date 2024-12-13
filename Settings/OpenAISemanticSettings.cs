using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelPersonalized.Settings
{
    public class OpenAISemanticSettings
    {
        private string _settingsMap { get; set; } = "SemanticKernel->Access->OpenAi";
        // It must be in the settings.json file
        private string _apiKey { get; set; }

        // It must be in the settings.json file
        private string _defaultModelId { get; set; }

        public string ApiKey 
        {
            get
            {
                if (string.IsNullOrEmpty(_apiKey)) throw new ArgumentNullException($"{_settingsMap}->{nameof(ApiKey)} - must be in your settings.json");
                return _apiKey;
            }
            set => _apiKey = value;
        }

        public string DefaultModelId 
        { 
            get
            {
                if (string.IsNullOrEmpty(_defaultModelId)) 
                    throw new ArgumentNullException($"{_settingsMap}->{nameof(DefaultModelId)} - must be in your settings.json");
                return _defaultModelId;
            }
            set => _defaultModelId = value;
        }

        public string OrganizationId { get; set; }
        public string ServiceId { get; set; }
        

    }
}
