using SalonCRM.Application.Branches.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Branches.Executors;

/// <summary>
/// Soft-deactivates a branch (IsActive = false). CentralOffice or BranchAdmin of that branch.
/// </summary>
public interface IDeactivateBranchExecutor
{
    Task<BranchResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid branchId,
        CancellationToken cancellationToken = default);
}
