using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiServerLibraryTest.Data
{
    public class ApiServerLibraryTestDbContext : IdentityDbContext<TestUser, TestRole, int>
    {
        public DbSet<App> Apps { get; set; }
        public DbSet<AppAuthorization> AppAuthorizations { get; set; }
        public DbSet<CredentialSet> CredentialSets { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Scope> Scopes { get; set; }
        public DbSet<ScopeApp> ScopeApps { get; set; }

        public DbSet<ScopeAppAuthorization> ScopeAppAuthorizations { get; set; }

        public ApiServerLibraryTestDbContext(DbContextOptions<ApiServerLibraryTestDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<App>().Property("CreatedDate").HasDefaultValueSql("DATE()");
            builder.Entity<App>().Property("CreatedDate").HasDefaultValueSql("DATE()");

            base.OnModelCreating(builder);
        }
    }
}
