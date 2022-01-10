namespace FBase.Api.Server.Models;
public class TokenResponse
{
    public string Username { get; set; } = "";
    public string Token { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public List<string> Roles { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}