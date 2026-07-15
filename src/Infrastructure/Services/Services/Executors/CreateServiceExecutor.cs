using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Services.DTOs;
using SalonCRM.Application.Services.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;
using ServiceEntity = SalonCRM.Domain.Entities.Service;

namespace SalonCRM.Infrastructure.Services.Services.Executors;

public class CreateServiceExecutor : ServiceExecutorBase, ICreateServiceExecutor
{
    public CreateServiceExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<ServiceResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        CreateServiceRequest request,
        CancellationToken cancellationToken = default)
    {
        await EnsureCanManageBranchAsync(callerId, callerRole, request.BranchId, cancellationToken);

        var branchExists = await DbContext.Branches.AnyAsync(b => b.Id == request.BranchId, cancellationToken);
        if (!branchExists)
        {
            throw new AppException("Branch not found.", AppErrorType.NotFound);
        }

        var name = request.Name.Trim();
        var nameTaken = await DbContext.Services.AnyAsync(
            s => s.BranchId == request.BranchId && s.Name == name,
            cancellationToken);
        if (nameTaken)
        {
            throw new AppException("A service with this name already exists for this branch.", AppErrorType.Conflict);
        }

        var service = new ServiceEntity
        {
            Name = name,
            Price = request.Price,
            BranchId = request.BranchId,
            ImagePath = string.IsNullOrWhiteSpace(request.ImagePath) ? null : request.ImagePath.Trim()
        };

        DbContext.Services.Add(service);
        await DbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(service);
    }
}
