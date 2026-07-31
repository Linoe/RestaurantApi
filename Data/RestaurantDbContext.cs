using Microsoft.EntityFrameworkCore;
using RestaurantApi.Model;

namespace RestaurantApi.Data;

public class RestaurantDbContext : DbContext
{
    public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<MenuItem> MenuItems => Set<MenuItem>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 資料庫模型規則設定

        modelBuilder.Entity<MenuItem>()
            .Property(item => item.Price)
            .HasPrecision(10, 2);

        modelBuilder.Entity<MenuItem>()
            .HasOne(item => item.Category)
            .WithMany(category => category.MenuItems)
            .HasForeignKey(item => item.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}
