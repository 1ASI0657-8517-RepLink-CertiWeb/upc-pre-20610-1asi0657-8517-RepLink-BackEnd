using CertiWeb.API.Security.Domain.Model.Aggregates;
using CertiWeb.API.Shared.Domain.Repositories;

namespace CertiWeb.API.Security.Domain.Repositories;

/// <summary>
/// Repository contract for security audit log entries.
/// </summary>
public interface ISecurityAuditLogRepository : IBaseRepository<SecurityAuditLog>
{
    /// <summary>
    /// Retrieves the most recent audit log entries, ordered by timestamp descending.
    /// </summary>
    /// <param name="count">Maximum number of entries to return.</param>
    Task<IEnumerable<SecurityAuditLog>> FindRecentAsync(int count);
}
