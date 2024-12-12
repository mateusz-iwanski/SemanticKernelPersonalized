using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SemanticKernelPersonalized.Agents;
using SemanticKernelPersonalized.AgentsManagement.Standalone;
using SemanticKernelPersonalized.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelPersonalized.AgentsManagement.KernelDirectly
{
    internal class ChatDialogKernel : ChatDialogBase, IChatDialogBase
    {
        private readonly IConnectorKernel _connector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatDialogStandAlone"/> class.
        /// </summary>
        /// <param name="connector">The connector to use for chat completion (OpenAiStandalone, Mistral, etc.).</param>
        /// <param name="logger">The logger to use for logging.</param>
        public ChatDialogKernel(IConnectorKernel connector, ILogger<ChatDialogStandAlone> logger) : base(logger)
            => _connector = connector;
        
        /// <summary>
        /// Sends a chat message and gets a response asynchronously.
        /// </summary>
        /// <param name="message">The message to send as plain text.</param>
        /// <param name="promptExecutionSettings">Provides execution settings for an AI request.</param>
        /// <param name="kernel">State for use throughout a Semantic Kernel workload.</param>
        /// <param name="messageAdditionalMetadata">Additional metadata for the message (whatever you want).</param>
        /// <param name="source">The source of the message (www, email, etc.).</param>
        /// <param name="name">The name of the message (greeting, trigger, system_notification, error, etc.).</param>
        /// <param name="mimeType">The MIME type of the message (use MimeTypes).</param>
        /// <param name="outputMimeType">The MIME type of the output message (use MimeTypes).</param>
        /// <param name="author">The author of the message.</param>
        /// <returns>The response from the chat completion service.</returns>
        public async Task<ChatMessageContent> GetChatMessageContentAsync(
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
            addMessage(
               AuthorRole.User,
               message,
               this,
               messageAdditionalMetadata,
               source,
               name,
               mimeType,
               author
               );

            var response = await Chat(promptExecutionSettings, kernel);

            addMessage(
               AuthorRole.Assistant,
               response.Content,
               this,
               null,
               GetModelId(),
               name,
               mimeType,
               author
               );

            return response;
        }

        protected override async Task<ChatMessageContent> Chat(
            PromptExecutionSettings? promptExecutionSettings = null,
            Kernel? kernel = null
            )
        {
            return new ChatMessageContent();
        }

        /// <summary>
        /// Checks if the connector is an image analyzer.
        /// </summary>
        /// <returns><c>true</c> if the connector is an image analyzer; otherwise, <c>false</c>.</returns>
        public bool IsImageAnalyzator() => _connector is IImageAnalyzer;

        /// <summary>
        /// Gets the model ID of the connector.
        /// </summary>
        /// <returns>The model ID of the connector.</returns>
        public override string GetModelId() => _connector.GetModelId();

        /// <summary>
        /// Gets the connector used for chat completion.
        /// </summary>
        /// <returns>The connector instance.</returns>
        public IConnectorKernel GetConnector() => _connector;
        
    }
}
