using Hortifia.Domain.Common;
using Hortifia.Domain.Interfaces;

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

    public Result SetExpectedWateringDate(List<WateringRequirement> wateringRequirements, List<LightCondition> lightRequirements, TimeOnly notificationTime)
    {
        double predictedDays = 0;

        if (wateringRequirements.Count == 0 || lightRequirements.Count == 0)
        {
            return Result.Failure("Insufficient data to calculate expected watering date.");
        }

        foreach (var requirement in wateringRequirements)
        {
            predictedDays += requirement switch
            {
                WateringRequirement.Dry => 14,
                WateringRequirement.Moist => 7,
                WateringRequirement.Wet => 4,
                _ => 7
            };
        }

        predictedDays /= wateringRequirements.Count;

        double multiplier = 1;

        foreach (var requirement in lightRequirements)
        {
            if (requirement == LightCondition.High)
            {
                multiplier += LightCondition switch
                {
                    LightCondition.High => 1,
                    LightCondition.Medium => 1.2,
                    LightCondition.Low => 1.5,
                    _ => 1
                };
            }
            else if (requirement == LightCondition.Medium)
            {
                multiplier += LightCondition switch
                {
                    LightCondition.High => 0.8,
                    LightCondition.Medium => 1,
                    LightCondition.Low => 1.2,
                    _ => 1
                };
            }
            else if (requirement == LightCondition.Low)
            {
                multiplier += LightCondition switch
                {
                    LightCondition.High => 0.7,
                    LightCondition.Medium => 0.9,
                    LightCondition.Low => 1.1,
                    _ => 1
                };
            }            
        }

        multiplier /= lightRequirements.Count;

        predictedDays *= multiplier;

        multiplier = Room.Temperature switch
        {
            > 25 => 0.9,
            >= 15 and <= 25 => 1,
            < 15 => 0.8,
            _ => 1
        };

        predictedDays *= multiplier;

        /* Check weather conditions(possibly for the days counted so far):
        - If temperature is above 25C: x0.9
        - If temperature is between 15C and 25C: x1
        - If temperature is below 15C and it's near the heater: x0.7
        - If temperature is below 15C and it's NOT near the heater: x0.9 */

        var predictedDaysRounded = (int)Math.Round(predictedDays);

        ExpectedWateringDate = LastWateringDate.AddDays(predictedDaysRounded);
        ExpectedWateringDate = ExpectedWateringDate.Date + notificationTime.ToTimeSpan();

        if (ExpectedWateringDate < DateTime.Now)
        {
            ExpectedWateringDate = DateTime.Now.AddMinutes(5);
        }

        return Result.Success();
    }
}