using CertiWeb.API.Inspections.Domain.Model.Aggregates;
using CertiWeb.API.Inspections.Domain.Model.Commands;
using CertiWeb.API.Inspections.Domain.Repositories;
using CertiWeb.API.Inspections.Domain.Services;
using CertiWeb.API.Shared.Domain.Repositories;

namespace CertiWeb.API.Inspections.Application.Internal.CommandServices;

/// <summary>
/// Implementation of the processed inspection event command service.
/// </summary>
public class ProcessedInspectionEventCommandServiceImpl(
    IProcessedInspectionEventRepository processedInspectionEventRepository,
    IUnitOfWork unitOfWork) : IProcessedInspectionEventCommandService
{
    public async Task<ProcessedInspectionEvent> Handle(CreateProcessedInspectionEventCommand command)
    {
        var processedEvent = new ProcessedInspectionEvent(command);
        await processedInspectionEventRepository.AddAsync(processedEvent);
        await unitOfWork.CompleteAsync();
        return processedEvent;
    }
}
