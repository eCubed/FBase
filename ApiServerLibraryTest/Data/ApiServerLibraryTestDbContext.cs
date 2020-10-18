using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiServerLibraryTest.Data
{
    public class ApiServerLibraryTestDbContext : IdentityDbContext<TestUser, TestRole, int>
    {
        public ApiServerLibraryTestDbContext(DbContextOptions<ApiServerLibraryTestDbContext> options) : base(options)
        {
        }
    }
}
