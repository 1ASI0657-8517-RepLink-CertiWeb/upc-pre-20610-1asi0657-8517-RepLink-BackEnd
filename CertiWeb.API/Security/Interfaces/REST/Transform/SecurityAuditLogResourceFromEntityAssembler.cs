using CertiWeb.API.Security.Domain.Model.Aggregates;
using CertiWeb.API.Security.Interfaces.REST.Resources;

namespace CertiWeb.API.Security.Interfaces.REST.Transform;

/// <summary>
/// Assembler that transforms a SecurityAuditLog entity into a SecurityAuditLogResource.
/// </summary>
public static class SecurityAuditLogResourceFromEntityAssembler
{
    public static SecurityAuditLogResource ToResourceFromEntity(SecurityAuditLog entity)
    {
        return new SecurityAuditLogResource(
            entity.Id,
            entity.Timestamp,
            entity.IpAddress,
            entity.Endpoint,
            entity.HttpMethod,
            entity.StatusCode,
            entity.UserId);
    }
}
