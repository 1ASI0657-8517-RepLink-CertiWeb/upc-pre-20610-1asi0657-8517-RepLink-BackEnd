namespace CertiWeb.API.Security.Domain.Model.Commands;

/// <summary>
/// Command to persist a rejected (401/403) request against a sensitive endpoint.
/// </summary>
/// <param name="IpAddress">Remote IP address of the caller, if available.</param>
/// <param name="Endpoint">Request path that was rejected.</param>
/// <param name="HttpMethod">HTTP method of the rejected request.</param>
/// <param name="StatusCode">The HTTP status code returned to the caller (401 or 403).</param>
/// <param name="UserId">Id of the authenticated user, when the token was valid but insufficiently privileged.</param>
public record CreateSecurityAuditLogCommand(
    string? IpAddress,
    string Endpoint,
    string HttpMethod,
    int StatusCode,
    int? UserId);
