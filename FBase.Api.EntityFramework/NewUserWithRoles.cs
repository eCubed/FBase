namespace FBase.Api.EntityFramework
{
    public class NewUserWithRoles
    {
        public string Username { get; set; } = "";
        public List<string> Roles { get; set; } = new List<string>();
    }
}
