using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SalonCRM.Application.Auth;
using SalonCRM.Application.Auth.DTOs;
using SalonCRM.Application.Auth.Executors;
using SalonCRM.Application.Auth.Options;
using SalonCRM.Domain.Entities;
using SalonCRM.Domain.Enums;
using SalonCRM.Infrastructure.Persistence;

namespace SalonCRM.Infrastructure.Services.Auth.Executors;

public class RegisterCustomerExecutor : AuthExecutorBase, IRegisterCustomerExecutor
{
    public RegisterCustomerExecutor(
        AppDbContext dbContext,
        IJwtTokenService jwtTokenService,
        IOptions<AuthSettings> authSettings,
        PasswordHasher<User> passwordHasher)
        : base(dbContext, jwtTokenService, authSettings, passwordHasher)
    {
    }

    public async Task<RegisterUserResponse> ExecuteAsync(RegisterCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);
        await EnsureEmailIsAvailableAsync(email, cancellationToken);

        var user = new User
        {
            Email = email,
            Role = UserRole.Customer,
            Phone = request.Phone,
            IsActive = true
        };
        user.PasswordHash = PasswordHasher.HashPassword(user, request.Password);

        DbContext.Users.Add(user);
        await DbContext.SaveChangesAsync(cancellationToken);

        return ToRegisterResponse(user);
    }
}
