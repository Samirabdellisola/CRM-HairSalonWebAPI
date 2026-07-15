using SalonCRM.Application.Expenses.DTOs;

namespace SalonCRM.Application.Expenses.Executors;

public interface IUpdateExpenseCategoryExecutor
{
    Task<ExpenseCategoryResponse> ExecuteAsync(
        Guid categoryId,
        UpdateExpenseCategoryRequest request,
        CancellationToken cancellationToken = default);
}
