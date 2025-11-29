using Hortifia.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Hortifia.Domain.Entities;

public class User : IdentityUser
{
    public string Nickname { get; private set; } = default!;
    public TimeOnly PreferredNotificationTime { get; private set; } = default!;

    //References
    public Coordinates Coordinates { get; private set; } = default!;
    public List<Room> Rooms { get; set; } = [];
    public List<Plant> Plants { get; set; } = [];
    public List <Post> Posts { get; set; } = [];
    public List <PostLike> PostLikes { get; set; } = [];

    public void SetDefaultData()
    {
        Nickname = "[Unnamed User]";
        Coordinates = Coordinates.Create(0, 0).Value!;
        PreferredNotificationTime = new TimeOnly(8, 0);
    }

    public Result UpdateData(string nickname, double latitude, double longitude, TimeOnly preferredNotificationTime)
    {
        if (string.IsNullOrEmpty(nickname.Trim()))
        {
            return Result.Failure($"Nickname cannot be null or empty.");
        }

        var coordinatesUpdateResult = Coordinates.Update(latitude, longitude);
        if (!coordinatesUpdateResult.IsSuccess)
        {
            return Result.Failure(coordinatesUpdateResult.ErrorMessage!);
        }

        Nickname = nickname;
        PreferredNotificationTime = preferredNotificationTime;

        return Result.Success();
    }
}
