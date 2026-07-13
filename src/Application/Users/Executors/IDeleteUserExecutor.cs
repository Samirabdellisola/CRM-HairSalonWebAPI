using SalonCRM.Application.Common.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Users.Executors;

/// <summary>
/// Deletes a target user. CentralOffice can delete anyone; BranchAdmin can delete
/// only Staff/Customer users within their own branch.
/// </summary>
public interface IDeleteUserExecutor
{
    Task<GenericResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid targetUserId,
        CancellationToken cancellationToken = default);
}
