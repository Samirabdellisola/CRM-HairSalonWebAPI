using SalonCRM.Infrastructure;

namespace SalonCRM.Api.Configurations;

public static class DbConfig
{
    /// <summary>
    /// Wires EF Core / Npgsql via Infrastructure DI.
    /// Production expects ConnectionStrings__DefaultConnection from Render env vars (Supabase).
    /// </summary>
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);
        return services;
    }
}
