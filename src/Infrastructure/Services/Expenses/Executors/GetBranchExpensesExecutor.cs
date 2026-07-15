using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Application.Expenses.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Expenses.Executors;

public class GetBranchExpensesExecutor : ExpenseExecutorBase, IGetBranchExpensesExecutor
{
    public GetBranchExpensesExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<IReadOnlyList<ExpenseResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid branchId,
        CancellationToken cancellationToken = default)
    {
        var branchExists = await DbContext.Branches.AnyAsync(b => b.Id == branchId, cancellationToken);
        if (!branchExists)
        {
            throw new AppException("Branch not found.", AppErrorType.NotFound);
        }

        await EnsureCanAccessBranchAsync(callerId, callerRole, branchId, cancellationToken);

        var expenses = await ExpensesWithCategory()
            .Where(e => e.ExpenseCategory.BranchId == branchId)
            .OrderByDescending(e => e.Date)
            .ToListAsync(cancellationToken);

        return expenses.Select(ToExpenseResponse).ToList();
    }
}
