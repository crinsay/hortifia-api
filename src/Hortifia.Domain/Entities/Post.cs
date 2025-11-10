namespace Hortifia.Domain.Entities;

public class Post
{
    public int Id { get; private set; }
    public string Title { get; set; } = default!;
    public DateTime CreateDate { get; private set; }
    public string Content { get; private set; } = default!;
    public string? ImgBlobName { get; set; }
    public int LikesNumber { get; private set; }
    public string AuthorId { get; private set; } = default!;

    //References
    public ICollection<Hashtag> Hashtags { get; private set; } = [];
    public User Author { get; private set; } = default!;

    public static Post Create(string title, string content, IEnumerable<string> hashtagsContent, string authorId)
    {
        var hashtags = hashtagsContent.Select(Hashtag.Create).ToList();

        return new Post
        {
            Title = title,
            Content = content,
            CreateDate = DateTime.UtcNow,
            Hashtags = hashtags,
            AuthorId = authorId
        };
    }
}