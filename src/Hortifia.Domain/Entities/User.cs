using Hortifia.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Hortifia.Domain.Entities;

public class User : IdentityUser
{
    public string Nickname { get; set; } = default!;

    //References
    public Coordinates Coordinates { get; private set; } = default!;
    public List<Room> Rooms { get; set; } = [];
    public List<Plant> Plants { get; set; } = [];
    public List <Post> Posts { get; set; } = [];
    public List <PostLike> PostLikes { get; set; } = [];

    public Result AddCustomData(string nickname, double latitude, double longitude)
    {
        var coordinatesCreationResult = Coordinates.Create(latitude, longitude);
        if (!coordinatesCreationResult.IsSuccess)
        {
            return Result.Failure(coordinatesCreationResult.ErrorMessage!);
        }

        Coordinates = coordinatesCreationResult.Value!;
        Nickname = nickname;

        return Result.Success();
    }

    public Result UpdateData(string nickname, double latitude, double longitude)
    {
        var coordinatesUpdateResult = Coordinates.Update(latitude, longitude);
        if (!coordinatesUpdateResult.IsSuccess)
        {
            return Result.Failure(coordinatesUpdateResult.ErrorMessage!);
        }

        Nickname = nickname;

        return Result.Success();
    }
}
