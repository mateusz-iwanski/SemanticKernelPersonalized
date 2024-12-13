using Azure;
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
using SemanticKernelPersonalized.Agents.KernelVersion;
using SemanticKernelPersonalized.Agents.KernelVersion.OpenAi;
using SemanticKernelPersonalized.Agents.Standalone.OpenAi;
using SemanticKernelPersonalized.AgentsManagement;
using SemanticKernelPersonalized.Builders;
using SemanticKernelPersonalized.Settings;
using System;

namespace SemanticKernelPersonalized
{
    internal class Program
    {
        public static Guid SESSION_UUID = Guid.NewGuid();

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
                .AddService(
                    serviceName: "TelemetryApplicationInsightsQuickstart",
                    serviceInstanceId: SESSION_UUID.ToString()  // this is "Role Instance" in Application Insights
                    );

             //.AddService( "YourNewServiceName",
             //   serviceNamespace: "YourNamespace",
             //   serviceVersion: "1.0.0",
             //   serviceInstanceId: Guid.NewGuid().ToString(),
             //   attributes: new Dictionary<string, object>
             //   {
             //       ["environment"] = "production",
             //       ["deployment.region"] = "west-europe"
             //   });

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
            Console.WriteLine("Hello World!");

            var openai = host.Services.GetService<OpenAIKernel>();
            var logger = host.Services.GetRequiredService<ILogger<ChatDialog>>();
            ChatDialog chatDialogKernel = new ChatDialog(openai, logger);
            chatDialogKernel.AddSystemMessage("You are helpful assistant.");
            var k = await chatDialogKernel.GetChatMessageContentAsync("pobierz url z www.rtk.pl");
            Console.WriteLine("Assistant: " + k.Content);

            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                Environment.Exit(0);
            };

            while (true)
            {
                Console.Write("User: ");
                var userInput = Console.ReadLine();
                if (string.IsNullOrEmpty(userInput)) continue;

                var response = await chatDialogKernel.GetChatMessageContentAsync(userInput);
                Console.WriteLine("Assistant: " + response.Content);
            }



            //var k = await chatDialogKernel.GetChatMessageContentAsync("what i the color of the sun");

            //var openai = host.Services.GetService<OpenAiStandalone>();
            //var logger = host.Services.GetRequiredService<ILogger<ChatDialog>>();
            //ChatDialog chatDialogStandAlone = new ChatDialog(openai, logger);
            //chatDialogStandAlone.AddSystemMessage("You are helpful assistant.");
            //var k = await chatDialogStandAlone.GetChatMessageContentAsync("pobierz url z https://rtk.pl/onas/");

            Console.WriteLine(k.Content);

        }
    }
}
