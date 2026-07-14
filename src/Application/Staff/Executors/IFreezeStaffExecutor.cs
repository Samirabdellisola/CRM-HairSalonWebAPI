using SalonCRM.Application.Staff.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Staff.Executors;

/// <summary>
/// Freezes a Staff user (IsFrozen = true, e.g. contract ended).
/// CentralOffice or BranchAdmin of their branch.
/// </summary>
public interface IFreezeStaffExecutor
{
    Task<StaffResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid staffId,
        CancellationToken cancellationToken = default);
}
