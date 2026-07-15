using SalonCRM.Application.Common.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Expenses.Executors;

public interface IDeleteExpenseExecutor
{
    Task<GenericResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid expenseId,
        CancellationToken cancellationToken = default);
}
