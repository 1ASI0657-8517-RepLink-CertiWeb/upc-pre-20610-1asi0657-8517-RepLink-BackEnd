using CertiWeb.API.Security.Domain.Model.Aggregates;
using CertiWeb.API.Security.Domain.Repositories;
using CertiWeb.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using CertiWeb.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CertiWeb.API.Security.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
/// Entity Framework Core implementation of the security audit log repository.
/// </summary>
public class SecurityAuditLogRepository(AppDbContext context)
    : BaseRepository<SecurityAuditLog>(context), ISecurityAuditLogRepository
{
    public async Task<IEnumerable<SecurityAuditLog>> FindRecentAsync(int count)
    {
        return await Context.Set<SecurityAuditLog>()
            .OrderByDescending(l => l.Timestamp)
            .Take(count)
            .ToListAsync();
    }
}
