using FBase.Api.EntityFramework;

namespace FBase.Api.Server
{
    public class ProgramSetupOptions
    {
        public string ConfigurationKey { get; set; } = "ConfigurationKey";
        public List<string> Roles { get; set; } = new List<string>();
        public List<NewUserWithRoles> UserWithRoles { get; set; } = new List<NewUserWithRoles>();
    }
}
