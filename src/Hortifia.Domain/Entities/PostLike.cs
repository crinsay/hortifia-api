namespace Hortifia.Domain.Entities;

public class PostLike
{
    public string UserId { get; set; } = default!;
    public int PostId { get; set; }
}