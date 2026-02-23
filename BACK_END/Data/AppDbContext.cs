
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Portfolio.Entities;
using Portfolio.Models;

namespace Portfolio.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityUserContext<UserRoles>(options)

{
  public DbSet<User> Users { get; set; }
  public DbSet<Article> Articles { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
  }
}
