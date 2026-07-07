using CertiWeb.API.Inspections.Domain.Model.Aggregates;
using CertiWeb.API.Inspections.Interfaces.REST.Resources;

namespace CertiWeb.API.Inspections.Interfaces.REST.Transform;

/// <summary>
/// Assembler that transforms a ProcessedInspectionEvent entity into an InspectionEventResource.
/// </summary>
public static class InspectionEventResourceFromEntityAssembler
{
    public static InspectionEventResource ToResourceFromEntity(ProcessedInspectionEvent entity)
    {
        return new InspectionEventResource(
            entity.Id,
            entity.ReceivedAt,
            entity.RawMessage,
            entity.Status);
    }
}
