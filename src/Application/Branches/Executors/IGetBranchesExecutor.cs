using SalonCRM.Application.Branches.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Branches.Executors;

/// <summary>
/// Lists branches. CentralOffice sees all; BranchAdmin sees only the branch they administer.
/// </summary>
public interface IGetBranchesExecutor
{
    Task<IReadOnlyList<BranchResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CancellationToken cancellationToken = default);
}
