using SalonCRM.Application.Branches.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Branches.Executors;

/// <summary>
/// Promotes a Staff user of the branch to BranchAdmin, demoting the previous admin if any.
/// CentralOffice or BranchAdmin of that branch.
/// </summary>
public interface IAssignBranchAdminExecutor
{
    Task<BranchResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid branchId,
        AssignBranchAdminRequest request,
        CancellationToken cancellationToken = default);
}
