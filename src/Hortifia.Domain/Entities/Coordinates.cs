using Hortifia.Domain.Common;

namespace Hortifia.Domain.Entities;

public class Coordinates
{
    public double Latitude { get; private set; }
    public double Longtitude { get; private set; }

    public static Result<Coordinates> Create(double latitude, double longtitude)
    {
        if (latitude < -90 || latitude > 90)
        {
            return Result<Coordinates>.Failure("Latitude must be between -90 and 90.");
        }

        if (longtitude < -180 || longtitude > 180)
        {
            return Result<Coordinates>.Failure("Longtitude must be between -180 and 180.");
        }

        return Result<Coordinates>.Success(new Coordinates
        {
            Latitude = latitude,
            Longtitude = longtitude
        });
    }
}
