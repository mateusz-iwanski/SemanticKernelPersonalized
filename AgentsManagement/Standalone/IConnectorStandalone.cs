using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SemanticKernelPersonalized.AgentsManagement.Standalone
{
    public interface IConnectorStandalone
    {
        IChatCompletionService GetChatCompletionService();
        string GetModelId();
    }
}