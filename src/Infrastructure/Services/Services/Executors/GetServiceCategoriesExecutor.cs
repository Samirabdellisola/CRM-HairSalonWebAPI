using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Services.DTOs;
using SalonCRM.Application.Services.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Services.Executors;

public class GetServiceCategoriesExecutor : ServiceExecutorBase, IGetServiceCategoriesExecutor
{
    public GetServiceCategoriesExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<IReadOnlyList<ServiceCategoryResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CancellationToken cancellationToken = default)
    {
        var scopedBranchId = await ResolveScopedBranchIdAsync(callerId, callerRole, cancellationToken);
        if (scopedBranchId == Guid.Empty)
        {
            return Array.Empty<ServiceCategoryResponse>();
        }

        var query = DbContext.ServiceCategories.AsQueryable();
        if (scopedBranchId.HasValue)
        {
            query = query.Where(c => c.BranchId == scopedBranchId.Value);
        }

        var categories = await query
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        return categories.Select(ToCategoryResponse).ToList();
    }
}
