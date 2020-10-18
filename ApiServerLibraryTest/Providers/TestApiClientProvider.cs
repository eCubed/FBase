using ApiServerLibraryTest.Models;
using FBase.ApiServer;

namespace ApiServerLibraryTest.Providers
{
    public class TestApiClientProvider : IApiClientProvider<ApiClient, int>
    {
        public ApiClient GetClientByApiKey(string publicKey)
        {
            if (publicKey == "TEST-CLIENT-1000")
                return new ApiClient { ApiKey = "TEST-CLIENT-1000", Secret = "TestSecret-12345", Id = 2, Name = "Test Client" };
            else
                return null;
        }
    }
}
