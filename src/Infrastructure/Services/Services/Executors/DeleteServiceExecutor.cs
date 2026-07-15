using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.DTOs;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Services.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Services.Executors;

public class DeleteServiceExecutor : ServiceExecutorBase, IDeleteServiceExecutor
{
    public DeleteServiceExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<GenericResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid serviceId,
        CancellationToken cancellationToken = default)
    {
        var service = await DbContext.Services.FirstOrDefaultAsync(s => s.Id == serviceId, cancellationToken);
        if (service is null)
        {
            throw new AppException("Service not found.", AppErrorType.NotFound);
        }

        await EnsureCanManageBranchAsync(callerId, callerRole, service.BranchId, cancellationToken);

        DbContext.Services.Remove(service);
        await DbContext.SaveChangesAsync(cancellationToken);

        return GenericResponse.Ok("Service deleted successfully.");
    }
}
