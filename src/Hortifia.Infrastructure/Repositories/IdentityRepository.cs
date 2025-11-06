using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Identity.Responses;
using Hortifia.Domain.Entities;
using Hortifia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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

    public void Delete(User user)
    {
        dbContext.Users.Remove(user);
    }

    public Task SaveChangesAsync()
        => dbContext.SaveChangesAsync();
}









