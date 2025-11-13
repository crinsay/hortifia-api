namespace Hortifia.Application.Posts.Dtos;

public class DetailedPostDto
{
    public int Id { get; init; }
    public string Title { get; init; } = default!;
    public DateTime CreateDate { get; init; }
    public string Content { get; init; } = default!;
    public string? ImgUrl { get; set; }
    public int LikesNumber { get; init; }
    public IEnumerable<string> Hashtags { get; init; } = [];
    public string Author { get; init; } = default!;
}
