using CertiWeb.API.Inspections.Domain.Model.Aggregates;
using CertiWeb.API.Inspections.Domain.Model.Queries;

namespace CertiWeb.API.Inspections.Domain.Services;

/// <summary>
/// Service interface for handling processed inspection event read operations.
/// </summary>
public interface IProcessedInspectionEventQueryService
{
    Task<IEnumerable<ProcessedInspectionEvent>> Handle(GetRecentProcessedInspectionEventsQuery query);
}
