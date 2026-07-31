namespace RestaurantApi.Model;

public class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    // EF 導覽屬性
    public List<MenuItem> MenuItems { get; set; } = new();
}
