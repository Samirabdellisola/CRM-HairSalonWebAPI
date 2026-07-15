using SalonCRM.Application.Services.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Services.Executors;

public interface IGetServiceByIdExecutor
{
    Task<ServiceResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid serviceId,
        CancellationToken cancellationToken = default);
}
