using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Services.DTOs;
using SalonCRM.Domain.Entities;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;
using ServiceEntity = SalonCRM.Domain.Entities.Service;

namespace SalonCRM.Infrastructure.Services.Services;

/// <summary>
/// Shared helpers for salon service executors: mapping and branch scope checks.
/// </summary>
public abstract class ServiceExecutorBase
{
    protected readonly AppDbContext DbContext;
    protected readonly IBranchScopeChecker BranchScopeChecker;

    protected ServiceExecutorBase(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
    {
        DbContext = dbContext;
        BranchScopeChecker = branchScopeChecker;
    }

    protected static ServiceResponse ToResponse(ServiceEntity service) => new()
    {
        Id = service.Id,
        Name = service.Name,
        Price = service.Price,
        ImagePath = service.ImagePath,
        BranchId = service.BranchId,
        ServiceCategoryId = service.ServiceCategoryId,
        CreatedAt = service.CreatedAt,
        UpdatedAt = service.UpdatedAt
    };

    protected static ServiceCategoryResponse ToCategoryResponse(ServiceCategory category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        BranchId = category.BranchId,
        CreatedAt = category.CreatedAt,
        UpdatedAt = category.UpdatedAt
    };

    protected async Task EnsureServiceCategoryValidForBranchAsync(
        Guid? serviceCategoryId,
        Guid branchId,
        CancellationToken cancellationToken)
    {
        if (!serviceCategoryId.HasValue)
        {
            return;
        }

        var category = await DbContext.ServiceCategories.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == serviceCategoryId.Value, cancellationToken);
        if (category is null)
        {
            throw new AppException("Service category not found.", AppErrorType.NotFound);
        }

        if (category.BranchId != branchId)
        {
            throw new AppException("Service category must belong to the same branch as the service.", AppErrorType.Validation);
        }
    }

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
            throw new AppException("Only CentralOffice or the BranchAdmin of this branch can manage services.", AppErrorType.Forbidden);
        }

        var administeredBranch = await BranchScopeChecker.GetAdministeredBranchAsync(callerId, cancellationToken);
        if (administeredBranch is null || administeredBranch.Id != branchId)
        {
            throw new AppException("BranchAdmin can only manage services for their own branch.", AppErrorType.Forbidden);
        }
    }

    protected async Task EnsureCanViewServiceAsync(
        Guid callerId,
        UserRole callerRole,
        Guid serviceBranchId,
        CancellationToken cancellationToken)
    {
        if (callerRole == UserRole.CentralOffice)
        {
            return;
        }

        if (callerRole == UserRole.BranchAdmin)
        {
            await EnsureCanManageBranchAsync(callerId, callerRole, serviceBranchId, cancellationToken);
            return;
        }

        if (callerRole is UserRole.Staff or UserRole.Customer)
        {
            var caller = await DbContext.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == callerId, cancellationToken);
            if (caller?.BranchId is null || caller.BranchId != serviceBranchId)
            {
                throw new AppException("You can only view services for your own branch.", AppErrorType.Forbidden);
            }

            return;
        }

        throw new AppException("You are not allowed to view this service.", AppErrorType.Forbidden);
    }

    protected async Task<Guid?> ResolveScopedBranchIdAsync(
        Guid callerId,
        UserRole callerRole,
        CancellationToken cancellationToken)
    {
        if (callerRole == UserRole.CentralOffice)
        {
            return null;
        }

        if (callerRole == UserRole.BranchAdmin)
        {
            var administeredBranch = await BranchScopeChecker.GetAdministeredBranchAsync(callerId, cancellationToken);
            return administeredBranch?.Id;
        }

        if (callerRole is UserRole.Staff or UserRole.Customer)
        {
            var caller = await DbContext.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == callerId, cancellationToken);
            return caller?.BranchId;
        }

        return Guid.Empty;
    }
}
