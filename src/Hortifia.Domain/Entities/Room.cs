using Hortifia.Domain.Interfaces;

namespace Hortifia.Domain.Entities;

public enum RoomType
{
    Ordinary = 0,
    Kitchen = 1,
    Bathroom = 2
}

public class Room : IOwnedResource
{
    public int Id { get; private set; }
    public string Name { get; private set; } = default!;
    public RoomType Type { get; private set; } = RoomType.Ordinary;
    public byte Humidity { get; private set; }
    public float Temperature { get; private set; }
    public string OwnerId { get; private set; } = default!;

    //References
    public List<Plant> Plants { get; set; } = [];

    public static Room Create(string name, RoomType type, byte humidity, float temperature, string userId)
    {
        return new Room
        {
            Name = name,
            Type = type,
            Humidity = humidity,
            Temperature = temperature,
            OwnerId = userId
        };
    }

    public void Update(string name, RoomType type, byte humidity, float temperature)
    {
        Name = name;
        Type = type;
        Humidity = humidity;
        Temperature = temperature;
    }
}