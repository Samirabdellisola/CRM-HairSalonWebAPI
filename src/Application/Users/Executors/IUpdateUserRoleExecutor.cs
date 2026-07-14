using SalonCRM.Application.Users.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Users.Executors;

/// <summary>
/// Changes a target user's role. CentralOffice can set Staff/Customer on any user;
/// BranchAdmin can set only Staff/Customer within their own branch.
/// Setting CentralOffice or BranchAdmin via this endpoint is always rejected
/// (BranchAdmin is assigned only when creating a branch or via assign-admin).
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
