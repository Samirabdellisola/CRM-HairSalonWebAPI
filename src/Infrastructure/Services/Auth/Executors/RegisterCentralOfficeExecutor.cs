using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SalonCRM.Application.Auth;
using SalonCRM.Application.Auth.DTOs;
using SalonCRM.Application.Auth.Executors;
using SalonCRM.Application.Auth.Options;
using SalonCRM.Application.Common.Exceptions;
using SalonCRM.Domain.Entities;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;

namespace SalonCRM.Infrastructure.Services.Auth.Executors;

public class RegisterCentralOfficeExecutor : AuthExecutorBase, IRegisterCentralOfficeExecutor
{
    public RegisterCentralOfficeExecutor(
        AppDbContext dbContext,
        IJwtTokenService jwtTokenService,
        IOptions<AuthSettings> authSettings,
        PasswordHasher<User> passwordHasher)
        : base(dbContext, jwtTokenService, authSettings, passwordHasher)
    {
    }

    public async Task<RegisterUserResponse> ExecuteAsync(RegisterCentralOfficeRequest request, CancellationToken cancellationToken = default)
    {
        var centralOfficeExists = await DbContext.Users.AnyAsync(u => u.Role == UserRole.CentralOffice, cancellationToken);
        if (centralOfficeExists)
        {
            throw new AppException("A Central Office user already exists.", AppErrorType.Validation);
        }

        var email = NormalizeEmail(request.Email);
        await EnsureEmailIsAvailableAsync(email, cancellationToken);

        var user = new User
        {
            Email = email,
            Role = UserRole.CentralOffice,
            IsActive = true
        };
        user.PasswordHash = PasswordHasher.HashPassword(user, request.Password);

        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync(cancellationToken);

        return ToRegisterResponse(user);
    }
}
