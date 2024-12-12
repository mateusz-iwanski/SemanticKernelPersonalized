using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using SemanticKernelPersonalized.HttpUtils;

namespace SemanticKernelPersonalized.Agents.OpenAi
{
    public class OpenAIModelValidator
    {
        private ApiClient apiClient = new ApiClient();

        public async Task test()
        {
            apiClient.SetAuthorizationHeader("Bearer", "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");

            var response = await apiClient.GetAsync("https://api.openai.com/v1/models");

            // Parsujemy odpowiedź JSON            
            var v = await response.Content.ReadAsStringAsync();

            Console.WriteLine(v);
        }
    }
}
