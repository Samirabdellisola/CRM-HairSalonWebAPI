using SalonCRM.Application.Auth.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Auth.Executors;

/// <summary>
/// Registers a new Staff account. Caller must be CentralOffice or BranchAdmin;
/// when the caller is a BranchAdmin, the request's BranchId must match their own branch.
/// </summary>
public interface IRegisterStaffExecutor
{
    Task<RegisterUserResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        RegisterStaffRequest request,
        CancellationToken cancellationToken = default);
}
