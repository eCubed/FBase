using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace FBase.Api.EntityFramework
{
    public class ApiServerDbContext<TUser, TRole, TUserKey> : IdentityDbContext<TUser, TRole, TUserKey>
        where TUser : IdentityUser<TUserKey>
        where TRole: IdentityRole<TUserKey>
        where TUserKey: IEquatable<TUserKey>
    {
        public DbSet<App<TUser, TUserKey>> Apps { get; set; } = null!;
        public DbSet<AppAuthorization<TUser, TUserKey>> AppAuthorizations { get; set; } = null!;
        public DbSet<AuthorizationCode<TUser, TUserKey>> AuthorizationCodes { get; set; } = null!;
        public DbSet<CredentialSet<TUser, TUserKey>> CredentialSets { get; set; } = null!;
        public DbSet<RefreshToken<TUser, TUserKey>> RefreshTokens { get; set; } = null!;
        public DbSet<Scope> Scopes { get; set; } = null!;
        public DbSet<ScopeApp<TUser, TUserKey>> ScopeApps { get; set; } = null!;

        public DbSet<ScopeAppAuthorization<TUser, TUserKey>> ScopeAppAuthorizations { get; set; } = null!;

        public ApiServerDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<App<TUser, TUserKey>>().Property("CreatedDate").HasDefaultValueSql("GETDATE()");
            builder.Entity<CredentialSet<TUser, TUserKey>>().Property("CreatedDate").HasDefaultValueSql("GETDATE()");
            builder.Entity<AppAuthorization<TUser, TUserKey>>().HasOne(aa => aa.User).WithMany().OnDelete(DeleteBehavior.NoAction);
            builder.Entity<AuthorizationCode<TUser, TUserKey>>().Property("CreatedDate").HasDefaultValueSql("GETDATE()");
            builder.Entity<AuthorizationCode<TUser, TUserKey>>().HasOne(ac => ac.User).WithMany().OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(builder);
        }
    }
}
