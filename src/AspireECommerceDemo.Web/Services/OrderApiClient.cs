using System.Net.Http.Json;

namespace AspireECommerceDemo.Web.Services;

public class OrderApiClient(HttpClient httpClient)
{
    public async Task<OrderDto?> CreateOrderAsync(CreateOrderRequest request)
    {
        var response = await httpClient.PostAsJsonAsync("/api/orders/", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderDto>();
    }

    public async Task<List<OrderDto>> GetOrdersAsync()
        => await httpClient.GetFromJsonAsync<List<OrderDto>>("/api/orders/") ?? [];

    public async Task<OrderDto?> GetOrderAsync(Guid id)
        => await httpClient.GetFromJsonAsync<OrderDto>($"/api/orders/{id}");
}

public record CreateOrderRequest(string CustomerName, string CustomerEmail, string ShippingAddress, List<OrderItemRequest> Items);
public record OrderItemRequest(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);
public record OrderDto(Guid Id, string CustomerName, string CustomerEmail, decimal TotalAmount, string Status, DateTime CreatedAt, List<OrderItemDto> Items);
public record OrderItemDto(Guid Id, Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);
