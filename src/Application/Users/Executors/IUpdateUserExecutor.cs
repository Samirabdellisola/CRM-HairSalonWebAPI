using SalonCRM.Application.Users.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Users.Executors;

/// <summary>
/// Updates Email/Phone/Address on a target user. CentralOffice may edit any
/// user; BranchAdmin may edit only Staff/Customer users within their own branch.
/// </summary>
public interface IUpdateUserExecutor
{
    Task<UserDetailsResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid targetUserId,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default);
}
