using ApiServerLibraryTest;
using ApiServerLibraryTest.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var config = new ApiServerLibraryTestConfig();
builder.Configuration.Bind("ApiServerLibraryTestConfig", config);
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

builder.Services.AddMvc();
builder.Services.AddMvcCore().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddDbContext<ApiServerLibraryTestDbContext>(options =>
{
    options.UseSqlServer(config.ConnectionString!);
});

builder.Services.AddIdentity<TestUser, TestRole>()
    .AddEntityFrameworkStores<ApiServerLibraryTestDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = tokenValidationParameters;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);


builder.Services.AddCors();
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// ---------------------
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    using(var scope = app.Services.CreateScope())
    {
        var db =  scope.ServiceProvider.GetRequiredService<ApiServerLibraryTestDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TestUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<TestRole>>();

        Seeder seeder = new Seeder(db!, userManager!, roleManager!);
        seeder.InitializeDataAsync().Wait();
        app.UseDeveloperExceptionPage();
    }
   
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

app.UseStaticFiles();
app.UseDefaultFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();

