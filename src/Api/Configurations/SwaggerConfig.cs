using Microsoft.OpenApi.Models;

namespace SalonCRM.Api.Configurations;

public static class SwaggerConfig
{
    public const string EnabledInProductionKey = "Swagger:EnabledInProduction";

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Salon CRM API",
                Version = "v1",
                Description = "Cloud-based Salon CRM backend API"
            });
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(
        this IApplicationBuilder app,
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        var enableInProduction = configuration.GetValue(EnabledInProductionKey, false);

        if (environment.IsDevelopment() || enableInProduction)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Salon CRM API v1");
                options.RoutePrefix = "swagger";
            });
        }

        return app;
    }
}
