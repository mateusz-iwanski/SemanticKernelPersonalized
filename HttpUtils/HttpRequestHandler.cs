using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SemanticKernelPersonalized.HttpUtils
{
    public class HttpRequestHandler : IDisposable
    {
        private HttpClientHandler _handler;
        private HttpClient _client;

        //private readonly ILogger _logger;

        public HttpRequestHandler()//ILogger logger)
        {
            //_logger = logger;
        }

        public void Setup(bool allowAutoRedirect, bool useCookies)
        {
            _handler = new HttpClientHandler
            {
                AllowAutoRedirect = allowAutoRedirect,
                UseCookies = useCookies,
                CookieContainer = new CookieContainer()
            };

            _client = new HttpClient(_handler);

            //_logger.Info("HttpClient setup completed with AllowAutoRedirect: {0}, UseCookies: {1}", allowAutoRedirect, useCookies);
        }

        public async Task<string> LoginAndGetRedirectedPageAsync(string url, Dictionary<string, string> loginCredentials, Firecrawl firecrawl)
        {
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var formData = new FormUrlEncodedContent(loginCredentials);

            //_logger.Info("Sending POST request to {0}", url);
            var response = await _client.PostAsync(url, formData);

            if (response.IsSuccessStatusCode)
            {
                //_logger.Info("Login successful, following redirection.");
                var redirectedResponse = await _client.GetAsync(response.RequestMessage.RequestUri);

                string resultAsMarkdown = await firecrawl.ScrapingPageAsync(response.RequestMessage.RequestUri.ToString());

                if (redirectedResponse.IsSuccessStatusCode)
                {
                  //  _logger.Info("Redirection successful, returning page content.");
                    return resultAsMarkdown;
                }
                else
                {
                    //_logger.Error("Failed to get redirected page: {0}", redirectedResponse.ReasonPhrase);
                    throw new HttpRequestException($"Error: {redirectedResponse.StatusCode}, Failed to get redirected page: {redirectedResponse.ReasonPhrase}");
                }
            }
            else
            {
                //_logger.Error("Failed to login: {0}", response.ReasonPhrase);
                throw new HttpRequestException($"Error: {response.StatusCode}, Failed to login: {response.ReasonPhrase}");
            }
        }

        public async Task<HttpResponseMessage> LoginAndGetRedirectedPageAsync(string url, Dictionary<string, string> loginCredentials)
        {
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var formData = new FormUrlEncodedContent(loginCredentials);

            //_logger.Info("Sending POST request to {0}", url);
            var response = await _client.PostAsync(url, formData);

            if (response.IsSuccessStatusCode)
            {
                //_logger.Info("Login successful.");
                return response;
            }
            else
            {
                //_logger.Error("Failed to login: {0}", response.ReasonPhrase);
                throw new HttpRequestException($"Error: {response.StatusCode}, Failed to login: {response.ReasonPhrase}");
            }
        }

        public async Task DownloadFileAsync(string fileUrl, string destinationPath, bool showHeaders = false)
        {
            //_logger.Info("Starting file download from {0}", fileUrl);
            using (var response = await _client.GetAsync(fileUrl))
            {
                response.EnsureSuccessStatusCode();

                if (showHeaders)
                {
                    foreach (var header in response.Headers)
                    {
                        Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
                    }
                }

                using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await response.Content.CopyToAsync(fileStream);
                }
                //_logger.Info("File downloaded successfully to {0}", destinationPath);
            }
        }

        ~HttpRequestHandler()
        {
            Dispose();
        }

        public void Dispose()
        {
            _client?.Dispose();
            _handler?.Dispose();
           // _logger.Info("HttpRequestHandler disposed.");
        }
    }
}
