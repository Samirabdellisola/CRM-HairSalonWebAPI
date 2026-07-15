using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Services.DTOs;
using SalonCRM.Application.Services.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Services.Executors;

public class UpdateServiceExecutor : ServiceExecutorBase, IUpdateServiceExecutor
{
    public UpdateServiceExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<ServiceResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid serviceId,
        UpdateServiceRequest request,
        CancellationToken cancellationToken = default)
    {
        var service = await DbContext.Services.FirstOrDefaultAsync(s => s.Id == serviceId, cancellationToken);
        if (service is null)
        {
            throw new AppException("Service not found.", AppErrorType.NotFound);
        }

        await EnsureCanManageBranchAsync(callerId, callerRole, service.BranchId, cancellationToken);

        var name = request.Name.Trim();
        var nameTaken = await DbContext.Services.AnyAsync(
            s => s.BranchId == service.BranchId && s.Name == name && s.Id != serviceId,
            cancellationToken);
        if (nameTaken)
        {
            throw new AppException("A service with this name already exists for this branch.", AppErrorType.Conflict);
        }

        service.Name = name;
        service.Price = request.Price;
        service.ImagePath = string.IsNullOrWhiteSpace(request.ImagePath) ? null : request.ImagePath.Trim();

        await DbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(service);
    }
}
