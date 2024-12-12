using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using SemanticKernelPersonalized.Settings;
using Microsoft.Extensions.Logging;
using SemanticKernelPersonalized.Agents.OpenAi.KelnerVersion;

namespace SemanticKernelPersonalized.Builders
{
    /// <summary>
    /// Represents a builder for configuring services.
    /// 
    /// Step 1:
    /// Look on appsettins_schema and add structure to your seetings JSON file.
    /// Adds the JSON configuration provider at path to builder (Microsoft.Extensions.Configuration.IConfigurationBuilder).
    /// 
    /// Step 2: 
    /// Add logging Nlog to the IServiceCollection
    /// 
    /// </summary>

    public class SemanticKernelServicesBuilder
    {
        public void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            // Add settings
            services.Configure<OpenAISemanticSettings>(context.Configuration.GetSection("SemanticKernel:Access:OpenAi"));
            services.Configure<FirecrawlSemanticSettings>(context.Configuration.GetSection("SemanticKernel:Access:Firecrawl"));
            services.Configure<AzureApplicationInsightsSettings>(context.Configuration.GetSection("SemanticKernel:Access:AzureInsightsApplication"));
            //services.AddSingleton<IDependencyConnectorFactory, DependencyConnectorFactory>();

            // Add services
            //services.AddScoped<OpenAiStandalone>();
            services.AddScoped<OpenAIKernel>();

        }
            
    }
}
