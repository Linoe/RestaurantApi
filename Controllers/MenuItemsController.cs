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
    public async Task<ActionResult<List<MenuItem>>> GetMenuItems()
    {
        List<MenuItem> items = await dbContext.MenuItems.ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<MenuItem>> CreateMenuItem(MenuItem item)
    {
        dbContext.MenuItems.Add(item);
        await dbContext.SaveChangesAsync();
        return Ok(item);
    }
}
