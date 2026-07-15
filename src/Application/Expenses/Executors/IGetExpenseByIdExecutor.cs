using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Expenses.Executors;

public interface IGetExpenseByIdExecutor
{
    Task<ExpenseResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid expenseId,
        CancellationToken cancellationToken = default);
}
