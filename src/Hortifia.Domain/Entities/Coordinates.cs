using Hortifia.Domain.Common;

namespace Hortifia.Domain.Entities;

public class Coordinates
{
    public double Latitude { get; private set; }
    public double Longtitude { get; private set; }

    public static Result<Coordinates> Create(double latitude, double longtitude)
    {
        var validationResult = Validate(latitude, longtitude);
        if (!validationResult.IsSuccess)
        {
            return Result<Coordinates>.Failure(validationResult.ErrorMessage!);
        }

        return Result<Coordinates>.Success(new Coordinates
        {
            Latitude = latitude,
            Longtitude = longtitude
        });
    }

    public Result Update(double latitude, double longtitude)
    {
        var validationResult = Validate(latitude, longtitude);
        if (!validationResult.IsSuccess)
        {
            return Result.Failure(validationResult.ErrorMessage!);
        }

        Latitude = latitude;
        Longtitude = longtitude;

        return Result.Success();
    }

    private static Result Validate(double latitude, double longtitude)
    {
        if (latitude < -90 || latitude > 90)
        {
            return Result.Failure("Latitude must be between -90 and 90.");
        }

        if (longtitude < -180 || longtitude > 180)
        {
            return Result.Failure("Longtitude must be between -180 and 180.");
        }

        return Result.Success();
    }
}
