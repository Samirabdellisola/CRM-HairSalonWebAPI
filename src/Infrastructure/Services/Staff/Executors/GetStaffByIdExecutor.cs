using SalonCRM.Application.Staff.DTOs;
using SalonCRM.Application.Staff.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Staff.Executors;

public class GetStaffByIdExecutor : StaffExecutorBase, IGetStaffByIdExecutor
{
    public GetStaffByIdExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<StaffResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid staffId,
        CancellationToken cancellationToken = default)
    {
        var staff = await GetStaffOrThrowAsync(staffId, cancellationToken);
        await EnsureCanViewOrEditStaffAsync(callerId, callerRole, staff, cancellationToken);
        return ToResponse(staff);
    }
}
