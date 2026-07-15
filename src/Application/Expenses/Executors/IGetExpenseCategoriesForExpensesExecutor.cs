using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Expenses.Executors;

public interface IGetExpenseCategoriesForExpensesExecutor
{
    Task<IReadOnlyList<ExpenseCategoryResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CancellationToken cancellationToken = default);
}
