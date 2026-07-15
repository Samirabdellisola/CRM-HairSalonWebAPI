using SalonCRM.Application.Common.DTOs;

namespace SalonCRM.Application.Expenses.Executors;

public interface IDeleteExpenseCategoryExecutor
{
    Task<GenericResponse> ExecuteAsync(Guid categoryId, CancellationToken cancellationToken = default);
}
