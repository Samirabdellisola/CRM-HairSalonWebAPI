using SalonCRM.Application.Customers.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Customers.Executors;

public interface IUploadCustomerPhotoExecutor
{
    Task<ProfileResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid customerId,
        UploadCustomerPhotoRequest request,
        CancellationToken cancellationToken = default);
}
