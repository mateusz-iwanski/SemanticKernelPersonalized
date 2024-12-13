using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelPersonalized.Settings
{
    public class AzureApplicationInsightsSettings
    {
        private string _settingsMap { get; set; } = "SemanticKernel->Access->AzureInsightsApplication";

        private string _connectionString { get; set; }

        // It must be in the settings.json file
        public string ConnectionString 
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString)) 
                    throw new ArgumentNullException($"{_settingsMap}->{nameof(ConnectionString)} - must be in your settings.json");
                return _connectionString;
            }
            set => _connectionString = value;
        }
    }
}
