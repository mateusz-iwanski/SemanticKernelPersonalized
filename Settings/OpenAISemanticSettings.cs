using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelPersonalized.Settings
{
    public class OpenAISemanticSettings
    {
        public string ApiKey { get; set; }
        public string OrganizationId { get; set; }
        public string ServiceId { get; set; }
        public string DefaultModelId { get; set; }

    }
}
