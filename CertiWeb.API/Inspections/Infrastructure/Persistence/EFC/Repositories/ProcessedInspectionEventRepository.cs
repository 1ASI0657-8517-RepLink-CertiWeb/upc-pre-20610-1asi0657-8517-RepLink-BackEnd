using CertiWeb.API.Inspections.Domain.Model.Aggregates;
using CertiWeb.API.Inspections.Domain.Repositories;
using CertiWeb.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using CertiWeb.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CertiWeb.API.Inspections.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
/// Entity Framework Core implementation of the processed inspection event repository.
/// </summary>
public class ProcessedInspectionEventRepository(AppDbContext context)
    : BaseRepository<ProcessedInspectionEvent>(context), IProcessedInspectionEventRepository
{
    public async Task<IEnumerable<ProcessedInspectionEvent>> FindRecentAsync(int count)
    {
        return await Context.Set<ProcessedInspectionEvent>()
            .OrderByDescending(e => e.ReceivedAt)
            .Take(count)
            .ToListAsync();
    }
}
