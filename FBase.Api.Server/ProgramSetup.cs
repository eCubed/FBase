using FBase.Api.EntityFramework;
using FBase.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

namespace FBase.Api.Server;

public static class ProgramSetup
{
    public static void Configure<TApiServerConfig, TDbContext, TUser, TRole, TUserKey, TSeeder>(
        string[] args, 
        ProgramSetupOptions<TApiServerConfig, TUser, TUserKey> options)
        where TApiServerConfig : class, IApiServerConfig, new()
        where TUserKey : IEquatable<TUserKey>
        where TUser: IdentityUser<TUserKey>, new()
        where TRole: IdentityRole<TUserKey>, new()
        where TDbContext : ApiServerDbContext<TUser, TRole, TUserKey>
        where TSeeder: SeederBase<TUser, TRole, TUserKey>, new()
    {
        var builder =  WebApplication.CreateBuilder(args);
        var config = new TApiServerConfig();
        builder.Configuration.Bind(options.ConfigurationKey, config);
        builder.Services.AddSingleton(config);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config.Issuer,
            ValidAudience = config.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.CryptionKey!)),
            ClockSkew = TimeSpan.Zero
        };
        builder.Services.AddSingleton(tokenValidationParameters);
        builder.Services.AddSingleton(options);

        builder.Services.AddMvc();
        builder.Services.AddMvcCore().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        builder.Services.AddDbContext<TDbContext>(options =>
        {
            options.UseSqlServer(config.ConnectionString!);
        });

        builder.Services.AddIdentity<TUser, TRole>()
            .AddEntityFrameworkStores<TDbContext>()
            .AddDefaultTokenProviders();

        options.SetupAdditionalEntities?.Invoke(config, builder);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = tokenValidationParameters;
        });

        builder.Services.AddCors();
        builder.Services.AddRazorPages();
        builder.Services.AddControllers();

        builder.Services.AddSingleton<ICrypter, Crypt>();

        options.RegisterAdditionalServices?.Invoke(config, builder);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<TDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<TRole>>();

                TSeeder seeder = new TSeeder();
                seeder.SetupPersistence(db, userManager, roleManager);
                seeder.InitializeDataAsync(options.Roles, options.UserWithRoles).Wait();
                app.UseDeveloperExceptionPage();
            }
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        });

        app.UseCors(options =>
        {
            options.AllowAnyOrigin();
            options.AllowAnyMethod();
            options.AllowAnyHeader();
        });

        var staticFileOptions = options.ProvideStaticFileOptions?.Invoke(config);

        if (staticFileOptions == null)
        {
            app.UseStaticFiles();
        }
        else
        {
            app.UseStaticFiles(staticFileOptions);
        }

        app.UseDefaultFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();
        app.MapControllers();

        app.Run();
    }
}
