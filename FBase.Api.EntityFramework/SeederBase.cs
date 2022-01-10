using Microsoft.AspNetCore.Identity;

namespace FBase.Api.EntityFramework
{
    public abstract class SeederBase<TUser, TRole, TUserKey>
        where TUser : IdentityUser<TUserKey>, new()
        where TRole : IdentityRole<TUserKey>, new()
        where TUserKey : IEquatable<TUserKey>
    {
        private ApiServerDbContext<TUser, TRole, TUserKey> db { get; set; } = null!;
        private UserManager<TUser> UM { get; set; } = null!;
        private RoleManager<TRole> RM { get; set; } = null!;

        public void SetupPersistence(ApiServerDbContext<TUser, TRole, TUserKey> context, UserManager<TUser> userManager, RoleManager<TRole> roleManager)
        {
            db = context;
            UM = userManager;
            RM = roleManager;
        }

        protected void CreateRoles(List<string> roles)
        {
            roles.ForEach(role =>
            {
                if (!RM.RoleExistsAsync(role).Result)
                {
                    TRole newRoleObject = new TRole();
                    newRoleObject.Name = role;
                    RM.CreateAsync(newRoleObject).Wait();
                }
            });
        }

        private async Task CreateUserAsync(string username, List<string> roleNames)
        {
            TUser user = await UM.FindByNameAsync(username);

            if (user == null)
            {
                string usernameToSave = username;
                user = new TUser()
                {
                    UserName = usernameToSave,
                    Email = usernameToSave,
                    EmailConfirmed = true
                };

                var res = await UM.CreateAsync(user, "Aaa000$");

                if (res.Succeeded)
                {
                    roleNames.ForEach(roleName =>
                    {
                        var addRoleRes = UM.AddToRoleAsync(user, roleName).Result;
                    });

                }
            }
        }

        private void CreateUsers(List<NewUserWithRoles> userWithRoles)
        {
            userWithRoles.ForEach(userWithRole =>
            {
                CreateUserAsync(userWithRole.Username, userWithRole.Roles).Wait();
            });
        }

        public virtual async Task InitializeDataAsync(List<string> roleNames, List<NewUserWithRoles> usersWithRoles)
        {
            await db.Database.EnsureCreatedAsync();
            CreateRoles(roleNames);
            CreateUsers(usersWithRoles);
        }
    }
}
