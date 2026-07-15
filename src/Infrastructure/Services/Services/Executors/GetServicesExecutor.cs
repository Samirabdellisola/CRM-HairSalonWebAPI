using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Services.DTOs;
using SalonCRM.Application.Services.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Services.Executors;

public class GetServicesExecutor : ServiceExecutorBase, IGetServicesExecutor
{
    public GetServicesExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<IReadOnlyList<ServiceResponse>> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CancellationToken cancellationToken = default)
    {
        var scopedBranchId = await ResolveScopedBranchIdAsync(callerId, callerRole, cancellationToken);
        if (scopedBranchId == Guid.Empty)
        {
            return Array.Empty<ServiceResponse>();
        }

        var query = DbContext.Services.AsQueryable();
        if (scopedBranchId.HasValue)
        {
            query = query.Where(s => s.BranchId == scopedBranchId.Value);
        }

        var services = await query
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);

        return services.Select(ToResponse).ToList();
    }
}
