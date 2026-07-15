using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Expenses.Executors;

public interface IGetExpensesExecutor
{
    Task<IReadOnlyList<ExpenseResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CancellationToken cancellationToken = default);
}
