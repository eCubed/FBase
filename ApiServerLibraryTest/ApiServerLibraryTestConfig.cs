using FBase.ApiServer;

namespace ApiServerLibraryTest
{
    public class ApiServerLibraryTestConfig : IApiServerConfig
    {
        public string? ConnectionString { get; set; }
        public string? CryptionKey { get; set; }
        public string? Audience { get; set; }
        public string? Issuer { get; set; }
    }
}
