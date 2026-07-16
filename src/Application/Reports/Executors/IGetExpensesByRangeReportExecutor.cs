using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Expenses.DTOs;
using SalonCRM.Application.Reports.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Reports.Executors;

public interface IGetExpensesByRangeReportExecutor
{
    Task<PagedResponse<PeriodGroupResponse<ExpenseResponse>>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        ReportRangeQuery query,
        CancellationToken cancellationToken = default);
}
