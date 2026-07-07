namespace CertiWeb.API.Security.Interfaces.REST.Resources;

/// <summary>
/// REST resource representing a security audit log entry.
/// </summary>
/// <param name="Id">Unique identifier of the log entry.</param>
/// <param name="Timestamp">UTC timestamp of when the attempt was detected.</param>
/// <param name="IpAddress">Remote IP address of the caller, if available.</param>
/// <param name="Endpoint">Request path that was rejected.</param>
/// <param name="HttpMethod">HTTP method of the rejected request.</param>
/// <param name="StatusCode">The HTTP status code returned to the caller (401 or 403).</param>
/// <param name="UserId">Id of the authenticated user, when the token was valid but insufficiently privileged.</param>
public record SecurityAuditLogResource(
    int Id,
    DateTime Timestamp,
    string? IpAddress,
    string Endpoint,
    string HttpMethod,
    int StatusCode,
    int? UserId);
