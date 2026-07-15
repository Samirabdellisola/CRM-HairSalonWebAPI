using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Application.Expenses.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Expenses.Executors;

public class GetExpenseCategoriesForExpensesExecutor : ExpenseExecutorBase, IGetExpenseCategoriesForExpensesExecutor
{
    public GetExpenseCategoriesForExpensesExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<IReadOnlyList<ExpenseCategoryResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CancellationToken cancellationToken = default)
    {
        var scopedBranchId = await ResolveScopedBranchIdAsync(callerId, callerRole, cancellationToken);
        if (scopedBranchId == Guid.Empty)
        {
            return Array.Empty<ExpenseCategoryResponse>();
        }

        var query = DbContext.ExpenseCategories.AsQueryable();
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
