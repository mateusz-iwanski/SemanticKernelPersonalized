//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;
//using System;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Reflection.PortableExecutable;
//using System.Text;
//using System.Threading.Tasks;

//namespace AiUtils.Firecrawl
//{
//    /// <summary>
//    /// payload = {
//    ///    "url": "<string>",
//    ///    "formats": ["markdown"],
//    ///    "onlyMainContent": True,
//    ///    "includeTags": ["<string>"],
//    ///    "excludeTags": ["<string>"],
//    ///    "headers": {},
//    ///    "waitFor": 123,
//    ///    "mobile": True,
//    ///    "skipTlsVerification": True,
//    ///    "timeout": 123,
//    ///    "extract": {
//    ///    "schema": { },
//    ///        "systemPrompt": "<string>",
//    ///        "prompt": "<string>"
//    ///    },
//    ///    "actions": [
//    ///        {
//    ///            "type": "wait",
//    ///            "milliseconds": 2,
//    ///            "selector": "#my-element"
//    ///        }
//    ///    ],
//    ///    "location": {
//    ///    "country": "<string>",
//    ///        "languages": ["en-US"]
//    ///    }
//    /// }
//    /// headers = {
//    ///    "Authorization": "Bearer <token>",
//    ///    "Content-Type": "application/json"
//    /// }
//    /// </summary>
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SemanticKernelPersonalized.Plugins.WebScrapping;
using SemanticKernelPersonalized.Settings;
using System.Net.Http.Headers;
using System.Text;

public class Firecrawl : IWebScraping
{
    private readonly HttpClient _httpClient;

    private readonly FirecrawlSemanticSettings _settings;

    public Firecrawl(IOptions<FirecrawlSemanticSettings> settings)
    {
        _httpClient = new HttpClient();
        _settings = settings.Value;

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
    }

    public async Task<string> ScrapingPageAsync(string url)
    {
        var payload = new
        {
            url = url,
            formats = new[] { "markdown" }
        };

        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_settings.ApiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error: {response.StatusCode}, Failed to scrape page: {response.ReasonPhrase}, Content: {errorContent}");
        }
    }

    public async Task<string> MapPageAsync(string url, string? search = null)
    {
        var payload = new
        {
            url = url,
            search = search ?? ""
        };

        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(_settings.ApiMapUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error: {response.StatusCode}, Failed to map page: {response.ReasonPhrase}, Content: {errorContent}");
        }
    }
}
