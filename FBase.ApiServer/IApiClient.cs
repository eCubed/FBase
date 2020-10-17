using FBase.Foundations;

namespace FBase.ApiServer
{
    public interface IApiClient<out TKey> : IIdentifiable<TKey>
    {
        string Name { get; set; }
        string PublicKey { get; set; }
        string PrivateKey { get; set; }
        string GetIdAsString();
    }
}
