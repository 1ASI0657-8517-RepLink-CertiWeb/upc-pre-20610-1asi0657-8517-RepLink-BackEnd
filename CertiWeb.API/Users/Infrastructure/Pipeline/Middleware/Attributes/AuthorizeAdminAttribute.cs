using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CertiWeb.API.Users.Domain.Model.Aggregates;

namespace CertiWeb.API.Users.Infrastructure.Pipeline.Middleware.Attributes;

/**
 * This attribute is used to decorate controllers and actions that require an admin role.
 * It relies on RequestAuthorizationMiddleware having already validated the token and stored
 * the resolved role claim in HttpContext.Items["UserRole"].
 * - If no user is signed in, it returns 401 (consistent with [Authorize]).
 * - If a user is signed in but does not carry the "admin" role, it returns 403.
 */
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAdminAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();

        if (allowAnonymous)
        {
            return;
        }

        var role = context.HttpContext.Items["UserRole"] as string;

        if (string.IsNullOrEmpty(role))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (!string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase))
        {
            // Note: intentionally not ForbidResult() - this project has no AddAuthentication()
            // scheme registered, so ForbidResult would fail trying to resolve IAuthenticationService.
            context.Result = new ObjectResult(new { message = "Forbidden: admin role required" })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}
