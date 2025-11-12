using Hortifia.Domain.Interfaces;

namespace Hortifia.Domain.Entities;

public class Post : IOwnedResource
{
    public int Id { get; private set; }
    public string Title { get; private set; } = default!;
    public DateTime CreateDate { get; private set; }
    public string Content { get; private set; } = default!;
    public string? ImgBlobName { get; set; }
    public int LikesNumber { get; private set; }
    public string OwnerId { get; private set; } = default!;

    //References
    public ICollection<Hashtag> Hashtags { get; private set; } = [];
    public User Author { get; private set; } = default!;

    public static Post Create(string title, string content, IEnumerable<string> hashtagsContent, string authorId)
    {
        return new Post
        {
            Title = title,
            Content = content,
            CreateDate = DateTime.UtcNow,
            Hashtags = [.. hashtagsContent.Select(Hashtag.Create)],
            OwnerId = authorId
        };
    }

    public void Update(string title, string content, IEnumerable<string> hashtagsContent)
    {
        Title = title;
        Content = content;
        Hashtags = [.. hashtagsContent.Select(Hashtag.Create)];
    }
}