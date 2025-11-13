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
            List<WateringRequirement> wateringRequirements,
            List<LightCondition> lightRequirements,
            TimeOnly notificationTime)
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

        /* Check weather conditions(possibly for the days counted so far):
        - If temperature is above 25C: x0.9
        - If temperature is between 15C and 25C: x1
        - If temperature is below 15C and it's near the heater: x0.7
        - If temperature is below 15C and it's NOT near the heater: x0.9 */

        var predictedDaysRounded = (int)Math.Round(predictedDays);

        var expectedDate = lastWateringDate.AddDays(predictedDaysRounded);
        expectedDate = expectedDate.Date + notificationTime.ToTimeSpan();

        return Result<DateTime>.Success(expectedDate);
    }
}
