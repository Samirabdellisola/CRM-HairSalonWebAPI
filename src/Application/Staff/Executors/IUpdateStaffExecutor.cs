using SalonCRM.Application.Staff.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Staff.Executors;

/// <summary>
/// Updates Email/Phone/Address on a Staff user. Allowed for the staff themselves,
/// the BranchAdmin of their branch, or CentralOffice. Does not change password or role.
/// </summary>
public interface IUpdateStaffExecutor
{
    Task<StaffResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid staffId,
        UpdateStaffRequest request,
        CancellationToken cancellationToken = default);
}
