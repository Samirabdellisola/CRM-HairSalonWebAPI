using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Customers.DTOs;
using SalonCRM.Domain.Entities;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Customers;

/// <summary>
/// Shared helpers for customer executors: response mapping, lookup, and scope checks.
/// </summary>
public abstract class CustomerExecutorBase
{
    protected readonly AppDbContext DbContext;
    protected readonly IBranchScopeChecker BranchScopeChecker;

    protected CustomerExecutorBase(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
    {
        DbContext = dbContext;
        BranchScopeChecker = branchScopeChecker;
    }

    protected static CustomerResponse ToCustomerResponse(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        Name = user.Name,
        Role = user.Role,
        Phone = user.Phone,
        Address = user.Address,
        BranchId = user.BranchId,
        Birthday = user.Birthday,
        IsActive = user.IsActive,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt
    };

    protected static ProfileResponse ToProfileResponse(Profile profile) => new()
    {
        Id = profile.Id,
        UserId = profile.UserId,
        HairColor = profile.HairColor,
        PhotoPath = profile.PhotoPath,
        Preferences = profile.Preferences,
        CreatedAt = profile.CreatedAt,
        UpdatedAt = profile.UpdatedAt
    };

    protected async Task<User> GetCustomerOrThrowAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Id == customerId, cancellationToken);
        if (user is null || user.Role != UserRole.Customer)
        {
            throw new AppException("Customer not found.", AppErrorType.NotFound);
        }

        return user;
    }

    protected async Task EnsureCanManageCustomerAsync(
        Guid callerId,
        UserRole callerRole,
        Guid? customerBranchId,
        CancellationToken cancellationToken)
    {
        if (callerRole == UserRole.CentralOffice)
        {
            return;
        }

        if (callerRole != UserRole.BranchAdmin)
        {
            throw new AppException("You are not allowed to manage this customer.", AppErrorType.Forbidden);
        }

        if (!customerBranchId.HasValue)
        {
            throw new AppException("BranchAdmin can only manage customers within their own branch.", AppErrorType.Forbidden);
        }

        var administeredBranch = await BranchScopeChecker.GetAdministeredBranchAsync(callerId, cancellationToken);
        if (administeredBranch is null || administeredBranch.Id != customerBranchId.Value)
        {
            throw new AppException("BranchAdmin can only manage customers within their own branch.", AppErrorType.Forbidden);
        }
    }

    protected async Task EnsureCanViewOrEditCustomerAsync(
        Guid callerId,
        UserRole callerRole,
        User customer,
        CancellationToken cancellationToken)
    {
        if (callerRole == UserRole.CentralOffice)
        {
            return;
        }

        if (callerRole == UserRole.Customer && callerId == customer.Id)
        {
            return;
        }

        await EnsureCanManageCustomerAsync(callerId, callerRole, customer.BranchId, cancellationToken);
    }

    protected async Task<Profile> GetOrCreateProfileAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var profile = await DbContext.Profiles.FirstOrDefaultAsync(p => p.UserId == customerId, cancellationToken);
        if (profile is not null)
        {
            return profile;
        }

        profile = new Profile { UserId = customerId };
        DbContext.Profiles.Add(profile);
        await DbContext.SaveChangesAsync(cancellationToken);
        return profile;
    }
}
