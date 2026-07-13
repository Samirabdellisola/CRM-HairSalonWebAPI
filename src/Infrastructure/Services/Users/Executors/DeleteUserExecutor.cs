using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Users.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Users.Executors;

public class DeleteUserExecutor : IDeleteUserExecutor
{
    private readonly AppDbContext _dbContext;
    private readonly IBranchScopeChecker _branchScopeChecker;

    public DeleteUserExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
    {
        _dbContext = dbContext;
        _branchScopeChecker = branchScopeChecker;
    }

    public async Task<GenericResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid targetUserId,
        CancellationToken cancellationToken = default)
    {
        var targetUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == targetUserId, cancellationToken);
        if (targetUser is null)
        {
            throw new AppException("User not found.", AppErrorType.NotFound);
        }

        if (callerRole == UserRole.BranchAdmin)
        {
            if (targetUser.Role != UserRole.Staff && targetUser.Role != UserRole.Customer)
            {
                throw new AppException("BranchAdmin can only delete Staff or Customer users.", AppErrorType.Forbidden);
            }

            var administeredBranch = await _branchScopeChecker.GetAdministeredBranchAsync(callerId, cancellationToken);
            if (administeredBranch is null || targetUser.BranchId != administeredBranch.Id)
            {
                throw new AppException("BranchAdmin can only manage users within their own branch.", AppErrorType.Forbidden);
            }
        }

        _dbContext.Users.Remove(targetUser);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return GenericResponse.Ok("User deleted successfully.");
    }
}
