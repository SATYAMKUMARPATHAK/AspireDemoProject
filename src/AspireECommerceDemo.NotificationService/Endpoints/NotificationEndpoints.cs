using AspireECommerceDemo.NotificationService.Data;
using Microsoft.EntityFrameworkCore;

namespace AspireECommerceDemo.NotificationService.Endpoints;

public static class NotificationEndpoints
{
    public static void MapNotificationEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/notifications");

        group.MapGet("/", async (NotificationDbContext db) =>
        {
            var notifications = await db.Notifications.OrderByDescending(n => n.CreatedAt).ToListAsync();
            return Results.Ok(notifications);
        });

        group.MapGet("/{email}", async (string email, NotificationDbContext db) =>
        {
            var notifications = await db.Notifications.Where(n => n.Recipient == email).OrderByDescending(n => n.CreatedAt).ToListAsync();
            return Results.Ok(notifications);
        });
    }
}
