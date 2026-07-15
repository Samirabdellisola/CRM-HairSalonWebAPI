using SalonCRM.Application.Expenses.DTOs;

namespace SalonCRM.Application.Expenses.Executors;

public interface IGetExpenseCategoryByIdExecutor
{
    Task<ExpenseCategoryResponse> ExecuteAsync(Guid categoryId, CancellationToken cancellationToken = default);
}
