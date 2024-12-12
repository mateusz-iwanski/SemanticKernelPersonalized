using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SemanticKernelPersonalized.Agents.KernelVersion.OpenAi;
using SemanticKernelPersonalized.Builders;
using SemanticKernelPersonalized.Settings;
using System;

namespace SemanticKernelPersonalized
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })

                .ConfigureServices((context, services) =>
                {
                    var sematicKernel = new SemanticKernelServicesBuilder();
                    sematicKernel.ConfigureServices(context, services);
                })
                .Build();

            // Replace the connection string with your Application Insights connection string
            var connectionString = host.Services.GetRequiredService<IOptions<AzureApplicationInsightsSettings>>().Value.ConnectionString;

            var resourceBuilder = ResourceBuilder
                .CreateDefault()
                .AddService("TelemetryApplicationInsightsQuickstart");

            // Enable model diagnostics with sensitive data.
            AppContext.SetSwitch("Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnosticsSensitive", true);

            using var traceProvider = Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(resourceBuilder)
                .AddSource("Microsoft.SemanticKernel*")
                .AddAzureMonitorTraceExporter(options => options.ConnectionString = connectionString)
                .Build();

            using var meterProvider = Sdk.CreateMeterProviderBuilder()
                .SetResourceBuilder(resourceBuilder)
                .AddMeter("Microsoft.SemanticKernel*")
                .AddAzureMonitorMetricExporter(options => options.ConnectionString = connectionString)
                .Build();

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                // Add OpenTelemetry as a logging provider
                builder.AddOpenTelemetry(options =>
                {
                    options.SetResourceBuilder(resourceBuilder);
                    options.AddAzureMonitorLogExporter(options => options.ConnectionString = connectionString);
                    // Format log messages. This is default to false.
                    options.IncludeFormattedMessage = true;
                    options.IncludeScopes = true;
                });
                builder.SetMinimumLevel(LogLevel.Information);
            });

            //////

            var openai = host.Services.GetService<OpenAIKernel>();
            await openai.GetResponseAsync("jaki kolor ma słońce");


        }
    }
}
