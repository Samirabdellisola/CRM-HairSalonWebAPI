using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalonCRM.Application.Auth;
using SalonCRM.Application.Auth.Executors;
using SalonCRM.Application.Auth.Options;
using SalonCRM.Application.Branches.Executors;
using SalonCRM.Application.Customers.Executors;
using SalonCRM.Application.Orders.Executors;
using SalonCRM.Application.Services.Executors;
using SalonCRM.Application.Staff.Executors;
using SalonCRM.Application.Users.Executors;
using SalonCRM.Domain.Entities;
using SalonCRM.Infrastructure.Persistence;
using SalonCRM.Infrastructure.Services;
using SalonCRM.Infrastructure.Services.Auth.Executors;
using SalonCRM.Infrastructure.Services.Branches.Executors;
using SalonCRM.Infrastructure.Services.Common;
using SalonCRM.Infrastructure.Services.Customers.Executors;
using SalonCRM.Infrastructure.Services.Orders.Executors;
using SalonCRM.Infrastructure.Services.Services.Executors;
using SalonCRM.Infrastructure.Services.Staff.Executors;
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

        services.AddScoped<ICreateBranchExecutor, CreateBranchExecutor>();
        services.AddScoped<IGetBranchesExecutor, GetBranchesExecutor>();
        services.AddScoped<IGetBranchByIdExecutor, GetBranchByIdExecutor>();
        services.AddScoped<IUpdateBranchExecutor, UpdateBranchExecutor>();
        services.AddScoped<IDeactivateBranchExecutor, DeactivateBranchExecutor>();
        services.AddScoped<IActivateBranchExecutor, ActivateBranchExecutor>();
        services.AddScoped<IFreezeBranchExecutor, FreezeBranchExecutor>();
        services.AddScoped<IAssignBranchAdminExecutor, AssignBranchAdminExecutor>();

        services.AddScoped<IGetStaffListExecutor, GetStaffListExecutor>();
        services.AddScoped<IGetStaffByIdExecutor, GetStaffByIdExecutor>();
        services.AddScoped<IGetBranchStaffExecutor, GetBranchStaffExecutor>();
        services.AddScoped<IUpdateStaffExecutor, UpdateStaffExecutor>();
        services.AddScoped<IActivateStaffExecutor, ActivateStaffExecutor>();
        services.AddScoped<IDeactivateStaffExecutor, DeactivateStaffExecutor>();
        services.AddScoped<IFreezeStaffExecutor, FreezeStaffExecutor>();

        services.AddScoped<IGetCustomersExecutor, GetCustomersExecutor>();
        services.AddScoped<IGetCustomerByIdExecutor, GetCustomerByIdExecutor>();
        services.AddScoped<IGetBranchCustomersExecutor, GetBranchCustomersExecutor>();
        services.AddScoped<IUpdateCustomerExecutor, UpdateCustomerExecutor>();
        services.AddScoped<IGetCustomerProfileExecutor, GetCustomerProfileExecutor>();
        services.AddScoped<IUpdateCustomerProfileExecutor, UpdateCustomerProfileExecutor>();
        services.AddScoped<IUploadCustomerPhotoExecutor, UploadCustomerPhotoExecutor>();
        services.AddScoped<IGetSpecialDatesExecutor, GetSpecialDatesExecutor>();

        services.AddScoped<IGetServicesExecutor, GetServicesExecutor>();
        services.AddScoped<IGetServiceByIdExecutor, GetServiceByIdExecutor>();
        services.AddScoped<ICreateServiceExecutor, CreateServiceExecutor>();
        services.AddScoped<IUpdateServiceExecutor, UpdateServiceExecutor>();
        services.AddScoped<IDeleteServiceExecutor, DeleteServiceExecutor>();

        services.AddScoped<IGetOrdersExecutor, GetOrdersExecutor>();
        services.AddScoped<IGetOrderByIdExecutor, GetOrderByIdExecutor>();
        services.AddScoped<IGetBranchOrdersExecutor, GetBranchOrdersExecutor>();
        services.AddScoped<IGetCustomerOrdersExecutor, GetCustomerOrdersExecutor>();
        services.AddScoped<IGetStaffOrdersExecutor, GetStaffOrdersExecutor>();
        services.AddScoped<ICreateOrderExecutor, CreateOrderExecutor>();
        services.AddScoped<IUpdateOrderExecutor, UpdateOrderExecutor>();
        services.AddScoped<IAddOrderServiceExecutor, AddOrderServiceExecutor>();
        services.AddScoped<IRemoveOrderServiceExecutor, RemoveOrderServiceExecutor>();
        services.AddScoped<ICompleteOrderExecutor, CompleteOrderExecutor>();
        services.AddScoped<ICancelOrderExecutor, CancelOrderExecutor>();
        services.AddScoped<IGetPendingPaymentOrdersExecutor, GetPendingPaymentOrdersExecutor>();
        services.AddScoped<IGetBranchPendingPaymentOrdersExecutor, GetBranchPendingPaymentOrdersExecutor>();

        // Repository implementations will be registered here as the CRM grows.

        return services;
    }
}
