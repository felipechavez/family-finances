namespace FinanceApp.API.Endpoints;
using FinanceApp.Application.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

/// <summary>
/// Maps in-app notification endpoints: list, mark-read, mark-all-read.
/// </summary>
internal static class NotificationsEndpoints
{
    internal static IEndpointRouteBuilder MapNotificationsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/notifications")
            .WithTags("Notifications")
            .RequireAuthorization();

        // GET /notifications — últimas 30 del usuario autenticado
        group.MapGet("/", async (ClaimsPrincipal user, INotificationService svc) =>
        {
            var userId = user.GetUserId();
            var items  = await svc.GetRecentAsync(userId);
            return TypedResults.Ok(items);
        })
        .WithName("GetNotifications");

        // GET /notifications/unread-count
        group.MapGet("/unread-count", async (ClaimsPrincipal user, INotificationService svc) =>
        {
            var userId = user.GetUserId();
            var count  = await svc.GetUnreadCountAsync(userId);
            return TypedResults.Ok(new { count });
        })
        .WithName("GetUnreadCount");

        // PATCH /notifications/{id}/read
        group.MapPatch("/{id:guid}/read", async (Guid id, ClaimsPrincipal user, INotificationService svc) =>
        {
            var userId = user.GetUserId();
            await svc.MarkReadAsync(id, userId);
            return TypedResults.NoContent();
        })
        .WithName("MarkNotificationRead");

        // PATCH /notifications/read-all
        group.MapPatch("/read-all", async (ClaimsPrincipal user, INotificationService svc) =>
        {
            var userId = user.GetUserId();
            await svc.MarkAllReadAsync(userId);
            return TypedResults.NoContent();
        })
        .WithName("MarkAllNotificationsRead");

        return app;
    }
}
