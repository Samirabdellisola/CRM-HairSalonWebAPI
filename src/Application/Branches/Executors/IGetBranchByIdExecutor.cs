using SalonCRM.Application.Branches.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Branches.Executors;

/// <summary>
/// Returns a single branch. BranchAdmin may only access the branch they administer.
/// </summary>
public interface IGetBranchByIdExecutor
{
    Task<BranchResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid branchId,
        CancellationToken cancellationToken = default);
}
