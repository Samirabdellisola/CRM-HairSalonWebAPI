using SalonCRM.Application.Branches.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Branches.Executors;

/// <summary>
/// Updates Name/Address/Phone. CentralOffice or the BranchAdmin of that branch.
/// </summary>
public interface IUpdateBranchExecutor
{
    Task<BranchResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid branchId,
        UpdateBranchRequest request,
        CancellationToken cancellationToken = default);
}
