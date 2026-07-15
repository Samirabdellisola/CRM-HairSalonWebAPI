using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Application.Expenses.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Expenses.Executors;

public class GetExpenseByIdExecutor : ExpenseExecutorBase, IGetExpenseByIdExecutor
{
    public GetExpenseByIdExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<ExpenseResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid expenseId,
        CancellationToken cancellationToken = default)
    {
        var expense = await ExpensesWithCategory()
            .FirstOrDefaultAsync(e => e.Id == expenseId, cancellationToken);
        if (expense is null)
        {
            throw new AppException("Expense not found.", AppErrorType.NotFound);
        }

        await EnsureCanAccessBranchAsync(callerId, callerRole, expense.ExpenseCategory.BranchId, cancellationToken);
        return ToExpenseResponse(expense);
    }
}
