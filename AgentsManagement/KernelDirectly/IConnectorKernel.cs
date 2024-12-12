using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SemanticKernelPersonalized
{
    public interface IConnectorKernel
    {
        string GetModelId();
    }
}