﻿namespace SemanticKernelPersonalized.Plugins.WebScrapping
{
    public interface IWebScraping
    {
        Task<string> ScrapingPageAsync(
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
        );

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