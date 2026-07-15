using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Application.Expenses.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Expenses.Executors;

public class GetExpensesExecutor : ExpenseExecutorBase, IGetExpensesExecutor
{
    public GetExpensesExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<IReadOnlyList<ExpenseResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CancellationToken cancellationToken = default)
    {
        var scopedBranchId = await ResolveScopedBranchIdAsync(callerId, callerRole, cancellationToken);
        if (scopedBranchId == Guid.Empty)
        {
            return Array.Empty<ExpenseResponse>();
        }

        var query = ExpensesWithCategory();
        if (scopedBranchId.HasValue)
        {
            query = query.Where(e => e.ExpenseCategory.BranchId == scopedBranchId.Value);
        }

        var expenses = await query
            .OrderByDescending(e => e.Date)
            .ToListAsync(cancellationToken);

        return expenses.Select(ToExpenseResponse).ToList();
    }
}
