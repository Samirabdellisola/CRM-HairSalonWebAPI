using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Staff.DTOs;
using SalonCRM.Domain.Entities;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Staff;

/// <summary>
/// Shared helpers for staff executors: response mapping, staff lookup, and scope checks.
/// </summary>
public abstract class StaffExecutorBase
{
    protected readonly AppDbContext DbContext;
    protected readonly IBranchScopeChecker BranchScopeChecker;

    protected StaffExecutorBase(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
    {
        DbContext = dbContext;
        BranchScopeChecker = branchScopeChecker;
    }

    protected static StaffResponse ToResponse(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        Name = user.Name,
        Role = user.Role,
        Phone = user.Phone,
        Address = user.Address,
        BranchId = user.BranchId,
        IsActive = user.IsActive,
        IsFrozen = user.IsFrozen,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt
    };

    protected async Task<User> GetStaffOrThrowAsync(Guid staffId, CancellationToken cancellationToken)
    {
        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == staffId, cancellationToken);
        if (user is null || user.Role != UserRole.Staff)
        {
            throw new AppException("Staff not found.", AppErrorType.NotFound);
        }

        return user;
    }

    /// <summary>
    /// CentralOffice always passes. BranchAdmin must administer the staff's branch.
    /// </summary>
    protected async Task EnsureCanManageStaffAsync(
        Guid callerId,
        UserRole callerRole,
        Guid? staffBranchId,
        CancellationToken cancellationToken)
    {
        if (callerRole == UserRole.CentralOffice)
        {
            return;
        }

        if (callerRole != UserRole.BranchAdmin)
        {
            throw new AppException("You are not allowed to manage this staff member.", AppErrorType.Forbidden);
        }

        if (!staffBranchId.HasValue)
        {
            throw new AppException("BranchAdmin can only manage staff within their own branch.", AppErrorType.Forbidden);
        }

        var administeredBranch = await BranchScopeChecker.GetAdministeredBranchAsync(callerId, cancellationToken);
        if (administeredBranch is null || administeredBranch.Id != staffBranchId.Value)
        {
            throw new AppException("BranchAdmin can only manage staff within their own branch.", AppErrorType.Forbidden);
        }
    }

    /// <summary>
    /// Allows CentralOffice, BranchAdmin of the staff's branch, or the staff themselves.
    /// </summary>
    protected async Task EnsureCanViewOrEditStaffAsync(
        Guid callerId,
        UserRole callerRole,
        User staff,
        CancellationToken cancellationToken)
    {
        if (callerRole == UserRole.CentralOffice)
        {
            return;
        }

        if (callerRole == UserRole.Staff && callerId == staff.Id)
        {
            return;
        }

        await EnsureCanManageStaffAsync(callerId, callerRole, staff.BranchId, cancellationToken);
    }
}
