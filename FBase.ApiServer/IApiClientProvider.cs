namespace FBase.ApiServer
{
    public interface IApiClientProvider<TApiClient, TKey>
        where TApiClient : class, IApiClient<TKey>
    {
        bool ClientExists(string clientId);
        string GetClientSecret(string clientId);
        TApiClient GetClientByPublicKey(string publicKey);
    }
}
