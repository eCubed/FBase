namespace FBase.ApiServer
{
    public interface IApiClientHasher
    {
        string GenerateHash(string apiKey, string secret, string arbitraryInput);
        bool CheckHash(string apiKey, string secret, string arbitraryInput, string clientHash);
    }
}
