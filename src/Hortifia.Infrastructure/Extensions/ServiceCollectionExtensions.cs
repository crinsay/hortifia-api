using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Domain.Entities;
using Hortifia.Infrastructure.Identity;
using Hortifia.Infrastructure.Persistence;
using Hortifia.Infrastructure.Repositories;
using Hortifia.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hortifia.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<HortifiaDbContext>(options =>
            options
            .UseSqlServer(configuration.GetConnectionString("HortifiaDb"))
            .EnableSensitiveDataLogging());

        services.AddIdentityApiEndpoints<User>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<HortifiaDbContext>();

        services.AddHttpClient<IPermapeopleApiService, PermapesopleApiService>(client =>
        {
            var baseUrl = configuration["Permapeople:BaseUrl"];
            var keyId = configuration["Permapeople:KeyId"];
            var keySecret = configuration["Permapeople:KeySecret"];

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("Permapeople BaseUrl is not configured.");
            }

            if (string.IsNullOrWhiteSpace(keyId) || string.IsNullOrWhiteSpace(keySecret))
            {
                throw new InvalidOperationException("Permapeople API keys are missing in configuration or secrets.json.");
            }

            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Add("x-permapeople-key-id", keyId);
            client.DefaultRequestHeaders.Add("x-permapeople-key-secret", keySecret);
        });

        services.AddHttpContextAccessor();

        services.AddScoped<IUserContext, UserContext>();

        services.AddScoped<IIdentityRepository, IdentityRepository>();
        services.AddScoped<IRoomsRepository, RoomsRepository>();
    }
}
