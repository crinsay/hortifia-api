using Hortifia.Application.Common.Interfaces.Repositories;
using Hortifia.Application.Identity.Responses;
using Hortifia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hortifia.Infrastructure.Repositories;

internal class IdentityRepository(HortifiaDbContext dbContext) : IIdentityRepository
{
    public async Task<UserDataResponse> GetUserDataById(string userId)
    {
        var userData = await dbContext.Users
            .Where(u => u.Id == userId)
            .Select(u => new UserDataResponse
            {
                Nickname = u.Nickname,
                Latitude = u.Coordinates.Latitude,
                Longtitude = u.Coordinates.Longtitude,
            })
            .FirstAsync();

        return userData;
    }
}
