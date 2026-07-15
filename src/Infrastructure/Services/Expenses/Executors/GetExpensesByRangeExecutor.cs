using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Application.Expenses.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Expenses.Executors;

public class GetExpensesByRangeExecutor : ExpenseExecutorBase, IGetExpensesByRangeExecutor
{
    public GetExpensesByRangeExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<IReadOnlyList<ExpenseResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var fromUtc = from.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(from, DateTimeKind.Utc)
            : from.ToUniversalTime();
        var toUtc = to.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(to, DateTimeKind.Utc)
            : to.ToUniversalTime();

        if (fromUtc > toUtc)
        {
            throw new AppException("'from' must be less than or equal to 'to'.", AppErrorType.Validation);
        }

        var scopedBranchId = await ResolveScopedBranchIdAsync(callerId, callerRole, cancellationToken);
        if (scopedBranchId == Guid.Empty)
        {
            return Array.Empty<ExpenseResponse>();
        }

        var query = ExpensesWithCategory()
            .Where(e => e.Date >= fromUtc && e.Date <= toUtc);

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
