using System.Text.Json;
using AspireECommerceDemo.CatalogService.Data;
using AspireECommerceDemo.CatalogService.Models;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace AspireECommerceDemo.CatalogService.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/products");

        group.MapGet("/", async (CatalogDbContext db, IConnectionMultiplexer redis) =>
        {
            var dbRedis = redis.GetDatabase();
            var cached = await dbRedis.StringGetAsync("products:all");
            if (!cached.IsNullOrEmpty)
            {
                return Results.Ok(JsonSerializer.Deserialize<List<ProductDto>>((string)cached!));
            }

            var products = await db.Products.Include(p => p.Category)
                .Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.ImageUrl, p.StockQuantity, p.CategoryId, p.Category != null ? p.Category.Name : null))
                .ToListAsync();

            await dbRedis.StringSetAsync("products:all", JsonSerializer.Serialize(products), TimeSpan.FromMinutes(5));
            return Results.Ok(products);
        });

        group.MapGet("/{id}", async (Guid id, CatalogDbContext db, IConnectionMultiplexer redis) =>
        {
            var dbRedis = redis.GetDatabase();
            var cacheKey = $"product:{id}";
            var cached = await dbRedis.StringGetAsync(cacheKey);
            if (!cached.IsNullOrEmpty)
            {
                return Results.Ok(JsonSerializer.Deserialize<ProductDto>((string)cached!));
            }

            var p = await db.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);
            if (p == null) return Results.NotFound();

            var dto = new ProductDto(p.Id, p.Name, p.Description, p.Price, p.ImageUrl, p.StockQuantity, p.CategoryId, p.Category?.Name);
            await dbRedis.StringSetAsync(cacheKey, JsonSerializer.Serialize(dto), TimeSpan.FromMinutes(5));
            return Results.Ok(dto);
        });

        group.MapGet("/category/{categoryId}", async (Guid categoryId, CatalogDbContext db) =>
        {
            var products = await db.Products.Include(p => p.Category).Where(p => p.CategoryId == categoryId)
                .Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.ImageUrl, p.StockQuantity, p.CategoryId, p.Category != null ? p.Category.Name : null))
                .ToListAsync();
            return Results.Ok(products);
        });

        group.MapGet("/search", async (string q, CatalogDbContext db) =>
        {
            var products = await db.Products.Include(p => p.Category)
                .Where(p => p.Name.Contains(q) || p.Description.Contains(q))
                .Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.ImageUrl, p.StockQuantity, p.CategoryId, p.Category != null ? p.Category.Name : null))
                .ToListAsync();
            return Results.Ok(products);
        });

        group.MapPost("/", async (Product product, CatalogDbContext db, IConnectionMultiplexer redis) =>
        {
            db.Products.Add(product);
            await db.SaveChangesAsync();
            
            var dbRedis = redis.GetDatabase();
            await dbRedis.KeyDeleteAsync("products:all");
            
            return Results.Created($"/api/products/{product.Id}", product);
        });
    }
}

public record ProductDto(Guid Id, string Name, string Description, decimal Price, string ImageUrl, int StockQuantity, Guid CategoryId, string? CategoryName);
