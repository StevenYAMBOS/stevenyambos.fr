
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Portfolio.Entities;
using Portfolio.Models;

namespace Portfolio.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
  // public DbSet<User> Users { get; set; }
  public DbSet<Article> Articles { get; set; }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    builder.Entity<ApplicationUser>(entity =>
    {
      entity.ToTable(name: "users");
    });
    builder.Entity<IdentityRole>(entity =>
    {
      entity.ToTable(name: "roles");
    });
  }
}

/* public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityUserContext<ApplicationUser>(options)

{
  // public DbSet<User> Users { get; set; }
  public DbSet<Article> Articles { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
  }
} */
