using System.Text;
using Amazon.Runtime;
using Amazon.S3;
using Cloudflare.NET.Core;
using Cloudflare.NET.R2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Data;
using Portfolio.Repositories;
using Portfolio.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var jwtSecret = builder.Configuration["AppSettings:Token"];
var key = Encoding.ASCII.GetBytes(jwtSecret);
var issuer = builder.Configuration["AppSettings:Issuer"];
var audience = builder.Configuration["AppSettings:Audience"];
var dbConfig = builder.Configuration["ConnectionStrings:DefaultConnection"];
var cloudflareApiToken = builder.Configuration["Cloudflare:ApiToken"];
var cloudflareAccountId = builder.Configuration["Cloudflare:AccountId"];
var cloudflareAccessKeyId = builder.Configuration["Cloudflare:AccessKeyId"];
var cloudflareSecretAccessKey = builder.Configuration["Cloudflare:SecretAccessKey"];
var cloudflareApiEndpoint = builder.Configuration["Cloudflare:JuridictionDefault"];

/* var accessKey = cloudflareAccessKeyId;
var secretKey = cloudflareSecretAccessKey;
var credentials = new BasicAWSCredentials(accessKey, secretKey);
IAmazonS3 s3Client = new AmazonS3Client(credentials, new AmazonS3Config
{
  // Provide your Cloudflare account ID
  ServiceURL = cloudflareApiEndpoint,
}); */

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
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

builder.Services.AddScoped<IAuthService, AuthService>();

// Configurer DbContext avec le connexion Psql
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(dbConfig));

builder.Services.AddCloudflareR2Client(options =>
{
    options.AccessKeyId = cloudflareAccessKeyId;
    options.SecretAccessKey = cloudflareSecretAccessKey;
    options.EndpointUrl = cloudflareApiEndpoint;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.MapControllers();

app.Run();
