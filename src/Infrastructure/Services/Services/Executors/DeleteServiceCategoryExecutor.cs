using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Services.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Services.Executors;

public class DeleteServiceCategoryExecutor : ServiceExecutorBase, IDeleteServiceCategoryExecutor
{
    public DeleteServiceCategoryExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<GenericResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        var category = await DbContext.ServiceCategories.FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);
        if (category is null)
        {
            throw new AppException("Service category not found.", AppErrorType.NotFound);
        }

        await EnsureCanManageBranchAsync(callerId, callerRole, category.BranchId, cancellationToken);

        var hasServices = await DbContext.Services.AnyAsync(s => s.ServiceCategoryId == categoryId, cancellationToken);
        if (hasServices)
        {
            throw new AppException("Cannot delete a service category that still has services.", AppErrorType.Conflict);
        }

        DbContext.ServiceCategories.Remove(category);
        await DbContext.SaveChangesAsync(cancellationToken);

        return GenericResponse.Ok("Service category deleted successfully.");
    }
}
