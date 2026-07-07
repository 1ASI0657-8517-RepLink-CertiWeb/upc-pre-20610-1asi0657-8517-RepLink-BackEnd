namespace CertiWeb.API.Security.Domain.Model.Queries;

/// <summary>
/// Query for retrieving the most recent security audit log entries, newest first.
/// </summary>
/// <param name="Count">Maximum number of entries to return.</param>
public record GetRecentSecurityAuditLogsQuery(int Count);
