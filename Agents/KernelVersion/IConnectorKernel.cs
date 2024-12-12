using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SemanticKernelPersonalized.Agents.KernelVersion
{
    public interface IConnectorStandalone
    {
        IChatCompletionService GetChatCompletionService();
        string GetModelId();
    }
}