namespace Hortifia.Domain.Entities;

public class PostLike
{
    public string UserId { get; private set; } = default!;
    public int PostId { get; private set; }

    //References
    public Post Post { get; set; } = default!;

    public static PostLike Create(string userId, int postId)
    {
        return new PostLike
        {
            UserId = userId,
            PostId = postId
        };
    }
}