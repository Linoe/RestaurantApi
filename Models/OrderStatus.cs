namespace RestaurantApi.Model;

/// <summary>
/// 正常下單狀態順序<br/>
/// Pending > Preparing > Completed<br/>
/// 取消下單 Cancelled
/// </summary>
public enum OrderStatus
{
    Pending = 0,
    Preparing = 1,
    Completed = 2,
    Cancelled = 3,
}
