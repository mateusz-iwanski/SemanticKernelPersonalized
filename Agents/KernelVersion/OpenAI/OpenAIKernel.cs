using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Services;
using OpenAI.Models;
using SemanticKernelPersonalized.AgentsManagement;
using SemanticKernelPersonalized.Plugins.WebScrapping;
using SemanticKernelPersonalized.Settings;



namespace SemanticKernelPersonalized.Agents.KernelVersion.OpenAi
{
    /// <summary>
    /// The OpenAIKernel class integrates with OpenAI's chat completion capabilities.
    /// It implements the IConnector interface, providing methods to customize and configure the OpenAI model,
    /// retrieve the current model ID, and access the chat completion service.
    /// Agents with kernel can use plugins.
    /// </summary>
    public class OpenAIKernel : IConnector, IImageAnalyzer
    {
        private Kernel _kernel;
        private readonly IOptions<OpenAISemanticSettings> _openAISettings;
        private readonly IOptions<FirecrawlSemanticSettings> _firecrawlSemanticSettings;
        private OpenAIPromptExecutionSettings _settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };

        private readonly ChatDialog _chatDialog;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAIKernel"/> class with default settings.
        /// If you want to change the default model ID settings, use the <see cref="CustomizeModel"/> method.
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
            _chatDialog = new ChatDialog(this, new Microsoft.Extensions.Logging.Abstractions.NullLogger<ChatDialog>());

            _firecrawlSemanticSettings = firecrawlSemanticSettings;            

            _kernel = new KernelBuilder().CreateKernelWithOpenAIChatCompletion(
                modelId: _openAISettings.Value.DefaultModelId,
                modelApiKey: _openAISettings.Value.ApiKey,
                chatDialog: _chatDialog,
                firecrawlSemanticSettings: _firecrawlSemanticSettings                
                );

            _chatDialog = new ChatDialog(this, new Microsoft.Extensions.Logging.Abstractions.NullLogger<ChatDialog>());

        }


        public ChatDialog GetConversationHistory() => _chatDialog;

        public async Task<ChatMessageContent> ChatAsync(
            string message,
            PromptExecutionSettings? promptExecutionSettings = null,
            Kernel? kernel = null,
            Dictionary<string, object>? messageAdditionalMetadata = null,
            string? source = null,
            string? name = null,
            string? mimeType = null,
            string? outputMimeType = null,
            string? author = null
            )
        {
            var response = await _chatDialog.GetChatMessageContentAsync(
                message,
                promptExecutionSettings,
                kernel,
                messageAdditionalMetadata,
                source,
                name,
                mimeType,
                outputMimeType,
                author
                );
            
            return response;
        }

        /// <summary>
        /// Customizes the model ID for the kernel.
        /// </summary>
        /// <param name="modelId">The new model ID to be set.</param>
        public void CustomizeModel(string modelId)
        {
            _kernel = new KernelBuilder().CreateKernelWithOpenAIChatCompletion(
                modelId: modelId,
                modelApiKey: _openAISettings.Value.ApiKey,
                chatDialog: _chatDialog,
                firecrawlSemanticSettings: _firecrawlSemanticSettings
                );
        }

        /// <summary>
        /// Configures the AI model execution settings with specified parameters.
        /// </summary>
        /// <param name="functionChoiceBehavior">Determines how the AI model should choose and execute functions.</param>
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
            _settings = new OpenAIPromptExecutionSettings()
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

        /// <summary>
        /// Gets the prompt execution settings.
        /// </summary>
        /// <returns>The prompt execution settings.</returns>
        public OpenAIPromptExecutionSettings GetPromptExecutionSettings() => _settings;

        /// <summary>
        /// Gets the model ID of the current kernel.
        /// </summary>
        /// <returns>The model ID.</returns>
        public string? GetModelId()
        {
            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            return chatCompletionService.GetModelId();
        }

        /// <summary>
        /// Gets the current kernel instance.
        /// </summary>
        /// <returns>The kernel instance.</returns>
        public Kernel GetKernel() => _kernel;        

        /// <summary>
        /// Gets the chat completion service.
        /// </summary>
        /// <returns>The chat completion service.</returns>
        public IChatCompletionService GetChatCompletionService()
        {
            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            return chatCompletionService;
        }
    }
}
