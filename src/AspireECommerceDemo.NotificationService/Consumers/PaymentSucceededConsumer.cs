using AspireECommerceDemo.Contracts.Events;
using AspireECommerceDemo.NotificationService.Data;
using AspireECommerceDemo.NotificationService.Models;
using MassTransit;

namespace AspireECommerceDemo.NotificationService.Consumers;

public class PaymentSucceededConsumer(NotificationDbContext db, ILogger<PaymentSucceededConsumer> logger) : IConsumer<PaymentSucceeded>
{
    public async Task Consume(ConsumeContext<PaymentSucceeded> context)
    {
        var ev = context.Message;
        logger.LogInformation("Received PaymentSucceeded for OrderId: {OrderId}", ev.OrderId);

        var notification = new Notification
        {
            Type = "Email",
            Recipient = "customer@example.com", // In a real app we'd query the order or pass it in event
            Subject = $"Payment Receipt - #{ev.OrderId}",
            Body = $"We have received your payment of {ev.Amount:C}. Transaction ID: {ev.TransactionId}",
            Status = NotificationStatus.Sent,
            SentAt = DateTime.UtcNow
        };

        db.Notifications.Add(notification);
        await db.SaveChangesAsync();
        logger.LogInformation("Sent Payment Receipt email for OrderId: {OrderId}", ev.OrderId);
    }
}
