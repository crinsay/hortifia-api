namespace Hortifia.Domain.Entities;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public string? ImgBlobName { get; set; }
    public int LikesNumber { get; set; }
    public string AuthorId { get; set; } = default!;
}