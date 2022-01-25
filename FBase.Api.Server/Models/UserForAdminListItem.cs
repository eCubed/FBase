namespace FBase.Api.Server.Models;

public class UserForAdminListItem<TUserKey>
    where TUserKey : IEquatable<TUserKey>
{
    public TUserKey Id { get; set; } = default!;
    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
    public bool EmailConfirmed { get; set; } = false;
    public bool LockedOut { get; set; } = false;
}
