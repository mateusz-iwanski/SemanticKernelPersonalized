using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Newtonsoft.Json;
using OpenAI.Assistants;
using SemanticKernelPersonalized.Agents;
using SemanticKernelPersonalized.Agents.KernelVersion;
using SemanticKernelPersonalized.Agents.KernelVersion.OpenAi;
using SemanticKernelPersonalized.AgentsManagement.Standalone;
using SemanticKernelPersonalized.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SemanticKernelPersonalized.AgentsManagement
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// ChatDialog collects basic information, metadata etc. which can be used to log in or save conversations. If you use telemetry, logging is handled by the
    /// Kernel, data from ChatDialog->ChatHistory should also be saved in another system. Data can be associated using the agent response id (gen_ai.response.id) saved in both systems.
    /// Connect data in the your systems - ChatHistory[]->(where role == assistant)->Metadata->gen_ai.response.id and conversation_uuid with the whole conversation.
    /// </remarks>
    internal class ChatDialog : ChatDialogBase, IChatDialogBase
    {
        private readonly IConnector _connector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatDialog"/> class.
        /// </summary>
        /// <param name="connector">The connector to use for chat completion (OpenAiStandalone, Mistral, etc.).</param>
        /// <param name="logger">The logger to use for logging.</param>
        public ChatDialog(IConnector connector, ILogger<IChatDialogBase> logger) : base(logger)
            => _connector = connector;

        protected override async Task<ChatMessageContent> Chat()
        {
            var chatCompletionService = _connector.GetChatCompletionService();
            if (chatCompletionService == null)
            {
                throw new InvalidOperationException("ChatCompletionService is not initialized.");
            }

            if (_chatHistory == null)
            {
                throw new InvalidOperationException("Chat history is not initialized.");
            }

            Kernel? kernel = _connector.GetKernel();
            ChatMessageContent response = null;

            // for standalone agent version
            if (kernel == null)
            {
                response = await chatCompletionService.GetChatMessageContentAsync(
                    chatHistory: _chatHistory,
                    executionSettings: _connector.GetPromptExecutionSettings(),
                    kernel: new Kernel()
                    );
            }
            // for Kernel agent version
            else
            {
                response = await chatCompletionService.GetChatMessageContentAsync(
                    chatHistory: _chatHistory,
                    executionSettings: _connector.GetPromptExecutionSettings(),
                    kernel: kernel
                    );
            }

            return response;
        }

        /// <summary>
        /// Checks if the connector is an image analyzer.
        /// </summary>
        /// <returns><c>true</c> if the connector is an image analyzer; otherwise, <c>false</c>.</returns>
        public bool IsImageAnalyzator() => _connector is IImageAnalyzer;

        /// <summary>
        /// Gets the connector used for chat completion.
        /// </summary>
        /// <returns>The connector instance.</returns>
        public IConnector GetConnector() => _connector;

        public override string GetModelId() => _connector.GetModelId();

    }
}
