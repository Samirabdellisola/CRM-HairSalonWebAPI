using SalonCRM.Application.Branches.DTOs;

namespace SalonCRM.Application.Branches.Executors;

/// <summary>
/// Returns a single branch by id. Public (no authentication required).
/// </summary>
public interface IGetBranchByIdExecutor
{
    Task<BranchResponse> ExecuteAsync(Guid branchId, CancellationToken cancellationToken = default);
}
