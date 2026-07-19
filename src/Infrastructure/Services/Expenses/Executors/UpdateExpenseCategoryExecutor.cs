using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Application.Expenses.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Expenses.Executors;

public class UpdateExpenseCategoryExecutor : ExpenseExecutorBase, IUpdateExpenseCategoryExecutor
{
    public UpdateExpenseCategoryExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<ExpenseCategoryResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid categoryId,
        UpdateExpenseCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var category = await DbContext.ExpenseCategories.FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);
        if (category is null)
        {
            throw new AppException("Expense category not found.", AppErrorType.NotFound);
        }

        await EnsureCanAccessBranchAsync(callerId, callerRole, category.BranchId, cancellationToken);

        var name = request.Name.Trim();
        var nameTaken = await DbContext.ExpenseCategories.AnyAsync(
            c => c.BranchId == category.BranchId && c.Name == name && c.Id != categoryId,
            cancellationToken);
        if (nameTaken)
        {
            throw new AppException("An expense category with this name already exists for this branch.", AppErrorType.Conflict);
        }

        category.Name = name;
        await DbContext.SaveChangesAsync(cancellationToken);

        return ToCategoryResponse(category);
    }
}
