namespace Hortifia.Domain.Entities;

public enum LightCondition
{
    Low = 0,
    Medium = 1,
    High = 2
}

public class Plant
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? ImgBlobName { get; set; }
    public bool IsNearHeater { get; set; }
    public LightCondition LightCondition { get; set; } = LightCondition.Medium;
    public byte WateringStatus { get; set; }
    public DateTime WateringDate { get; set; }
    public bool IsFavourite { get; set; }
    public string UserId { get; set; } = default!;
    public int RoomId { get; set; }

    //References
    public Room Room { get; set; } = default!;
}