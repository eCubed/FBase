using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FBase.ApiClient
{
    public class HttpRequestBuilder
    {
        private HttpMethod Method { get; set; }
        private string Url { get; set; }
        private HttpContent Content { get; set; }
        private string BearerToken { get; set; }
        private string AcceptHeader { get; set; }
        private List<KeyValuePair<string, string>> Headers { get; set; }

        public HttpRequestBuilder()
        {
            Url = "";
            AcceptHeader = "application/json";
        }

        public HttpRequestBuilder SetMethod(HttpMethod method)
        {
            Method = method;
            return this;
        }

        public HttpRequestBuilder SetUrl(string url)
        {
            Url = url;
            return this;
        }

        public HttpRequestBuilder SetContent(HttpContent content)
        {
            Content = content;
            return this;
        }

        public HttpRequestBuilder SetBearerToken(string bearerToken)
        {
            BearerToken = bearerToken;
            return this;
        }

        public HttpRequestBuilder SetAcceptHeader(string acceptHeader)
        {
            AcceptHeader = acceptHeader;
            return this;
        }

        public HttpRequestBuilder AddHeader(KeyValuePair<string, string> header)
        {
            if (Headers == null)
                Headers = new List<KeyValuePair<string, string>>();

            Headers.Add(header);
            return this;
        }

        public HttpRequestBuilder AddHeaders(List<KeyValuePair<string, string>> headers)
        {
            if (Headers == null)
                Headers = new List<KeyValuePair<string, string>>();

            if (headers != null)
            {
                headers.ForEach(header =>
                {
                    Headers.Add(header);
                });
            }

            return this;
        }

        /// <summary>
        /// This function actually is the one that creates the HttpRequestMessage objects internally
        /// depending on the availability of the property values that may have been set.
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SendAsync(HttpClient client)
        {
            var request = new HttpRequestMessage { Method = Method, RequestUri = new Uri(this.Url) };

            // The auth bearer if specified
            if (!string.IsNullOrEmpty(BearerToken))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", BearerToken);

            // Other headers.
            if (Headers != null)
            {
                Headers.ForEach(headerKvp =>
                {
                    request.Headers.Add(headerKvp.Key, headerKvp.Value);
                });
            }

            // Accept headers
            request.Headers.Accept.Clear();
            if (!string.IsNullOrEmpty(AcceptHeader))
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(AcceptHeader));

            // The payload if any.
            if (Content != null)
                request.Content = Content;

            return await client.SendAsync(request);
          
        }
    }
}
