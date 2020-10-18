using FBase.ApiServer;

namespace ApiServerLibraryTest.Models
{
    public class ApiClient : IApiClient<int>
    {
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public string Secret { get; set; }
        public int Id { get; set; }
    }
}
