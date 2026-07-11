namespace AspireECommerceDemo.Contracts.Events;

public record OrderCreated(
    Guid OrderId,
    string CustomerName,
    string CustomerEmail,
    decimal TotalAmount,
    DateTime CreatedAt,
    List<OrderItemDto> Items);

public record OrderCancelled(
    Guid OrderId,
    string Reason,
    DateTime CancelledAt);

public record OrderItemDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice);
