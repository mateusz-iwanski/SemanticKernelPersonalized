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
        public KernelBuilder() { return; }

        public Kernel CreateKernelWithOpenAIChatCompletion(
            string modelId, 
            string modelApiKey,
            IOptions<FirecrawlSemanticSettings>? firecrawlSemanticSettings = null
            )
        {
            var builder = Kernel.CreateBuilder();

            builder.AddOpenAIChatCompletion(
                modelId,
                modelApiKey
               );

            AddPlugins(
                builder,
                firecrawlSemanticSettings
                );

            return builder.Build();

        }

        private void AddPlugins(
            IKernelBuilder builder,
            IOptions<FirecrawlSemanticSettings>? firecrawlSemanticSettings
            )
        {
            if (firecrawlSemanticSettings != null)
            {
                // add configuration
                builder.Services.Configure<FirecrawlSemanticSettings>(options =>
                {
                    options.ApiKey = firecrawlSemanticSettings.Value.ApiKey;
                    options.ApiUrl = firecrawlSemanticSettings.Value.ApiUrl;
                    options.ApiMapUrl = firecrawlSemanticSettings.Value.ApiMapUrl;
                });

                // add services
                builder.Services.AddSingleton<IWebScraping, Firecrawl>();
                builder.Services.AddSingleton<Firecrawl>();

                // plugins
                builder.Plugins.AddFromType<WebScrapingPlugin>("WebScrapping");
            }
        }

        //public Kernel CreateKernelWithChatCompletion(
        //    IOptions<FirecrawlSemanticSettings> firecrawlSemanticSettings,
        //    string modelId,
        //    string apiKey
        //    )
        //{
        //    var builder = Kernel.CreateBuilder();

        //    AddChatCompletionToKernel(builder, modelId, apiKey);


        //    // add services
        //    builder.Services.AddSingleton<IWebScraping, Firecrawl>();
        //    builder.Services.AddSingleton<Firecrawl>();

        //    // plugins
        //    builder.Plugins.AddFromType<WebScrapingPlugin>("WebScrapping");

        //    // add configuration
        //    builder.Services.Configure<FirecrawlSemanticSettings>(options =>
        //    {
        //        options.ApiKey = firecrawlSemanticSettings.Value.ApiKey;
        //        options.ApiUrl = firecrawlSemanticSettings.Value.ApiUrl;
        //        options.ApiMapUrl = firecrawlSemanticSettings.Value.ApiMapUrl;
        //    });

        //    return builder.Build();
        //}

        //public void AddChatCompletionToKernel(IKernelBuilder builder, string modelId, string apiKey)
        //{            
        //    builder.AddOpenAIChatCompletion(
        //        modelId,
        //        apiKey
        //        );
        //}
    }
}
