using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Application.Expenses.Executors;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Expenses.Executors;

public class GetExpenseCategoriesExecutor : ExpenseExecutorBase, IGetExpenseCategoriesExecutor
{
    public GetExpenseCategoriesExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<IReadOnlyList<ExpenseCategoryResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var categories = await DbContext.ExpenseCategories
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        return categories.Select(ToCategoryResponse).ToList();
    }
}
