using Microsoft.OpenApi.Models;

namespace SalonCRM.Api.Configurations;

public static class SwaggerConfig
{
    public const string EnabledInProductionKey = "Swagger:EnabledInProduction";
    private const string BearerSecurityScheme = "Bearer";

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

            // Lets Swagger UI send "Authorization: Bearer {token}" for protected endpoints.
            options.AddSecurityDefinition(BearerSecurityScheme, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = BearerSecurityScheme,
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter the access token obtained from /auth/login or /auth/refresh-token."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = BearerSecurityScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
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
