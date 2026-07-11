using AspireECommerceDemo.CatalogService.Data;
using AspireECommerceDemo.CatalogService.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddSqlServerDbContext<CatalogDbContext>("catalogdb");
builder.AddRedisClient("redis");

var app = builder.Build();

app.MapDefaultEndpoints();

// Auto-migrate and seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await db.Database.EnsureCreatedAsync();
    await SeedData.InitializeAsync(db);
}

app.MapProductEndpoints();
app.MapCategoryEndpoints();

app.Run();
