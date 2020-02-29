using System.Data.Entity;

namespace MyStuff.DotNet.EntityFramework
{
    public class MyStuffDbContext : DbContext
    {
        public DbSet<MyThing> MyThings { get; set; }

        public MyStuffDbContext() : base()
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
