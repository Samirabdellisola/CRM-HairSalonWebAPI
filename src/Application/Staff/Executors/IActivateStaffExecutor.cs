using SalonCRM.Application.Staff.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Staff.Executors;

/// <summary>
/// Activates a Staff user (IsActive = true). CentralOffice or BranchAdmin of their branch.
/// </summary>
public interface IActivateStaffExecutor
{
    Task<StaffResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid staffId,
        CancellationToken cancellationToken = default);
}
