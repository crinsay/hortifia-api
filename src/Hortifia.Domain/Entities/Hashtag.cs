namespace Hortifia.Domain.Entities;

public class Hashtag
{
    public int Id { get; set; }
    public string Content { get; set; } = default!;
    public int PostId { get; set; }

    public static Hashtag Create(string content)
    {
        return new Hashtag
        {
            Content = content
        };
    }
}