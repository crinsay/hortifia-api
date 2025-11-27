using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Domain.Entities;
using Hortifia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hortifia.Infrastructure.Repositories;

internal class UserDeviceTokenRepository(HortifiaDbContext dbContext) : IUserDeviceTokenRepository
{
    public async Task AddTokenAsync(UserDeviceToken userDeviceToken)
    {
        dbContext.UserDeviceTokens.Add(userDeviceToken);
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<string>> GetTokensByUserIdAsync(string userId)
    {
        var tokens = await dbContext.UserDeviceTokens
            .Where(t => t.UserId == userId)
            .Select(t => t.DeviceToken)
            .ToListAsync();

        return tokens;
    }

    public async Task<bool> TokenExistsAsync(string userId, string deviceToken)
    {
        return await dbContext.UserDeviceTokens
            .AnyAsync(t => t.UserId == userId &&
                           t.DeviceToken == deviceToken);
    }

    public async Task DeleteAsync(UserDeviceToken userDeviceToken)
    {
        dbContext.UserDeviceTokens.Remove(userDeviceToken);
        await dbContext.SaveChangesAsync();
    }
}
