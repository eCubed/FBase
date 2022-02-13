namespace FBase.Api;

public interface ICredentialValuesProvider
{
    string GenerateClientId();
    string GenerateClientSecret();
}
