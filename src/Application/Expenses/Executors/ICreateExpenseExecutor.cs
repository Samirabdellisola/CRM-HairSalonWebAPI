using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Expenses.Executors;

public interface ICreateExpenseExecutor
{
    Task<ExpenseResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CreateExpenseRequest request,
        CancellationToken cancellationToken = default);
}
