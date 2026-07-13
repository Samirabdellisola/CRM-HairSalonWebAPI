using SalonCRM.Domain.Entities;

namespace SalonCRM.Infrastructure.Services.Common;

/// <summary>
/// Resolves which branch a BranchAdmin user administers, used to enforce
/// own-branch-only authorization across Auth and Users executors.
/// </summary>
public interface IBranchScopeChecker
{
    Task<Branch?> GetAdministeredBranchAsync(Guid branchAdminUserId, CancellationToken cancellationToken = default);
}
