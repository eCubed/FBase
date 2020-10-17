namespace FBase.ApiServer
{
    public interface IApiClientHasher
    {
        string GenerateHash(string clientId, string clientSecret, string arbitraryInput);
        bool CheckHash(string apiKey, string clientSecret, string xInputValue, string clientHash);
    }
}
