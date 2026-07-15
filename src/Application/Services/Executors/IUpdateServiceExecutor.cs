using SalonCRM.Application.Services.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Services.Executors;

public interface IUpdateServiceExecutor
{
    Task<ServiceResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid serviceId,
        UpdateServiceRequest request,
        CancellationToken cancellationToken = default);
}
