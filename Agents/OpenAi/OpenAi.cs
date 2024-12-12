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
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections;

namespace SemanticKernelPersonalized.Agents.OpenAi
{
    public class OpenAi : IConnector, AgentsManagement.IImageAnalyzer
    {
        private readonly ILogger<OpenAi> _logger;
        private readonly OpenAISemanticSettings _openAISettings;
        private OpenAIChatCompletionService _chatCompletionService { get; set; }

        /// <summary>
        /// Create a new instance of the OpenAIChatCompletionService with default model id.
        /// Look on settings JSON file and check what default model id is set.
        /// </summary>
        /// <param name="openAISettings"></param>
        /// <param name="logger"></param>
        public OpenAi(IOptions<OpenAISemanticSettings> openAISettings, ILogger<OpenAi> logger)
{
            _openAISettings = openAISettings.Value;

            _logger = logger;

            if (
                string.IsNullOrEmpty(_openAISettings.DefaultModelId) ||
                string.IsNullOrEmpty(_openAISettings.ApiKey)
                )
            {
                throw new Exception("Your settings file doesn't have OpenAi data.");
            }

            _chatCompletionService = new OpenAIChatCompletionService(
                modelId: _openAISettings.DefaultModelId,
                apiKey: _openAISettings.ApiKey,
                organization: _openAISettings.OrganizationId  // Optional
                                                              //loggerFactory: logger  
                                                              // TODO: Add logger factory in DI
            );
        }

        /// <summary>
        /// Changes the model ID for the OpenAIChatCompletionService.
        /// </summary>
        /// <param name="newModelId">The new model ID to use.</param>
        public void ChangeModelId(string newModelId)
        {
            _chatCompletionService = new OpenAIChatCompletionService(
                modelId: newModelId,
                apiKey: _openAISettings.ApiKey,
                organization: _openAISettings.OrganizationId // Optional

            //loggerFactory: logger
            );

            _logger.LogInformation($"Changed modelId for OpenAI to: {newModelId}");
        }

        public IChatCompletionService GetChatCompletionService() => _chatCompletionService;

        public string GetModelId() => _chatCompletionService.GetModelId();

    }
}
