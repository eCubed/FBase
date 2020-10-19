using FBase.ApiServer;
using System.Collections.Generic;

namespace FBase.ApiClient
{
    public static class Utils
    {
        public static List<KeyValuePair<string, string>> PrepareApiKeyClientCallHeaders(string apiKey, string secret,
            string arbitraryData, IApiClientHasher apiClientHasher, List<KeyValuePair<string, string>> additionalHeaders = null)
        {
            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();

            headers.Add(new KeyValuePair<string, string>("x-ApiKey", apiKey));
            headers.Add(new KeyValuePair<string, string>("x-Data", arbitraryData));
            headers.Add(new KeyValuePair<string, string>("x-Hash", apiClientHasher.GenerateHash(apiKey, secret, arbitraryData)));

            if (additionalHeaders != null && additionalHeaders.Count > 0)
                headers.AddRange(additionalHeaders);

            return headers;
        }
    }
}
