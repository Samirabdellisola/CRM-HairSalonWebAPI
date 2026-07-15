using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Domain.Entities;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Expenses;

/// <summary>
/// Shared helpers for expense and expense-category executors.
/// </summary>
public abstract class ExpenseExecutorBase
{
    protected readonly AppDbContext DbContext;
    protected readonly IBranchScopeChecker BranchScopeChecker;

    protected ExpenseExecutorBase(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
    {
        DbContext = dbContext;
        BranchScopeChecker = branchScopeChecker;
    }

    protected static ExpenseCategoryResponse ToCategoryResponse(ExpenseCategory category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        BranchId = category.BranchId,
        CreatedAt = category.CreatedAt,
        UpdatedAt = category.UpdatedAt
    };

    protected static ExpenseResponse ToExpenseResponse(Expense expense) => new()
    {
        Id = expense.Id,
        ExpenseCategoryId = expense.ExpenseCategoryId,
        Name = expense.Name,
        Price = expense.Price,
        Date = expense.Date,
        BranchId = expense.ExpenseCategory.BranchId,
        CreatedAt = expense.CreatedAt,
        UpdatedAt = expense.UpdatedAt
    };

    protected IQueryable<Expense> ExpensesWithCategory() =>
        DbContext.Expenses.Include(e => e.ExpenseCategory).AsQueryable();

    protected async Task EnsureCanAccessBranchAsync(
        Guid callerId,
        UserRole callerRole,
        Guid branchId,
        CancellationToken cancellationToken)
    {
        if (callerRole == UserRole.CentralOffice)
        {
            return;
        }

        if (callerRole != UserRole.BranchAdmin)
        {
            throw new AppException("You are not allowed to access expenses for this branch.", AppErrorType.Forbidden);
        }

        var administeredBranch = await BranchScopeChecker.GetAdministeredBranchAsync(callerId, cancellationToken);
        if (administeredBranch is null || administeredBranch.Id != branchId)
        {
            throw new AppException("BranchAdmin can only access expenses for their own branch.", AppErrorType.Forbidden);
        }
    }

    protected async Task<Guid?> ResolveScopedBranchIdAsync(
        Guid callerId,
        UserRole callerRole,
        CancellationToken cancellationToken)
    {
        if (callerRole == UserRole.CentralOffice)
        {
            return null;
        }

        if (callerRole == UserRole.BranchAdmin)
        {
            var administeredBranch = await BranchScopeChecker.GetAdministeredBranchAsync(callerId, cancellationToken);
            return administeredBranch?.Id;
        }

        return Guid.Empty;
    }
}
