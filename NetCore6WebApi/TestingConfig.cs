using FBase.Api.Server;

namespace NetCore6WebApi
{
    public class TestingConfig : IApiServerConfig
    {
        public string ConnectionString { get; set; } = "";
        public string CryptionKey { get; set; } = "";
        public string Issuer { get; set; } = "";
        public string Audience { get; set; } = "";
    }
}
