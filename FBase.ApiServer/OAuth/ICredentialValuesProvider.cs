namespace FBase.ApiServer.OAuth
{
    public interface ICredentialValuesProvider
    {
        string GenerateClientId();
        string GenerateClientSecret();
    }
}
