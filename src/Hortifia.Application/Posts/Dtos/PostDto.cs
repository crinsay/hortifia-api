using Hortifia.Domain.Entities;

namespace Hortifia.Application.Posts.Dtos;

public class PostDto
{
    public int Id { get; init; }
    public string Title { get; init; } = default!;
    public DateTime CreateDate { get; init; }
    public string Content { get; init; } = default!;
    public string? ImgUrl { get; set; }
    public IEnumerable<string> Hashtags { get; init; } = [];

    public static PostDto CreateFromEntity(Post post)
    {
        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            CreateDate = post.CreateDate,
            Content = post.Content,
            Hashtags = post.Hashtags.Select(h => h.Content)
        };
    }
}
