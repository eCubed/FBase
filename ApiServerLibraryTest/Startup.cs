using ApiServerLibraryTest.Data;
using ApiServerLibraryTest.Models;
using ApiServerLibraryTest.Providers;
using FBase.ApiServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

namespace ApiServerLibraryTest
{
    public class Startup
    {
        private IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var config = new ApiServerLibraryTestConfig();
            Configuration.Bind("ApiServerLibraryTestConfig", config);
            services.AddSingleton(config);

            services.AddMvc();
            services.AddMvcCore().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });

            services.AddDbContext<ApiServerLibraryTestDbContext>(options =>
            {
                options.UseSqlServer(config.ConnectionString);
            });

            services.AddIdentity<TestUser, TestRole>()
                .AddEntityFrameworkStores<ApiServerLibraryTestDbContext>()
                .AddDefaultTokenProviders();



            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "Test Issuer",
                    ValidAudience = "Test Audience",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Aaaa-0000-XYZWT")),
                };
            });


            services.AddTransient<IApiClientHasher, DefaultApiClientHasher>();
            services.AddTransient<IApiClientProvider<ApiClient, int>, TestApiClientProvider>();

            services.AddCors();
            services.AddRazorPages();
            services.AddControllers();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequiresClientIdClaim",
                    policy => policy.RequireClaim("ClientId"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApiServerLibraryTestDbContext db,
            UserManager<TestUser> userManager, RoleManager<TestRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                Seeder seeder = new Seeder(db, userManager, roleManager);
                seeder.InitializeDataAsync().Wait();
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();
            app.UseStaticFiles();
            app.UseDefaultFiles();
            app.UseApiKeyMiddleware<ApiClient, int>();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
