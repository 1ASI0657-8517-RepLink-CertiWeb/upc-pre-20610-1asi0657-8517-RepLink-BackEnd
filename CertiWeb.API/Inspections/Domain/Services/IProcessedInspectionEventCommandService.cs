using CertiWeb.API.Inspections.Domain.Model.Aggregates;
using CertiWeb.API.Inspections.Domain.Model.Commands;

namespace CertiWeb.API.Inspections.Domain.Services;

/// <summary>
/// Service interface for handling processed inspection event write operations.
/// </summary>
public interface IProcessedInspectionEventCommandService
{
    Task<ProcessedInspectionEvent> Handle(CreateProcessedInspectionEventCommand command);
}
