using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Portfolio.Entities;

namespace Portfolio.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
  public DbSet<Article> Articles { get; set; }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.Entity<ApplicationUser>(entity =>
    {
      entity.ToTable("users");
      // Pour stocker le rôle en string lisible ("Admin", "User")
      entity.Property(u => u.Role)
                .HasConversion<string>();
    });

    builder.Entity<IdentityRole>(entity =>
    {
      entity.ToTable("roles");
    });
  }
}