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
    public async Task<ActionResult<List<CategoryDto>>> GetCategories()
    {
        List<CategoryDto> categories = await dbContext.Categories
            .Select(category => new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
            })
            .ToListAsync();


        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        Category? category = await dbContext.Categories.FindAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        CategoryDto result = new()
        {
            Id = category.Id,
            Name = category.Name,
        };

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto request)
    {
        Category category = new()
        {
            Name = request.Name,
        };

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        CategoryDto result = new()
        {
            Id = category.Id,
            Name = category.Name,
        };

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory(int id, CreateCategoryDto request)
    {
        Category? category = await dbContext.Categories.FindAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        category.Name = request.Name;
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        Category? category = await dbContext.Categories.FindAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}
