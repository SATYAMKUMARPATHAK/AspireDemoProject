using AspireECommerceDemo.Contracts.Events;
using AspireECommerceDemo.PaymentService.Data;
using AspireECommerceDemo.PaymentService.Models;
using MassTransit;

namespace AspireECommerceDemo.PaymentService.Consumers;

public class OrderCreatedConsumer(PaymentDbContext db, ILogger<OrderCreatedConsumer> logger, IPublishEndpoint publishEndpoint) : IConsumer<OrderCreated>
{
    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        var ev = context.Message;
        logger.LogInformation("Processing payment for OrderId: {OrderId}", ev.OrderId);

        var payment = new Payment
        {
            OrderId = ev.OrderId,
            Amount = ev.TotalAmount,
            Status = PaymentStatus.Processing
        };
        db.Payments.Add(payment);
        await db.SaveChangesAsync();

        // Simulate payment processing delay
        await Task.Delay(2000);

        // 80% chance of success for demo purposes
        var random = new Random();
        if (random.Next(1, 101) <= 80)
        {
            payment.Status = PaymentStatus.Succeeded;
            payment.TransactionId = $"txn_{Guid.NewGuid():N}";
            payment.ProcessedAt = DateTime.UtcNow;
            
            await publishEndpoint.Publish(new PaymentSucceeded(payment.Id, payment.OrderId, payment.Amount, payment.TransactionId, payment.ProcessedAt.Value));
            logger.LogInformation("Payment succeeded for OrderId: {OrderId}", ev.OrderId);
        }
        else
        {
            payment.Status = PaymentStatus.Failed;
            payment.FailureReason = "Insufficient funds / Card declined";
            payment.ProcessedAt = DateTime.UtcNow;
            
            await publishEndpoint.Publish(new PaymentFailed(payment.Id, payment.OrderId, payment.Amount, payment.FailureReason, payment.ProcessedAt.Value));
            logger.LogWarning("Payment failed for OrderId: {OrderId}", ev.OrderId);
        }

        await db.SaveChangesAsync();
    }
}
