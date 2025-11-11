using Hortifia.Domain.Interfaces;

namespace Hortifia.Domain.Entities;

public enum LightCondition
{
    Low = 0,
    Medium = 1,
    High = 2
}

public class Plant : IOwnedResource
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string CommonName { get; set; } = default!;
    public string? ImgBlobName { get; set; }
    public bool IsNearHeater { get; set; }
    public LightCondition LightCondition { get; set; } = LightCondition.Medium;
    public DateTime LastWateringDate { get; set; }
    public DateTime ExpectedWateringDate { get; set; }
    public bool IsFavourite { get; set; }
    public string OwnerId { get; set; } = default!;
    public int RoomId { get; set; }

    //References
    public Room Room { get; set; } = default!;
}