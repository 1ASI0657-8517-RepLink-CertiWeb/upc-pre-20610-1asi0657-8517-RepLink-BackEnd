using CertiWeb.API.Inspections.Domain.Model.Aggregates;
using CertiWeb.API.Shared.Domain.Repositories;

namespace CertiWeb.API.Inspections.Domain.Repositories;

/// <summary>
/// Repository contract for processed inspection events.
/// </summary>
public interface IProcessedInspectionEventRepository : IBaseRepository<ProcessedInspectionEvent>
{
    /// <summary>
    /// Retrieves the most recently processed events, ordered by received date descending.
    /// </summary>
    /// <param name="count">Maximum number of entries to return.</param>
    Task<IEnumerable<ProcessedInspectionEvent>> FindRecentAsync(int count);
}
