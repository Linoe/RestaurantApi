using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantApi.Data;
using RestaurantApi.Model;

namespace RestaurantApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class MenuItemsController : ControllerBase
{
    private readonly RestaurantDbContext dbContext;

    public MenuItemsController(RestaurantDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<MenuItemDto>>> GetMenuItems()
    {
        List<MenuItemDto> items = await dbContext.MenuItems
            .Select(menuItem => new MenuItemDto()
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                IsAvailable = menuItem.IsAvailable,
                CategoryId = menuItem.CategoryId,
                CategoryName = menuItem.Category == null ? null : menuItem.Category.Name,
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<MenuItemDto>> CreateMenuItem(CreateMenuItemDto request)
    {
        // 檢查外鍵 CategoryId 存在
        if (!await dbContext.Categories.AnyAsync(category => category.Id == request.CategoryId))
        {
            return BadRequest($"CategoryId {request.CategoryId} 不存在");
        }

        MenuItem item = new()
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CategoryId = request.CategoryId,
        };

        dbContext.MenuItems.Add(item);
        await dbContext.SaveChangesAsync();

        // 保存時 EF 不會自動載入 Category 需要重新取得，以便能得到 CategoryName 資訊
        await dbContext.Entry(item)
            .Reference(menuItem => menuItem.Category)
            .LoadAsync();

        if (item is null)
        {
            return NotFound();
        }

        MenuItemDto result = new()
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            IsAvailable = item.IsAvailable,
            CategoryId = item.CategoryId,
            CategoryName = item.Category?.Name,
        };

        return Ok(result);
    }
}
