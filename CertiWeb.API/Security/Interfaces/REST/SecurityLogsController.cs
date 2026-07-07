using System.Net.Mime;
using CertiWeb.API.Security.Domain.Model.Queries;
using CertiWeb.API.Security.Domain.Services;
using CertiWeb.API.Security.Interfaces.REST.Resources;
using CertiWeb.API.Security.Interfaces.REST.Transform;
using CertiWeb.API.Users.Infrastructure.Pipeline.Middleware.Attributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CertiWeb.API.Security.Interfaces.REST;

/// <summary>
/// REST API controller for reviewing security audit logs (AC-01: rejected/unauthorized access attempts).
/// Restricted to admin users only.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Admin-only endpoint for reviewing security audit logs.")]
[AuthorizeAdmin]
public class SecurityLogsController(ISecurityAuditLogQueryService securityAuditLogQueryService) : ControllerBase
{
    /// <summary>
    /// Retrieves the most recent security audit log entries, newest first.
    /// </summary>
    /// <param name="count">Maximum number of entries to return (default 50).</param>
    [HttpGet]
    [SwaggerOperation(Summary = "List the most recent security audit log entries (admin only)")]
    [SwaggerResponse(StatusCodes.Status200OK, "The most recent security audit log entries")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "No valid authentication token was supplied")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "The authenticated user does not have the admin role")]
    public async Task<ActionResult<IEnumerable<SecurityAuditLogResource>>> GetRecentLogs([FromQuery] int count = 50)
    {
        var query = new GetRecentSecurityAuditLogsQuery(count);
        var logs = await securityAuditLogQueryService.Handle(query);
        var resources = logs.Select(SecurityAuditLogResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}
