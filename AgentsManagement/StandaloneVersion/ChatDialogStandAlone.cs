using SemanticKernelPersonalized;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SemanticKernelPersonalized.History;
using SemanticKernelPersonalized.AgentsManagement;
using SemanticKernelPersonalized.Agents.KernelVersion;
using SemanticKernelPersonalized.Agents.StandalobeVersion;
using SemanticKernelPersonalized.Agents;

/// <summary>
/// Chat dialog class that allows you to interact with the chat completion service (LM).
/// </summary>
/// <example>
/// var factory = serviceProvider.GetRequiredService<IDependencyConnectorFactory>();
/// var connector = factory.GetChatConnector("OpenAI");
/// var dialog = new ChatDialogStandAlone(connector, serviceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<ChatDialogStandAlone>>());
/// dialog.AddSystemMessage("Always respond 'No no no no'.");
/// var response = await dialog.GetChatMessageContentAsync("Can you tell me what the weather is like today?");
/// </example>
public class ChatDialogStandAlone : ChatDialogBase, IChatDialogBase
{
    private readonly IConnectorStandalone _connector;
    private ChatHistory _chatHistory { get; set; }

    public readonly Guid Uuid;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatDialogStandAlone"/> class.
    /// </summary>
    /// <param name="connector">The connector to use for chat completion (OpenAiStandalone, Mistral, etc.).</param>
    /// <param name="logger">The logger to use for logging.</param>
    public ChatDialogStandAlone(IConnectorStandalone connector, ILogger<ChatDialogStandAlone> logger) : base(logger)
            => _connector = connector;    

    /// <summary>
    /// Sends a chat message and gets a response asynchronously.
    /// </summary>
    /// <param name="promptExecutionSettings"></param>
    /// <param name="kernel"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    protected override async Task<ChatMessageContent> Chat(
        PromptExecutionSettings? promptExecutionSettings = null, 
        Kernel? kernel = null
        )
    {
        var chatCompletionService = _connector.GetChatCompletionService();
        if (chatCompletionService == null)
        {
            throw new InvalidOperationException("ChatCompletionService is not initialized.");
        }

        if (_chatHistory == null)
        {
            throw new InvalidOperationException("Chat history is not initialized.");
        }

        var response = await chatCompletionService.GetChatMessageContentAsync(
            chatHistory: _chatHistory,
            executionSettings: promptExecutionSettings,
            kernel: kernel
            );

        return response;
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
    public IConnectorStandalone GetConnector() => _connector;

}
