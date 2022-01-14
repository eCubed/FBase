namespace FBase.Api.Server.Models;

public class ConfirmEmailModel
{
    public string EncryptedEmail { get; set; } = "";
    public string EmailConfirmationToken { get; set; } = "";
}
