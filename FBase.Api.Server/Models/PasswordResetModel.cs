namespace FBase.Api.Server.Models;

public class PasswordResetModel
{
    public string EncryptedEmail { get; set; } = "";
    public string NewPassword { get; set; } = "";
    public string PasswordResetToken { get; set; } = "";
}
