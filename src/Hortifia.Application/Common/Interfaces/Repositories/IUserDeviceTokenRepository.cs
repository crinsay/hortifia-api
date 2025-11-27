using Hortifia.Domain.Entities;

namespace Hortifia.Application.Common.Interfaces.Repositories;

public interface IUserDeviceTokenRepository
{
    Task AddTokenAsync(UserDeviceToken userDeviceToken);
    Task<List<string>> GetTokensByUserIdAsync(string userId);
    Task<bool> TokenExistsAsync(string userId, string deviceToken);
    Task DeleteAsync(UserDeviceToken userDeviceToken);
}
