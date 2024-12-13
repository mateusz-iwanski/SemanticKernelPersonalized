# OpenAI Kernel Integration

This document provides a guide on how to integrate and use the OpenAI Kernel within your .NET 9 project.

## Setup

1. **Configure Dependency Injection:**
						
Ensure that your `Program.cs` or `Startup.cs` is configured to add the necessary services.
Look on main README.md for more information.

### Example Code

Below is an example of how to use the OpenAI Kern

var openai = host.Services.GetService<OpenAIKernel>();

var logger = host.Services.GetRequiredService<ILogger<ChatDialog>>();

// cahnge model if you want to, default is set in settings file
// openai.CustomizeModel = "gpt-3.5-turbo";

// configure settings
// openai.ConfigureSettings(...)

ChatDialog chatDialogKernel = new ChatDialog(openai, logger);

chatDialogKernel.AddSystemMessage("You are helpful assistant.");
var k = await chatDialogKernel.GetChatMessageContentAsync("pobierz url z https://rtk.pl/onas/");

Console.WriteLine(k.Content);


### Explanation

1. **Dependency Injection:**
   - `OpenAIKernel` and `ILogger` are retrieved from the dependency injection container.
   
2. **ChatDialog Initialization:**
   - An instance of `ChatDialog` is created using the `OpenAIKernel` and `ILogger`.

3. **Adding System Message:**
   - A system message is added to the chat dialog to set the context for the assistant.

4. **Fetching Chat Message Content:**
   - The `GetChatMessageContentAsync` method is called with a specific query, and the result is printed to the console.


   