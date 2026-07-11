using AspireECommerceDemo.PaymentService.Data;
using Microsoft.EntityFrameworkCore;

namespace AspireECommerceDemo.PaymentService.Endpoints;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/payments");

        group.MapGet("/", async (PaymentDbContext db) =>
        {
            var payments = await db.Payments.OrderByDescending(p => p.CreatedAt).ToListAsync();
            return Results.Ok(payments);
        });

        group.MapGet("/order/{orderId}", async (Guid orderId, PaymentDbContext db) =>
        {
            var payment = await db.Payments.Where(p => p.OrderId == orderId).OrderByDescending(p => p.CreatedAt).FirstOrDefaultAsync();
            return payment != null ? Results.Ok(payment) : Results.NotFound();
        });
    }
}
