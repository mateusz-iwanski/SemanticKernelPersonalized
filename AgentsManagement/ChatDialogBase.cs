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
    public abstract class ChatDialogBase
    {
        protected ChatHistory _chatHistory { get; set; }

        public readonly Guid Uuid;

        public ChatDialogBase(ILogger<ChatDialogStandAlone> logger)
        {
            _chatHistory = new ChatHistory();
            Uuid = Guid.NewGuid();
        }

        abstract protected Task<ChatMessageContent> Chat(
            PromptExecutionSettings? promptExecutionSettings = null,
            Kernel? kernel = null
            );

        abstract public string GetModelId();

        /// <summary>
        /// Add request history -> sends a chat message and gets a response asynchronously -> add assistant history.
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
            var response = await Chat(promptExecutionSettings, kernel);

            // Add assistant message to the chat history
            addMessage(
                AuthorRole.Assistant,
                response.Content, // Fix: Use response.Content instead of response
                this,
                null,
                GetModelId(),
                name,
                mimeType,
                author
                );

            return response;
        }

        /// <summary>
        /// Adds a system message to the chat history.
        /// </summary>
        /// <param name="message">The message to send as plain text.</param>
        /// <param name="messageAdditionalMetadata">Additional metadata for the message (whatever you want).</param>
        /// <param name="source">The source of the message (www, email, etc.).</param>
        /// <param name="name">The name of the message (greeting, trigger, system_notification, error, etc.).</param>
        /// <param name="mimeType">The MIME type of the message (use MimeTypes).</param>
        /// <param name="author">The author of the message.</param>
        public virtual void addMessage(
            AuthorRole role,
            string message,
            ChatDialogBase dialog,
            Dictionary<string, object>? messageAdditionalMetadata = null,            
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
        public virtual void AddSystemMessage(
            string message,
            ChatDialogBase dialog,
            Dictionary<string, object>? messageAdditionalMetadata = null,
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
        /// Gets the UUID.
        /// </summary>
        public virtual Guid GetUuid() => Uuid;
    }
}
