using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Expenses.Executors;

public interface ICreateExpenseCategoryExecutor
{
    Task<ExpenseCategoryResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CreateExpenseCategoryRequest request,
        CancellationToken cancellationToken = default);
}
