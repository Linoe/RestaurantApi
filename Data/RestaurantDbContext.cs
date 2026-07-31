using Microsoft.EntityFrameworkCore;
using RestaurantApi.Model;

namespace RestaurantApi.Data;

public class RestaurantDbContext : DbContext
{
    public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
}
