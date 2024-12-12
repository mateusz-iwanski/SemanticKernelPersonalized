using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI.VectorStores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemanticKernelPersonalized.AgentsManagement;
using System.Net.Mime;
using Microsoft.SemanticKernel.Text;

namespace SemanticKernelPersonalized.History
{
    /// <summary>
    /// The <see cref="ChatHistoryHandler"/> class provides methods to add text and image content to a chat history.
    /// </summary>
    public class ChatHistoryHandler
    {
        /// <summary>
        /// Adds text content to the chat history.
        /// </summary>
        /// <param name="chatHistory">The chat history to which the text content will be added.</param>
        /// <param name="dialog">The ChatDialogBase instance associated with the chat history.</param>
        /// <param name="role">The role of the author of the message.</param>
        /// <param name="message">The text message to be added.</param>
        /// <param name="messageAdditionalMetadata">Additional metadata for the message.</param>
        /// <param name="source">The source of the message.</param>
        /// <param name="name">The name associated with the message.</param>
        /// <param name="mimeType">The MIME type of the message, try to use MimeTypes.</param>
        /// <param name="author">The author of the message (default - unknown).</param>
        /// <remarks>
        /// This method creates a new text content item and adds it to the chat history with the specified metadata.
        /// </remarks>
        public static void AddTextContent(
            ChatHistory chatHistory,
            ChatDialogBase dialog,
            AuthorRole role,
            string message,
            Dictionary<string, object>? messageAdditionalMetadata = null,
            string? source = null,
            string? name = null,
            string? mimeType = null,
            string author = "unknown")
        {
            // TODO: Add LogRecord.TraceId - look : https://learn.microsoft.com/en-us/semantic-kernel/concepts/enterprise-readiness/observability/telemetry-with-console?tabs=Powershell-CreateFile%2CEnvironmentFile&pivots=programming-language-csharp
            chatHistory.Add(new()
            {
                Role = AuthorRole.User,
                MimeType = mimeType,
                Metadata = HistoryMetadataBuilder.MessageContent(
                    dialog: dialog,
                    author: author,
                    additionalMetaData: messageAdditionalMetadata,
                    source: source,
                    name: name
                    ),
                Items = new ChatMessageContentItemCollection
                {
                    new TextContent {
                        Text = message,
                        Metadata = HistoryMetadataBuilder.ItemContent(dialog, author)
                    },
                }
            });
        }

        /// <summary>
        /// Adds image content to the chat history. 
        /// </summary>
        /// <param name="chatHistory">The chat history to which the image content will be added.</param>
        /// <param name="dialog">The ChatDialogBase instance associated with the chat history.</param>
        /// <param name="role">The role of the author of the message.</param>
        /// <param name="message">The text message to be added along with the images.</param>
        /// <param name="imagePaths">A list of paths to the images to be added.</param>
        /// <param name="messageAdditionalMetadata">Additional metadata for the message.</param>
        /// <param name="source">The source of the message.</param>
        /// <param name="name">The name associated with the message.</param>
        /// <param name="imageMimeTypes">The MIME type of the images, try to use MimeTypes.</param>
        /// <param name="messageMimeTypes">The MIME type of the message.</param>
        /// <param name="author">The author of the message (default - unknow).</param>
        /// <remarks>        
        /// This method creates new image content items and adds them to the chat history along with the specified text message and metadata.
        /// Add a list with images (imagePaths) that have one type of mimetype, you have one type for whole list.
        /// </remarks>
        public static void AddImageContent(
            ChatHistory chatHistory,
            ChatDialogBase dialog,
            AuthorRole role,
            string message,
            List<string> imagePaths,
            Dictionary<string, object>? messageAdditionalMetadata,
            string? source,
            string? name,
            string? imageMimeTypes,
            string? messageMimeTypes,
            string author = "unknown")
        {

            List<ImageContent> listimage = new List<ImageContent>();
            imagePaths.ToList().ForEach(x => listimage.Add(new ImageContent
            {
                MimeType = imageMimeTypes,
                Uri = new Uri(x),
                Metadata = HistoryMetadataBuilder.ItemContent(dialog, author)
            }));

            var items = new ChatMessageContentItemCollection
            {
                new TextContent
                {
                    MimeType = messageMimeTypes,
                    Text = message,
                    Metadata = HistoryMetadataBuilder.ItemContent(dialog, author)
                }
            };

            foreach (var image in listimage)
            {
                items.Add(image);
            }

            chatHistory.Add(new()
            {
                Role = AuthorRole.User,
                MimeType = "url",
                Metadata = HistoryMetadataBuilder.MessageContent(
                    dialog: dialog,
                    additionalMetaData: messageAdditionalMetadata,
                    author: author,
                    source: source,
                    name: name
                    ),
                Items = items
            });
        }
    }
}
