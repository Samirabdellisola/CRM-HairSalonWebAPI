using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Reports.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Reports.Executors;

public interface IGetTurnoverByRangeExecutor
{
    Task<PagedResponse<PeriodGroupResponse<PaymentReportItem>>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        ReportRangeQuery query,
        CancellationToken cancellationToken = default);
}
