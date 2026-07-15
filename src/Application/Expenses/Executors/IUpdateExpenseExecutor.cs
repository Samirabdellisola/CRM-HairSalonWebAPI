using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Expenses.Executors;

public interface IUpdateExpenseExecutor
{
    Task<ExpenseResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid expenseId,
        UpdateExpenseRequest request,
        CancellationToken cancellationToken = default);
}
