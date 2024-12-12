
# Usage

Add AI agent as standalone instance

## Typical usage

This demonstrates how to create an instance of an AI agent as standalone instance and interact with it to get responses based on chat history.
Use them directly in your code without injecting them into the kernel.
Use the `ChatDialogStandAlone` class to interact with the AI agent.
Rather use builtin agent directly in kernel, but if there is not such agent, you can use standalone instance.

### Example

var host = Host.CreateDefaultBuilder(args)
.ConfigureAppConfiguration((context, config) =>
{
    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    config.AddEnvironmentVariables();
    if (args != null)
    {
        config.AddCommandLine(args);
    }
})

.ConfigureServices((context, services) =>
{
    var sematicKernel = new SemanticKernelServicesBuilder();
    sematicKernel.ConfigureServices(context, services);
})
.Build();

var serviceProvider = host.Services;

var factory = serviceProvider.GetRequiredService<IDependencyConnectorFactory>();
var connector = factory.GetChatConnector("OpenAI");
var dialog = new ChatDialogStandAlone(connector, serviceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<ChatDialog>>());
var responses = await dialog.GetChatMessageContentAsync(
    message: "What is the capital of Poland?"
);

## Custom Plugins usage

This demonstrates how to create an instance of an AI agent using the Semantic Kernel plugin (Firecrawl service) and interact with it to get responses based on chat history.

### Example

The following code snippet shows how to create an instance of an AI agent and use the Semantic Kernel plugin to interact with it:

// IDependencyConnectorFactory is a service that allows you to create an instance of one of the chat connector.
// It can be more than one chat connector, for example, OpenAI, GPT-3 etc.
var factory = serviceProvider.GetRequiredService<IDependencyConnectorFactory>();

// GetChatConnector method creates an instance of the chat connector with specific agent.
var connector = factory.GetChatConnector("OpenAI");

// ChatDialog is a class that allows you to interact with the models.
var dialog = new ChatDialog(connector, serviceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<ChatDialog>>());

// AddSystemMessage method adds a system message to the chat history.
dialog.AddSystemMessage("You are verry helpful assistant");

// OpenAIPromptExecutionSettings is a class that allows you to set the execution settings for the OpenAI plugin.
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

// Create an instance of the kernel (Semantic Kernel).
var kernelBuilder = serviceProvider.GetRequiredService<IKernelBuilder>();

// Build the kernel.
var kernel = kernelBuilder.Build();

// GetChatMessageContentAsync use plugin (if it fits into the script) to get the response based on the chat history.
var responses = await dialog.GetChatMessageContentAsync(
    message: "show all links from the website www.rtk.pl",
    promptExecutionSettings: openAIPromptExecutionSettings,
    kernel: kernel
    );



