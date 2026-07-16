using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Services.DTOs;
using SalonCRM.Application.Services.Executors;
using SalonCRM.Domain.Entities;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Services.Executors;

public class CreateServiceCategoryExecutor : ServiceExecutorBase, ICreateServiceCategoryExecutor
{
    public CreateServiceCategoryExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<ServiceCategoryResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CreateServiceCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureCanManageBranchAsync(callerId, callerRole, request.BranchId, cancellationToken);

        var branchExists = await DbContext.Branches.AnyAsync(b => b.Id == request.BranchId, cancellationToken);
        if (!branchExists)
        {
            throw new AppException("Branch not found.", AppErrorType.NotFound);
        }

        var name = request.Name.Trim();
        var nameTaken = await DbContext.ServiceCategories.AnyAsync(
            c => c.BranchId == request.BranchId && c.Name == name,
            cancellationToken);
        if (nameTaken)
        {
            throw new AppException("A service category with this name already exists for this branch.", AppErrorType.Conflict);
        }

        var category = new ServiceCategory
        {
            Name = name,
            BranchId = request.BranchId
        };

        DbContext.ServiceCategories.Add(category);
        await DbContext.SaveChangesAsync(cancellationToken);

        return ToCategoryResponse(category);
    }
}
