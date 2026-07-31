using Microsoft.AspNetCore.Mvc;
using RestaurantApi.Data;

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
}
