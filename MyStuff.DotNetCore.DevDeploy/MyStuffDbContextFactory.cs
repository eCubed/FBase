using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MyStuff.DotNetCore.EntityFramework;
using System.IO;

namespace MyStuff.DotNetCore.DevDeploy
{
    public class MyStuffDbContextFactory : IDesignTimeDbContextFactory<MyStuffDbContext>
    {

        public MyStuffDbContext CreateDbContext(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddUserSecrets<Startup>()
                 .Build();

            MyStuffConfig config = new MyStuffConfig();
            configuration.Bind("MyStuffConfig", config);

            var builder = new DbContextOptionsBuilder<MyStuffDbContext>();

            builder.UseSqlServer(config.ConnectionString, b => b.MigrationsAssembly("MyStuff.DotNetCore.DevDeploy"));

            return new MyStuffDbContext(builder.Options);
        }
    }
}
