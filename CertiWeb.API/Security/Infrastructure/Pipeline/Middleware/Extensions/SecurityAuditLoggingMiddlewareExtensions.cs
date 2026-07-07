using CertiWeb.API.Security.Infrastructure.Pipeline.Middleware.Components;

namespace CertiWeb.API.Security.Infrastructure.Pipeline.Middleware.Extensions;

/// <summary>
/// Extension method to register SecurityAuditLoggingMiddleware in the ASP.NET Core pipeline.
/// </summary>
public static class SecurityAuditLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityAuditLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SecurityAuditLoggingMiddleware>();
    }
}
