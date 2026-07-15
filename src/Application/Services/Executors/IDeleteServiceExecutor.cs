using SalonCRM.Application.Common.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Services.Executors;

public interface IDeleteServiceExecutor
{
    Task<GenericResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid serviceId,
        CancellationToken cancellationToken = default);
}
