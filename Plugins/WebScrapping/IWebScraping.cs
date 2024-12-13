namespace SemanticKernelPersonalized.Plugins.WebScrapping
{
    public interface IWebScraping
    {
        Task<string> ScrapingPageAsync(string url);
        Task<string> MapPageAsync(
           string url,
           string? search = null,
           bool ignoreSitemap = true,
           bool sitemapOnly = false,
           bool includeSubdomains = false,
           int limit = 5000
       );
    }
}