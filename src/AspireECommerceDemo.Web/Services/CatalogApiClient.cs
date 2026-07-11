using System.Net.Http.Json;

namespace AspireECommerceDemo.Web.Services;

public class CatalogApiClient(HttpClient httpClient)
{
    public async Task<List<ProductDto>> GetProductsAsync()
        => await httpClient.GetFromJsonAsync<List<ProductDto>>("/api/products/") ?? [];

    public async Task<ProductDto?> GetProductAsync(Guid id)
        => await httpClient.GetFromJsonAsync<ProductDto>($"/api/products/{id}");

    public async Task<List<CategoryDto>> GetCategoriesAsync()
        => await httpClient.GetFromJsonAsync<List<CategoryDto>>("/api/categories/") ?? [];

    public async Task<List<ProductDto>> SearchProductsAsync(string query)
        => await httpClient.GetFromJsonAsync<List<ProductDto>>($"/api/products/search?q={query}") ?? [];
}

public record ProductDto(Guid Id, string Name, string Description, decimal Price, string ImageUrl, int StockQuantity, Guid CategoryId, string? CategoryName);
public record CategoryDto(Guid Id, string Name, string Description, string ImageUrl);
