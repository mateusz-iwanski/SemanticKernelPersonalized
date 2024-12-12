using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Services;
using OpenAI.Models;
using SemanticKernelPersonalized.Plugins.WebScrapping;
using SemanticKernelPersonalized.Settings;



namespace SemanticKernelPersonalized.Agents.KernelVersion.OpenAi
{
    public class OpenAIKernel : IConnectorKernel
    {
        private Kernel _kernel;
        private readonly IOptions<OpenAISemanticSettings> _openAISettings;
        private readonly IOptions<FirecrawlSemanticSettings> _firecrawlSemanticSettings;

        private PromptExecutionSettings _settings { get; set; } = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAIKernel"/> class with default settings.
        /// If you want to change the default model ID settings, use the <see cref="CustomizeModel"/> method 
        /// If you want to change the default kernel settings, use the <see cref="SetModelSettings"/> method.
        /// </summary>
        /// <param name="openAISettings">The settings for OpenAI, including API key, organization ID, service ID, and default model ID.</param>
        /// <param name="firecrawlSemanticSettings">The settings for Firecrawl, including API key, API URL, and API map URL.</param>
        /// <remarks>
        /// This constructor initializes the kernel with chat completion capabilities using the provided OpenAI and Firecrawl settings.
        /// It also sets the default model settings for the OpenAIChatCompletionService:
        ///     - FunctionChoiceBehavior: Auto()
        /// </remarks>
        public OpenAIKernel(
            IOptions<OpenAISemanticSettings> openAISettings, 
            IOptions<FirecrawlSemanticSettings> firecrawlSemanticSettings
            )
        {
            _openAISettings = openAISettings;
            _firecrawlSemanticSettings = firecrawlSemanticSettings;

            _kernel = new KernelBuilder().CreateKernelWithOpenAIChatCompletion(
                modelId: _openAISettings.Value.DefaultModelId,
                modelApiKey: _openAISettings.Value.ApiKey,
                firecrawlSemanticSettings: _firecrawlSemanticSettings
                );
        }

        public void CustomizeModel(string modelId)
        {
            _kernel = new KernelBuilder().CreateKernelWithOpenAIChatCompletion(
                modelId: _openAISettings.Value.DefaultModelId,
                modelApiKey: _openAISettings.Value.ApiKey,
                firecrawlSemanticSettings: _firecrawlSemanticSettings
                );
        }

        /// <summary>
        /// Set the kernel settings for the OpenAIChatCompletionService.
        /// </summary>
        /// <remarks>
        /// Default settings are created in the constructor
        /// </remarks>
        /// <param name="functionChoiceBehavior"></param>
        public void ConfigureSettings(
            FunctionChoiceBehavior functionChoiceBehavior
            )
        {
            _settings = new() { 
                FunctionChoiceBehavior = functionChoiceBehavior, 
            };
        }

        /// <summary>
        /// Invokes the kernel with the provided prompt and returns the result as a string.
        /// </summary>
        /// <param name="prompt">The prompt to be processed by the kernel.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the response from the kernel as a string.</returns>
        public async Task<string> InvokeAsync(string prompt)
        {
            // Add for logging in Azure Application Insights -> Dependency -> Dependecy (name)
            // I use it for tracking the request
            // It will appear in the Application Insights logs, in the first dependency thread
            var promptConfig = JsonSerializer.Deserialize<PromptTemplateConfig>(
                """
                { "name" : "MapScrapper" }
                """
                )!;

            promptConfig.Template = prompt;

            promptConfig.AddExecutionSettings(_settings);

            var func = _kernel.CreateFunctionFromPrompt(promptConfig);
            var result = await _kernel.InvokeAsync(func);

            return result.GetValue<string>();
        }

        /// <summary>
        /// Get the model ID of the current kernel.
        /// </summary>
        /// <returns>Model ID</returns>
        public string? GetModelId()
        {
            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            return chatCompletionService.GetModelId();
        }
    }
}
