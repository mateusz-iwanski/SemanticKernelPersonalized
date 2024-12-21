using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SemanticKernelPersonalized.AgentsManagement;

namespace SemanticKernelPersonalized.Agents
{
    public interface IConnector
    {
        public IChatCompletionService GetChatCompletionService();
        public void CustomizeModel(string modelId);
        public string GetModelId();
        public Kernel? GetKernel();
        public void ConfigureSettings(
            FunctionChoiceBehavior functionChoiceBehavior,
            int maxTokens,
            double temperature,
            double topP,
            double frequencyPenalty,
            double presencePenalty,
            List<string> stopSequences,
            Dictionary<int, int> tokenSelectionBiases
        );
        public OpenAIPromptExecutionSettings GetPromptExecutionSettings();
        public ChatDialog getConversationHistory();
    }
}