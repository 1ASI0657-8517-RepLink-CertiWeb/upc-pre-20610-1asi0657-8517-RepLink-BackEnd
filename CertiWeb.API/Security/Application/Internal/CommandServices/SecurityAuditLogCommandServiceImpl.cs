using CertiWeb.API.Security.Domain.Model.Aggregates;
using CertiWeb.API.Security.Domain.Model.Commands;
using CertiWeb.API.Security.Domain.Repositories;
using CertiWeb.API.Security.Domain.Services;
using CertiWeb.API.Shared.Domain.Repositories;

namespace CertiWeb.API.Security.Application.Internal.CommandServices;

/// <summary>
/// Implementation of the security audit log command service.
/// </summary>
public class SecurityAuditLogCommandServiceImpl(
    ISecurityAuditLogRepository securityAuditLogRepository,
    IUnitOfWork unitOfWork) : ISecurityAuditLogCommandService
{
    public async Task<SecurityAuditLog> Handle(CreateSecurityAuditLogCommand command)
    {
        var log = new SecurityAuditLog(command);
        await securityAuditLogRepository.AddAsync(log);
        await unitOfWork.CompleteAsync();
        return log;
    }
}
