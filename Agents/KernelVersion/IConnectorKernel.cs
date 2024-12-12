using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SemanticKernelPersonalized.Agents.KernelVersion
{
    public interface IConnectorKernel
    {
        public void CustomizeModel(string modelId);
        string GetModelId();
    }
}