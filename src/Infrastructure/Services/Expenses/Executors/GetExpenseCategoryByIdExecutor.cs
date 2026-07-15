using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Application.Expenses.Executors;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Expenses.Executors;

public class GetExpenseCategoryByIdExecutor : ExpenseExecutorBase, IGetExpenseCategoryByIdExecutor
{
    public GetExpenseCategoryByIdExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<ExpenseCategoryResponse> ExecuteAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var category = await DbContext.ExpenseCategories.FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);
        if (category is null)
        {
            throw new AppException("Expense category not found.", AppErrorType.NotFound);
        }

        return ToCategoryResponse(category);
    }
}
