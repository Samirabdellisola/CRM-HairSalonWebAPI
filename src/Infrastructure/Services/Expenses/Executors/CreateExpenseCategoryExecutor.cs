using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Application.Expenses.Executors;
using SalonCRM.Domain.Entities;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Expenses.Executors;

public class CreateExpenseCategoryExecutor : ExpenseExecutorBase, ICreateExpenseCategoryExecutor
{
    public CreateExpenseCategoryExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<ExpenseCategoryResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CreateExpenseCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureCanAccessBranchAsync(callerId, callerRole, request.BranchId, cancellationToken);

        var branchExists = await DbContext.Branches.AnyAsync(b => b.Id == request.BranchId, cancellationToken);
        if (!branchExists)
        {
            throw new AppException("Branch not found.", AppErrorType.NotFound);
        }

        var name = request.Name.Trim();
        var nameTaken = await DbContext.ExpenseCategories.AnyAsync(
            c => c.BranchId == request.BranchId && c.Name == name,
            cancellationToken);
        if (nameTaken)
        {
            throw new AppException("An expense category with this name already exists for this branch.", AppErrorType.Conflict);
        }

        var category = new ExpenseCategory
        {
            Name = name,
            BranchId = request.BranchId
        };

        DbContext.ExpenseCategories.Add(category);
        await DbContext.SaveChangesAsync(cancellationToken);

        return ToCategoryResponse(category);
    }
}
