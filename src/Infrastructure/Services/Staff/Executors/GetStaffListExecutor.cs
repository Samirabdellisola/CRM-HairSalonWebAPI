using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Staff.DTOs;
using SalonCRM.Application.Staff.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Staff.Executors;

public class GetStaffListExecutor : StaffExecutorBase, IGetStaffListExecutor
{
    public GetStaffListExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<IReadOnlyList<StaffResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CancellationToken cancellationToken = default)
    {
        var query = DbContext.Users.Where(u => u.Role == UserRole.Staff);

        if (callerRole == UserRole.BranchAdmin)
        {
            var administeredBranch = await BranchScopeChecker.GetAdministeredBranchAsync(callerId, cancellationToken);
            if (administeredBranch is null)
            {
                return Array.Empty<StaffResponse>();
            }

            query = query.Where(u => u.BranchId == administeredBranch.Id);
        }

        var staff = await query
            .OrderBy(u => u.Email)
            .ToListAsync(cancellationToken);

        return staff.Select(ToResponse).ToList();
    }
}
