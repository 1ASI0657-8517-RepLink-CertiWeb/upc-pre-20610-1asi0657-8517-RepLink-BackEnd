using CertiWeb.API.Security.Domain.Model.Aggregates;
using CertiWeb.API.Security.Domain.Model.Commands;

namespace CertiWeb.API.Security.Domain.Services;

/// <summary>
/// Service interface for handling security audit log write operations.
/// </summary>
public interface ISecurityAuditLogCommandService
{
    Task<SecurityAuditLog> Handle(CreateSecurityAuditLogCommand command);
}
