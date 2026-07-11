using AspireECommerceDemo.CatalogService.Data;
using AspireECommerceDemo.CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace AspireECommerceDemo.CatalogService.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/categories");

        group.MapGet("/", async (CatalogDbContext db) =>
        {
            var categories = await db.Categories.Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.ImageUrl)).ToListAsync();
            return Results.Ok(categories);
        });

        group.MapGet("/{id}", async (Guid id, CatalogDbContext db) =>
        {
            var category = await db.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return Results.NotFound();
            return Results.Ok(category);
        });
    }
}

public record CategoryDto(Guid Id, string Name, string Description, string ImageUrl);
