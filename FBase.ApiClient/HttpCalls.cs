using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FBase.ApiClient
{
    public static class HttpCalls
    {
        public static async Task<HttpResponseMessage> GetAsync(string url, string acceptHeader = null,
            string bearerToken = null, List<KeyValuePair<string, string>> headers = null)
        {
            var builder = new HttpRequestBuilder()
                    .SetMethod(HttpMethod.Get)
                    .SetUrl(url)
                    .SetAcceptHeader(acceptHeader)
                    .AddHeaders(headers)
                    .SetBearerToken(bearerToken);

            return await builder.SendAsync();
        }

        public static async Task<TResponse> GetAsync<TResponse>(string url, string acceptHeader = null,
            string bearerToken = null, List<KeyValuePair<string, string>> headers = null)
        {
            var httpResponseMessage = await GetAsync(url, acceptHeader, bearerToken, headers);

            return await httpResponseMessage.ContentAsTypeAsync<TResponse>();
        }

        public static async Task<HttpResponseMessage> PostAsync(string url, HttpContent content, string acceptHeader = null,
            string bearerToken = null, List<KeyValuePair<string, string>> headers = null)
        {
            var builder = new HttpRequestBuilder()
                    .SetMethod(HttpMethod.Post)
                    .SetUrl(url)
                    .SetAcceptHeader(acceptHeader)
                    .AddHeaders(headers)
                    .SetBearerToken(bearerToken)
                    .SetContent(content);

            return await builder.SendAsync();
        }

        public static async Task<TResponseData> PostAsync<TBody, TResponseData>(string url, TBody body, string acceptHeader = null,
            string bearerToken = null, List<KeyValuePair<string, string>> headers = null)
        {
            var httpResponseMessage = await PostAsync(url, new JsonContent<TBody>(body), acceptHeader, bearerToken, headers);

            return await httpResponseMessage.ContentAsTypeAsync<TResponseData>();
        }

        public static async Task<TResponseData> GetTokenAsync<TResponseData>(string tokenEndpoint, string identifier, string passcode, string grantType)
        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>((grantType == "password") ? "username" : "client_id", identifier));
            parameters.Add(new KeyValuePair<string, string>((grantType == "password") ? "password" : "client_secret", passcode));
            parameters.Add(new KeyValuePair<string, string>("grant_type", grantType));

            var httpResponseMessage = await PostAsync(tokenEndpoint, new FormUrlEncodedContent(parameters), "application/x-www-form-urlencoded");

            return await httpResponseMessage.ContentAsTypeAsync<TResponseData>();
        }

        public static async Task<TResponseData> UploadAsync<TResponseData>(string url, string absolutePath, string accessToken = "")
        {
            var httpResponseMessage = await PostAsync(url, new FileContent(absolutePath, "file"), acceptHeader: null, bearerToken: accessToken);

            return await httpResponseMessage.ContentAsTypeAsync<TResponseData>();
        }

        public static async Task<HttpResponseMessage> PutAsync(string url, HttpContent content, string acceptHeader = null,
            string bearerToken = null, List<KeyValuePair<string, string>> headers = null)
        {
            var builder = new HttpRequestBuilder()
                    .SetMethod(HttpMethod.Put)
                    .SetUrl(url)
                    .SetAcceptHeader(acceptHeader)
                    .AddHeaders(headers)
                    .SetBearerToken(bearerToken)
                    .SetContent(content);

            return await builder.SendAsync();
        }

        public static async Task<TResponseData> PutAsync<TBody, TResponseData>(string url, TBody body, string acceptHeader = null,
            string bearerToken = null, List<KeyValuePair<string, string>> headers = null)
        {
            var httpResponseMessage = await PutAsync(url, new JsonContent<TBody>(body));

            return await httpResponseMessage.ContentAsTypeAsync<TResponseData>();
        }

        public static async Task<HttpResponseMessage> DeleteAsync(string url, string acceptHeader = null,
            string bearerToken = null, List<KeyValuePair<string, string>> headers = null)
        {
            var builder = new HttpRequestBuilder()
                    .SetMethod(HttpMethod.Delete)
                    .SetUrl(url)
                    .SetAcceptHeader(acceptHeader)
                    .AddHeaders(headers)
                    .SetBearerToken(bearerToken);

            return await builder.SendAsync();
        }
    }
}
