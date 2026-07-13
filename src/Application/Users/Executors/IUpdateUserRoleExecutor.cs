using SalonCRM.Application.Users.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Users.Executors;

/// <summary>
/// Changes a target user's role. CentralOffice can set BranchAdmin/Staff/Customer on
/// any user; BranchAdmin can set only Staff/Customer, only within their own branch.
/// Setting CentralOffice is always rejected.
/// </summary>
public interface IUpdateUserRoleExecutor
{
    Task<UserDetailsResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid targetUserId,
        UpdateUserRoleRequest request,
        CancellationToken cancellationToken = default);
}
