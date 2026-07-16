using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Services.DTOs;
using SalonCRM.Application.Services.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Services.Executors;

public class UpdateServiceCategoryExecutor : ServiceExecutorBase, IUpdateServiceCategoryExecutor
{
    public UpdateServiceCategoryExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<ServiceCategoryResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid categoryId,
        UpdateServiceCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var category = await DbContext.ServiceCategories.FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);
        if (category is null)
        {
            throw new AppException("Service category not found.", AppErrorType.NotFound);
        }

        await EnsureCanManageBranchAsync(callerId, callerRole, category.BranchId, cancellationToken);

        var name = request.Name.Trim();
        var nameTaken = await DbContext.ServiceCategories.AnyAsync(
            c => c.BranchId == category.BranchId && c.Name == name && c.Id != categoryId,
            cancellationToken);
        if (nameTaken)
        {
            throw new AppException("A service category with this name already exists for this branch.", AppErrorType.Conflict);
        }

        category.Name = name;
        await DbContext.SaveChangesAsync(cancellationToken);

        return ToCategoryResponse(category);
    }
}
