namespace FBase.ApiServer
{
    public interface IAppConfig
    {
        string ConnectionString { get; set; }
        string CryptionKey { get; set; }
        string Issuer { get; set; }
        string Audience { get; set; }
    }
}
