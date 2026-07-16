using SalonCRM.Application.Branches.DTOs;

namespace SalonCRM.Application.Branches.Executors;

/// <summary>
/// Lists all branches. Public (no authentication required).
/// </summary>
public interface IGetBranchesExecutor
{
    Task<IReadOnlyList<BranchResponse>> ExecuteAsync(CancellationToken cancellationToken = default);
}
