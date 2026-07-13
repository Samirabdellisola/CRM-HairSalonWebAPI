using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalonCRM.Infrastructure.Persistence;

namespace SalonCRM.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers Infrastructure services (EF Core + Npgsql for Supabase PostgreSQL).
    /// Connection string is read from "ConnectionStrings:DefaultConnection"
    /// (env var ConnectionStrings__DefaultConnection on Render).
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Repository implementations will be registered here as the CRM grows.

        return services;
    }
}
