using SalonCRM.Application.Services.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Services.Executors;

public interface IGetServicesExecutor
{
    Task<IReadOnlyList<ServiceResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CancellationToken cancellationToken = default);
}
