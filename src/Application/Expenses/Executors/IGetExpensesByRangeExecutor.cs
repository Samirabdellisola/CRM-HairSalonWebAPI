using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Expenses.Executors;

public interface IGetExpensesByRangeExecutor
{
    Task<IReadOnlyList<ExpenseResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);
}
