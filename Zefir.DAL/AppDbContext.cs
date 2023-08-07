using Microsoft.EntityFrameworkCore;
using Zefir.Core.Entity;

namespace Zefir.DAL;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
        Database.MigrateAsync();
    }

    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Characteristics> Characteristics { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Basket> Baskets { get; set; } = null!;
}
