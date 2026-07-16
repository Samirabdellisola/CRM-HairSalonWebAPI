using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Branches.DTOs;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Domain.Entities;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Branches;

/// <summary>
/// Shared helpers for branch executors: response mapping and BranchAdmin scope checks.
/// </summary>
public abstract class BranchExecutorBase
{
    protected readonly AppDbContext DbContext;
    protected readonly IBranchScopeChecker BranchScopeChecker;

    protected BranchExecutorBase(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
    {
        DbContext = dbContext;
        BranchScopeChecker = branchScopeChecker;
    }

    protected IQueryable<Branch> BranchesWithAdmin =>
        DbContext.Branches.Include(b => b.AdminUser);

    protected static BranchResponse ToResponse(Branch branch) => new()
    {
        Id = branch.Id,
        Name = branch.Name,
        Address = branch.Address,
        Phone = branch.Phone,
        IsActive = branch.IsActive,
        IsFrozen = branch.IsFrozen,
        AdminName = branch.AdminUser?.Name,
        CreatedAt = branch.CreatedAt,
        UpdatedAt = branch.UpdatedAt
    };

    /// <summary>
    /// Ensures a BranchAdmin caller administers the given branch. CentralOffice always passes.
    /// </summary>
    protected async Task EnsureCanManageBranchAsync(
        Guid callerId,
        UserRole callerRole,
        Guid branchId,
        CancellationToken cancellationToken)
    {
        if (callerRole == UserRole.CentralOffice)
        {
            return;
        }

        if (callerRole != UserRole.BranchAdmin)
        {
            throw new AppException("You are not allowed to manage this branch.", AppErrorType.Forbidden);
        }

        var administeredBranch = await BranchScopeChecker.GetAdministeredBranchAsync(callerId, cancellationToken);
        if (administeredBranch is null || administeredBranch.Id != branchId)
        {
            throw new AppException("BranchAdmin can only manage their own branch.", AppErrorType.Forbidden);
        }
    }

    protected async Task RevokeAllRefreshTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;
        var activeTokens = await DbContext.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.RevokedAt = utcNow;
        }
    }
}
