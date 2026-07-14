using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Staff.DTOs;
using SalonCRM.Application.Staff.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Staff.Executors;

public class GetBranchStaffExecutor : StaffExecutorBase, IGetBranchStaffExecutor
{
    public GetBranchStaffExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<IReadOnlyList<StaffResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid branchId,
        CancellationToken cancellationToken = default)
    {
        var branchExists = await DbContext.Branches.AnyAsync(b => b.Id == branchId, cancellationToken);
        if (!branchExists)
        {
            throw new AppException("Branch not found.", AppErrorType.NotFound);
        }

        await EnsureCanManageStaffAsync(callerId, callerRole, branchId, cancellationToken);

        var staff = await DbContext.Users
            .Where(u => u.Role == UserRole.Staff && u.BranchId == branchId)
            .OrderBy(u => u.Email)
            .ToListAsync(cancellationToken);

        return staff.Select(ToResponse).ToList();
    }
}
