namespace SalonCRM.Api.Configurations;

public static class CorsConfig
{
    public const string SectionName = "Cors";
    public const string PolicyName = "Frontend";

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = ResolveOrigins(configuration);

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                policy.WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }

    private static string[] ResolveOrigins(IConfiguration configuration)
    {
        var fromArray = configuration.GetSection($"{SectionName}:AllowedOrigins").Get<string[]>();
        if (fromArray is { Length: > 0 })
        {
            return fromArray
                .Where(o => !string.IsNullOrWhiteSpace(o))
                .Select(o => o.Trim().TrimEnd('/'))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        // Render/env fallback: Cors__AllowedOrigins=https://a.com,https://b.com
        var csv = configuration[$"{SectionName}:AllowedOrigins"];
        if (!string.IsNullOrWhiteSpace(csv))
        {
            return csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(o => o.TrimEnd('/'))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        return ["http://localhost:5173"];
    }
}
