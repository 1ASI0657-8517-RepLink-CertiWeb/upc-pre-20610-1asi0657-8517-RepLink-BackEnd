using CertiWeb.API.Security.Domain.Model.Commands;
using CertiWeb.API.Security.Domain.Services;
using CertiWeb.API.Shared.Infrastructure.BackgroundTasks;
using CertiWeb.API.Users.Domain.Model.Aggregates;
using Microsoft.Extensions.DependencyInjection;

namespace CertiWeb.API.Security.Infrastructure.Pipeline.Middleware.Components;

/// <summary>
/// Middleware implementing AC-01: when a modifying request against a sensitive endpoint
/// (PATCH/PUT/DELETE on /api/v1/cars/*) is rejected with 401 or 403, the attempt is recorded
/// asynchronously via the background task queue so it never adds latency to the response
/// that has already been decided by the authorization pipeline.
/// </summary>
public class SecurityAuditLoggingMiddleware(
    RequestDelegate next,
    IBackgroundTaskQueue backgroundTaskQueue,
    IServiceProvider serviceProvider)
{
    private static readonly string[] SensitiveMethods = { "PATCH", "PUT", "DELETE" };
    private const string SensitivePathPrefix = "/api/v1/cars";

    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);

        var statusCode = context.Response.StatusCode;
        if (statusCode != StatusCodes.Status401Unauthorized && statusCode != StatusCodes.Status403Forbidden)
            return;

        if (!SensitiveMethods.Contains(context.Request.Method, StringComparer.OrdinalIgnoreCase))
            return;

        if (!context.Request.Path.StartsWithSegments(SensitivePathPrefix, StringComparison.OrdinalIgnoreCase))
            return;

        // Capture only plain values before enqueueing: HttpContext may be recycled/disposed
        // once this request completes, so nothing from it can be safely touched later.
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        var endpoint = context.Request.Path.Value ?? string.Empty;
        var httpMethod = context.Request.Method;
        var userId = (context.Items["User"] as User)?.Id;

        backgroundTaskQueue.QueueBackgroundWorkItem(async _ =>
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var commandService = scope.ServiceProvider.GetRequiredService<ISecurityAuditLogCommandService>();
                await commandService.Handle(new CreateSecurityAuditLogCommand(
                    ipAddress,
                    endpoint,
                    httpMethod,
                    statusCode,
                    userId));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to persist security audit log: {ex.Message}");
            }
        });
    }
}
