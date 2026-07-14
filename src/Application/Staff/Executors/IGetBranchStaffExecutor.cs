using SalonCRM.Application.Staff.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Staff.Executors;

/// <summary>
/// Lists Staff users belonging to a branch. CentralOffice or BranchAdmin of that branch.
/// </summary>
public interface IGetBranchStaffExecutor
{
    Task<IReadOnlyList<StaffResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid branchId,
        CancellationToken cancellationToken = default);
}
