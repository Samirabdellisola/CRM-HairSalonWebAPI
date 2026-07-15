using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Payments.DTOs;
using SalonCRM.Application.Payments.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Payments.Executors;

public class GetPaymentsExecutor : PaymentExecutorBase, IGetPaymentsExecutor
{
    public GetPaymentsExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<IReadOnlyList<PaymentResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CancellationToken cancellationToken = default)
    {
        var query = DbContext.Payments.AsQueryable();

        if (callerRole == UserRole.BranchAdmin)
        {
            var administeredBranch = await BranchScopeChecker.GetAdministeredBranchAsync(callerId, cancellationToken);
            if (administeredBranch is null)
            {
                return Array.Empty<PaymentResponse>();
            }

            query = query.Where(p => p.BranchId == administeredBranch.Id);
        }
        else if (callerRole == UserRole.Staff)
        {
            query = query.Where(p => p.StaffId == callerId);
        }
        else if (callerRole == UserRole.Customer)
        {
            query = query.Where(p => p.CustomerId == callerId);
        }
        else if (callerRole != UserRole.CentralOffice)
        {
            return Array.Empty<PaymentResponse>();
        }

        var payments = await query
            .OrderByDescending(p => p.Date)
            .ToListAsync(cancellationToken);

        return payments.Select(ToResponse).ToList();
    }
}
