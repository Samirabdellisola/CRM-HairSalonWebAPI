using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Expenses.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Expenses.Executors;

public class DeleteExpenseCategoryExecutor : ExpenseExecutorBase, IDeleteExpenseCategoryExecutor
{
    public DeleteExpenseCategoryExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<GenericResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        var category = await DbContext.ExpenseCategories.FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);
        if (category is null)
        {
            throw new AppException("Expense category not found.", AppErrorType.NotFound);
        }

        await EnsureCanAccessBranchAsync(callerId, callerRole, category.BranchId, cancellationToken);

        var hasExpenses = await DbContext.Expenses.AnyAsync(e => e.ExpenseCategoryId == categoryId, cancellationToken);
        if (hasExpenses)
        {
            throw new AppException("Cannot delete an expense category that still has expenses.", AppErrorType.Conflict);
        }

        DbContext.ExpenseCategories.Remove(category);
        await DbContext.SaveChangesAsync(cancellationToken);

        return GenericResponse.Ok("Expense category deleted successfully.");
    }
}
