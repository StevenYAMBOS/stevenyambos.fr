using System.Text;
using System.Text.Json.Serialization;
using Cloudflare.NET.Core;
using Cloudflare.NET.R2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Data;
using Portfolio.Entities;
using Portfolio.Repositories;
using Portfolio.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Variables d'environnements
var jwtSecret = builder.Configuration["AppSettings:Token"]!;
var key = Encoding.ASCII.GetBytes(jwtSecret);
var issuer = builder.Configuration["AppSettings:Issuer"]!;
var audience = builder.Configuration["AppSettings:Audience"]!;
var dbConfig = builder.Configuration["ConnectionStrings:DefaultConnection"]!;
var cloudflareApiToken = builder.Configuration["Cloudflare:ApiToken"]!;
var cloudflareAccountId = builder.Configuration["Cloudflare:AccountId"]!;
var cloudflareAccessKeyId = builder.Configuration["Cloudflare:AccessKeyId"]!;
var cloudflareSecretAccessKey = builder.Configuration["Cloudflare:SecretAccessKey"]!;
var cloudflareApiEndpoint = builder.Configuration["Cloudflare:JuridictionDefault"]!;
var cloudflareEndpointUrl = builder.Configuration["R2:EndpointUrl"]!;
var cloudflareEndpointPublicUrl = builder.Configuration["R2:PublicUrl"]!;

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.IncludeErrorDetails = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Injection de dépendances
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<TokenService, TokenService>();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(dbConfig));
builder.Services.AddCloudflareApiClient(options =>
{
    options.ApiToken = cloudflareApiEndpoint;
    options.AccountId = cloudflareAccountId;
    options.DefaultTimeout = TimeSpan.FromSeconds(30);
    options.RateLimiting.IsEnabled = true;
    options.RateLimiting.EnableProactiveThrottling = true;
    options.RateLimiting.QuotaLowThreshold = 0.1;
});
builder.Services.AddCloudflareR2Client(options =>
{
    options.AccessKeyId = cloudflareAccessKeyId;
    options.SecretAccessKey = cloudflareSecretAccessKey;
    options.EndpointUrl = cloudflareEndpointUrl;
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
        });
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleHelper.EnsureRolesCreated(roleManager);
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.MapControllers();
app.UseCors();

app.Run();
