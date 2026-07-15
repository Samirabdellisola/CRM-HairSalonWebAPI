using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Expenses.Executors;

public interface IGetBranchExpensesExecutor
{
    Task<IReadOnlyList<ExpenseResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid branchId,
        CancellationToken cancellationToken = default);
}
