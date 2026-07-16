using Microsoft.EntityFrameworkCore;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Application.Staff.DTOs;
using SalonCRM.Application.Staff.Executors;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services.Common;

namespace SalonCRM.Infrastructure.Services.Staff.Executors;

public class UpdateStaffExecutor : StaffExecutorBase, IUpdateStaffExecutor
{
    public UpdateStaffExecutor(AppDbContext dbContext, IBranchScopeChecker branchScopeChecker)
        : base(dbContext, branchScopeChecker)
    {
    }

    public async Task<StaffResponse> ExecuteAsync(
        Guid callerId,
        UserRole callerRole,
        Guid staffId,
        UpdateStaffRequest request,
        CancellationToken cancellationToken = default)
    {
        var staff = await GetStaffOrThrowAsync(staffId, cancellationToken);
        await EnsureCanViewOrEditStaffAsync(callerId, callerRole, staff, cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            if (normalizedEmail != staff.Email)
            {
                var emailTaken = await DbContext.Users
                    .AnyAsync(u => u.Email == normalizedEmail && u.Id != staffId, cancellationToken);
                if (emailTaken)
                {
                    throw new AppException("Email is already registered.", AppErrorType.Conflict);
                }

                staff.Email = normalizedEmail;
            }
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            staff.Name = request.Name.Trim();
        }

        if (request.Phone is not null)
        {
            staff.Phone = request.Phone;
        }

        if (request.Address is not null)
        {
            staff.Address = request.Address;
        }

        await DbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(staff);
    }
}
