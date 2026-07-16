using SalonCRM.Application.Common.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Services.Executors;

public interface IDeleteServiceCategoryExecutor
{
    Task<GenericResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid categoryId,
        CancellationToken cancellationToken = default);
}
