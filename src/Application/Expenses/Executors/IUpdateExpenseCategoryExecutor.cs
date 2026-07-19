using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Expenses.Executors;

public interface IUpdateExpenseCategoryExecutor
{
    Task<ExpenseCategoryResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid categoryId,
        UpdateExpenseCategoryRequest request,
        CancellationToken cancellationToken = default);
}
