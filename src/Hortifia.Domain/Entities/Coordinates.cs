using Hortifia.Domain.Common;

namespace Hortifia.Domain.Entities;

public class Coordinates
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public static Result<Coordinates> Create(double latitude, double longitude)
    {
        var validationResult = Validate(latitude, longitude);
        if (!validationResult.IsSuccess)
        {
            return Result<Coordinates>.Failure(validationResult.ErrorMessage!);
        }

        return Result<Coordinates>.Success(new Coordinates
        {
            Latitude = latitude,
            Longitude = longitude
        });
    }

    public Result Update(double latitude, double longitude)
    {
        var validationResult = Validate(latitude, longitude);
        if (!validationResult.IsSuccess)
        {
            return Result.Failure(validationResult.ErrorMessage!);
        }

        Latitude = latitude;
        Longitude = longitude;

        return Result.Success();
    }

    private static Result Validate(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
        {
            return Result.Failure("Latitude must be between -90 and 90.");
        }

        if (longitude < -180 || longitude > 180)
        {
            return Result.Failure("Longitude must be between -180 and 180.");
        }

        return Result.Success();
    }
}
