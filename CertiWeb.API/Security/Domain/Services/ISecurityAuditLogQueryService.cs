using CertiWeb.API.Security.Domain.Model.Aggregates;
using CertiWeb.API.Security.Domain.Model.Queries;

namespace CertiWeb.API.Security.Domain.Services;

/// <summary>
/// Service interface for handling security audit log read operations.
/// </summary>
public interface ISecurityAuditLogQueryService
{
    Task<IEnumerable<SecurityAuditLog>> Handle(GetRecentSecurityAuditLogsQuery query);
}
