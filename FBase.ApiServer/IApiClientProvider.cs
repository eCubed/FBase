namespace FBase.ApiServer
{
    public interface IApiClientProvider<TApiClient, TKey>
        where TApiClient : class, IApiClient<TKey>
    {
        TApiClient GetClientByApiKey(string publicKey);
    }
}
