using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using SemanticKernelPersonalized.Plugins.WebScrapping;
using SemanticKernelPersonalized.Settings;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelPersonalized.Agents.KernelVersion
{
    public class KernelBuilder
    {
        private readonly OpenAISemanticSettings _accessOpenAi;

        public KernelBuilder() { return; }

        public Kernel CreateKernelWithChatCompletion(
            IOptions<OpenAISemanticSettings> openAISettings,
            IOptions<FirecrawlSemanticSettings> firecrawlSemanticSettings
        )
        {
            var builder = Kernel.CreateBuilder();

            AddChatCompletionToKernel(builder, openAISettings);

            // add services
            builder.Services.AddSingleton<IWebScraping, Firecrawl>();
            builder.Services.AddSingleton<Firecrawl>();

            // plugins
            builder.Plugins.AddFromType<WebScrapingPlugin>("WebScrapping");


            // add configuration
            builder.Services.Configure<FirecrawlSemanticSettings>(options =>
            {
                options.ApiKey = firecrawlSemanticSettings.Value.ApiKey;
                options.ApiUrl = firecrawlSemanticSettings.Value.ApiUrl;
                options.ApiMapUrl = firecrawlSemanticSettings.Value.ApiMapUrl;
            });

            return builder.Build();
        }

        public void AddChatCompletionToKernel(IKernelBuilder builder, IOptions<OpenAISemanticSettings> access)
        {
            builder.AddOpenAIChatCompletion(
                access.Value.DefaultModelId,
                access.Value.ApiKey
                );
        }
    }
}
