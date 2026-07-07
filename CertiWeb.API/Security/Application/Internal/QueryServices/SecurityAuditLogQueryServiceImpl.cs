using CertiWeb.API.Security.Domain.Model.Aggregates;
using CertiWeb.API.Security.Domain.Model.Queries;
using CertiWeb.API.Security.Domain.Repositories;
using CertiWeb.API.Security.Domain.Services;

namespace CertiWeb.API.Security.Application.Internal.QueryServices;

/// <summary>
/// Implementation of the security audit log query service.
/// </summary>
public class SecurityAuditLogQueryServiceImpl(ISecurityAuditLogRepository securityAuditLogRepository)
    : ISecurityAuditLogQueryService
{
    public async Task<IEnumerable<SecurityAuditLog>> Handle(GetRecentSecurityAuditLogsQuery query)
    {
        var count = query.Count <= 0 ? 50 : query.Count;
        return await securityAuditLogRepository.FindRecentAsync(count);
    }
}
