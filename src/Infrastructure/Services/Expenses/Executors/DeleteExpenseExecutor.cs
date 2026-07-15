using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Expenses.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Expenses.Executors;

public class DeleteExpenseExecutor : ExpenseExecutorBase, IDeleteExpenseExecutor
{
    public DeleteExpenseExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<GenericResponse> ExecuteAsync(
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

        DbContext.Expenses.Remove(expense);
        await DbContext.SaveChangesAsync(cancellationToken);

        return GenericResponse.Ok("Expense deleted successfully.");
    }
}
