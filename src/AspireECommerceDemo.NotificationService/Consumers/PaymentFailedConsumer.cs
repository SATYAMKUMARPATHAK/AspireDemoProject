using AspireECommerceDemo.Contracts.Events;
using AspireECommerceDemo.NotificationService.Data;
using AspireECommerceDemo.NotificationService.Models;
using MassTransit;

namespace AspireECommerceDemo.NotificationService.Consumers;

public class PaymentFailedConsumer(NotificationDbContext db, ILogger<PaymentFailedConsumer> logger) : IConsumer<PaymentFailed>
{
    public async Task Consume(ConsumeContext<PaymentFailed> context)
    {
        var ev = context.Message;
        logger.LogInformation("Received PaymentFailed for OrderId: {OrderId}", ev.OrderId);

        var notification = new Notification
        {
            Type = "Email",
            Recipient = "customer@example.com",
            Subject = $"Payment Failed - #{ev.OrderId}",
            Body = $"Your payment of {ev.Amount:C} failed. Reason: {ev.Reason}. Please update your payment method.",
            Status = NotificationStatus.Sent,
            SentAt = DateTime.UtcNow
        };

        db.Notifications.Add(notification);
        await db.SaveChangesAsync();
        logger.LogInformation("Sent Payment Failed email for OrderId: {OrderId}", ev.OrderId);
    }
}
