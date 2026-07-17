using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Orders.DTOs;
using SalonCRM.Application.Orders.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Orders.Executors;

public class GetStaffOrdersExecutor : OrderExecutorBase, IGetStaffOrdersExecutor
{
    public GetStaffOrdersExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<IReadOnlyList<OrderResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid staffId,
        CancellationToken cancellationToken = default)
    {
        var staff = await DbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == staffId, cancellationToken);
        if (staff is null || staff.Role != UserRole.Staff)
        {
            throw new AppException("Staff not found.", AppErrorType.NotFound);
        }

        if (callerRole == UserRole.Staff)
        {
            if (callerId != staffId)
            {
                throw new AppException("Staff can only view their own orders.", AppErrorType.Forbidden);
            }
        }
        else if (callerRole == UserRole.BranchAdmin)
        {
            if (!staff.BranchId.HasValue)
            {
                throw new AppException("BranchAdmin can only view staff within their own branch.", AppErrorType.Forbidden);
            }

            await EnsureCanManageBranchAsync(callerId, callerRole, staff.BranchId.Value, cancellationToken);
        }
        else if (callerRole != UserRole.CentralOffice)
        {
            throw new AppException("You are not allowed to list staff orders.", AppErrorType.Forbidden);
        }

        var orders = await DbContext.Orders
            .Where(o => o.StaffId == staffId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

        return orders.Select(ToResponse).ToList();
    }
}
