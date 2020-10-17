namespace FBase.Cryptography
{
    public interface ICrypter
    {
        string Encrypt(string message, string key);
        string Decrypt(string cipherText, string key);
    }
}
