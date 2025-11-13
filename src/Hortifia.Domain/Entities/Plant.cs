using Hortifia.Domain.Common;
using Hortifia.Domain.Interfaces;
using Hortifia.Domain.Services;

namespace Hortifia.Domain.Entities;

public enum LightCondition
{
    Low = 0,
    Medium = 1,
    High = 2
}
public enum WateringRequirement
{
    Dry = 0,
    Moist = 1,
    Wet = 2
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
    public int PlantApiId { get; set; }

    //References
    public Room Room { get; set; } = default!;

    public static Plant Create(string name, string commonName, string? picture, bool isNearHeater,
        LightCondition lightCondition, DateTime lastWateringDate, int roomId,  string ownerId, int plantApiId, Room room)
    {
        return new Plant
        {
            Name = name,
            CommonName = commonName,
            ImgBlobName = picture,
            IsNearHeater = isNearHeater,
            LightCondition = lightCondition,
            LastWateringDate = lastWateringDate,
            IsFavourite = false,
            RoomId = roomId,
            OwnerId = ownerId,
            PlantApiId = plantApiId,
            Room = room
        };
    }

    public void Update(string name, string commonName, string? picture, bool isNearHeater,
        LightCondition lightCondition, DateTime lastWateringDate, int roomId, int plantApiId, Room room)
    {
        Name = name;
        CommonName = commonName;
        ImgBlobName = picture;
        IsNearHeater = isNearHeater;
        LightCondition = lightCondition;
        LastWateringDate = lastWateringDate;
        IsFavourite = false;
        RoomId = roomId;
        PlantApiId = plantApiId;
        Room = room;
    }

    public void ToggleFavourite()
    {
        IsFavourite = !IsFavourite;
    }

    public void UpdateLastWateringDate()
    {
        LastWateringDate = DateTime.UtcNow;
    }

    public Result SetExpectedWateringDate(List<WateringRequirement> wateringRequirements, List<LightCondition> lightRequirements, TimeOnly notificationTime)
    {
        if (wateringRequirements.Count == 0 || lightRequirements.Count == 0)
        {
            return Result.Failure("Insufficient data to calculate expected watering date.");
        }

        var result = WateringScheduler.CalculateExpectedWateringDate(
            lastWateringDate: LastWateringDate,
            plantLightCondition: LightCondition,
            isNearHeater: IsNearHeater,
            roomTemperature: Room.Temperature,
            wateringRequirements: wateringRequirements,
            lightRequirements: lightRequirements,
            notificationTime: notificationTime);

        if (!result.IsSuccess)
        {
            return Result.Failure(result.ErrorMessage!);
        }

        var expectedWateringDate = result.Value;

        if (expectedWateringDate < DateTime.UtcNow)
        {
            expectedWateringDate = DateTime.UtcNow.AddMinutes(5);
        }

        ExpectedWateringDate = expectedWateringDate;

        return Result.Success();
    }
}
