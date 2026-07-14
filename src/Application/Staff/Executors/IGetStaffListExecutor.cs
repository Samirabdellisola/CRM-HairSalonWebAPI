using SalonCRM.Application.Staff.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Staff.Executors;

/// <summary>
/// Lists Staff users. CentralOffice sees all; BranchAdmin sees only staff of their branch.
/// </summary>
public interface IGetStaffListExecutor
{
    Task<IReadOnlyList<StaffResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CancellationToken cancellationToken = default);
}
