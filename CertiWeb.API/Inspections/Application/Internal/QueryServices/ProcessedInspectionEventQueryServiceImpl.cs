using CertiWeb.API.Inspections.Domain.Model.Aggregates;
using CertiWeb.API.Inspections.Domain.Model.Queries;
using CertiWeb.API.Inspections.Domain.Repositories;
using CertiWeb.API.Inspections.Domain.Services;

namespace CertiWeb.API.Inspections.Application.Internal.QueryServices;

/// <summary>
/// Implementation of the processed inspection event query service.
/// </summary>
public class ProcessedInspectionEventQueryServiceImpl(IProcessedInspectionEventRepository processedInspectionEventRepository)
    : IProcessedInspectionEventQueryService
{
    public async Task<IEnumerable<ProcessedInspectionEvent>> Handle(GetRecentProcessedInspectionEventsQuery query)
    {
        var count = query.Count <= 0 ? 50 : query.Count;
        return await processedInspectionEventRepository.FindRecentAsync(count);
    }
}
