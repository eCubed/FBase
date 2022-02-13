using FBase.Api.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace NetCore6WebApi.Data;

public class TestingDbContext : ApiServerDbContext<TestingUser, TestingRole, string>
{
    public DbSet<Thing> Things { get; set; } = null!;
    public TestingDbContext(DbContextOptions<TestingDbContext> options) : base(options)
    {
    }
}
