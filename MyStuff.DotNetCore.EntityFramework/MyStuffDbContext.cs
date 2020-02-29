using Microsoft.EntityFrameworkCore;

namespace MyStuff.DotNetCore.EntityFramework
{
    public class MyStuffDbContext : DbContext
    {
        public DbSet<MyThing> MyThings { get; set; }

        public MyStuffDbContext(DbContextOptions<MyStuffDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
