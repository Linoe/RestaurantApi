using Microsoft.EntityFrameworkCore;
using RestaurantApi.Model;

namespace RestaurantApi.Data;

public class RestaurantDbContext : DbContext
{
    public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<MenuItem> MenuItems => Set<MenuItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 資料庫模型規則設定

        base.OnModelCreating(modelBuilder);
    }
}
