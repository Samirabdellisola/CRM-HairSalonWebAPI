using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Application.Expenses.Executors;
using SalonCRM.Domain.Entities;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Expenses.Executors;

public class CreateExpenseExecutor : ExpenseExecutorBase, ICreateExpenseExecutor
{
    public CreateExpenseExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<ExpenseResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CreateExpenseRequest request,
        CancellationToken cancellationToken = default)
    {
        var category = await DbContext.ExpenseCategories
            .FirstOrDefaultAsync(c => c.Id == request.ExpenseCategoryId, cancellationToken);
        if (category is null)
        {
            throw new AppException("Expense category not found.", AppErrorType.NotFound);
        }

        await EnsureCanAccessBranchAsync(callerId, callerRole, category.BranchId, cancellationToken);

        var expense = new Expense
        {
            ExpenseCategoryId = category.Id,
            Name = request.Name.Trim(),
            Price = request.Price,
            Date = request.Date?.ToUniversalTime() ?? DateTime.UtcNow,
            ExpenseCategory = category
        };

        DbContext.Expenses.Add(expense);
        await DbContext.SaveChangesAsync(cancellationToken);

        return ToExpenseResponse(expense);
    }
}
