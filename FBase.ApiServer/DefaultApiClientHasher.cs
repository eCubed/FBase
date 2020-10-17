using FBase.Cryptography;

namespace FBase.ApiServer
{
    public class DefaultApiClientHasher : IApiClientHasher
    {
        public string GenerateHash(string clientId, string clientSecret, string arbitraryInput)
        {
            return Hasher.GetHash($"{clientId}.{clientSecret}.{arbitraryInput}", Hasher.HashType.MD5);
        }

        public bool CheckHash(string clientId, string clientSecret, string arbitraryInput, string clientHash)
        {
            return GenerateHash(clientId, clientSecret, arbitraryInput) == clientHash;
        }
    }
}
