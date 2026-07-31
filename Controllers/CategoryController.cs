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
}
