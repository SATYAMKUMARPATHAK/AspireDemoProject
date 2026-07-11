using AspireECommerceDemo.Contracts.Events;
using AspireECommerceDemo.OrderService.Data;
using AspireECommerceDemo.OrderService.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AspireECommerceDemo.OrderService.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/orders");

        group.MapPost("/", async (CreateOrderRequest request, OrderDbContext db, IPublishEndpoint publishEndpoint) =>
        {
            var order = new Order
            {
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                ShippingAddress = request.ShippingAddress,
                TotalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice),
                Items = request.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            db.Orders.Add(order);
            await db.SaveChangesAsync();

            var eventItems = order.Items.Select(i => new AspireECommerceDemo.Contracts.Events.OrderItemDto(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice)).ToList();
            var orderCreatedEvent = new OrderCreated(order.Id, order.CustomerName, order.CustomerEmail, order.TotalAmount, order.CreatedAt, eventItems);
            
            await publishEndpoint.Publish(orderCreatedEvent);

            return Results.Created($"/api/orders/{order.Id}", MapToDto(order));
        });

        group.MapGet("/", async (OrderDbContext db) =>
        {
            var orders = await db.Orders.Include(o => o.Items).OrderByDescending(o => o.CreatedAt).ToListAsync();
            return Results.Ok(orders.Select(MapToDto));
        });

        group.MapGet("/{id}", async (Guid id, OrderDbContext db) =>
        {
            var order = await db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
            return order != null ? Results.Ok(MapToDto(order)) : Results.NotFound();
        });

        group.MapGet("/customer/{email}", async (string email, OrderDbContext db) =>
        {
            var orders = await db.Orders.Include(o => o.Items).Where(o => o.CustomerEmail == email).ToListAsync();
            return Results.Ok(orders.Select(MapToDto));
        });

        group.MapPut("/{id}/cancel", async (Guid id, OrderDbContext db, IPublishEndpoint publishEndpoint) =>
        {
            var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return Results.NotFound();

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            await publishEndpoint.Publish(new OrderCancelled(order.Id, "User requested cancellation", DateTime.UtcNow));

            return Results.NoContent();
        });
    }

    private static OrderResponseDto MapToDto(Order o) => new OrderResponseDto(
        o.Id, o.CustomerName, o.CustomerEmail, o.TotalAmount, o.Status.ToString(), o.CreatedAt,
        o.Items.Select(i => new OrderItemResponseDto(i.Id, i.ProductId, i.ProductName, i.Quantity, i.UnitPrice)).ToList()
    );
}

public record CreateOrderRequest(string CustomerName, string CustomerEmail, string ShippingAddress, List<OrderItemRequest> Items);
public record OrderItemRequest(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);

public record OrderResponseDto(Guid Id, string CustomerName, string CustomerEmail, decimal TotalAmount, string Status, DateTime CreatedAt, List<OrderItemResponseDto> Items);
public record OrderItemResponseDto(Guid Id, Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);
