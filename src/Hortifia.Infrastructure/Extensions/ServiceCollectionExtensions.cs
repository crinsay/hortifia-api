using Hortifia.Infrastructure.Persistence;
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
    }
}
