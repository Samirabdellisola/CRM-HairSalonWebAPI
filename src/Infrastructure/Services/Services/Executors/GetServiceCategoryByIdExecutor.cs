using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Services.DTOs;
using SalonCRM.Application.Services.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Services.Executors;

public class GetServiceCategoryByIdExecutor : ServiceExecutorBase, IGetServiceCategoryByIdExecutor
{
    public GetServiceCategoryByIdExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<ServiceCategoryResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        var category = await DbContext.ServiceCategories.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);
        if (category is null)
        {
            throw new AppException("Service category not found.", AppErrorType.NotFound);
        }

        await EnsureCanViewServiceAsync(callerId, callerRole, category.BranchId, cancellationToken);

        return ToCategoryResponse(category);
    }
}
