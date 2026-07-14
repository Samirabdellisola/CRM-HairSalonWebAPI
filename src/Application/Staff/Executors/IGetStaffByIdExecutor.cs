using SalonCRM.Application.Staff.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Staff.Executors;

/// <summary>
/// Returns a Staff user by id. CentralOffice, BranchAdmin of that staff's branch, or the staff themselves.
/// </summary>
public interface IGetStaffByIdExecutor
{
    Task<StaffResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid staffId,
        CancellationToken cancellationToken = default);
}
