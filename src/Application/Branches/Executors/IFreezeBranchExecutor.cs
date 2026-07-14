using SalonCRM.Application.Branches.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Branches.Executors;

/// <summary>
/// Freezes a branch (IsFrozen = true). CentralOffice or BranchAdmin of that branch.
/// </summary>
public interface IFreezeBranchExecutor
{
    Task<BranchResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid branchId,
        CancellationToken cancellationToken = default);
}
