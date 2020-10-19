using FBase.ApiServer;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FBase.ApiClient
{
    public static class HttpCalls
    {
        #region GET

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

        public static async Task<TypedHttpResponseMessage<TResponseBodyType, TErrorType>> GetAsync<TResponseBodyType, TErrorType>(string url, string acceptHeader = null,
            string bearerToken = null, List<KeyValuePair<string, string>> headers = null)
        {
            var httpResponseMessage = await GetAsync(url, acceptHeader, bearerToken, headers);
            var response = new TypedHttpResponseMessage<TResponseBodyType, TErrorType>();
            await response.SetAsync(httpResponseMessage);
            return response;
        }

        public static async Task<TypedHttpResponseMessage<TResponseBodyType, TErrorType>> GetAsync<TResponseBodyType, TErrorType>(string url, 
            string apiKey, string secret, string arbitraryData, IApiClientHasher apiClientHasher, string acceptHeader = null, List<KeyValuePair<string, string>> additionalHeaders = null)
        {
            var apiKeyClientHeaders = Utils.PrepareApiKeyClientCallHeaders(apiKey, secret, arbitraryData, apiClientHasher, additionalHeaders);

            var httpResponseMessage = await GetAsync(url, acceptHeader, headers: apiKeyClientHeaders);
            var response = new TypedHttpResponseMessage<TResponseBodyType, TErrorType>();
            await response.SetAsync(httpResponseMessage);
            return response;
        }

        #endregion

        #region POST
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

        public static async Task<TypedHttpResponseMessage<TResponseBodyType, TErrorType>> PostHttpContentAsync<TContentBody, TResponseBodyType, TErrorType>(string url, TContentBody contentBody,
            string apiKey, string secret, string arbitraryData, IApiClientHasher apiClientHasher, string acceptHeader = null, List<KeyValuePair<string, string>> additionalHeaders = null)
            where TContentBody : HttpContent
        {
            var apiKeyClientHeaders = Utils.PrepareApiKeyClientCallHeaders(apiKey, secret, arbitraryData, apiClientHasher, additionalHeaders);

            var httpResponseMessage = await PostAsync(url, contentBody, acceptHeader, headers: apiKeyClientHeaders);

            var response = new TypedHttpResponseMessage<TResponseBodyType, TErrorType>();
            await response.SetAsync(httpResponseMessage);
            return response;
        }

        public static async Task<TypedHttpResponseMessage<TResponseBodyType, TErrorType>> PostHttpContentAsync<TContentBody, TResponseBodyType, TErrorType>(string url, TContentBody contentBody, string acceptHeader = null,
            string bearerToken = null, List<KeyValuePair<string, string>> headers = null)
            where TContentBody : HttpContent
        {
            var httpResponseMessage = await PostAsync(url, contentBody, acceptHeader, bearerToken, headers);

            var response = new TypedHttpResponseMessage<TResponseBodyType, TErrorType>();
            await response.SetAsync(httpResponseMessage);
            return response;
        }

        public static async Task<TypedHttpResponseMessage<TResponseBodyType, TErrorType>> PostAsync<TRequestBodyType, TResponseBodyType, TErrorType>(string url, TRequestBodyType body, string acceptHeader = null,
            string bearerToken = null, List<KeyValuePair<string, string>> headers = null)
        {
            var httpResponseMessage = await PostAsync(url, new JsonContent<TRequestBodyType>(body), acceptHeader, bearerToken, headers);
            var response = new TypedHttpResponseMessage<TResponseBodyType, TErrorType>();
            await response.SetAsync(httpResponseMessage);
            return response;
        }

        public static async Task<TypedHttpResponseMessage<TResponseBodyType, TErrorType>> PostAsync<TRequestBodyType, TResponseBodyType, TErrorType>(string url, TRequestBodyType body,
            string apiKey, string secret, string arbitraryData, IApiClientHasher apiClientHasher, string acceptHeader = null, List<KeyValuePair<string, string>> additionalHeaders = null)
        {
            var apiKeyClientHeaders = Utils.PrepareApiKeyClientCallHeaders(apiKey, secret, arbitraryData, apiClientHasher, additionalHeaders);

            var httpResponseMessage = await PostAsync(url, new JsonContent<TRequestBodyType>(body), acceptHeader, headers: apiKeyClientHeaders);
            var response = new TypedHttpResponseMessage<TResponseBodyType, TErrorType>();
            await response.SetAsync(httpResponseMessage);
            return response;
        }

        #endregion

        #region POST - Upload
        public static async Task<TypedHttpResponseMessage<TResponseBodyType, TErrorType>> UploadAsync<TResponseBodyType, TErrorType>(string url, string absolutePath, string accessToken = "")
        {
            var httpResponseMessage = await PostAsync(url, new FileContent(absolutePath, "file"), acceptHeader: null, bearerToken: accessToken);
            var response = new TypedHttpResponseMessage<TResponseBodyType, TErrorType>();
            await response.SetAsync(httpResponseMessage);
            return response;
        }

        public static async Task<TypedHttpResponseMessage<TResponseBodyType, TErrorType>> UploadAsync<TResponseBodyType, TErrorType>(string url, string absolutePath, 
            string apiKey, string secret, string arbitraryData, IApiClientHasher apiClientHasher, List<KeyValuePair<string, string>> additionalHeaders)
        {
            var apiKeyClientHeaders = Utils.PrepareApiKeyClientCallHeaders(apiKey, secret, arbitraryData, apiClientHasher, additionalHeaders);

            var httpResponseMessage = await PostAsync(url, new FileContent(absolutePath, "file"), headers: apiKeyClientHeaders);
            var response = new TypedHttpResponseMessage<TResponseBodyType, TErrorType>();
            await response.SetAsync(httpResponseMessage);
            return response;
        }

        #endregion

        #region PUT

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

        public static async Task<TypedHttpResponseMessage<TResponseBodyType, TErrorType>> PutAsync<TRequestBodyType, TResponseBodyType, TErrorType>(string url, TRequestBodyType body, string acceptHeader = null,
            string bearerToken = null, List<KeyValuePair<string, string>> headers = null)
        {
            var httpResponseMessage = await PutAsync(url, new JsonContent<TRequestBodyType>(body), acceptHeader, bearerToken, headers);
            var response = new TypedHttpResponseMessage<TResponseBodyType, TErrorType>();
            await response.SetAsync(httpResponseMessage);
            return response;
        }

        public static async Task<TypedHttpResponseMessage<TResponseBodyType, TErrorType>> PutAsync<TRequestBodyType, TResponseBodyType, TErrorType>(string url, TRequestBodyType body, 
            string apiKey, string secret, string arbitraryData, IApiClientHasher apiClientHasher, string acceptHeader = null, List<KeyValuePair<string, string>> additionalHeaders = null)
        {
            var apiKeyClientHeaders = Utils.PrepareApiKeyClientCallHeaders(apiKey, secret, arbitraryData, apiClientHasher, additionalHeaders);

            var httpResponseMessage = await PutAsync(url, new JsonContent<TRequestBodyType>(body), acceptHeader, headers: apiKeyClientHeaders);
            var response = new TypedHttpResponseMessage<TResponseBodyType, TErrorType>();
            await response.SetAsync(httpResponseMessage);
            return response;
        }

        #endregion

        #region DELETE

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


        public static async Task<TypedHttpResponseMessage<TResponseBodyType, TErrorType>> DeleteAsync<TResponseBodyType, TErrorType>(string url, string acceptHeader = null,
            string bearerToken = null, List<KeyValuePair<string, string>> headers = null)
        {
            var httpResponseMessage = await DeleteAsync(url, acceptHeader, bearerToken, headers);
            var response = new TypedHttpResponseMessage<TResponseBodyType, TErrorType>();
            await response.SetAsync(httpResponseMessage);
            return response;
        }

        public static async Task<TypedHttpResponseMessage<TResponseBodyType, TErrorType>> DeleteAsync<TResponseBodyType, TErrorType>(string url, 
            string apiKey, string secret, string arbitraryData, IApiClientHasher apiClientHasher, string acceptHeader = null, List<KeyValuePair<string, string>> additionalHeaders = null)
        {
            var apiKeyClientHeaders = Utils.PrepareApiKeyClientCallHeaders(apiKey, secret, arbitraryData, apiClientHasher, additionalHeaders);

            var httpResponseMessage = await DeleteAsync(url, acceptHeader, headers: apiKeyClientHeaders);
            var response = new TypedHttpResponseMessage<TResponseBodyType, TErrorType>();
            await response.SetAsync(httpResponseMessage);
            return response;
        }

        #endregion


        /*
        public static async Task<TypedHttpResponseMessage<TResponseBodyType, TErrorType>> GetTokenAsync<TResponseBodyType, TErrorType>(string tokenEndpoint, string identifier, string passcode, string grantType)

        {
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>((grantType == "password") ? "username" : "client_id", identifier));
            parameters.Add(new KeyValuePair<string, string>((grantType == "password") ? "password" : "client_secret", passcode));
            parameters.Add(new KeyValuePair<string, string>("grant_type", grantType));

            var httpResponseMessage = await PostAsync(tokenEndpoint, new FormUrlEncodedContent(parameters), "application/x-www-form-urlencoded");
            var response = new TypedHttpResponseMessage<TResponseBodyType, TErrorType>();
            await response.SetAsync(httpResponseMessage);
            return response;
        }
        */
    }
}
