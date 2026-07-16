using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Branches.DTOs;
using SalonCRM.Application.Branches.Executors;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Domain.Entities;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Branches.Executors;

public class CreateBranchExecutor : BranchExecutorBase, ICreateBranchExecutor
{
    private readonly PasswordHasher<User> _passwordHasher;

    public CreateBranchExecutor(
        AppDbContext dbContext,
        IBranchScopeChecker branchScopeChecker,
        PasswordHasher<User> passwordHasher)
        : base(dbContext, branchScopeChecker)
    {
        _passwordHasher = passwordHasher;
    }

    public async Task<BranchResponse> ExecuteAsync(CreateBranchRequest request, CancellationToken cancellationToken = default)
    {
        var name = request.Name.Trim();
        var nameTaken = await DbContext.Branches.AnyAsync(b => b.Name == name, cancellationToken);
        if (nameTaken)
        {
            throw new AppException("A branch with this name already exists.", AppErrorType.Conflict);
        }

        var email = request.AdminEmail.Trim().ToLowerInvariant();
        var emailTaken = await DbContext.Users.AnyAsync(u => u.Email == email, cancellationToken);
        if (emailTaken)
        {
            throw new AppException("Email is already registered.", AppErrorType.Conflict);
        }

        var branch = new Branch
        {
            Name = name,
            Address = request.Address.Trim(),
            Phone = request.Phone.Trim(),
            IsActive = true,
            IsFrozen = false
        };

        var admin = new User
        {
            Email = email,
            Name = request.AdminName.Trim(),
            Role = UserRole.BranchAdmin,
            IsActive = true
        };
        admin.PasswordHash = _passwordHasher.HashPassword(admin, request.AdminPassword);

        DbContext.Branches.Add(branch);
        DbContext.Users.Add(admin);
        await DbContext.SaveChangesAsync(cancellationToken);

        admin.BranchId = branch.Id;
        branch.AdminId = admin.Id;
        branch.AdminUser = admin;
        await DbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(branch);
    }
}
