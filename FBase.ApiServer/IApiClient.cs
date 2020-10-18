using FBase.Foundations;

namespace FBase.ApiServer
{
    public interface IApiClient<out TKey> : IIdentifiable<TKey>
    {
        string Name { get; set; }
        string ApiKey { get; set; }
        string Secret { get; set; }
    }
}
