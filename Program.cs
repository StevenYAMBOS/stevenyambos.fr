using System.Text;
using System.Text.Json.Serialization;
using Cloudflare.NET.Core;
using Cloudflare.NET.R2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Portfolio.Data;
using Portfolio.Entities;
using Portfolio.Repositories;
using Portfolio.Services;
using Serilog;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
}).AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "stevenyambos.fr API",
        Description = "API du site internet `stevenyambos.fr`.",
        Contact = new OpenApiContact
        {
            Name = "Développeur (Steven YAMBOS)",
            Url = new Uri("https://www.linkedin.com/in/steven-yambos/")
        },
        Version = "v1"
    });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Entrer un JWT valide",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

// Variables d'environnements
// var jwtSecret = builder.Configuration["AppSettings:Token"]!;
// var issuer = builder.Configuration["AppSettings:Issuer"]!;
// var audience = builder.Configuration["AppSettings:Audience"]!;
// var dbConfig = builder.Configuration["ConnectionStrings:DefaultConnection"]!;
// var cloudflareApiToken = builder.Configuration["Cloudflare:ApiToken"]!;
// var cloudflareAccountId = builder.Configuration["Cloudflare:AccountId"]!;
// var cloudflareAccessKeyId = builder.Configuration["Cloudflare:AccessKeyId"]!;
// var cloudflareSecretAccessKey = builder.Configuration["Cloudflare:SecretAccessKey"]!;
// var cloudflareApiEndpoint = builder.Configuration["Cloudflare:JuridictionDefault"]!;
// var cloudflareEndpointUrl = builder.Configuration["R2:EndpointUrl"]!;
// var cloudflareEndpointPublicUrl = builder.Configuration["R2:PublicUrl"]!;
var jwtSecret = Environment.GetEnvironmentVariable("AppSettingsToken");
var key = Encoding.ASCII.GetBytes(jwtSecret!);
var issuer = Environment.GetEnvironmentVariable("AppSettingsIssuer");
var audience = Environment.GetEnvironmentVariable("AppSettingsAudience");
var dbConfig = Environment.GetEnvironmentVariable("ConnectionStringsDefaultConnection");
var cloudflareApiToken = Environment.GetEnvironmentVariable("CloudflareApiToken");
var cloudflareAccountId = Environment.GetEnvironmentVariable("CloudflareAccountId");
var cloudflareAccessKeyId = Environment.GetEnvironmentVariable("CloudflareAccessKeyId");
var cloudflareSecretAccessKey = Environment.GetEnvironmentVariable("CloudflareSecretAccessKey");
var cloudflareApiEndpoint = Environment.GetEnvironmentVariable("CloudflareJuridictionDefault");
var cloudflareEndpointUrl = Environment.GetEnvironmentVariable("R2EndpointUrl");
var cloudflareEndpointPublicUrl = Environment.GetEnvironmentVariable("R2PublicUrl");

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
builder.Services.AddScoped<IUserProfileService, ProfilService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IEmailService, EmailService>();

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

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromSeconds(10);
        limiterOptions.QueueLimit = 0;
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleHelper.EnsureRolesCreated(roleManager);
}

app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseAuthentication();
app.MapControllers();
app.UseSerilogRequestLogging();
app.UseCors();

app.Run();
