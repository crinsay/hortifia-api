namespace Hortifia.Application.Identity.Responses;

public class UserDataResponse
{
    public required string Nickname { get; init; }
    public required double Latitude { get; init; }
    public required double Longtitude { get; init; }
    public required TimeOnly PreferredNotificationTime { get; init; }
    public string? CityName { get; set; }
}
