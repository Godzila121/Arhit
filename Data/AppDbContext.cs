using Microsoft.EntityFrameworkCore;


public class AppDbContext : DbContext
{
public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


public DbSet<User> Users { get; set; } = null!;
public DbSet<Restaurant> Restaurants { get; set; } = null!;
public DbSet<MenuItem> MenuItems { get; set; } = null!;
public DbSet<Courier> Couriers { get; set; } = null!;
public DbSet<Order> Orders { get; set; } = null!;
public DbSet<OrderItem> OrderItems { get; set; } = null!;
public DbSet<Review> Reviews { get; set; } = null!;


protected override void OnModelCreating(ModelBuilder modelBuilder)
{
base.OnModelCreating(modelBuilder);


modelBuilder.Entity<MenuItem>()
.HasOne(mi => mi.Restaurant)
.WithMany(r => r.MenuItems)
.HasForeignKey(mi => mi.RestaurantId)
.OnDelete(DeleteBehavior.Cascade);


modelBuilder.Entity<OrderItem>()
.HasOne(oi => oi.MenuItem)
.WithMany()
.HasForeignKey(oi => oi.MenuItemId);
}
}