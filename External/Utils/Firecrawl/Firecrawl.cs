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

    /// <summary>
    /// Maps a webpage starting from the specified URL with optional search query and additional parameters.
    /// </summary>
    /// <param name="url">The base URL to start crawling from.</param>
    /// <param name="search">Search query to use for mapping. Limited to 1000 search results during the Alpha phase.</param>
    /// <param name="ignoreSitemap">Ignore the website sitemap when crawling. Default is false.</param>
    /// <param name="sitemapOnly">Only return links found in the website sitemap. Default is false.</param>
    /// <param name="includeSubdomains">Include subdomains of the website. Default is false.</param>
    /// <param name="limit">Maximum number of links to return. Required range: x < 5000. Default is 5000.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the mapped page content as a string.</returns>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails.</exception>
    public async Task<string> MapPageAsync(
        string url,
        string? search = null,
        bool ignoreSitemap = false,
        bool sitemapOnly = false,
        bool includeSubdomains = false,
        int limit = 5000
    )
    {
        if (limit > 5000)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), "Limit must be less than or equal to 5000.");
        }

        var payload = new
        {
            url = url,
            search = search ?? "",
            ignoreSitemap = ignoreSitemap,
            sitemapOnly = sitemapOnly,
            includeSubdomains = includeSubdomains,
            limit = limit
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
