using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using RestaurantApi.Data;

var builder = WebApplication.CreateBuilder(args);

// 註冊 RestaurantDbContext（連線字串由環境變數 ConnectionStrings__DefaultConnection 注入）
builder.Services.AddDbContext<RestaurantDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 開發環境測試用功能，由於是在 UseHttpsRedirection() 之前啟用，因此會透過 HTTP 直接存取
if (app.Environment.IsDevelopment())
{
    // 起始資料庫不存在資料時，自動新增測試資料
    using (IServiceScope scope = app.Services.CreateScope())
    {
        RestaurantDbContext dbContext = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();
        DbInitializer.Seed(dbContext);
    }
    // 啟用 Swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

// EB ALB 做 SSL 終止後以 HTTP 轉發，需還原原始 Scheme 避免誤判導致重導向迴圈
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection(); // 判斷是否為 HTTPS 並重導向
app.MapControllers();

app.Run();
