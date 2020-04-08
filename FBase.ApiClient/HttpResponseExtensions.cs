using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace FBase.ApiClient
{
    public static class HttpResponseExtensions
    {
        public static async Task<T> ContentAsTypeAsync<T>(this HttpResponseMessage response)
        {
            var data = await response.Content.ReadAsStringAsync();

            return string.IsNullOrEmpty(data) ? default(T) :
                JsonSerializer.Deserialize<T>(data, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    IgnoreNullValues = true
                });
        }

        // What about content as bytes!?
    }
}
