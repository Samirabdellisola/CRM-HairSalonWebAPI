using SalonCRM.Application.Branches.DTOs;

namespace SalonCRM.Application.Branches.Executors;

/// <summary>
/// Creates a branch and its initial BranchAdmin user. CentralOffice only.
/// </summary>
public interface ICreateBranchExecutor
{
    Task<BranchResponse> ExecuteAsync(CreateBranchRequest request, CancellationToken cancellationToken = default);
}
