using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SemanticKernelPersonalized.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelPersonalized.AgentsManagement
{
    /// <summary>
    /// Abstract base class for managing chat dialogs.
    /// </summary>
    public abstract class ChatDialogBase
    {
        /// <summary>
        /// Gets or sets the chat history.
        /// </summary>
        protected ChatHistory _chatHistory { get; set; }

        /// <summary>
        /// Gets or sets the logger instance.
        /// </summary>
        protected ILogger<IChatDialogBase> _logger { get; set; }

        /// <summary>
        /// Gets the unique identifier for the chat session.
        /// </summary>
        public readonly Guid Uuid;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatDialogBase"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public ChatDialogBase(ILogger<IChatDialogBase> logger)
        {
            _chatHistory = new ChatHistory();
            _logger = logger;
            Uuid = Guid.NewGuid();
        }

        /// <summary>
        /// Sends a chat message and gets a response asynchronously.
        /// </summary>
        /// <returns>The response from the chat completion service.</returns>
        abstract protected Task<ChatMessageContent> Chat();

        /// <summary>
        /// Gets the model identifier.
        /// </summary>
        /// <returns>The model identifier.</returns>
        abstract public string GetModelId();

        /// <summary>
        /// Adds a user message to the chat history, sends a chat message, and gets a response asynchronously.
        /// </summary>
        /// <param name="message">The message to send as plain text.</param>
        /// <param name="promptExecutionSettings">Provides execution settings for an AI request.</param>
        /// <param name="kernel">State for use throughout a Semantic Kernel workload.</param>
        /// <param name="messageAdditionalMetadata">Additional metadata for the message (optional).</param>
        /// <param name="source">The source of the message (optional).</param>
        /// <param name="name">The name of the message (optional).</param>
        /// <param name="mimeType">The MIME type of the message (optional).</param>
        /// <param name="outputMimeType">The MIME type of the output message (optional).</param>
        /// <param name="author">The author of the message (optional).</param>
        /// <returns>The response from the chat completion service.</returns>
        /// <remarks>
        /// The conversation is marked in additional information with `gen_ai.response.id`, i.e., the conversation id from the AI agent response.
        /// This id can be used to merge information between the logging system (e.g., Application Insights) and another system that records the necessary information.
        /// </remarks>
        public virtual async Task<ChatMessageContent> GetChatMessageContentAsync(
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
            // Add user message to the chat history
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

            // Get response from the chat completion service
            var response = await Chat();
            // this will be used to store the response id
            object? agentAiResponseId = response.Metadata["Id"];

            // Add assistant message to the chat history
            addMessage(
                AuthorRole.Assistant,
                response.Content, // Fix: Use response.Content instead of response
                this,
                new Dictionary<string, object?>() { { "gen_ai.response.id", agentAiResponseId } },  // mark conversation
                GetModelId(),
                name,
                mimeType,
                author
                );

            return response;
        }

        /// <summary>
        /// Adds a message to the chat history.
        /// </summary>
        /// <param name="role">The role of the message author (User, Assistant, System).</param>
        /// <param name="message">The message content.</param>
        /// <param name="dialog">The chat dialog instance.</param>
        /// <param name="messageAdditionalMetadata">Additional metadata for the message (optional).</param>
        /// <param name="source">The source of the message (optional).</param>
        /// <param name="name">The name of the message (optional).</param>
        /// <param name="mimeType">The MIME type of the message (optional).</param>
        /// <param name="author">The author of the message (optional).</param>
        public virtual void addMessage(
            AuthorRole role,
            string message,
            ChatDialogBase dialog,
            Dictionary<string, object?>? messageAdditionalMetadata = null,
            string? source = null,
            string? name = null,
            string? mimeType = null,
            string? author = null
            )
        {
            ChatHistoryHandler.AddTextContent(
                chatHistory: _chatHistory,
                dialog: this,
                role: role,
                message: message,
                messageAdditionalMetadata: messageAdditionalMetadata,
                source: source,
                name: name,
                mimeType: mimeType,
                author: author
                );
        }

        /// <summary>
        /// Adds a system message to the chat history.
        /// </summary>
        /// <param name="message">The message content.</param>
        /// <param name="messageAdditionalMetadata">Additional metadata for the message (optional).</param>
        /// <param name="source">The source of the message (optional).</param>
        /// <param name="name">The name of the message (optional).</param>
        /// <param name="mimeType">The MIME type of the message (optional).</param>
        /// <param name="author">The author of the message (optional).</param>
        public virtual void AddSystemMessage(
            string message,
            Dictionary<string, object?>? messageAdditionalMetadata = null,
            string? source = null,
            string? name = null,
            string? mimeType = null,
            string? author = null
            )
        {
            addMessage(
                AuthorRole.System,
                message,
                this,
                messageAdditionalMetadata,
                source,
                name,
                mimeType,
                author
                );
        }

        /// <summary>
        /// Clears the chat history.
        /// </summary>
        public virtual void ClearHistory() => _chatHistory.Clear();

        /// <summary>
        /// Gets the chat history.
        /// </summary>
        /// <returns>The chat history.</returns>
        public virtual ChatHistory GetHistory() => _chatHistory;

        /// <summary>
        /// Gets the unique identifier for the chat session.
        /// </summary>
        /// <returns>The unique identifier.</returns>
        public virtual Guid GetUuid() => Uuid;
    }
}
