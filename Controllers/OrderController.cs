using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantApi.Data;
using RestaurantApi.Model;

namespace RestaurantApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly RestaurantDbContext dbContext;

    public OrderController(RestaurantDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    // GET /api/order?status=Pending
    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetOrders([FromQuery] OrderStatus? status)
    {
        IQueryable<Order> query = dbContext.Orders
            .Include(order => order.Items)
            .ThenInclude(item => item.MenuItem);

        // 如果網址帶有 status 額外查找
        if (status.HasValue)
        {
            query = query.Where(order => order.Status == status.Value);
        }

        // 按照時間排序
        List<Order> orders = await query
            .OrderByDescending(order => order.CreatedAt)
            .ToListAsync();

        // 中介資料轉換
        List<OrderDto> result = orders
            .Select(ToDto)
            .ToList();

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        Order? order = await dbContext.Orders
            .Include(o => o.Items)
            .ThenInclude(item => item.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        return Ok(ToDto(order));
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto request)
    {
        // 檢查最少存在一項餐點
        if (request.Items == null || request.Items.Count == 0)
        {
            return BadRequest($"訂單至少需要一項品項");
        }

        Order order = new()
        {
            CustomerName = request.CustomerName,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.Now,
        };

        // 建立訂單中餐點資訊
        foreach (CreateOrderItemDto requestItem in request.Items)
        {
            // 檢查訂單的餐點資訊正確
            if (requestItem.Quantity <= 0)
            {
                return BadRequest("數量必須大於 0");
            }

            MenuItem? menuItem = await dbContext.MenuItems.FindAsync(requestItem.MenuItemId);

            if (menuItem == null)
            {
                return BadRequest($"MenuItemId {requestItem.MenuItemId} 不存在");
            }

            if (!menuItem.IsAvailable)
            {
                return BadRequest($"品項「{menuItem.Name}」目前無法販售");
            }

            order.Items.Add(new OrderItem
            {
                MenuItemId = menuItem.Id,
                Quantity = requestItem.Quantity,
                UnitPrice = menuItem.Price,
            });
        }

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();

        Order? created = await dbContext.Orders
            .Include(o => o.Items)
            .ThenInclude(item => item.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == order.Id);

        return Ok(ToDto(order));
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, UpdateOrderStatusDto request)
    {
        Order? order = await dbContext.Orders.FindAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        order.Status = request.Status;
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        Order? order = await dbContext.Orders.FindAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        dbContext.Orders.Remove(order);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    private static OrderDto ToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            Items = order.Items.Select(item => new OrderItemDto
            {
                MenuItemId = item.MenuItemId,
                MenuItemName = item.MenuItem?.Name ?? string.Empty,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
            }).ToList(),
        };
    }
}
