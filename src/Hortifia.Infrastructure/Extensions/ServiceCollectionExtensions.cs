using Hortifia.Application.Common.Interfaces;
using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Domain.Constants;
using Hortifia.Domain.Entities;
using Hortifia.Infrastructure.Authorization.Requirements.MustBeOwner;
using Hortifia.Infrastructure.Identity;
using Hortifia.Infrastructure.Persistence;
using Hortifia.Infrastructure.Persistence.MigrationManager;
using Hortifia.Infrastructure.Repositories;
using Hortifia.Infrastructure.Services;
using Hortifia.Infrastructure.Services.BlobStorage;
using Microsoft.AspNetCore.Authorization;
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

        services.AddScoped<IMigrationManager, MigrationManager>();

        services.AddIdentityApiEndpoints<User>()
            .AddRoles<IdentityRole>()
            .AddClaimsPrincipalFactory<HortifiaUserClaimsPrincipalFactory>()
            .AddEntityFrameworkStores<HortifiaDbContext>();

        services.AddHttpClient<IPermapeopleApiService, PermapeopleApiService>(client =>
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

        services.Configure<BlobStorageSettings>(configuration.GetSection("BlobStorage"));
        services.AddScoped<IBlobStorageService, BlobStorageService>();

        services.AddScoped<IUserContext, UserContext>();

        services.AddAuthorizationBuilder()
            .AddPolicy(HortifiaPolicies.MustBeOwner, policy =>
                policy.Requirements.Add(new MustBeOwnerRequirement()));
        services.AddSingleton<IAuthorizationHandler, MustBeOwnerRequirementHandler>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IIdentityRepository, IdentityRepository>();
        services.AddScoped<IRoomsRepository, RoomsRepository>();
        services.AddScoped<IPlantsRepository, PlantsRepository>();
        services.AddScoped<IPostsRepository, PostsRepository>();
    }
}
