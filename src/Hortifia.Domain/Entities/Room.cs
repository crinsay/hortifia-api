namespace Hortifia.Domain.Entities;

public enum RoomType
{
    Ordinary = 0,
    Kitchen = 1,
    Bathroom = 2
}

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public RoomType Type { get; set; } = RoomType.Ordinary;
    public byte Humidity { get; set; }
    public float Temperature { get; set; }
    public string UserId { get; set; } = default!;
}