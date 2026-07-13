using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalonCRM.Application.Auth;
using SalonCRM.Application.Auth.Executors;
using SalonCRM.Application.Auth.Options;
using SalonCRM.Application.Users.Executors;
using SalonCRM.Domain.Entities;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services;
using SalonCRM.Infrastructure.Services.Auth.Executors;
using SalonCRM.Infrastructure.Services.Common;
using SalonCRM.Infrastructure.Services.Users.Executors;

namespace SalonCRM.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers Infrastructure services (EF Core + Npgsql for Supabase PostgreSQL,
    /// Auth & Users executors). Connection string and JWT secrets are read from
    /// configuration (env vars ConnectionStrings__DefaultConnection, Jwt__Key, etc. on Render).
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<AuthSettings>(configuration.GetSection(AuthSettings.SectionName));

        services.AddSingleton(new PasswordHasher<User>());
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IEmailService, DebugEmailService>();
        services.AddScoped<IBranchScopeChecker, BranchScopeChecker>();

        services.AddScoped<ILoginExecutor, LoginExecutor>();
        services.AddScoped<ILogoutExecutor, LogoutExecutor>();
        services.AddScoped<IRefreshTokenExecutor, RefreshTokenExecutor>();
        services.AddScoped<IRegisterCustomerExecutor, RegisterCustomerExecutor>();
        services.AddScoped<IRegisterStaffExecutor, RegisterStaffExecutor>();
        services.AddScoped<IRegisterCentralOfficeExecutor, RegisterCentralOfficeExecutor>();
        services.AddScoped<IChangePasswordExecutor, ChangePasswordExecutor>();
        services.AddScoped<IForgotPasswordExecutor, ForgotPasswordExecutor>();
        services.AddScoped<IResetPasswordExecutor, ResetPasswordExecutor>();

        services.AddScoped<IUpdateUserExecutor, UpdateUserExecutor>();
        services.AddScoped<IUpdateUserRoleExecutor, UpdateUserRoleExecutor>();
        services.AddScoped<IDeleteUserExecutor, DeleteUserExecutor>();

        // Repository implementations will be registered here as the CRM grows.

        return services;
    }
}
