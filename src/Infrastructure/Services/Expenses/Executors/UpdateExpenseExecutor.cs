using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Application.Expenses.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Expenses.Executors;

public class UpdateExpenseExecutor : ExpenseExecutorBase, IUpdateExpenseExecutor
{
    public UpdateExpenseExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<ExpenseResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid expenseId,
        UpdateExpenseRequest request,
        CancellationToken cancellationToken = default)
    {
        var expense = await ExpensesWithCategory()
            .FirstOrDefaultAsync(e => e.Id == expenseId, cancellationToken);
        if (expense is null)
        {
            throw new AppException("Expense not found.", AppErrorType.NotFound);
        }

        await EnsureCanAccessBranchAsync(callerId, callerRole, expense.ExpenseCategory.BranchId, cancellationToken);

        var category = await DbContext.ExpenseCategories
            .FirstOrDefaultAsync(c => c.Id == request.ExpenseCategoryId, cancellationToken);
        if (category is null)
        {
            throw new AppException("Expense category not found.", AppErrorType.NotFound);
        }

        if (category.BranchId != expense.ExpenseCategory.BranchId)
        {
            throw new AppException("Expense category must belong to the same branch.", AppErrorType.Validation);
        }

        await EnsureCanAccessBranchAsync(callerId, callerRole, category.BranchId, cancellationToken);

        expense.ExpenseCategoryId = category.Id;
        expense.ExpenseCategory = category;
        expense.Name = request.Name.Trim();
        expense.Price = request.Price;
        expense.Date = request.Date.ToUniversalTime();

        await DbContext.SaveChangesAsync(cancellationToken);

        return ToExpenseResponse(expense);
    }
}
