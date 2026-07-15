using SalonCRM.Application.Services.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Services.Executors;

public interface ICreateServiceExecutor
{
    Task<ServiceResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CreateServiceRequest request,
        CancellationToken cancellationToken = default);
}
