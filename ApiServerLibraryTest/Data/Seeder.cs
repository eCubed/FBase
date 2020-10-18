using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiServerLibraryTest.Data
{
    public class Seeder
    {
        private ApiServerLibraryTestDbContext db { get; set; }
        private UserManager<TestUser> UM { get; set; }
        private RoleManager<TestRole> RM { get; set; }

        public Seeder(ApiServerLibraryTestDbContext context, UserManager<TestUser> userManager,
           RoleManager<TestRole> roleManager)
        {
            db = context;
            UM = userManager;
            RM = roleManager;
        }

        public async Task CreateRolesAsync()
        {
            if (!(await RM.RoleExistsAsync(RoleNames.Administrator)))
                await RM.CreateAsync(new TestRole { Name = RoleNames.Administrator });

            if (!(await RM.RoleExistsAsync(RoleNames.Subscriber)))
                await RM.CreateAsync(new TestRole { Name = RoleNames.Subscriber });
        }

        private async Task CreateUserAsync(string username, List<string> roleNames)
        {
            TestUser user = await UM.FindByNameAsync(username);

            if (user == null)
            {
                string usernameToSave = $"{username}@test123123.com";
                user = new TestUser
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

        public async Task CreateUsersAsync()
        {
            await CreateUserAsync("admin", new List<string> { RoleNames.Administrator, RoleNames.Subscriber });
            await CreateUserAsync("user1", new List<string> { RoleNames.Subscriber });
            await CreateUserAsync("user2", new List<string> { RoleNames.Subscriber });
            await CreateUserAsync("user3", new List<string> { RoleNames.Subscriber });
        }

        public async Task InitializeDataAsync()
        {
            await db.Database.EnsureCreatedAsync();
            await CreateRolesAsync();
            await CreateUsersAsync();
        }
    }
}
