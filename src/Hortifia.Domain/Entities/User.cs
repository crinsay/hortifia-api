using Microsoft.AspNetCore.Identity;

namespace Hortifia.Domain.Entities;

public class User : IdentityUser
{
    public string Nickname { get; set; } = default!;

    //References
    public Coordinates Coordinates { get; set; } = default!;
    public List<Room> Rooms { get; set; } = [];
    public List<Plant> Plants { get; set; } = [];
    public List <Post> Posts { get; set; } = [];
    public List <PostLike> PostLikes { get; set; } = [];
}
