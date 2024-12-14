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

    /// <summary>
    /// Asynchronously scrapes content from a specified URL using the web scraping API.
    /// </summary>
    /// <param name="url">The URL of the webpage to scrape. Required parameter.</param>
    /// <param name="formats">Formats to include in the output. Available options: markdown, html, rawHtml, links, screenshot, extract, screenshot@fullPage. Default is markdown.</param>
    /// <param name="onlyMainContent">Only return the main content of the page excluding headers, navs, footers, etc. Default is true.</param>
    /// <param name="includeTags">Tags to include in the output.</param>
    /// <param name="excludeTags">Tags to exclude from the output.</param>
    /// <param name="headers">Headers to send with the request. Can be used to send cookies, user-agent, etc.</param>
    /// <param name="waitFor">Specify a delay in milliseconds before fetching the content, allowing the page sufficient time to load. Default is 0.</param>
    /// <param name="mobile">Set to true if you want to emulate scraping from a mobile device. Useful for testing responsive pages and taking mobile screenshots. Default is false.</param>
    /// <param name="skipTlsVerification">Skip TLS certificate verification when making requests. Default is false.</param>
    /// <param name="timeout">Timeout in milliseconds for the request. Default is 30000.</param>
    /// <param name="extract">Extract object containing extraction parameters.</param>
    /// <param name="actions">Actions to perform on the page before grabbing the content.</param>
    /// <returns>A string containing the scraped content in the requested format(s).</returns>
    /// <remarks>
    /// This method supports various scraping options including:
    /// - Multiple output formats (markdown, HTML, rawHTML, links, screenshots, extract, screenshot@fullPage)
    /// - Content filtering (main content only, specific tags inclusion/exclusion)
    /// - Location and language emulation
    /// - Mobile device emulation
    /// - Custom HTTP headers
    /// - Base64 image handling
    /// 
    /// Default behavior:
    /// - Returns markdown format
    /// - Extracts main content only (excludes headers, footers, navigation)
    /// - Uses PL location with Polish language
    /// - Desktop device emulation
    /// - Removes base64 encoded images
    /// </remarks>
    /// <exception cref="HttpRequestException">
    /// Thrown when the API request fails, including the status code and error message from the server.
    /// </exception>
    public async Task<string> ScrapingPageAsync(
        string url,
        string[]? formats = null,
        bool onlyMainContent = true,
        string[] includeTags = null,
        string[] excludeTags = null,
        //object headers = null,
        int waitFor = 0,
        bool mobile = false,
        bool skipTlsVerification = false,
        int timeout = 30000
        //object extract = null,
        //object[] actions = null
        )
    {
        var supportedFormats = new[] { "markdown", "html", "rawHtml", "links", "screenshot", "extract", "screenshot@fullPage" };

        // Check if the requested formats are supported
        if (formats != null && formats.Any(format => !supportedFormats.Contains(format)))
        {
            throw new ArgumentException("The requested format(s) are not supported for scraping. Supported formats are: " +
                string.Join(", ", supportedFormats));
        }

        // Construct the request payload with all available options
        var payload = new
        {
            url = url,  // Required: Target URL to scrape
            formats = formats ?? new[] { "markdown" },  // Output format(s), default to markdown if null
            onlyMainContent = onlyMainContent,  // Excludes headers, navs, footers based on parameter
            includeTags = includeTags ?? new string[] { },  // Tags to include in the output
            excludeTags = excludeTags ?? new string[] { },  // Tags to exclude from the output
            //headers = headers ?? new { },  // Optional: Custom HTTP headers for the request
            waitFor = waitFor,  // Delay in milliseconds before scraping
            mobile = mobile,  // Desktop/Mobile device emulation
            skipTlsVerification = skipTlsVerification,  // Skip TLS verification
            timeout = timeout,  // Timeout for the request
            //extract = extract,  // Extract object containing extraction parameters
            //actions = actions  // Actions to perform on the page before grabbing the content
        };

        // Serialize payload to JSON and prepare the HTTP content
        var content = new StringContent(
            JsonConvert.SerializeObject(payload),
            Encoding.UTF8,
            "application/json"
        );

        // Send POST request to the scraping API
        var response = await _httpClient.PostAsync(_settings.ApiUrl, content);

        // Handle successful response
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }

        // Handle error response with detailed exception
        var errorContent = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"Error: {response.StatusCode}, Failed to scrape page: {response.ReasonPhrase}, Content: {errorContent}"
        );
    }

    /// <summary>
    /// Maps a webpage starting from the specified URL with optional search query and additional parameters.
    /// </summary>
    /// <param name="url">The base URL to start crawling from.</param>
    /// <param name="search">Search query to use for mapping. Limited to 1000 search results during the Alpha phase.</param>
    /// <param name="ignoreSitemap">Ignore the website sitemap when crawling.</param>
    /// <param name="sitemapOnly">Only return links found in the website sitemap.</param>
    /// <param name="includeSubdomains">Include subdomains of the website.</param>
    /// <param name="limit">Maximum number of links to return. Required range: x < 5000.</param>
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
