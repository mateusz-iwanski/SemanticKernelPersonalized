using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SemanticKernelPersonalized.AgentsManagement.Standalone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelPersonalized.AgentsManagement
{
    public interface IChatDialogBase
    {
        void AddSystemMessage(
            string message,            
            Dictionary<string, object>? messageAdditionalMetadata = null,
            string? source = null,
            string? name = null,
            string? mimeType = null,
            string? author = null
            );

        Task<ChatMessageContent> GetChatMessageContentAsync(
            string message,
            PromptExecutionSettings? promptExecutionSettings = null,
            Kernel? kernel = null,
            Dictionary<string, object>? messageAdditionalMetadata = null,
            string? source = null,
            string? name = null,
            string? mimeType = null,
            string? outputMimeType = null,
            string? author = null
            );

        bool IsImageAnalyzator();
        string GetModelId();
        void ClearHistory();
        ChatHistory GetHistory();
        Guid GetUuid();
    }
}
