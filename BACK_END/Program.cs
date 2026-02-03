using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{

}).AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddRoles<IdentityRole>();

var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:secret"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key)
  };
});

// Config des politiques d'authorisation
builder.Services.AddAuthorizationBuilder()
.AddPolicy("AdminPolicy", options =>
{
  options.RequireAuthenticatedUser();
  options.RequireRole("admin");
})
.AddPolicy("UserPolicy", options =>
{
  options.RequireAuthenticatedUser();
  options.RequireRole("user");
});

// Configurer DbContext avec le connexion Psql
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Ajouter les services au conteneur
builder.Services.AddControllers();

var app = builder.Build();

// Petit middleware
if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers(); // C'est cool ça -> `Adds endpoints for controller actions to the IEndpointRouteBuilder without specifying any routes.`

// app.MapGet("/", () => "Hello World!");

app.Run();
