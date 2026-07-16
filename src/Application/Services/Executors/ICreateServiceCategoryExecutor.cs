using SalonCRM.Application.Services.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Services.Executors;

public interface ICreateServiceCategoryExecutor
{
    Task<ServiceCategoryResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CreateServiceCategoryRequest request,
        CancellationToken cancellationToken = default);
}
