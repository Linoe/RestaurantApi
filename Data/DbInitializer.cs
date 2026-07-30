using RestaurantApi.Model;

namespace RestaurantApi.Data;

public static class DbInitializer
{
    public static void Seed(RestaurantDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();

        if (dbContext.Categories.Any())
        {
            return;
        }

        Category mainDish = new() { Name = "主餐" };
        Category drink = new() { Name = "飲料" };
        Category side = new() { Name = "小菜" };

        dbContext.Categories.AddRange(mainDish, drink, side);
        dbContext.SaveChanges();

        dbContext.MenuItems.AddRange(
            new MenuItem { Name = "招牌牛肉麵", Price = 180, CategoryId = mainDish.Id },
            new MenuItem { Name = "滷肉飯", Price = 60, CategoryId = mainDish.Id },
            new MenuItem { Name = "紅茶", Price = 25, CategoryId = drink.Id },
            new MenuItem { Name = "珍珠奶茶", Price = 55, CategoryId = drink.Id },
            new MenuItem { Name = "滷豆干", Price = 30, CategoryId = side.Id }
        );

        dbContext.SaveChanges();
    }
}
