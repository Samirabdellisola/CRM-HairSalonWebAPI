using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Expenses.Executors;

public interface IGetExpenseCategoryByIdExecutor
{
    Task<ExpenseCategoryResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid categoryId,
        CancellationToken cancellationToken = default);
}
