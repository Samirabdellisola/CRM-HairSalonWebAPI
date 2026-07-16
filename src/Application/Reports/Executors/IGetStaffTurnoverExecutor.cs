using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Reports.DTOs;
using SalonCRM.Domain.Enums;

namespace SalonCRM.Application.Reports.Executors;

public interface IGetStaffTurnoverExecutor
{
    Task<PagedResponse<StaffGroupResponse<PaymentReportItem>>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        ReportRangeQuery query,
        CancellationToken cancellationToken = default);
}
