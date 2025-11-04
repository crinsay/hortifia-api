using Hortifia.API.Handlers;
using Microsoft.OpenApi.Models;

namespace Hortifia.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPresentation(this IServiceCollection services, IHostBuilder builder)
        {
            services.AddAuthentication();

            services.AddControllers();

            services.AddSwaggerWithAuthorization();

            services.AddProblemDetails();
            services.AddExceptionHandler<AppExceptionHandler>();

            services.AddEndpointsApiExplorer();
        }

        private static void AddSwaggerWithAuthorization(this IServiceCollection services)
        {
            services.AddSwaggerGen(cfg =>
            {
                cfg.AddSecurityDefinition("bearerAuthentication",
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer"
                    });

                cfg.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "bearerAuthentication"
                        }
                    },
                    []
                }
            });
            });
        }
    }
}
