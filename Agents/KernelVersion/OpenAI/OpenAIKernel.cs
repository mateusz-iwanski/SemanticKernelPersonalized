﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Services;
using SemanticKernelPersonalized.Plugins.WebScrapping;
using SemanticKernelPersonalized.Settings;



namespace SemanticKernelPersonalized.Agents.KernelVersion.OpenAi
{
    public class OpenAIKernel : IConnectorKernel
    {
        private readonly Kernel _kernel;

        public OpenAIKernel(
            IOptions<OpenAISemanticSettings> openAISettings, 
            IOptions<FirecrawlSemanticSettings> firecrawlSemanticSettings
            )
        {
            _kernel = new KernelBuilder().CreateKernelWithChatCompletion(
                openAISettings,
                firecrawlSemanticSettings
                );
        }
        public async Task<string> GetResponseAsync(string input)
        {
            //IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
            //kernelBuilder.AddOpenAIChatCompletion(
            //    modelId: "gpt-4",
            //    apiKey: _openAisettings.ApiKey,
            //    orgId: "", // Optional
            //    serviceId: "", // Optional; for targeting specific services within Semantic Kernel
            //    httpClient: new HttpClient() // Optional; if not provided, the HttpClient from the kernel will be used
            //);
            //Kernel kernel = kernelBuilder.Build();

            //kernel.Plugins.AddFromType<IWebScraping>("WebScrapping");

            //var kernel = new KernelBuilder().CreateKernelWithChatCompletion(_openAisettings, _firecrawlSemanticSettings);

            // Enable planning
            PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };

            string configPayload = """
            {
                "schema": 1,
                "name": "HelloAI",
                "description": "Say hello to an AI",
                "type": "completion",
                "completion": {
                "max_tokens": 256,
                "temperature": 0.5,
                "top_p": 0.0,
                "presence_penalty": 0.0,
                "frequency_penalty": 0.0
                }
            }
            """;
            //var promptConfig = JsonSerializer.Deserialize<PromptTemplateConfig>(configPayload)!;

            //promptConfig.Template = "pobierz url z www.rtk.pl";
            //var func = kernel.CreateFunctionFromPrompt(promptConfig);
            //var result = await kernel.InvokeAsync(func);

            var result = await kernel.InvokePromptAsync("pobierz url z www.rtk.pl", new(settings));
            Console.WriteLine(result.GetValue<string>());

            return result.GetValue<string>();
        }

        public string? GetModelId()
        {
            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            return chatCompletionService.GetModelId();
        }
    }
}