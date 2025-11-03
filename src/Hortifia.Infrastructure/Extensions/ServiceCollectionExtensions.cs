using Hortifia.Application.Common.Interfaces.Identity;
using Hortifia.Domain.Common.Interfaces.Repositories;
using Hortifia.Domain.Entities;
using Hortifia.Infrastructure.Identity;
using Hortifia.Infrastructure.Persistence;
using Hortifia.Infrastructure.Repositories;
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

        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IRoomsRepository, RoomsRepository>();
    }
}
