using SalonCRM.Application.Expenses.DTOs;

namespace SalonCRM.Application.Expenses.Executors;

public interface IGetExpenseCategoriesExecutor
{
    Task<IReadOnlyList<ExpenseCategoryResponse>> ExecuteAsync(CancellationToken cancellationToken = default);
}
