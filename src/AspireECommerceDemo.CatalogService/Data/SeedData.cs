using AspireECommerceDemo.CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace AspireECommerceDemo.CatalogService.Data;

public static class SeedData
{
    public static async Task InitializeAsync(CatalogDbContext db)
    {
        if (await db.Categories.AnyAsync())
        {
            return;
        }

        var electronics = new Category { Id = new Guid("11111111-1111-1111-1111-111111111111"), Name = "Electronics", Description = "Gadgets and devices", ImageUrl = "https://placehold.co/150x150/e4e4e7/18181b?text=Electronics" };
        var clothing = new Category { Id = new Guid("22222222-2222-2222-2222-222222222222"), Name = "Clothing", Description = "Apparel and accessories", ImageUrl = "https://placehold.co/150x150/e4e4e7/18181b?text=Clothing" };
        var books = new Category { Id = new Guid("33333333-3333-3333-3333-333333333333"), Name = "Books", Description = "Physical and digital books", ImageUrl = "https://placehold.co/150x150/e4e4e7/18181b?text=Books" };
        var home = new Category { Id = new Guid("44444444-4444-4444-4444-444444444444"), Name = "Home & Kitchen", Description = "Home appliances and furniture", ImageUrl = "https://placehold.co/150x150/e4e4e7/18181b?text=Home" };

        db.Categories.AddRange(electronics, clothing, books, home);

        db.Products.AddRange(
            new Product { Name = "Smartphone X", Description = "Latest smartphone", Price = 999.99m, StockQuantity = 50, CategoryId = electronics.Id, ImageUrl = "https://placehold.co/150x150/e4e4e7/18181b?text=Phone" },
            new Product { Name = "Laptop Pro", Description = "High performance laptop", Price = 1999.99m, StockQuantity = 20, CategoryId = electronics.Id, ImageUrl = "https://placehold.co/150x150/e4e4e7/18181b?text=Laptop" },
            new Product { Name = "Wireless Earbuds", Description = "Noise cancelling", Price = 199.99m, StockQuantity = 100, CategoryId = electronics.Id, ImageUrl = "https://placehold.co/150x150/e4e4e7/18181b?text=Earbuds" },
            
            new Product { Name = "T-Shirt", Description = "Cotton t-shirt", Price = 19.99m, StockQuantity = 200, CategoryId = clothing.Id, ImageUrl = "https://placehold.co/150x150/e4e4e7/18181b?text=Shirt" },
            new Product { Name = "Jeans", Description = "Denim jeans", Price = 49.99m, StockQuantity = 150, CategoryId = clothing.Id, ImageUrl = "https://placehold.co/150x150/e4e4e7/18181b?text=Jeans" },
            
            new Product { Name = "C# in Depth", Description = "Learn C# deeply", Price = 39.99m, StockQuantity = 30, CategoryId = books.Id, ImageUrl = "https://placehold.co/150x150/e4e4e7/18181b?text=Book" },
            new Product { Name = "Domain-Driven Design", Description = "Tackling complexity", Price = 45.99m, StockQuantity = 25, CategoryId = books.Id, ImageUrl = "https://placehold.co/150x150/e4e4e7/18181b?text=Book" },
            
            new Product { Name = "Coffee Maker", Description = "Brew perfect coffee", Price = 89.99m, StockQuantity = 40, CategoryId = home.Id, ImageUrl = "https://placehold.co/150x150/e4e4e7/18181b?text=Coffee" },
            new Product { Name = "Blender", Description = "High speed blender", Price = 59.99m, StockQuantity = 60, CategoryId = home.Id, ImageUrl = "https://placehold.co/150x150/e4e4e7/18181b?text=Blender" }
        );

        await db.SaveChangesAsync();
    }
}
