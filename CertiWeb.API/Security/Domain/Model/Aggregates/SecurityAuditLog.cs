using System.Diagnostics.CodeAnalysis;
using CertiWeb.API.Security.Domain.Model.Commands;

namespace CertiWeb.API.Security.Domain.Model.Aggregates;

/// <summary>
/// Represents a single unauthorized/forbidden access attempt recorded for security auditing (AC-01).
/// </summary>
public class SecurityAuditLog
{
    public int Id { get; private set; }

    /// <summary>
    /// UTC timestamp of when the attempt was detected.
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// Remote IP address of the caller, if available.
    /// </summary>
    public string? IpAddress { get; private set; }

    /// <summary>
    /// Request path that was rejected (e.g. /api/v1/cars/5).
    /// </summary>
    public required string Endpoint { get; set; }

    /// <summary>
    /// HTTP method of the rejected request (PATCH, PUT, DELETE, ...).
    /// </summary>
    public required string HttpMethod { get; set; }

    /// <summary>
    /// The HTTP status code that was returned to the caller (401 or 403).
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Id of the authenticated user, when the JWT was valid but lacked sufficient permissions.
    /// Null when there was no token or the token was invalid.
    /// </summary>
    public int? UserId { get; set; }

    [SetsRequiredMembers]
    public SecurityAuditLog()
    {
        Endpoint = string.Empty;
        HttpMethod = string.Empty;
        Timestamp = DateTime.UtcNow;
    }

    [SetsRequiredMembers]
    public SecurityAuditLog(CreateSecurityAuditLogCommand command)
    {
        Timestamp = DateTime.UtcNow;
        IpAddress = command.IpAddress;
        Endpoint = command.Endpoint;
        HttpMethod = command.HttpMethod;
        StatusCode = command.StatusCode;
        UserId = command.UserId;
    }
}
