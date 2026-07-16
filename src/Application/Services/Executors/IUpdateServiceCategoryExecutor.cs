using SalonCRM.Application.Services.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Services.Executors;

public interface IUpdateServiceCategoryExecutor
{
    Task<ServiceCategoryResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid categoryId,
        UpdateServiceCategoryRequest request,
        CancellationToken cancellationToken = default);
}
