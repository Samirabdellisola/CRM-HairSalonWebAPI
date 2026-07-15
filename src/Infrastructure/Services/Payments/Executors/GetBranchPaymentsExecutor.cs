using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Payments.DTOs;
using SalonCRM.Application.Payments.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Payments.Executors;

public class GetBranchPaymentsExecutor : PaymentExecutorBase, IGetBranchPaymentsExecutor
{
    public GetBranchPaymentsExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<IReadOnlyList<PaymentResponse>> ExecuteAsync(
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

        if (callerRole != UserRole.CentralOffice && callerRole != UserRole.BranchAdmin)
        {
            throw new AppException("You are not allowed to list branch payments.", AppErrorType.Forbidden);
        }

        await EnsureCanManageBranchAsync(callerId, callerRole, branchId, cancellationToken);

        var payments = await DbContext.Payments
            .Where(p => p.BranchId == branchId)
            .OrderByDescending(p => p.Date)
            .ToListAsync(cancellationToken);

        return payments.Select(ToResponse).ToList();
    }
}
