using FBase.ApiServer.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace ApiServerLibraryTest.Data
{
    public class ApiServerLibraryTestDbContext : ApiServerDbContext<TestUser, TestRole, int>
    {

        public ApiServerLibraryTestDbContext(DbContextOptions<ApiServerLibraryTestDbContext> options) : base(options)
        {
        }
    }
}
