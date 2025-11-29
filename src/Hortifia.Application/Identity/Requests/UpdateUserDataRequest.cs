namespace Hortifia.Application.Identity.Requests;

public class UpdateUserDataRequest
{
    public required string Nickname { get; init; }
    public required double Latitude { get; init; }
    public required double Longtitude { get; init; }
    public required TimeOnly PreferredNotificationTime { get; init; }
}
