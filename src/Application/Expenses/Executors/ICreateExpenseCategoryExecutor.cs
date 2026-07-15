using SalonCRM.Application.Expenses.DTOs;

namespace SalonCRM.Application.Expenses.Executors;

public interface ICreateExpenseCategoryExecutor
{
    Task<ExpenseCategoryResponse> ExecuteAsync(CreateExpenseCategoryRequest request, CancellationToken cancellationToken = default);
}
