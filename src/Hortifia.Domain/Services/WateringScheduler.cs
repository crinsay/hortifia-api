using Hortifia.Domain.Common;
using Hortifia.Domain.Entities;

namespace Hortifia.Domain.Services;

public class WateringScheduler
{
    public static Result<DateTime> CalculateExpectedWateringDate(
            DateTime lastWateringDate,
            LightCondition plantLightCondition,
            bool isNearHeater,
            double roomTemperature,
            byte roomHumidity,
            List<WateringRequirement> wateringRequirements,
            List<LightCondition> lightRequirements,
            TimeOnly notificationTime,
            List<float?> temperatures)
    {
        double predictedDays = 0;

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

        double multiplier = 0;

        foreach (var requirement in lightRequirements)
        {
            if (requirement == LightCondition.High)
            {
                multiplier += plantLightCondition switch
                {
                    LightCondition.High => 1,
                    LightCondition.Medium => 1.2,
                    LightCondition.Low => 1.5,
                    _ => 1
                };
            }
            else if (requirement == LightCondition.Medium)
            {
                multiplier += plantLightCondition switch
                {
                    LightCondition.High => 0.8,
                    LightCondition.Medium => 1,
                    LightCondition.Low => 1.2,
                    _ => 1
                };
            }
            else if (requirement == LightCondition.Low)
            {
                multiplier += plantLightCondition switch
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

        multiplier = roomTemperature switch
        {
            > 25 => 0.9,
            >= 15 and <= 25 => 1,
            < 15 => 0.8,
            _ => 1
        };

        predictedDays *= multiplier;

        multiplier = roomHumidity switch
        {
            >= 0 and < 30 => 0.8,      
            >= 30 and < 50 => 0.9,     
            >= 50 and <= 70 => 1.0,    
            > 70 and <= 85 => 1.1,     
            > 85 and <= 100 => 1.2,    
            _ => 1.0
        };

        predictedDays *= multiplier;
        multiplier = 1;

        for (int i = 0; i < predictedDays && i < temperatures.Count; i++)
        {
            multiplier *= temperatures[i] switch
            {
                > 25 => 0.9,
                >= 15 and <= 25 => 1,
                < 15 when isNearHeater => 0.7,
                < 15 when !isNearHeater => 0.9,
                _ => 1
            };
        }

        predictedDays *= multiplier;
        var predictedDaysRounded = (int)Math.Round(predictedDays);

        var expectedDate = lastWateringDate.AddDays(predictedDaysRounded);
        expectedDate = expectedDate.Date + notificationTime.ToTimeSpan();

        return Result<DateTime>.Success(expectedDate);
    }
}
