using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;

namespace SemanticKernelPersonalized.HttpUtils
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public ApiClient()
        {
        }

        public async Task<HttpResponseMessage> PostJsonAsync(string url, object data, string mediaType = "application/json", bool checkStatusCode=true)
        {
            var jsonContent = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonContent, Encoding.UTF8, mediaType);

            var response = await _httpClient.PostAsync(url, content);
            if (checkStatusCode) response.EnsureSuccessStatusCode();

            return response;
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return response;
        }

        public async Task<HttpResponseMessage> PostJsonAsync(string url, string dataRequest, string mediaType = "application/json")
        {
            try
            {
                JsonConvert.DeserializeObject<object>(dataRequest);
            }
            catch (System.Text.Json.JsonException)
            {
                throw new ArgumentException("The provided dataRequest is not a valid JSON string.");
            }

            var content = new StringContent(dataRequest, Encoding.UTF8, mediaType);

            var response = await _httpClient.PostAsync(url, content);
            
            response.EnsureSuccessStatusCode();

            return response;
        }

        public async Task<HttpResponseMessage> PostFileAsync(string url, Stream fileStream, string fileName, string model, string mediaType = "multipart/form-data")
        {
            using var content = new MultipartFormDataContent();
            using var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType);
            content.Add(fileContent, "file", fileName);
            content.Add(new StringContent(model), "model");

            var response = await _httpClient.PostAsync(url, content);
            readResponse(response);

            return response;
        }

        public async Task<T> ReadContentAsAsync<T>(HttpResponseMessage response)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public void SetAuthorizationHeader(string scheme, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(scheme, token);
        }   



        private async void readResponse(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Status Code: {response.StatusCode}");
            Console.WriteLine($"Response Content: {responseContent}");
            response.EnsureSuccessStatusCode();
        }
    }
}
