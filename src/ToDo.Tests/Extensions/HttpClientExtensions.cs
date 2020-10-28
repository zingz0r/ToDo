using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ToDo.Tests.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<TResult> GetAsync<TResult>(this HttpClient client, string url, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to send request to {url}. Error: {e.Message}");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Url GET failed from {url}. ErrorCode: {response.StatusCode}");
            }

            return JsonConvert.DeserializeObject<TResult>(await response.Content.ReadAsStringAsync());
        }

        public static async Task PostAsync<TResult>(this HttpClient client, string url,
            object payload, CancellationToken cancellationToken)
        {

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to send request to {url}. Error: {e.Message}");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Url POST failed from {url}. ErrorCode: {response.StatusCode}");
            }
        }

        public static async Task PatchAsync<TResult>(this HttpClient client, string url,
            object payload, CancellationToken cancellationToken)
        {

            var request = new HttpRequestMessage(HttpMethod.Patch, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to send request to {url}. Error: {e.Message}");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Url PATCH failed from {url}. ErrorCode: {response.StatusCode}");
            }
        }

        public static async Task DeleteAsync(this HttpClient client, string url, CancellationToken cancellationToken)
        {

            var request = new HttpRequestMessage(HttpMethod.Delete, url);

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to send request to {url}. Error: {e.Message}");
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Url DELETE failed from {url}. ErrorCode: {response.StatusCode}");
            }
        }
    }
}