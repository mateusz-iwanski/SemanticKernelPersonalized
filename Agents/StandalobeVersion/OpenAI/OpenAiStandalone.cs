using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Primitives;
using Microsoft.SemanticKernel.Services;
using SemanticKernelPersonalized.Settings;
using Microsoft.Extensions.AI;
using System.Configuration;
using SemanticKernelPersonalized.Agents.KernelVersion;
using SemanticKernelPersonalized.AgentsManagement;

namespace SemanticKernelPersonalized.Agents.Standalone.OpenAi
{

    /// <summary>
    /// Standalone agent doesn't have a kernel, so it uses the OpenAIChatCompletionService directly.
    /// OpenAiStandalone agent doesn't use plugins.
    /// </summary>
    public class OpenAiStandalone : IConnector, IImageAnalyzer
    {
        private readonly ILogger<OpenAiStandalone> _logger;
        private readonly OpenAISemanticSettings _openAISettings;
        private OpenAIChatCompletionService _chatCompletionService { get; set; }
        private OpenAIPromptExecutionSettings _settings { get; set; } = new() { };

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAiStandalone"/> class with default settings.
        /// If you want to change the default model ID settings, use the <see cref="CustomizeModel"/> method 
        /// If you want to change the default kernel settings, use the <see cref="SetModelSettings"/> method.
        /// </summary>
        /// <param name="openAISettings">The settings for OpenAI, including API key, organization ID, service ID, and default model ID.</param>
        /// <remarks>
        /// In the constructor set the default model settings for the OpenAIChatCompletionService:
        /// </remarks>
        public OpenAiStandalone(IOptions<OpenAISemanticSettings> openAISettings, ILogger<OpenAiStandalone> logger)
        {
            _openAISettings = openAISettings.Value;

            _logger = logger;

            _chatCompletionService = new OpenAIChatCompletionService(
                modelId: _openAISettings.DefaultModelId,
                apiKey: _openAISettings.ApiKey,
                organization: _openAISettings.OrganizationId  
            );
        }
        public void CustomizeModel(string modelId)
        {
            _chatCompletionService = new OpenAIChatCompletionService(
                modelId: modelId,
                apiKey: _openAISettings.ApiKey,
                organization: _openAISettings.OrganizationId);
        }

        /// <summary>
        /// Configures the AI model execution settings with specified parameters.
        /// </summary>
        /// <param name="functionChoiceBehavior">Determines how the AI model should choose and execute functions. In standalone agent it won't work, it doesn't use plugins. </param>
        /// <param name="maxTokens">Maximum number of tokens to generate in the response.</param>
        /// <param name="temperature">Controls randomness in the response. Values range from 0 to 1, where higher values produce more random output.</param>
        /// <param name="topP">Controls diversity via nucleus sampling. Values range from 0 to 1, providing finer control over output randomness.</param>
        /// <param name="frequencyPenalty">Reduces repetition by penalizing frequent tokens. Values typically range from -2.0 to 2.0, where positive values decrease the likelihood of repeated information.</param>
        /// <param name="presencePenalty">Reduces repetition by penalizing tokens that have appeared at all. Values typically range from -2.0 to 2.0, where positive values decrease the likelihood of using any token that has appeared before.</param>
        /// <param name="stopSequences">List of sequences that will cause the text generation to stop when encountered.</param>
        /// <param name="tokenSelectionBiases">Dictionary to modify likelihood of specific tokens. Keys are token IDs and values are bias values where positive values increase likelihood, negative values decrease it.</param>
        public void ConfigureSettings(
            FunctionChoiceBehavior functionChoiceBehavior,
            int maxTokens,
            double temperature,
            double topP,
            double frequencyPenalty,
            double presencePenalty,
            List<string> stopSequences,
            Dictionary<int, int> tokenSelectionBiases
        )
        {
            _settings = new()
            {
                FunctionChoiceBehavior = functionChoiceBehavior,

                // Response length and randomness
                MaxTokens = maxTokens,
                Temperature = temperature,
                TopP = topP,

                // Token selection
                FrequencyPenalty = frequencyPenalty,
                PresencePenalty = presencePenalty,

                // Stop sequences
                StopSequences = stopSequences, //new List<string> { "STOP", "\n" },

                // Token counting
                TokenSelectionBiases = tokenSelectionBiases
                //new Dictionary<int, int>
                //{
                //    { 123, 10 },  // Increase likelihood of token 123
                //    { 456, -10 }  // Decrease likelihood of token 456
                //}
            };
        }

        public OpenAIPromptExecutionSettings GetPromptExecutionSettings() => _settings;

        public IChatCompletionService GetChatCompletionService() => _chatCompletionService;

        public string GetModelId() => _chatCompletionService.GetModelId();

        // Standalone version doesn't have a kernel
        public Kernel? GetKernel() => null;

        public ChatDialog GetConversationHistory()
        {
            throw new NotImplementedException();
        }
    }
}
