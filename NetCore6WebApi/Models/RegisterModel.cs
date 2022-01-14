using FBase.Api.Server.Models;

namespace NetCore6WebApi.Models;

public class RegisterModel : IRegisterModel
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}
