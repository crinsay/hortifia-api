using Hortifia.Application.Identity.Responses;
using Hortifia.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hortifia.Application.Common.Interfaces.Repositories;

public interface IIdentityRepository
{
    Task<User?> GetUserById(string userId, bool includePostLikes = false);
    Task<UserDataResponse?> GetUserDataById(string userId);

    void Delete(User user);
    Task SaveChangesAsync();
}
