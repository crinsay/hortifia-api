using Hortifia.Domain.Common;
using Hortifia.Domain.Entities;

namespace Hortifia.Application.Plants.Static;

public static class PlantDataParser
{
    public static Result<List<WateringRequirement>> ParseWaterRequirements(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<List<WateringRequirement>>.Failure("No watering requirement");
        }

        var requirements = new List<WateringRequirement>();
        foreach (var v in value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (Enum.TryParse<WateringRequirement>(v, ignoreCase: true, out var result))
            {
                requirements.Add(result);
            }
        }

        if (requirements.Count == 0)
        {
            return Result<List<WateringRequirement>>.Failure("No watering requirement");
        }

        return Result<List<WateringRequirement>>.Success(requirements);
    }

    public static Result<List<LightCondition>> ParseLightCondition(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<List<LightCondition>>.Failure("No light condition");
        }

        var conditions = new List<LightCondition>();

        foreach (var v in value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var token = v.ToLowerInvariant();
            LightCondition? mapped = token switch
            {
                "full sun" => LightCondition.High,
                "partial sun/shade" => LightCondition.Medium,
                "full shade" => LightCondition.Low,
                _ => null
            };

            if (mapped is not null)
            {
                conditions.Add(mapped.Value);
            }
        }

        if (conditions.Count == 0)
        {
            return Result<List<LightCondition>>.Failure("No light condition");
        }

        return Result<List<LightCondition>>.Success(conditions);
    }
}