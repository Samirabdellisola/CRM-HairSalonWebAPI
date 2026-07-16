using SalonCRM.Application.Services.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Services.Executors;

public interface IGetServiceCategoryByIdExecutor
{
    Task<ServiceCategoryResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid categoryId,
        CancellationToken cancellationToken = default);
}
