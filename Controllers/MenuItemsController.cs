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

    // GET /api/menuitems?categoryId=1
    [HttpGet]
    public async Task<ActionResult<List<MenuItemDto>>> GetMenuItems([FromQuery] int? categoryId)
    {
        // 如果網址帶有 categoryId 額外查找
        IQueryable<MenuItem> query = dbContext.MenuItems.AsNoTracking();
        if (categoryId.HasValue)
        {
            query = query.Where(item => item.CategoryId == categoryId.Value);
        }

        List<MenuItemDto> items = await query
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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MenuItemDto>> GetMenuItem(int id)
    {
        MenuItem? item = await dbContext.MenuItems
            .Include(menuItem => menuItem.Category)
            .FirstOrDefaultAsync(menuItem => menuItem.Id == id);

        if (item == null)
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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateMenuItem(int id, UpdateMenuItemDto request)
    {
        MenuItem? item = await dbContext.MenuItems.FindAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        // 檢查外鍵 CategoryId 存在
        if (!await dbContext.Categories.AnyAsync(category => category.Id == request.CategoryId))
        {
            return BadRequest($"CategoryId {request.CategoryId} 不存在");
        }

        item.Name = request.Name;
        item.Description = request.Description;
        item.Price = request.Price;
        item.IsAvailable = request.IsAvailable;
        item.CategoryId = request.CategoryId;
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMenuItem(int id)
    {
        MenuItem? item = await dbContext.MenuItems.FindAsync(id);

        if (item == null)
        {
            return NotFound();
        }

        dbContext.MenuItems.Remove(item);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}
