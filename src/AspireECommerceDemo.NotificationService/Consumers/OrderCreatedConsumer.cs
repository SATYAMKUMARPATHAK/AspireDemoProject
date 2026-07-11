using AspireECommerceDemo.Contracts.Events;
using AspireECommerceDemo.NotificationService.Data;
using AspireECommerceDemo.NotificationService.Models;
using MassTransit;

namespace AspireECommerceDemo.NotificationService.Consumers;

public class OrderCreatedConsumer(NotificationDbContext db, ILogger<OrderCreatedConsumer> logger) : IConsumer<OrderCreated>
{
    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        var ev = context.Message;
        logger.LogInformation("Received OrderCreated for OrderId: {OrderId}", ev.OrderId);

        var notification = new Notification
        {
            Type = "Email",
            Recipient = ev.CustomerEmail,
            Subject = $"Order Confirmation - #{ev.OrderId}",
            Body = $"Thank you {ev.CustomerName} for your order of {ev.TotalAmount:C}!",
            Status = NotificationStatus.Sent,
            SentAt = DateTime.UtcNow
        };

        db.Notifications.Add(notification);
        await db.SaveChangesAsync();
        logger.LogInformation("Sent Order Confirmation email to {Email}", ev.CustomerEmail);
    }
}
