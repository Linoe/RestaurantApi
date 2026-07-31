using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantApi.Data;
using RestaurantApi.Model;

namespace RestaurantApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly RestaurantDbContext dbContext;

    public CategoryController(RestaurantDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<Category>>> GetCategories()
    {
        List<Category> items = await dbContext.Categories.ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> CreateCategory(Category item)
    {
        dbContext.Categories.Add(item);
        await dbContext.SaveChangesAsync();
        return Ok(item);
    }
}
