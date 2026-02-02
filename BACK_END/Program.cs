using Microsoft.EntityFrameworkCore;
using Portfolio.Data;

var builder = WebApplication.CreateBuilder(args);

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

app.MapGet("/", () => "Hello World!");

app.Run();
