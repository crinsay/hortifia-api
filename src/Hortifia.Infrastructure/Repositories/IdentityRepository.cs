using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Identity.Responses;
using Hortifia.Domain.Entities;
using Hortifia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hortifia.Infrastructure.Repositories;

internal class IdentityRepository(HortifiaDbContext dbContext) : IIdentityRepository
{
    public async Task<User?> GetUserById(string userId, bool includePostLikes = false)
    {
        var mainQuery = dbContext.Users.AsQueryable();

        if (includePostLikes)
        {
            mainQuery = mainQuery.Include(u => u.PostLikes);
        }

        var user = await mainQuery.FirstOrDefaultAsync(u => u.Id == userId);

        return user;
    }

    public async Task<UserDataResponse?> GetUserDataById(string userId)
    {
        var userData = await dbContext.Users
            .Where(u => u.Id == userId)
            .Select(u => new UserDataResponse
            {
                Nickname = u.Nickname,
                Latitude = u.Coordinates.Latitude,
                Longtitude = u.Coordinates.Longtitude,
            })
            .FirstOrDefaultAsync();

        return userData;
    }

    public async Task<(double? Latitude, double? Longititude)> GetUserCoordinatesAsync(string userId)
    {
        var coordinates = await dbContext.Users
            .Where(u => u.Id == userId)
            .Select(u => new { u.Coordinates.Latitude, u.Coordinates.Longtitude })
            .FirstOrDefaultAsync();

        return (coordinates?.Latitude, coordinates?.Longtitude);
    }

    public void Delete(User user)
    {
        dbContext.Users.Remove(user);
    }

    public Task SaveChangesAsync()
        => dbContext.SaveChangesAsync();
}
