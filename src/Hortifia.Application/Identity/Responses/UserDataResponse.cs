using Hortifia.Domain.Entities;

namespace Hortifia.Application.Identity.Responses;

public class UserDataResponse
{
    public required string Id { get; init; }
    public required string Nickname { get; init; }
    public required double Latitude { get; init; }
    public required double Longitude { get; init; }
    public required TimeOnly PreferredNotificationTime { get; init; }
    public string? CityName { get; set; }

    public static UserDataResponse CreateFromEntity(User user)
    {
        return new UserDataResponse
        {
            Id = user.Id,
            Nickname = user.Nickname,
            Latitude = user.Coordinates.Latitude,
            Longitude = user.Coordinates.Longitude,
            PreferredNotificationTime = user.PreferredNotificationTime
        };
    }
}
