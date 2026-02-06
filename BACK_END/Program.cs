using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Data;
using Portfolio.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var jwtSecret = builder.Configuration["AppSettings:Token"];
var key = Encoding.ASCII.GetBytes(jwtSecret);
var issuer = builder.Configuration["AppSettings:Issuer"];
var audience = builder.Configuration["AppSettings:Audience"];

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
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Petit middleware
if (app.Environment.IsDevelopment())
{
  app.Logger.LogInformation("Lancement de l'app !");
  app.UseDeveloperExceptionPage();
  /*   app.MapOpenApi();
    app.MapScalarApiReference(); */
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.MapControllers(); // C'est cool ça -> `Adds endpoints for controller actions to the IEndpointRouteBuilder without specifying any routes.`

app.Run();