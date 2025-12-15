using Hortifia.Application.Identity.Responses;
using Hortifia.Domain.Entities;

namespace Hortifia.Application.Common.Interfaces.Repositories;

public interface IIdentityRepository
{
    Task<User?> GetUserById(string userId, bool includePostLikes = false);
    Task<UserDataResponse?> GetUserDataById(string userId);
    Task<(double? Latitude, double? Longitude)> GetUserCoordinatesAsync(string userId);
    void Delete(User user);
    Task SaveChangesAsync();
}
