using SalonCRM.Application.Staff.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Staff.Executors;

/// <summary>
/// Deactivates a Staff user (IsActive = false). CentralOffice or BranchAdmin of their branch.
/// </summary>
public interface IDeactivateStaffExecutor
{
    Task<StaffResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid staffId,
        CancellationToken cancellationToken = default);
}
