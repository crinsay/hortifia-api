namespace Hortifia.Domain.Entities;

public class UserDeviceToken
{
    public string UserId { get; private set; } = default!;
    public string DeviceToken { get; private set; } = default!;
    public User User { get; private set; } = default!;

    public static UserDeviceToken Create(string userId, string deviceToken)
    {
        return new UserDeviceToken
        {
            UserId = userId,
            DeviceToken = deviceToken
        };
    }
}
