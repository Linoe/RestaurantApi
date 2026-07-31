namespace RestaurantApi.Model;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public Order? Order { get; set; }

    public int MenuItemId { get; set; }

    public MenuItem? MenuItem { get; set; }

    public int Quantity { get; set; }

    // 下單當下的單價快照，避免菜單改價後影響歷史訂單金額
    public decimal UnitPrice { get; set; }
}
