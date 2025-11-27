using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
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
using Hortifia.Infrastructure.Services.BlobStorage;
using Hortifia.Infrastructure.Services.ExternalApis;
using Hortifia.Infrastructure.Services.Firebase;
using Hortifia.Infrastructure.Services.Quartz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Hortifia.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("HortifiaDb");
        services.AddDbContext<HortifiaDbContext>(options =>
            options
            .UseSqlServer(connectionString)
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

        services.AddHttpClient<IWeatherApiService, WeatherApiService>(client =>
        {
            var baseUrl = configuration["WeatherApi:BaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("WeatherApi BaseUrl is not configured.");
            }

            client.BaseAddress = new Uri(baseUrl);
        });

        services.AddHttpClient<ICityApiService, CityApiService>(client =>
        {
            var baseUrl = configuration["CityApi:BaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("CityApi BaseUrl is not configured.");
            }

            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.UserAgent.ParseAdd(configuration["CityApi:UserAgent"]);
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
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IUserDeviceTokenRepository, UserDeviceTokenRepository>();
        services.AddScoped<IQuartzSchedulerService, QuartzSchedulerService>();

        var firebaseJson = configuration["FirebaseServiceAccount"];

        if (string.IsNullOrWhiteSpace(firebaseJson))
        {
            throw new InvalidOperationException("FirebaseServiceAccount is missing.");
        }

        var serviceAccount = CredentialFactory.FromJson<ServiceAccountCredential>(firebaseJson);
        var googleCredential = serviceAccount.ToGoogleCredential();

        FirebaseApp.Create(new AppOptions
        {
            Credential = googleCredential
        });

        services.AddQuartz(q =>
        {
            q.UsePersistentStore(p =>
            {
                p.UseSqlServer(cfg =>
                {
                    cfg.ConnectionString = connectionString!;
                });
            });

            q.SetProperty("quartz.serializer.type", "json");
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
    }
}