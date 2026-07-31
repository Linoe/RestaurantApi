using Microsoft.AspNetCore.Mvc;
using RestaurantApi.Data;

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
}
